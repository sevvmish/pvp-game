using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Conds
{
    public string cond_id;
    public string cond_type;
    public int cond_stack;
    public float damage_or_heal;
    public string cond_bulk;
    public string cond_message;
    public bool isCrit;
    public int spell_index;
    public float cond_time;
    public bool isChecked;
    public float coord_x, coord_z;
    public string [] additional_data;
    public bool isToDelete;
}


public class ConditionsAnalys
{
    
    int Limit = general.SessionNumberOfPlayers * 30;

    public List<Conds> curr_conds = new List<Conds>();

    public void Check(string Data)
    {
        
        if (curr_conds.Count>Limit)
        {
            for (int i=0;i<(curr_conds.Count - Limit); i++)
            {
                curr_conds.Remove(curr_conds[0]);
            }
        }

        for (int i = 0; i < curr_conds.Count; i++)
        {
            if (curr_conds[i].isToDelete)
            {
                curr_conds.Remove(curr_conds[i]);
            }
            
        }


        if (Data != "" && Data != null)
        {
            string[] getstrcond1 = Data.Split(',');
                                    
            for (int i = 0; i < getstrcond1.Length-1; i++)
            {

                string[] exact_getstrcond = getstrcond1[i].Split(':');
                if (getstrcond1[i].Length > 1)
                {
                    string IDForCheck = exact_getstrcond[0];
                    string BulkData = exact_getstrcond[1];
                    bool isNegative = false;

                    for (int ii = 0; ii < curr_conds.Count; ii++)
                    {

                        if (curr_conds[ii].cond_id == IDForCheck)
                        {
                            isNegative = true;
                            if (curr_conds[ii].cond_bulk != BulkData)
                            {
                                Decode(getstrcond1[i], ii);
                            }
                        }
                    }

                    if (!isNegative)
                    {
                        Decode(getstrcond1[i], -1);

                    }
                }
            }
        }
    }

  
    public void Decode(string Data, int index_for_existing_conds)
    {
        int Index = 0;

        if (index_for_existing_conds == -1)
        {
            curr_conds.Add(new Conds());
            Index = curr_conds.Count - 1;

        } else
        {
            Index = index_for_existing_conds;
        }

        
        string[] getstrcond1 = Data.Split(':');
        
        curr_conds[Index].cond_id = getstrcond1[0];

        
        curr_conds[Index].cond_bulk = getstrcond1[1];
        
        
        string[] getstrcond = getstrcond1[1].Split('-');
        curr_conds[Index].cond_type = getstrcond[0];

        curr_conds[Index].cond_stack = 0;

        if (curr_conds[Index].cond_type == "co") //condition type in conditions
        {
            curr_conds[Index].spell_index = int.Parse(getstrcond[1]);
            curr_conds[Index].cond_time = float.Parse(getstrcond[2], CultureInfo.InvariantCulture);
            
            if (getstrcond.Length>3)
            {
                curr_conds[Index].cond_stack = int.Parse(getstrcond[3]);                
            }
        }
        else if (curr_conds[Index].cond_type == "dt" || curr_conds[Index].cond_type == "dg") //damage taken or given
        {
            curr_conds[Index].damage_or_heal = float.Parse(getstrcond[1], CultureInfo.InvariantCulture);

            if (getstrcond[2] == "s")
            {
                curr_conds[Index].isCrit = false;
            }
            else if (getstrcond[2] == "c")
            {
                curr_conds[Index].isCrit = true;
            }

            curr_conds[Index].spell_index = int.Parse(getstrcond[3]);
        }
        else if (curr_conds[Index].cond_type == "me" || curr_conds[Index].cond_type == "him") //messages of conditions
        {
            curr_conds[Index].cond_message = getstrcond[1];
        }
        else if (curr_conds[Index].cond_type == "st") //condition type in conditions
        {
            curr_conds[Index].spell_index = int.Parse(getstrcond[1]);
            curr_conds[Index].cond_time = float.Parse(getstrcond[2], CultureInfo.InvariantCulture);
        }
        else if (curr_conds[Index].cond_type == "ca") //condition type in conditions
        {
            
            if (getstrcond[1] == "cncld")
            {
                curr_conds[Index].cond_message = "CANCELED";
                curr_conds[Index].spell_index = int.Parse(getstrcond[2]);
            }
            else
            {
                curr_conds[Index].spell_index = int.Parse(getstrcond[1]);
                curr_conds[Index].cond_time = float.Parse(getstrcond[2], CultureInfo.InvariantCulture); 
            }

            

        } else if (curr_conds[Index].cond_type.Contains("=")) {
            string[] anothergetstrcond = getstrcond1[1].Split('=');
            curr_conds[Index].cond_type = anothergetstrcond[0];
            if (curr_conds[Index].cond_type == "cs") //spell bolt flying around
            {
                curr_conds[Index].spell_index = int.Parse(anothergetstrcond[1]);
                curr_conds[Index].coord_x = float.Parse(anothergetstrcond[2], CultureInfo.InvariantCulture);
                curr_conds[Index].coord_z = float.Parse(anothergetstrcond[3], CultureInfo.InvariantCulture);

            } 
            else if (curr_conds[Index].cond_type == "ad")
            {
                curr_conds[Index].spell_index = int.Parse(anothergetstrcond[1]);
                curr_conds[Index].additional_data = new string[anothergetstrcond.Length - 2];
                for (int i = 0; i < curr_conds[Index].additional_data.Length; i++)
                {
                    curr_conds[Index].additional_data[i] = anothergetstrcond[2 + i];
                }
            }

        } else if (curr_conds[Index].cond_type == "hg" || curr_conds[Index].cond_type == "ht") //damage taken or given
        {
            curr_conds[Index].damage_or_heal = float.Parse(getstrcond[1], CultureInfo.InvariantCulture);

            if (getstrcond[2] == "s")
            {
                curr_conds[Index].isCrit = false;
            }
            else if (getstrcond[2] == "c")
            {
                curr_conds[Index].isCrit = true;
            }

            curr_conds[Index].spell_index = int.Parse(getstrcond[3]);
        } 


    }
}

public struct AnimationsForPlayers
{
    public Animator animator;
    private AudioSource MyAudioSource;
    //private AudioClip BasicMovement;
    private AudioClip BasicWeaponHit;
    private effects MyEffects;
    private int PrevAnimationState;
    private bool isIdle0, isIdle1;

    public int CurrentAnimationState;

    public AnimationsForPlayers(Animator animat, AudioSource AudSource)
    {
        
        PrevAnimationState = 0;
        MyEffects = animat.gameObject.GetComponent<effects>();
        animator = animat;
        MyAudioSource = AudSource;
        CurrentAnimationState = 0;
        //BasicMovement = null;
        BasicWeaponHit = null;
        isIdle0 = false;
        isIdle1 = false;

        if (general.MainPlayerClass == 1)
        {
            //BasicMovement = Resources.Load<AudioClip>("sounds/heavy movement");
            BasicWeaponHit = Resources.Load<AudioClip>("sounds/onehand swing bigger");
        }
    }

