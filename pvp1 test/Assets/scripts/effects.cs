using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class effects : MonoBehaviour
{
    private List<Conds> CurrentConds = new List<Conds>();
    
    public AudioSource MyAudioSourse;
    private AudioClip HitWith1HSword;
    public GameObject BlockWithShield, WeaponTrail, StunEffect, ShieldSlam, ShieldSlamEff, CritSwordEff;

    private Animator PlayerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        PlayerAnimator = this.gameObject.GetComponent<Animator>();
        StunEffect.SetActive(false);
        HitWith1HSword = Resources.Load<AudioClip>("sounds/hit by weapon sword");
        ShieldSlam.SetActive(false);
        BlockWithShield.SetActive(false);
        WeaponTrail.SetActive(false);
        ShieldSlamEff.SetActive(false);
        CritSwordEff.SetActive(false);

    }

    private void FixedUpdate()
    {
        if (PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("stunned"))
        {
            if (!StunEffect.activeSelf)
            {
                StunEffect.SetActive(true);
                
            }
        } 
        else if (!PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("stunned"))
        {
            if (StunEffect.activeSelf)
            {
                StunEffect.SetActive(false);
            }
        }

        if (PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("shield slam"))
        {
            if (!ShieldSlam.activeSelf)
            {
                ShieldSlam.SetActive(true);
                ShieldSlamEff.SetActive(true);
            }
        }
        else if (!PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("shield slam"))
        {
            if (ShieldSlam.activeSelf)
            {
                ShieldSlam.SetActive(false);
                ShieldSlamEff.SetActive(false);
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
                    StartCoroutine(TurnOnSomeEffect(BlockWithShield, 0.5f));

                    break;
            }

            if (ConditionToProcess.cond_type=="dt" && ConditionToProcess.damage_or_heal>0 &&  DB.GetSpellByNumber(ConditionToProcess.spell_index).spell_type==spellsIDs.spell_types.direct_melee )
            {
                if (general.MainPlayerClass==1)
                {
                    MyAudioSourse.clip = HitWith1HSword;
                    MyAudioSourse.Play();
                }
            }

            if (ConditionToProcess.cond_type == "dg" && ConditionToProcess.damage_or_heal > 0 && ConditionToProcess.isCrit)
            {
                if (general.MainPlayerClass == 1)
                {
                    StartCoroutine(TurnOnSomeEffect(CritSwordEff, 1f));
                }
            }



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


}
