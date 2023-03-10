using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI NutrientsText;
    public TextMeshProUGUI ScoreText;
    private GameController _gameController;

    void Start()
    {
        _gameController = GameObject.FindObjectOfType<GameController>();
    }

    void Update()
    {
        NutrientsText.text = Mathf.RoundToInt(_gameController.Nutrients).ToString();
        ScoreText.text = Mathf.RoundToInt(_gameController.Score).ToString();
    }
}
