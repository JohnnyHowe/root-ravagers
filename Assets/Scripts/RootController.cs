using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootController : MonoBehaviour
{
    public Vector3 TestNodes;

    public List<RootNode> GetOriginRootNodes() {
        return new List<RootNode>();
    }

    public List<RootNode> GetAllRootNodes()
    {
        return new List<RootNode>();
    }

    void Update()
    {
        _DrawRoots();
    }

    private void _DrawRoots() {

    }
}
