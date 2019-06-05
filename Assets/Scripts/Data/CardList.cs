using UnityEngine;

[CreateAssetMenu(fileName = "CardList", menuName = "ProtoTriad/CardList")]
public class CardList : ScriptableObject
{
    [SerializeField]
    private CardData[] cardDatasArray = null;

    public CardData GetRandomCard() {
        int randomCardIndex = Random.Range(0, cardDatasArray.Length);
        return cardDatasArray[randomCardIndex];
    }

    public CardData[] GetRandomCard(int _cardToDrawNumber) {
        CardData[] cardsPicked = new CardData[_cardToDrawNumber];

        for (int i = 0; i < cardsPicked.Length; i++) {
            int randomCardIndex = Random.Range(0, cardDatasArray.Length);
            cardsPicked[i] = cardDatasArray[randomCardIndex];
        }

        return cardsPicked;
    }
}
