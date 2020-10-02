using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class plchoose : MonoBehaviour
{
    public Button pl1, pl2, pl3, pl4, pl5, pl6, pl7, pl8;
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, true);
        Camera.main.aspect = 16f / 9f;
        Application.targetFrameRate = 60;
        pl1.onClick.AddListener(play1);
        pl2.onClick.AddListener(play2);
        pl3.onClick.AddListener(play3);
        pl4.onClick.AddListener(play4);
        pl5.onClick.AddListener(play5);
        pl6.onClick.AddListener(play6);
        pl7.onClick.AddListener(play7);
        pl8.onClick.AddListener(play8);

    }

    

    void play1()
    {
        general.SessionPlayerID = "player1";
        general.SessionTicket = "session";
                
        SceneManager.LoadScene("SampleScene");
    }

    void play2()
    {
        general.SessionPlayerID = "player2";
        general.SessionTicket = "session";
        
        SceneManager.LoadScene("SampleScene");
    }

    void play3()
    {
        general.SessionPlayerID = "player3";
        general.SessionTicket = "session1";
        GetPlayersData();
        SceneManager.LoadScene("SampleScene");
    }

    void play4()
    {
        general.SessionPlayerID = "player4";
        general.SessionTicket = "session1";
        GetPlayersData();
        SceneManager.LoadScene("SampleScene");
    }

    void play5()
    {
        general.SessionPlayerID = "player5";
        general.SessionTicket = "session2";
        GetPlayersData();
        SceneManager.LoadScene("SampleScene");
    }

    void play6()
    {
        general.SessionPlayerID = "player6";
        general.SessionTicket = "session2";
        GetPlayersData();
        SceneManager.LoadScene("SampleScene");
    }

    void play7()
    {
        general.SessionPlayerID = "player7";
        general.SessionTicket = "session3";
        GetPlayersData();
        SceneManager.LoadScene("SampleScene");
    }

    void play8()
    {
        general.SessionPlayerID = "player8";
        general.SessionTicket = "session3";
        GetPlayersData();
        SceneManager.LoadScene("SampleScene");
    }


    void GetPlayersData()
    {
        
    }


}
