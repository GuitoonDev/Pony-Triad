using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Audio;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public UnityAction<Card> OnCardAnimationFinished;

    [Header("Power Texts")]
    [SerializeField] private TextMeshPro powerUpText = null;
    [SerializeField] private TextMeshPro powerDownText = null;
    [SerializeField] private TextMeshPro powerLeftText = null;
    [SerializeField] private TextMeshPro powerRightText = null;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer cardImage = null;
    [SerializeField] private SpriteRenderer cardBackground = null;

    [Header("Player Colors")]
    [SerializeField] private PlayersColorsList playersColorsList = null;

    [Header("Sounds")]
    [SerializeField] private AudioClip selectCardSound = null;
    [SerializeField] private AudioClip turnCardSound = null;

    [Header("Card Datas")]
    [SerializeField] private CardData data;

    public bool Interactable { get; set; }

    public CardData Data {
        get {
            return data;
        }
        set {
            if (data != value) {
                data = value;

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

    public bool Hidden {
        set {
            Animator.SetBool("Hidden", !Interactable);
        }
    }


    private PlayerNumber playerOwner = PlayerNumber.None;
    public PlayerNumber PlayerOwner {
        get { return playerOwner; }
        set {
            if (playerOwner != value) {
                playerOwner = value;
                newPlayerOwner = playerOwner;

                cardBackground.color = playersColorsList.GetColorByPlayer(playerOwner);
            }
        }
    }

    private Animator animator;
    private Animator Animator {
        get {
            if (animator == null) {
                animator = GetComponent<Animator>();
            }

            return animator;
        }
    }
    private int shineTriggerId;
    private int overPlayerIntId;

    private PlayerNumber newPlayerOwner = PlayerNumber.None;

    private Dictionary<CardDirection, CardPower> cardPowersByDirection = new Dictionary<CardDirection, CardPower>();

    private float zDistanceToCamera = 0;

    private Vector3 beforeDragPosition = Vector3.zero;

    private CardBoardArea currentAreaSelected = null;

    public CardPower GetPowerByDirection(CardDirection _targetDirection) {
        return cardPowersByDirection[_targetDirection];
    }

    private void Start() {
        shineTriggerId = Animator.StringToHash("Shine");
        overPlayerIntId = Animator.StringToHash("OverPlayer");

        UpdatePowers();
        UpdateView();
    }

    private void OnValidate() {
        UpdateView();
    }

    public void StartShinyAnimation() {
        Animator.SetTrigger(shineTriggerId);
    }

    public void ChangePlayerOwner(CardDirection _targetDirection, PlayerNumber _newPlayerOwner) {
        if (newPlayerOwner != _newPlayerOwner) {
            newPlayerOwner = _newPlayerOwner;
            StartRotationAnimation(_targetDirection);

            AudioManager.Instance.PlaySound(turnCardSound);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (Interactable) {
            Animator.SetInteger(overPlayerIntId, (int) PlayerOwner);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (Interactable) {
            Animator.SetInteger(overPlayerIntId, (int) PlayerNumber.None);
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (Interactable) {
            Animator.SetInteger(overPlayerIntId, (int) PlayerNumber.None);

            beforeDragPosition = transform.localPosition;
            zDistanceToCamera = Mathf.Abs(beforeDragPosition.z - Camera.main.transform.position.z);

            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistanceToCamera));

            AudioManager.Instance.PlaySound(selectCardSound);
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (Interactable) {
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistanceToCamera));

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            CardBoardArea hitArea = null;
            foreach (RaycastResult raycastItem in raycastResults) {
                hitArea = raycastItem.gameObject.GetComponent<CardBoardArea>();
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
            Animator.SetInteger(overPlayerIntId, (int) PlayerNumber.None);

            transform.localPosition = beforeDragPosition;

            if (currentAreaSelected != null) {
                currentAreaSelected.Card = this;
                currentAreaSelected = null;
            }

            AudioManager.Instance.PlaySound(selectCardSound);
        }
    }

    private void StartRotationAnimation(CardDirection _targetDirection) {
        string formattedRotationTrigger = string.Format("Rotate{0}", _targetDirection.ToString());
        Animator.SetTrigger(formattedRotationTrigger);
    }

    private void UpdatePowers() {
        if (data != null) {
            cardPowersByDirection[CardDirection.Up] = data.PowerUp;
            cardPowersByDirection[CardDirection.Down] = data.PowerDown;
            cardPowersByDirection[CardDirection.Left] = data.PowerLeft;
            cardPowersByDirection[CardDirection.Right] = data.PowerRight;
        }
    }

    private void UpdateView() {
        if (data != null) {
            cardImage.sprite = data.SpriteImage;

            powerUpText.text = FormatPower(data.PowerUp);
            powerDownText.text = FormatPower(data.PowerDown);
            powerLeftText.text = FormatPower(data.PowerLeft);
            powerRightText.text = FormatPower(data.PowerRight);
        }
        else {
            Debug.LogWarning("No card datas set to update view", this);
        }
    }

    private string FormatPower(CardPower _power) {
        return _power == CardPower.Ace ? "A" : _power.ToString("d");
    }

    #region Animation Event Functions
    private void UpdatePlayerOwner() {
        PlayerOwner = newPlayerOwner;
    }

    private void RotationAnimationFinished() {
        if (OnCardAnimationFinished != null) {
            OnCardAnimationFinished(this);
        }
    }
    #endregion
}
