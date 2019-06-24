public struct CardBoardAreaWon
{
    public CardBoardPartView cardBoardArea;
    public CardDirection direction;

    public CardBoardAreaWon(CardBoardPartView _cardBoardArea, CardDirection _direction) {
        cardBoardArea = _cardBoardArea;
        direction = _direction;
    }
}