    public void RefreshAnimations(int state)
    {



        /*
        if ((PrevAnimationState==3 || PrevAnimationState==8 || PrevAnimationState == 10 || PrevAnimationState == 13 || PrevAnimationState == 15 || PrevAnimationState == 18 || PrevAnimationState == 22) && (state < 2) )
        {
            Idle();
        }

        
        
        if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(1).IsName("Idle")) && CurrentAnimationState != 0)
        {
            CurrentAnimationState = 0;
        }
        else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Run") && CurrentAnimationState != 1)
        {
            CurrentAnimationState = 1;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("turning") && CurrentAnimationState != -1)
        {
            CurrentAnimationState = -1;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Runback") && CurrentAnimationState != -2)
        {
            CurrentAnimationState = -2;
        }


        //data control send to EFFECTS
        //STUNNED
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("stunned") && !MyEffects.isStunned)
        {
            MyEffects.isStunned = true;
        }
        else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("stunned") && MyEffects.isStunned)
        {
            MyEffects.isStunned = false;
        }
        //SHIELD SLAM
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("shield slam") && !MyEffects.isShieldSlam)
        {
            MyEffects.isShieldSlam = true;
        }
        else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("shield slam") && MyEffects.isShieldSlam)
        {
            MyEffects.isShieldSlam = false;
        }
        //CASTING
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("casting") && !MyEffects.isCasting)
        {
            MyEffects.isCasting = true;
        }
        else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("casting") && MyEffects.isCasting )
        {
            MyEffects.isCasting = false;
        }


        //changing state
        if (CurrentAnimationState != state)
        {
            switch (state)
            {
                case -2:
                    if (CurrentAnimationState < 2)
                    {
                        Runback();
                    }
                    break;
                case -1:
                    if (CurrentAnimationState < 2)
                    {
                        turning();
                    }
                    break;
                case 0:
                    if (CurrentAnimationState < 2)
                    {
                        Idle();
                    }
                    break;
                case 1:
                    if (CurrentAnimationState < 2)
                    {
                        Run();
                    }
                    break;
                case 2:
                    
                    if (!animator.GetCurrentAnimatorStateInfo(1).IsName("HitWith1H2"))
                    {
                        HitWith1H();
                    }
                    
                    break;
                case 3:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("casting"))
                    {
                        Casting();
                    }
                    break;
                case 4:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("ReceiveDamage"))
                    {
                        ReceiveDamage();
                    }
                    break;
                case 5:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("shoot spell"))
                    {
                        ShootSpell();
                    }
                    break;
                case 6:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
                    {
                        Dodge();
                    }
                    break;
                case 7:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Block"))
                    {
                        Block();
                    }
                    break;
                case 8:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("stunned"))
                    {
                        Stunned();
                    }
                    break;
                case 9:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("shield slam"))
                    {
                        ShieldSlam();
                    }
                    break;
                case 10:
                    if (!animator.GetCurrentAnimatorStateInfo(1).IsName("ShieldOn"))
                    {
                        ShieldOn();
                    }
                    break;
                case 11:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("alternative attack"))
                    {
                        AltAttack();
                    }
                    break;
                case 12:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("superior attack"))
                    {
                        SuperAttack();
                    }
                    break;
                case 13:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("channeling spell"))
                    {
                        ChannelingSpell();
                    }
                    break;
                case 14:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("invisibility"))
                    {
                        invisibility();
                    }
                    break;
                case 15:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("hurricane"))
                    {
                        hurricane();
                    }
                    break;
                case 16:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("heroic leap"))
                    {
                        heroicleap();
                    }
                    break;
                case 17:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("fall down"))
                    {
                        falldown();
                    }
                    break;
                case 18:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("lying"))
                    {
                        lying();
                    }
                    break;
                case 19:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("get up"))
                    {
                        getup();
                    }
                    break;
                case 20:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("raise gun"))
                    {
                        raise_gun();
                    }
                    break;
                case 21:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("shot gun"))
                    {
                        shot_gun();
                    }
                    break;
                case 22:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("death"))
                    {
                        death();
                    }
                    break;
            }
        }

        PrevAnimationState = CurrentAnimationState;
        */



        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            isIdle0 = true;
        }

        if (animator.GetCurrentAnimatorStateInfo(1).IsName("Idle1"))
        {
            isIdle1 = true;
        }


        if ((CurrentAnimationState == 3 || CurrentAnimationState == 8 || CurrentAnimationState == 10 || CurrentAnimationState == 13 || CurrentAnimationState == 15 || CurrentAnimationState == 18 || CurrentAnimationState == 22) && (state < 2))
        {
            Idle();
        }

        /*
        if (isIdle0 && state == 101 && animator.GetCurrentAnimatorStateInfo(1).IsName("ShieldOn"))
        {
            //animator.StopPlayback();
            animator.Play("Run");
        } else if (isIdle0 && state == 102 && animator.GetCurrentAnimatorStateInfo(1).IsName("ShieldOn"))
        {
            //animator.StopPlayback();
            animator.Play("Runback");
        }
        */

        if (isIdle0 && state==1)
        {
            animator.StopPlayback();
            animator.Play("Run");
        }
        
               
        if (state==0 && animator.GetCurrentAnimatorStateInfo(0).IsName("Run") && isIdle1)
        {
            animator.StopPlayback();
            animator.Play("Idle");
        }

        if (
            (animator.GetCurrentAnimatorStateInfo(0).IsName("channeling spell") && state!=13) ||
            (animator.GetCurrentAnimatorStateInfo(0).IsName("casting") && state != 3) ||
            //(animator.GetCurrentAnimatorStateInfo(0).IsName("stunned") && state != 8) ||
            //(animator.GetCurrentAnimatorStateInfo(1).IsName("ShieldOn") && (state != 10 && state != 101 && state != 102)) ||            
            (animator.GetCurrentAnimatorStateInfo(0).IsName("hurricane") && state != 15) ||
            (animator.GetCurrentAnimatorStateInfo(0).IsName("lying") && state != 18)             
            )
        {
            animator.StopPlayback();
            animator.Play("Idle");
        }

        if (CurrentAnimationState !=0 && (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && animator.GetCurrentAnimatorStateInfo(1).IsName("Idle1")))
        {
            CurrentAnimationState = 0;
            //Debug.Log("worked to 00000000000000");
        }

        /*
        if (state>2)
        {
            Debug.Log(state + " - state          curr state - "  + CurrentAnimationState);
        }
        */

