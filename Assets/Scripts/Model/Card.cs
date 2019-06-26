using UnityEngine;

namespace PonyTriad.Model
{
    public class Card
    {
        public virtual Sprite Sprite { get; protected set; }

        public virtual CardPower PowerUp { get; protected set; }
        public virtual CardPower PowerDown { get; protected set; }
        public virtual CardPower PowerLeft { get; protected set; }
        public virtual CardPower PowerRight { get; protected set; }

        public virtual PlayerNumber PlayerOwner { get; set; }
        public virtual Vector2Int? BoardPosition { get; set; }

        public virtual CardPower GetPowerByDirection(CardDirection _targetDirection) {
            CardPower resultPower = CardPower.Zero;

            switch (_targetDirection) {
                case CardDirection.Up:
                    resultPower = PowerUp;
                    break;

                case CardDirection.Down:
                    resultPower = PowerDown;
                    break;

                case CardDirection.Left:
                    resultPower = PowerLeft;
                    break;

                case CardDirection.Right:
                    resultPower = PowerRight;
                    break;
            }

            return resultPower;
        }

        public Card() {

        }

        public Card(PlayerNumber _playerNumber, CardDefinition _definition) {
            PowerUp = _definition.PowerUp;
            PowerDown = _definition.PowerDown;
            PowerLeft = _definition.PowerLeft;
            PowerRight = _definition.PowerRight;

            Sprite = _definition.SpriteImage;

            PlayerOwner = _playerNumber;
        }
    }
}
