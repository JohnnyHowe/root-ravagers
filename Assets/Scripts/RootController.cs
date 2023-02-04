using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootController : MonoBehaviour
{
    [Header("Generation Behaviour")]
    public float MasterSpeedMultiplier = 1f;
    public float GrowSpeedSeconds = 1;
    private float _timeUntilNextGrowthSeconds;

    public FloatRange NodeDistance = new FloatRange(0.2f, 1);

    public FloatRange NewOriginTimeSeconds = new FloatRange(1, 5);
    private float _timeUntilNextOriginSeconds;

    public float MaxAngle = 90;
    public float OriginY = 5f;

    [Header("Boring Things")]
    public int MaxSearchRootDepth = 100;
    public FloatRange ZRange = new FloatRange(0, -1);
    private List<RootNode> _leaves;

    void Awake()
    {
        _leaves = new List<RootNode>();
        _timeUntilNextGrowthSeconds = GrowSpeedSeconds;
        _timeUntilNextOriginSeconds = NewOriginTimeSeconds.RandomInRange();
    }

    void Update()
    {
        if (_leaves.Count > 0)
        {
            _timeUntilNextGrowthSeconds -= Time.deltaTime * MasterSpeedMultiplier;
            if (_timeUntilNextGrowthSeconds <= 0)
            {
                int leafIndex = (int)Mathf.Min(_leaves.Count - 1, Random.Range(0f, _leaves.Count));
                _CreateNewNode(_leaves[leafIndex]);
                _timeUntilNextGrowthSeconds += GrowSpeedSeconds;
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
    }

    // ===========================================================================================
    // Public Methods
    // ===========================================================================================

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

    public void DoThing(RootNode node, RootAction action)
    {
        switch (action)
        {
            case RootAction.Cut:
                _RemoveNode(node);
                break;
        }
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
        return new Vector3(Random.Range(-10f, 10f), OriginY, ZRange.RandomInRange());
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
        return parent.Position + dir * NodeDistance.RandomInRange();
    }

    private void _RemoveNode(RootNode node)
    {
        foreach (RootNode child in _GetChildren(node)) {
            child.Parent = null;
        }
        if (!node.IsOrphan) {
            if (!_leaves.Contains(node.Parent)) _leaves.Add(node.Parent);
        }
    }

    private List<RootNode> _GetChildren(RootNode parent)
    {
        List<RootNode> children = new List<RootNode>();
        foreach (RootNode node in _GetAllRootNodes()) {
            if (parent == node.Parent) {
                children.Add(node);
            }
        }
        return children;
    }

    private List<RootNode> _GetAllRootNodes()
    {
        List<RootNode> nodes = new List<RootNode>();
        foreach (RootNode leaf in GetLeaves())
        {
            RootNode currentNode = leaf;
            nodes.Add(currentNode);

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
                nodes.Add(currentNode);
            }
        }
        return nodes;
    }
}
