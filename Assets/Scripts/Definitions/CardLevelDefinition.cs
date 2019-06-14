using UnityEngine;

[CreateAssetMenu(fileName = "CardList", menuName = "ProtoTriad/CardList")]
public class CardLevelDefinition : ScriptableObject
{
    [SerializeField] private CardDefinition[] cardDataArray = null;

    public CardDefinition GetRandomCard() {
        int randomCardIndex = Random.Range(0, cardDataArray.Length);
        return cardDataArray[randomCardIndex];
    }

    public CardDefinition[] GetRandomCard(int _cardToDrawNumber) {
        CardDefinition[] cardsPicked = new CardDefinition[_cardToDrawNumber];

        for (int i = 0; i < cardsPicked.Length; i++) {
            int randomCardIndex = Random.Range(0, cardDataArray.Length);
            cardsPicked[i] = cardDataArray[randomCardIndex];
        }

        return cardsPicked;
    }
}
