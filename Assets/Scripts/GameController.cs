using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public float Nutrients = 1f;
    public float Score = 0;
    public float NutrientsY = -6f;
    public float SuccSpeed = 0.1f;
    private RootController _rootController;
    private PowerUpManager _powerUpManager;
    private bool _gameOver = false;
    public float DifficultyInceaseSpeed = 1f;

    void Start()
    {
        _rootController = GameObject.FindObjectOfType<RootController>();
        _powerUpManager = GameObject.FindObjectOfType<PowerUpManager>();
    }

    void Update()
    {
        _rootController.MasterSpeedMultiplier += DifficultyInceaseSpeed * Time.deltaTime;
        Nutrients = Mathf.Max(0, Nutrients - _GetRumberOfRootsStealing() * SuccSpeed * Time.deltaTime);
        if (Nutrients == 0) _gameOver = true;

        if (_gameOver == false) Score += 10f * Time.deltaTime;
    }

    private int _GetRumberOfRootsStealing()
    {
        int count = 0;
        foreach (RootNode leaf in _rootController.GetLeavesWithOrigin())
        {
            if (leaf.Position.y <= NutrientsY) count++;
        }
        return count;
    }

    public bool IsGameOver()
    {
        return _gameOver;
    }

    public List<Interactable> GetInteractables()
    {
        List<Interactable> interactables = new List<Interactable>();
        foreach (Interactable interactable in _rootController.GetInteractableNodes()) interactables.Add(interactable);
        foreach (Interactable interactable in _powerUpManager.GetAvailablePowerUps()) interactables.Add(interactable);
        return interactables;
    }
}
