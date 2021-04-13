using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class testing_tcp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        encryption.InitEncodingConnection(general.Ports.tcp2324);
        
        print(connection.SendAndGetTCP($"{general.PacketID}~0~0~sbsdfdsv~df43h", general.Ports.tcp2324, general.LoginServerIP, true));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
