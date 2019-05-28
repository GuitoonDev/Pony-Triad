using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private readonly int cardsPerPlayer = 5;

    [SerializeField] private string mainMenuSceneName = null;

    [Space]

    [SerializeField] [EnumFlag("Active Game Rules")] private GameRules activeGameRules = default(GameRules);

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

    private List<CardWon> _sameCardsWon = new List<CardWon>();
    private Dictionary<int, List<CardWon>> _plusCardsWon = new Dictionary<int, List<CardWon>>();

    private void Awake() {
        Instance = this;
    }

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

    private void UpdateCardsOwnersPhase(SelectableArea _playedCardArea, bool _isCombo) {
        cardPlayedCount++;

        int cardsWon = 0;

        int leftPosition = _playedCardArea.BoardCoordinates.x - 1;
        int rightPosition = _playedCardArea.BoardCoordinates.x + 1;
        int upPosition = _playedCardArea.BoardCoordinates.y - 1;
        int downPosition = _playedCardArea.BoardCoordinates.y + 1;

        if (leftPosition >= 0) {
            Card cardToCompare = selectableAreasList[leftPosition, _playedCardArea.BoardCoordinates.y].Card;

            if (cardToCompare != null) {
                int powerDiff = ((int) _playedCardArea.Card.GetPowerByDirection(CardDirection.Left)) - ((int) cardToCompare.GetPowerByDirection(CardDirection.Right));

                int powerAdd = ((int) _playedCardArea.Card.GetPowerByDirection(CardDirection.Left)) + ((int) cardToCompare.GetPowerByDirection(CardDirection.Right));

                List<CardWon> cardsWonList;
                if (!_plusCardsWon.TryGetValue(powerAdd, out cardsWonList)) {
                    cardsWonList = new List<CardWon>();
                }

                cardsWonList.Add(new CardWon() {
                    direction = CardDirection.Right,
                    card = cardToCompare
                });

                _plusCardsWon[powerAdd] = cardsWonList;

                if (powerDiff > 0) {
                    cardToCompare.ChangePlayerOwner(CardDirection.Right, _playedCardArea.Card.PlayerOwner);
                    cardsWon++;
                }
                else if (powerDiff == 0) {
                    _sameCardsWon.Add(new CardWon() {
                        direction = CardDirection.Right,
                        card = cardToCompare
                    });
                }
            }
        }

        if (rightPosition < selectableAreasList.GetLength(0)) {
            Card cardToCompare = selectableAreasList[rightPosition, _playedCardArea.BoardCoordinates.y].Card;

            if (cardToCompare != null) {
                int powerDiff = ((int) _playedCardArea.Card.GetPowerByDirection(CardDirection.Right)) - ((int) cardToCompare.GetPowerByDirection(CardDirection.Left));

                int powerAdd = ((int) _playedCardArea.Card.GetPowerByDirection(CardDirection.Right)) + ((int) cardToCompare.GetPowerByDirection(CardDirection.Left));

                if (powerDiff > 0) {
                    cardToCompare.ChangePlayerOwner(CardDirection.Left, _playedCardArea.Card.PlayerOwner);
                    cardsWon++;
                }
                else if (powerDiff == 0) {
                    _sameCardsWon.Add(new CardWon() {
                        direction = CardDirection.Left,
                        card = cardToCompare
                    });
                }
            }
        }

        if (upPosition >= 0) {
            Card cardToCompare = selectableAreasList[_playedCardArea.BoardCoordinates.x, upPosition].Card;

            if (cardToCompare != null) {
                int powerDiff = ((int) _playedCardArea.Card.GetPowerByDirection(CardDirection.Up)) - ((int) cardToCompare.GetPowerByDirection(CardDirection.Down));

                int powerAdd = ((int) _playedCardArea.Card.GetPowerByDirection(CardDirection.Up)) + ((int) cardToCompare.GetPowerByDirection(CardDirection.Down));

                if (powerDiff > 0) {
                    cardToCompare.ChangePlayerOwner(CardDirection.Down, _playedCardArea.Card.PlayerOwner);
                    cardsWon++;
                }
                else if (powerDiff == 0) {
                    _sameCardsWon.Add(new CardWon() {
                        direction = CardDirection.Down,
                        card = cardToCompare
                    });
                }
            }
        }

        if (downPosition < selectableAreasList.GetLength(1)) {
            Card cardToCompare = selectableAreasList[_playedCardArea.BoardCoordinates.x, downPosition].Card;

            if (cardToCompare != null) {
                int powerDiff = ((int) _playedCardArea.Card.GetPowerByDirection(CardDirection.Down)) - ((int) cardToCompare.GetPowerByDirection(CardDirection.Up));

                int powerAdd = ((int) _playedCardArea.Card.GetPowerByDirection(CardDirection.Down)) + ((int) cardToCompare.GetPowerByDirection(CardDirection.Up));

                if (powerDiff > 0) {
                    cardToCompare.ChangePlayerOwner(CardDirection.Up, _playedCardArea.Card.PlayerOwner);
                    cardsWon++;
                }
                else if (powerDiff == 0) {
                    _sameCardsWon.Add(new CardWon() {
                        direction = CardDirection.Up,
                        card = cardToCompare
                    });
                }
            }
        }

        cardHandByPlayer[CurrentPlayer].RemoveCard(_playedCardArea.Card);
        cardHandByPlayer[CurrentPlayer].Enable(false);

        cardsWonCount = cardsWon;

        if (activeGameRules.HasFlag(GameRules.Same) && _sameCardsWon.Count > 1) {
            cardsWonCount += _sameCardsWon.Count;

            foreach (CardWon cardWonItem in _sameCardsWon) {
                cardWonItem.card.ChangePlayerOwner(cardWonItem.direction, _playedCardArea.Card.PlayerOwner);
            }
        }

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
        _sameCardsWon.Clear();
        _plusCardsWon.Clear();

        cardsRotationFinishedCount = cardsWonCount = 0;

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

    public bool HasRuleSet(GameRules rulesToTest) {
        return activeGameRules.HasFlag(rulesToTest);
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
