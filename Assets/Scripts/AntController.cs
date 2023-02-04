using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AntController : MonoBehaviour
{
    public float MaxAttackRange = 3.0f;
    public float CuttingTime = 2.0f;
    public List<AntGroup> AntGroups = new List<AntGroup>();
    public Waypoint WaypointPrefab;
    public RootController _rootController;
    public GameController _gameController;
    public float Speed = 10f;
    public Interactable ItemHeld = null;

    private List<Waypoint> _waypoints = new List<Waypoint>();
    private const float WaypointHitTolerance = 0.1f;

    void Start()
    {
        _rootController = FindObjectOfType<RootController>();
        _gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        HandleInput();
        HandleAntMovingToWaypoints();
    }

    private void HandleAntMovingToWaypoints()
    {
        if (!_waypoints.Any()) { return; }

        var firstWaypoint = _waypoints.First();
        AntGroup antGroup = GetAntGroup();
        Vector3 delta = firstWaypoint.transform.position - antGroup.transform.position;

        ItemHeld?.TryMoveItem(antGroup.transform.position);

        if (delta.magnitude > WaypointHitTolerance)
        {
            // Move towards waypoint
            Vector3 direction = delta.normalized;
            antGroup.transform.position += direction * Speed * Time.deltaTime;
        }
        else
        {
            // Do waypoint action
            var done = firstWaypoint.Task.PerformAction();
            if (done)
            {
                Destroy(firstWaypoint.gameObject);
                _waypoints.Remove(firstWaypoint);
            }
        }

        // Make line from ant to to first waypoint
        firstWaypoint.Line.positionCount = 2;
        firstWaypoint.Line.SetPositions(new Vector3[] { firstWaypoint.transform.position, antGroup.transform.position });
    }

    private void HandleInput()
    {
        HandleWaypointReset();
        HandleMoveCommand();
        HandleAttackCommand();
    }

    private void HandleWaypointReset()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _waypoints.ForEach(waypoint => Destroy(waypoint.gameObject));
            _waypoints.Clear();
        }
    }

    private void HandleMoveCommand()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnAntMove();
        }
    }

    private void HandleAttackCommand()
    {
        if (Input.GetMouseButtonDown(1))
        {
            OnAntAttack();
        }
    }

    private void OnAntAttack()
    {
        Interactable node = GetClosestNode(GetMousePosition(), MaxAttackRange);
        if (node == null) return;

        var newWaypoint = CreateWaypoint(node.GetLocation());

        newWaypoint.GetComponent<SpriteRenderer>().color = Color.red;

        List<TaskType> taskTypes = node.GetTaskTypes();
        if (ItemHeld == null)
        {
            // No item held
            if (taskTypes.Contains(TaskType.Cut))
            {
                newWaypoint.Task = CreateCuttingAction(node);
            }
            else if (taskTypes.Contains(TaskType.Pickup))
            {
                newWaypoint.Task = _CreatePickupTask(node);
            }
        }
        else
        {
            // item held
            if (taskTypes.Contains(TaskType.Use))
            {
                newWaypoint.Task = _CreateUseTask(node);
            }
        }
    }

    private Task _CreatePickupTask(Interactable interactable)
    {
        Task task = new Task();
        task.TypeOfTask = TaskType.Pickup;
        task.Position = interactable.GetLocation();
        task.Target = interactable;
        task.PerformAction = () =>
        {
            ItemHeld = interactable;
            return true;
        };
        return task;
    }

    private Task _CreateUseTask(Interactable interactable)
    {
        Task task = new Task();
        task.TypeOfTask = TaskType.Use;
        task.Position = interactable.GetLocation();
        task.Target = interactable;
        return task;
    }

    private void OnAntMove()
    {
        var newWaypoint = CreateWaypoint(GetMousePosition());
        newWaypoint.Task = CreateWalkingAction();
    }

    public Waypoint CreateWaypoint(Vector3 position)
    {
        position.z = transform.position.z;

        var newWaypoint = Instantiate(WaypointPrefab, position, Quaternion.identity);

        if (_waypoints.Count > 0)
        {
            var last = _waypoints.Last();
            newWaypoint.Line.positionCount = 2;
            newWaypoint.Line.SetPositions(new[] { last.transform.position, newWaypoint.transform.position });
        }
        _waypoints.Add(newWaypoint);
        return newWaypoint;
    }

    private Task CreateCuttingAction(Interactable node)
    {
        Task task = new Task
        {
            TimeRemaining = CuttingTime,
        };
        Func<bool> action = () =>
        {
            if (task.TimeRemaining > 0)
            {
                task.TimeRemaining -= Time.deltaTime;
                var jiggle = Math.Min(WaypointHitTolerance * 0.5f, 0.1f);
                AntGroup antGroup = GetAntGroup();
                var position = node.GetLocation();
                position.z = antGroup.transform.position.z;
                antGroup.transform.position = position + new Vector3(UnityEngine.Random.Range(-jiggle, jiggle), UnityEngine.Random.Range(-jiggle, jiggle), 0);
                return false;
            }
            else
            {
                _rootController.DoThing(node, RootAction.Cut);
                return true;
            }
        };

        task.PerformAction = action;
        return task;
    }

    private Task CreateWalkingAction()
    {
        return new Task
        {
        };
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;
        return mousePosition;
    }

    private Interactable GetClosestNode(Vector3 mousePosition, float maxRange = float.PositiveInfinity)
    {
        var nodes = _gameController.GetInteractables();
        float closest = float.PositiveInfinity;
        Interactable closestNode = null;

        foreach (var node in nodes)
        {
            var distance = ((Vector2)(mousePosition - node.GetLocation())).magnitude;
            if (distance < maxRange && distance < closest)
            {
                closestNode = node;
                closest = distance;
            }
        }
        return closestNode;
    }

    private AntGroup GetAntGroup()
    {
        return AntGroups.First();
    }
}
