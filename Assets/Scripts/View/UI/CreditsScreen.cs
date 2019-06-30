using UnityEngine;
using UnityEngine.UI;

public class CreditsScreen : MonoBehaviour
{
    [SerializeField] private Image panel = null;

    public void Show() {
        panel.gameObject.SetActive(true);
    }

    public void Hide() {
        panel.gameObject.SetActive(false);
    }
}
