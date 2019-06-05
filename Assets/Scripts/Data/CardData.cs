using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "ProtoTriad/CardData")]
public class CardData : ScriptableObject
{
    [SerializeField] private Sprite spriteImage = null;
    public Sprite SpriteImage => spriteImage;

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
