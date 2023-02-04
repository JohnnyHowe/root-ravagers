using System.Collections.Generic;
using UnityEngine;


public interface Interactable
{
    List<TaskType> GetTaskTypes();
    void OnTaskComplete();  // e.g. cutting through wood, using item
    Vector3 GetLocation();
    void TryMoveItem(Vector3 newPosition);
}