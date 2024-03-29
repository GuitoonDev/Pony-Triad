﻿using System.Collections.Generic;
using UnityEngine;

namespace PonyTriad.Model
{
    public class Game
    {
        public static GameRule activeRules;

        public Board board;

        private Dictionary<PlayerNumber, Player> playerByNumber = new Dictionary<PlayerNumber, Player>();
        private PlayerNumber currentPlayer;

        private int cardPlayedCount = 0;
        public bool IsOver {
            get {
                return cardPlayedCount == board.CardsPlayedArray.GetLength(0) * board.CardsPlayedArray.GetLength(1);
            }
        }

        public Player GetPlayerByNumber(PlayerNumber _targetPlayer) {
            return playerByNumber[_targetPlayer];
        }

        public Dictionary<PlayerNumber, List<Card>> GetCurrentOwnedCardByPlayer() {
            Dictionary<PlayerNumber, List<Card>> currentOwnedCardByPlayer = board.GetCurrentOwnedCardByPlayer();

            currentOwnedCardByPlayer[PlayerNumber.One].AddRange(playerByNumber[PlayerNumber.One].CardHand);
            currentOwnedCardByPlayer[PlayerNumber.Two].AddRange(playerByNumber[PlayerNumber.Two].CardHand);

            return currentOwnedCardByPlayer;
        }

        public Game(CardLevelData[] _cardDefinitionArrayByLevel, int _cardNumber, GameRule _activeGameRules) {
            Dictionary<PlayerNumber, List<Card>> customDeckByPlayer = CustomGameHolder.NextCardDeckByPlayer;
            if (customDeckByPlayer != null) {
                playerByNumber[PlayerNumber.One] = new Player(PlayerNumber.One, _cardDefinitionArrayByLevel, customDeckByPlayer[PlayerNumber.One]);
                playerByNumber[PlayerNumber.Two] = new Player(PlayerNumber.Two, _cardDefinitionArrayByLevel, customDeckByPlayer[PlayerNumber.Two]);
            }
            else {
                int[] randomCardLevelArray = new int[_cardNumber];
                for (int i = 0; i < randomCardLevelArray.Length; i++) {
                    randomCardLevelArray[i] = Random.Range(0, _cardDefinitionArrayByLevel.Length);
                }

                playerByNumber[PlayerNumber.One] = new Player(PlayerNumber.One, _cardDefinitionArrayByLevel, randomCardLevelArray);
                playerByNumber[PlayerNumber.Two] = new Player(PlayerNumber.Two, _cardDefinitionArrayByLevel, randomCardLevelArray);
            }

            board = new Board();

            activeRules = _activeGameRules;
        }

        public Dictionary<GamePhase, ResultPhaseData> PlayCard(PlayerNumber _currentPlayer, Card _playedCard, Vector2Int _boardPosition) {
            cardPlayedCount++;

            playerByNumber[_currentPlayer].PlayCard(_playedCard);

            Dictionary<GamePhase, ResultPhaseData> playerTurnResultByPhase = new Dictionary<GamePhase, ResultPhaseData>();

            _playedCard.BoardPosition = _boardPosition;
            board.CardsPlayedArray[_boardPosition.x, _boardPosition.y] = _playedCard;

            Dictionary<CardDirection, Card> adjacentCardsByDirection = GetAdjacentCardsOnBoard(_boardPosition);

            if (activeRules.HasFlag(GameRule.Same)) {
                ProcessSamePhase(_playedCard, adjacentCardsByDirection, ref playerTurnResultByPhase);
            }

            if (activeRules.HasFlag(GameRule.Plus)) {
                ProcessPlusPhase(_playedCard, adjacentCardsByDirection, ref playerTurnResultByPhase);
            }

            ProcessNormalPhase(_playedCard, adjacentCardsByDirection, ref playerTurnResultByPhase);

            UpdatePlayersScore();

            return playerTurnResultByPhase;
        }

