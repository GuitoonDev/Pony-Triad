using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RuleDescriptionPanel : MonoBehaviour
{
    [SerializeField] private Image backgroundPanel = null;
    [SerializeField] private TextMeshProUGUI titleText = null;

    public void Show(RuleBarItem _targetRuleBarItem, string _titleText) {
        backgroundPanel.gameObject.SetActive(true);
        titleText.text = _titleText;

        transform.position = _targetRuleBarItem.transform.position;

        // StartCoroutine(UpdatePosition(_targetRuleBarItem));
    }

    public void Hide() {
        backgroundPanel.gameObject.SetActive(false);
    }

    private IEnumerator UpdatePosition(RuleBarItem _targetRuleBarItem) {
        yield return new WaitForEndOfFrame();

        Rect transformRect = GetComponent<RectTransform>().rect;

        Vector2 newPosition = _targetRuleBarItem.transform.position;
        newPosition.x -= transformRect.width;
        transform.position = newPosition;

        backgroundPanel.gameObject.SetActive(true);
    }
}
