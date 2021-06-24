using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class test : MonoBehaviour
{
    private void Start()
    {
        Screen.SetResolution(1280, 720, true);
        Camera.main.aspect = 16f / 9f;

        PVPStatisticsPanel panel1 = new PVPStatisticsPanel("0~7~0~5~mage-2-0-0~barbarian-3-1-1~warrior-1-2-1~rogue-4-3-1~wizard-5-4-1~", GameObject.Find("Canvas").transform);    
    }



}
