using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testing_tcp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        encryption.InitEncodingConnection(general.Ports.tcp2324);
                
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
