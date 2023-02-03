using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AntController : MonoBehaviour
{


    public List<AntGroup> AntGroups = new List<AntGroup>();

    public Waypoint WaypointPrefab;

    private List<Waypoint> _waypoints = new List<Waypoint>();

    private bool _mouse1Down = false;

    public float Speed = 1f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (_waypoints.Any())
        {
            var firstWaypoint = _waypoints.First();
            Vector3 mousePosition = GetMousePosition();

            var closestAnt = GetClosestAnt(mousePosition);

            Vector3 delta = firstWaypoint.transform.position - closestAnt.transform.position;
            Vector3 direction = delta.normalized;

            // Move the object towards the mouse
            closestAnt.transform.position += direction * Speed * Time.deltaTime;

            if (delta.magnitude < 1)
            {
                Destroy(firstWaypoint);
                _waypoints.Remove(firstWaypoint);
            }
        }


        var mouse1Down = Input.GetMouseButton(1);
        if (mouse1Down && !_mouse1Down)
        {
            OnMouseDown();
        }
        _mouse1Down = mouse1Down;
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
        //GameObject newWaypointObject = new GameObject($"waypoint{_waypoints.Count}", typeof(Waypoint));

        //var newWaypoint = newWaypointObject.GetComponent<Waypoint>();

        //_waypoints.Add(newWaypoint);

        //newWaypointObject.transform.position = GetMousePosition();

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
