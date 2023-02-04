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
    private bool _scoresPopulated = false;
    private Persistence _persistence;
    Transform currentUserHighScoreEntry;

    void Start()
    {
        // UIContainer.gameObject.SetActive(false);
        _gameController = GameObject.FindObjectOfType<GameController>();
        _persistence = GameObject.FindObjectOfType<Persistence>();
        HighScoreEntryPrototype.gameObject.SetActive(false);
    }

    void Update()
    {
        // if (_gameController.IsGameOver() && !_scoresPopulated)
        if (!_scoresPopulated)
        {
            _scoresPopulated = true;
            UIContainer.gameObject.SetActive(true);
            PopulateLeaderboard(_persistence.Data.Scores);
            _persistence.Save();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TMP_InputField inputField = currentUserHighScoreEntry.GetChild(3).GetComponent<TMP_InputField>();
            inputField.gameObject.SetActive(false);
            currentUserHighScoreEntry.GetChild(1).gameObject.SetActive(true);
            currentUserHighScoreEntry.GetChild(1).GetComponent<TextMeshProUGUI>().text = inputField.text;
            _persistence.Data.Scores[inputField.text] = (int) _gameController.Score;
            _persistence.Save();
        }
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

        highScores["aa"] = -1;
        highScores["ab"] = 2;
        highScores["ac"] = 3;
        highScores["aw"] = 3;
        highScores["a1"] = 3;
        highScores["a2"] = 3;
        highScores["a3"] = 3;
        highScores["a4"] = 3;
        highScores["a5"] = 3;
        highScores["a6"] = 3;
        highScores["a7"] = 3;
        highScores["a9"] = 3;
        highScores["a8"] = 3;
        highScores["ae"] = 10;
        highScores["ar"] = 3;
        highScores["aj"] = 6;

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
                currentUserHighScoreEntry = highScoreEntryUI;
            }
            else
            {
                highScoreEntryUI.GetChild(3).gameObject.SetActive(false);
            }
            rank ++;
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
