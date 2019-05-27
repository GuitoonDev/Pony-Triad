using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using Audio;

[RequireComponent(typeof(SortingGroup))]
public class CardsHand : MonoBehaviour
{
    public Action OnHandReady;

    [SerializeField] private PlayerNumber playerId = PlayerNumber.None;
    [SerializeField] private Card cardPrefab = null;
    [SerializeField] private SpriteRenderer currentTurnArrow = null;

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

    public void Init(CardDatas[] _cardDatasList, bool _enabled) {
        cardList = new List<Card>();

        for (int cardAnimationsFinishedCount = 0; cardAnimationsFinishedCount < _cardDatasList.Length; cardAnimationsFinishedCount++) {
            Card newCard = Instantiate(cardPrefab, transform);
            newCard.Datas = _cardDatasList[cardAnimationsFinishedCount];
            newCard.PlayerOwner = playerId;
            newCard.Interactable = _enabled;

            if (!GameManager.Instance.HasRuleSet(GameRules.Open)) {
                newCard.Hidden = true;
            }

            float endPosition = cardAnimationsFinishedCount * (-newCard.SpriteRenderer.bounds.size.y * 0.525f);
            newCard.transform.localPosition = new Vector3(0, endPosition + 10, (_cardDatasList.Length - cardAnimationsFinishedCount) * 0.001f);
            newCard.transform.DOLocalMoveY(endPosition, 0.35f, false)
                .SetDelay((_cardDatasList.Length - cardAnimationsFinishedCount) * 0.095f)
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

            if (!GameManager.Instance.HasRuleSet(GameRules.Open)) {
                cardItem.Hidden = true;
            }
        }

        currentTurnArrow.gameObject.SetActive(_enabled);

        SortingGroup.sortingOrder = _enabled ? 2 : 1;
    }

    public void RemoveCard(Card _card) {
        cardList.Remove(_card);
    }

    private void CardAnimationFinished() {
        cardsDrawFinishedCount++;

        if (cardsDrawFinishedCount >= cardList.Count && OnHandReady != null) {
            OnHandReady();
        }
    }
}
