using UnityEngine;

[CreateAssetMenu(fileName = "CardList", menuName = "ProtoTriad/CardList")]
public class CardList : ScriptableObject
{
    [SerializeField]
    private CardDatas[] cardDatasList = null;

    public CardDatas GetRandomCard() {
        int randomCardIndex = Random.Range(0, cardDatasList.Length);
        return cardDatasList[randomCardIndex];
    }

    public CardDatas[] GetRandomCard(int _cardNumber) {
        CardDatas[] cardsPicked = new CardDatas[_cardNumber];

        for (int i = 0; i < cardsPicked.Length; i++) {
            int randomCardIndex = Random.Range(0, cardDatasList.Length);
            cardsPicked[i] = cardDatasList[randomCardIndex];
        }

        return cardsPicked;
    }
}
