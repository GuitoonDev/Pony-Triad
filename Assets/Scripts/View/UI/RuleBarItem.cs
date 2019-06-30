using UnityEngine;
using UnityEngine.EventSystems;

public class RuleBarItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string title = null;

    public RuleDescriptionPanel RuleDescriptionPanel { get; set; }

    public void OnPointerEnter(PointerEventData eventData) {
        RuleDescriptionPanel.Show(this, title);
    }

    public void OnPointerExit(PointerEventData eventData) {
        RuleDescriptionPanel.Hide();
    }
}
