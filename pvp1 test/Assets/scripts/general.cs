using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;

public class general
{
    public static int GameServerTCPPort = 2323;
    public static int GameServerUDPPort = 2325;
    public static string HUB1_ip = "45.67.57.30";
    public static string GameServerIP;  //134.0.116.169       45.67.57.30      185.18.53.239

    public static int SetupServerTCPPort = 2326;
    public static string SetupServerIP = "45.67.57.30"; //185.18.53.239   "45.67.57.30"

    public static int LoginServerTCPPort = 2324;
    public static string LoginServerIP = "45.67.57.30"; //185.18.53.239   "45.67.57.30"

    public static string SessionPlayerID; //playerX
    public static string SessionTicket; //sessionX
    public static string CurrentTicket; //current ticket

    public static List<SessionData> DataForSession = new List<SessionData>();

    public static string CharacterName;
    public static int CharacterType;
    public static int SessionNumberOfPlayers; //5
    public static int MainPlayerClass;
    public static string MainPlayerName;
    public static int MainPlayerOrder;
    public static int MainPlayerTeam;
    public const float Tick = 0.05f;


    public static void SetSessionData()
    {
        MainPlayerOrder = DataForSession[0].PlayerOrder;
        MainPlayerName = DataForSession[0].PlayerName;
        MainPlayerClass = DataForSession[0].PlayerClass;
        MainPlayerTeam = DataForSession[0].PlayerTeam;
        SessionNumberOfPlayers = DataForSession.Count;
    }

    public static GameObject GetPlayerByClass(int ClassNumber)
    {
        GameObject result = null;

        switch (ClassNumber)
        {
            case -1:
                result = Resources.Load<GameObject>("prefabs/warr 1 prefab");
                break;
            case 1:
                result = Resources.Load<GameObject>("prefabs/warr 1 prefab");
                break;
            case 2:
                result = Resources.Load<GameObject>("prefabs/mage 1 prefab");
                break;
            case 3:
                result = Resources.Load<GameObject>("prefabs/barbarian 1 prefab");
                break;
            case 4:
                result = Resources.Load<GameObject>("prefabs/rogue 1 prefab");
                break;
            case 5:
                result = Resources.Load<GameObject>("prefabs/wizard 1 prefab");
                break;

        }

        return result;
    }

}


public struct SessionData
{
    public int PlayerOrder;
    public string PlayerName;
    public int PlayerClass;
    public int PlayerTeam;
    public int Spell1;
    public int Spell2;
    public int Spell3;
    public int Spell4;
    public int Spell5;
    public int Spell6;

    public SessionData(int Order, string Name, int Class, int Team, int Spel1, int Spel2, int Spel3, int Spel4, int Spel5, int Spel6)
    {
        PlayerOrder = Order;
        PlayerName = Name;
        PlayerClass = Class;
        PlayerTeam = Team;
        Spell1 = Spel1;
        Spell2 = Spel2;
        Spell3 = Spel3;
        Spell4 = Spel4;
        Spell5 = Spel5;
        Spell6 = Spel6;
    }


    public static string SendSessionDataRequest()
    {

        return "0~4~" + general.SessionPlayerID + "~" + general.SessionTicket;
    }


    public static void GetDataSessionPlayers(string Data)
    {
        //general.DataForSession
        if (general.DataForSession.Count > 0)
        {
            for (int i = 0; i < general.DataForSession.Count; i++)
            {
                general.DataForSession.Remove(general.DataForSession[i]);
            }
        }

        string[] getData = Data.Split('~');
        int order = int.Parse(getData[0]);
        int specification = int.Parse(getData[1]);
        general.SessionNumberOfPlayers = int.Parse(getData[2]);
        general.DataForSession.Add(new SessionData(int.Parse(getData[3]), getData[4], int.Parse(getData[5]), int.Parse(getData[6]), int.Parse(getData[7]), int.Parse(getData[8]), int.Parse(getData[9]), int.Parse(getData[10]), int.Parse(getData[11]), int.Parse(getData[12])));

        int ort = 13;
        for (int i = 0; i < (general.SessionNumberOfPlayers - 1); i++)
        {
            general.DataForSession.Add(new SessionData(int.Parse(getData[ort]), getData[ort + 1], int.Parse(getData[ort + 2]), int.Parse(getData[ort + 3]), 0, 0, 0, 0, 0, 0));
            ort += 4;
        }
    }

}



public static class sr
{
    public static bool isConnectionError;

    public static void isServerOff()
    {
        if (sr.SendAndGetLoginSetup("0~0~0~0") != "online")
        {
            Debug.Log("BBBBBBAAAAAAAAAADDDDDDDDD");
            sr.isConnectionError = true;

        }

    }


    public static string SendAndGetOnlySetup(string DataForSending)
    {
        int CurrentPort = general.SetupServerTCPPort;
        string CurrentIP = general.SetupServerIP;
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
            isConnectionError = true;
            Debug.Log(ex);
            result = ex.ToString();

            /*
            sck.Shutdown(SocketShutdown.Both);
            sck.Close();
            */
            return result;

        }
        //===============================SEND======================================
        try
        {
            sck.Send(Encoding.UTF8.GetBytes(DataForSending));
        }
        catch (Exception ex)
        {
            isConnectionError = true;
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
            isConnectionError = true;
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



    public static string SendAndGetLoginSetup(string DataForSending)
    {
        int CurrentPort = general.LoginServerTCPPort;
        string CurrentIP = general.LoginServerIP;
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
            isConnectionError = true;
            Debug.Log(ex);
            result = ex.ToString();

            /*
            sck.Shutdown(SocketShutdown.Both);
            sck.Close();
            */
            return result;

        }
        //===============================SEND======================================
        try
        {
            sck.Send(Encoding.UTF8.GetBytes(DataForSending));
        }
        catch (Exception ex)
        {
            isConnectionError = true;
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
            isConnectionError = true;
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

