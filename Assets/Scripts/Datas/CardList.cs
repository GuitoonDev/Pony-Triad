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
}