        switch (state)
        {
            case -2:
                if (CurrentAnimationState < 2)
                {
                    Runback();
                }
                break;
            case -1:
                if (CurrentAnimationState < 2)
                {
                    turning();
                }
                break;
            case 0:
                if (CurrentAnimationState < 2)
                {
                    Idle();
                }
                break;
            case 1:
                if (CurrentAnimationState < 2)
                {
                    Run();
                }
                break;
            case 2:

                if (!animator.GetCurrentAnimatorStateInfo(1).IsName("HitWith1H2"))
                {
                    HitWith1H();
                }

                break;
            case 3:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("casting"))
                {
                    Casting();
                }
                break;
            case 4:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("ReceiveDamage"))
                {
                    ReceiveDamage();
                }
                break;
            case 5:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("shoot spell"))
                {
                    ShootSpell();
                }
                break;
            case 6:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
                {
                    Dodge();
                }
                break;
            case 7:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Block"))
                {
                    Block();
                }
                break;
            case 8:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("stunned"))
                {
                    Stunned();
                }
                break;
            case 9:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("shield slam"))
                {
                    ShieldSlam();
                }
                break;
            case 10:
                if (!animator.GetCurrentAnimatorStateInfo(1).IsName("ShieldOn"))
                {
                    ShieldOn();
                }
                break;
            case 101:
                if (!animator.GetCurrentAnimatorStateInfo(1).IsName("ShieldOn"))
                {
                    ShieldOnForward();
                }
                break;
            case 102:
                if (!animator.GetCurrentAnimatorStateInfo(1).IsName("ShieldOn"))
                {
                    ShieldOnBackward();
                }
                break;
            case 11:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("alternative attack") && CurrentAnimationState!=11)
                {
                    AltAttack();
                }
                break;
            case 12:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("superior attack"))
                {
                    SuperAttack();
                }
                break;
            case 13:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("channeling spell"))
                {
                    ChannelingSpell();
                }
                break;
            case 14:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("invisibility"))
                {
                    invisibility();
                }
                break;
            case 15:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("hurricane"))
                {
                    hurricane();
                }
                break;
            case 16:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("heroic leap") && CurrentAnimationState!=16)
                {
                    heroicleap();
                }
                break;
            case 17:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("fall down"))
                {
                    falldown();
                }
                break;
            case 18:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("lying"))
                {
                    lying();
                }
                break;
            case 19:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("get up"))
                {
                    getup();
                }
                break;
            case 20:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("raise gun"))
                {
                    raise_gun();
                }
                break;
            case 21:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("shot gun"))
                {
                    shot_gun();
                }
                break;
            case 22:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("death"))
                {
                    death();
                }
                break;
            case 23:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("buff"))
                {
                    buff();
                }
                break;
        }

        //data control send to EFFECTS
        //STUNNED
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("stunned") && !MyEffects.isStunned)
        {
            MyEffects.isStunned = true;
        }
        else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("stunned") && MyEffects.isStunned)
        {
            MyEffects.isStunned = false;
        }
        //SHIELD SLAM
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("shield slam") && !MyEffects.isShieldSlam)
        {
            MyEffects.isShieldSlam = true;
        }
        else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("shield slam") && MyEffects.isShieldSlam)
        {
            MyEffects.isShieldSlam = false;
        }
        //CASTING
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("casting") && !MyEffects.isCasting)
        {
            MyEffects.isCasting = true;
        }
        else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("casting") && MyEffects.isCasting)
        {
            MyEffects.isCasting = false;
        }
        //channeling
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("channeling spell") && !MyEffects.isChanneling)
        {
            MyEffects.isChanneling = true;
        }
        else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("channeling spell") && MyEffects.isChanneling)
        {
            MyEffects.isChanneling = false;
        }

    }

    void Runback()
    {
        animator.Play("Runback");
        CurrentAnimationState = -2;
    }

    void turning()
    {
        animator.Play("turning");
        CurrentAnimationState = -1;
    }

    void Idle()
    {
        animator.StopPlayback();
        animator.Play("Idle");
        animator.Play("Idle1");
        CurrentAnimationState = 0;
                
    }

    void Run()
    {
        
        animator.Play("Run");
        CurrentAnimationState = 1;
    }

    void HitWith1H()
    {
        animator.StopPlayback();
        animator.Play("HitWith1H2");
        CurrentAnimationState = 2;
        
    }

    void Casting()
    {
        animator.Play("casting");
        CurrentAnimationState = 3;
    }

    void ReceiveDamage()
    {
        animator.Play("ReceiveDamage");
        CurrentAnimationState = 4;
        
    }


    void ShootSpell()
    {
        animator.Play("shoot spell");
        CurrentAnimationState = 5;

    }

    void Dodge()
    {
        animator.Play("Dodge");
        CurrentAnimationState = 6;
        
    }

    void Block()
    {
        animator.Play("Block");
        CurrentAnimationState = 7;
        
    }

    void Stunned()
    {
        animator.Play("stunned");
        CurrentAnimationState = 8;
        

    }

    void ShieldSlam()
    {
        animator.Play("shield slam");
        CurrentAnimationState = 9;

    }

    void ShieldOn()
    {
        animator.Play("ShieldOn");
       
        CurrentAnimationState = 10;

    }


    void ShieldOnForward()
    {
        animator.Play("ShieldOn");
        animator.Play("Run");
        CurrentAnimationState = 101;
    }

    void ShieldOnBackward()
    {
        animator.Play("ShieldOn");
        animator.Play("Runback");
        CurrentAnimationState = 102;
    }

    void AltAttack()
    {
        animator.Play("alternative attack");
        CurrentAnimationState = 11;

    }

    void SuperAttack()
    {
        animator.Play("superior attack");
        CurrentAnimationState = 12;

    }


    void ChannelingSpell()
    {
        animator.Play("channeling spell");
        CurrentAnimationState = 13;

    }

    void invisibility()
    {
        animator.Play("invisibility");
        CurrentAnimationState = 14;
    }

    void hurricane()
    {
        animator.Play("hurricane");
        CurrentAnimationState = 15;
    }

    void heroicleap()
    {
        animator.Play("heroic leap");
        CurrentAnimationState = 16;
    }

    void falldown()
    {
        animator.Play("fall down");
        CurrentAnimationState = 17;
    }

    void lying()
    {
        animator.Play("lying");
        CurrentAnimationState = 18;
    }

    void getup()
    {
        animator.Play("get up");
        CurrentAnimationState = 19;
    }

    void raise_gun()
    {
        animator.Play("raise gun");
        CurrentAnimationState = 20;
    }

    void shot_gun()
    {
        animator.Play("shot gun");
        CurrentAnimationState = 21;
    }

    void death()
    {
        animator.Play("death");
        CurrentAnimationState = 22;
    }

    void buff()
    {
        animator.Play("buff");
        CurrentAnimationState = 23;
    }
}


