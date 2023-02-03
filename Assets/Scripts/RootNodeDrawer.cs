using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNodeDrawer : MonoBehaviour
{
    public LineRenderer RendererPrototype;
    private List<LineRenderer> _lineRenderers;

    public SpriteRenderer InteractablePrototype;
    private List<SpriteRenderer> _interactableRenderers;

    private RootController _rootController;

    void Start()
    {
        _rootController = GetComponent<RootController>();
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
            List<Vector3> positions = new List<Vector3>();
            foreach (RootNode node in rootPaths[i])
            {
                positions.Add(node.Position);
            }
            LineRenderer renderer = _GetLineRenderer(i);
            renderer.positionCount = positions.Count;
            renderer.SetPositions(positions.ToArray());
        }
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
        return _lineRenderers[i];
    }

    private void _DrawInteractables()
    {
        List<RootNode> interactableNodes = _rootController.GetInteractableNodes();
        for (int i = 0; i < interactableNodes.Count; i++)
        {
            SpriteRenderer s = _GetInteractableSpriteRenderer(i);
            s.gameObject.transform.SetPositionAndRotation(interactableNodes[i].Position, Quaternion.identity);
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
        return _interactableRenderers[i];
    }
}
