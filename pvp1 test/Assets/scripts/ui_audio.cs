using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_audio : MonoBehaviour
{
    private static AudioSource _source;
    private static AudioClip SimpleClick1, SimpleClick2;
    // Start is called before the first frame update
    void Start()
    {
        _source = this.gameObject.GetComponent<AudioSource>();
        SimpleClick1 = Resources.Load<AudioClip>("sounds/swing very huge");
        SimpleClick1 = Resources.Load<AudioClip>("sounds/swing very huge");
    }

    public static void ClickSound()
    {
        _source.clip = SimpleClick1;
        _source.Play();
    }

}