public struct ReceivePlayersData
{
    public string player_id;
    public int player_class;
    public string player_name;
    private float position_x;
    private float position_y;
    private float position_z;
    private float rotation_x;
    private float rotation_y;
    private float rotation_z;
    public float speed;
    public int animation_id;
    public string conditions;
    public float health_pool;
    public float max_health_pool;
    public float energy;

    public Vector3 position;
    public Vector3 rotation;

    private string[] all_health;

    public void ReceiveForPlayers(string ReceivedPacket)
    {
        string[] getstr = new string[13];
        
        //try
        //{
        getstr = ReceivedPacket.Split('~');
        position_x = float.Parse(getstr[0].Replace('.',','));//float.Parse(getstr[0], CultureInfo.InvariantCulture);
        position_y = 0;
        position_z = float.Parse(getstr[1].Replace('.', ','));
        rotation_x = 0;
        rotation_y = float.Parse(getstr[2].Replace('.', ','));
        rotation_z = 0;
        //speed = float.Parse(getstr[3], CultureInfo.InvariantCulture);
        animation_id = int.Parse(getstr[3], CultureInfo.InvariantCulture);
        conditions = getstr[4];
            
        all_health = getstr[5].Split('=');
        health_pool = float.Parse(all_health[0], CultureInfo.InvariantCulture);
        max_health_pool = float.Parse(all_health[1], CultureInfo.InvariantCulture);
            
        energy = int.Parse(getstr[6], CultureInfo.InvariantCulture);
        position = new Vector3(position_x, position_y, position_z);
        rotation = new Vector3(rotation_x, rotation_y, rotation_z);
                
    }

    public void ReceiveForNonPlayers(string ReceivedPacket)
    {
        string[] getstr = new string[13];

        //try
        //{
        getstr = ReceivedPacket.Split('~');
        player_id = getstr[0];
        player_class = int.Parse(getstr[1], CultureInfo.InvariantCulture);
        player_name = getstr[2];
        position_x = float.Parse(getstr[3].Replace('.', ','));//float.Parse(getstr[0], CultureInfo.InvariantCulture);
        position_y = 0;
        position_z = float.Parse(getstr[4].Replace('.', ','));
        rotation_x = 0;
        rotation_y = float.Parse(getstr[5].Replace('.', ','));
        rotation_z = 0;
        //speed = float.Parse(getstr[3], CultureInfo.InvariantCulture);
        animation_id = int.Parse(getstr[6], CultureInfo.InvariantCulture);
        conditions = getstr[7];

        all_health = getstr[8].Split('=');
        health_pool = float.Parse(all_health[0], CultureInfo.InvariantCulture);
        max_health_pool = float.Parse(all_health[1], CultureInfo.InvariantCulture);

        energy = int.Parse(getstr[9], CultureInfo.InvariantCulture);
        position = new Vector3(position_x, position_y, position_z);
        rotation = new Vector3(rotation_x, rotation_y, rotation_z);

    }
}



public struct ToSend
{
    public int OrderToSend;
    public int Specification;
    private string PlayerID;
    private string TemporaryTable;
    public float HorizontalTouch;
    public float VerticalTouch;
    public int Butt1;
    public int Butt2;
    public int Butt3;
    public int Butt4;
    public int Butt5;
    public int Butt6;
    private float limit;

    public static int counter;
    public static int counter_ts;

    private void MakeClean()
    {
        Specification = 0;
        HorizontalTouch = 0;
        VerticalTouch = 0;
        Butt1 = 0;
        Butt2 = 0;
        Butt3 = 0;
        Butt4 = 0;
        Butt5 = 0;
        Butt6 = 0;
    }

    public string ToSendKillSignal()
    {
        PlayerID = general.SessionPlayerID;
        TemporaryTable = general.SessionTicket;

        return OrderToSend.ToString() + "~3~" + PlayerID + "~" + TemporaryTable;
    }


    


    public string ToSendMovement(float Horiz, float Vert)
    {
        MakeClean();
        PlayerID = general.SessionPlayerID;
        TemporaryTable = general.SessionTicket;
       
        HorizontalTouch = Horiz;
        VerticalTouch = Vert;

        //Debug.Log(HorizontalTouch + " -=======");

        StringBuilder Result = new StringBuilder(70);
        OrderToSend++;

        
        if (general.HUB1_ip != "127.0.0.1" && general.HUB1_ip != "192.168.0.103")
        {
            Result.Append(general.PacketID + "~" + OrderToSend.ToString() + "~0~" + PlayerID + "~" + TemporaryTable + "~" + HorizontalTouch.ToString("f1").Replace(',', '.') + "~" + VerticalTouch.ToString("f1").Replace(',', '.')); //+  + "~0~0~0~0~0~0|"
        } else
        {
            Result.Append(general.PacketID + "~" + OrderToSend.ToString() + "~0~" + PlayerID + "~" + TemporaryTable + "~" + HorizontalTouch.ToString("f1") + "~" + VerticalTouch.ToString("f1")); //+  + "~0~0~0~0~0~0|"
        }
         
        

        return Result.ToString();
    }

    public string ToSendButtons(float Horiz, float Vert, int But1, int But2, int But3, int But4, int But5, int But6)
    {
        MakeClean();
        PlayerID = general.SessionPlayerID;
        TemporaryTable = general.SessionTicket;

        
        HorizontalTouch = Horiz;
        VerticalTouch = Vert;

        Butt1 = But1;
        Butt2 = But2;
        Butt3 = But3;
        Butt4 = But4;
        Butt5 = But5;
        Butt6 = But6;
        StringBuilder Result = new StringBuilder(70);
        OrderToSend++;
        if (general.HUB1_ip != "127.0.0.1" && general.HUB1_ip != "192.168.0.103")
        {
            Result.Append(general.PacketID + "~"+ OrderToSend.ToString() + "~1~" + PlayerID + "~" + TemporaryTable + "~" + HorizontalTouch.ToString("f1").Replace(',', '.') + "~" + VerticalTouch.ToString("f1").Replace(',', '.') + "~" + Butt1.ToString() + "~" + Butt2.ToString() + "~" + Butt3.ToString() + "~" + Butt4.ToString() + "~" + Butt5.ToString() + "~" + Butt6.ToString() );
        } else
        {
            Result.Append(general.PacketID + "~" + OrderToSend.ToString() + "~1~" + PlayerID + "~" + TemporaryTable + "~" + HorizontalTouch.ToString("f1") + "~" + VerticalTouch.ToString("f1") + "~" + Butt1.ToString() + "~" + Butt2.ToString() + "~" + Butt3.ToString() + "~" + Butt4.ToString() + "~" + Butt5.ToString() + "~" + Butt6.ToString());
        }
        
        return Result.ToString();
    }

}

