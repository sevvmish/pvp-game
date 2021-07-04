using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.IO;

public class test : MonoBehaviour
{
    private List<IEnumerator> tasks = new List<IEnumerator>();
    private void Start()
    {
        Screen.SetResolution(1280, 720, true);
        Camera.main.aspect = 16f / 9f;

        tasks.Add(p1("one"));
        tasks.Add(p2("two", "three"));

        StartCoroutine(mainn(tasks));
    }

    public IEnumerator mainn(List<IEnumerator> _data)
    {
        for (int i = 0; i < _data.Count; i++)
        {
            StartCoroutine(_data[i]);
        }

        yield return new WaitForSeconds(0);
    }

    public IEnumerator p1(string x)
    {
        print(x);

        yield return new WaitForSeconds(0);
    }

    public IEnumerator p2(string x, string y)
    {
        print(x + " - " + y);

        yield return new WaitForSeconds(0);
    }


}
