﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


public class connection : MonoBehaviour
{
    private static Socket socket_udp;
    private int port_udp = general.GameServerUDPPort;
    private static IPEndPoint ipendpoint_udp;
    private static EndPoint endpoint_udp;
    private static StringBuilder raw_data_received = new StringBuilder(1024);
    private static byte[] buffer_received_udp = new byte[256 * general.SessionNumberOfPlayers];



    public static connection connector;
    private Socket sck;
    private int CurrentPort = general.GameServerUDPPort;
    private string CurrentIP = general.GameServerIP;
    IPEndPoint endpoint;
    EndPoint remoteIp;
    public string DataFromServer1;
    static string LastReceivedPacket;
    

    public static string DataForCheck;

    private TcpClient client;
    NetworkStream stream;

    CancellationTokenSource cts;
    CancellationToken token;

    // Start is called before the first frame update
    void Awake()
    {
        
        //ListenToServer();
        //ReceiveData();
        //AnotherReconnect();
        //TryStreamReceive();      

    }



    private void Start()
    {
        cts = new CancellationTokenSource();
        token = cts.Token;

        
        
        connector = this;
        //Reconnect();
        Reconnect2();
        //ListenToServer();
    }


    /*
    public void Reconnect()
    {

        SendAndReceive.OrderReceived = 0;
        SendAndReceive.DataForSending.OrderToSend = 0;

        endpoint = new IPEndPoint(IPAddress.Parse(CurrentIP), CurrentPort);
        remoteIp = new IPEndPoint(IPAddress.Any, 0);
        sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram , ProtocolType.Udp);
        try
        {
            sck.Connect(endpoint);


        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }
    */

    public void Reconnect2()
    {        
        socket_udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        endpoint = new IPEndPoint(IPAddress.Parse(CurrentIP), CurrentPort);
        ipendpoint_udp = new IPEndPoint(IPAddress.Any, port_udp);
        endpoint_udp = (EndPoint)ipendpoint_udp;

        //socket_udp.Bind(endpoint_udp);
        socket_udp.BeginReceiveFrom(buffer_received_udp, 0, buffer_received_udp.Length, SocketFlags.None, ref endpoint_udp, new AsyncCallback(ReceiveCallbackUDP), socket_udp);
    }

