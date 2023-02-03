using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootController : MonoBehaviour
{
    public int MaxSearchRootDepth = 100;
    public List<RootNode> Leaves;

    void Awake()
    {
        // Test data
        RootNode o1 = new RootNode(Vector3.zero, null);
        RootNode n1 = new RootNode(Vector3.down, o1);
        RootNode n2 = new RootNode(new Vector3(1, -2, 0), n1);

        Leaves = new List<RootNode>() { n2 };

        {
            RootNode origin = new RootNode(new Vector3(-1, 0, 0), null);
            RootNode node1 = new RootNode(new Vector3(-2, -1, 0), origin);
            RootNode node2 = new RootNode(new Vector3(-2, -4, 0), node1);
            RootNode node3 = new RootNode(new Vector3(0, -4, 0), node2);
            Leaves.Add(node3);

            RootNode b1 = new RootNode(new Vector3(-4, -2, 0), node1);
            RootNode b2 = new RootNode(new Vector3(-5, -3, 0), b1);
            Leaves.Add(b2);
        }
        // foreach (List<RootNode> nl in GetFullRootPaths()) {
        //     foreach (RootNode n in nl) {
        //         Debug.Log(n.Position);
        //     }
        //     Debug.Log("");
        // }
    }

    public List<RootNode> GetAllRootNodes()
    {
        return new List<RootNode>();
    }

    public List<RootNode> GetLeaves()
    {
        return Leaves;
    }

    public List<List<RootNode>> GetFullRootPaths()
    {
        List<List<RootNode>> paths = new List<List<RootNode>>();
        foreach (RootNode leaf in GetLeaves())
        {
            List<RootNode> path = new List<RootNode>();
            RootNode currentNode = leaf;
            path.Add(currentNode);

            int iteration = 0;
            while (!currentNode.IsOrigin)
            {
                if (iteration > MaxSearchRootDepth)
                {
                    Debug.LogError("Max root search depth exceeded, probably a cycle");
                    break;
                }
                currentNode = currentNode.Parent;
                path.Add(currentNode);
            }
            paths.Add(path);
        }
        return paths;
    }
}
