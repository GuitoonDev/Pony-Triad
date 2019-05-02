using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Action<Card> OnCardAnimationFinished;
    public Action<SelectableArea> OnCardPlayed;

    [SerializeField] private SpriteRenderer selectionBorder = null;

    private Card card;
    public Card Card {
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

                selectionBorder.gameObject.SetActive(IsAreaEmpty);

                OnCardPlayed(this);
            }
        }
    }

    public Vector2Int BoardCoordinates { get; set; }

    public bool IsAreaEmpty {
        get { return card == null; }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (IsAreaEmpty) {
            selectionBorder.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (IsAreaEmpty) {
            selectionBorder.gameObject.SetActive(false);
        }
    }

    private void CardAnimationFinished(Card _cardTarget) {
        if (OnCardAnimationFinished != null) {
            OnCardAnimationFinished(_cardTarget);
        }
    }
}