    private static void ReceiveCallbackUDP(IAsyncResult ar)
    {
        try
        {
            raw_data_received.Clear();

            int n = socket_udp.EndReceiveFrom(ar, ref endpoint_udp);
            raw_data_received.Append(Encoding.UTF8.GetString(buffer_received_udp, 0, n));
            //Console.WriteLine(raw_data_received);

            if (raw_data_received.ToString() != "" && raw_data_received.ToString() != null)
            {                
                print(raw_data_received.ToString());
                RawPacketsProcess(raw_data_received.ToString());
            }

            socket_udp.BeginReceiveFrom(buffer_received_udp, 0, buffer_received_udp.Length, SocketFlags.None, ref endpoint_udp, new AsyncCallback(ReceiveCallbackUDP), socket_udp);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

    }




    public void TalkToServer(string DataForServer)
    {

        try
        {
            //sck.Send(Encoding.UTF8.GetBytes(DataForServer));
            //print("-----=====----- " + DataForServer);
            socket_udp.SendTo(Encoding.UTF8.GetBytes(DataForServer), endpoint);
            
        }
        catch (Exception ex)
        {
            print(ex + "88888888888888888888888888888888888888888888888888888");
        }

        //==================================================================================

        

    }


    public async void SendToServer(string Data)
    {
        //await Task.Run(() => ReceiveData());
        //await Task.Run(() => TryStreamReceive());
        

        await Task.Run(() => TalkToServer(Data));
    }


    public async void ListenToServer()
    {
        

        //await Task.Run(() => ReceiveData(token));
        //await Task.Run(() => TryStreamReceive());
        //await Task.Run(() => TalkToServer(Data));

    }


    /*
    public void ReceiveData(CancellationToken token)
    {

        howmany++;

        try
        {

            StringBuilder messrec = new StringBuilder();
            byte[] msgBuff = new byte[1024 * general.SessionNumberOfPlayers];
            int size = 0;

            

            {
                //size = sck.Receive(msgBuff);
                size = sck.ReceiveFrom(msgBuff, ref remoteIp);
                messrec.Append(Encoding.UTF8.GetString(msgBuff, 0, size));
                cur_time++;

                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Операция прервана токеном");
                    return;
                }
            }
            while (sck.Available > 0) ;

            isstart = true;

            if (messrec.ToString() != "" && messrec.ToString() != null)
            {
                
                print(messrec.ToString());

                howmany--;

                RawPacketsProcess(messrec.ToString());
                


            }
        }
        catch (Exception ex)
        {
            print(ex + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

        }


        //}
    }
    */


    public static void RawPacketsProcess(string RawData)
    {
        string[] getstr;
        getstr = RawData.Split('|');

        /*
        for (int i=0; i< getstr.Length; i++)
        {
            print(i + "->" + getstr[i]);
        }
        print("///////////////////////////////////////////////////////////////////////////");
        */

        if (getstr[0] != "")
        {
            playercontrol.RawPackets.Add((LastReceivedPacket + getstr[0]));
            //print("CORRECTION MADE - " + playercontrol.RawPackets[playercontrol.RawPackets.Count - 1]);
        }
        
        else
        {

            for (int i = 1; i < getstr.Length; i++)
            {
                if (playercontrol.RawPackets.Count > 1)
                {
                    if (playercontrol.RawPackets[playercontrol.RawPackets.Count - 1] != getstr[i])
                    {
                        playercontrol.RawPackets.Add(getstr[i]);
                    }
                }
                else
                {
                    playercontrol.RawPackets.Add(getstr[i]);
                }
            }
        }

        LastReceivedPacket = playercontrol.RawPackets[playercontrol.RawPackets.Count - 1];



    }



    public static string SendAndGetTCP(string DataForSending)
    {
        int CurrentPort_tcp = general.GameServerTCPPort;
        string CurrentIP_tcp = general.GameServerIP;
        string result = null;

        IPEndPoint endpoint_tcp = new IPEndPoint(IPAddress.Parse(CurrentIP_tcp), CurrentPort_tcp);
        Socket sck_tcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //=============================CONNECT======================================
        try
        {
            sck_tcp.Connect(endpoint_tcp);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            result = ex.ToString();

            sck_tcp.Shutdown(SocketShutdown.Both);
            sck_tcp.Close();
            return result;
        }
        //===============================SEND======================================
        try
        {
            sck_tcp.Send(Encoding.UTF8.GetBytes(DataForSending));
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            result = ex.ToString();

            sck_tcp.Shutdown(SocketShutdown.Both);
            sck_tcp.Close();
            return result;
        }
        //================================GET======================================
        try
        {
            StringBuilder messrec = new StringBuilder();
            byte[] msgBuff = new byte[256 * general.SessionNumberOfPlayers];
            int size = 0;

            {
                size = sck_tcp.Receive(msgBuff);
                messrec.Append(Encoding.UTF8.GetString(msgBuff, 0, size));
            }
            while (sck_tcp.Available > 0) ;

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

            sck_tcp.Shutdown(SocketShutdown.Both);
            sck_tcp.Close();
            return result;
        }
        //error case
        sck_tcp.Shutdown(SocketShutdown.Both);
        sck_tcp.Close();
        return result;

    }


    
    void OnApplicationQuit()
    {
        /*
        cts.Cancel();

        int CurrentPort_tcp = general.GameServerTCPPort;
        string CurrentIP_tcp = general.GameServerIP;

        IPEndPoint endpoint_tcp = new IPEndPoint(IPAddress.Parse(CurrentIP_tcp), CurrentPort_tcp);
        Socket sck_tcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //=============================CONNECT======================================
        try
        {
            sck_tcp.Connect(endpoint_tcp);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
        //===============================SEND======================================
        try
        {
            sck_tcp.Send(Encoding.UTF8.GetBytes(SendAndReceive.DataForSending.ToSendKillSignal()));
        }
        catch (Exception ex)
        {
            Debug.Log(ex);

        }
        */
        socket_udp.Shutdown(SocketShutdown.Both);
        socket_udp.Close();
    }
    


    /*
    public void ConnectionDestroy()
    {

        sck.Shutdown(SocketShutdown.Both);
        sck.Close();
        
    }
    */

}