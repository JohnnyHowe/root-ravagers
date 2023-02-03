using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootController : MonoBehaviour
{
    public float MasterSpeedMultiplier = 1f;
    public float GrowSpeedSeconds = 1;
    private float _timeUntilNextGrowthSeconds = 0;

    public FloatRange NodeDistance = new FloatRange(0.2f, 1);

    public FloatRange NewOriginTimeSeconds = new FloatRange(1, 5);
    private float _timeUntilNextOriginSeconds = 0;

    public int MaxSearchRootDepth = 100;
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
                int leafIndex = (int) Mathf.Min(_leaves.Count - 1, Random.Range(0f, _leaves.Count));
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

    public List<RootNode> GetAllRootNodes()
    {
        return new List<RootNode>();
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
        return new Vector3(Random.Range(-5f, 5f), 0, 0);
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
        // TODO smarter pos choice - angles maybe?
        Vector3 dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 0), 0).normalized;
        return parent.Position + dir * NodeDistance.RandomInRange();
    }
}
