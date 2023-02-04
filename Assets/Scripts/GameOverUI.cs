using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public Transform UIContainer;
    private GameController _gameController;
    public Transform HighScoreEntryPrototype;

    void Start()
    {
        UIContainer.gameObject.SetActive(false);
        _gameController = GameObject.FindObjectOfType<GameController>();
        HighScoreEntryPrototype.gameObject.SetActive(false);
    }

    void Update()
    {
        if (_gameController.IsGameOver())
        {
            UIContainer.gameObject.SetActive(true);
        }
    }

    public void OnReplayPress()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PopulateLeaderboard(Dictionary<string, int> highScores)
    {
        foreach (var (name, score) in highScores.OrderBy(a => a.Value)) {
            Transform highScoreEntryUI = _GetNewHighScoreEntry();
            highScoreEntryUI.GetChild(1).GetComponent<TextMeshProUGUI>().text = name;
            highScoreEntryUI.GetChild(2).GetComponent<TextMeshProUGUI>().text = score.ToString();
        }
    }

    private Transform _GetNewHighScoreEntry() {
        Transform h = Instantiate(HighScoreEntryPrototype);
        h.parent = HighScoreEntryPrototype.parent;
        h.gameObject.SetActive(true);
        return h;
    }
}
