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


public class Conds
{
    public string cond_id;
    public string cond_type;
    public float damage_or_heal;
    public string cond_bulk;
    public string cond_message;
    public bool isCrit;
    public int spell_index;
    public float cond_time;
    public bool isChecked;
    public float coord_x, coord_z;
}


public class ConditionsAnalys
{

    int Limit = general.SessionNumberOfPlayers * 10;

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
                        }
                    }

                    if (!isNegative)
                    {
                        Decode(getstrcond1[i]);

                    }
                }
            }
        }
    }

  
    public void Decode(string Data)
    {
        
        curr_conds.Add(new Conds());
        int Index = curr_conds.Count - 1;
        string[] getstrcond1 = Data.Split(':');
        
        curr_conds[Index].cond_id = getstrcond1[0];

        
        curr_conds[Index].cond_bulk = getstrcond1[1];
        
        
        string[] getstrcond = getstrcond1[1].Split('-');
        curr_conds[Index].cond_type = getstrcond[0];

        if (curr_conds[Index].cond_type == "co") //condition type in conditions
        {
            curr_conds[Index].spell_index = int.Parse(getstrcond[1]);
            curr_conds[Index].cond_time = float.Parse(getstrcond[2], CultureInfo.InvariantCulture);            
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
        else if (curr_conds[Index].cond_type == "me") //messages of conditions
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
            }
            else
            {
                curr_conds[Index].spell_index = int.Parse(getstrcond[1]);
                curr_conds[Index].cond_time = float.Parse(getstrcond[2], CultureInfo.InvariantCulture);
            }

            

        } else if (curr_conds[Index].cond_type.Contains("="))
        {
            string[] anothergetstrcond = getstrcond1[1].Split('=');
            curr_conds[Index].cond_type = anothergetstrcond[0];
            if (curr_conds[Index].cond_type == "cs") //spell bolt flying around
            {
                curr_conds[Index].spell_index = int.Parse(anothergetstrcond[1]);
                curr_conds[Index].coord_x = float.Parse(anothergetstrcond[2], CultureInfo.InvariantCulture);
                curr_conds[Index].coord_z = float.Parse(anothergetstrcond[3], CultureInfo.InvariantCulture);

            }
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

        if (general.MainPlayerClass == 1)
        {
            //BasicMovement = Resources.Load<AudioClip>("sounds/heavy movement");
            BasicWeaponHit = Resources.Load<AudioClip>("sounds/onehand swing bigger");
        }
    }

    public void RefreshAnimations(int state)
    {
        if ((PrevAnimationState==3 || PrevAnimationState==8 || PrevAnimationState == 10) && (state==1 || state==0) )
        {
            Idle();
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && CurrentAnimationState != 0)
        {
            CurrentAnimationState = 0;
        }
        else if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Run") || animator.GetCurrentAnimatorStateInfo(0).IsName("Runback")) && CurrentAnimationState != 1)
        {
            CurrentAnimationState = 1;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run") && playercontrol.MyJoystick.Vertical < 0 && CurrentAnimationState==1)
        {
            animator.Play("Runback");
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Runback") && playercontrol.MyJoystick.Vertical >= 0 && CurrentAnimationState == 1)
        {
            animator.Play("Run");
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
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("HitWith1H"))
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
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("ShieldOn"))
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
            }
        }

        PrevAnimationState = CurrentAnimationState;
    }

    void Idle()
    {
        
        animator.Play("Idle");
        CurrentAnimationState = 0;
        //MyAudioSource.Stop();
        //MyAudioSource.clip = null;
        
    }

    void Run()
    {
        /*
        if (MyAudioSource.clip != BasicMovement)
        {
            MyAudioSource.loop = true;
            MyAudioSource.clip = BasicMovement;
            MyAudioSource.Play();
        }
        */
        if (playercontrol.MyJoystick.Vertical >= 0)
        {
            animator.Play("Run");
        } else
        {
            animator.Play("Runback");
        }
        CurrentAnimationState = 1;
    }

    void HitWith1H()
    {
        animator.Play("HitWith1H");
        CurrentAnimationState = 2;
        MyAudioSource.loop = false;
        MyAudioSource.clip = BasicWeaponHit;
        MyAudioSource.Play();
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

    void Death()
    {
        animator.Play("Death");
        CurrentAnimationState = 4;
        //CurrentAnimationState = 4;
        //StartCoroutine(CheckEnd("ReceiveDamage"));
    }

    

}


