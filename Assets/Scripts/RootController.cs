using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RootController : MonoBehaviour
{
    [Header("Generation Behaviour")]
    public float MasterSpeedMultiplier = 1f;
    public float GrowSpeedSeconds = 1;
    private float _timeUntilNextGrowthSeconds;

    public FloatRange NodeDistance = new FloatRange(0.2f, 1);
    public float ForkChance = 0.2f;

    public FloatRange NewOriginTimeSeconds = new FloatRange(1, 5);
    private float _timeUntilNextOriginSeconds;

    public float MaxAngle = 90;
    public float OriginY = 5f;
    public float RootDecayPeriod = 1;
    private float _timeUntilNextDecay;
    [Header("Boring Things")]
    public float MaxX = 5f;
    public int MaxSearchRootDepth = 100;
    public FloatRange ZRange = new FloatRange(0, -1);
    private List<RootNode> _leaves;

    void Awake()
    {
        _leaves = new List<RootNode>();
        _timeUntilNextGrowthSeconds = GrowSpeedSeconds;
        _timeUntilNextOriginSeconds = NewOriginTimeSeconds.RandomInRange();
        _timeUntilNextDecay = RootDecayPeriod;
    }

    void Update()
    {
        if (_leaves.Count > 0)
        {
            _timeUntilNextGrowthSeconds -= Time.deltaTime * MasterSpeedMultiplier;
            if (_timeUntilNextGrowthSeconds <= 0)
            {
                List<RootNode> leavesWithOrigins;
                if (Random.Range(0f, 1f) < ForkChance)
                {
                    leavesWithOrigins = GetAllNodesWithOrigin();
                }
                else
                {
                    leavesWithOrigins = GetLeavesWithOrigin();
                }

                if (leavesWithOrigins.Count > 0)
                {
                    int leafIndex = (int)Mathf.Min(leavesWithOrigins.Count - 1, Random.Range(0f, leavesWithOrigins.Count));
                    _CreateNewNode(leavesWithOrigins[leafIndex]);
                    _timeUntilNextGrowthSeconds += GrowSpeedSeconds;
                }
            }

            // New origins?
            _timeUntilNextOriginSeconds -= Time.deltaTime * MasterSpeedMultiplier;
            if (_timeUntilNextOriginSeconds <= 0)
            {
                _StartNewOrigin();
                _timeUntilNextOriginSeconds += NewOriginTimeSeconds.RandomInRange();
            }
        }
        else
        {
            _StartNewOrigin();
        }
        _UpdateDeadRootDecay();
    }

    // ===========================================================================================
    // Public Methods
    // ===========================================================================================


    public List<RootNode> GetAllRootNodes()
    {
        return GetFullRootPaths().SelectMany(a => a).ToList();
    }

    public List<RootNode> GetLeaves()
    {
        return _leaves;
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
            while (!currentNode.IsOrphan)
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

    /// <summary>
    /// Get a list of interactable nodes
    /// Is just all nodes that are not origin or leaf
    /// </summary>
    public List<RootNode> GetInteractableNodes()
    {
        List<RootNode> nodes = new List<RootNode>();
        foreach (RootNode leaf in GetLeaves())
        {
            if (!leaf.HasOrigin) continue;

            RootNode currentNode = leaf;

            int iteration = 0;
            while (!currentNode.IsOrigin)
            {
                iteration++;
                if (iteration > MaxSearchRootDepth)
                {
                    Debug.LogError("Max root search depth exceeded, probably a cycle");
                    break;
                }

                currentNode = currentNode.Parent;
                if (!currentNode.IsOrigin) nodes.Add(currentNode);
            }
        }
        return nodes;
    }

    public void DoThing(Interactable interactable, RootAction action)
    {
        foreach (RootNode node in GetAllRootNodes())
        {
            if (node.Position == interactable.GetLocation())
            {
                DoThing(node, action);
                return;
            }
        }
    }

    public void DoThing(RootNode node, RootAction action)
    {
        switch (action)
        {
            case RootAction.Cut:
                RemoveNode(node);
                break;
        }
    }

    public List<RootNode> GetLeavesWithOrigin()
    {
        List<RootNode> nodes = new List<RootNode>();
        foreach (RootNode leaf in GetLeaves())
        {
            if (leaf.HasOrigin) nodes.Add(leaf);
        }
        return nodes;
    }

    public List<RootNode> GetAllNodesWithOrigin()
    {
        List<RootNode> nodes = new List<RootNode>();
        foreach (RootNode leaf in GetAllRootNodes())
        {
            if (leaf.HasOrigin) nodes.Add(leaf);
        }
        return nodes;
    }

    // ===========================================================================================
    // Root generation/growth
    // ===========================================================================================

    /// <summary>
    /// Create new root node, add it to leaves, return reference
    /// </summary>
    private RootNode _StartNewOrigin()
    {
        RootNode node = new RootNode(_GetValidOriginPosition(), null, true);
        _leaves.Add(node);
        return node;
    }

    private Vector3 _GetValidOriginPosition()
    {
        return _ClampX(new Vector3(Random.Range(-10f, 10f), OriginY, ZRange.RandomInRange()));
    }

    private RootNode _CreateNewNode(RootNode parent)
    {
        RootNode node = new RootNode(_GetNextNodePosition(parent), parent, false);
        _leaves.Remove(parent);
        _leaves.Add(node);
        return node;
    }

    private Vector3 _GetNextNodePosition(RootNode parent)
    {
        float r = Random.Range(-1f, 1f);
        Vector3 dir = new Vector3(r, r - 1, 0).normalized;
        return _ClampX(parent.Position + dir * NodeDistance.RandomInRange());
    }

    public void RemoveNode(RootNode node)
    {
        // Remove from leaves (if in)
        _leaves.Remove(node);
        // Is node even known about?
        List<RootNode> children = _GetChildren(node);
        if (children.Count == 0) return;

        // Remove all references to node from children
        foreach (RootNode child in children)
        {
            child.Parent = null;
        }



        // Add parent to leaves
        if (!node.IsOrphan)
        {
            if (_GetChildren(node.Parent).Count == 1)
            {
                if (!_leaves.Contains(node.Parent)) _leaves.Add(node.Parent);
            }
        }
    }

    private List<RootNode> _GetChildren(RootNode parent)
    {
        List<RootNode> children = new List<RootNode>();
        foreach (RootNode node in GetAllRootNodes())
        {
            if (parent == node.Parent)
            {
                children.Add(node);
            }
        }
        return children;
    }

    private Vector3 _ClampX(Vector3 position)
    {
        Vector3 p = position;
        p.x = _ClampX(p.x);
        return p;
    }

    private float _ClampX(float unclampedX)
    {
        return Mathf.Clamp(unclampedX, -MaxX, MaxX);
    }

    private void _UpdateDeadRootDecay()
    {
        // if enough time has passed
        _timeUntilNextDecay -= Time.deltaTime;
        if (_timeUntilNextDecay > 0) return;
        _timeUntilNextDecay += RootDecayPeriod;

        // Get roots with no origin
        foreach (List<RootNode> nodes in GetFullRootPaths())
        {
            if (nodes[0].HasOrigin) continue;

            RemoveNode(nodes[nodes.Count - 1]);
            RemoveNode(nodes[0]);
        }
    }
}
