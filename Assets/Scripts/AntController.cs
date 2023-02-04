using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AntController : MonoBehaviour
{
    private const float WaypointHitTolerance = 0.1f;
    public List<AntGroup> AntGroups = new List<AntGroup>();

    public Waypoint WaypointPrefab;

    public RootController _rootController;

    private List<Waypoint> _waypoints = new List<Waypoint>();


    public float Speed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        _rootController = GameObject.FindObjectOfType<RootController>();

    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        HandleAntMovingToWaypoints();
    }

    private void HandleAntMovingToWaypoints()
    {
        if (!_waypoints.Any())
        {
            return;
        }
        var firstWaypoint = _waypoints.First();
        Vector3 mousePosition = GetMousePosition();

        var closestAnt = GetClosestAnt(mousePosition);

        firstWaypoint.Line.positionCount = 2;
        firstWaypoint.Line.SetPositions(new Vector3[] { firstWaypoint.transform.position, closestAnt.transform.position });

        Vector3 delta = firstWaypoint.transform.position - closestAnt.transform.position;
        Vector3 direction = delta.normalized;

        closestAnt.transform.position += direction * Speed * Time.deltaTime;

        if (delta.magnitude < WaypointHitTolerance)
        {
            if (firstWaypoint.Target != null)
            {
                _rootController.DoThing(firstWaypoint.Target, RootAction.Cut);
            }
            Destroy(firstWaypoint.gameObject);
            _waypoints.Remove(firstWaypoint);
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _waypoints.ForEach(waypoint => Destroy(waypoint.gameObject));
            _waypoints.Clear();
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnMouseDown();
        }
    }

    private Vector3 GetMousePosition()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;
        return mousePosition;
    }

    private void OnMouseDown()
    {
        Vector3 mousePosition = GetMousePosition();
        var node = GetNodeInRange(mousePosition);

        var position = node?.Position ?? mousePosition;
        mousePosition.z = transform.position.z;

        var newWaypoint = Instantiate(WaypointPrefab, position, Quaternion.identity);

        if (node != null)
        {
            newWaypoint.GetComponent<SpriteRenderer>().color = Color.red;
            newWaypoint.Target = node;

        }
        if (_waypoints.Count > 0)
        {
            var last = _waypoints.Last();
            newWaypoint.Line.positionCount = 2;
            newWaypoint.Line.SetPositions(new[] { last.transform.position, newWaypoint.transform.position });
        }
        _waypoints.Add(newWaypoint);
    }

    private RootNode GetNodeInRange(Vector3 mousePosition)
    {
        var nodes = _rootController.GetInteractableNodes();

        foreach (var node in nodes)
        {
            var distance = ((Vector2)(mousePosition - node.Position)).magnitude;
            if (distance < 0.1f)
            {
                return node;
            }
        }
        return null;
    }

    private AntGroup GetClosestAnt(Vector3 mousePosition)
    {
        return AntGroups.First();
    }
}
