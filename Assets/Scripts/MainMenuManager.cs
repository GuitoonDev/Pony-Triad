using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PonyTriad.Audio;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField, Scene]
    private int gameScene;

    [Header("UI")]
    [SerializeField]
    private Image uiPanel = null;
    [SerializeField]
    private GameObject creditsScreen = null;

    private void Start()
    {
        AudioManager.Instance.PlayGameMusic();
    }

    #region Unity Event Methods

    public void LaunchGame()
    {
        uiPanel.gameObject.SetActive(false);
        SceneManager.LoadScene(gameScene);
    }

    public void ShowCreditsScreen()
    {
        creditsScreen.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion
}
