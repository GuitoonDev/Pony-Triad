public class Game
{
    public Player playerOne;
    public Player playerTwo;

    public Board board;

    public Game() {
        playerOne = new Player(PlayerNumber.One);
        playerTwo = new Player(PlayerNumber.Two);

        board = new Board();
    }
}
