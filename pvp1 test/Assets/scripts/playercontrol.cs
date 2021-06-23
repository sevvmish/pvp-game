using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Globalization;
using UnityEngine.SceneManagement;
using System.Threading;
//using UnityEngine.UIElements;

public class playercontrol : MonoBehaviour
{
    private bool isStart;

    //Joystick data from Canvas
    private FloatingJoystick MyJoystickTemp;
    public static Joysticks MyJoystick;

    public TextMeshProUGUI TempText1, TempText2, MyHPText, OtherLatency, DataAnalys, Temporary;
    public static string ToOtherLatency;    
    
    //main packets for geting data to player from connection
    public static List<string> RawPackets = new List<string>();
    
    //messages from damage and conds
    List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>(10);
    List<Touch> touches = new List<Touch>();

    //My Player
    public static PlayerUI MyUI;
    private GameObject MainPlayerGameObject;
    private effects MyEffects;

    //conditions of myPlayer
    private ConditionsAnalys MyConds = new ConditionsAnalys();

    //main player transform
    private Transform PlayerTransform;
    public static float AgregateHoriz, AgregateVertic;

    //private Animator PlayerAnimator;
    AnimationsForPlayers myanimator;
    
    //other players data    
    public static List<players> OtherGamers;
    
    float cur_time, cur_time_2, cur_speed, cur_time_lat;

    Vector3 PrevPos, Player2NewPos, Player2NewRot, CorrectionForPosition, CorrectionForRotation, little_pos, little_rot;

    public static Vector3 PlayerCurrentSpeed;

    //koeffs and counts=======================================================
    List<float> CountsBeforeServerReq = new List<float>(12);
    float CountSumForCounts, DeltaForLerpMovingNRotation, AverageCountForDeltaForLerp, DeltaForRotOnly;

    float koeffpos = 3f; //1.6
    float koeffrot = 1f; //1.6
    
    
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
    public static Buttons ButtonsManagement = new Buttons();

    private TextMeshProUGUI ButtonMessage;
    List<string> PoolOfMessages = new List<string>();

    //regulating send and receive
    public static bool isSendingOK = true, isStopMovement;


    //button sending MODE
    public static bool isButtonSend;
    public static string ButtonMessToSend;


    private float summofbadlat;
    private bool isBadLat;

