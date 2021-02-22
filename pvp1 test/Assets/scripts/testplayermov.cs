using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Globalization;

public class testplayermov : MonoBehaviour
{
    private FixedJoystick MyJoystickTemp;
    
    public static TextMeshProUGUI OtherLatency1;
    public static string ToOtherLatency;
    public static bool isPacketReceived;
    public static string RawPacketReceived;
    public static List<string> RawPackets = new List<string>();

    List<Text> texts = new List<Text>(10);

    private Transform PlayerTransform;
    //private Animator PlayerAnimator;
    AnimationsForPlayers myanimator;
    Button butt1;
    Button butt2;
    Button butt3;
    Button butt4;
    Button buttswitch;
    private static Joysticks MyJoystick;
    private string ButtonDataToSend;
    //public static List<players> OtherGamers = new List<players>(general.SessionNumberOfPlayers - 1);
    private float AgregateHoriz, AgregateVertic;
    Vector3 speed = Vector3.zero;

    public GameObject OtherPlayerPrefab;
    float cur_time, cur_time_2, koeff_ALL = 1, whatisplus, missdirectionPos, missdirectionRot, cur_speed;

    string MyConditions, HisConditions;

    Vector3 PrevPos, Player2NewPos, Player2NewRot, CorrectionForPosition, CorrectionForRotation, posspeed, little_pos, little_rot;

    //koeffs and counts=======================================================
    List<float> CountsBeforeServerReq = new List<float>(12);
    float CountSumForCounts, DeltaForLerpMovingNRotation, AverageCountForDeltaForLerp, AverageCountForUpdate;

    float koeffpos = 1f; //1.6
    float koeffrot = 1f; //1.6
    //float ticker_hor, ticker_ver, previous_hor, previous_ver, how_many;

    bool isOnce;

    //latency
    float latency_timer;
    int latency_check, button_order, conditions_order;
    bool isLatency;
    float CountForQuit;
    public static List<float> LatencyMain = new List<float>(12);

    float yyy = 0, oldyyy, gooodyyy;
    Vector3 distt;

    void Start()
    {


        Screen.SetResolution(1280, 720, true);
        Camera.main.aspect = 16f / 9f;
        Application.targetFrameRate = 60;
        PlayerTransform = this.GetComponent<Transform>();
        //PlayerAnimator = this.GetComponent<Animator>();
        myanimator = new AnimationsForPlayers(this.GetComponent<Animator>(), this.GetComponent<AudioSource>());
        MyJoystickTemp = GameObject.Find("Fixed Joystick control").GetComponent<FixedJoystick>();

        PlayerTransform.position = Vector3.zero;
        PlayerTransform.rotation = Quaternion.Euler(Vector3.zero);
        
        //text messages
        GameObject abra = GameObject.Find("InfoText");
        for (int i = 0; i < 10; i++)
        {
            GameObject temp = Instantiate(abra, Vector3.zero, Quaternion.identity, GameObject.Find("CanvasUI").transform);
            texts.Add(temp.GetComponent<Text>());
        }
        //=====================
        //connection.connector.TalkToServer("121212");


    }





    // Update is called once per frame
    void Update()
    {
        

        MyJoystick.CheckTouches(MyJoystickTemp.Vertical, MyJoystickTemp.Horizontal);


        if (Input.GetKeyDown(KeyCode.I))
        {
            MakeSign("проверка связи", new Vector2(200, 200));
            /*
            for (int i = 0; i < OtherGamers[0].Conds.curr_conds.Count; i++)
            {
                print(OtherGamers[0].Conds.curr_conds[i].cond_id + " - " + OtherGamers[0].Conds.curr_conds[i].cond_bulk);
            }
            */
        }

        //ANIMATIONS===========================================================
        //print("my - " + myanimator.CurrentAnimationState +" - "+ SendAndReceive.MyPlayerData.animation_id + " - "  + myanimator.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        myanimator.RefreshAnimations(1);
        //if (myanimator.CurrentAnimationState!=SendAndReceive.MyPlayerData.animation_id)
        //{
        //    myanimator.RefreshAnimations(SendAndReceive.MyPlayerData.animation_id);
        //}
        //ANIMATIONS===========================================================




        //==================================================================

        if (cur_time >= 0.04f)
        {


            AgregateHoriz = 0;
            AgregateVertic = 0;





            //COUNTS FOR LERP           
            CountsBeforeServerReq.Add(CountSumForCounts);
            AverageCountForDeltaForLerp = AverageSumOfList();
            CountSumForCounts = 0;

            cur_time = 0;

        }
        else
        {
            cur_time += Time.deltaTime;

        }

        //==================================================================



        AgregateHoriz += MyJoystick.Horizontal;
        AgregateVertic += MyJoystick.Vertical;

        PredictionMachine(MyJoystick.Horizontal,
            MyJoystick.Vertical, PlayerTransform.position, PlayerTransform.rotation.eulerAngles,
            out Player2NewPos, out Player2NewRot);

        CountSumForCounts++;

        DeltaForLerpMovingNRotation = CountSumForCounts / AverageCountForDeltaForLerp;

        /*
        if (CountSumForCounts==1 && AgregateHoriz==0)
        {
            CorrectionForRotation = Vector3.zero;
            PlayerTransform.rotation = Quaternion.Euler(SendAndReceive.MyPlayerData.rotation);
        }

        if (CountSumForCounts == 1 && AgregateVertic == 0)
        {
            CorrectionForPosition = Vector3.zero;
            PlayerTransform.position = SendAndReceive.MyPlayerData.position;
        }
        */

        PlayerTransform.position = Vector3.Lerp(PlayerTransform.position, (Player2NewPos ), DeltaForLerpMovingNRotation);

       
        PlayerTransform.rotation = Quaternion.Lerp(PlayerTransform.rotation, Quaternion.Euler(Player2NewRot ), DeltaForLerpMovingNRotation);

       

    }



