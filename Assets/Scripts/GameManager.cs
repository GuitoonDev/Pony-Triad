using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    private readonly int cardsPerPlayer = 5;

    [Header("Cards List")]
    [SerializeField] private CardList cardsList = null;

    [Header("Player One")]
    [SerializeField] private TextMeshPro playerOneScore = null;
    [SerializeField] private CardsHand playerOneCardsHand = null;

    [Header("Player Two")]
    [SerializeField] private TextMeshPro playerTwoScore = null;
    [SerializeField] private CardsHand playerTwoCardsHand = null;

    [Header("Field Areas")]
    [SerializeField] private VerticalListableAreas[] areasList = null;

    private void Start() {
        CardDatas[] playerCards = new CardDatas[cardsPerPlayer];
        for (int i = 0; i < playerCards.Length; i++) {
            playerCards[i] = cardsList.GetRandomCard();
        }
        playerOneCardsHand.Init(playerCards);

        playerCards = new CardDatas[cardsPerPlayer];
        for (int i = 0; i < playerCards.Length; i++) {
            playerCards[i] = cardsList.GetRandomCard();
        }
        playerTwoCardsHand.Init(playerCards);
    }

    [System.Serializable]
    public class VerticalListableAreas
    {
        [HideInInspector] public string name = "Vertical List";

        [SerializeField] private SelectableArea[] items = null;

        public SelectableArea this[int _index] => items[_index];
    }

}
