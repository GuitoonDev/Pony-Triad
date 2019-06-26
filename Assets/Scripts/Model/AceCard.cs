namespace PonyTriad.Model
{
    public class AceCard : Card
    {
        public AceCard() {
            PowerUp = CardPower.Ace;
            PowerDown = CardPower.Ace;
            PowerLeft = CardPower.Ace;
            PowerRight = CardPower.Ace;

            Sprite = null;

            PlayerOwner = PlayerNumber.None;
        }
    }
}