public static class SendAndReceive
{
    public static ToSend DataForSending;
    public static ReceivePlayersData MyPlayerData;
    //public static ReceivePlayersData[] OtherPlayerData = new ReceivePlayersData[general.SessionNumberOfPlayers - 1];
    public static ReceivePlayersData[] OtherPlayerData = new ReceivePlayersData[100];
    private static List<string> non_players_ui = new List<string>();
    public static int OrderReceived;
    public static int SpecificationReceived;
    public static int ButtonState;
    public static string MessageType;
    public static float SpellCooldown;
    public static int SwButtonCond;
    public static int SpellIndex;
    public static string UniqCode;


    public static void GetHeader(string Header)
    {
        string[] getstr = new string[5];
        getstr = Header.Split('~');
        OrderReceived = int.Parse(getstr[0]);
        SpecificationReceived = int.Parse(getstr[1]);
        if (SpecificationReceived == 1)
        {
            ButtonState = int.Parse(getstr[2]);
            MessageType = MessageTypeDecode(int.Parse(getstr[3]));
            SpellCooldown = float.Parse(getstr[4].Replace('.', ','));            
        }

        if (SpecificationReceived == 2)
        {
            SpellIndex = int.Parse(getstr[2]);
            MessageType = MessageTypeDecode(int.Parse(getstr[3]));
            SpellCooldown = float.Parse(getstr[4].Replace('.', ','));
        }
    }

    public static void TranslateDataFromServer(string ReceivedData)
    {
        //string[] getstr = new string[general.SessionNumberOfPlayers + 1];
        string[] getstr;

        try
        {
            
            getstr = ReceivedData.Split('^');
            //1
            GetHeader(getstr[0]);
            if (SpecificationReceived == 0)
            {
                int index = 0;
                //2
                MyPlayerData.ReceiveForPlayers(getstr[1]);
                //... ...
                for (int i = 0; i < (general.SessionNumberOfPlayers - 1); i++)
                //for (int i = 0; i < (playercontrol.OtherGamers.Count); i++)
                {
                    OtherPlayerData[index].ReceiveForPlayers(getstr[2 + i]);
                    index++;
                }

                if ((getstr.Length-1) > (general.SessionNumberOfPlayers + 1))
                {                    
                   
                    int MaxMany = general.SessionNumberOfPlayers+1;
                    Debug.Log((getstr.Length-1) + " - maxL     maxmenu - " + (general.SessionNumberOfPlayers + 1));


                    for (int i = (general.SessionNumberOfPlayers + 1); i < (getstr.Length-1); i++)
                    {
                        
                        OtherPlayerData[index].ReceiveForNonPlayers(getstr[i]);
                        Debug.Log(i + "ddddddddddddddd");

                        if (!non_players_ui.Contains(OtherPlayerData[index].player_id))
                        {                            
                            non_players_ui.Add(OtherPlayerData[index].player_id);
                            playercontrol.AddNonPlayer(OtherPlayerData[index].position, OtherPlayerData[index].rotation, i, OtherPlayerData[index].player_class, OtherPlayerData[index].player_name);
                        }
                        index++;
                    }
                }
            }

            
            
        }
        catch (Exception ex)
        {
            Debug.Log(ex + " -> " + ReceivedData);
            
        }
    }

    private static string MessageTypeDecode(int messtype)
    {
        string ReturnMess = "";

        switch (messtype)
        {
            case 0:
                ReturnMess = "OK";
                break;
            case 1:
                ReturnMess = "global cooldown";
                break;
            case 2:
                ReturnMess = "this spell cooldown";
                break;
            case 3:
                ReturnMess = "too far";
                break;
            case 4:
                ReturnMess = "cant hit ally";
                break;
            case 5:
                ReturnMess = "direct damage";
                break;
            case 6:
                ReturnMess = "DOT";
                break;
            case 7:
                ReturnMess = "not enough energy";
                break;
            case 8:
                ReturnMess = "hit+DOT";
                break;
            case 10:
                ReturnMess = "you are stunned!";
                break;
            case 11:
                ReturnMess = "you are feared";
                break;
            case 12:
                ReturnMess = "you are busy with another spell";
                break;
            case 13:
                ReturnMess = "you allready on this spell";
                break;
            case 14:
                ReturnMess = "you are knocked down";
                break;

        }
        return ReturnMess;
    }
}



public struct Joysticks
{
    public float Vertical, Horizontal;
    public void CheckTouches(float vert, float hor)
    {
        Vertical = 0;
        Horizontal = 0;

        if ((vert < -0.85f || vert > 0.85f) && (hor < 0.15f || hor > -0.15f))
        {
            Horizontal = 0;
        }

        if ((hor < -0.85f || hor > 0.85f) && (vert < 0.15f || vert > -0.15f))
        {
            Vertical = 0;
        }

        if (vert < -0.85f || vert > 0.85f)
        {
            Vertical = vert * 1.2f;
        }
        else if ((vert >= -0.85f && vert < -0.6f) || (vert <= 0.85f && vert > 0.6f))
        {
            Vertical = vert * 1f;
        }
        else if ((vert >= -0.6f && vert < -0.25f) || (vert <= 0.6f && vert > 0.25f))
        {
            Vertical = vert * 0.9f;
        }
        else if ((vert >= -0.25f && vert <= 0f) || (vert <= 0.25f && vert >= 0f))
        {
            Vertical = vert * 0.6f;
        }

        //=================================

        if (hor < -0.85f || hor > 0.85f)
        {
            Horizontal = hor * 1.5f; //2
        }
        else if ((hor >= -0.85f && hor < -0.6f) || (hor <= 0.85f && hor > 0.6f))
        {
            Horizontal = hor * 1f;//1.5
        }
        else if ((hor >= -0.6f && hor < -0.25f) || (hor <= 0.6f && hor > 0.25f))
        {
            Horizontal = hor * 0.8f;//1.2
        }
        else if ((hor >= -0.25f && hor <= 0f) || (hor <= 0.25f && hor >= 0f))
        {
            Horizontal = hor * 0.5f;//0.8
        }

        //Vertical = vert;
        //Horizontal = hor;
    }
}


public class PlayerUI : MonoBehaviour
{
    public GameObject AllObject;
    public RectTransform UIPosition;
    private GameObject cond_example;
    public float HealthCurrentAmount = 0;
    private float EnergyCurrentAmount = 0;
    private float HealthAllAmount = 0;
    private float EnergyAllAmount = 100;
    private Image HealthButton;
    private Image EnergyButton;
    private bool isRestarting;

    private Image CastingBar;
    private Image CastingSpellImage;

    private int CondObjectLenth = 21;
    public bool isMainPlayer;

    public bool isCasting = false;
    private TextMeshProUGUI CancelationText, StacksText;
    private bool isShowStopCastingText;

    public class CondManager
    {
        public string cond_id;
        public string cond_sprite;
        public GameObject con_object;
        public bool isBusy;
        public int spell_index;
        public float spell_timer;
        public int stacks;
    }

