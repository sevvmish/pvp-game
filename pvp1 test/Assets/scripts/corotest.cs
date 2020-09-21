using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class corotest : MonoBehaviour
{
    
    private Socket sck;
    private int CurrentPort = 2324;
    private string CurrentIP = "45.67.57.30";
    float cur_time, cur_time1;
    int curpacketnum;

    // Start is called before the first frame update
    void Start()
    {
       
        
        
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(CurrentIP), CurrentPort);
        sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            sck.Connect(endpoint);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }

        //Reconnect();
        
    }


    public async void Update()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {
            Send("tttttttttttttttttttttttttttt");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Send("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        }

        if (cur_time>0.03f)
        {
            cur_time = 0;
            
            await Task.Run(() => ReceiveData());
        } else
        {
            cur_time += Time.deltaTime;
        }
              
    }


    void ReceiveData()
    {
        
        //sck.Send(Encoding.UTF8.GetBytes("ttttttttyyyyyyyyttttttttt"));
        StringBuilder messrec = new StringBuilder(300);
        byte[] msgBuff = new byte[255];
        int size = 0;

        {
            size = sck.Receive(msgBuff, 100, SocketFlags.None);

            messrec.Append(Encoding.UTF8.GetString(msgBuff, 0, size));
        }
        while (sck.Available > 0) ;
        if (messrec.ToString() == "")
        {
            print("nnnnnnnuuuuuuuuulllllllllllllll");
        } else {
            print(messrec.ToString());
        }
    }
    

    void Send(string packet)
    {
        sck.Send(Encoding.UTF8.GetBytes(packet));
    }

    public void OnDestroy()
    {
        sck.Shutdown(SocketShutdown.Both);
        print("destrio");
        sck.Close();
    }




}
