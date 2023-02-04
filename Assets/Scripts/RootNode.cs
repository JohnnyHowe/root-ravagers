using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RootNode : Interactable
{
    public RootNode(Vector3 position, RootNode parent, bool isOrigin)
    {
        Position = position;
        Parent = parent;
        IsOrigin = isOrigin;
    }

    public List<TaskType> GetTaskTypes()
    {
        return new List<TaskType> { TaskType.Cut, TaskType.Use };
    }

    public RootNode Parent;
    public Vector3 Position;
    public bool IsOrigin;
    public bool IsOrphan
    {
        get => Parent == null;
    }
    public bool HasOrigin
    {
        get
        {
            if (IsOrigin) return true;
            if (IsOrphan) return false;
            return Parent.HasOrigin;
        }
    }
}