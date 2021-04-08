﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class login_setup : MonoBehaviour
{
    public Canvas NewLogOrGuest, CanvasLogin, CanvasGetServer, ConnectionError, CanvasWaiting;    
    public TMP_InputField login_input;
    public TMP_InputField password_input;
    public Button login_button, createnew_button, beforelogin_newlog, beforelogin_asguest, MoscowButton;
    public TextMeshProUGUI InfoMessages, Createnewtext, Logintext, ConnError, AwaitingText, TheLoginText, ThePasswordText;

    

    // Start is called before the first frame update    
    private void Start()
    {
        sr.isConnectionError = false;

        //langu.SetLangu("eng");

        Screen.SetResolution(1280, 720, true);
        Camera.main.aspect = 16f / 9f;
        NewLogOrGuest.gameObject.SetActive(false);
        CanvasLogin.gameObject.SetActive(false);

        Createnewtext.text = lang.CreateNewText;
        Logintext.text = lang.LoginText;
        ConnError.text = lang.ConnectionErrorText;
        AwaitingText.text = lang.AwaitingText;
        TheLoginText.text = lang.TheLoginText;
        ThePasswordText.text = lang.ThePasswordText;

        /*
        if (PlayerPrefs.GetInt("EnterAs") == 0)
        {
            NewLogOrGuest.gameObject.SetActive(true);
            login_button.gameObject.SetActive(false);
            createnew_button.gameObject.SetActive(true);

        }
        else if (PlayerPrefs.GetInt("EnterAs") == 2)
        {
            StartCoroutine(WaitingAndEnter());

        }
        else if (PlayerPrefs.GetInt("EnterAs") == 1)
        {

            CanvasLogin.gameObject.SetActive(true);
            login_button.gameObject.SetActive(true);
            createnew_button.gameObject.SetActive(false);

        }
       */
        
        
        CanvasGetServer.gameObject.SetActive(false);
        ConnectionError.gameObject.SetActive(false);
        
        beforelogin_newlog.onClick.AddListener(Beforelogin_newlog);
        login_button.onClick.AddListener(TryLoginSend);
        beforelogin_asguest.onClick.AddListener(Beforelogin_asguest);
        createnew_button.onClick.AddListener(CreateNewLog);
        login_button.interactable = false;
    }

    private void FixedUpdate()
    {
        if (sr.isConnectionError)
        {
            StartCoroutine(ConnectionErr());
        }

        if (login_input.text != null && password_input.text != null)
        {

            bool isOK = true;
            if (login_input.text.Length < 8 || login_input.text.Length > 16)
            {
                isOK = false;
            }

            if (password_input.text.Length < 8 || password_input.text.Length > 16)
            {
                isOK = false;
            }


            if (isOK)
            {
                login_button.interactable = true;
                createnew_button.interactable = true;
                //InfoMessages.text = "";
            }
            else
            {
                login_button.interactable = false;
                createnew_button.interactable = false;
            }
        }
        else
        {
            login_button.interactable = false;
            createnew_button.interactable = false;
        }
    }



    public void Beforelogin_asguest()
    {

        if (PlayerPrefs.GetString("GuestLogin")=="")
        {
            string result1 = sr.SendAndGetLoginSetup("0~2");
                        
            string[] getstr1 = result1.Split('~');
            
            if (result1==null || result1=="")
            {
                StartCoroutine(ConnectionErr());
                
            } else
            {                
                PlayerPrefs.SetString("GuestLogin", getstr1[2]);
                PlayerPrefs.SetString("GuestPassword", getstr1[3]);
                string result = sr.SendAndGetLoginSetup("0~1~" + PlayerPrefs.GetString("GuestLogin") + "~" + PlayerPrefs.GetString("GuestPassword"));
                string[] getstr = result.Split('~');
                general.CurrentTicket = getstr[2];
                print(general.CurrentTicket + " - " + PlayerPrefs.GetString("GuestLogin") + " - " + PlayerPrefs.GetString("GuestPassword"));
            }            
        } 
        else
        {
            string result = sr.SendAndGetLoginSetup("0~1~" + PlayerPrefs.GetString("GuestLogin") + "~" + PlayerPrefs.GetString("GuestPassword"));
            string[] getstr = result.Split('~');

            if (result == null || result == "")
            {
                StartCoroutine(ConnectionErr());

            } else
            {
                general.CurrentTicket = getstr[2];
                print(general.CurrentTicket + " - " + PlayerPrefs.GetString("GuestLogin") + " - " + PlayerPrefs.GetString("GuestPassword"));
            }            
        }

        //add the server map
        PlayerPrefs.SetInt("EnterAs", 2);
        SceneManager.LoadScene("player_choose");
    }


    public void Beforelogin_newlog()
    {
        NewLogOrGuest.gameObject.SetActive(false);
        CanvasLogin.gameObject.SetActive(true);
    }

    private void TryLoginSend()
    {
        string result = sr.SendAndGetLoginSetup("0~1~" + login_input.text + "~" + password_input.text);
        string[] getstr = result.Split('~');

        if (result == null || result == "")
        {
            StartCoroutine(ConnectionErr());
        }
        else if (getstr[2]=="wll" || getstr[2] == "wlp" || getstr[2] == "ude" || getstr[2] == "wp")
        {
            StartCoroutine(Info(getstr[2]));
        } 
        else
        {
            general.CurrentTicket = getstr[2];
            print(general.CurrentTicket + " - " + login_input.text + " - " + password_input.text);
            //PlayerPrefs.SetInt("EnterAs", 1);
            SceneManager.LoadScene("player_choose");
        }
    }

    private void CreateNewLog()
    {
        string result = sr.SendAndGetLoginSetup("0~0~" + login_input.text + "~" + password_input.text);

        print(result);

        string[] getstr = result.Split('~');

        if (result == null || result == "")
        {
            StartCoroutine(ConnectionErr());
        }
        else if (getstr[2] == "wll" || getstr[2] == "wlp" || getstr[2] == "uae" || getstr[2] == "ecu")
        {
            StartCoroutine(Info(getstr[2]));
        }
        else
        {
            string result11 = sr.SendAndGetLoginSetup("0~1~" + login_input.text + "~" + password_input.text);
            string[] getstr11 = result11.Split('~');

            if (result == null || result == "")
            {
                StartCoroutine(ConnectionErr());
            }
            else if (getstr11[2] == "wll" || getstr11[2] == "wlp" || getstr11[2] == "ude" || getstr11[2] == "wp")
            {
                StartCoroutine(Info(getstr11[2]));
            }
            else
            {
                general.CurrentTicket = getstr11[2];
                print(general.CurrentTicket + " - " + login_input.text + " - " + password_input.text);
                PlayerPrefs.SetInt("EnterAs", 1);
                SceneManager.LoadScene("player_choose");
            }
        }
    }

   
    IEnumerator ConnectionErr()
    {
        ConnectionError.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("login");
    }

    IEnumerator WaitingAndEnter()
    {
        CanvasWaiting.gameObject.SetActive(true);


        string result = sr.SendAndGetLoginSetup("0~1~" + PlayerPrefs.GetString("GuestLogin") + "~" + PlayerPrefs.GetString("GuestPassword"));
        string[] getstr = result.Split('~');

        if (result == null || result == "")
        {
            StartCoroutine(ConnectionErr());

        }
        else
        {
            general.CurrentTicket = getstr[2];
            print(general.CurrentTicket + " - " + PlayerPrefs.GetString("GuestLogin") + " - " + PlayerPrefs.GetString("GuestPassword"));
        }


        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("player_choose");
    }

    IEnumerator Info(string Message)
    {
        InfoMessages.text = codes.GetCodeResult(Message);        
        yield return new WaitForSeconds(1f);
        InfoMessages.text = "";
    }

}




public static class codes
{
    public static string GetCodeResult(string CodeResult)
    {
        string result = null;

        switch(CodeResult)
        {
            case "wll":
                result = lang.wlltext;
                break;
            case "wlp":
                result = lang.wlptext;
                break;
            case "uae":
                result = lang.uaetext;
                break;
            case "ecu":
                result = lang.ecutext;
                break;
            case "ude":
                result = lang.udetext;
                break;
            case "wp":
                result = lang.wptext;
                break;
            case "uc":
                result = lang.uctext;
                break;
        }

        return result;

    }
}


