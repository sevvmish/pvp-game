using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class effects : MonoBehaviour
{
    public SessionData PlayerSessionData;

    //private SessionData[] OtherPlayers = new SessionData[general.SessionNumberOfPlayers - 1];

    public int MyPlayerClass;
    private Transform VFXRespPlace;

    private List<Conds> CurrentConds = new List<Conds>();
    private List<Conds> CastingSpell = new List<Conds>();
    private List<Vector2> CastingCoords = new List<Vector2>();

    private List<GameObject> Cancels = new List<GameObject>();

    private ObjectPooling FireSteps;

    public bool isStunned, isShieldSlam, isCasting, isChanneling, isSpellShooting;

    public AudioSource MyAudioSourse;
    private AudioClip HitWith1HSword, ShieldSlamSound, SwingHuge, BuffSound, BloodLoss, CancelCastingEffinBar, CastingSpellSound;

    //common effects
    public GameObject StunEffect, BloodLossEff, ExplosionFireBall;

    //warr 1 effects
    public GameObject BlockWithShield, WeaponTrail, ShieldSlam, ShieldSlamEff, CritSwordEff, BuffEff, ShieldOnEff, ShieldChargeEff;

    //mage 1 effects
    public GameObject CastingEffFireHandL, CastingEffFireHandR, Fireball, Meteor, FireHandEff, FireStepEff;

    private Animator PlayerAnimator;    
    private Vector2 CastingPos;


    private Vector3 GetPlayerCoordsByOrderNumber(int OrderNumber)
    {
        Vector3 result = Vector3.zero;

        Transform source = VFXRespPlace;

        for (int i = 0; i < source.childCount; i++)
        {
            if (OrderNumber == source.GetChild(i).gameObject.GetComponent<effects>().PlayerSessionData.PlayerOrder)
            {
                return source.GetChild(i).position ;
            }
        }

        return result;

    }

    // Start is called before the first frame update
    void Start()
    {
        
        VFXRespPlace = GameObject.Find("OtherPlayers").transform;
        
        PlayerAnimator = this.gameObject.GetComponent<Animator>();
        StunEffect.SetActive(false);
        BloodLossEff.SetActive(false);
        ExplosionFireBall.SetActive(false);

        SwingHuge = Resources.Load<AudioClip>("sounds/swing very huge");
        BuffSound = Resources.Load<AudioClip>("sounds/buff sound1");
        BloodLoss = Resources.Load<AudioClip>("sounds/blood loss");
        HitWith1HSword = Resources.Load<AudioClip>("sounds/hit by weapon sword");
        ShieldSlamSound = Resources.Load<AudioClip>("sounds/shield slam");
        CancelCastingEffinBar = Resources.Load<AudioClip>("sounds/canceled spell sound");
        CastingSpellSound = Resources.Load<AudioClip>("sounds/casting spell");
        

        if (MyPlayerClass == 1)
        {
            
            ShieldSlam.SetActive(false);
            BlockWithShield.SetActive(false);
            WeaponTrail.SetActive(false);
            ShieldSlamEff.SetActive(false);
            CritSwordEff.SetActive(false);
            ShieldChargeEff.SetActive(false);
            BuffEff.SetActive(false);
            ShieldOnEff.SetActive(false);
        }
        if (MyPlayerClass == 2)
        {            
            CastingEffFireHandL.SetActive(false);
            CastingEffFireHandR.SetActive(false);
            Fireball.SetActive(false);
            Meteor.SetActive(false);
            FireHandEff.SetActive(false);
            FireSteps = new ObjectPooling(20, FireStepEff, VFXRespPlace);
            FireStepEff.SetActive(false);
        }

    }

    

    private void FixedUpdate()
    {

       
        if (Input.GetKeyDown(KeyCode.O))
        {
            print(GetPlayerCoordsByOrderNumber(1) + " - " + GetPlayerCoordsByOrderNumber(2));
        }

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

        
        
        if (isCasting)
        {
            
            if (!MyAudioSourse.isPlaying || (MyAudioSourse.isPlaying && MyAudioSourse.clip.name != "casting spell"))
            {
                StartCoroutine(PlaySomeSound(CastingSpellSound, 0, true));                
            }

            //casting effects MAGE 1
            if (MyPlayerClass == 2)
            {
                
                //if (CurrentConds[CurrentConds.Count-1].spell_index == 51)
                //{

                    if (!CastingEffFireHandL.activeSelf)
                    {
                        CastingEffFireHandL.SetActive(true);
                        CastingEffFireHandR.SetActive(true);
                    }
                //}
            }


        } else
        {
            //casting effects MAGE 1
            if (MyPlayerClass == 2)
            {
                if (CastingEffFireHandL.activeSelf)
                {
                    CastingEffFireHandL.SetActive(false);
                    CastingEffFireHandR.SetActive(false);
                }
            }

            if (MyAudioSourse.isPlaying && MyAudioSourse.clip.name == "casting spell")
            {
                StopSound();                
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

        if (!SomeConds.isChecked)
        {


            if (SomeConds.cond_type == "cs")
            {
                bool isOK = true;

                for (int i = 0; i < CurrentConds.Count; i++)
                {
                    if (CurrentConds[i].spell_index == SomeConds.spell_index)
                    {
                        CurrentConds[i].coord_x = SomeConds.coord_x;
                        CurrentConds[i].coord_z = SomeConds.coord_z;
                        isOK = false;
                    }
                }

                if (isOK)
                {
                    CurrentConds.Add(SomeConds);
                    int CurrCondIndex = CurrentConds.Count - 1;

                    switch (SomeConds.spell_index)
                    {
                        case 51:
                            StartCoroutine(SpellShooting51(CurrentConds[CurrCondIndex]));
                            break;
                        case 52:
                            StartCoroutine(SpellShooting52(CurrentConds[CurrCondIndex]));
                            break;
                        case 54:
                            StartCoroutine(SpellShooting54(CurrentConds[CurrCondIndex]));
                            break;
                    }

                }

            }



            switch (SomeConds.cond_bulk)
            {

                case "me-b":

                    if (MyPlayerClass == 1) StartCoroutine(TurnOnSomeEffect(BlockWithShield, 0.5f, 0));

                    break;
            }

            if (SomeConds.cond_type.Contains("dt") && SomeConds.damage_or_heal > 0)
            {

                if (DB.GetSpellByNumber(SomeConds.spell_index).spell_type == spellsIDs.spell_types.direct_melee)
                {
                    StartCoroutine(PlaySomeSound(HitWith1HSword, 0, false));

                }
                else if (DB.GetSpellByNumber(SomeConds.spell_index).spell_type == spellsIDs.spell_types.direct_magic)
                {

                    StartCoroutine(PlaySomeSound(HitWith1HSword, 0, false));
                    StartCoroutine(TurnOnSomeEffect(ExplosionFireBall, 1f, 0));
                }

            }


            if (SomeConds.cond_type == "dt" && SomeConds.spell_index == 4)
            {
                StartCoroutine(PlaySomeSound(ShieldSlamSound, 0.2f, false));
            }


            if (SomeConds.cond_type == "ca" && SomeConds.cond_message == "CANCELED")
            {
                
                CancelCasting();
            }



            //ONLY FOR WARRIOR====================================            
            if (SomeConds.cond_type == "ca" && SomeConds.spell_index == 4)
            {
                StartCoroutine(TurnOnSomeEffect(ShieldChargeEff, SomeConds.cond_time, 0));

            }


            if (SomeConds.cond_type == "co" && SomeConds.spell_index == 3)
            {
                PlayerAnimator.Play("buff");
                StartCoroutine(TurnOnSomeEffect(BuffEff, 6f, 0));
                StartCoroutine(PlaySomeSound(BuffSound, 0, false));
            }

            if (SomeConds.cond_type == "co" && SomeConds.spell_index == 5)
            {
                StartCoroutine(TurnOnSomeEffect(ShieldOnEff, SomeConds.cond_time, 0));

            }

                                
            if (SomeConds.cond_type == "dt" && SomeConds.spell_index == 2 && SomeConds.damage_or_heal > 0)
            {
                StartCoroutine(TurnOnSomeEffect(BloodLossEff, 0.8f, 0));
                StartCoroutine(PlaySomeSound(BloodLoss, 0, false));
            }
            //=====================================================



            
            //MMMMMMMMAAAAAAAAAGGGGGGGGGGGGEEEEEEEEE
           
            if (SomeConds.cond_type == "co" && SomeConds.spell_index == 53)
            {
                StartCoroutine(TurnOnSomeEffect(FireHandEff, 2.5f, 0.5f));
                StartCoroutine(TurnOnSomeEffect(CastingEffFireHandR, 2.5f, 0));                    
            }
            if (SomeConds.cond_type == "ca" && MyPlayerClass == 2 && SomeConds.cond_message == "CANCELED" && (FireHandEff.activeSelf || CastingEffFireHandR.activeSelf))
            {                
                StartCoroutine(TurnOFFSomeEffect(FireHandEff, 0.61f));
                StartCoroutine(TurnOFFSomeEffect(CastingEffFireHandR, 0.05f));
                
            }




           
            //=====================================================




        }


    }

  

    public void StopSound()
    {
        MyAudioSourse.Stop();
        MyAudioSourse.loop = false;
    }

    public void CancelCasting()
    {
        if (MyAudioSourse.clip == CastingSpellSound)
        {            
            MyAudioSourse.Stop();
            StartCoroutine(PlaySomeSound(CancelCastingEffinBar, 0, false));
        }        
    }

    IEnumerator TurnOFFSomeEffect(GameObject SomeEffect, float CleanTime)
    {
        Cancels.Add(SomeEffect);
        SomeEffect.SetActive(false);
        yield return new WaitForSeconds(CleanTime);
        Cancels.Remove(SomeEffect);
    }


    IEnumerator TurnOnSomeEffect(GameObject SomeEffect, float HowLongTimer, float DelayBeforStart)
    {
        yield return new WaitForSeconds(DelayBeforStart);

        if (!SomeEffect.activeSelf)
        {
            SomeEffect.SetActive(true);

            //yield return new WaitForSeconds(HowLongTimer);
            float delta = HowLongTimer / 0.04f;
            for (float i=0; i< HowLongTimer; i+=0.04f)
            {
                
                if (Cancels.Contains(SomeEffect))
                {
                    
                    break;
                }

                yield return new WaitForSeconds(0.04f);
            }
            
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


    IEnumerator SpellShooting51(Conds CurrConditions)
    {
        GameObject SpellSource = Instantiate(Fireball, Vector3.zero, Quaternion.identity, VFXRespPlace);
        SpellSource.SetActive(true);
        for (float i=0; i<2; i+=0.04f)
        {            
            SpellSource.transform.position = new Vector3(CurrConditions.coord_x, 1, CurrConditions.coord_z);
            //print(isSpellShooting + "=============================");
            yield return new WaitForSeconds(0.04f);
        }
        while (isSpellShooting);
        SpellSource.SetActive(false);
        Destroy(SpellSource);
        CurrentConds.Remove(CurrConditions);
    }

    IEnumerator SpellShooting52(Conds CurrConditions)
    {
        
        GameObject SpellSource = Instantiate(Meteor, Vector3.zero, Quaternion.identity, VFXRespPlace);
        SpellSource.SetActive(true);
        SpellSource.transform.position = new Vector3(CurrConditions.coord_x, 0, CurrConditions.coord_z);
        StartCoroutine(TurnOnSomeEffect(SpellSource.transform.GetChild(0).gameObject, 1.5f, 1.5f));
        isSpellShooting = false;
        yield return new WaitForSeconds(0.1f);
        CurrentConds.Remove(CurrConditions);
        
        yield return new WaitForSeconds(7f);       
        SpellSource.SetActive(false);
        Destroy(SpellSource);
        
    }

    IEnumerator SpellShooting54(Conds CurrConditions)
    {

        GameObject SpellSource = FireSteps.GetObject();
        SpellSource.transform.position = new Vector3(CurrConditions.coord_x, 0, CurrConditions.coord_z);
        isSpellShooting = false;
        yield return new WaitForSeconds(0.1f);
        CurrentConds.Remove(CurrConditions);
        yield return new WaitForSeconds(3f);
        SpellSource.SetActive(false);

    }


    public IEnumerator SpellShooting(Conds CastingConds)
    {

        print(CastingConds.coord_x + " - " + CastingConds.coord_z);

        yield return new WaitForSeconds(0.04f);


    }



}