        private void UpdatePlayersScore() {
            int playerOneCardOnBoardCount = 0, playerTwoCardOnBoardCount = 0;

            for (int x = 0; x < board.CardsPlayedArray.GetLength(0); x++) {
                for (int y = 0; y < board.CardsPlayedArray.GetLength(1); y++) {
                    Card cardOnBoardItem = board.CardsPlayedArray[x, y];

                    if (cardOnBoardItem != null) {
                        switch (cardOnBoardItem.PlayerOwner) {
                            case PlayerNumber.One:
                                playerOneCardOnBoardCount++;
                                break;

                            case PlayerNumber.Two:
                                playerTwoCardOnBoardCount++;
                                break;
                        }
                    }
                }
            }

            playerByNumber[PlayerNumber.One].Score = playerOneCardOnBoardCount + playerByNumber[PlayerNumber.One].CardHand.Count;
            playerByNumber[PlayerNumber.Two].Score = playerTwoCardOnBoardCount + playerByNumber[PlayerNumber.Two].CardHand.Count;
        }

        private void ProcessSamePhase(Card _playedCard, Dictionary<CardDirection, Card> _adjacentCardsByDirection, ref Dictionary<GamePhase, ResultPhaseData> _turnResultByPhase) {
            List<CardOnBoardWon> cardWonList = new List<CardOnBoardWon>();

            foreach (KeyValuePair<CardDirection, Card> opponentCardItem in _adjacentCardsByDirection) {
                CardDirection oppositeDirection = GetOppositeDirection(opponentCardItem.Key);

                bool isSamePower = (_playedCard.GetPowerByDirection(opponentCardItem.Key) == opponentCardItem.Value.GetPowerByDirection(oppositeDirection));
                bool isRealCard = (opponentCardItem.Value.BoardPosition.HasValue);
                if (isSamePower && isRealCard) {
                    cardWonList.Add(new CardOnBoardWon(opponentCardItem.Value, oppositeDirection));
                }
            }

            bool allCardsAreOwnedByCurrentPlayer = true;
            foreach (CardOnBoardWon cardItem in cardWonList) {
                if (cardItem.card.PlayerOwner != _playedCard.PlayerOwner) {
                    allCardsAreOwnedByCurrentPlayer = false;
                }
            }

            if (cardWonList.Count > 1 && !allCardsAreOwnedByCurrentPlayer) {
                ResultPhaseData sameResultPhaseData = new ResultPhaseData();
                foreach (CardOnBoardWon cardItem in cardWonList) {
                    cardItem.card.PlayerOwner = _playedCard.PlayerOwner;
                }
                sameResultPhaseData.cardWonList = cardWonList;

                ProcessComboPhase(cardWonList, ref sameResultPhaseData.comboCardList);

                _turnResultByPhase[GamePhase.Same] = sameResultPhaseData;
            }
        }