public struct ReceivePlayersData
{
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
        
        try
        {
            getstr = ReceivedPacket.Split('~');
            position_x = float.Parse(getstr[0], CultureInfo.InvariantCulture);
            position_y = float.Parse(getstr[1], CultureInfo.InvariantCulture);
            position_z = float.Parse(getstr[2], CultureInfo.InvariantCulture);
            rotation_x = float.Parse(getstr[3], CultureInfo.InvariantCulture);
            rotation_y = float.Parse(getstr[4], CultureInfo.InvariantCulture);
            rotation_z = float.Parse(getstr[5], CultureInfo.InvariantCulture);
            speed = float.Parse(getstr[6], CultureInfo.InvariantCulture);
            animation_id = int.Parse(getstr[7]);
            conditions = getstr[8];
            
            all_health = getstr[9].Split('=');
            health_pool = float.Parse(all_health[0], CultureInfo.InvariantCulture);
            max_health_pool = float.Parse(all_health[1], CultureInfo.InvariantCulture);
            
            energy = float.Parse(getstr[10], CultureInfo.InvariantCulture);
            position = new Vector3(position_x, position_y, position_z);
            rotation = new Vector3(rotation_x, rotation_y, rotation_z);

            
            
        }
        catch (Exception ex)
        {
            Debug.Log("wrong packet format - " + ReceivedPacket);
            //if (ReceivedPacket!=null) playercontrol.OtherLatency1.text = playercontrol.OtherLatency1.text + "wrong packet format - " + ReceivedPacket;
        }

        
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
        StringBuilder Result = new StringBuilder(70);
        OrderToSend++;
        Result.Append(OrderToSend.ToString() + "~0~" + PlayerID + "~" + TemporaryTable + "~" + HorizontalTouch.ToString("f2").Replace(',', '.') + "~" + VerticalTouch.ToString("f2").Replace(',', '.') + "~0~0~0~0~0~0|"); //+ "~0~0~0~0~0"
        
        return Result.ToString();
    }

    public string ToSendButtons(int But1, int But2, int But3, int But4, int But5, int But6)
    {
        MakeClean();
        PlayerID = general.SessionPlayerID;
        TemporaryTable = general.SessionTicket;
        Butt1 = But1;
        Butt2 = But2;
        Butt3 = But3;
        Butt4 = But4;
        Butt5 = But5;
        Butt6 = But6;
        StringBuilder Result = new StringBuilder(70);
        OrderToSend++;
        Result.Append(OrderToSend.ToString() + "~1~" + PlayerID + "~" + TemporaryTable + "~0~0~" + Butt1.ToString() + "~" + Butt2.ToString() + "~" + Butt3.ToString() + "~" + Butt4.ToString() + "~" + Butt5.ToString() + "~" + Butt6.ToString() + "|");
        
        return Result.ToString();
    }

}

