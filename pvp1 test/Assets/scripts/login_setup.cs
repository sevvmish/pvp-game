using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using TMPro;
using UnityEngine.UI;

public class login_setup : MonoBehaviour
{
    public TMP_InputField login_input;
    public TMP_InputField password_input;
    public Button login_button;
    public TextMeshProUGUI InfoMessages;

    // Start is called before the first frame update    
    private void Start()
    {
        Screen.SetResolution(1280, 720, true);
        Camera.main.aspect = 16f / 9f;
        login_button.onClick.AddListener(TryLoginSend);
        login_button.interactable = false;
    }

    private void Update()
    {
        if (login_input.text != null && password_input.text != null)
        {

            bool isOK = true;
            if (login_input.text.Length < 8 || login_input.text.Length > 18)
            {
                isOK = false;
            }

            if (password_input.text.Length < 8 || password_input.text.Length > 18)
            {
                isOK = false;
            }


            if (isOK)
            {
                login_button.interactable = true;
                InfoMessages.text = "";
            }
            else
            {
                login_button.interactable = false;
            }
        }
        else
        {
            login_button.interactable = false;
        }


    }

    private void TryLoginSend()
    {
        print(SendAndGetLoginSetup("0~1~" + login_input.text + "~" + password_input.text));
    }

    public static string SendAndGetLoginSetup(string DataForSending)
    {
        int CurrentPort = 2324;
        string CurrentIP = "45.67.57.30";
        string result = null;

        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(CurrentIP), CurrentPort);
        Socket sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //=============================CONNECT======================================
        try
        {
            sck.Connect(endpoint);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            result = ex.ToString();

            sck.Shutdown(SocketShutdown.Both);
            sck.Close();
            return result;
        }
        //===============================SEND======================================
        try
        {
            sck.Send(Encoding.UTF8.GetBytes(DataForSending));
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            result = ex.ToString();

            sck.Shutdown(SocketShutdown.Both);
            sck.Close();
            return result;
        }
        //================================GET======================================
        try
        {
            StringBuilder messrec = new StringBuilder();
            byte[] msgBuff = new byte[255];
            int size = 0;

            {
                size = sck.Receive(msgBuff);
                messrec.Append(Encoding.UTF8.GetString(msgBuff, 0, size));
            }
            while (sck.Available > 0) ;

            if (messrec.ToString() != "" && messrec.ToString() != null)
            {
                result = messrec.ToString();
            }
            else
            {
                result = "error in received data";
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            result = ex.ToString();

            sck.Shutdown(SocketShutdown.Both);
            sck.Close();
            return result;
        }
        //error case
        sck.Shutdown(SocketShutdown.Both);
        sck.Close();
        return result;

    }


}

