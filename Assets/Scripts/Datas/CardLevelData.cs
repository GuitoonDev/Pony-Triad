using UnityEngine;

[CreateAssetMenu(fileName = "CardList", menuName = "Pony Triad/Card List")]
public class CardLevelData : ScriptableObject
{
    [Range(1, 10)]
    [SerializeField] private int level;
    [SerializeField] private CardData[] cardDataArray = null;

    public CardData GetRandomCard() {
        int randomCardIndex = Random.Range(0, cardDataArray.Length);
        return cardDataArray[randomCardIndex];
    }

    public CardData[] GetRandomCard(int _cardToDrawNumber) {
        CardData[] cardsPicked = new CardData[_cardToDrawNumber];

        for (int i = 0; i < cardsPicked.Length; i++) {
            cardsPicked[i] = GetRandomCard();
        }

        return cardsPicked;
    }
}
