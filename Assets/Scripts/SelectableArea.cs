using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SpriteRenderer selectionBorder = null;

    private Card card;
    public Card Card {
        get {
            return card;
        }
        set {
            card = value;

            selectionBorder.gameObject.SetActive(IsAreaEmpty);
        }
    }

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
}
