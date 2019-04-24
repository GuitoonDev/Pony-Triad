using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class Card : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
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

                UpdatePowers();
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

    public bool Interactable { get; set; } = true;

    private PlayerNumber playerOwner = PlayerNumber.None;
    public PlayerNumber PlayerOwner {
        get { return playerOwner; }
        set {
            if (playerOwner != value) {
                // TODO rotation card animation

                playerOwner = value;
                switch (playerOwner) {
                    case PlayerNumber.One:
                        cardBackground.color = playerOneColor;
                        break;
                    case PlayerNumber.Two:
                        cardBackground.color = playerTwoColor;
                        break;
                    default:
                        cardBackground.color = Color.gray;
                        break;
                }
            }
        }
    }

    private Dictionary<CardDirection, CardPower> cardPowersByDirection = new Dictionary<CardDirection, CardPower>();

    private float zDistanceToCamera = 0;

    private Vector3 beforeDragPosition = Vector3.zero;

    private SelectableArea currentAreaSelected = null;

    public CardPower GetPowerByDirection(CardDirection _targetDirection) {
        return cardPowersByDirection[_targetDirection];
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (Interactable) {
            beforeDragPosition = transform.localPosition;
            zDistanceToCamera = Mathf.Abs(beforeDragPosition.z - Camera.main.transform.position.z);

            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistanceToCamera));
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (Interactable) {
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistanceToCamera));

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            SelectableArea hitArea = null;
            foreach (RaycastResult raycastItem in raycastResults) {
                hitArea = raycastItem.gameObject.GetComponent<SelectableArea>();
                if (hitArea != null) {
                    break;
                }
            }

            if (currentAreaSelected != hitArea) {
                if (currentAreaSelected != null) {
                    currentAreaSelected.OnPointerExit(eventData);
                }

                currentAreaSelected = hitArea;

                if (currentAreaSelected != null) {
                    currentAreaSelected.OnPointerEnter(eventData);
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (Interactable) {
            transform.localPosition = beforeDragPosition;

            if (currentAreaSelected != null) {
                currentAreaSelected.Card = this;
                currentAreaSelected = null;
            }
        }
    }

    private void OnValidate() {
        UpdateView();
    }

    private void Start() {
        UpdatePowers();
        UpdateView();
    }

    private void Update() {
        bool isVersoSide = transform.rotation.eulerAngles.y > 90 && transform.rotation.eulerAngles.y < 270;
        if (cardVerso.gameObject.activeSelf != isVersoSide) {
            cardVerso.gameObject.SetActive(isVersoSide);
        }
    }

    public bool IsLooseBattle(CardDirection _targetDirection, CardPower _powerToCompare, PlayerNumber _opponentPlayer) {
        bool isPlayerOwnerChanged = (playerOwner != _opponentPlayer && cardPowersByDirection[_targetDirection] < _powerToCompare);
        if (isPlayerOwnerChanged) {
            Debug.LogWarning("This card change his player's owner");
            PlayerOwner = _opponentPlayer;
        }

        return isPlayerOwnerChanged;
    }

    private void UpdatePowers() {
        if (datas != null) {
            cardPowersByDirection[CardDirection.Up] = datas.PowerUp;
            cardPowersByDirection[CardDirection.Down] = datas.PowerDown;
            cardPowersByDirection[CardDirection.Left] = datas.PowerLeft;
            cardPowersByDirection[CardDirection.Right] = datas.PowerRight;
        }
    }

    private void UpdateView() {
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
