using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public Transform UIContainer;
    private GameController _gameController;

    void Start()
    {
        UIContainer.gameObject.SetActive(false);
        _gameController = GameObject.FindObjectOfType<GameController>();
    }

    void Update()
    {
        if (_gameController.IsGameOver())
        {
            UIContainer.gameObject.SetActive(true);
        }
    }
}
