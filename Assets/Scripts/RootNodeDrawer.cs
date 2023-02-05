using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RootNodeDrawer : MonoBehaviour
{
    public LineRenderer RendererPrototype;
    private List<LineRenderer> _lineRenderers;

    public SpriteRenderer InteractablePrototype;
    private List<SpriteRenderer> _interactableRenderers;

    private RootController _rootController;
    private AntController _antController;
    public float InteractableSize = 0.1f;
    public float TargetInteractableSize = 2;

    void Start()
    {
        _rootController = GetComponent<RootController>();
        _antController = GameObject.FindObjectOfType<AntController>();
        _lineRenderers = new List<LineRenderer>();
        _interactableRenderers = new List<SpriteRenderer>();
        RendererPrototype.gameObject.SetActive(false);
        InteractablePrototype.gameObject.SetActive(false);
    }

    void Update()
    {
        _DrawRoots();
        _DrawInteractables();
    }

    private void _DrawRoots()
    {
        List<List<RootNode>> rootPaths = _rootController.GetFullRootPaths();
        for (int i = 0; i < rootPaths.Count; i++)
        {
            var positions = rootPaths[i].Select(node => node.Position).Reverse().ToArray();
            LineRenderer renderer = _GetLineRenderer(i);
            renderer.gameObject.SetActive(true);
            renderer.positionCount = positions.Length;
            renderer.SetPositions(positions);
        }
        for (int i = rootPaths.Count; i < _lineRenderers.Count; i++)
        {
            _lineRenderers[i].gameObject.SetActive(false);
        }
        _HideUnusedRenderers(rootPaths.Count);
    }

    private LineRenderer _GetLineRenderer(int i)
    {
        while (i >= _lineRenderers.Count)
        {
            LineRenderer renderer = Instantiate(RendererPrototype);
            renderer.gameObject.SetActive(true);
            renderer.transform.parent = RendererPrototype.transform.parent;
            _lineRenderers.Add(renderer);
        }
        _lineRenderers[i].gameObject.SetActive(true);
        return _lineRenderers[i];
    }

    private void _DrawInteractables()
    {
        List<RootNode> interactableNodes = _rootController.GetInteractableNodes();
        _HideUnusedIneractables(interactableNodes.Count);
        for (int i = 0; i < interactableNodes.Count; i++)
        {
            SpriteRenderer s = _GetInteractableSpriteRenderer(i);
            Vector3 p = interactableNodes[i].Position;
            p.z = 0;
            s.gameObject.transform.SetPositionAndRotation(p, Quaternion.identity);
            if (_antController.TargetedInteractable == interactableNodes[i])
            {
                s.gameObject.transform.localScale = Vector3.one * TargetInteractableSize;
            }
            else
            {
                s.gameObject.transform.localScale = Vector3.one * InteractableSize;
            }
        }
    }

    private void _HideUnusedIneractables(int nUsed)
    {
        for (int i = nUsed; i < _interactableRenderers.Count; i++)
        {
            _interactableRenderers[i].gameObject.SetActive(false);
        }
    }

    private void _HideUnusedRenderers(int nUsed)
    {
        for (int i = nUsed; i < _lineRenderers.Count; i++)
        {
            _lineRenderers[i].gameObject.SetActive(false);
        }
    }

    private SpriteRenderer _GetInteractableSpriteRenderer(int i)
    {
        while (i >= _interactableRenderers.Count)
        {
            SpriteRenderer renderer = Instantiate(InteractablePrototype);
            renderer.gameObject.SetActive(true);
            renderer.transform.parent = InteractablePrototype.transform.parent;
            _interactableRenderers.Add(renderer);
        }
        _interactableRenderers[i].gameObject.SetActive(true);
        return _interactableRenderers[i];
    }
}
