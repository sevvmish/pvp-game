using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class effects : MonoBehaviour
{
        
    public int MyPlayerClass;

    private List<Conds> CurrentConds = new List<Conds>();

    public bool isStunned, isShieldSlam;

    public AudioSource MyAudioSourse;
    private AudioClip HitWith1HSword, ShieldSlamSound, SwingHuge, BuffSound, BloodLoss, CancelCastingEffinBar, CastingSpellSound;
    public GameObject BlockWithShield, WeaponTrail, StunEffect, ShieldSlam, ShieldSlamEff, CritSwordEff, ShieldChargeEff, BuffEff, BloodLossEff, ShieldOnEff;

    private Animator PlayerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        PlayerAnimator = this.gameObject.GetComponent<Animator>();
        StunEffect.SetActive(false);
        BloodLossEff.SetActive(false);

        SwingHuge = Resources.Load<AudioClip>("sounds/swing very huge");
        BuffSound = Resources.Load<AudioClip>("sounds/buff sound1");
        BloodLoss = Resources.Load<AudioClip>("sounds/blood loss");
        CancelCastingEffinBar = Resources.Load<AudioClip>("sounds/canceled spell sound");
        CastingSpellSound = Resources.Load<AudioClip>("sounds/casting spell");

        if (MyPlayerClass == 1)
        {
            HitWith1HSword = Resources.Load<AudioClip>("sounds/hit by weapon sword");
            ShieldSlamSound = Resources.Load<AudioClip>("sounds/shield slam");
            ShieldSlam.SetActive(false);
            BlockWithShield.SetActive(false);
            WeaponTrail.SetActive(false);
            ShieldSlamEff.SetActive(false);
            CritSwordEff.SetActive(false);
            ShieldChargeEff.SetActive(false);
            BuffEff.SetActive(false);            
            ShieldOnEff.SetActive(false);
        }
        
    }

    

    private void FixedUpdate()
    {

        if (isStunned)
        {
            if (!StunEffect.activeSelf)
            {
                StunEffect.SetActive(true);

            }
        } 
        else
        {
            if (StunEffect.activeSelf)
            {
                StunEffect.SetActive(false);
            }
        }

        if (MyPlayerClass == 1)
        {
            if (isShieldSlam)
            {
                if (!ShieldSlam.activeSelf)
                {
                    ShieldSlam.SetActive(true);
                    ShieldSlamEff.SetActive(true);
                    StartCoroutine(PlaySomeSound(SwingHuge, 0.2f, false));
                }
            }
            else
            {
                if (ShieldSlam.activeSelf)
                {
                    ShieldSlam.SetActive(false);
                    ShieldSlamEff.SetActive(false);
                }
            }
        }

    }

    public void RegisterConds(Conds SomeConds)
    {
        
        if (CurrentConds.Count > 0)
        {
            if (CurrentConds.Count > general.SessionNumberOfPlayers * 10)
            {
                CurrentConds.Remove(CurrentConds[0]);
            }

            bool isgood = true;
            for (int i = 0; i < CurrentConds.Count; i++)
            {
                if (CurrentConds[i].cond_id == SomeConds.cond_id)
                {
                    isgood = false;
                }
            }
            if (isgood)
            {

                CurrentConds.Add(SomeConds);
                WorkWithCond(CurrentConds[CurrentConds.Count - 1]);
            }
        }
        else
        {
            CurrentConds.Add(SomeConds);
            WorkWithCond(CurrentConds[CurrentConds.Count - 1]);
        }

        
       
    }

    private void WorkWithCond(Conds ConditionToProcess)
    {
        print(ConditionToProcess.cond_id + "-ID, " + ConditionToProcess.cond_bulk + " - all  ///" + ConditionToProcess.isChecked);

        if (!ConditionToProcess.isChecked)
        {
            switch (ConditionToProcess.cond_bulk)
            {

                case "me-b":

                    if (MyPlayerClass == 1) StartCoroutine(TurnOnSomeEffect(BlockWithShield, 0.5f));

                    break;
            }

            if (ConditionToProcess.cond_type=="dt" && ConditionToProcess.damage_or_heal>0 &&  DB.GetSpellByNumber(ConditionToProcess.spell_index).spell_type==spellsIDs.spell_types.direct_melee )
            {
                if (MyPlayerClass == 1)
                {
                    MyAudioSourse.clip = HitWith1HSword;
                    MyAudioSourse.Play();
                }
            }


            if (ConditionToProcess.cond_type == "dt" && ConditionToProcess.spell_index == 4)
            {
                StartCoroutine(PlaySomeSound(ShieldSlamSound, 0.2f, false));                          
            }

            if (ConditionToProcess.cond_type == "ca" && ConditionToProcess.cond_message != "CANCELED")
            {                
                StartCoroutine(PlaySomeSound(CastingSpellSound, 0, true));
            }

            if (ConditionToProcess.cond_type == "ca" && ConditionToProcess.spell_index==4)
            {
                StartCoroutine(TurnOnSomeEffect(ShieldChargeEff, ConditionToProcess.cond_time));
                
            }



            if (ConditionToProcess.cond_type == "dg" && ConditionToProcess.damage_or_heal > 0 && ConditionToProcess.isCrit)
            {
                if (MyPlayerClass == 1)
                {
                    StartCoroutine(TurnOnSomeEffect(CritSwordEff, 1f));
                }
            }


            if (ConditionToProcess.cond_type == "co" && ConditionToProcess.spell_index==3)
            {
                PlayerAnimator.Play("buff");
                StartCoroutine(TurnOnSomeEffect(BuffEff, 6f));
                StartCoroutine(PlaySomeSound(BuffSound, 0, false));
            }

            if (ConditionToProcess.cond_type == "co" && ConditionToProcess.spell_index == 5)
            {                
                StartCoroutine(TurnOnSomeEffect(ShieldOnEff, ConditionToProcess.cond_time));
                
            }

            if (ConditionToProcess.cond_type == "dt" && ConditionToProcess.spell_index == 2 && ConditionToProcess.damage_or_heal > 0)
            {
                StartCoroutine(TurnOnSomeEffect(BloodLossEff, 0.8f));
                StartCoroutine(PlaySomeSound(BloodLoss, 0, false));
            }



        }
    }


    public void CancelCasting()
    {
        if (MyAudioSourse.clip == CastingSpellSound)
        {
            
            MyAudioSourse.Stop();
            StartCoroutine(PlaySomeSound(CancelCastingEffinBar, 0, false));
        }

        
    }

    IEnumerator TurnOnSomeEffect(GameObject SomeEffect, float timer)
    {
        if (!SomeEffect.activeSelf)
        {
            SomeEffect.SetActive(true);
            
            yield return new WaitForSeconds(timer);
            SomeEffect.SetActive(false);
        }
    }

    IEnumerator PlaySomeSound(AudioClip AClip, float DelayTime, bool isLooping)
    {
        
        yield return new WaitForSeconds(DelayTime);
        MyAudioSourse.loop = isLooping;
        MyAudioSourse.clip = AClip;
        MyAudioSourse.Play();
    }


}
