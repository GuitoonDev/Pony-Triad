using UnityEngine;

[CreateAssetMenu(fileName = "CardDefinition", menuName = "ProtoTriad/Card Definition")]
public class CardDefinition : ScriptableObject
{
    [SerializeField] private Sprite spriteImage = null;
    public Sprite SpriteImage => spriteImage;

    [Header("Sides Power")]
    [SerializeField] private CardPower powerUp = CardPower.Zero;
    public CardPower PowerUp { get { return powerUp; } }

    [SerializeField] private CardPower powerDown = CardPower.Zero;
    public CardPower PowerDown { get { return powerDown; } }

    [SerializeField] private CardPower powerLeft = CardPower.Zero;
    public CardPower PowerLeft { get { return powerLeft; } }

    [SerializeField] private CardPower powerRight = CardPower.Zero;
    public CardPower PowerRight { get { return powerRight; } }
}
