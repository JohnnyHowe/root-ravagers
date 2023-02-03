using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNodeDrawer : MonoBehaviour
{
    public LineRenderer RendererPrototype;
    private RootController _rootController;
    private List<LineRenderer> _lineRenderers;

    void Start()
    {
        _rootController = GetComponent<RootController>();
        _lineRenderers = new List<LineRenderer>();
        RendererPrototype.gameObject.SetActive(false);
    }

    void Update()
    {
        List<List<RootNode>> rootPaths = _rootController.GetFullRootPaths();
        for (int i = 0; i < rootPaths.Count; i++) {

            List<Vector3> positions = new List<Vector3>();
            foreach (RootNode node in rootPaths[i]) {
                positions.Add(node.Position);
            }
            LineRenderer renderer = _GetLineRenderer(i);
            renderer.positionCount = positions.Count;
            renderer.SetPositions(positions.ToArray());
        }
    }

    private LineRenderer _GetLineRenderer(int i)
    {
        while (i >= _lineRenderers.Count) {
            LineRenderer renderer = Instantiate(RendererPrototype);
            renderer.gameObject.SetActive(true);
            renderer.transform.parent = RendererPrototype.transform.parent;
            _lineRenderers.Add(renderer);
        }
        return _lineRenderers[i];
    }
}
