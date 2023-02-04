using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public Transform UIContainer;
    private GameController _gameController;
    public Transform HighScoreEntryPrototype;
    private bool _scoresPopulated = false;
    private Persistence _persistence;
    Transform currentUserHighScoreEntry;
    public Color CurrentUserHighScoreEntryColor = Color.green;

    void Start()
    {
        UIContainer.gameObject.SetActive(false);
        _gameController = GameObject.FindObjectOfType<GameController>();
        _persistence = GameObject.FindObjectOfType<Persistence>();
        HighScoreEntryPrototype.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!_gameController.IsGameOver()) return;
        if (!_scoresPopulated)
        {
            _scoresPopulated = true;
            UIContainer.gameObject.SetActive(true);
            PopulateLeaderboard(_persistence.Data.Scores);
            _persistence.Save();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Save();
        }
    }

    void OnDestroy()
    {
        Save();
    }

    private void Save()
    {
        TMP_InputField inputField = currentUserHighScoreEntry.GetChild(3).GetComponent<TMP_InputField>();
        inputField.gameObject.SetActive(false);
        currentUserHighScoreEntry.GetChild(1).gameObject.SetActive(true);
        currentUserHighScoreEntry.GetChild(1).GetComponent<TextMeshProUGUI>().text = inputField.text;
        _persistence.Data.Scores[inputField.text] = (int)_gameController.Score;
        _persistence.Save();

    }

    public void OnReplayPress()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PopulateLeaderboard(Dictionary<string, int> highScores)
    {
        // Inject current users'
        string currentUserName = "CURRENTUSER1234567890";
        int currentScore = (int)_gameController.Score;
        highScores[currentUserName] = currentScore;

        int rank = 1;
        foreach (var (name, score) in highScores.OrderBy(a => -a.Value))
        {
            Transform highScoreEntryUI = _GetNewHighScoreEntry();
            highScoreEntryUI.GetChild(1).GetComponent<TextMeshProUGUI>().text = name;
            highScoreEntryUI.GetChild(2).GetComponent<TextMeshProUGUI>().text = score.ToString();
            highScoreEntryUI.GetChild(4).GetComponent<TextMeshProUGUI>().text = "#" + rank.ToString();
            if (name == currentUserName && score == currentScore)
            {
                highScoreEntryUI.GetChild(3).gameObject.SetActive(true);
                highScoreEntryUI.GetChild(1).gameObject.SetActive(false);
                highScoreEntryUI.GetChild(0).GetComponent<Image>().color = CurrentUserHighScoreEntryColor;
                currentUserHighScoreEntry = highScoreEntryUI;
            }
            else
            {
                highScoreEntryUI.GetChild(3).gameObject.SetActive(false);
            }
            rank++;
        }
    }

    private Transform _GetNewHighScoreEntry()
    {
        Transform h = Instantiate(HighScoreEntryPrototype);
        h.parent = HighScoreEntryPrototype.parent;
        h.gameObject.SetActive(true);
        return h;
    }
}
