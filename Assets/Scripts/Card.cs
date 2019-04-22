using UnityEngine;
using TMPro;
using static CardData;

public class Card : MonoBehaviour
{
    [Header("Power Texts")]
    [SerializeField] private TextMeshPro powerUpText = null;
    [SerializeField] private TextMeshPro powerDownText = null;
    [SerializeField] private TextMeshPro powerLeftText = null;
    [SerializeField] private TextMeshPro powerRightText = null;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer cardImage = null;
    [SerializeField] private SpriteRenderer cardVerso = null;
    [SerializeField] private SpriteRenderer cardBackground = null;

    [Header("Player Colors")]
    [SerializeField] private Color playerOneColor = default(Color);
    [SerializeField] private Color playerTwoColor = default(Color);

    [Header("Card Datas")]
    [SerializeField] private CardData datas;
    public CardData Datas {
        get {
            return datas;
        }
        set {
            if (datas != value) {
                datas = value;
                UpdateView();
            }
        }
    }

    private int playerOwner = 0;

    private void Start() {
        UpdateView();
    }

    private void OnValidate() {
        UpdateView();
    }

    private void Update() {
        bool isVersoSide = transform.rotation.eulerAngles.y > 90 && transform.rotation.eulerAngles.y < 270;
        if (cardVerso.gameObject.activeSelf != isVersoSide) {
            cardVerso.gameObject.SetActive(isVersoSide);
        }
    }

    private void UpdateView() {
        cardBackground.color = playerOwner == 0 ? playerOneColor : playerTwoColor;

        if (datas != null) {
            cardImage.sprite = datas.SpriteImage;

            powerUpText.text = FormatPower(datas.PowerUp);
            powerDownText.text = FormatPower(datas.PowerDown);
            powerLeftText.text = FormatPower(datas.PowerLeft);
            powerRightText.text = FormatPower(datas.PowerRight);
        }
        else {
            Debug.LogWarning("No card datas set to update view", this);
        }
    }

    private string FormatPower(CardPower _power) {
        return _power == CardPower.Ace ? "A" : _power.ToString("d");
    }
}
