using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Globalization;
using UnityEngine.EventSystems;
//using UnityEngine.UIElements;

public class playercontrol : MonoBehaviour
{
    private bool isStart;

    //Joystick data from Canvas
    private FloatingJoystick MyJoystickTemp;
    public static Joysticks MyJoystick;

    public TextMeshProUGUI TempText1, TempText2, MyHPText, OtherLatency, DataAnalys, Temporary;
    //public static TextMeshProUGUI OtherLatency1;
    public static string ToOtherLatency;    
    
    //main packets for geting data to player from connection
    public static List<string> RawPackets = new List<string>();
    
    //messages from damage and conds
    List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>(10);
    List<Touch> touches = new List<Touch>();

    //My Player
    private PlayerUI MyUI;
    private GameObject MainPlayerGameObject;
    private effects MyEffects;

    //conditions of myPlayer
    private ConditionsAnalys MyConds = new ConditionsAnalys();

    //private ConditionsAnalys OtherPlayerConds = new ConditionsAnalys();
    

    //main player transform
    private Transform PlayerTransform;
    private float AgregateHoriz, AgregateVertic;

    //private Animator PlayerAnimator;
    AnimationsForPlayers myanimator;
    
    //other players data    
    public static List<players> OtherGamers;
    
    float cur_time, cur_time_2, cur_speed;

    Vector3 PrevPos, Player2NewPos, Player2NewRot, CorrectionForPosition, CorrectionForRotation, little_pos, little_rot;

    public static Vector3 PlayerCurrentSpeed;

    //koeffs and counts=======================================================
    List<float> CountsBeforeServerReq = new List<float>(12);
    float CountSumForCounts, DeltaForLerpMovingNRotation, AverageCountForDeltaForLerp, DeltaForRotOnly;

    float koeffpos = 3f; //1.6
    float koeffrot = 1f; //1.6
    //float ticker_hor, ticker_ver, previous_hor, previous_ver, how_many;

    
    //latency
    float latency_timer;
    int latency_check, button_order, conditions_order;
    bool isLatency;
    float ResultDelta;
    public static List<float> LatencyMain = new List<float>(12);    

    private Vector3 speed = Vector3.zero;    
    private float rotAngle, sighAngle;
    private Camera CurrentCamera;

    //
    private bool isMessageShowing;
    Buttons ButtonsManagement = new Buttons();

    private TextMeshProUGUI ButtonMessage;
    List<string> PoolOfMessages = new List<string>();

    //regulating send and receive
    public static bool isSendingOK = true;


    //button sending MODE
    public static bool isButtonSend;
    public static string ButtonMessToSend;


    void Start()
    {

        string SessionResult = connection.SendAndGetTCP(SessionData.SendSessionDataRequest());
        SessionData.GetDataSessionPlayers(SessionResult);
        general.SetSessionData();
        
        if (general.DataForSession.Count > 0)
        {
            for (int i = 0; i < general.DataForSession.Count; i++)
            {
                print(general.DataForSession[i].PlayerName + " - " + general.DataForSession[i].PlayerClass + " - " + general.DataForSession[i].PlayerOrder);
            }
        }
        

        ButtonsManagement.Init();
        
        Screen.SetResolution(1280, 720, true);
        Camera.main.aspect = 16f / 9f;
        
        PlayerTransform = this.GetComponent<Transform>();

        
        MainPlayerGameObject = Instantiate(general.GetPlayerByClass(general.MainPlayerClass), Vector3.zero, Quaternion.identity, this.gameObject.transform);
        MainPlayerGameObject.transform.localPosition = Vector3.zero;
        MainPlayerGameObject.GetComponent<players>().enabled = false;
        
        myanimator = new AnimationsForPlayers(MainPlayerGameObject.GetComponent<Animator>(), MainPlayerGameObject.GetComponent<AudioSource>());
        MyJoystickTemp = GameObject.Find("Floating Joystick").GetComponent<FloatingJoystick>();
        CurrentCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        
        DataAnalys = GameObject.Find("CheckData").GetComponent<TextMeshProUGUI>();

        for (int i = 0; i < 10; i++)
        {
            GameObject temp = Instantiate(GameObject.Find("InfoText"), Vector3.zero, Quaternion.identity, GameObject.Find("CanvasUI").transform);
            texts.Add(temp.GetComponent<TextMeshProUGUI>());
        }

        //=====================

        OtherGamers = new List<players>(general.SessionNumberOfPlayers - 1);
        for (int i = 1; i <= general.SessionNumberOfPlayers-1; i++)
        {
            AddPlayer(Vector3.zero, Vector3.zero, i);
        }
        
        StartCoroutine(SyncForPosition());

        GameObject temp1 = Instantiate(Resources.Load<GameObject>("prefabs/playerUI"), new Vector3(0, 0, 0), Quaternion.identity, GameObject.Find("CanvasInterface").transform);
        temp1.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, -20);
        temp1.SetActive(true);
        MyUI = new PlayerUI(temp1, true);

