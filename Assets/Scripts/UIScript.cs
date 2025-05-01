using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour
{
    public GameObject pauseScreen;

    public bool gameIsPaused;

    public GameObject player;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused) ResumeGame();
            else PauseGame();
        }
    }


    public void PauseGame()
    {
        gameIsPaused = true;
        pauseScreen.SetActive(true);
        player.GetComponent<CursorController>().enabled = false;
    }


    public void ResumeGame()
    {
        gameIsPaused = false;
        pauseScreen.SetActive(false);
        player.GetComponent<CursorController>().enabled = true;
    }


    public void RestartLevel()
    {
        SceneLoader.LoadLevel(SceneLoader.GetCurrentLevelSceneName());
        SceneLoader.ShowLoadingScreen();
    }

    public void QuitGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }


    public void GoToMainMenu()
    {
        Debug.Log("Go to main menu");
        SceneLoader.LoadLevel("MainMenu");
        SceneLoader.ShowLoadingScreen();
    }
    

}