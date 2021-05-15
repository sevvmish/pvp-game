using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class start_after : MonoBehaviour
{


    public GameObject[] objects;
    public float[] _after_s;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(false);
            StartCoroutine(start_after_sec(_after_s[i], objects[i]));
        }
    }


    IEnumerator start_after_sec(float _seconds, GameObject obj)
    {
        yield return new WaitForSeconds(_seconds);
        obj.SetActive(true);
                
    }
}
