using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Audio;

public class GameManager : MonoBehaviour
{
    [System.Flags]
    enum GameRules
    {
        None = 0,

        Open = 1 << 0,

        Plus = 1 << 1,
        Same = 1 << 2,

        Borderless = 1 << 3,
        AceWalls = 1 << 4,

        Battle = 1 << 5,
    }

    private readonly int cardsPerPlayer = 5;

    [SerializeField] private string mainMenuSceneName = null;

    [Space]

    [SerializeField] [EnumFlag("Game Rules")] private GameRules gameRules = default(GameRules);


    [Space]

    [SerializeField] private RandomArrow randomArrow = null;

    [Header("Player One")]
    [SerializeField] private TextMeshPro playerOneScoreText = null;
    [SerializeField] private CardsHand playerOneCardsHand = null;

    [Header("Player Two")]
    [SerializeField] private TextMeshPro playerTwoScoreText = null;
    [SerializeField] private CardsHand playerTwoCardsHand = null;

    [Header("Field Areas")]
    [SerializeField] private VerticalListableAreas[] verticalListableAreasList = null;

    [Header("Canvas Elements")]
    [SerializeField] private Canvas uiCanvas = null;
    [SerializeField] private TextMeshProUGUI winText = null;

    [Header("Cards Lists")]
    [SerializeField] private CardList[] cardsListArray = null;


    private int currentPlayerOneScore = 0;
    private int CurrentPlayerOneScore {
        get { return currentPlayerOneScore; }
        set {
            currentPlayerOneScore = value;
            playerOneScoreText.text = currentPlayerOneScore.ToString();
        }
    }

    private int currentPlayerTwoScore = 0;
    private int CurrentPlayerTwoScore {
        get { return currentPlayerTwoScore; }
        set {
            currentPlayerTwoScore = value;
            playerTwoScoreText.text = currentPlayerTwoScore.ToString();
        }
    }

    private PlayerNumber lastPlayer = PlayerNumber.None;
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

    private SelectableArea[,] selectableAreasList = new SelectableArea[3, 3];
    private Dictionary<PlayerNumber, CardsHand> cardHandByPlayer = new Dictionary<PlayerNumber, CardsHand>();

    private int handsReadyCount = 0;
    private int cardPlayedCount = 0;
    private int cardsRotationFinishedCount = 0;
    private int cardsWonCount = 0;

    private void Start() {
        int[] randomCardLevelArray = new int[5];
        for (int i = 0; i < randomCardLevelArray.Length; i++) {
            randomCardLevelArray[i] = Random.Range(0, cardsListArray.Length - 1);
        }

        CardDatas[] playerCards = new CardDatas[cardsPerPlayer];
        for (int i = 0; i < playerCards.Length; i++) {
            playerCards[i] = cardsListArray[randomCardLevelArray[i]].GetRandomCard();
        }
        playerCards.Shuffle();
        playerOneCardsHand.OnHandReady += PlayerHandReady;
        playerOneCardsHand.Init(playerCards, false);
        CurrentPlayerOneScore = cardsPerPlayer;

        playerCards = new CardDatas[cardsPerPlayer];
        for (int i = 0; i < playerCards.Length; i++) {
            playerCards[i] = cardsListArray[randomCardLevelArray[i]].GetRandomCard();
        }
        playerCards.Shuffle();
        playerOneCardsHand.OnHandReady += PlayerHandReady;
        playerTwoCardsHand.Init(playerCards, false);
        CurrentPlayerTwoScore = cardsPerPlayer;

        cardHandByPlayer[PlayerNumber.One] = playerOneCardsHand;
        cardHandByPlayer[PlayerNumber.Two] = playerTwoCardsHand;

        for (int positionX = 0; positionX < selectableAreasList.GetLength(0); positionX++) {
            for (int positionY = 0; positionY < selectableAreasList.GetLength(1); positionY++) {
                SelectableArea newSelectableArea = verticalListableAreasList[positionX][positionY];
                newSelectableArea.OnCardPlayed += UpdateCardsOwnersPhase;
                newSelectableArea.OnCardAnimationFinished += CardAnimationFinished;
                newSelectableArea.BoardCoordinates = new Vector2Int(positionX, positionY);
                selectableAreasList[positionX, positionY] = newSelectableArea;
            }
        }

        lastPlayer = Random.Range(0, 2) < 1 ? PlayerNumber.One : PlayerNumber.Two;

        AudioManager.Instance.PlayGameMusic();
    }

    private void PlayerHandReady() {
        handsReadyCount++;

        if (handsReadyCount >= cardHandByPlayer.Count) {
            randomArrow.OnAnimationComplete += DisplayFirstPlayerArrow;
            randomArrow.StartAnimation((int) lastPlayer);
        }
    }

    private void DisplayFirstPlayerArrow() {
        randomArrow.OnAnimationComplete -= DisplayFirstPlayerArrow;

        CurrentPlayer = lastPlayer;
        cardHandByPlayer[CurrentPlayer].Enable(true);
    }

