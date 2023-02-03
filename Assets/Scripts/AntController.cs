using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AntController : MonoBehaviour
{
    private const float WaypointHitTolerance = 0.1f;
    public List<AntGroup> AntGroups = new List<AntGroup>();

    public Waypoint WaypointPrefab;

    private List<Waypoint> _waypoints = new List<Waypoint>();


    public float Speed = 1f;
    // Start is called before the first frame update
    void Start()
    {

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

        Vector3 delta = firstWaypoint.transform.position - closestAnt.transform.position;
        Vector3 direction = delta.normalized;

        closestAnt.transform.position += direction * Speed * Time.deltaTime;

        if (delta.magnitude < WaypointHitTolerance)
        {
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
        var newWaypoint = Instantiate(WaypointPrefab, mousePosition, Quaternion.identity);
        if (_waypoints.Count > 0)
        {
            var last = _waypoints.Last();
            var relativeLastPosition = newWaypoint.transform.worldToLocalMatrix.MultiplyPoint(last.transform.position);
            newWaypoint.Line.positionCount = 2;
            newWaypoint.Line.SetPositions(new[] { last.transform.position, newWaypoint.transform.position });
        }
        _waypoints.Add(newWaypoint);


    }

    private AntGroup GetClosestAnt(Vector3 mousePosition)
    {
        return AntGroups.First();
    }
}