        private void ProcessPlusPhase(Card _playedCard, Dictionary<CardDirection, Card> _adjacentCardsByDirection, ref Dictionary<GamePhase, ResultPhaseData> _turnResultByPhase) {
            List<CardOnBoardWon> cardWonList = new List<CardOnBoardWon>();
            Dictionary<int, List<CardOnBoardWon>> cardWonByAddPowers = new Dictionary<int, List<CardOnBoardWon>>();

            foreach (KeyValuePair<CardDirection, Card> opponentCardItem in _adjacentCardsByDirection) {
                bool isRealCard = (opponentCardItem.Value.BoardPosition.HasValue);
                if (isRealCard) {
                    CardDirection opponentCardOppositeDirection = GetOppositeDirection(opponentCardItem.Key);

                    int powerAdd = ((int) _playedCard.GetPowerByDirection(opponentCardItem.Key)) + ((int) opponentCardItem.Value.GetPowerByDirection(opponentCardOppositeDirection));

                    List<CardOnBoardWon> powerAddCardWonList;
                    if (!cardWonByAddPowers.TryGetValue(powerAdd, out powerAddCardWonList)) {
                        powerAddCardWonList = new List<CardOnBoardWon>();
                        cardWonByAddPowers[powerAdd] = powerAddCardWonList;
                    }

                    powerAddCardWonList.Add(new CardOnBoardWon(opponentCardItem.Value, opponentCardOppositeDirection));
                }
            }

            foreach (List<CardOnBoardWon> cardWonByAddPowerItem in cardWonByAddPowers.Values) {
                if (cardWonByAddPowerItem.Count > 1) {
                    cardWonList.AddRange(cardWonByAddPowerItem);
                }
            }

            bool allCardsAreOwnedByCurrentPlayer = true;
            foreach (CardOnBoardWon cardItem in cardWonList) {
                if (cardItem.card.PlayerOwner != _playedCard.PlayerOwner) {
                    allCardsAreOwnedByCurrentPlayer = false;
                }
            }

            if (cardWonList.Count > 1 && !allCardsAreOwnedByCurrentPlayer) {
                ResultPhaseData plusResultPhaseData = new ResultPhaseData();
                foreach (CardOnBoardWon cardItem in cardWonList) {
                    cardItem.card.PlayerOwner = _playedCard.PlayerOwner;
                }
                plusResultPhaseData.cardWonList = cardWonList;

                ProcessComboPhase(cardWonList, ref plusResultPhaseData.comboCardList);
                _turnResultByPhase[GamePhase.Plus] = plusResultPhaseData;
            }
        }

        private void ProcessNormalPhase(Card _playedCard, Dictionary<CardDirection, Card> _adjacentCardsByDirection, ref Dictionary<GamePhase, ResultPhaseData> _turnResultByPhase) {
            List<CardOnBoardWon> cardWonList = CardFight(_playedCard, _adjacentCardsByDirection);

            if (cardWonList.Count > 0) {
                ResultPhaseData normalResultPhaseData = new ResultPhaseData();
                normalResultPhaseData.cardWonList = cardWonList;
                _turnResultByPhase[GamePhase.Normal] = normalResultPhaseData;
            }
        }

        private void ProcessComboPhase(List<CardOnBoardWon> _cardsWinList, ref Queue<List<CardOnBoardWon>> _comboCardList) {
            List<CardOnBoardWon> newComboCardsList = new List<CardOnBoardWon>();

            foreach (CardOnBoardWon comboCardItem in _cardsWinList) {
                Dictionary<CardDirection, Card> adjacentCardsOnBoard = GetAdjacentCardsOnBoard(comboCardItem.card.BoardPosition.Value);
                List<CardOnBoardWon> cardsWonInFight = CardFight(comboCardItem.card, adjacentCardsOnBoard);
                newComboCardsList.AddRange(cardsWonInFight);
            }

            if (newComboCardsList.Count > 0) {
                if (_comboCardList == null) {
                    _comboCardList = new Queue<List<CardOnBoardWon>>();
                }

                _comboCardList.Enqueue(newComboCardsList);
                ProcessComboPhase(newComboCardsList, ref _comboCardList);
            }
        }

        private List<CardOnBoardWon> CardFight(Card _targetCardOnBoard, Dictionary<CardDirection, Card> _adjacentCardsOnBoard) {
            List<CardOnBoardWon> cardsWonInFight = new List<CardOnBoardWon>();

            foreach (KeyValuePair<CardDirection, Card> opponentCardItem in _adjacentCardsOnBoard) {
                bool isRealCard = (opponentCardItem.Value.BoardPosition.HasValue);
                if (isRealCard) {
                    CardDirection opponentCardOppositeDirection = GetOppositeDirection(opponentCardItem.Key);

                    int powerDiff = ((int) _targetCardOnBoard.GetPowerByDirection(opponentCardItem.Key)) - ((int) opponentCardItem.Value.GetPowerByDirection(opponentCardOppositeDirection));

                    if (opponentCardItem.Value.PlayerOwner != _targetCardOnBoard.PlayerOwner) {
                        bool isTargetCardWon;
                        if (activeRules.HasFlag(GameRule.Reversed)) {
                            if (activeRules.HasFlag(GameRule.FallenAce) && powerDiff == (int) (CardPower.Ace - CardPower.One)) {
                                isTargetCardWon = true;
                            }
                            else {
                                isTargetCardWon = powerDiff < 0;
                            }
                        }
                        else {
                            if (activeRules.HasFlag(GameRule.FallenAce) && powerDiff == (int) (CardPower.One - CardPower.Ace)) {
                                isTargetCardWon = true;
                            }
                            else {
                                isTargetCardWon = powerDiff > 0;
                            }
                        }

                        if (isTargetCardWon) {
                            opponentCardItem.Value.PlayerOwner = _targetCardOnBoard.PlayerOwner;
                            cardsWonInFight.Add(new CardOnBoardWon(opponentCardItem.Value, opponentCardOppositeDirection));
                        }
                    }
                }
            }

            return cardsWonInFight;
        }