    public List<CondManager> CondObjects = new List<CondManager>();
    private Vector2[] CondPositions = {
        new Vector2(75,0),
        new Vector2(130,0),
        new Vector2(185,0),
        new Vector2(240,0),
        new Vector2(295,0),
        new Vector2(350,0),
        new Vector2(405,0),
        
        
        //=============================
        new Vector2(75,-75),
        new Vector2(130,-75),
        new Vector2(185,-75),
        new Vector2(240,-75),
        new Vector2(295,-75),
        new Vector2(350,-75),
        new Vector2(405,-75),


    };


    private Vector2[] CondPositionsForOtherPlayer = {  //-65    -120
        new Vector2(0,35),
        new Vector2(34,35),
        new Vector2(68,35),
        new Vector2(102,35),
        new Vector2(0,70),
        new Vector2(34,70),
        new Vector2(68,70),
        
        
        //=============================
        new Vector2(102,70),
        //new Vector2(40,50),
        //new Vector2(80,50),
        //new Vector2(120,50),
        //new Vector2(160,50),
        //new Vector2(200,50),
        //new Vector2(240,50),

    };




    public PlayerUI(GameObject AllO, bool isitMainPlayer)
    {
        AllObject = AllO;
        isMainPlayer = isitMainPlayer;
        if (!isMainPlayer)
        {
            CondPositions = CondPositionsForOtherPlayer;
        }
        
        cond_example = AllObject.transform.GetChild(2).gameObject;
        cond_example.SetActive(false);
        for (int i = 0; i < CondObjectLenth; i++)
        {
            CondObjects.Add(new CondManager());
            CondObjects[i].con_object = Instantiate(cond_example, Vector3.zero, Quaternion.identity, AllObject.transform.GetChild(4).gameObject.transform);
            CondObjects[i].con_object.SetActive(false);
        }

        HealthButton = AllObject.transform.GetChild(0).Find("HealthBarButton").GetComponent<Image>();
        EnergyButton = AllObject.transform.GetChild(1).Find("EnergyBarButton").GetComponent<Image>();

        CastingBar = AllObject.transform.GetChild(3).Find("CastingBar").GetComponent<Image>();
        CastingSpellImage = AllObject.transform.GetChild(3).Find("SpellForCast").GetComponent<Image>();
        CancelationText = AllObject.transform.GetChild(3).Find("CanceledText").GetComponent<TextMeshProUGUI>();
        CastingBar.gameObject.SetActive(false);
        CastingSpellImage.gameObject.SetActive(false);
        CancelationText.gameObject.SetActive(false);
        CancelationText.text = languages.lang.Canceled;
        UIPosition = AllObject.GetComponent<RectTransform>();
        ResetAllConds();
    }

    public void StopCurrentCasting()
    {
        isShowStopCastingText = true;
        CastingBar.gameObject.SetActive(false);
        CastingSpellImage.gameObject.SetActive(false);
        isCasting = false;
        CancelationText.gameObject.SetActive(false);
    }

    public IEnumerator AddCasting(Conds data)
    {
        int spell_ind = data.spell_index;
        float spell_time = data.cond_time;
        isCasting = true;
        CastingBar.gameObject.SetActive(true);
        CastingSpellImage.gameObject.SetActive(true);
        CastingBar.fillAmount = 1;
        CastingSpellImage.sprite = DB.GetSpellByNumber(spell_ind).Spell1_icon;

        float delta = 1 / (spell_time / 0.1f);

        
        for (float i = spell_time; i > 0; i -= 0.05f)
        {
            //CastingBar.fillAmount -=delta;
            CastingBar.fillAmount = data.cond_time / spell_time;

            if (isShowStopCastingText)
            {
                CancelationText.gameObject.SetActive(true);
                break;
            }

            if (data.cond_time<0.1f)
            {
                break;
            }
            yield return new WaitForSeconds(0.05f);
        }
        

       

        isCasting = false;
        CastingBar.gameObject.SetActive(false);
        CastingSpellImage.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        
        isShowStopCastingText = false;
        CancelationText.gameObject.SetActive(false);
    }

