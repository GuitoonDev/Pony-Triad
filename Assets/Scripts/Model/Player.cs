public class Player
{
    private const int PLAYER_CARDS_AT_START = 5;

    public Card[] cardHand;

    public int score;

    public PlayerNumber number;

    public Player(PlayerNumber _playerNumber) {
        cardHand = new Card[PLAYER_CARDS_AT_START];

        score = PLAYER_CARDS_AT_START;

        number = _playerNumber;
    }
}
