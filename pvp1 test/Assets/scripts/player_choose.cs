using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class player_choose : MonoBehaviour
{
    public Button pl1, pl2, pl3, pl4, pl5, ChooseAnother, EnterTheGame;

    public GameObject warr, mage, barb, rog, wiz, ConnectionError;

    public TextMeshProUGUI EnterTheGameText, ChooseAnotherText, ConnError;

    public struct chars
    {
        public string char_name;
        public int char_type;

        public chars(string nam, int char_t)
        {
            char_name = nam;
            char_type = char_t;
        }
    }

    private chars CurrentPlayerChosen;
    private List<chars> WhatCharacters = new List<chars>();

    public GameObject err_log_window;
    MessageInfo error_messages;

    // Start is called before the first frame update
    void Start()
    {
        error_messages = new MessageInfo(err_log_window);

        Screen.SetResolution(1280, 720, true);
        Camera.main.aspect = 16f / 9f;

        EnterTheGameText.text = lang.EnterTheGameText;
        ChooseAnotherText.text = lang.ChooseAnotherText;
        ConnError.text = lang.ConnectionErrorText;

        ConnectionError.SetActive(false);
        warr.SetActive(false);
        mage.SetActive(false);
        barb.SetActive(false);
        rog.SetActive(false);
        wiz.SetActive(false);
        pl1.gameObject.SetActive(false);
        pl2.gameObject.SetActive(false);
        pl3.gameObject.SetActive(false);
        pl4.gameObject.SetActive(false);
        pl5.gameObject.SetActive(false);

        pl1.onClick.AddListener(p1);
        pl2.onClick.AddListener(p2);
        pl3.onClick.AddListener(p3);
        pl4.onClick.AddListener(p4);
        pl5.onClick.AddListener(p5);
        ChooseAnother.onClick.AddListener(ChooseAnotPl);
        EnterTheGame.onClick.AddListener(EnterTheG);

        WhatCharacters.Add(new chars("none", 0));


        string result = null;
        //result = sr.SendAndGetLoginSetup("1~0~" + general.CurrentTicket);
        try
        {
            result = connection.SendAndGetTCP($"{general.PacketID}~1~0~{general.CurrentTicket}", general.Ports.tcp2324, general.LoginServerIP, true);
        }
        catch (System.Exception ex)
        {
            StartCoroutine(error_messages.process_error("con_err"));
        }


        print(result);

        string[] getstr = result.Split('~');



        switch (getstr[2])
        {           
            case "nst":
                print("wrong login");
                StartCoroutine(error_messages.process_error("nst"));
                break;
            case "nc":
                print("no characters");                
                SceneManager.LoadScene("player_get_new");
                break;
            case "ns":
                encryption.InitEncodingConnection(general.Ports.tcp2324);
                SceneManager.LoadScene("player_choose");
                break;

        }

        if (getstr[2]!= "nst" && getstr[2] != "nc" && getstr[2] != "ns")
        {
            int index = int.Parse(getstr[2]);
            
            if (index==5)
            {
                ChooseAnother.gameObject.SetActive(false);
            }

            for (int i=1; i<=index; i++)
            {

                switch(i)
                {
                    case 1: //3 4
                        pl1.gameObject.SetActive(true);
                        SetName(pl1, getstr[3]);
                        WhatCharacters.Add(new chars(getstr[3], int.Parse(getstr[4])));
                        print(getstr[3] + " - " + int.Parse(getstr[4]));
                        break;
                    case 2: //5 6
                        pl2.gameObject.SetActive(true);
                        SetName(pl2, getstr[5]);
                        WhatCharacters.Add(new chars(getstr[5], int.Parse(getstr[6])));
                        break;
                    case 3:
                        pl3.gameObject.SetActive(true);
                        SetName(pl3, getstr[7]);
                        WhatCharacters.Add(new chars(getstr[7], int.Parse(getstr[8])));
                        break;
                    case 4:
                        pl4.gameObject.SetActive(true);
                        SetName(pl4, getstr[9]);
                        WhatCharacters.Add(new chars(getstr[9], int.Parse(getstr[10])));
                        break;
                    case 5:
                        pl5.gameObject.SetActive(true);
                        SetName(pl5, getstr[11]);
                        WhatCharacters.Add(new chars(getstr[11], int.Parse(getstr[12])));
                        break;
                }
            }

            GetPlayerByNumber(WhatCharacters[1].char_type);
            CurrentPlayerChosen = WhatCharacters[1];
        }
        
    }


    private void ChooseAnotPl()
    {
        SceneManager.LoadScene("player_get_new");
    }

    private void EnterTheG()
    {
        general.CharacterName = CurrentPlayerChosen.char_name;
        general.CharacterType = CurrentPlayerChosen.char_type;
        SceneManager.LoadScene("player_setup");
    }

    


    IEnumerator ConnectionErr()
    {
        ConnectionError.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("player_choose");
    }

    IEnumerator OK()
    {
        ConnectionError.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("player_choose");
    }

    private void SetName(Button button, string ButtonName)
    {
        button.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = ButtonName;
    }

    private void p1()
    {
        GetPlayerByNumber(WhatCharacters[1].char_type);
        CurrentPlayerChosen = WhatCharacters[1];
    }

    private void p2()
    {
        GetPlayerByNumber(WhatCharacters[2].char_type);
        CurrentPlayerChosen = WhatCharacters[2];
    }

    private void p3()
    {
        GetPlayerByNumber(WhatCharacters[3].char_type);
        CurrentPlayerChosen = WhatCharacters[3];
    }

    private void p4()
    {
        GetPlayerByNumber(WhatCharacters[4].char_type);
        CurrentPlayerChosen = WhatCharacters[4];
    }

    private void p5()
    {
        GetPlayerByNumber(WhatCharacters[5].char_type);
        CurrentPlayerChosen = WhatCharacters[5];
    }


    private void GetPlayerByNumber(int Number)
    {
        
        switch(Number)
        {
            case 1:
                warr.SetActive(true);
                mage.SetActive(false);
                barb.SetActive(false);
                rog.SetActive(false);
                wiz.SetActive(false);
                break;
            case 2:
                warr.SetActive(false);
                mage.SetActive(true);
                barb.SetActive(false);
                rog.SetActive(false);
                wiz.SetActive(false);
                break;
            case 3:
                warr.SetActive(false);
                mage.SetActive(false);
                barb.SetActive(true);
                rog.SetActive(false);
                wiz.SetActive(false);
                break;
            case 4:
                warr.SetActive(false);
                mage.SetActive(false);
                barb.SetActive(false);
                rog.SetActive(true);
                wiz.SetActive(false);
                break;
            case 5:
                warr.SetActive(false);
                mage.SetActive(false);
                barb.SetActive(false);
                rog.SetActive(false);
                wiz.SetActive(true);
                break;
        }
                
    }

}
