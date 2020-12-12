using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using PonyTriad.Model;
using PonyTriad.Audio;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public UnityAction<CardView> OnCardAnimationFinished;

    [Header("Power Texts")]
    [SerializeField] private TextMeshPro powerUpText = null;
    [SerializeField] private TextMeshPro powerDownText = null;
    [SerializeField] private TextMeshPro powerLeftText = null;
    [SerializeField] private TextMeshPro powerRightText = null;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer cardImage = null;
    [SerializeField] private SpriteRenderer cardBackground = null;

    [Header("Player Colors")]
    [SerializeField] private PlayersColorsData playersColorsList = null;

    [Header("Sounds")]
    [SerializeField] private AudioClip selectCardSound = null;
    [SerializeField] private AudioClip turnCardSound = null;
    [SerializeField] private AudioClip specialCardSound = null;

    private SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer {
        get {
            if (spriteRenderer == null) {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            return spriteRenderer;
        }
    }

    private bool interactable;
    public bool Interactable {
        get { return interactable; }
        set {
            interactable = value;
            Animator.SetBool("Hidden", !interactable && !isOpen && !IsOnBoard);
        }
    }

    private PlayerNumber playerOwner = PlayerNumber.None;
    public PlayerNumber PlayerOwner {
        get { return playerOwner; }
        private set {
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

    public Card Model { get; private set; }
    public bool IsOnBoard { get; set; }

    private int shineTriggerId;
    private int overPlayerIntId;
    private bool isOpen;

    private PlayerNumber newPlayerOwner = PlayerNumber.None;

    private Dictionary<CardDirection, CardPower> cardPowersByDirection = new Dictionary<CardDirection, CardPower>();

    private float zDistanceToCamera = 0;

    private Vector3 beforeDragPosition = Vector3.zero;

    private CardBoardPartView currentAreaSelected = null;

    public CardPower GetPowerByDirection(CardDirection _targetDirection) {
        return cardPowersByDirection[_targetDirection];
    }

    private void Start() {
        shineTriggerId = Animator.StringToHash("Shine");
        overPlayerIntId = Animator.StringToHash("OverPlayer");
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

            CardBoardPartView hitArea = null;
            foreach (RaycastResult raycastItem in raycastResults) {
                hitArea = raycastItem.gameObject.GetComponent<CardBoardPartView>();
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

    public void Init(Card _cardModel, bool _isOpenRuleActive) {
        Model = _cardModel;

        isOpen = _isOpenRuleActive;

        cardImage.sprite = _cardModel.Sprite;

        PlayerOwner = _cardModel.PlayerOwner;

        powerUpText.text = FormatPower(_cardModel.PowerUp);
        powerDownText.text = FormatPower(_cardModel.PowerDown);
        powerLeftText.text = FormatPower(_cardModel.PowerLeft);
        powerRightText.text = FormatPower(_cardModel.PowerRight);
    }

    public void StartShinyAnimation() {
        AudioManager.Instance.PlaySound(specialCardSound);
        Animator.SetTrigger(shineTriggerId);
    }

    public void ChangePlayerOwner(CardDirection _targetDirection, PlayerNumber _newPlayerOwner) {
        if (newPlayerOwner != _newPlayerOwner) {
            newPlayerOwner = _newPlayerOwner;

            string formattedRotationTrigger = string.Format("Rotate{0}", _targetDirection.ToString());
            Animator.SetTrigger(formattedRotationTrigger);

            AudioManager.Instance.PlaySound(turnCardSound);
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