public static class SendAndReceive
{
    public static ToSend DataForSending;
    public static ReceivePlayersData MyPlayerData;
    public static ReceivePlayersData[] OtherPlayerData = new ReceivePlayersData[general.SessionNumberOfPlayers - 1];
    public static int OrderReceived;
    public static int SpecificationReceived;
    public static int ButtonState;
    public static string MessageType;
    public static float SpellCooldown;
    public static int SwButtonCond;



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
            SpellCooldown = float.Parse(getstr[4], CultureInfo.InvariantCulture);            
        }
    }

    public static void TranslateDataFromServer(string ReceivedData)
    {
        string[] getstr = new string[general.SessionNumberOfPlayers + 1];


        try
        {
            getstr = ReceivedData.Split('^');
            //1
            GetHeader(getstr[0]);
            if (SpecificationReceived == 0)
            {
                //2
                MyPlayerData.ReceiveForPlayers(getstr[1]);
                //... ...
                for (int i = 0; i < (general.SessionNumberOfPlayers - 1); i++)
                {
                    OtherPlayerData[i].ReceiveForPlayers(getstr[2 + i]);
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
    private GameObject cond_example;
    private float HealthCurrentAmount = 0;
    private float EnergyCurrentAmount = 0;
    private float HealthAllAmount = 0;
    private float EnergyAllAmount = 100;
    private Image HealthButton;
    private Image EnergyButton;

    private Image CastingBar;
    private Image CastingSpellImage;

    private int CondObjectLenth = 21;
    public bool isMainPlayer;

    public bool isCasting = false;
    private TextMeshProUGUI CancelationText;
    private bool isShowStopCastingText;

    public class CondManager
    {
        public string cond_id;
        public string cond_sprite;
        public GameObject con_object;
        public bool isBusy;
        public int spell_index;
        public float spell_timer;
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


    private Vector2[] CondPositionsForOtherPlayer = {
        new Vector2(0,-65),
        new Vector2(40,-65),
        new Vector2(80,-65),
        new Vector2(120,-65),
        new Vector2(160,-65),
        new Vector2(200,-65),
        new Vector2(240,-65),
        
        
        //=============================
        new Vector2(0,-120),
        new Vector2(40,-120),
        new Vector2(80,-120),
        new Vector2(120,-120),
        new Vector2(160,-120),
        new Vector2(200,-120),
        new Vector2(240,-120),

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
            CondObjects[i].con_object = Instantiate(cond_example, Vector3.zero, Quaternion.identity, AllObject.transform);
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
        CancelationText.text = lang.Canceled;
    }

    public void StopCurrentCasting()
    {
        isShowStopCastingText = true;
        CastingBar.gameObject.SetActive(false);
        CastingSpellImage.gameObject.SetActive(false);
        isCasting = false;
        CancelationText.gameObject.SetActive(false);
    }

    public IEnumerator AddCasting(string castID, int spell_ind, float spell_time)
    {
            
        isCasting = true;
        CastingBar.gameObject.SetActive(true);
        CastingSpellImage.gameObject.SetActive(true);
        CastingBar.fillAmount = 1;
        CastingSpellImage.sprite = DB.GetSpellByNumber(spell_ind).Spell1_icon;

        float delta = 1 / (spell_time / 0.1f);

        for (float i = spell_time; i > 0; i -= 0.1f)
        {
            CastingBar.fillAmount -=delta;
            if (isShowStopCastingText)
            {
                CancelationText.gameObject.SetActive(true);
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }

        isCasting = false;
        CastingBar.gameObject.SetActive(false);
        CastingSpellImage.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        
        isShowStopCastingText = false;
        CancelationText.gameObject.SetActive(false);
    }


    public IEnumerator AddCondition(string condID, int spell_ind, float spell_time)
    {
        print(condID + ": " + spell_ind + " - ");
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
                
                CondObjects[index].con_object.SetActive(true);
                CondObjects[index].con_object.GetComponent<RectTransform>().anchoredPosition = position;
                CondObjects[index].con_object.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = CondObjects[index].spell_timer.ToString();
                CondObjects[index].con_object.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = DB.GetSpellByNumber(spell_ind).Spell1_icon;

                

                do
                {
                    yield return new WaitForSeconds(0.1f);
                    
                    CondObjects[index].spell_timer -= 0.1f;
                    CondObjects[index].con_object.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = CondObjects[index].spell_timer.ToString("f0");
                    
                } while (CondObjects[index].spell_timer > 0);

                //string texter = CondObjects[index].con_object.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text;
                Vector2 expCoords = CondObjects[index].con_object.GetComponent<RectTransform>().anchoredPosition;

                CondObjects[index].con_object.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                CondObjects[index].isBusy = false;
                CondObjects[index].con_object.SetActive(false);
                CondObjects[index].con_object.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "";
                CondObjects[index].con_object.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = null;

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
        SpellButton1.onClick.AddListener(Button1Pressed);

        SpellButton2 = GameObject.Find("spell2").GetComponent<Button>();
        SpellButton2.onClick.AddListener(Button2Pressed);

        SpellButton3 = GameObject.Find("spell3").GetComponent<Button>();
        SpellButton3.onClick.AddListener(Button3Pressed);

        SpellButton4 = GameObject.Find("spell4").GetComponent<Button>();
        SpellButton4.onClick.AddListener(Button4Pressed);

        SpellButton5 = GameObject.Find("spell5").GetComponent<Button>();
        SpellButton5.onClick.AddListener(Button5Pressed);

        SpellButton6 = GameObject.Find("spell6").GetComponent<Button>();
        SpellButton6.onClick.AddListener(Button6Pressed);

        SpellButton1.image.sprite = DB.GetSpellByNumber(general.DataForSession[0].Spell1).Spell1_icon;
        SpellButton2.image.sprite = DB.GetSpellByNumber(general.DataForSession[0].Spell2).Spell1_icon;
        SpellButton3.image.sprite = DB.GetSpellByNumber(general.DataForSession[0].Spell3).Spell1_icon;
        SpellButton4.image.sprite = DB.GetSpellByNumber(general.DataForSession[0].Spell4).Spell1_icon;
        SpellButton5.image.sprite = DB.GetSpellByNumber(general.DataForSession[0].Spell5).Spell1_icon;
        SpellButton6.image.sprite = DB.GetSpellByNumber(general.DataForSession[0].Spell6).Spell1_icon;
    }

    private void Button1Pressed()
    {
        if (GetButton(1).interactable)
        {
           
            playercontrol.isButtonSend = true;
            playercontrol.ButtonMessToSend = SendAndReceive.DataForSending.ToSendButtons(1, 0, 0, 0, 0, 0);
            WhatButtonPressed.set(SendAndReceive.DataForSending.OrderToSend, 1);
        }
    }

    private  void Button2Pressed()
    {
        if (GetButton(2).interactable)
        {
            
            playercontrol.isButtonSend = true;
            playercontrol.ButtonMessToSend = SendAndReceive.DataForSending.ToSendButtons(0, 1, 0, 0, 0, 0);
            WhatButtonPressed.set(SendAndReceive.DataForSending.OrderToSend, 2);
        }
    }

    private  void Button3Pressed()
    {
        if (GetButton(3).interactable)
        {
            
            playercontrol.isButtonSend = true;
            playercontrol.ButtonMessToSend = SendAndReceive.DataForSending.ToSendButtons(0, 0, 1, 0, 0, 0);
            WhatButtonPressed.set(SendAndReceive.DataForSending.OrderToSend, 3);
        }
    }

    private  void Button4Pressed()
    {
        if (GetButton(4).interactable)
        {
            
            playercontrol.isButtonSend = true;
            playercontrol.ButtonMessToSend = SendAndReceive.DataForSending.ToSendButtons(0, 0, 0, 1, 0, 0);
            WhatButtonPressed.set(SendAndReceive.DataForSending.OrderToSend, 4);
        }
    }

    private void Button5Pressed()
    {
        if (GetButton(5).interactable)
        {
            
            playercontrol.isButtonSend = true;
            playercontrol.ButtonMessToSend = SendAndReceive.DataForSending.ToSendButtons(0, 0, 0, 0, 1, 0);
            WhatButtonPressed.set(SendAndReceive.DataForSending.OrderToSend, 5);
        }
    }

    private void Button6Pressed()
    {
        if (GetButton(6).interactable)
        {

            playercontrol.isButtonSend = true;
            playercontrol.ButtonMessToSend = SendAndReceive.DataForSending.ToSendButtons(0, 0, 0, 0, 0, 1);
            WhatButtonPressed.set(SendAndReceive.DataForSending.OrderToSend, 6);
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
                CoolDown = 0.5f;
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