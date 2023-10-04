using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public static test Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("already has");
        }
        Instance = this;
    }

    public void someFunct()
    {
        Debug.Log("EHEH");
    }
}
