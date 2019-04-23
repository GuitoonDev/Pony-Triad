using UnityEngine;
using TMPro;

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
                selectableAreasList[positionX, positionY] = newSelectableArea;
            }
        }
    }

    private void CheckNeighbourCardsPower(SelectableArea _selectableArea) {
        Debug.Log("Card played");

        cardPlayedCount++;
        if (cardPlayedCount == selectableAreasList.GetLength(0) * selectableAreasList.GetLength(1)) {
            Debug.Log("End of the game");
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
