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

        [SerializeField] private CardBoardView[] items = null;

        public CardBoardView this[int _index] => items[_index];
    }

    public static GameController Instance { get; private set; }

    private readonly int cardsPerPlayer = 5;

    [SerializeField] private string mainMenuSceneName = null;

    [Space]

    [SerializeField] [EnumFlag("Active Game Rules")] private GameRule activeGameRules = default(GameRule);

    [Space]

    [SerializeField] private RandomArrowView randomArrow = null;

    [Header("Player One")]
    [SerializeField] private PlayerView playerOneCardsHand = null;

    [Header("Player Two")]
    [SerializeField] private PlayerView playerTwoCardsHand = null;

    [Header("End Game Colors")]
    [SerializeField] private Color drawColor = default(Color);
    [SerializeField] private PlayersColorsDefinition playersColorsList = null;

    [Header("Field Areas")]
    [SerializeField] private VerticalListableAreas[] verticalListableAreasList = null;

    [Header("Cards Lists")]
    [SerializeField] private CardLevelDefinition[] cardsListArray = null;

    [Header("UI")]
    [SerializeField] private Canvas uiCanvas = null;
    [SerializeField] private SpecialRuleText sameRuleTextPrefab = null;
    [SerializeField] private SpecialRuleText plusRuleTextPrefab = null;
    [SerializeField] private SpecialRuleText comboRuleTextPrefab = null;
    [SerializeField] private Image winScreen = null;
    [SerializeField] private TextMeshProUGUI winText = null;

    private PlayerNumber currentPlayer = PlayerNumber.None;
    private PlayerNumber CurrentPlayer {
        get {
            return currentPlayer;
        }
        set {
            if (currentPlayer != value && value != PlayerNumber.None) {
                if (currentPlayer != PlayerNumber.None) {
                    cardHandByPlayer[currentPlayer].Enable(false);
                }

                currentPlayer = value;

                cardHandByPlayer[currentPlayer].Enable(true);
            }
            else if (value == PlayerNumber.None) {
                foreach (PlayerView handItem in cardHandByPlayer.Values) {
                    handItem.Enable(false);
                }
            }
        }
    }

    private CardBoardView[,] selectableAreasList = new CardBoardView[3, 3];
    private Dictionary<PlayerNumber, PlayerView> cardHandByPlayer = new Dictionary<PlayerNumber, PlayerView>();

    private CardBoardView playedCardBoardArea = null;
    private Dictionary<CardDirection, CardBoardView> playedCardOpponentCardAreasByDirection = null;

    private List<CardBoardAreaWon> sameRuleCardsWonList = new List<CardBoardAreaWon>();

    private List<CardBoardAreaWon> cardsWonList = new List<CardBoardAreaWon>();

    public bool HasRuleSet(GameRule _rulesToTest) {
        return activeGameRules.HasFlag(_rulesToTest);
    }

    private CardDirection GetOppositeDirection(CardDirection _targetDirection) {
        CardDirection _oppositeDirection = CardDirection.Up;
        switch (_targetDirection) {
            case CardDirection.Up:
                _oppositeDirection = CardDirection.Down;
                break;

            case CardDirection.Down:
                _oppositeDirection = CardDirection.Up;
                break;

            case CardDirection.Left:
                _oppositeDirection = CardDirection.Right;
                break;

            case CardDirection.Right:
                _oppositeDirection = CardDirection.Left;
                break;
        }

        return _oppositeDirection;
    }

    private Dictionary<CardDirection, CardBoardView> GetCardAreasAround(CardBoardView _targetCardBoardArea) {
        Dictionary<CardDirection, CardBoardView> cardBoardAreasAround = new Dictionary<CardDirection, CardBoardView>();

        int leftPosition = _targetCardBoardArea.BoardCoordinates.x - 1;
        int rightPosition = _targetCardBoardArea.BoardCoordinates.x + 1;
        int upPosition = _targetCardBoardArea.BoardCoordinates.y - 1;
        int downPosition = _targetCardBoardArea.BoardCoordinates.y + 1;

        if (activeGameRules.HasFlag(GameRule.Borderless)) {
            if (leftPosition < 0) {
                leftPosition = selectableAreasList.GetLength(0) - 1;
            }
            if (rightPosition >= selectableAreasList.GetLength(0)) {
                rightPosition = 0;
            }
            if (upPosition < 0) {
                upPosition = selectableAreasList.GetLength(1) - 1;
            }
            if (downPosition >= selectableAreasList.GetLength(1)) {
                downPosition = 0;
            }
        }

        if (leftPosition >= 0 && !selectableAreasList[leftPosition, _targetCardBoardArea.BoardCoordinates.y].IsEmpty) {
            cardBoardAreasAround[CardDirection.Left] = selectableAreasList[leftPosition, _targetCardBoardArea.BoardCoordinates.y];
        }
        if (rightPosition < selectableAreasList.GetLength(0) && !selectableAreasList[rightPosition, _targetCardBoardArea.BoardCoordinates.y].IsEmpty) {
            cardBoardAreasAround[CardDirection.Right] = selectableAreasList[rightPosition, _targetCardBoardArea.BoardCoordinates.y];
        }
        if (upPosition >= 0 && !selectableAreasList[_targetCardBoardArea.BoardCoordinates.x, upPosition].IsEmpty) {
            cardBoardAreasAround[CardDirection.Up] = selectableAreasList[_targetCardBoardArea.BoardCoordinates.x, upPosition];
        }
        if (downPosition < selectableAreasList.GetLength(1) && !selectableAreasList[_targetCardBoardArea.BoardCoordinates.x, downPosition].IsEmpty) {
            cardBoardAreasAround[CardDirection.Down] = selectableAreasList[_targetCardBoardArea.BoardCoordinates.x, downPosition];
        }

        return cardBoardAreasAround;
    }

    private void Awake() {
        Instance = this;
    }

    #region UI Methods
    public void SelectNewGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SelectMainMenu() {
        SceneManager.LoadScene(mainMenuSceneName);
    }
    #endregion
}
