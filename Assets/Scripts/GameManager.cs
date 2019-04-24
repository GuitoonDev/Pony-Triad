using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    private readonly int cardsPerPlayer = 5;

    [Header("Cards List")]
    [SerializeField] private CardList cardsList = null;

    [Header("Player One")]
    [SerializeField] private TextMeshPro playerOneScoreText = null;
    [SerializeField] private CardsHand playerOneCardsHand = null;

    [Header("Player Two")]
    [SerializeField] private TextMeshPro playerTwoScoreText = null;
    [SerializeField] private CardsHand playerTwoCardsHand = null;

    [Header("Field Areas")]
    [SerializeField] private VerticalListableAreas[] verticalListableAreasList = null;

    private SelectableArea[,] selectableAreasList = new SelectableArea[3, 3];

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

    private int cardPlayedCount = 0;

    private void Start() {
        CardDatas[] playerCards = new CardDatas[cardsPerPlayer];
        for (int i = 0; i < playerCards.Length; i++) {
            playerCards[i] = cardsList.GetRandomCard();
        }
        playerOneCardsHand.Init(playerCards);
        CurrentPlayerOneScore = cardsPerPlayer;

        playerCards = new CardDatas[cardsPerPlayer];
        for (int i = 0; i < playerCards.Length; i++) {
            playerCards[i] = cardsList.GetRandomCard();
        }
        playerTwoCardsHand.Init(playerCards);
        CurrentPlayerTwoScore = cardsPerPlayer;

        for (int positionX = 0; positionX < selectableAreasList.GetLength(0); positionX++) {
            for (int positionY = 0; positionY < selectableAreasList.GetLength(1); positionY++) {
                SelectableArea newSelectableArea = verticalListableAreasList[positionX][positionY];
                newSelectableArea.OnCardPlayed += UpdateCardsOwners;
                newSelectableArea.BoardCoordinates = new Vector2Int(positionX, positionY);
                selectableAreasList[positionX, positionY] = newSelectableArea;
            }
        }
    }

    private void UpdateCardsOwners(SelectableArea _selectableArea) {
        Debug.LogWarningFormat("Card played coordinates : {0}", _selectableArea.BoardCoordinates);

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

        switch (_selectableArea.Card.PlayerOwner) {
            case PlayerNumber.One:
                CurrentPlayerOneScore += cardsWon;
                CurrentPlayerTwoScore -= cardsWon;
                break;

            case PlayerNumber.Two:
                CurrentPlayerTwoScore += cardsWon;
                CurrentPlayerOneScore -= cardsWon;
                break;
        }

        CheckEndGame();
    }

    private void CheckEndGame() {
        if (cardPlayedCount == selectableAreasList.GetLength(0) * selectableAreasList.GetLength(1)) {
            Debug.Log("End of the game");
            if (currentPlayerOneScore > currentPlayerTwoScore) {
                Debug.Log("Player One wins !");
            }
            else if (currentPlayerOneScore < currentPlayerTwoScore) {
                Debug.Log("Player Two wins !");
            }
            else {
                Debug.Log("Draw game !");
            }
        }
    }

    [System.Serializable]
    public class VerticalListableAreas
    {
        [HideInInspector] public string name = "Vertical List";

        [SerializeField] private SelectableArea[] items = null;

        public SelectableArea this[int _index] => items[_index];
    }

}
