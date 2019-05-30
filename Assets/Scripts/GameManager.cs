using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Audio;
using AnimatorStateMachineLibrary;

[RequireComponent(typeof(Animator))]
public class GameManager : MonoBehaviour
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

    [Header("Field Areas")]
    [SerializeField] private VerticalListableAreas[] verticalListableAreasList = null;

    [Header("Canvas Elements")]
    [SerializeField] private Canvas uiCanvas = null;
    [SerializeField] private SpecialRuleText sameRuleTextPrefab = null;
    [SerializeField] private SpecialRuleText plusRuleTextPrefab = null;
    [SerializeField] private SpecialRuleText comboRuleTextPrefab = null;
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

    private Animator animator;
    private Animator Animator {
        get {
            if (animator == null) {
                animator = GetComponent<Animator>();
            }
            return animator;
        }
    }

    private CardBoardArea[,] selectableAreasList = new CardBoardArea[3, 3];
    private Dictionary<PlayerNumber, CardsHand> cardHandByPlayer = new Dictionary<PlayerNumber, CardsHand>();

    private int cardPlayedCount = 0;
    private int cardsRotateCount = 0;
    private int cardsRotationFinishedCount = 0;
    private int cardsWonCount = 0;

    private bool isComboEnabled;

    private CardBoardArea playedCardBoardArea = null;
    private Dictionary<CardDirection, CardBoardArea> playedCardOpponentCardAreasByDirection = null;

    private List<CardWon> cardsWonList = new List<CardWon>();

    public bool HasRuleSet(GameRule _rulesToTest) {
        return activeGameRules.HasFlag(_rulesToTest);
    }

    public CardDirection GetOppositeDirection(CardDirection _targetDirection) {
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

    [StateEnterMethod("Base Layer.Intro")]
    private void IntroState() {
        int[] randomCardLevelArray = new int[5];
        for (int i = 0; i < randomCardLevelArray.Length; i++) {
            randomCardLevelArray[i] = Random.Range(0, cardsListArray.Length - 1);
        }

        CardDatas[] playerCards = new CardDatas[cardsPerPlayer];
        for (int i = 0; i < playerCards.Length; i++) {
            playerCards[i] = cardsListArray[randomCardLevelArray[i]].GetRandomCard();
        }
        playerOneCardsHand.Init(playerCards, false);
        playerOneCardsHand.OnHandReady += PlayerHandReady;

        playerCards = new CardDatas[cardsPerPlayer];
        for (int i = 0; i < playerCards.Length; i++) {
            playerCards[i] = cardsListArray[randomCardLevelArray[i]].GetRandomCard();
        }
        playerTwoCardsHand.Init(playerCards, false);
        playerOneCardsHand.OnHandReady += PlayerHandReady;

        cardHandByPlayer[PlayerNumber.One] = playerOneCardsHand;
        cardHandByPlayer[PlayerNumber.Two] = playerTwoCardsHand;

        for (int positionX = 0; positionX < selectableAreasList.GetLength(0); positionX++) {
            for (int positionY = 0; positionY < selectableAreasList.GetLength(1); positionY++) {
                CardBoardArea newSelectableArea = verticalListableAreasList[positionX][positionY];
                newSelectableArea.OnCardPlayed += PlayedCardPhase;
                newSelectableArea.OnCardAnimationFinished += CardAnimationFinished;
                newSelectableArea.BoardCoordinates = new Vector2Int(positionX, positionY);
                selectableAreasList[positionX, positionY] = newSelectableArea;
            }
        }

        currentPlayer = Random.Range(0, 2) < 1 ? PlayerNumber.One : PlayerNumber.Two;

        AudioManager.Instance.PlayGameMusic();
    }
    private void PlayerHandReady(CardsHand _cardHandReady) {
        _cardHandReady.OnHandReady -= PlayerHandReady;

        bool isAllCardHandsReady = true;
        foreach (CardsHand cardsHandItem in cardHandByPlayer.Values) {
            isAllCardHandsReady = cardsHandItem.Ready;
            if (isAllCardHandsReady == false) {
                break;
            }
        }

        if (isAllCardHandsReady) {
            randomArrow.OnAnimationComplete += BeginGame;
            randomArrow.StartAnimation(currentPlayer);
        }
    }

    private void BeginGame() {
        randomArrow.OnAnimationComplete -= BeginGame;
        Animator.SetTrigger("NextState");
    }

    [StateEnterMethod("Base Layer.PickCard")]
    private void PickCardState() {
        switch (CurrentPlayer) {
            case PlayerNumber.One:
                CurrentPlayer = PlayerNumber.Two;
                break;

            case PlayerNumber.Two:
                CurrentPlayer = PlayerNumber.One;
                break;
        }
    }

    private void PlayedCardPhase(CardBoardArea _playedCardArea) {
        cardPlayedCount++;

        cardHandByPlayer[CurrentPlayer].RemoveCard(_playedCardArea.Card);
        cardHandByPlayer[CurrentPlayer].Enable(false);

        playedCardBoardArea = _playedCardArea;
        playedCardOpponentCardAreasByDirection = GetCardAreasAround(_playedCardArea);

        // if (!isComboUpdate && activeGameRules.HasFlag(GameRule.Plus)) {
        //     foreach (List<CardWon> plusCardsValuesItem in plusCardsValuesList.Values) {
        //         if (plusCardsValuesItem.Count > 1) {
        //             plusCardsWonList.AddRange(plusCardsValuesItem);
        //         }
        //     }
        // }

        Animator.SetTrigger("NextState");
    }


    [StateEnterMethod("Base Layer.SameRule")]
    private void SameRuleState() {
        if (activeGameRules.HasFlag(GameRule.Same)) {
            cardsWonList.Clear();

            foreach (KeyValuePair<CardDirection, CardBoardArea> opponentCardByDirectionItem in playedCardOpponentCardAreasByDirection) {
                CardDirection opponentCardOppositeDirection = GetOppositeDirection(opponentCardByDirectionItem.Key);
                int powerDiff = ((int) playedCardBoardArea.Card.GetPowerByDirection(opponentCardByDirectionItem.Key)) - ((int) opponentCardByDirectionItem.Value.Card.GetPowerByDirection(opponentCardOppositeDirection));

                if (powerDiff == 0 && opponentCardByDirectionItem.Value.Card.PlayerOwner != playedCardBoardArea.Card.PlayerOwner) {
                    cardsWonCount++;
                    cardsWonList.Add(new CardWon() {
                        card = opponentCardByDirectionItem.Value.Card,
                        direction = opponentCardOppositeDirection,
                        newPlayerOwner = playedCardBoardArea.Card.PlayerOwner,
                    });
                }
            }

            if (cardsWonList.Count > 1) {
                ProcessWonCardsList();
                isComboEnabled = true;
            }
            else {
                Animator.SetTrigger("NextState");
            }
        }
        else {
            Animator.SetTrigger("NextState");
        }
    }


    [StateEnterMethod("Base Layer.PlusRule")]
    private void PlusRuleState() {
        if (activeGameRules.HasFlag(GameRule.Same)) {
            cardsWonList.Clear();

            // TODO plus rule
            Animator.SetTrigger("NextState");
        }
        else {
            Animator.SetTrigger("NextState");
        }
    }


    [StateEnterMethod("Base Layer.Fight")]
    private void FightState() {
        cardsWonList.Clear();
    }

    private void ProcessComboFightsPhase() {

    }

    private List<CardWon> CardFight(CardBoardArea _targetCardBoardArea) {
        List<CardWon> cardsWonInFight = new List<CardWon>();

        Dictionary<CardDirection, CardBoardArea> opponentCardBoardAreas = GetCardAreasAround(_targetCardBoardArea);

        foreach (KeyValuePair<CardDirection, CardBoardArea> opponentCardBoardAreaItem in opponentCardBoardAreas) {

        }

        return cardsWonInFight;
    }

    private void ProcessWonCardsList() {
        foreach (CardWon cardWonItem in cardsWonList) {
            cardWonItem.card.ChangePlayerOwner(cardWonItem.direction, cardWonItem.newPlayerOwner);
        }
    }

    private void CardsWonAnimationStart(List<CardWon> _cardsWonList) {
        cardsRotationFinishedCount = 0;
        foreach (CardWon cardWonItem in _cardsWonList) {
            cardWonItem.card.ChangePlayerOwner(cardWonItem.direction, cardWonItem.newPlayerOwner);
            cardsRotateCount++;
            cardsWonCount++;
        }
        _cardsWonList.Clear();
    }

    private void CardAnimationFinished(CardBoardArea _cardTarget) {
        cardsRotationFinishedCount++;
        if (cardsRotationFinishedCount == cardsRotateCount) {
            cardsRotationFinishedCount = 0;
            ProcessWonCardsList();
        }
    }

    private void BetweenTurnPhase() {
        isComboEnabled = false;
        playedCardOpponentCardAreasByDirection.Clear();

        switch (CurrentPlayer) {
            case PlayerNumber.One:
                cardHandByPlayer[PlayerNumber.One].CurrentPlayerScore += cardsWonCount;
                cardHandByPlayer[PlayerNumber.Two].CurrentPlayerScore -= cardsWonCount;
                break;

            case PlayerNumber.Two:
                cardHandByPlayer[PlayerNumber.Two].CurrentPlayerScore += cardsWonCount;
                cardHandByPlayer[PlayerNumber.One].CurrentPlayerScore -= cardsWonCount;
                break;
        }

        cardsWonCount = 0;

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

        if (cardHandByPlayer[PlayerNumber.One].CurrentPlayerScore > cardHandByPlayer[PlayerNumber.Two].CurrentPlayerScore) {
            // See ColorUtility.ToHtmlStringRGB
            winText.text = "<color=\"blue\">Blue</color>\nwins !";
            AudioManager.Instance.PlayVictoryMusic();
        }
        else if (cardHandByPlayer[PlayerNumber.One].CurrentPlayerScore < cardHandByPlayer[PlayerNumber.Two].CurrentPlayerScore) {
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
}
