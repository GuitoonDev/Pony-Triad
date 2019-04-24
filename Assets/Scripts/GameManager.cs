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
    private int currentPlayerTwoScore = 0;

    private int cardPlayedCount = 0;

    private void Start() {
        CardDatas[] playerCards = new CardDatas[cardsPerPlayer];
        for (int i = 0; i < playerCards.Length; i++) {
            playerCards[i] = cardsList.GetRandomCard();
        }
        playerOneCardsHand.Init(playerCards);
        currentPlayerOneScore = cardsPerPlayer;
        playerOneScoreText.text = currentPlayerOneScore.ToString();

        playerCards = new CardDatas[cardsPerPlayer];
        for (int i = 0; i < playerCards.Length; i++) {
            playerCards[i] = cardsList.GetRandomCard();
        }
        playerTwoCardsHand.Init(playerCards);
        currentPlayerTwoScore = cardsPerPlayer;
        playerTwoScoreText.text = currentPlayerTwoScore.ToString();

        for (int positionX = 0; positionX < selectableAreasList.GetLength(0); positionX++) {
            for (int positionY = 0; positionY < selectableAreasList.GetLength(1); positionY++) {
                SelectableArea newSelectableArea = verticalListableAreasList[positionX][positionY];
                newSelectableArea.OnCardPlayed += CheckNeighbourCardsPower;
                newSelectableArea.BoardCoordinates = new Vector2Int(positionX, positionY);
                selectableAreasList[positionX, positionY] = newSelectableArea;
            }
        }
    }

    private void CheckNeighbourCardsPower(SelectableArea _selectableArea) {
        Debug.LogWarningFormat("Card played coordinates : {0}", _selectableArea.BoardCoordinates);

        int leftPosition = _selectableArea.BoardCoordinates.x - 1;
        int rightPosition = _selectableArea.BoardCoordinates.x + 1;
        int upPosition = _selectableArea.BoardCoordinates.y - 1;
        int downPosition = _selectableArea.BoardCoordinates.y + 1;

        int cardsWon = 0;

        bool isPlayedCardSuperior;
        Card cardToCompare = selectableAreasList[leftPosition, _selectableArea.BoardCoordinates.y].Card;
        bool isCardExists = (leftPosition >= 0 && cardToCompare != null);
        if (isCardExists) {
            isPlayedCardSuperior = (cardToCompare.Datas.PowerRight < _selectableArea.Card.Datas.PowerLeft);
            if (isPlayedCardSuperior) {
                cardsWon++;
                cardToCompare.PlayerOwner = _selectableArea.Card.PlayerOwner;
            }
        }

        if (rightPosition < selectableAreasList.GetLength(0)) {

        }

        if (upPosition >= 0) {

        }

        if (downPosition < selectableAreasList.GetLength(1)) {

        }

        CheckEndGame();
    }

    private void CheckEndGame() {
        cardPlayedCount++;
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
