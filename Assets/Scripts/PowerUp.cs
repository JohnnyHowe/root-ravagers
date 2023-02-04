using System.Collections.Generic;
using UnityEngine;


public abstract class PowerUp : MonoBehaviour, Interactable
{
    public List<TaskType> GetTaskTypes()
    {
        return new List<TaskType>() {
            TaskType.Pickup,
            TaskType.Use
        };
    }

    public bool IsAvailable = true;

    public virtual void OnTaskComplete() { }
    public void TryMoveItem(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
    public Vector3 GetLocation()
    {
        return transform.position;
    }
}