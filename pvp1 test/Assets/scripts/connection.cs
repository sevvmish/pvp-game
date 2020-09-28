using System.Collections;
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

    public static connection connector;
    private Socket sck;
    private int CurrentPort = general.GameServerUDPPort;
    private string CurrentIP = general.GameServerIP;
    IPEndPoint endpoint;
    EndPoint remoteIp;
    public string DataFromServer1;
    static string LastReceivedPacket;
    float cur_time, cur_time2, howmany;
    bool isstart, isBrokenPacket;

    public static string DataForCheck;

    private TcpClient client;
    NetworkStream stream;

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
        isstart = true;
        connector = this;
        Reconnect();
        //ListenToServer();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ListenToServer();
        }

        if (DataForCheck !=(SendAndReceive.DataForSending.OrderToSend + " - to send|   " + SendAndReceive.OrderReceived + " - to rece| == " + (SendAndReceive.DataForSending.OrderToSend- SendAndReceive.OrderReceived) +  "  - over:" + howmany + " == " + cur_time + " - timer\n"))
        {
            DataForCheck = SendAndReceive.DataForSending.OrderToSend + " - to send|   " + SendAndReceive.OrderReceived + " - to rece| == " + (SendAndReceive.DataForSending.OrderToSend - SendAndReceive.OrderReceived) + "  - over:" + howmany + " == " + cur_time + " - timer\n";
            //print(DataForCheck);
        }


        if ((SendAndReceive.DataForSending.OrderToSend - SendAndReceive.OrderReceived) < 4)
        {
            ListenToServer();
        }

        /*
        if (isstart)
        {
            ListenToServer();
            isstart = false;
        } 
        
        
        if (SendAndReceive.OrderReceived < SendAndReceive.DataForSending.OrderToSend)
        {
            
            ListenToServer();
            
        }
        */

    }

    public void Reconnect()
    {
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


    public void TalkToServer(string DataForServer)
    {

        try
        {
            //sck.Send(Encoding.UTF8.GetBytes(DataForServer));
            //print("-----=====----- " + DataForServer);
            sck.SendTo(Encoding.UTF8.GetBytes(DataForServer), endpoint);
            
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
        await Task.Run(() => ReceiveData());
        //await Task.Run(() => TryStreamReceive());
        //await Task.Run(() => TalkToServer(Data));

    }



    public void ReceiveData()
    {

        howmany++;

        try
        {

            StringBuilder messrec = new StringBuilder();
            byte[] msgBuff = new byte[256*general.SessionNumberOfPlayers];
            int size = 0;
            
            {
                //size = sck.Receive(msgBuff);
                size = sck.ReceiveFrom(msgBuff, ref remoteIp);
                messrec.Append(Encoding.UTF8.GetString(msgBuff, 0, size));
                cur_time++;
            }
            while (sck.Available > 0) ;

            isstart = true;

            if (messrec.ToString() != "" && messrec.ToString() != null)
            {
                /*
                if (bytes > 200)
                {
                    print(messrec.ToString());
                }
                */
                //print(messrec.ToString());

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

    public static void RawPacketsProcess(string RawData)
    {
        string[] getstr;
        getstr = RawData.Split('|');

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
                          
        sck.Shutdown(SocketShutdown.Both);
        sck.Close();
    }


    public void ConnectionDestroy()
    {

        sck.Shutdown(SocketShutdown.Both);
        sck.Close();
        //stream.Close();
        //client.Close();
    }


}