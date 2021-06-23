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
        PVPStatisticsPanel panel1 = new PVPStatisticsPanel("efv", GameObject.Find("Canvas").transform);    
    }



}