    void Start()
    {
        Screen.SetResolution(1280, 720, true);
        Camera.main.aspect = 16f / 9f;
        Application.targetFrameRate = 60;
        general.isRestarting = false;

        /*
        string EncodingData = connection.SendAndGetTCP($"0~6~{general.SessionPlayerID}~{general.SessionTicket}~0");
        general.PlayerEncryption.ProcessInitDataFromServerUDP(EncodingData);
        connection.SendAndGetTCP($"0~6~{general.SessionPlayerID}~{general.SessionTicket}~1~{general.PlayerEncryption.open_key_from_client_one}~{general.PlayerEncryption.open_key_from_client_two}~{general.PlayerEncryption.open_key_from_client_three}");
        */

        
        print(encryption.InitEncodingConnection(general.Ports.tcp2323) + " - result of encoding");

        //print("new packetID - " + general.PacketID);
        
        string SessionResult = connection.SendAndGetTCP(SessionData.SendSessionDataRequest(), general.Ports.tcp2323, general.GameServerIP, true);
        print(SessionResult  +" - result of sess");
        SessionData.GetDataSessionPlayers(SessionResult);
        general.SetSessionData();

        
               
        switch(general.DataForSession[0].Zone_Type)
        {
            case 0:
                break;

            case 1:
                GameObject zone = Instantiate(Resources.Load<GameObject>("prefabs/location1"), Vector3.zero, Quaternion.identity, GameObject.Find("OtherPlayers").transform);
                
                break;

            case 2:
                break;

            case 3:
                break;

        }
        
        
        if (general.DataForSession.Count > 0)
        {
            for (int i = 0; i < general.DataForSession.Count; i++)
            {
                print(general.DataForSession[i].PlayerName + " - " + general.DataForSession[i].PlayerClass + " - " + general.DataForSession[i].PlayerOrder);
            }
        }
        print("player - " + general.SessionPlayerID + " .......  ticketforsession - " + general.SessionTicket);

        ButtonsManagement.Init();
        
        

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

        OtherGamers = new List<players>();
        
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


    public void AddPlayer(Vector3 pos, Vector3 rot, int order)
    {        
        int WhatPlayersClass = general.DataForSession[order].PlayerClass;
        GameObject ggg = Instantiate(general.GetPlayerByClass(WhatPlayersClass), Vector3.zero, Quaternion.identity, GameObject.Find("OtherPlayers").transform);
        ggg.GetComponent<effects>().MyPlayerClass = WhatPlayersClass;
        ggg.GetComponent<players>().NumberInSendAndReceive = (order-1);
        ggg.GetComponent<players>().OtherPlayerName = general.DataForSession[order].PlayerName;
        //ggg.GetComponent<effects>().PlayerSessionData = general.DataForSession[order];
        ggg.GetComponent<effects>().PlayerSessionDataOrder = general.DataForSession[order].PlayerOrder;
        ggg.GetComponent<players>().CreateUI();
        OtherGamers.Add(ggg.GetComponent<players>());       
    }


    public static void AddNonPlayer(Vector3 pos, Vector3 rot, int order, int whatclass, string name)
    {
        
        GameObject ggg = Instantiate(general.GetPlayerByClass(whatclass), Vector3.zero, Quaternion.identity, GameObject.Find("OtherPlayers").transform);
        ggg.GetComponent<effects>().MyPlayerClass = whatclass;

        ggg.GetComponent<players>().NumberInSendAndReceive = (order-2);
        ggg.GetComponent<players>().OtherPlayerName = name;
        //ggg.GetComponent<effects>().PlayerSessionData = general.DataForSession[order];
       
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

        

        if (SendAndReceive.OtherPlayerData.Length > 0)
        {
            for (int i = 0; i < OtherGamers.Count; i++)
            {
                //print(OtherGamers.Count + " - KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK");
                OtherGamers[i].Conds.Check(SendAndReceive.OtherPlayerData[i].conditions);
                if (OtherGamers[i].isUIadded)
                {
                    OtherGamers[i].OtherPlayerUI.HealthInput(SendAndReceive.OtherPlayerData[0].health_pool, SendAndReceive.OtherPlayerData[0].max_health_pool);
                    OtherGamers[i].OtherPlayerUI.EnergyInput(SendAndReceive.OtherPlayerData[0].energy);
                }
            }
        }

        MyJoystick.CheckTouches(MyJoystickTemp.Vertical, MyJoystickTemp.Horizontal);

        MyUI.HealthInput(SendAndReceive.MyPlayerData.health_pool, SendAndReceive.MyPlayerData.max_health_pool);
        MyUI.EnergyInput(SendAndReceive.MyPlayerData.energy);

        if (Input.GetKeyDown(KeyCode.H))
        {
            for (int i = 0; i < OtherGamers.Count; i++)
            {
                print(i + " - " + OtherGamers[i].OtherPlayerUI.HealthCurrentAmount + " : " + OtherGamers[i].OtherPlayerName);
            }

            print("0 - " + SendAndReceive.OtherPlayerData[0].health_pool);
            print("1 - " + SendAndReceive.OtherPlayerData[1].health_pool);

        }


        //button cooldowns======================================================
        if (SendAndReceive.SpecificationReceived == 1)
        {
            StartCoroutine(killmess(SendAndReceive.MessageType));
            StartCoroutine(ButtonsManagement.buttoncooldown());
        }
        if (SendAndReceive.SpecificationReceived == 2)
        {
            StartCoroutine(ButtonsManagement.CurrentButtonCooldown());
        }
        //=========================================================================


        if (SendAndReceive.SpecificationReceived == 1 && button_order != SendAndReceive.OrderReceived)
        {

            button_order = SendAndReceive.OrderReceived;
            print(SendAndReceive.ButtonState + " - " + SendAndReceive.MessageType + " - " + SendAndReceive.SpellCooldown + SendAndReceive.SwButtonCond + " : -" + RawPackets[0]);
            
        }


        if (Input.GetKeyDown(KeyCode.T))
        {
            for (int i = 0; i < OtherGamers.Count; i++)
            {
                print(i + " - " + OtherGamers[i].OtherPlayerName + " lllllllllllllllll");
            }
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

            if (LatencyMain[LatencyMain.Count-1] > 100)
            {
                isBadLat = true;
                StartCoroutine(BadLatency());
            }
            

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

        if (cur_time_lat>0.1f)
        {
            cur_time_lat = 0;
            if (LatencyMain.Count > 1)
            {
                TempText1.text = "current - " + LatencyMain[LatencyMain.Count - 1].ToString("f0") + "   average - " + AverageLatency().ToString("f0");
            }
        } else
        {
            cur_time_lat += Time.deltaTime;
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
                    //print(Thread.CurrentThread.ManagedThreadId + " - from playercontorl");
                    SendAndReceive.DataForSending.HorizontalTouch = AgregateHoriz;
                    SendAndReceive.DataForSending.VerticalTouch = AgregateVertic;
                    connection.TalkToServer(ButtonMessToSend);
                    AgregateHoriz = 0;
                    AgregateVertic = 0;
                }
                else
                {
                    connection.TalkToServer(SendAndReceive.DataForSending.ToSendMovement(AgregateHoriz, AgregateVertic));
                    //print(Thread.CurrentThread.ManagedThreadId + " - from playercontorl");
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
                                
                connection.connector.Reconnect2();
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
            
            DeltaForLerpMovingNRotation *= 0.7f;
        }
        else
        {
            DeltaForLerpMovingNRotation = 0.07f;
        }

        //print(DeltaForLerpMovingNRotation.ToString("f2") + " - <=");


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

        if (SendAndReceive.OtherPlayerData.Length > 0)
        {
            for (int i = 0; i < OtherGamers.Count; i++)
            {
                //OtherGamers[i].OtherPlayerUI.GetComponent<RectTransform>().anchoredPosition = new Vector3(CurrentCamera.WorldToScreenPoint(OtherGamers[i].PlayerTransform.position).x, CurrentCamera.WorldToScreenPoint(OtherGamers[i].PlayerTransform.position).y);
                //print(OtherGamers[i].name + " - " + CurrentCamera.WorldToScreenPoint(OtherGamers[i].PlayerTransform.position).z);
                if (OtherGamers[i].PlayerEffects.MyPlayerClass==4)
                {
                    OtherGamers[i].OtherPlayerUI.UIPosition.anchoredPosition = new Vector3(CurrentCamera.WorldToScreenPoint(OtherGamers[i].PlayerTransform.position).x - 60, CurrentCamera.WorldToScreenPoint(OtherGamers[i].PlayerTransform.position).y + 150 - ((CurrentCamera.WorldToScreenPoint(OtherGamers[i].PlayerTransform.position).z / 10f) * (CurrentCamera.WorldToScreenPoint(OtherGamers[i].PlayerTransform.position).z / 10f) * (CurrentCamera.WorldToScreenPoint(OtherGamers[i].PlayerTransform.position).z / 10f) * 10f));
                }
                else
                {
                    OtherGamers[i].OtherPlayerUI.UIPosition.anchoredPosition = new Vector3(CurrentCamera.WorldToScreenPoint(OtherGamers[i].PlayerTransform.position).x - 60, CurrentCamera.WorldToScreenPoint(OtherGamers[i].PlayerTransform.position).y + 170 - ((CurrentCamera.WorldToScreenPoint(OtherGamers[i].PlayerTransform.position).z / 10f) * (CurrentCamera.WorldToScreenPoint(OtherGamers[i].PlayerTransform.position).z / 10f) * (CurrentCamera.WorldToScreenPoint(OtherGamers[i].PlayerTransform.position).z / 10f) * 10f));
                }
                
                OtherGamers[i].SyncPosNRot(DeltaForLerpMovingNRotation, AverageCountForDeltaForLerp);
            }
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
                if (MyConds.curr_conds[i].spell_index==1007 && !general.isRestarting)
                {
                    general.isRestarting = true;
                    print(connection.SendAndGetTCP($"{general.PacketID}~0~7~{general.SessionPlayerID}~{general.SessionTicket}", general.Ports.tcp2323, general.GameServerIP, true));
                    StartCoroutine(RestartLevel(MyConds.curr_conds[i]));
                }

                MyEffects.RegisterConds(MyConds.curr_conds[i]);
                
                //if (MyConds.curr_conds[i].cond_type == "cs")
                //{
                    MyConds.curr_conds[i].isChecked = true;
                //}

                if (MyConds.curr_conds[i].cond_type == "dt" && MyConds.curr_conds[i].damage_or_heal > 0)
                {
                    MakeSign(MyConds.curr_conds[i].damage_or_heal.ToString("f0"), PlayerTransform.position + new Vector3(0, 2, 0), Color.red, MyConds.curr_conds[i].isCrit, true);
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
                                MakeSign(MyConds.curr_conds[i].damage_or_heal.ToString("f0"), OtherGamers[ii].transform.position + new Vector3(0, 2, 0), Color.yellow, MyConds.curr_conds[i].isCrit, true);
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
                    
                    StartCoroutine(MyUI.AddCondition(MyConds.curr_conds[i]));                    
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
                        MakeSign("DODGE", PlayerTransform.position, Color.white, false, true);
                        MyConds.curr_conds[i].isChecked = true;
                    }
                    else if (MyConds.curr_conds[i].cond_message == "b")
                    {
                        MakeSign("BLOCKED", PlayerTransform.position, Color.white, false, true);
                        MyConds.curr_conds[i].isChecked = true;
                    }
                    else if (MyConds.curr_conds[i].cond_message == "r")
                    {
                        MakeSign("RESISTED", PlayerTransform.position, Color.white, false, true);
                        MyConds.curr_conds[i].isChecked = true;
                    }
                    else if (MyConds.curr_conds[i].cond_message == "i")
                    {
                        MakeSign("IMMUNE", PlayerTransform.position, Color.white, false, true);
                        MyConds.curr_conds[i].isChecked = true;
                    }

                }


                /*
                else if (MyConds.curr_conds[i].cond_type == "him")
                {
                    if (MyConds.curr_conds[i].cond_message == "r")
                    {
                        MakeSign("RESISTED", PlayerTransform.position, Color.white, false, true);
                        MyConds.curr_conds[i].isChecked = true;
                    } else if (MyConds.curr_conds[i].cond_message == "i")
                    {
                        MakeSign("IMMUNE", PlayerTransform.position, Color.white, false, true);
                        MyConds.curr_conds[i].isChecked = true;
                    }

                }
                */

                else if (MyConds.curr_conds[i].cond_type == "ht" && MyConds.curr_conds[i].damage_or_heal > 0)
                {
                    print(MyConds.curr_conds[i].cond_bulk);
                    MakeSign("+" + MyConds.curr_conds[i].damage_or_heal.ToString("f0"), PlayerTransform.position + new Vector3(0, 2, 0), Color.green, MyConds.curr_conds[i].isCrit, false);
                    MyConds.curr_conds[i].isChecked = true;
                }

                else if (MyConds.curr_conds[i].cond_type == "hg" && MyConds.curr_conds[i].damage_or_heal > 0)
                {
                    print(MyConds.curr_conds[i].cond_bulk);
                    for (int ii = 0; ii < OtherGamers.Count; ii++)
                    {
                        for (int iii = 0; iii < OtherGamers[ii].Conds.curr_conds.Count; iii++)
                        {
                            if (OtherGamers[ii].Conds.curr_conds[iii].cond_id == MyConds.curr_conds[i].cond_id)
                            {
                                MakeSign(MyConds.curr_conds[i].damage_or_heal.ToString("f0"), OtherGamers[ii].transform.position + new Vector3(0, 2, 0), Color.green, MyConds.curr_conds[i].isCrit, false);
                                
                                MyConds.curr_conds[i].isChecked = true;
                            }
                        }
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
                        if (OtherGamers[ii].isUIadded) {
                            
                            StartCoroutine(OtherGamers[ii].OtherPlayerUI.AddCondition(OtherGamers[ii].Conds.curr_conds[iii]));
                            
                        }
                        OtherGamers[ii].Conds.curr_conds[iii].isChecked = true;

                    }


                    if (OtherGamers[ii].Conds.curr_conds[iii].cond_type == "me" )
                    {
                        for (int g = 0; g < MyConds.curr_conds.Count; g++)
                        {
                            if (MyConds.curr_conds[g].cond_type == "him" && MyConds.curr_conds[g].cond_id== OtherGamers[ii].Conds.curr_conds[iii].cond_id)
                            {
                                switch(OtherGamers[ii].Conds.curr_conds[iii].cond_message)
                                {
                                    case "d":
                                        MakeSign("DODGE", OtherGamers[ii].transform.position + new Vector3(0, 2, 0), Color.white, false, true);
                                        OtherGamers[ii].Conds.curr_conds[iii].isChecked = true;
                                        MyConds.curr_conds[g].isChecked = true;
                                        break;
                                    case "b":
                                        MakeSign("BLOCKED", OtherGamers[ii].transform.position + new Vector3(0, 2, 0), Color.white, false, true);
                                        OtherGamers[ii].Conds.curr_conds[iii].isChecked = true;
                                        MyConds.curr_conds[g].isChecked = true;
                                        break;
                                    case "r":
                                        MakeSign("RESISTED", OtherGamers[ii].transform.position + new Vector3(0, 2, 0), Color.white, false, true);
                                        OtherGamers[ii].Conds.curr_conds[iii].isChecked = true;
                                        MyConds.curr_conds[g].isChecked = true;
                                        break;
                                    case "i":
                                        MakeSign("IMMUNE", OtherGamers[ii].transform.position + new Vector3(0, 2, 0), Color.white, false, true);
                                        OtherGamers[ii].Conds.curr_conds[iii].isChecked = true;
                                        MyConds.curr_conds[g].isChecked = true;
                                        break;
                                }
                            }
                        }

                        /*
                        if (OtherGamers[ii].Conds.curr_conds[iii].cond_message == "d")
                        {
                            MakeSign("DODGE", OtherGamers[ii].transform.position + new Vector3(0, 2, 0), Color.white, false, true);
                            OtherGamers[ii].Conds.curr_conds[iii].isChecked = true;

                        }
                        else if (OtherGamers[ii].Conds.curr_conds[iii].cond_message == "b")
                        {
                            MakeSign("BLOCKED", OtherGamers[ii].transform.position + new Vector3(0, 2, 0), Color.white, false, true);
                            OtherGamers[ii].Conds.curr_conds[iii].isChecked = true;
                        }
                        if (OtherGamers[ii].Conds.curr_conds[iii].cond_message == "r")
                        {
                            MakeSign("RESISTED", OtherGamers[ii].transform.position + new Vector3(0, 2, 0), Color.white, false, true);
                            OtherGamers[ii].Conds.curr_conds[iii].isChecked = true;

                        }
                        if (OtherGamers[ii].Conds.curr_conds[iii].cond_message == "i")
                        {
                            MakeSign("IMMUNE", OtherGamers[ii].transform.position + new Vector3(0, 2, 0), Color.white, false, true);
                            OtherGamers[ii].Conds.curr_conds[iii].isChecked = true;

                        }
                        */
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



    public void MakeSign(string message_text, Vector3 position, Color color, bool isBigger, bool isUP)
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

        StartCoroutine(ShowMessage(TakenObject, position, isUP));
        
    }



    IEnumerator ShowMessage(TextMeshProUGUI mess_window, Vector3 position, bool isUP)
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

            if (isUP)
            {
                mess_window.transform.position = new Vector3(CurrentCamera.WorldToScreenPoint(position).x - 150 + coordX + deltaX * 2f, CurrentCamera.WorldToScreenPoint(position).y - 25 + i * 0.4f * i, CurrentCamera.WorldToScreenPoint(position).z);
            }
            else
            {
                mess_window.transform.position = new Vector3(CurrentCamera.WorldToScreenPoint(position).x - 150 + coordX + deltaX * 2f, CurrentCamera.WorldToScreenPoint(position).y - 200 - i * 0.4f * i, CurrentCamera.WorldToScreenPoint(position).z);
            }

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

    IEnumerator BadLatency()
    {
        isBadLat = true;

        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.02f);
            if (LatencyMain[LatencyMain.Count-1] > 100)
            {
                summofbadlat += LatencyMain[LatencyMain.Count - 1];
            }
        }
        print("sum of lat - " + summofbadlat);
        summofbadlat = 0;
        isBadLat = false;
    }

    public static void ResetAllCondAnalysData(ConditionsAnalys _any_data)
    {
        _any_data.curr_conds.Clear();
    }


    public IEnumerator RestartLevel(Conds _data)
    {
        float _timer = _data.cond_time;
        print(_timer + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        yield return new WaitForSeconds(_timer+0.4f);       

        general.DataForSession.Clear();
      
        SceneManager.LoadScene("SampleScene");
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
