using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class start_after : MonoBehaviour
{


    public GameObject[] objects;
    public float[] _after_s;
    public float[] _after_s_end;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(false);
            StartCoroutine(start_after_sec(_after_s[i], objects[i]));
        }

        
        for (int i = 0; i < _after_s_end.Length; i++)
        {
            if (_after_s_end[i]>0)
            {                
                StartCoroutine(end_after_sec(_after_s_end[i], objects[i]));
            }
            
        }
    }


    IEnumerator start_after_sec(float _seconds, GameObject obj)
    {
        yield return new WaitForSeconds(_seconds);
        if (!obj.activeSelf) obj.SetActive(true);
                
    }

    IEnumerator end_after_sec(float _seconds, GameObject obj)
    {
        
        yield return new WaitForSeconds(_seconds);
        if (obj.activeSelf)
        {
            obj.SetActive(false);
           
        }

    }


}
