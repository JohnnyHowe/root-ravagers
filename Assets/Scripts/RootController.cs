using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootController : MonoBehaviour
{
    [Header("Generation Behaviour")]
    public float MasterSpeedMultiplier = 1f;
    public float GrowSpeedSeconds = 1;
    private float _timeUntilNextGrowthSeconds = 0;

    public FloatRange NodeDistance = new FloatRange(0.2f, 1);

    public FloatRange NewOriginTimeSeconds = new FloatRange(1, 5);
    private float _timeUntilNextOriginSeconds = 0;

    public float MaxAngle = 90;

    [Header("Boring Things")]
    public int MaxSearchRootDepth = 100;
    public FloatRange ZRange = new FloatRange(0, -1);
    private List<RootNode> _leaves;

    void Awake()
    {
        _leaves = new List<RootNode>();
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

    /// <summary>
    /// Get a list of interactable nodes
    /// Is just all nodes that are not origin or leaf
    /// </summary>
    public List<RootNode> GetInteractableNodes()
    {
        List<RootNode> nodes = new List<RootNode>();
        foreach (RootNode leaf in GetLeaves())
        {
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

    // ===========================================================================================
    // Root generation/growth
    // ===========================================================================================

    /// <summary>
    /// Create new root node, add it to leaves, return reference
    /// </summary>
    private RootNode _StartNewOrigin()
    {
        RootNode node = new RootNode(_GetValidOriginPosition(), null);
        _leaves.Add(node);
        return node;
    }

    private Vector3 _GetValidOriginPosition()
    {
        return new Vector3(Random.Range(-10f, 10f), 0, Random.Range(0, -1f));
    }

    private RootNode _CreateNewNode(RootNode parent)
    {
        RootNode node = new RootNode(_GetNextNodePosition(parent), parent);
        _leaves.Remove(parent);
        _leaves.Add(node);
        return node;
    }

    private Vector3 _GetNextNodePosition(RootNode parent)
    {
        float lastAngleFromNorth;
        if (parent.IsOrigin) {
            lastAngleFromNorth = 180;
        } else {
            lastAngleFromNorth = Vector3.SignedAngle(Vector3.up, parent.Position - parent.Parent.Position, Vector3.forward);
        }

        float angleChange = Random.Range(-90f, 90f);
        float nextAngleFromNorth = lastAngleFromNorth + angleChange;

        if (nextAngleFromNorth >= 270) nextAngleFromNorth = 270;
        if (nextAngleFromNorth >= -90 && nextAngleFromNorth <= 0) nextAngleFromNorth = 270;
        if (nextAngleFromNorth >= 0 && nextAngleFromNorth <= 90) nextAngleFromNorth = 90;


        // TODO smarter pos choice - angles maybe?
        Vector3 dir = new Vector3(Random.Range(-1f, 1f), ZRange.RandomInRange(), 0).normalized;
        return parent.Position + dir * NodeDistance.RandomInRange();
    }
}
