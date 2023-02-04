using System;
using UnityEngine;


public class Task
{
    public TaskType TypeOfTask;
    public Vector3 Position;
    public Interactable Target;

    public float TimeRemaining;

    public Func<bool> PerformAction = () => true;

}