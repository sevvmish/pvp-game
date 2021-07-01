﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class general 
{
    public static bool isRus = true;
    
    public enum Ports
    {
        tcp2323=2323,
        tcp2324=2324,
        tcp2326=2326,
        udp2325=2325
    }

    public static int GameServerTCPPort = 2323;
    public static int GameServerUDPPort = 2325;
    public static string HUB1_ip = "31.172.66.150";
    public static string GameServerIP;  //134.0.116.169       45.67.57.30      185.18.53.239    usa - 170.130.40.170 //45.67.58.92   fornex dotnet small //91.228.155.57   //192.168.0.103  //31.172.66.150

    public static int SetupServerTCPPort = (int)Ports.tcp2326;
    public static string SetupServerIP = "31.172.66.150"; //185.18.53.239   "45.67.57.30"

    public static int LoginServerTCPPort = (int)Ports.tcp2324;
    public static string LoginServerIP = "31.172.66.150"; //185.18.53.239   "45.67.57.30"

    public static string SessionPlayerID; //playerX
    public static string SessionTicket; //sessionX
    public static string CurrentTicket; //current ticket

    public static List<SessionData> DataForSession = new List<SessionData>();

    public static encryption PlayerEncryption = new encryption();
    public static byte[] SecretKey;
    public static string PacketID;

    public static bool isRestarting;

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
        //SessionNumberOfPlayers = DataForSession.Count;
    }

    public static string get_random_set_of_symb(int nub_of_symb)
    {
        string[] arr_name = { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "a", "s", "d", "f", "g", "h", "j", "k", "l", "z", "x", "c", "v", "b", "n", "m", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "Z", "X", "C", "V", "B", "N", "M", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
        string result = "";
        System.Random rnd = new System.Random();
        for (int i = 0; i < nub_of_symb; i++)
        {
            result = result + arr_name[rnd.Next(0, arr_name.Length - 1)];
        }

        return result;
    }

    public static GameObject GetPlayerByClass(int ClassNumber)
    {
        GameObject result = null;

        switch (ClassNumber)
        {
            case -2:
                result = Resources.Load<GameObject>("prefabs/pugalo heavy");
                break;
            case -1:
                result = Resources.Load<GameObject>("prefabs/pugalo light");
                break;
            case 1:
                result = Resources.Load<GameObject>("prefabs/warr 1 prefab");
                break;
            case 2:
                result = Resources.Load<GameObject>("prefabs/elementalist 1 prefab");
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
    public int Zone_Type;
    public string PlayerName;
    public int PlayerClass;
    public int PlayerTeam;
    public int Spell1;
    public int Spell2;
    public int Spell3;
    public int Spell4;
    public int Spell5;
    public int Spell6;

    public SessionData(int Order, int Zone, string Name, int Class, int Team, int Spel1, int Spel2, int Spel3, int Spel4, int Spel5, int Spel6)
    {
        PlayerOrder = Order;
        Zone_Type = Zone;
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

        return $"{general.PacketID}~0~4~{general.SessionPlayerID}~{general.SessionTicket}";
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
                
        general.DataForSession.Add(new SessionData(int.Parse(getData[3]), int.Parse(getData[4]), getData[5], int.Parse(getData[6]), int.Parse(getData[7]), int.Parse(getData[8]), int.Parse(getData[9]), int.Parse(getData[10]), int.Parse(getData[11]), int.Parse(getData[12]), 997));

        int ort = 13;
        for (int i = 0; i < (int.Parse(getData[2]) - 1); i++)
        {
            general.DataForSession.Add(new SessionData(int.Parse(getData[ort]), 0, getData[ort + 2], int.Parse(getData[ort + 2]), int.Parse(getData[ort + 3]), 0, 0, 0, 0, 0, 0));
            ort += 4;
        }
    }

}


/*
public static class sr
{
    public static bool isConnectionError;

    
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
*/

public class MessageInfo : MonoBehaviour
{
    public GameObject mess_obj;    
    private TextMeshProUGUI mess_text_mesh;
        
    public MessageInfo(GameObject obj)
    {                
        mess_obj = obj;
        mess_obj.SetActive(false);
        mess_text_mesh = mess_obj.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();       
    }
        
    public IEnumerator process_error(string _code, general.Ports port)
    {
        bool is_restarting = false;
        bool is_fast_restaritng = false;

        switch (_code)
        {            
            case "ecu":
                is_restarting = true;
                break;
            case "egt":
                is_restarting = true;
                break;
            case "err":
                is_restarting = true;
                break;
            case "con_err":
                is_restarting = true;
                break;
            case "nst":
                is_restarting = true;
                break;
            case "ns":
                is_fast_restaritng = true;
                break;
        }

        if (is_fast_restaritng)
        {
            encryption.InitEncodingConnection(port);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {

            mess_obj.SetActive(true);
            mess_text_mesh.text = codes.GetCodeResult(_code);
            yield return new WaitForSeconds(3f);
            mess_obj.SetActive(false);

            if (is_restarting)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    public IEnumerator process_error(string _code)
    {
        bool is_restarting = false;
        
        switch (_code)
        {
            case "ecu":
                is_restarting = true;
                break;
            case "egt":
                is_restarting = true;
                break;
            case "err":
                is_restarting = true;
                break;
            case "con_err":
                is_restarting = true;
                break;
            case "nst":
                is_restarting = true;
                break;
            case "ns":
                is_restarting = true;
                break;
        }

        

        mess_obj.SetActive(true);
        mess_text_mesh.text = codes.GetCodeResult(_code);
        yield return new WaitForSeconds(2f);
        mess_obj.SetActive(false);

        if (is_restarting)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
    }
}

public static class codes
{
    public static string GetCodeResult(string CodeResult)
    {
        string result = null;

        switch (CodeResult)
        {
            case "wll":
                result = languages.lang.wlltext; //long login
                break;
            case "wds":
                result = languages.lang.wdstext; //wrong symbols
                break;
            case "wlp":
                result = languages.lang.wlptext; //long pass
                break;
            case "uae":
                result = languages.lang.uaetext; //user allready exists
                break;
            case "ecu":
                result = languages.lang.ecutext; // err creating login
                break;
            case "ude":
                result = languages.lang.udetext; //no user with such name
                break;
            case "wp":
                result = languages.lang.wptext; //wrong pass
                break;
            case "load":
                result = languages.lang.loadtext; //loading...
                break;

            case "egt":
                result = languages.lang.egttext; //err getting ticket
                break;
            case "err":
                result = languages.lang.errtext; //just err
                break;
            case "con_err":
                result = languages.lang.conerrtext;
                break;
            case "nst":
                result = languages.lang.nsttext ; //loading...
                break;
            case "wcn":
                result = languages.lang.wcntext ; //loading...
                break;
            case "cae":
                result = languages.lang.caetext; //loading...
                break;
            case "tae":
                result = languages.lang.taetext ; //loading...
                break;
            case "nd":
                result = languages.lang.errtext; //loading...
                break;
            case "ns":
                result = languages.lang.nsctext; //loading...
                break;
            case "eit":
                result = languages.lang.eittext; //loading...
                break;
            case "nss":
                result = languages.lang.nsstext; //loading...
                break;
            case "rsn":
                result = languages.lang.nsstext; //loading...
                break;
            case "dbe":
                result = languages.lang.dbetext; //loading...
                break;

            default:
                result = "none";
                break;
        }

        return result;

    }
}


