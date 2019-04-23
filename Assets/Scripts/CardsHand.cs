using UnityEngine;

public class CardsHand : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;

    public void Init(CardDatas[] _cardDatasList) {
        for (int i = 0; i < _cardDatasList.Length; i++) {
            Card newCard = Instantiate(cardPrefab, transform);
            newCard.Datas = _cardDatasList[i];
            newCard.transform.localPosition = new Vector2(0, i * (-newCard.SpriteRenderer.bounds.size.y * 0.5f));
        }
    }
}
