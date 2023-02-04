using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Waypoint : MonoBehaviour
{
    public LineRenderer Line;
    public SpriteRenderer Renderer;

    [NonSerialized]
    public Task Task;
}
