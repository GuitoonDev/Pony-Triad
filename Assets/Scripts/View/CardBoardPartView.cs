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
                card.IsOnBoard = true;
                card.Interactable = false;

                card.OnCardAnimationFinished += CardAnimationFinished;

                selectionBorder.gameObject.SetActive(false);

                OnCardPlayed(this);
            }
        }
    }

    public Vector2Int BoardPosition { get; set; }

    public void OnPointerEnter(PointerEventData eventData) {
        if (card == null) {
            selectionBorder.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (card == null) {
            selectionBorder.gameObject.SetActive(false);
        }
    }

    private void CardAnimationFinished(CardView _cardTarget) {
        if (OnCardAnimationFinished != null) {
            OnCardAnimationFinished(this);
        }
    }
}
