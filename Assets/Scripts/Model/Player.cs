using System.Collections.Generic;
using UnityEngine.Events;

namespace PonyTriad.Model
{
    public class Player
    {
        public UnityAction OnUpdate;

        public int Score { get; set; }

        public List<Card> CardHand { get; private set; }

        public PlayerNumber Number { get; private set; }

        public Player(PlayerNumber _playerNumber, CardLevelDefinition[] _cardDefinitionArrayByLevel, int[] _randomCardLevelArray) {
            CardHand = new List<Card>();

            for (int i = 0; i < _randomCardLevelArray.Length; i++) {
                CardDefinition randomPickedCard = _cardDefinitionArrayByLevel[_randomCardLevelArray[i]].GetRandomCard();

                Card newCard = new Card(_playerNumber, randomPickedCard);
                CardHand.Add(newCard);
            }
            CardHand.Shuffle();

            Score = CardHand.Count;
            Number = _playerNumber;
        }

        public Player(PlayerNumber _playerNumber, CardLevelDefinition[] _cardDefinitionArrayByLevel, List<Card> _definedCardList) {
            CardHand = _definedCardList;
            CardHand.Shuffle();

            Score = CardHand.Count;
            Number = _playerNumber;
        }

        public void PlayCard(Card _playedCard) {
            CardHand.Remove(_playedCard);
        }
    }
}
