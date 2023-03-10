using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public GameObject pauseMenuUI;
    public AudioLowPassFilter AudioFilter;

    private bool isPaused = true;


    // Update is called once per frame
    void Update()
    {
        if (!isPaused && Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = true;
            AudioFilter.enabled = true;
        } else if (isPaused && Input.anyKeyDown)
        {
            isPaused = false;
            AudioFilter.enabled = false;
        }

        if (isPaused)
        {
            ActivateMenu();
        }
        else
        {
            DeactivateMenu();
        }
    }

    void ActivateMenu()
    {
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
    }

    void DeactivateMenu()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
    }
}
