using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField] private bool randomRules = false;

    [Space]

    [SerializeField] private RandomArrowView randomArrow = null;

    [Header("Player One")]
    [SerializeField] private PlayerView playerOneView = null;

    [Header("Player Two")]
    [SerializeField] private PlayerView playerTwoView = null;

    [Header("Field Areas")]
    [SerializeField] private VerticalListableAreas[] verticalListableAreasList = null;

    [Header("Cards Lists")]
    [SerializeField] private CardLevelDefinition[] cardsListArray = null;

    [Header("User Interface")]
    [SerializeField] private Canvas uiCanvas = null;
    [SerializeField] private SpecialRuleText sameRuleTextPrefab = null;
    [SerializeField] private SpecialRuleText plusRuleTextPrefab = null;
    [SerializeField] private SpecialRuleText comboRuleTextPrefab = null;
    [SerializeField] private WinScreen winScreen = null;
    [SerializeField] private RuleBar ruleBarHolder = null;

    #region UI Methods
    public void SelectNewGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SelectMainMenu() {
        SceneManager.LoadScene(mainMenuSceneName);
    }
    #endregion
}
