using UnityEngine;

[CreateAssetMenu(fileName = "CardList", menuName = "ProtoTriad/CardList")]
public class CardList : ScriptableObject
{
    [SerializeField]
    private CardDatas[] cardDatasArray = null;

    public CardDatas GetRandomCard() {
        int randomCardIndex = Random.Range(0, cardDatasArray.Length);
        return cardDatasArray[randomCardIndex];
    }

    public CardDatas[] GetRandomCard(int _cardToDrawNumber) {
        CardDatas[] cardsPicked = new CardDatas[_cardToDrawNumber];

        for (int i = 0; i < cardsPicked.Length; i++) {
            int randomCardIndex = Random.Range(0, cardDatasArray.Length);
            cardsPicked[i] = cardDatasArray[randomCardIndex];
        }

        return cardsPicked;
    }
}
