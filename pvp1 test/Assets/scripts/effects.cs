using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class effects : MonoBehaviour
{
    //public SessionData PlayerSessionData;
    public int PlayerSessionDataOrder;

    //private SessionData[] OtherPlayers = new SessionData[general.SessionNumberOfPlayers - 1];

    public int MyPlayerClass;
    private Transform VFXRespPlace;

    private List<Conds> CurrentConds = new List<Conds>();
    private List<Conds> CastingSpell = new List<Conds>();
    private List<Vector2> CastingCoords = new List<Vector2>();
    public static HashSet<string> IDAllreadyUsed = new HashSet<string>();
    private List<GameObject> Cancels = new List<GameObject>();

    private bool isCheckingFrozenSpilesSpell58;

    public bool isStunned, isShieldSlam, isCasting, isChanneling, isSpellShooting;

    public AudioSource MyAudioSourse;
    private AudioClip HitWith1HSword, ShieldSlamSound, SwingHuge, BuffSound, BloodLoss, CancelCastingEffinBar, CastingSpellSound;

    //common effects
    public GameObject StunEffect, BloodLossEff, ExplosionFireBall, FrozenSpikes, FrozenSpikesBroken, StrafeEff, StunScreenEffect;

    //warr 1 effects
    public GameObject BlockWithShield, WeaponTrail, ShieldSlam, ShieldSlamEff, CritSwordEff, BuffEff, ShieldOnEff, ShieldChargeEff, RocksEff;

    //mage 1 effects
    public GameObject CastingEffFireHandL, CastingEffFireHandR, Fireball, Meteor, FireHandEff, FireStepEff, IceNova;
    private ObjectPooling FireSteps;
    

    //barbarian 1 effects
    public GameObject SplashEffSimpleHit;
    public GameObject WhirlWind;
    public GameObject SlamPlace;

    //rogue effects
    public GameObject MainMesh;
    public GameObject SkeletonMesh;
    public GameObject FogEffect;
    public GameObject UnFogEffect;
    public SkinnedMeshRenderer RogSkin;
    public Material NormalRogue;
    public Material InvisibleRogue;
    public GameObject FogWith;
    public GameObject BackStabEffect;
    public GameObject PistolOnBelt;
    public GameObject PistolRightHand;
    public GameObject DaggerRightHand;
    public GameObject BulletTrail;
    public GameObject ButcheryEff;
    public GameObject SmokePuff;
    public GameObject Fuseeff;
    public GameObject CheckForRogueInvis;

    //wizard 1 effects
    private ObjectPooling DeathBeamsList;
    public GameObject DeathBeam;
    public GameObject ChargeDeathBeam;
    public GameObject BlackHoleEff;
    public GameObject AutoHealShield;

    private Animator PlayerAnimator;    
    private Vector2 CastingPos;



    private Vector3 GetPlayerCoordsByOrderNumber(int OrderNumber)
    {
        Vector3 result = Vector3.zero;

        Transform source = VFXRespPlace;

        for (int i = 0; i < source.childCount; i++)
        {
            if (OrderNumber == PlayerSessionDataOrder)
            {
                return source.GetChild(i).position;
            }
        }

        return result;

    }

    // Start is called before the first frame update
    void Start()
    {        
        VFXRespPlace = GameObject.Find("OtherPlayers").transform;        
        PlayerAnimator = this.gameObject.GetComponent<Animator>();

        IDAllreadyUsed.Clear();

        if (PlayerSessionDataOrder == 0)
        {
            StunScreenEffect = Instantiate(Resources.Load<GameObject>("prefabs/StunScreenEff"), Vector3.zero, Quaternion.identity, GameObject.Find("Canvas").transform);
            StunScreenEffect.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            StunScreenEffect.SetActive(false);
        }

        StunEffect.SetActive(false);
        BloodLossEff.SetActive(false);
        ExplosionFireBall.SetActive(false);
        FrozenSpikes.SetActive(false);
        FrozenSpikesBroken.SetActive(false);
        StrafeEff.SetActive(false);

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
            RocksEff.SetActive(false);
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
            IceNova.SetActive(false);

                       
        }

        if (MyPlayerClass == 3)
        {
            SplashEffSimpleHit.SetActive(false);
            WhirlWind.SetActive(false);
            SlamPlace.SetActive(false);
        }

        if (MyPlayerClass == 4)
        {
            FogEffect.SetActive(false);
            UnFogEffect.SetActive(false);
            SkeletonMesh.SetActive(true);
            MainMesh.SetActive(true);
            FogWith.SetActive(false);
            BackStabEffect.SetActive(false);
            PistolOnBelt.SetActive(true);
            PistolRightHand.SetActive(false);
            DaggerRightHand.SetActive(true);
            ButcheryEff.SetActive(false);
            BulletTrail.SetActive(false);
            SmokePuff.SetActive(false);
            Fuseeff.SetActive(false);
            CheckForRogueInvis.SetActive(false);
        }

        if (MyPlayerClass == 5)
        {
            //DeathBeamsList = new ObjectPooling(40, DeathBeam, VFXRespPlace);
            DeathBeam.SetActive(false);
            ChargeDeathBeam.SetActive(false);
            BlackHoleEff.SetActive(false);
            AutoHealShield.SetActive(false);
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
                StunScreenEffect.SetActive(true);
            }
        } 
        else
        {
            if (StunEffect.activeSelf)
            {
                StunEffect.SetActive(false);
                StunScreenEffect.SetActive(false);
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
               
                if (!CastingEffFireHandL.activeSelf)
                {
                    CastingEffFireHandL.SetActive(true);
                    CastingEffFireHandR.SetActive(true);
                }
               
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
       

        //======================================================================================================================
        if (1==1) //!SomeConds.isChecked
        {
            /*
            if (SomeConds.cond_type == "cs")
            {
                bool isOK = true;

                for (int i = 0; i < CurrentConds.Count; i++)
                {
                    if (CurrentConds[i].spell_index == SomeConds.spell_index && CurrentConds[i].cond_id == SomeConds.cond_id)
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
                        case 55:
                            StartCoroutine(SpellShooting55(CurrentConds[CurrCondIndex]));
                            break;

                        case 103:
                            StartCoroutine(SpellShooting103(CurrentConds[CurrCondIndex]));
                            break;


                        case 153:                            
                            StartCoroutine(SpellShooting153(CurrentConds[CurrCondIndex]));
                            break;
                        case 156:
                            StartCoroutine(SpellShooting156(CurrentConds[CurrCondIndex]));
                            break;


                        
                        case 202:
                            StartCoroutine(SpellShooting202(CurrentConds[CurrCondIndex]));
                            break;
                    }

                }

            }
            */

            //===========================
            if (SomeConds.cond_type=="co" && !SomeConds.isChecked)
            {
                if (SomeConds.spell_index==3)
                {
                    StartCoroutine(TurnOnSomeEffect(BuffEff, 6f, 0));
                    StartCoroutine(PlaySomeSound(BuffSound, 0, false));
                }
            }
            //===========================
            if (SomeConds.cond_type == "dt" && !SomeConds.isChecked)
            {
                if ((SomeConds.spell_index == 2 || SomeConds.spell_index == 7 || SomeConds.spell_index == 8) && SomeConds.damage_or_heal > 0)
                {
                    //StartCoroutine(TurnOnSomeEffect(BloodLossEff, 0.8f, 0));
                    StartCoroutine(StandartSelfEffect(SomeConds, new List<GameObject>() { BloodLossEff}, 0, 0.8f, this.gameObject.transform));
                    
                }

                if (SomeConds.spell_index == 4 || SomeConds.spell_index == 9) 
                {
                    StartCoroutine(PlaySomeSound(ShieldSlamSound, 0.2f, false));
                }

                if (SomeConds.spell_index == 51 || SomeConds.spell_index == 53)
                {
                    //StartCoroutine(TurnOnSomeEffect(ExplosionFireBall, 1f, 0));
                    StartCoroutine(StandartSelfEffect(SomeConds, new List<GameObject>() { ExplosionFireBall }, 0, 1f, this.gameObject.transform));
                }

                if (!IDAllreadyUsed.Contains(SomeConds.cond_id)) StartCoroutine(ToDelete(SomeConds, 1f));
            }



            //conditions and casting
            if (!IDAllreadyUsed.Contains(SomeConds.cond_id)) // && ((SomeConds.cond_time>0.2f && SomeConds.cond_type=="co") || (SomeConds.cond_type == "ca"))
            {               
                IDAllreadyUsed.Add(SomeConds.cond_id);

                //=============================
                switch (SomeConds.spell_index)
                {                    
                    case 4:                        
                        if (SomeConds.cond_type == "ca") StartCoroutine(TurnOnSomeEffect(ShieldChargeEff, SomeConds.cond_time, 0));
                        break;

                    case 5:
                        if (SomeConds.cond_type == "co") StartCoroutine(Condition_5(SomeConds));
                        break;

                    case 9:
                        if (SomeConds.cond_type == "co") StartCoroutine(TurnOnSomeEffect(RocksEff, 3f, 0));
                        break;

                    case 51:
                        if (SomeConds.cond_type=="cs") StartCoroutine(SpellShooting51(SomeConds));
                        break;

                    case 52:
                        if (SomeConds.cond_type == "cs") StartCoroutine(SpellShooting52(SomeConds));
                        break;

                    case 53:
                        if (SomeConds.cond_type == "co") StartCoroutine(Condition_53(SomeConds));
                        break;

                    case 54:
                        if (SomeConds.cond_type == "cs") StartCoroutine(SpellShooting54(SomeConds));
                        break;

                    case 55:
                        if (SomeConds.cond_type == "cs") StartCoroutine(SpellShooting55(SomeConds));
                        break;

                    case 58:
                        if (SomeConds.cond_type == "co") StartCoroutine(Condition_58(SomeConds));
                        break;

                    case 201:
                        if (SomeConds.cond_type == "ca") StartCoroutine(StandartCasting(SomeConds, new List<GameObject>() { ChargeDeathBeam }));
                       
                        if (SomeConds.cond_type == "co") StartCoroutine(StandartCasting(SomeConds, new List<GameObject>() { DeathBeam }));
                        break;

                    case 202:
                        if (SomeConds.cond_type == "cs") StartCoroutine(SpellShooting202(SomeConds));
                        break;

                    case 997:
                        if (SomeConds.cond_type == "co") StartCoroutine(TurnOnSomeEffect(StrafeEff, 0.5f, 0));
                        IDAllreadyUsed.Remove(SomeConds.cond_id);
                        SomeConds.isToDelete = true;
                        break;

                }


                //======================================
                if (SomeConds.cond_bulk=="me-b")
                {
                    if (MyPlayerClass == 1) StartCoroutine(TurnOnSomeEffect(BlockWithShield, 0.5f, 0));
                    IDAllreadyUsed.Remove(SomeConds.cond_id);
                }

                //======================================


            }



            /*
            
            if (SomeConds.cond_type.Contains("dt") && SomeConds.damage_or_heal > 0)
            {

                if (DB.GetSpellByNumber(SomeConds.spell_index).spell_type == spellsIDs.spell_types.direct_melee)
                {
                    StartCoroutine(PlaySomeSound(HitWith1HSword, 0, false));
                    SomeConds.isToDelete = true;
                }
                else if (DB.GetSpellByNumber(SomeConds.spell_index).spell_type == spellsIDs.spell_types.direct_magic)
                {

                    StartCoroutine(PlaySomeSound(HitWith1HSword, 0, false));
                    StartCoroutine(TurnOnSomeEffect(ExplosionFireBall, 1f, 0));
                    SomeConds.isToDelete = true;
                }

            }

            


            if (SomeConds.cond_type == "ca" && SomeConds.cond_message == "CANCELED")
            {
                
                CancelCasting();
            }





            //BARBARIAN==========================
            if (SomeConds.cond_type == "dg" && SomeConds.spell_index == 101)
            {
                StartCoroutine(TurnOnSomeEffect(SplashEffSimpleHit, 1f, 0));                
            }


            if (SomeConds.cond_type == "co" && SomeConds.spell_index == 102)
            {
                StartCoroutine(TurnOnSomeEffect(WhirlWind, 3f, 0));
            }

            if (SomeConds.cond_message == "CANCELED" && SomeConds.spell_index==102 && WhirlWind.activeSelf)
            {
                StartCoroutine(TurnOFFSomeEffect(WhirlWind, 0));
            }

            //=============================

            //ROGUE==========================
            if (SomeConds.cond_type == "co" && SomeConds.spell_index == 153)
            {
                                
                if (PlayerSessionDataOrder != 0)
                {
                    StartCoroutine(TurnOffAfterDelay(SkeletonMesh, 0.1f));
                    StartCoroutine(TurnOffAfterDelay(MainMesh, 0.1f));
                } else
                {
                    RogSkin.material = InvisibleRogue;
                    FogWith.SetActive(true);
                }
            }

            if (SomeConds.cond_type == "co" && SomeConds.spell_index == 156)
            {
                PistolOnBelt.SetActive(false);
                Fuseeff.SetActive(true);
                PistolRightHand.SetActive(true);
                DaggerRightHand.SetActive(false);                
            }
            if (SomeConds.cond_message == "CANCELED" && SomeConds.spell_index == 156 && PistolRightHand.activeSelf)
            {
                StartCoroutine(TurnOnAfterDelay(PistolOnBelt, 0));
                Fuseeff.SetActive(false);
                StartCoroutine(TurnOffAfterDelay(PistolRightHand, 0));
                StartCoroutine(TurnOnAfterDelay(DaggerRightHand, 0));
            }



            if (SomeConds.cond_type == "co" && SomeConds.spell_index == 154)
            {
                StartCoroutine(TurnOnSomeEffect(ButcheryEff, 1.2f, 0));
            }
            if (SomeConds.cond_message == "CANCELED" && SomeConds.spell_index == 154 && ButcheryEff.activeSelf)
            {
                StartCoroutine(TurnOFFSomeEffect(ButcheryEff, 0));
            }


            if (SomeConds.cond_type == "co" && SomeConds.spell_index == 157)
            {
                StartCoroutine(TurnOnSomeEffect(UnFogEffect, 2, 0.5f));
                if (PlayerSessionDataOrder != 0)
                {
                    StartCoroutine(TurnOnAfterDelay(MainMesh, 1));
                    StartCoroutine(TurnOnAfterDelay(SkeletonMesh, 1));
                } else
                {
                    StartCoroutine(StartVisible());
                }
            }

            if (SomeConds.cond_type == "dg" && SomeConds.spell_index == 152)
            {
                StartCoroutine(TurnOnSomeEffect(BackStabEffect, 1f, 0));
            }

            if (SomeConds.cond_type == "ad" && SomeConds.spell_index == 153)
            {
                StartCoroutine(ShowInvis153(SomeConds));
            }
            //=================================

            //WIZARD===================================
            
            if (SomeConds.cond_type == "ca" && SomeConds.spell_index == 201)
            {
                
                ChargeDeathBeam.SetActive(true);
            } 
            
            if (SomeConds.cond_message == "CANCELED" && SomeConds.spell_index == 201 && ChargeDeathBeam.activeSelf)
            {
                ChargeDeathBeam.SetActive(false);
            }

            
            if (SomeConds.cond_type == "co" && SomeConds.spell_index == 201)
            {
                
                StartCoroutine(TurnOnSomeEffect(DeathBeam, SomeConds.cond_time, 0));
                ChargeDeathBeam.SetActive(false);
            }
            

            if (SomeConds.cond_message == "CANCELED" && SomeConds.spell_index == 201 && DeathBeam.activeSelf ) //
            {
                //DeathBeam.SetActive(false);
                StartCoroutine(TurnOFFSomeEffect(DeathBeam, 0));
                PlayerAnimator.Play("Idle");
                PlayerAnimator.Play("Idle1");
            }
            

            if (SomeConds.cond_type == "co" && SomeConds.spell_index == 204)
            {                
                StartCoroutine(TurnOnSomeEffect(AutoHealShield, SomeConds.cond_time, 0));                
            }

            //=========================================
            */
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

    IEnumerator TurnOnAfterDelay(GameObject SomeObject, float DelayBeforStart)
    {
        yield return new WaitForSeconds(DelayBeforStart);

        if (!SomeObject.activeSelf)
        {
            SomeObject.SetActive(true);                       
        }
    }

    IEnumerator TurnOffAfterDelay(GameObject SomeObject, float DelayBeforStart)
    {
        yield return new WaitForSeconds(DelayBeforStart);

        if (SomeObject.activeSelf)
        {
            SomeObject.SetActive(false);
        }
    }

    IEnumerator StunScreenWholeEff(float TimeForStun)
    {
        StunScreenEffect.SetActive(true);

        for (float i = 0; i <= 10f; i++)
        {
            StunScreenEffect.gameObject.GetComponent<RectTransform>().localScale = Vector3.Lerp(new Vector3(2.5f, 2.5f, 1), new Vector3(1, 1, 1), (i / 10f));
            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(TimeForStun - 1f);

        for (float i = 0; i <= 10f; i++)
        {
            StunScreenEffect.gameObject.GetComponent<RectTransform>().localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(2.5f, 2.5f, 1), (i / 10f));
            yield return new WaitForSeconds(0.03f);
        }

        StunScreenEffect.SetActive(false);
    }

    IEnumerator StunScreenFastEnd()
    {
        
        for (float i = 0; i <= 10f; i++)
        {
            StunScreenEffect.gameObject.GetComponent<RectTransform>().localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(2.5f, 2.5f, 2), (i / 10f));
            yield return new WaitForSeconds(0.03f);
        }

        StunScreenEffect.SetActive(false);
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

        for (float i=0; i<2; i+=0.03f)
        {
            if (CurrConditions.coord_x==999 && CurrConditions.coord_z==999)
            {
                break;
            }
            SpellSource.transform.position = new Vector3(CurrConditions.coord_x, 1, CurrConditions.coord_z);            
            yield return new WaitForSeconds(0.03f);
            
                         
        }
        yield return new WaitForSeconds(0.4f);
        SpellSource.SetActive(false);
        Destroy(SpellSource);
        CurrConditions.isToDelete = true;
        IDAllreadyUsed.Remove(CurrConditions.cond_id);
        //CurrentConds.Remove(CurrConditions);

    }

    IEnumerator SpellShooting52(Conds CurrConditions)
    {
        
        GameObject SpellSource = Instantiate(Meteor, Vector3.zero, Quaternion.identity, VFXRespPlace);
        SpellSource.SetActive(true);
        SpellSource.transform.position = new Vector3(CurrConditions.coord_x, 0, CurrConditions.coord_z);
        StartCoroutine(TurnOnSomeEffect(SpellSource.transform.GetChild(0).gameObject, 1.5f, 1.5f));
        
        yield return new WaitForSeconds(0.1f);
        
        
        yield return new WaitForSeconds(7f);       
        SpellSource.SetActive(false);
        Destroy(SpellSource);

        CurrConditions.isToDelete = true;
        IDAllreadyUsed.Remove(CurrConditions.cond_id);
    }

    IEnumerator SpellShooting54(Conds CurrConditions)
    {

        GameObject SpellSource = FireSteps.GetObject();
        SpellSource.transform.position = new Vector3(CurrConditions.coord_x, 0, CurrConditions.coord_z);
        
        yield return new WaitForSeconds(0.1f);
        
        yield return new WaitForSeconds(1f);
        SpellSource.SetActive(false);

        CurrConditions.isToDelete = true;
        IDAllreadyUsed.Remove(CurrConditions.cond_id);
    }

    IEnumerator SpellShooting55(Conds CurrConditions)
    {
        GameObject SpellSource = Instantiate(IceNova, Vector3.zero, Quaternion.identity, VFXRespPlace);
        SpellSource.transform.position = new Vector3(CurrConditions.coord_x, 0.05f, CurrConditions.coord_z);
        SpellSource.SetActive(true);
        
        
        yield return new WaitForSeconds(4f);        
        SpellSource.SetActive(false);
        Destroy(SpellSource);

        yield return new WaitForSeconds(1f);
        CurrConditions.isToDelete = true;
        IDAllreadyUsed.Remove(CurrConditions.cond_id);
    }

    IEnumerator SpellShooting103(Conds CurrConditions)
    {
        GameObject SpellSource = Instantiate(SlamPlace, Vector3.zero, Quaternion.identity, VFXRespPlace);
        SpellSource.transform.localEulerAngles = new Vector3(-90, 0, 0);
        SpellSource.transform.position = new Vector3(CurrConditions.coord_x, 0.02f, CurrConditions.coord_z);
        SpellSource.SetActive(true);        
        yield return new WaitForSeconds(1f);        
        Destroy(SpellSource);
    }

    IEnumerator SpellShooting153(Conds CurrConditions)
    {
        
        GameObject SpellSource = Instantiate(FogEffect, Vector3.zero, Quaternion.identity, VFXRespPlace);
        SpellSource.transform.position = new Vector3(CurrConditions.coord_x, 0, CurrConditions.coord_z);
        SpellSource.transform.localEulerAngles = new Vector3(-90,0,0);
        SpellSource.SetActive(true);

        yield return new WaitForSeconds(2f);
        Destroy(SpellSource);
    }

    IEnumerator SpellShooting156(Conds CurrConditions)
    {
        GameObject SpellSource = Instantiate(BulletTrail, new Vector3(this.gameObject.transform.position.x, 1.3f, this.gameObject.transform.position.z), Quaternion.identity, VFXRespPlace);        
        SpellSource.SetActive(true);
        SmokePuff.SetActive(true);
        
        yield return new WaitForSeconds(0.05f);
        SpellSource.transform.position = Vector3.Lerp(new Vector3(this.gameObject.transform.position.x, 1.3f, this.gameObject.transform.position.z), new Vector3(CurrConditions.coord_x, 1.3f, CurrConditions.coord_z), 0.5f);
        yield return new WaitForSeconds(0.05f);
        SpellSource.transform.position = Vector3.Lerp(new Vector3(this.gameObject.transform.position.x, 1.3f, this.gameObject.transform.position.z), new Vector3(CurrConditions.coord_x, 1.3f, CurrConditions.coord_z), 1);
        Fuseeff.SetActive(false);

        StartCoroutine(TurnOnAfterDelay(PistolOnBelt, 1f));
        StartCoroutine(TurnOffAfterDelay(PistolRightHand, 1f));
        StartCoroutine(TurnOnAfterDelay(DaggerRightHand, 1f));

        yield return new WaitForSeconds(1.2f);
        SmokePuff.SetActive(false);        
        Destroy(SpellSource);
    }

    IEnumerator StartVisible()
    {
        yield return new WaitForSeconds(1f);
        RogSkin.material = NormalRogue;
        FogWith.SetActive(false);
    }



    IEnumerator SpellShooting202(Conds CurrConditions)
    {
        GameObject SpellSource = Instantiate(BlackHoleEff, Vector3.zero, Quaternion.identity, VFXRespPlace);
        SpellSource.transform.position = new Vector3(CurrConditions.coord_x, 0.1f, CurrConditions.coord_z);
        SpellSource.SetActive(true);

        do
        {
            if (CurrConditions.coord_x==999 && CurrConditions.coord_z == 999)
            {
                break;
            }
            SpellSource.transform.position = new Vector3(CurrConditions.coord_x, 0.1f, CurrConditions.coord_z);
            yield return new WaitForSeconds(0.1f);
        } while (CurrConditions.cond_time>0 || CurrConditions.cond_message!="CANCELED");
                
        SpellSource.SetActive(false);
        Destroy(SpellSource);

        CurrConditions.isToDelete = true;
        IDAllreadyUsed.Remove(CurrConditions.cond_id);
    }

    IEnumerator ShowInvis153(Conds CurrConditions)
    {
        GameObject SpellSource = Instantiate(CheckForRogueInvis, Vector3.zero, Quaternion.identity, VFXRespPlace);
        float coord_x = float.Parse(CurrConditions.additional_data[0], CultureInfo.InvariantCulture);
        float coord_z = float.Parse(CurrConditions.additional_data[1], CultureInfo.InvariantCulture);
        float rot_y = float.Parse(CurrConditions.additional_data[2], CultureInfo.InvariantCulture);
        SpellSource.transform.position = new Vector3(coord_x, 0.1f, coord_z);
        SpellSource.transform.localEulerAngles = new Vector3(0, rot_y, 0);
        SpellSource.SetActive(true);
        
        //yield return new WaitForSeconds(0.1f);
        CurrentConds.Remove(CurrConditions);
        yield return new WaitForSeconds(2f);
        SpellSource.SetActive(false);
        Destroy(SpellSource);
    }


    public IEnumerator SpellShooting(Conds CastingConds)
    {

        print(CastingConds.coord_x + " - " + CastingConds.coord_z);

        yield return new WaitForSeconds(0.04f);


    }

    public IEnumerator ToDelete(Conds _curr_conds, float _after_sec)
    {
        IDAllreadyUsed.Add(_curr_conds.cond_id);
        yield return new WaitForSeconds(_after_sec);
        _curr_conds.isToDelete = true;
        IDAllreadyUsed.Remove(_curr_conds.cond_id);
    }


    //FREEZED spell 58
    public IEnumerator Condition_58(Conds _curr_conds) 
    {
        GameObject SpellSource = Instantiate(FrozenSpikes, Vector3.zero, Quaternion.identity, VFXRespPlace);
        SpellSource.transform.position = FrozenSpikes.transform.position;
        SpellSource.SetActive(true);
        //FrozenSpikes.SetActive(true);

        do
        {
            //if (!FrozenSpikes.activeSelf) FrozenSpikes.SetActive(true);
            if (!SpellSource.activeSelf) SpellSource.SetActive(true);
            yield return new WaitForSeconds(0.1f);

        } while (_curr_conds.cond_time > 0 || _curr_conds.cond_message == "CANCELED");

        //FrozenSpikes.SetActive(false);
        SpellSource.SetActive(false);

        //FrozenSpikesBroken.SetActive(true);
        GameObject SpellSource1 = Instantiate(FrozenSpikesBroken, Vector3.zero, Quaternion.identity, VFXRespPlace);
        SpellSource1.transform.position = FrozenSpikesBroken.transform.position;
        SpellSource1.SetActive(true);

        yield return new WaitForSeconds(2f);
        //FrozenSpikesBroken.SetActive(false);
        SpellSource1.SetActive(false);

        Destroy(SpellSource);
        Destroy(SpellSource1);

        _curr_conds.isToDelete = true;
        IDAllreadyUsed.Remove(_curr_conds.cond_id);
    }

    //STUN effect
    public IEnumerator Condition_1002(Conds _curr_conds)
    {
        if (_curr_conds.cond_time<0.5f || PlayerSessionDataOrder != 0)
        {
            yield return null;
        }

        //start stun
        StunScreenEffect.SetActive(true);
        for (float i = 0; i <= 10f; i++)
        {
            StunScreenEffect.gameObject.GetComponent<RectTransform>().localScale = Vector3.Lerp(new Vector3(2.5f, 2.5f, 1), new Vector3(1, 1, 1), (i / 10f));
            yield return new WaitForSeconds(0.03f);
        }

        do
        {            
            if (!StunScreenEffect.activeSelf) StunScreenEffect.SetActive(true);
            yield return new WaitForSeconds(0.2f);

        } while (_curr_conds.cond_time > 0 || _curr_conds.cond_message == "CANCELED");

        //end stun
        for (float i = 0; i <= 10f; i++)
        {
            StunScreenEffect.gameObject.GetComponent<RectTransform>().localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(2.5f, 2.5f, 1), (i / 10f));
            yield return new WaitForSeconds(0.03f);
        }
        StunScreenEffect.SetActive(false);

        yield return new WaitForSeconds(0.1f);
    }


    //SHIELD ON spell 5
    public IEnumerator Condition_5(Conds _curr_conds)
    {
        ShieldOnEff.SetActive(true);

        do
        {
            if (!ShieldOnEff.activeSelf) ShieldOnEff.SetActive(true);
            yield return new WaitForSeconds(0.1f);

        } while (_curr_conds.cond_time > 0 || _curr_conds.cond_message == "CANCELED");

        ShieldOnEff.SetActive(false);
        _curr_conds.isToDelete = true;
        IDAllreadyUsed.Remove(_curr_conds.cond_id);
    }

    //fire hands 53
    public IEnumerator Condition_53(Conds _curr_conds)
    {
        CastingEffFireHandR.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        FireHandEff.SetActive(true);        

        do
        {
            if (!CastingEffFireHandR.activeSelf) FireHandEff.SetActive(true);

            if (_curr_conds.cond_time > 0.5f)
            {
                if (!FireHandEff.activeSelf) CastingEffFireHandR.SetActive(true);
            } else
            {
                if (FireHandEff.activeSelf) CastingEffFireHandR.SetActive(false);
            }
            yield return new WaitForSeconds(0.250f);

        } while (_curr_conds.cond_time > 0 || _curr_conds.cond_message == "CANCELED");

        FireHandEff.SetActive(false);
        CastingEffFireHandR.SetActive(false);
        yield return new WaitForSeconds(1f);
        _curr_conds.isToDelete = true;
        IDAllreadyUsed.Remove(_curr_conds.cond_id);
    }


    //standart form
    public IEnumerator StandartCasting(Conds _curr_conds, List<GameObject> _obj)
    {
        
        for (int i = 0; i < _obj.Count; i++)
        {
            _obj[i].SetActive(true);
        }

        do
        {
            for (int i = 0; i < _obj.Count; i++)
            {
                if (!_obj[i].activeSelf) _obj[i].SetActive(true);
            }            
            yield return new WaitForSeconds(0.1f);

        } while (_curr_conds.cond_time > 0 || _curr_conds.cond_message == "CANCELED");

        for (int i = 0; i < _obj.Count; i++)
        {
            _obj[i].SetActive(false);
        }

        yield return new WaitForSeconds(1f);
        _curr_conds.isToDelete = true;
        IDAllreadyUsed.Remove(_curr_conds.cond_id);
    }


    public IEnumerator StandartSelfEffect(Conds _curr_conds, List<GameObject> _obj, float delay, float duration, Transform Location)
    {
        yield return new WaitForSeconds(delay);
        List<GameObject> CurrentEffects = new List<GameObject>();        
                
        for (int i = 0; i < _obj.Count; i++)
        {
            GameObject SpellSource = Instantiate(_obj[i], Vector3.zero, Quaternion.identity, Location);
            SpellSource.transform.position = _obj[i].transform.position;
            SpellSource.SetActive(true);

            CurrentEffects.Add(SpellSource);
        }

        for (float u = 0; u < duration; u+=0.1f)        
        {
            for (int i = 0; i < CurrentEffects.Count; i++)
            {
                if (!CurrentEffects[i].activeSelf) CurrentEffects[i].SetActive(true);
            }
            yield return new WaitForSeconds(0.1f);

        }

        for (int i = 0; i < CurrentEffects.Count; i++)
        {
            CurrentEffects[i].SetActive(false);
            Destroy(CurrentEffects[i]);
        }

        yield return new WaitForSeconds(1f);
        CurrentEffects.Clear();
        _curr_conds.isToDelete = true;
        IDAllreadyUsed.Remove(_curr_conds.cond_id);
    }


}