    float AverageLatency()
    {
        float result = 0;

        if (LatencyMain.Count > 10)
        {
            LatencyMain.Remove(LatencyMain[0]);
        }
        for (int i = 0; i < LatencyMain.Count; i++)
        {
            result += LatencyMain[i];
        }


        return result = result / (float)LatencyMain.Count;
    }

    float AverageSumOfList()
    {
        float result = 0;

        if (CountsBeforeServerReq.Count > 10)
        {
            CountsBeforeServerReq.Remove(CountsBeforeServerReq[0]);
        }
        for (int i = 0; i < CountsBeforeServerReq.Count; i++)
        {
            result += CountsBeforeServerReq[i];
        }

        return result = result / (float)CountsBeforeServerReq.Count;
    }




    void PredictionMachine(float Horizontal, float Vertical, Vector3 old_pos, Vector3 old_rot, out Vector3 position_delta, out Vector3 rotation_delta)
    {


        float degree_to_radian = 0.0174532924f;
        float delta_for_rotation = 4f;
        float delta_for_position = 0.4f;



        Vector3 PreviousPosition = old_pos;
        Vector3 PreviousRotation = old_rot;

        //horizontal touch and rotation
        float new_rotation_x = PreviousRotation.x;
        float new_rotation_y = PreviousRotation.y + Horizontal * delta_for_rotation * koeffrot;
        float new_rotation_z = PreviousRotation.z;

        if (new_rotation_y >= 360)
        {
            new_rotation_y = new_rotation_y - 360;
        }
        if (new_rotation_y < 0)
        {
            new_rotation_y = 360 - new_rotation_y;
        }


        //vertical touch and movement
        float new_position_x = PreviousPosition.x + Mathf.Sin(PreviousRotation.y * degree_to_radian) * Mathf.Cos(PreviousRotation.x * degree_to_radian) * delta_for_position * Vertical * koeffpos;
        float new_position_y = PreviousPosition.y + Mathf.Sin(-PreviousRotation.x * degree_to_radian) * delta_for_position * Vertical * koeffpos;
        float new_position_z = PreviousPosition.z + Mathf.Cos(PreviousRotation.x * degree_to_radian) * Mathf.Cos(PreviousRotation.y * degree_to_radian) * delta_for_position * Vertical * koeffpos;

        position_delta = new Vector3(new_position_x, new_position_y, new_position_z);
        rotation_delta = new Vector3(new_rotation_x, new_rotation_y, new_rotation_z);

    }



    public void MakeSign(string message_text, Vector2 position)
    {
        Text TakenObject = null;
        bool isFound = false;
        print(texts.Count);
        for (int i = 0; i < texts.Count; i++)
        {
            if (texts[i].text == "" || texts[i].text == null)
            {
                TakenObject = texts[i];
                isFound = true;
                i = 5;
            }
        }

        if (!isFound)
        {
            TakenObject = texts[0];
        }

        TakenObject.text = message_text;

        StartCoroutine(ShowMessage(TakenObject, position));

    }


    IEnumerator ShowMessage(Text mess_window, Vector2 position)
    {
        for (float i = 0; i < 10; i++)
        {
            mess_window.rectTransform.anchoredPosition = new Vector2(position.x, position.y + i * 10f);
            yield return new WaitForSeconds(0.1f);
        }
        mess_window.text = "";
    }


}
