﻿using System.Collections.Generic;
using UnityEngine;

namespace PonyTriad.Model
{
    public class Board
    {
        public Card[,] CardsPlayedArray { get; private set; }

        public Board() {
            CardsPlayedArray = new Card[3, 3];
        }

        public void PlayCard(Card _playedCard, Vector2Int _cardPlayedCoordinates) {
            bool isBoardPartEmpty = (CardsPlayedArray[_cardPlayedCoordinates.x, _cardPlayedCoordinates.y] == null);
            if (isBoardPartEmpty) {
                CardsPlayedArray[_cardPlayedCoordinates.x, _cardPlayedCoordinates.y] = _playedCard;
            }
        }

        public Dictionary<PlayerNumber, int> GetBoardScoreByPlayer() {
            Dictionary<PlayerNumber, int> scoreResult = new Dictionary<PlayerNumber, int>();

            for (int positionX = 0; positionX < CardsPlayedArray.GetLength(0); positionX++) {
                for (int positionY = 0; positionY < CardsPlayedArray.GetLength(1); positionY++) {
                    Card currentCard = CardsPlayedArray[positionX, positionY];

                    if (currentCard != null) {
                        PlayerNumber currentCardPlayerOwner = currentCard.PlayerOwner;

                        int playerScore;
                        if (!scoreResult.TryGetValue(currentCardPlayerOwner, out playerScore)) {
                            scoreResult[currentCardPlayerOwner] = 0;
                        }
                        scoreResult[currentCardPlayerOwner] = playerScore + 1;
                    }
                }
            }

            return scoreResult;
        }

        public Dictionary<PlayerNumber, List<Card>> GetCurrentOwnedCardByPlayer() {
            Dictionary<PlayerNumber, List<Card>> currentOwnedCardByPlayer = new Dictionary<PlayerNumber, List<Card>>();

            List<Card> playerOneCardDeck = new List<Card>();
            List<Card> playerTwoCardDeck = new List<Card>();

            for (int positionX = 0; positionX < CardsPlayedArray.GetLength(0); positionX++) {
                for (int positionY = 0; positionY < CardsPlayedArray.GetLength(1); positionY++) {
                    Card currentCard = CardsPlayedArray[positionX, positionY];

                    if (currentCard != null) {
                        switch (currentCard.PlayerOwner) {
                            case PlayerNumber.One:
                                playerOneCardDeck.Add(currentCard);
                                break;

                            case PlayerNumber.Two:
                                playerTwoCardDeck.Add(currentCard);
                                break;
                        }
                    }
                }
            }

            currentOwnedCardByPlayer[PlayerNumber.One] = playerOneCardDeck;
            currentOwnedCardByPlayer[PlayerNumber.Two] = playerTwoCardDeck;
            return currentOwnedCardByPlayer;
        }
    }
}
