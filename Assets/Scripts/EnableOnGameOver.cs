using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnGameOver : MonoBehaviour
{
    // Start is called before the first frame update
    public Behaviour[] ToDisable = new Behaviour[] { };
    private GameController _gameController;
    private bool _lastGameOver = false;

    void Start()
    {
        _gameController = GameObject.FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        var gameOver = _gameController.IsGameOver();
        if (gameOver != _lastGameOver)
        {
            foreach (var behaviour in ToDisable)
            {
                behaviour.enabled = gameOver;
            }
        }
        _lastGameOver = gameOver;
    }
}
