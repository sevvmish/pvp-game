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

        //print(connection.SendAndGetTCP($"{general.PacketID}~0~0~tester22~passpass1", general.Ports.tcp2324, general.LoginServerIP, true));
        //print(connection.SendAndGetTCP($"{general.PacketID}~0~1~tester22~{encryption.FromByteToString(encryption.GetHash384("passpass12"))}", general.Ports.tcp2324, general.LoginServerIP, true));
        //print(connection.SendAndGetTCP($"{general.PacketID}~0~2", general.Ports.tcp2324, general.LoginServerIP, true));
        print(connection.SendAndGetTCP($"{general.PacketID}~1~0~LnVY3pRjHa", general.Ports.tcp2324, general.LoginServerIP, true));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
