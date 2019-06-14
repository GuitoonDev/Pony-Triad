public struct CardBoardAreaWon
{
    public CardBoardView cardBoardArea;
    public CardDirection direction;

    public CardBoardAreaWon(CardBoardView _cardBoardArea, CardDirection _direction) {
        cardBoardArea = _cardBoardArea;
        direction = _direction;
    }
}