using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class check_borders : MonoBehaviour
{

    public GameObject[] borders;

    // Start is called before the first frame update
    void Start()
    {
        string result = "";

        for (int i=0; i<borders.Length; i++)
        {
            result = result +
                (borders[i].transform.position.x - borders[i].transform.localScale.x / 2).ToString().Replace(',','.') + "," +
                (borders[i].transform.position.z - borders[i].transform.localScale.z / 2).ToString().Replace(',', '.') + "," +
                (borders[i].transform.position.x + borders[i].transform.localScale.x / 2).ToString().Replace(',', '.') + "," +
                (borders[i].transform.position.z + borders[i].transform.localScale.z / 2).ToString().Replace(',', '.') + ",\n";
        }

        print(result);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
