using UnityEngine;
using PonyTriad.Model;

public struct CardOnBoardWon
{
    public Card card;
    public CardDirection direction;

    public CardOnBoardWon(Card _card, CardDirection _direction) {
        card = _card;
        direction = _direction;
    }
}