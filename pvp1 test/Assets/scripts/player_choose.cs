using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class player_choose : MonoBehaviour
{
    private Vector3 WarrPos = new Vector3(0, 0, 0);
    private Vector3 MagePos = new Vector3(-7, 0, 0);
    private Vector3 BarbarPos = new Vector3(-14, 0, 0);
    private Vector3 RogPos = new Vector3(-21, 0, 0);
    private Vector3 WizPos = new Vector3(-28, 0, 0);

    public Transform PlayerLine;

    public Button pl1, pl2, pl3, pl4, pl5;

    private int CurrentPlayerNumber = 1;
    private bool isBusy;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, true);
        Camera.main.aspect = 16f / 9f;
        pl1.onClick.AddListener(Click1);
        pl2.onClick.AddListener(Click2);
        pl3.onClick.AddListener(Click3);
        pl4.onClick.AddListener(Click4);
        pl5.onClick.AddListener(Click5);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isBusy)
        {
            if (pl1.interactable)
            {
                pl1.interactable = false;
            }
            if (pl2.interactable)
            {
                pl2.interactable = false;
            }
            if (pl3.interactable)
            {
                pl3.interactable = false;
            }
            if (pl4.interactable)
            {
                pl4.interactable = false;
            }
            if (pl5.interactable)
            {
                pl5.interactable = false;
            }
        }


        if (!isBusy)
        {
            if (!pl1.interactable)
            {
                pl1.interactable = true;
            }
            if (!pl2.interactable)
            {
                pl2.interactable = true;
            }
            if (!pl3.interactable)
            {
                pl3.interactable = true;
            }
            if (!pl4.interactable)
            {
                pl4.interactable = true;
            }
            if (!pl5.interactable)
            {
                pl5.interactable = true;
            }
        }
    }

    private void Click1()
    {
        CurrentPlayerNumber = 1;
        StartCoroutine(ChangePlayer(GetPlayerByNumber(CurrentPlayerNumber)));
    }

    private void Click2()
    {
        CurrentPlayerNumber = 2;
        StartCoroutine(ChangePlayer(GetPlayerByNumber(CurrentPlayerNumber)));
    }

    private void Click3()
    {
        CurrentPlayerNumber = 3;
        StartCoroutine(ChangePlayer(GetPlayerByNumber(CurrentPlayerNumber)));
    }

    private void Click4()
    {
        CurrentPlayerNumber = 4;
        StartCoroutine(ChangePlayer(GetPlayerByNumber(CurrentPlayerNumber)));
    }

    private void Click5()
    {
        CurrentPlayerNumber = 5;
        StartCoroutine(ChangePlayer(GetPlayerByNumber(CurrentPlayerNumber)));
    }

    private Vector3 GetPlayerByNumber(int Number)
    {
        Vector3 result = Vector3.zero;

        switch(Number)
        {
            case 1:
                result = WarrPos;
                break;
            case 2:
                result = MagePos;
                break;
            case 3:
                result = BarbarPos;
                break;
            case 4:
                result = RogPos;
                break;
            case 5:
                result = WizPos;
                break;

        }

        return result;
    }

    IEnumerator ChangePlayer(Vector3 NewCoords)
    {
        isBusy = true;

        for (float i=0; i<1; i+=0.05f)
        {

            PlayerLine.position = Vector3.Lerp(PlayerLine.position, NewCoords, i);

            yield return new WaitForSeconds(0.05f);
        }

        isBusy = false;

    }

}
