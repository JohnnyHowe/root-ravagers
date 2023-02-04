using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Waypoint : MonoBehaviour
{
    public LineRenderer Line;

    [NonSerialized]
    public RootNode Target;
    
    // Start is called before the first frame update
    void Start()
    {
        //Line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
