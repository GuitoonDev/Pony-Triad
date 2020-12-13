using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PonyTriad.Audio;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField, Scene] private int gameScene = default;

    [SerializeField] private Image uiPanel = null;
    [SerializeField] private CreditsScreen creditsScreen = null;

    private void Start() {
        AudioManager.Instance.PlayGameMusic();
    }

    public void LaunchGame() {
        uiPanel.gameObject.SetActive(false);
        SceneManager.LoadScene(gameScene);
    }

    public void CreditsScreen() {
        creditsScreen.Show();
    }

    public void QuitGame() {
        Application.Quit();
    }
}