        private CardDirection GetOppositeDirection(CardDirection _targetDirection) {
            CardDirection _oppositeDirection = default;
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

        private Dictionary<CardDirection, Card> GetAdjacentCardsOnBoard(Vector2Int? _targetCardBoardPosition) {
            Dictionary<CardDirection, Card> cardBoardAreasAround = new Dictionary<CardDirection, Card>();

            if (_targetCardBoardPosition.Value != null) {
                Vector2Int cardBoardPosition = _targetCardBoardPosition.Value;

                int leftPosition = cardBoardPosition.x - 1;
                int rightPosition = cardBoardPosition.x + 1;
                int upPosition = cardBoardPosition.y - 1;
                int downPosition = cardBoardPosition.y + 1;

                if (activeRules.HasFlag(GameRule.Borderless)) {
                    if (leftPosition < 0) {
                        leftPosition = board.CardsPlayedArray.GetLength(0) - 1;
                    }
                    if (rightPosition >= board.CardsPlayedArray.GetLength(0)) {
                        rightPosition = 0;
                    }
                    if (upPosition < 0) {
                        upPosition = board.CardsPlayedArray.GetLength(1) - 1;
                    }
                    if (downPosition >= board.CardsPlayedArray.GetLength(1)) {
                        downPosition = 0;
                    }
                }
                else if (activeRules.HasFlag(GameRule.Same | GameRule.SameWalls)) {
                    if (leftPosition < 0) {
                        cardBoardAreasAround[CardDirection.Left] = new AceCard();
                    }
                    if (rightPosition >= board.CardsPlayedArray.GetLength(0)) {
                        cardBoardAreasAround[CardDirection.Right] = new AceCard();
                    }
                    if (upPosition < 0) {
                        cardBoardAreasAround[CardDirection.Up] = new AceCard();
                    }
                    if (downPosition >= board.CardsPlayedArray.GetLength(1)) {
                        cardBoardAreasAround[CardDirection.Down] = new AceCard();
                    }
                }

                Card targetCard = board.CardsPlayedArray[leftPosition, cardBoardPosition.y];
                if (leftPosition >= 0 && targetCard != null) {
                    cardBoardAreasAround[CardDirection.Left] = targetCard;
                }

                targetCard = board.CardsPlayedArray[rightPosition, cardBoardPosition.y];
                if (rightPosition < board.CardsPlayedArray.GetLength(0) && targetCard != null) {
                    cardBoardAreasAround[CardDirection.Right] = targetCard;
                }

                targetCard = board.CardsPlayedArray[cardBoardPosition.x, upPosition];
                if (upPosition >= 0 && targetCard != null) {
                    cardBoardAreasAround[CardDirection.Up] = targetCard;
                }

                targetCard = board.CardsPlayedArray[cardBoardPosition.x, downPosition];
                if (downPosition < board.CardsPlayedArray.GetLength(1) && targetCard != null) {
                    cardBoardAreasAround[CardDirection.Down] = targetCard;
                }
            }

            return cardBoardAreasAround;
        }
    }
}
