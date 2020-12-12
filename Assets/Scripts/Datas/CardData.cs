using UnityEngine;
using UnityEngine.Serialization;

//[CreateAssetMenu(fileName = "CardData", menuName = "Pony Triad/Card Data")]
public class CardData : ScriptableObject
{
    public Sprite spriteImage;

    [Header("Up")]
    public CardPower powerUp = CardPower.Zero;
    [Header("Down")]
    public CardPower powerDown = CardPower.Zero;
    [Header("Left")]
    public CardPower powerLeft = CardPower.Zero;
    [Header("Right")]
    public CardPower powerRight = CardPower.Zero;
}
