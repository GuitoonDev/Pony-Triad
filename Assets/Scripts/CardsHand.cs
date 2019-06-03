using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using Audio;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(SortingGroup))]
public class CardsHand : MonoBehaviour
{
    public UnityAction<CardsHand> OnHandReady;

    [SerializeField] private PlayerNumber playerId = PlayerNumber.None;
    [SerializeField] private Card cardPrefab = null;
    [SerializeField] private TextMeshPro playerScoreText = null;
    [SerializeField] private SpriteRenderer currentTurnArrow = null;

    [Header("Player Colors")]
    [SerializeField] private PlayersColorsList playersColorsList = null;

    [Header("Sounds")]
    [SerializeField] private AudioClip drawCardSound = null;

    private List<Card> cardList = null;

    private int cardsDrawFinishedCount = 0;

    private SortingGroup sortingGroup;
    public SortingGroup SortingGroup {
        get {
            if (sortingGroup == null) {
                sortingGroup = GetComponent<SortingGroup>();
            }

            return sortingGroup;
        }
    }

    private int currentPlayerScore = 0;
    public int CurrentPlayerScore {
        get { return currentPlayerScore; }
        set {
            currentPlayerScore = value;
            playerScoreText.text = currentPlayerScore.ToString();
        }
    }

    public bool Ready { get; private set; }

    private void Start() {
        VertexGradient newColorGradient = playerScoreText.colorGradient;
        newColorGradient.bottomLeft = newColorGradient.bottomRight = playersColorsList.GetColorByPlayer(playerId);
        playerScoreText.colorGradient = newColorGradient;
    }

    public void Init(CardDatas[] _cardDatasList, bool _enabled) {
        _cardDatasList.Shuffle();
        cardList = new List<Card>();

        CurrentPlayerScore = _cardDatasList.Length;

        for (int cardIndex = 0; cardIndex < _cardDatasList.Length; cardIndex++) {
            Card newCard = Instantiate(cardPrefab, transform);
            newCard.Datas = _cardDatasList[cardIndex];
            newCard.PlayerOwner = playerId;
            newCard.Interactable = _enabled;

            if (!GameManager.Instance.HasRuleSet(GameRule.Open)) {
                newCard.Hidden = true;
            }

            float endPosition = cardIndex * (-newCard.SpriteRenderer.bounds.size.y * 0.525f);
            newCard.transform.localPosition = new Vector3(0, endPosition + 10, (_cardDatasList.Length - cardIndex) * 0.001f);
            newCard.transform.DOLocalMoveY(endPosition, 0.35f, false)
                .SetDelay((_cardDatasList.Length - cardIndex) * 0.095f)
                .OnStart(() => {
                    AudioManager.Instance.PlaySound(drawCardSound);
                })
                .OnComplete(CardAnimationFinished);

            cardList.Add(newCard);
        }
    }

    public void Enable(bool _enabled) {
        foreach (Card cardItem in cardList) {
            cardItem.Interactable = _enabled;

            if (!GameManager.Instance.HasRuleSet(GameRule.Open)) {
                cardItem.Hidden = true;
            }
        }

        currentTurnArrow.gameObject.SetActive(_enabled);

        SortingGroup.sortingOrder = _enabled ? 2 : 1;
    }

    public void RemoveCard(Card _card) {
        cardList.Remove(_card);
    }

    #region Animation Event Functions
    private void CardAnimationFinished() {
        cardsDrawFinishedCount++;

        if (cardsDrawFinishedCount >= cardList.Count && OnHandReady != null) {
            Ready = true;
            OnHandReady(this);
        }
    }
    #endregion
}
