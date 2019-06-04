using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public partial class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class VerticalListableAreas
    {
        [HideInInspector] public string name = "Vertical List";

        [SerializeField] private CardBoardArea[] items = null;

        public CardBoardArea this[int _index] => items[_index];
    }

    public static GameManager Instance { get; private set; }

    private readonly int cardsPerPlayer = 5;

    [SerializeField] private string mainMenuSceneName = null;

    [Space]

    [SerializeField] [EnumFlag("Active Game Rules")] private GameRule activeGameRules = default(GameRule);

    [Space]

    [SerializeField] private RandomArrow randomArrow = null;

    [Header("Player One")]
    [SerializeField] private CardsHand playerOneCardsHand = null;

    [Header("Player Two")]
    [SerializeField] private CardsHand playerTwoCardsHand = null;

    [Header("Eng Game Colors")]
    [SerializeField] private Color drawColor = default(Color);
    [SerializeField] private PlayersColorsList playersColorsList = null;

    [Header("Field Areas")]
    [SerializeField] private VerticalListableAreas[] verticalListableAreasList = null;

    [Header("Canvas Elements")]
    [SerializeField] private Canvas uiCanvas = null;
    [SerializeField] private SpecialRuleText sameRuleTextPrefab = null;
    [SerializeField] private SpecialRuleText plusRuleTextPrefab = null;
    [SerializeField] private SpecialRuleText comboRuleTextPrefab = null;
    [SerializeField] private Image winScreen = null;
    [SerializeField] private TextMeshProUGUI winText = null;

    [Header("Cards Lists")]
    [SerializeField] private CardList[] cardsListArray = null;

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
                foreach (CardsHand handItem in cardHandByPlayer.Values) {
                    handItem.Enable(false);
                }
            }
        }
    }

    private CardBoardArea[,] selectableAreasList = new CardBoardArea[3, 3];
    private Dictionary<PlayerNumber, CardsHand> cardHandByPlayer = new Dictionary<PlayerNumber, CardsHand>();

    private CardBoardArea playedCardBoardArea = null;
    private Dictionary<CardDirection, CardBoardArea> playedCardOpponentCardAreasByDirection = null;

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

    private Dictionary<CardDirection, CardBoardArea> GetCardAreasAround(CardBoardArea _targetCardBoardArea) {
        Dictionary<CardDirection, CardBoardArea> cardBoardAreasAround = new Dictionary<CardDirection, CardBoardArea>();

        int leftPosition = _targetCardBoardArea.BoardCoordinates.x - 1;
        int rightPosition = _targetCardBoardArea.BoardCoordinates.x + 1;
        int upPosition = _targetCardBoardArea.BoardCoordinates.y - 1;
        int downPosition = _targetCardBoardArea.BoardCoordinates.y + 1;

        if (leftPosition >= 0 && !selectableAreasList[leftPosition, _targetCardBoardArea.BoardCoordinates.y].IsEmpty) {
            cardBoardAreasAround[CardDirection.Left] = selectableAreasList[leftPosition, _targetCardBoardArea.BoardCoordinates.y];
        }
        // else if (activeGameRules.HasFlag(GameRule.Borderless)) {
        //     leftPosition = selectableAreasList.GetLength(0) - 1;
        // }
        // cardBoardAreasAround[CardDirection.Left] = selectableAreasList[leftPosition, _targetCardBoardArea.BoardCoordinates.y];

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
