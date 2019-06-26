using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public partial class GameController : MonoBehaviour
{
    [System.Serializable]
    public class VerticalListableAreas
    {
        [HideInInspector] public string name = "Vertical List";

        [SerializeField] private CardBoardPartView[] items = null;

        public CardBoardPartView this[int _index] => items[_index];
    }

    private readonly int cardsPerPlayer = 5;

    [SerializeField] private string mainMenuSceneName = null;

    [Space]

    [SerializeField] [EnumFlag("Active Game Rules")] private GameRule activeGameRules = default(GameRule);

    [Space]

    [SerializeField] private RandomArrowView randomArrow = null;

    [Header("Player One")]
    [SerializeField] private PlayerView playerOneView = null;

    [Header("Player Two")]
    [SerializeField] private PlayerView playerTwoView = null;

    [Header("End Game Colors")]
    [SerializeField] private Color drawColor = default(Color);
    [SerializeField] private PlayersColorsDefinition playersColorsList = null;

    [Header("Field Areas")]
    [SerializeField] private VerticalListableAreas[] verticalListableAreasList = null;

    [Header("Cards Lists")]
    [SerializeField] private CardLevelDefinition[] cardsListArray = null;

    [Header("User Interface")]
    [SerializeField] private Canvas uiCanvas = null;
    [SerializeField] private SpecialRuleText sameRuleTextPrefab = null;
    [SerializeField] private SpecialRuleText plusRuleTextPrefab = null;
    [SerializeField] private SpecialRuleText comboRuleTextPrefab = null;
    [SerializeField] private Image winScreen = null;
    [SerializeField] private TextMeshProUGUI winText = null;


    #region UI Methods
    public void SelectNewGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SelectMainMenu() {
        SceneManager.LoadScene(mainMenuSceneName);
    }
    #endregion
}