    private void ResetAllConds()
    {
        for (int iii = 0; iii < CondObjects.Count; iii++)
        {
           
            Animator animator = CondObjects[iii].con_object.transform.GetChild(0).gameObject.GetComponent<Animator>();
            TextMeshProUGUI spell_timer = CondObjects[iii].con_object.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI spell_stacks = CondObjects[iii].con_object.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();

            CondObjects[iii].con_object.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            CondObjects[iii].isBusy = false;
            CondObjects[iii].cond_id = null;
            CondObjects[iii].spell_index = 0;
            CondObjects[iii].spell_timer = 0;


            CondObjects[iii].con_object.SetActive(false);
            spell_timer.text = "";
            CondObjects[iii].con_object.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = null;
            spell_stacks.text = "";
            Color curcolor = CondObjects[iii].con_object.transform.GetChild(0).gameObject.GetComponent<Image>().color;
            curcolor.a = 1f;
            CondObjects[iii].con_object.transform.GetChild(0).gameObject.GetComponent<Image>().color = curcolor;

                       

        }

        

    }
    public IEnumerator AddCondition(Conds data)
    {
        string condID = data.cond_id;
        int spell_ind = data.spell_index;
        float spell_time = data.cond_time;
        int stacks = data.cond_stack;

        if (data.spell_index==1000 || data.spell_index == 1001 || data.spell_index == 1002 || data.spell_index == 1003 || data.spell_index == 1004 || data.spell_index == 1005)
        {
            yield break;
        }

        if (data.spell_index == 1007)
        {
            yield return new WaitForSeconds(data.cond_time);
            ResetAllConds();          
            
            yield break;
        }

        //print(condID + ": " + spell_ind + " - ");
        bool isOK = true;

        for (int i = 0; i < CondObjects.Count; i++)
        {
            if (CondObjects[i].cond_id == condID)
            {
                isOK = false;
                break;
            }
        }

        if (isOK)
        {
            bool isSameSpell = false;

            
            for (int i = 0; i < CondObjects.Count; i++)
            {
                if (CondObjects[i].isBusy && CondObjects[i].spell_index == spell_ind)
                {
                    isSameSpell = true;
                    if (spell_time > CondObjects[i].spell_timer)
                    {
                        CondObjects[i].spell_timer = spell_time;
                        
                    }

                    break;
                }
            }
            

            if (!isSameSpell)
            {
                int index = 0;
                Vector2 position = Vector2.zero;

                for (int i = 0; i < CondObjects.Count; i++)
                {
                    if (!CondObjects[i].isBusy)
                    {
                        index = i;
                        break;
                    }
                }

                for (int ii = 0; ii < CondPositions.Length; ii++)
                {
                    bool isCheck = true;

                    for (int i = 0; i < CondObjects.Count; i++)
                    {
                        if (CondObjects[i].con_object.GetComponent<RectTransform>().anchoredPosition == CondPositions[ii])
                        {
                            isCheck = false;
                        }
                    }

                    if (isCheck)
                    {
                        position = CondPositions[ii];
                        break;
                    }
                }

                CondObjects[index].isBusy = true;
                CondObjects[index].cond_id = condID;
                CondObjects[index].spell_index = spell_ind;
                CondObjects[index].spell_timer = spell_time;
                CondObjects[index].stacks = stacks;
                
                CondObjects[index].con_object.SetActive(true);
                CondObjects[index].con_object.GetComponent<RectTransform>().anchoredPosition = position;
                TextMeshProUGUI spell_timer = CondObjects[index].con_object.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI spell_stacks = CondObjects[index].con_object.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();

                spell_timer.text = CondObjects[index].spell_timer.ToString();
                if (data.cond_stack > 1)
                {
                    spell_stacks.text = data.cond_stack.ToString();
                }
                CondObjects[index].con_object.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = DB.GetSpellByNumber(spell_ind).Spell1_icon;
                Animator animator = CondObjects[index].con_object.transform.GetChild(0).gameObject.GetComponent<Animator>();
                
                

                animator.SetBool("appear", true);

                do
                {
                    yield return new WaitForSeconds(0.4f);

                    //CondObjects[index].spell_timer -= 0.2f;                    
                    if (data.cond_time==99)
                    {
                        spell_timer.text = "";
                    } 
                    else
                    {
                        spell_timer.text = data.cond_time.ToString("f0");
                    }



                    if (data.cond_stack > 1 && spell_stacks.text!= data.cond_stack.ToString())
                    {
                        spell_stacks.text = data.cond_stack.ToString();
                        animator.SetBool("long", false);                        
                        animator.SetBool("appear", false);
                        animator.SetBool("tremor", true);
                        animator.Play("idle");
                    }
                    else
                    {

                        if (data.cond_time < 2f)
                        {
                            animator.SetBool("long", true);                            
                            animator.SetBool("appear", false);
                            animator.SetBool("tremor", false);
                            animator.Play("idle");
                        }                        
                        else if (data.cond_time >= 2f)
                        {
                            
                            animator.SetBool("long", false);                           
                            animator.SetBool("appear", false);
                            animator.SetBool("tremor", false);
                            //animator.Play("idle");
                            
                        }
                    }

                    bool isRestarting = false;
                    for (int i = 0; i < playercontrol.MyUI.CondObjects.Count; i++)
                    {
                        if (playercontrol.MyUI.CondObjects[i].spell_index==1006 || playercontrol.MyUI.CondObjects[i].spell_index == 1007)
                        {
                            isRestarting = true;
                            break;
                        }
                    }

                    if (data.cond_time == 0 || isRestarting || data.cond_message== "CANCELED")
                    {
                        break;
                    }
                } while (data.cond_time > 0.4f);

                yield return new WaitForSeconds(0.5f);

                //string texter = CondObjects[index].con_object.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text;
                Vector2 expCoords = CondObjects[index].con_object.GetComponent<RectTransform>().anchoredPosition;
                animator.SetBool("long", false);
                animator.SetBool("appear", false); 
                animator.SetBool("tremor", false);
                CondObjects[index].con_object.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                CondObjects[index].isBusy = false;
                CondObjects[index].con_object.SetActive(false);
                spell_timer.text = "";
                CondObjects[index].con_object.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = null;
                CondObjects[index].con_object.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.white;
                spell_stacks.text = "";

                //JustDO(CondObjects[index], position, spell_time);

                CheckEmptySlotsConditions(expCoords);

            }
        }                
    }

    private void CheckEmptySlotsConditions(Vector2 ExpPosition)
    {
        int place_number = -1;
        for (int i=0; i<CondPositions.Length; i++)
        {
            if (CondPositions[i]== ExpPosition)
            {
                place_number = i;
                break;
            }
        }
        
        if (place_number >= 0)
        {
                        
            for (int i = 0; i < CondObjects.Count; i++)
            {
                if (CondObjects[i].isBusy)
                {
                    for (int ii=0; ii < CondPositions.Length; ii++)
                    {
                        if (CondObjects[i].con_object.GetComponent<RectTransform>().anchoredPosition == CondPositions[ii])
                        {
                            if (ii>place_number && ii>0)
                            {
                                CondObjects[i].con_object.GetComponent<RectTransform>().anchoredPosition = CondPositions[ii - 1];                                
                            }
                        }
                    }
                }
            }
        }

    }


    public void HealthInput(float HealthAm, float MaxHealthAm)
    {
        HealthCurrentAmount = HealthAm;
        HealthAllAmount = MaxHealthAm;
     
        HealthButton.fillAmount = HealthCurrentAmount / HealthAllAmount;
    }

    public void EnergyInput(float EnergyAm)
    {
        EnergyCurrentAmount = EnergyAm;
        EnergyButton.fillAmount = EnergyCurrentAmount / EnergyAllAmount;
    }

}


public class Buttons
{
    public  Button SpellButton1, SpellButton2, SpellButton3, SpellButton4, SpellButton5, SpellButton6;
    public  string WhatPacketAwaiting;
    public  float CountTries;
    public  int ButtonNumber;

    public static HashSet<string> unique_codes = new HashSet<string>();
    

    public struct WhichButton
    {
        public int OrderForCheking;
        public int ButtonNumber;

        public void set(int ord, int butt)
        {
            OrderForCheking = ord;
            ButtonNumber = butt;
        }


    }

    WhichButton WhatButtonPressed;


    public  void Init()
    {
        SpellButton1 = GameObject.Find("spell1").GetComponent<Button>();
        //SpellButton1.onClick.AddListener(Button1Pressed);

        SpellButton2 = GameObject.Find("spell2").GetComponent<Button>();
        //SpellButton2.onClick.AddListener(Button2Pressed);
        
        SpellButton3 = GameObject.Find("spell3").GetComponent<Button>();
        //SpellButton3.onClick.AddListener(Button3Pressed);

        SpellButton4 = GameObject.Find("spell4").GetComponent<Button>();
        //SpellButton4.onClick.AddListener(Button4Pressed);

        SpellButton5 = GameObject.Find("spell5").GetComponent<Button>();
        //SpellButton5.onClick.AddListener(Button5Pressed);

        SpellButton6 = GameObject.Find("spell6").GetComponent<Button>();
        //SpellButton6.onClick.AddListener(Button6Pressed);

                
        SpellButton1.image.sprite = DB.GetSpellByNumber(general.DataForSession[0].Spell1).Spell1_icon;
        SpellButton2.image.sprite = DB.GetSpellByNumber(general.DataForSession[0].Spell2).Spell1_icon;
        SpellButton3.image.sprite = DB.GetSpellByNumber(general.DataForSession[0].Spell3).Spell1_icon;
        SpellButton4.image.sprite = DB.GetSpellByNumber(general.DataForSession[0].Spell4).Spell1_icon;
        SpellButton5.image.sprite = DB.GetSpellByNumber(general.DataForSession[0].Spell5).Spell1_icon;
        SpellButton6.image.sprite = DB.GetSpellByNumber(general.DataForSession[0].Spell6).Spell1_icon;
    }


