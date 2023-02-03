using UnityEngine;

[System.Serializable]
public class RootNode
{
    public RootNode(Vector3 position, RootNode parent)
    {
        Position = position;
        Parent = parent;
    }

    public RootNode Parent;
    public Vector3 Position;
    public bool IsOrigin
    {
        get => Parent == null;
    }
}