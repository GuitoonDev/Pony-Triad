public struct CardWon
{
    public CardBoardArea cardBoardArea;
    public CardDirection direction;
    public PlayerNumber newPlayerOwner;

    public CardWon(CardBoardArea _cardBoardArea, CardDirection _direction, PlayerNumber _newPlayerOwner) {
        cardBoardArea = _cardBoardArea;
        direction = _direction;
        newPlayerOwner = _newPlayerOwner;
    }
}