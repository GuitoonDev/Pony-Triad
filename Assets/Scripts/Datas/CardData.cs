using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "ProtoTriad/CardData")]
public class CardData : ScriptableObject
{
    public enum CardPower
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ace = 10
    }

    [SerializeField] private Sprite spriteImage = null;
    public Sprite SpriteImage {
        get {
            return spriteImage;
        }
    }

    [Header("Sides Power")]
    [SerializeField] private CardPower powerUp = CardPower.One;
    public CardPower PowerUp {
        get {
            return powerUp;
        }
    }

    [SerializeField] private CardPower powerDown = CardPower.One;
    public CardPower PowerDown {
        get {
            return powerDown;
        }
    }

    [SerializeField] private CardPower powerLeft = CardPower.One;
    public CardPower PowerLeft {
        get {
            return powerLeft;
        }
    }

    [SerializeField] private CardPower powerRight = CardPower.One;
    public CardPower PowerRight {
        get {
            return powerRight;
        }
    }
}
