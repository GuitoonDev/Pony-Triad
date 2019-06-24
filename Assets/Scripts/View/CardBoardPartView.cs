using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CardBoardPartView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityAction<CardBoardPartView> OnCardAnimationFinished;
    public UnityAction<CardBoardPartView> OnCardPlayed;

    [SerializeField] private SpriteRenderer selectionBorder = null;

    private CardView card;
    public CardView Card {
        get {
            return card;
        }
        set {
            if (card == null) {
                card = value;
                card.transform.SetParent(transform);
                card.transform.localPosition = Vector3.zero;
                card.Interactable = false;

                card.OnCardAnimationFinished += CardAnimationFinished;

                selectionBorder.gameObject.SetActive(IsEmpty);

                OnCardPlayed(this);
            }
        }
    }

    public Vector2Int BoardCoordinates { get; set; }

    public bool IsEmpty {
        get { return card == null; }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (IsEmpty) {
            selectionBorder.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (IsEmpty) {
            selectionBorder.gameObject.SetActive(false);
        }
    }

    private void CardAnimationFinished(CardView _cardTarget) {
        if (OnCardAnimationFinished != null) {
            OnCardAnimationFinished(this);
        }
    }
}
