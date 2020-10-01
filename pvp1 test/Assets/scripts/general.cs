using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class general
{
    public static int GameServerTCPPort = 2323;
    public static int GameServerUDPPort = 2325;
    public static string GameServerIP = "45.67.57.30";  //134.0.116.169       45.67.57.30

    public static int LoginServerTCPPort = 2324;
    public static string LoginServerIP = "45.67.57.30";

    public static string SessionPlayerID; //playerX
    public static string SessionTicket; //sessionX

    public static int SessionNumberOfPlayers = 2;
    public static int MainPlayerClass = 2;

    public static GameObject GetPlayerByClass(int ClassNumber)
    {
        GameObject result = null;

        switch(ClassNumber)
        {
            case 1:
                result = Resources.Load<GameObject>("prefabs/warr 1 prefab");
                break;
            case 2:
                result = Resources.Load<GameObject>("prefabs/mage 1 prefab");
                break;
            case 3:
                break;

        }

        return result;
    }

}
