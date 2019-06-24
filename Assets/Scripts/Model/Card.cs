using UnityEngine;

public class Card
{
    public CardPower Up { get; private set; }
    public CardPower Down { get; private set; }
    public CardPower Left { get; private set; }
    public CardPower Right { get; private set; }

    public Sprite Sprite { get; private set; }

    public PlayerNumber PlayerOwner { get; set; }

    public Card(CardDefinition _definition, PlayerNumber _playerNumber) {
        Up = _definition.PowerUp;
        Down = _definition.PowerDown;
        Left = _definition.PowerLeft;
        Right = _definition.PowerRight;

        Sprite = _definition.SpriteImage;

        PlayerOwner = _playerNumber;
    }
}
