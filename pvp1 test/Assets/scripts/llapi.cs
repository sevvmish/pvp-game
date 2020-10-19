using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class llapi : MonoBehaviour
{

    ConnectionConfig config;
    int myUnreliableChannelId;
    HostTopology topology;
    int hostId, connectionId, bufferLength=100;
    byte error;
    float cur_time;

    // Start is called before the first frame update
    void Start()
    {
        NetworkTransport.Init();

        config = new ConnectionConfig();
        
        myUnreliableChannelId = config.AddChannel(QosType.Unreliable);
        topology = new HostTopology(config, 1);
        hostId = NetworkTransport.AddHost(topology, 0, null);
        connectionId = NetworkTransport.Connect(hostId, "45.67.57.30", 2325, 0, out error);
    }

    // Update is called once per frame
    void Update()
    {
        if (cur_time>1)
        {
            cur_time = 0;

            byte[] buffer = Encoding.Unicode.GetBytes("123456");     //Encoding.UTF8.GetBytes("123456");// Encoding.ASCII.GetBytes("test testovich");
            bufferLength = buffer.Length;

            NetworkTransport.Send(hostId, connectionId, myUnreliableChannelId, buffer, bufferLength, out error);


            /*
            int outHostId;
            int outConnectionId;
            int outChannelId;
            byte[] buffer1 = new byte[1024];
            int bufferSize = 1024;
            int receiveSize;
            
            NetworkEventType evnt = NetworkTransport.Receive(out outHostId, out outConnectionId, out outChannelId, buffer, bufferSize, out receiveSize, out error);
            switch (evnt)
            {
                case NetworkEventType.ConnectEvent:
                    if (outHostId == hostId &&
                        outConnectionId == connectionId &&
                        (NetworkError)error == NetworkError.Ok)
                    {
                        Debug.Log("Connected");
                    }
                    break;
                case NetworkEventType.DisconnectEvent:
                    if (outHostId == hostId &&
                        outConnectionId == connectionId)
                    {
                        Debug.Log("Connected, error:" + error.ToString());
                    }
                    break;
            }

            */

        }
        else
        {
            cur_time += Time.deltaTime;
        }
    }
}
