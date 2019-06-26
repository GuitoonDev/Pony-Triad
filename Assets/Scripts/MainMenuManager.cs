using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PonyTriad.Audio;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = null;

    [SerializeField] private Image uiPanel = null;

    private void Start() {
        AudioManager.Instance.PlayGameMusic();
    }

    public void LaunchGame() {
        uiPanel.gameObject.SetActive(false);
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
