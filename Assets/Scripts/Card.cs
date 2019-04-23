using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class Card : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
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
    [SerializeField] private CardDatas datas;
    public CardDatas Datas {
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

    private SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer {
        get {
            if (spriteRenderer == null) {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            return spriteRenderer;
        }
    }

    public PlayerNumber PlayerOwner { get; set; }

    private float zDistanceToCamera = 0;
    private bool isDragged = false;

    private Vector3 beforeDragPosition = Vector3.zero;

    private SelectableArea currentAreaSelected = null;

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

    public void OnPointerDown(PointerEventData eventData) {
        isDragged = true;

        beforeDragPosition = transform.localPosition;
        zDistanceToCamera = Mathf.Abs(beforeDragPosition.z - Camera.main.transform.position.z);

        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistanceToCamera));
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistanceToCamera));

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (RaycastResult raycastItem in raycastResults) {
            SelectableArea hitArea = raycastItem.gameObject.GetComponent<SelectableArea>();
            if (hitArea != null) {
                if (currentAreaSelected != hitArea) {
                    if (currentAreaSelected != null) {
                        currentAreaSelected.OnPointerExit(eventData);
                    }
                    currentAreaSelected = hitArea;
                    currentAreaSelected.OnPointerEnter(eventData);
                }
                break;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        isDragged = false;
        transform.localPosition = beforeDragPosition;

        if (currentAreaSelected != null) {
            currentAreaSelected.Card = this;
            currentAreaSelected = null;
        }
    }

    private void UpdateView() {
        cardBackground.color = PlayerOwner == 0 ? playerOneColor : playerTwoColor;

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
