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

    private Waypoint _cutting;

    private float _cuttingTimeRemaining = 0;
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

        Vector3 mousePosition = GetMousePosition();
        AntGroup closestAnt = GetClosestAnt(mousePosition);
        if (_cutting != null)
        {
            _cuttingTimeRemaining -= Time.deltaTime;
            if (_cuttingTimeRemaining < 0)
            {
                _rootController.DoThing(_cutting.Target, RootAction.Cut);
                Destroy(_cutting.gameObject);
                _waypoints.Remove(_cutting);
                _cutting = null;
            }
            else
            {
                closestAnt.transform.position = _cutting.Target.Position + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
            }
        }
        else
        {
            if (!_waypoints.Any())
            {
                return;
            }
            var firstWaypoint = _waypoints.First();


            firstWaypoint.Line.positionCount = 2;
            firstWaypoint.Line.SetPositions(new Vector3[] { firstWaypoint.transform.position, closestAnt.transform.position });

            Vector3 delta = firstWaypoint.transform.position - closestAnt.transform.position;
            Vector3 direction = delta.normalized;

            closestAnt.transform.position += direction * Speed * Time.deltaTime;

            if (delta.magnitude < WaypointHitTolerance)
            {
                if (firstWaypoint.Target != null)
                {
                    _cutting = firstWaypoint;
                    _cuttingTimeRemaining = 1.0f;
                }
                else
                {
                    Destroy(firstWaypoint.gameObject);
                    _waypoints.Remove(firstWaypoint);
                }
            }
        }


    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _waypoints.ForEach(waypoint => Destroy(waypoint.gameObject));
            _waypoints.Clear();
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnAntMove();
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnAntAttack();
        }
    }

    private void OnAntAttack()
    {
        var node = GetTargetedNode(GetMousePosition());
        if (node == null) return;

        var position = node.Position;
        position.z = transform.position.z;

        var newWaypoint = Instantiate(WaypointPrefab, position, Quaternion.identity);
        newWaypoint.GetComponent<SpriteRenderer>().color = Color.red;
        newWaypoint.Target = node;

        if (_waypoints.Count > 0)
        {
            var last = _waypoints.Last();
            newWaypoint.Line.positionCount = 2;
            newWaypoint.Line.SetPositions(new[] { last.transform.position, newWaypoint.transform.position });
        }
        _waypoints.Add(newWaypoint);

    }

    private void OnAntMove()
    {
        Vector3 mousePosition = GetMousePosition();

        var position = mousePosition;
        position.z = transform.position.z;

        var newWaypoint = Instantiate(WaypointPrefab, position, Quaternion.identity);

        if (_waypoints.Count > 0)
        {
            var last = _waypoints.Last();
            newWaypoint.Line.positionCount = 2;
            newWaypoint.Line.SetPositions(new[] { last.transform.position, newWaypoint.transform.position });
        }
        _waypoints.Add(newWaypoint);
    }


    private Vector3 GetMousePosition()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;
        return mousePosition;
    }

    private RootNode GetTargetedNode(Vector3 mousePosition)
    {
        var nodes = _rootController.GetInteractableNodes();
        float closest = float.PositiveInfinity;
        RootNode closestNode = null;

        foreach (var node in nodes)
        {
            var distance = ((Vector2)(mousePosition - node.Position)).magnitude;
            if (distance < closest)
            {
                closestNode = node;
                closest = distance;
            }
        }
        return closestNode;
    }

    private AntGroup GetClosestAnt(Vector3 mousePosition)
    {
        return AntGroups.First();
    }
}