    private void UpdateCardsOwnersPhase(SelectableArea _selectableArea) {
        cardPlayedCount++;

        int cardsWon = 0;

        int leftPosition = _selectableArea.BoardCoordinates.x - 1;
        int rightPosition = _selectableArea.BoardCoordinates.x + 1;
        int upPosition = _selectableArea.BoardCoordinates.y - 1;
        int downPosition = _selectableArea.BoardCoordinates.y + 1;

        if (leftPosition >= 0) {
            Card cardToCompare = selectableAreasList[leftPosition, _selectableArea.BoardCoordinates.y].Card;
            bool isCardWon = (cardToCompare != null && cardToCompare.IsLooseBattle(CardDirection.Right, _selectableArea.Card.GetPowerByDirection(CardDirection.Left), _selectableArea.Card.PlayerOwner));
            if (isCardWon) {
                cardsWon++;
            }
        }

        if (rightPosition < selectableAreasList.GetLength(0)) {
            Card cardToCompare = selectableAreasList[rightPosition, _selectableArea.BoardCoordinates.y].Card;
            bool isCardWon = (cardToCompare != null && cardToCompare.IsLooseBattle(CardDirection.Left, _selectableArea.Card.GetPowerByDirection(CardDirection.Right), _selectableArea.Card.PlayerOwner));
            if (isCardWon) {
                cardsWon++;
            }
        }

        if (upPosition >= 0) {
            Card cardToCompare = selectableAreasList[_selectableArea.BoardCoordinates.x, upPosition].Card;
            bool isCardWon = (cardToCompare != null && cardToCompare.IsLooseBattle(CardDirection.Down, _selectableArea.Card.GetPowerByDirection(CardDirection.Up), _selectableArea.Card.PlayerOwner));
            if (isCardWon) {
                cardsWon++;
            }
        }

        if (downPosition < selectableAreasList.GetLength(1)) {
            Card cardToCompare = selectableAreasList[_selectableArea.BoardCoordinates.x, downPosition].Card;
            bool isCardWon = (cardToCompare != null && cardToCompare.IsLooseBattle(CardDirection.Up, _selectableArea.Card.GetPowerByDirection(CardDirection.Down), _selectableArea.Card.PlayerOwner));
            if (isCardWon) {
                cardsWon++;
            }
        }

        cardHandByPlayer[CurrentPlayer].RemoveCard(_selectableArea.Card);
        cardHandByPlayer[CurrentPlayer].Enable(false);

        cardsWonCount = cardsWon;
        if (cardsWonCount == 0) {
            BetweenTurnPhase();
        }
    }

    private void CardAnimationFinished(Card _cardTarget) {
        cardsRotationFinishedCount++;

        if (cardsRotationFinishedCount == cardsWonCount) {
            BetweenTurnPhase();
        }
    }

    private void BetweenTurnPhase() {
        switch (CurrentPlayer) {
            case PlayerNumber.One:
                CurrentPlayerOneScore += cardsWonCount;
                CurrentPlayerTwoScore -= cardsWonCount;
                break;

            case PlayerNumber.Two:
                CurrentPlayerTwoScore += cardsWonCount;
                CurrentPlayerOneScore -= cardsWonCount;
                break;
        }

        cardsRotationFinishedCount = cardsWonCount = 0;

        bool isGameOver = (cardPlayedCount == selectableAreasList.GetLength(0) * selectableAreasList.GetLength(1));
        if (isGameOver) {
            CheckWinner();
        }
        else {
            switch (CurrentPlayer) {
                case PlayerNumber.One:
                    CurrentPlayer = PlayerNumber.Two;
                    break;

                case PlayerNumber.Two:
                    CurrentPlayer = PlayerNumber.One;
                    break;
            }
        }
    }

    private void CheckWinner() {
        CurrentPlayer = PlayerNumber.None;

        uiCanvas.gameObject.SetActive(true);

        if (currentPlayerOneScore > currentPlayerTwoScore) {
            // See ColorUtility.ToHtmlStringRGB
            winText.text = "<color=\"blue\">Blue</color>\nwins !";
            AudioManager.Instance.PlayVictoryMusic();
        }
        else if (currentPlayerOneScore < currentPlayerTwoScore) {
            // See ColorUtility.ToHtmlStringRGB
            winText.text = "<color=\"red\">Red</color>\nwins !";
            AudioManager.Instance.PlayVictoryMusic();
        }
        else {
            winText.text = "<color=\"green\">Draw</color> !";
        }
    }

    #region UI Methods
    public void SelectNewGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SelectMainMenu() {
        SceneManager.LoadScene(mainMenuSceneName);
    }
    #endregion

    [System.Serializable]
    public class VerticalListableAreas
    {
        [HideInInspector] public string name = "Vertical List";

        [SerializeField] private SelectableArea[] items = null;

        public SelectableArea this[int _index] => items[_index];
    }
}