    public void Button1Pressed()
    {
        if (GetButton(1).interactable)
        {
            //CheckTouchForStrafe.isButtonTouched = true;
            playercontrol.isButtonSend = true;
            playercontrol.ButtonMessToSend = SendAndReceive.DataForSending.ToSendButtons(playercontrol.AgregateHoriz, playercontrol.AgregateVertic, 1, 0, 0, 0, 0, 0);
            WhatButtonPressed.set(SendAndReceive.DataForSending.OrderToSend, 1);
        }
    }

    public  void Button2Pressed()
    {
        if (GetButton(2).interactable)
        {
            
            playercontrol.isButtonSend = true;
            playercontrol.ButtonMessToSend = SendAndReceive.DataForSending.ToSendButtons(playercontrol.AgregateHoriz, playercontrol.AgregateVertic, 0, 1, 0, 0, 0, 0);
            WhatButtonPressed.set(SendAndReceive.DataForSending.OrderToSend, 2);
        }
    }

    public  void Button3Pressed()
    {
        if (GetButton(3).interactable)
        {
            
            playercontrol.isButtonSend = true;
            playercontrol.ButtonMessToSend = SendAndReceive.DataForSending.ToSendButtons(playercontrol.AgregateHoriz, playercontrol.AgregateVertic, 0, 0, 1, 0, 0, 0);
            WhatButtonPressed.set(SendAndReceive.DataForSending.OrderToSend, 3);
        }
    }

    public  void Button4Pressed()
    {
        if (GetButton(4).interactable)
        {
            
            playercontrol.isButtonSend = true;
            playercontrol.ButtonMessToSend = SendAndReceive.DataForSending.ToSendButtons(playercontrol.AgregateHoriz, playercontrol.AgregateVertic, 0, 0, 0, 1, 0, 0);
            WhatButtonPressed.set(SendAndReceive.DataForSending.OrderToSend, 4);
        }
    }

    public void Button5Pressed()
    {
        if (GetButton(5).interactable)
        {
            
            playercontrol.isButtonSend = true;
            playercontrol.ButtonMessToSend = SendAndReceive.DataForSending.ToSendButtons(playercontrol.AgregateHoriz, playercontrol.AgregateVertic, 0, 0, 0, 0, 1, 0);
            WhatButtonPressed.set(SendAndReceive.DataForSending.OrderToSend, 5);
        }
    }

    public void Button6Pressed()
    {
        if (GetButton(6).interactable)
        {

            playercontrol.isButtonSend = true;
            playercontrol.ButtonMessToSend = SendAndReceive.DataForSending.ToSendButtons(playercontrol.AgregateHoriz, playercontrol.AgregateVertic, 0, 0, 0, 0, 0, 1);
            WhatButtonPressed.set(SendAndReceive.DataForSending.OrderToSend, 6);
        }
    }


    public IEnumerator CurrentButtonCooldown()
    {
        if (unique_codes.Contains(SendAndReceive.UniqCode))
        {
            yield break;
        }

        unique_codes.Add(SendAndReceive.UniqCode);

        int number_of_button = 0;
        int spell_index = SendAndReceive.SpellIndex;

        if (spell_index == general.DataForSession[0].Spell1) number_of_button = 1;
        if (spell_index == general.DataForSession[0].Spell2) number_of_button = 2;
        if (spell_index == general.DataForSession[0].Spell3) number_of_button = 3;
        if (spell_index == general.DataForSession[0].Spell4) number_of_button = 4;
        if (spell_index == general.DataForSession[0].Spell5) number_of_button = 5;
        if (spell_index == general.DataForSession[0].Spell6) number_of_button = 6;

        Button CurrentButton = GetButton(number_of_button);

        if (CurrentButton != null)
        {
            float CoolDown = SendAndReceive.SpellCooldown;
            
            Image CurrentButtonImage = CurrentButton.gameObject.GetComponent<Image>();

            CurrentButtonImage.fillAmount = 0;
            CurrentButton.interactable = false;

            for (float i = 0; i < CoolDown; i += 0.1f)
            {
                CurrentButtonImage.fillAmount = i / CoolDown;
                yield return new WaitForSeconds(0.1f);
            }

            CurrentButtonImage.fillAmount = 1;
            CurrentButton.interactable = true;
            CurrentButton = null;
        }
    }



    public IEnumerator buttoncooldown()
    {

        
        Button CurrentButton = GetButton(WhatButtonPressed.ButtonNumber);
        if (CurrentButton != null)
        {
            float CoolDown = SendAndReceive.SpellCooldown;
            if (CoolDown == 0 && SendAndReceive.ButtonState==1 )
            {
                CoolDown = 0.7f;
            }
            Image CurrentButtonImage = CurrentButton.gameObject.GetComponent<Image>();


            WhatButtonPressed.set(0, 0);

            CurrentButtonImage.fillAmount = 0;
            CurrentButton.interactable = false;

            for (float i = 0; i < CoolDown; i += 0.1f)
            {
                CurrentButtonImage.fillAmount = i / CoolDown;
                yield return new WaitForSeconds(0.1f);
            }

            CurrentButtonImage.fillAmount = 1;
            CurrentButton.interactable = true;
            CurrentButton = null;
        }

    }

    

    private Button GetButton(int numb)
    {
    Button result = null;
    switch (numb)
    {
        case 1:
            result = SpellButton1;
            break;
        case 2:
            result = SpellButton2;
            break;
        case 3:
            result = SpellButton3;
            break;
        case 4:
            result = SpellButton4;
            break;
        case 5:
            result = SpellButton5;
            break;
        case 6:
            result = SpellButton6;
            break;
    }
    return result;
    }

}



public class ObjectPooling : MonoBehaviour
{
    public GameObject[] Objects;

    public ObjectPooling(int Index, GameObject Example, Transform Storage)
    {
        Objects = new GameObject[Index];

        for (int i = 0; i < Objects.Length; i++)
        {
            Objects[i] = Instantiate(Example, Storage);
            Objects[i].SetActive(false);
        }
    }

    public GameObject GetObject()
    {
        GameObject result = Objects[0];
        for (int i = 0; i < Objects.Length; i++)
        {
            if (!Objects[i].activeSelf)
            {
                Objects[i].SetActive(true);
                return Objects[i];
            }
        }

        return result;

    }
}

/*
public static class CheckTouchForStrafe
{    
    public static bool isNowhereTouched;
}
*/
