using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string startLevelSceneName;

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(startLevelSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }



}
