using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private string mainGameSceneName = "Main";

    // Called when pressing the Play Again button
    public void PlayAgain()
    {
        SceneManager.LoadScene(mainGameSceneName);
    }

    // Quit button if you want to exit game
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
