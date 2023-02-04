using UnityEngine;

[System.Serializable]
public class RootNode
{
    public RootNode(Vector3 position, RootNode parent, bool isOrigin)
    {
        Position = position;
        Parent = parent;
        IsOrigin = isOrigin;
    }

    public RootNode Parent;
    public Vector3 Position;
    public bool IsOrigin;
    public bool IsOrphan {
        get => Parent == null;
    }
    public bool HasOrigin {
        get {
            if (IsOrigin) return true;
            if (IsOrphan) return false;
            return Parent.HasOrigin;
        }
    }
}