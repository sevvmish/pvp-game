using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class test : MonoBehaviour
{
    float cur_time;

    int CurrentPort = 2325;
    string CurrentIP = "45.67.57.30";
    string result = null;

    IPEndPoint endpoint;
    EndPoint remoteIp;
    Socket sck;



    private void Start()
    {

        endpoint = new IPEndPoint(IPAddress.Parse(CurrentIP), CurrentPort);
        remoteIp = new IPEndPoint(IPAddress.Any, 0);
        sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        sck.Connect(endpoint);
    }

    private void Update()
    {

        
        if (cur_time > 0.1f)
        {

            sck.SendTo(Encoding.UTF8.GetBytes("dhdhdh"), endpoint);

            cur_time = 0;

        }
        else
        {
            cur_time += Time.deltaTime;
        }
             
    }

    private void OnDestroy()
    {
        sck.Shutdown(SocketShutdown.Both);
        sck.Close();
    }


    public static void SendAndGetLoginSetup(string DataForSending)
    {
        
        //=============================CONNECT======================================
        /*
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
            //return result;
        }
        
        //===============================SEND======================================
        try
        {
            //sck.Send(Encoding.UTF8.GetBytes(DataForSending));
            sck.SendTo(Encoding.UTF8.GetBytes(DataForSending), endpoint);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            result = ex.ToString();

            sck.Shutdown(SocketShutdown.Both);
            sck.Close();
            //return result;
        }
        //================================GET======================================
        
        
        try
        {
            StringBuilder messrec = new StringBuilder();
            byte[] msgBuff = new byte[255];
            int size = 0;

            {
                //size = sck.Receive(msgBuff);
                size = sck.ReceiveFrom(msgBuff, ref remoteIp);
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
        */
        //error case
        //sck.Shutdown(SocketShutdown.Both);
        //sck.Close();
        //return result;

    }



}