        MyEffects = MainPlayerGameObject.GetComponent<effects>();
        MyEffects.MyPlayerClass = general.MainPlayerClass;
        
        ButtonMessage = GameObject.Find("ButtonMessages").GetComponent<TextMeshProUGUI>();

    }


    void AddPlayer(Vector3 pos, Vector3 rot, int order)
    {
        //print(general.DataForSession[order].PlayerClass + "================================");
        int WhatPlayersClass = general.DataForSession[order].PlayerClass;
        GameObject ggg = Instantiate(general.GetPlayerByClass(WhatPlayersClass), Vector3.zero, Quaternion.identity, GameObject.Find("OtherPlayers").transform);
        ggg.GetComponent<effects>().MyPlayerClass = WhatPlayersClass;
        ggg.GetComponent<players>().NumberInSendAndReceive = (order-1);
        ggg.GetComponent<players>().OtherPlayerName = general.DataForSession[order].PlayerName;
        ggg.GetComponent<effects>().PlayerSessionData = general.DataForSession[order];
        ggg.GetComponent<players>().CreateUI();
        OtherGamers.Add(ggg.GetComponent<players>());

        

    }


    

    // Update is called once per frame
    void Update()
    {

        

        if (RawPackets.Count > 0)
        {
            SendAndReceive.TranslateDataFromServer(RawPackets[0]);
            RawPackets.Remove(RawPackets[0]);
        }


        MyConds.Check(SendAndReceive.MyPlayerData.conditions);
        //OtherPlayerConds.Check(SendAndReceive.OtherPlayerData[0].conditions);

        for (int i = 0; i < OtherGamers.Count; i++)
        {
            OtherGamers[i].Conds.Check(SendAndReceive.OtherPlayerData[i].conditions);
            OtherGamers[i].OtherPlayerUI.HealthInput(SendAndReceive.OtherPlayerData[0].health_pool, SendAndReceive.OtherPlayerData[0].max_health_pool);
            OtherGamers[i].OtherPlayerUI.EnergyInput(SendAndReceive.OtherPlayerData[0].energy);
        }


        //print(SendAndReceive.OrderReceived + "---------------============ \n");

        MyJoystick.CheckTouches(MyJoystickTemp.Vertical, MyJoystickTemp.Horizontal);


        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.touchCount > 0 && (Input.GetTouch(i).phase == TouchPhase.Began || Input.GetTouch(i).phase == TouchPhase.Moved || Input.GetTouch(i).phase == TouchPhase.Stationary))
                {

                    if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                    {
                        if (!CheckTouchForStrafe.isNowhereTouched) CheckTouchForStrafe.isNowhereTouched = true;
                    }
                }

            }
        } else
        {
            if (CheckTouchForStrafe.isNowhereTouched) CheckTouchForStrafe.isNowhereTouched = false;
        }


        

        if (CheckTouchForStrafe.isNowhereTouched)
        {
            Temporary.text = "TTTOOOUUUCCCHHHED NOWHERE";
        } 
        else
        {
            Temporary.text = " ";
        }

        //print(PlayerTransform.rotation.eulerAngles.y + " - from me: " + SendAndReceive.MyPlayerData.rotation.y + " - from server");


        MyUI.HealthInput(SendAndReceive.MyPlayerData.health_pool, SendAndReceive.MyPlayerData.max_health_pool);
        MyUI.EnergyInput(SendAndReceive.MyPlayerData.energy);




        /*
        if (Input.GetKeyDown(KeyCode.U))
        {
            for (int i=0; i<MyConds.curr_conds.Count; i++)
            {
                print(MyConds.curr_conds[i].cond_id + " - " + MyConds.curr_conds[i].cond_bulk);
            }
        }
        */

            
        if (Input.GetKeyDown(KeyCode.H))
        {
            for (int i = 0; i < OtherGamers.Count; i++)
            {
                print(i + " - " + OtherGamers[i].OtherPlayerUI.HealthCurrentAmount + " : " + OtherGamers[i].OtherPlayerName);
            }

        print("0 - " + SendAndReceive.OtherPlayerData[0].health_pool);
        print("1 - " + SendAndReceive.OtherPlayerData[1].health_pool);

        }
            


        if (SendAndReceive.SpecificationReceived == 1)
        {

            StartCoroutine(killmess(SendAndReceive.MessageType));

            //ButtonsManagement.CheckResponse();
            StartCoroutine(ButtonsManagement.buttoncooldown());
        }

        if (SendAndReceive.SpecificationReceived == 1 && button_order != SendAndReceive.OrderReceived)
        {

            button_order = SendAndReceive.OrderReceived;
            print(SendAndReceive.ButtonState + " - " + SendAndReceive.MessageType + " - " + SendAndReceive.SpellCooldown + SendAndReceive.SwButtonCond + " : -" + RawPackets[0]);

            //print("me - " + SendAndReceive.MyPlayerData.conditions + " = " + SendAndReceive.OrderReceived + "   him - " + SendAndReceive.OtherPlayerData[0].conditions + " = " + SendAndReceive.OrderReceived);
        }



        //working with conditions!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        CheckMessagesAndConditions();
        CheckMessagesAndConditionsForOther();

        //MY ANIMATIONS===========================================================        
        myanimator.RefreshAnimations(SendAndReceive.MyPlayerData.animation_id);

        //LATENCY
        if (latency_check == SendAndReceive.OrderReceived && isLatency)
        {
            LatencyMain.Add(latency_timer * 1000f);
            TempText1.text = AverageLatency().ToString("f0");

            latency_timer = 0;
            isLatency = false;

        }
        else
        {
            latency_timer += Time.deltaTime;
            if (latency_timer > 2f)
            {
                latency_timer = 0;
                isLatency = false;
            }
        }


        //speed Vector3 CHECK
        if (cur_speed > 0.2f)
        {
            DataAnalys.text = connection.DataForCheck;

            cur_speed = 0;

            TempText2.text = (Vector3.Distance(PlayerCurrentSpeed, PlayerTransform.position) * 5f).ToString("f1");
            PlayerCurrentSpeed = PlayerTransform.position;
            //connection.connector.ListenToServer();
        }
        else
        {
            cur_speed += Time.deltaTime;
        }




        if (cur_time_2 > 0.1f)
        {
            //OtherLatency.text = ToOtherLatency + "\n";
            cur_time_2 = 0;
        }
        else
        {
            cur_time_2 += Time.deltaTime;
        }


        //==================================================================
        if (cur_time >= general.Tick)
        {

            MyHPText.text = SendAndReceive.MyPlayerData.health_pool.ToString();



            try
            {
                if (isButtonSend)
                {
                    isButtonSend = false;
                    connection.connector.TalkToServer(ButtonMessToSend);
                }
                else
                {
                    connection.connector.TalkToServer(SendAndReceive.DataForSending.ToSendMovement(AgregateHoriz, AgregateVertic));

                    AgregateHoriz = 0;
                    AgregateVertic = 0;


                }



                if (!isLatency)
                {
                    isLatency = true;
                    latency_check = SendAndReceive.DataForSending.OrderToSend;
                }

            }
            catch (Exception ex)
            {
                print(ex);

                //connection.connector.AnotherReconnect();
                connection.connector.Reconnect();
            }




            //CORRECTIONS
            CorrectionForPosition = SendAndReceive.MyPlayerData.position - PlayerTransform.position;
            CorrectionForRotation = SendAndReceive.MyPlayerData.rotation - PlayerTransform.rotation.eulerAngles;

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

        //DeltaForLerpMovingNRotation = CountSumForCounts / AverageCountForDeltaForLerp;
        DeltaForRotOnly = CountSumForCounts / AverageCountForDeltaForLerp;

        if (AgregateVertic == 0)
        {
            //ResultDelta*=0.7f;
            //DeltaForLerpMovingNRotation *= ResultDelta;
            DeltaForLerpMovingNRotation *= 0.7f;
        }
        else
        {
            DeltaForLerpMovingNRotation = 0.07f;
        }


        if (AverageCountForDeltaForLerp == 0) AverageCountForDeltaForLerp = 1;
        PlayerTransform.position = Vector3.SmoothDamp(PlayerTransform.position, (Player2NewPos + CorrectionForPosition / (AverageCountForDeltaForLerp)), ref speed, DeltaForLerpMovingNRotation); //DeltaForLerpMovingNRotation/6f

        rotAngle = Quaternion.Angle(Quaternion.Euler(SendAndReceive.MyPlayerData.rotation), PlayerTransform.rotation);

        if ((SendAndReceive.MyPlayerData.rotation.y - PlayerTransform.rotation.eulerAngles.y) >= 0 && Mathf.Abs(SendAndReceive.MyPlayerData.rotation.y - PlayerTransform.rotation.eulerAngles.y) < 180)
        {
            sighAngle = 1;
        }
        else if ((SendAndReceive.MyPlayerData.rotation.y - PlayerTransform.rotation.eulerAngles.y) < 0 && Mathf.Abs(SendAndReceive.MyPlayerData.rotation.y - PlayerTransform.rotation.eulerAngles.y) < 180)
        {
            sighAngle = -1;
        }
        else if ((SendAndReceive.MyPlayerData.rotation.y - PlayerTransform.rotation.eulerAngles.y) >= 0 && Mathf.Abs(SendAndReceive.MyPlayerData.rotation.y - PlayerTransform.rotation.eulerAngles.y) > 180)
        {
            sighAngle = -1;
        }
        else if ((SendAndReceive.MyPlayerData.rotation.y - PlayerTransform.rotation.eulerAngles.y) < 0 && Mathf.Abs(SendAndReceive.MyPlayerData.rotation.y - PlayerTransform.rotation.eulerAngles.y) > 180)
        {
            sighAngle = 1;
        }


        PlayerTransform.rotation = Quaternion.AngleAxis((PlayerTransform.rotation.eulerAngles.y + rotAngle * sighAngle / AverageCountForDeltaForLerp), Vector3.up);

        for (int i = 0; i < OtherGamers.Count; i++)
        {
            OtherGamers[i].SyncPosNRot(DeltaForLerpMovingNRotation, AverageCountForDeltaForLerp);
        }

       
    }



    float AverageLatency()
    {
        float result = 0;

        if (LatencyMain.Count>10)
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

        if (CountsBeforeServerReq.Count>10)
        {
            CountsBeforeServerReq.Remove(CountsBeforeServerReq[0]);
        }
        for (int i = 0; i < CountsBeforeServerReq.Count; i++)
        {
            result += CountsBeforeServerReq[i];
        }
        
        return result = result / (float)CountsBeforeServerReq.Count;        
    }




    private void CheckMessagesAndConditions()
    {        

        for (int i = 0; i < MyConds.curr_conds.Count; i++)
        {
            if (!MyConds.curr_conds[i].isChecked)
            {

                MyEffects.RegisterConds(MyConds.curr_conds[i]);
                
                //if (MyConds.curr_conds[i].cond_type == "cs")
                //{
                    MyConds.curr_conds[i].isChecked = true;
                //}

                if (MyConds.curr_conds[i].cond_type == "dt" && MyConds.curr_conds[i].damage_or_heal > 0)
                {
                    MakeSign(MyConds.curr_conds[i].damage_or_heal.ToString("f0"), PlayerTransform.position + new Vector3(0, 2, 0), Color.red, MyConds.curr_conds[i].isCrit);
                    MyConds.curr_conds[i].isChecked = true;
                }
                else if (MyConds.curr_conds[i].cond_type == "dg" && MyConds.curr_conds[i].damage_or_heal > 0)
                {
                    for (int ii = 0; ii < OtherGamers.Count; ii++)
                    {
                        for (int iii = 0; iii < OtherGamers[ii].Conds.curr_conds.Count; iii++)
                        {
                            if (OtherGamers[ii].Conds.curr_conds[iii].cond_id == MyConds.curr_conds[i].cond_id)
                            {
                                MakeSign(MyConds.curr_conds[i].damage_or_heal.ToString("f0"), OtherGamers[ii].transform.position + new Vector3(0, 2, 0), Color.yellow, MyConds.curr_conds[i].isCrit);
                                //MyConds.curr_conds.Remove(MyConds.curr_conds[i]);
                                MyConds.curr_conds[i].isChecked = true;
                            }
                        }
                    }

                }
                else if (MyConds.curr_conds[i].cond_type == "dg" && MyConds.curr_conds[i].damage_or_heal == 0)
                {
                    //====================================
                }
                else if (MyConds.curr_conds[i].cond_type == "co")
                {
                    //MyUI.AddCondition(MyConds.curr_conds[i].cond_id, MyConds.curr_conds[i].spell_index, MyConds.curr_conds[i].cond_time);
                    StartCoroutine(MyUI.AddCondition(MyConds.curr_conds[i].cond_id, MyConds.curr_conds[i].spell_index, MyConds.curr_conds[i].cond_time));
                    //MyConds.curr_conds.Remove(MyConds.curr_conds[i]);
                    MyConds.curr_conds[i].isChecked = true;
                }

                else if (MyConds.curr_conds[i].cond_type == "ca")
                {

                    if (!MyUI.isCasting)
                    {
                        StartCoroutine(MyUI.AddCasting(MyConds.curr_conds[i].cond_id, MyConds.curr_conds[i].spell_index, MyConds.curr_conds[i].cond_time));
                        MyConds.curr_conds[i].isChecked = true;
                    }
                    if (MyConds.curr_conds[i].cond_message == "CANCELED")
                    {
                        MyUI.StopCurrentCasting();
                        MyEffects.CancelCasting();
                        MyConds.curr_conds[i].isChecked = true;
                    }
                }


                else if (MyConds.curr_conds[i].cond_type == "me")
                {
                    if (MyConds.curr_conds[i].cond_message == "d")
                    {
                        MakeSign("DODGE", PlayerTransform.position, Color.white, false);
                        MyConds.curr_conds[i].isChecked = true;
                    }
                    else if (MyConds.curr_conds[i].cond_message == "b")
                    {
                        MakeSign("BLOCKED", PlayerTransform.position, Color.white, false);
                        MyConds.curr_conds[i].isChecked = true;
                    }

                }



            }


        }
    }


    private void CheckMessagesAndConditionsForOther()
    {
        
        for (int ii = 0; ii < OtherGamers.Count ; ii++)
        {
            for (int iii = 0; iii < OtherGamers[ii].Conds.curr_conds.Count; iii++)
            {

                if (!OtherGamers[ii].Conds.curr_conds[iii].isChecked)
                {

                    OtherGamers[ii].PlayerEffects.RegisterConds(OtherGamers[ii].Conds.curr_conds[iii]);

                    
                    if (OtherGamers[ii].Conds.curr_conds[iii].cond_type == "co")
                    {
                        //MyUI.AddCondition(MyConds.curr_conds[i].cond_id, MyConds.curr_conds[i].spell_index, MyConds.curr_conds[i].cond_time);
                        StartCoroutine(OtherGamers[ii].OtherPlayerUI.AddCondition(OtherGamers[ii].Conds.curr_conds[iii].cond_id, OtherGamers[ii].Conds.curr_conds[iii].spell_index, OtherGamers[ii].Conds.curr_conds[iii].cond_time));

                        //MyConds.curr_conds.Remove(MyConds.curr_conds[i]);
                        OtherGamers[ii].Conds.curr_conds[iii].isChecked = true;
                    }


                    if (OtherGamers[ii].Conds.curr_conds[iii].cond_type == "me" )
                    {
                        if (OtherGamers[ii].Conds.curr_conds[iii].cond_message == "d")
                        {
                            MakeSign("DODGE", OtherGamers[ii].transform.position + new Vector3(0, 2, 0), Color.white, false);
                            OtherGamers[ii].Conds.curr_conds[iii].isChecked = true;

                        }
                        else if (OtherGamers[ii].Conds.curr_conds[iii].cond_message == "b")
                        {
                            MakeSign("BLOCKED", OtherGamers[ii].transform.position + new Vector3(0, 2, 0), Color.white, false);
                            OtherGamers[ii].Conds.curr_conds[iii].isChecked = true;
                        }
                    }

                    OtherGamers[ii].Conds.curr_conds[iii].isChecked = true;

                }
            }
        }

    }
          
    

    void PredictionMachine(float Horizontal, float Vertical, Vector3 old_pos, Vector3 old_rot, out Vector3 position_delta, out Vector3 rotation_delta)
    {

        
        float degree_to_radian = 0.0174532924f;
        float delta_for_rotation = 1f;
        float delta_for_position = 0.06f;

        

        Vector3 PreviousPosition = old_pos;
        Vector3 PreviousRotation = old_rot;
        
        //horizontal touch and rotation
        float new_rotation_x = PreviousRotation.x;
        float new_rotation_y = PreviousRotation.y + Horizontal * delta_for_rotation * koeffrot;
        float new_rotation_z = PreviousRotation.z;

        if (new_rotation_y >= 360) {
            new_rotation_y = new_rotation_y - 360;
        }
        if (new_rotation_y < 0) {
            new_rotation_y = 360 + new_rotation_y;
        }


        //vertical touch and movement
        float new_position_x = PreviousPosition.x + Mathf.Sin(PreviousRotation.y * degree_to_radian) * Mathf.Cos(PreviousRotation.x * degree_to_radian) * delta_for_position * Vertical * koeffpos;
        float new_position_y = PreviousPosition.y + Mathf.Sin(-PreviousRotation.x * degree_to_radian) * delta_for_position * Vertical * koeffpos;
        float new_position_z = PreviousPosition.z + Mathf.Cos(PreviousRotation.x * degree_to_radian) * Mathf.Cos(PreviousRotation.y * degree_to_radian) * delta_for_position * Vertical * koeffpos;

        position_delta = new Vector3(new_position_x, new_position_y, new_position_z);
        rotation_delta = new Vector3(new_rotation_x, new_rotation_y, new_rotation_z);        

    }



    public void MakeSign(string message_text, Vector3 position, Color color, bool isBigger)
    {
        //Vector3 RightPos = CurrentCamera.WorldToScreenPoint(position);
        

        TextMeshProUGUI TakenObject = null;
        bool isFound = false;
                
        for (int i = 0; i < texts.Count; i++)
        {
            if (texts[i].text == "" || texts[i].text == null)
            {
                TakenObject = texts[i];
                isFound = true;
                break;
            }
        }
        
        if (!isFound)
        {
            TakenObject = texts[0];
        }

        TakenObject.text = message_text;
        TakenObject.color = color;

        if (isBigger) {
            TakenObject.rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        } else
        {
            TakenObject.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }

        StartCoroutine(ShowMessage(TakenObject, position));
        
    }



    IEnumerator ShowMessage(TextMeshProUGUI mess_window, Vector3 position)
    {
        if (isMessageShowing)
        {
            yield return new WaitForSeconds(0.1f);
        }

        isMessageShowing = true;

        float deltaX = 0;
        int rnd = UnityEngine.Random.Range(0, 3);
        float coordX = 0;

        switch (rnd)
        {
            case 0:
                coordX = UnityEngine.Random.Range(20f, 50f)*-1;
                break;
            case 1:

                break;
            case 2:
                coordX = UnityEngine.Random.Range(20f, 50f);
                break;
        }

        

        for (float i = 0; i < 20; i+=0.9f)
        {
            if (rnd==0)
            {
                deltaX = -i;
            } 
            else if (rnd == 2)
            {
                deltaX = i;
            }
            //mess_window.rectTransform.anchoredPosition = new Vector2(position.x - 150, position.y + i * 10f-25);
            mess_window.transform.position = new Vector3(CurrentCamera.WorldToScreenPoint(position).x-150 + coordX + deltaX * 2f , CurrentCamera.WorldToScreenPoint(position).y-25 + i*0.4f*i, CurrentCamera.WorldToScreenPoint(position).z);
            yield return new WaitForSeconds(0.05f);
            
            if (i > 0.5f)
            {
                isMessageShowing = false;
            }
        }
        mess_window.text = "";
    }


    IEnumerator SyncForPosition()
    {
        yield return new WaitForSeconds(0.1f);
        PlayerTransform.position = SendAndReceive.MyPlayerData.position;
        PlayerTransform.rotation = Quaternion.Euler(SendAndReceive.MyPlayerData.rotation);
        isStart = true;
    }


    public IEnumerator killmess(string MessFromButt)
    {
        if (PoolOfMessages.Count>3)
        {
            PoolOfMessages.Remove(PoolOfMessages[0]);
        }

        PoolOfMessages.Add((MessFromButt + "\n"));
        string allmess = null;
        if (PoolOfMessages.Count > 0)
        {
            for (int i = 0; i < PoolOfMessages.Count; i++)
            {
                allmess += PoolOfMessages[i];
            }
        }
        ButtonMessage.text = allmess;

        yield return new WaitForSeconds(3f);

        if (PoolOfMessages.Count > 0)
            PoolOfMessages.Remove(PoolOfMessages[0]);

        allmess = "";
        if (PoolOfMessages.Count > 0)
        {
            for (int i = 0; i < PoolOfMessages.Count; i++)
            {
                allmess += PoolOfMessages[i];
            }
        }
        ButtonMessage.text = allmess;
    }


}
