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
using System.IO;

public class test : MonoBehaviour
{
    private void Start()
    {
        Screen.SetResolution(1280, 720, true);
        Camera.main.aspect = 16f / 9f;

        //PVPStatisticsPanel panel1 = new PVPStatisticsPanel("0~7~0~5~mage-2-0-0~barbarian-3-1-1~warrior-1-2-1~rogue-4-3-1~wizard-5-4-1~", GameObject.Find("Canvas").transform);    

        print(Application.dataPath);

        test11 trytest = new test11();
        trytest.tt1 = "test1";
        trytest.tt2 = "test2";

        /*
        using (FileStream fs = new FileStream(Application.dataPath +  "/Resources/user.json", FileMode.OpenOrCreate))
        {
            byte[] arr = Encoding.UTF8.GetBytes(JsonUtility.ToJson(""));
            fs.Write(arr, 0, arr.Length);

        }
        */

        /*
        using (FileStream fs = new FileStream(Application.dataPath + "/Resources/user.json", FileMode.Open))
        {
           
            byte[] arr = new byte[fs.Length];
            fs.Read(arr, 0, arr.Length);

            
            test11 newtest = new test11();
            newtest = JsonUtility.FromJson<test11>(Encoding.UTF8.GetString(arr));
            print(newtest.tt1 + " - " + newtest.tt2);

        }
        */
    }

    [Serializable]
    public class test11
    {
        public string tt1;
        public string tt2;
    }

}
