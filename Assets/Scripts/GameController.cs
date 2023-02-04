using UnityEngine;

public class GameController : MonoBehaviour
{
    public float Nutrients = 1f;
    public float NutrientsY = -6f;
    public float SuccSpeed = 0.1f;
    private RootController _rootController;

    void Start()
    {
        _rootController = GameObject.FindObjectOfType<RootController>();
    }

    void Update() {
        Nutrients = Mathf.Max(0, Nutrients - _GetRumberOfRootsStealing() * SuccSpeed * Time.deltaTime);
    }

    private int _GetRumberOfRootsStealing()
    {
        int count = 0;
        foreach (RootNode leaf in _rootController._GetLeavesWithOrigin()) {
            if (leaf.Position.y <= NutrientsY) count ++;
        }
        return count;
    }
}
