using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
public class CardsHand : MonoBehaviour
{
    [SerializeField] private PlayerNumber playerId = PlayerNumber.None;
    [SerializeField] private Card cardPrefab = null;

    private SortingGroup sortingGroup;
    public SortingGroup SortingGroup {
        get {
            if (sortingGroup == null) {
                sortingGroup = GetComponent<SortingGroup>();
            }

            return sortingGroup;
        }
    }

    public void Init(CardDatas[] _cardDatasList) {
        for (int i = 0; i < _cardDatasList.Length; i++) {
            Card newCard = Instantiate(cardPrefab, transform);
            newCard.Datas = _cardDatasList[i];
            newCard.PlayerOwner = playerId;
            newCard.transform.localPosition = new Vector3(0, i * (-newCard.SpriteRenderer.bounds.size.y * 0.5f), _cardDatasList.Length - i);
        }
    }
}
