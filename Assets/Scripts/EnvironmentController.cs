using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
public Transform HealthDisplayObject;
    private Vector3 _healthDisplayObjectStart;
    public Vector3 HealthDisplayObjectOffset;
    private GameController _gameController;

    void Awake()
    {
        _gameController = GameObject.FindObjectOfType<GameController>();
        _healthDisplayObjectStart = HealthDisplayObject.position;
    }

    void Update()
    {
        HealthDisplayObject.position = Vector3.Lerp(_healthDisplayObjectStart, _healthDisplayObjectStart + HealthDisplayObjectOffset, 1 - _gameController.NutrientsDecimal);
    }
}
