using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_shake_starter : MonoBehaviour
{

    public float MaxDistance;
    public float TimeDelay;
    public float HowLong;
    public float Amplitude;
    public float Frequency;
    private Transform sourcer;
    private float distance;

    private void OnEnable()
    {

        if (MaxDistance==0)
        {
            MaxDistance = 0.01f;
        }
        
        if (HowLong>0.2f)
        {            
            StartCoroutine(DoShake());
        }

       
    }

    IEnumerator DoShake()
    {
        sourcer = GameObject.Find("Main Player").GetComponent<Transform>();
        distance = Vector3.Distance(this.transform.position, sourcer.transform.position);
        
        yield return new WaitForSeconds(TimeDelay);
        distance = Vector3.Distance(this.transform.position, sourcer.transform.position);
        camera_shake.Instance.SimpleHit(0, HowLong, distance/MaxDistance, Amplitude, Frequency);
       

        

    }


}
