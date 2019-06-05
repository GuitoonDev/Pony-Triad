using UnityEngine;

[CreateAssetMenu(fileName = "CardList", menuName = "ProtoTriad/CardList")]
public class CardList : ScriptableObject
{
    [SerializeField] private CardData[] cardDataArray = null;

    public CardData GetRandomCard() {
        int randomCardIndex = Random.Range(0, cardDataArray.Length);
        return cardDataArray[randomCardIndex];
    }

    public CardData[] GetRandomCard(int _cardToDrawNumber) {
        CardData[] cardsPicked = new CardData[_cardToDrawNumber];

        for (int i = 0; i < cardsPicked.Length; i++) {
            int randomCardIndex = Random.Range(0, cardDataArray.Length);
            cardsPicked[i] = cardDataArray[randomCardIndex];
        }

        return cardsPicked;
    }
}
