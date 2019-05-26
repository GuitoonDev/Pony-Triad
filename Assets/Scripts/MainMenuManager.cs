using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName;

    public void LaunchGame() {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
