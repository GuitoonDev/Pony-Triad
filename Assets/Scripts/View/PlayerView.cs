using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;
using PonyTriad.Model;
using PonyTriad.Audio;

[RequireComponent(typeof(SortingGroup))]
public class PlayerView : MonoBehaviour
{
    public UnityAction<PlayerView> OnHandReady;

    [SerializeField] private CardView cardPrefab = null;
    [SerializeField] private TextMeshPro playerScoreText = null;
    [SerializeField] private SpriteRenderer currentTurnArrow = null;

    [Header("Player Colors")]
    [SerializeField] private PlayersColorsData playersColorsList = null;

    [Header("Sounds")]
    [SerializeField] private AudioClip drawCardSound = null;

    private List<CardView> cardHandView = null;

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

    private int currentPlayerScore;
    public int CurrentPlayerScore {
        get { return currentPlayerScore; }
        set {
            currentPlayerScore = value;
            playerScoreText.text = currentPlayerScore.ToString();
        }
    }

    public bool Ready { get; private set; }

    public void Init(Player _playerModel, bool _enabled) {
        VertexGradient newColorGradient = playerScoreText.colorGradient;
        newColorGradient.bottomLeft = newColorGradient.bottomRight = playersColorsList.GetColorByPlayer(_playerModel.Number);
        playerScoreText.colorGradient = newColorGradient;

        CurrentPlayerScore = _playerModel.Score;

        cardHandView = new List<CardView>();
        for (int cardIndex = 0; cardIndex < _playerModel.CardHand.Count; cardIndex++) {
            CardView newCard = Instantiate(cardPrefab, transform);

            bool isAllOpenRuleActive = Game.activeRules.HasFlag(GameRule.AllOpen);
            bool isThreeOpenRuleActive = Game.activeRules.HasFlag(GameRule.ThreeOpen);
            bool isOpenCard = (isAllOpenRuleActive || (isThreeOpenRuleActive && cardIndex < 3));
            newCard.Init(_playerModel.CardHand[cardIndex], isOpenCard);

            float endPosition = cardIndex * (-newCard.SpriteRenderer.bounds.size.y * 0.525f);
            newCard.transform.localPosition = new Vector3(0, endPosition + 10, (_playerModel.CardHand.Count - cardIndex) * 0.001f);
            newCard.transform.DOLocalMoveY(endPosition, 0.35f, false)
                .SetDelay((_playerModel.CardHand.Count - cardIndex) * 0.095f)
                .OnStart(() => {
                    AudioManager.Instance.PlaySound(drawCardSound);
                })
                .OnComplete(CardAnimationFinished);

            cardHandView.Add(newCard);
        }
    }

    public void Enable(bool _enabled) {
        foreach (CardView cardItem in cardHandView) {
            cardItem.Interactable = _enabled;
        }

        currentTurnArrow.gameObject.SetActive(_enabled);

        SortingGroup.sortingOrder = _enabled ? 2 : 1;
    }

    public void RemoveCard(CardView _card) {
        cardHandView.Remove(_card);
    }

    private void UpdateView() {
    }

    #region Animation Event Functions
    private void CardAnimationFinished() {
        cardsDrawFinishedCount++;

        bool isAllCardAnimationFinished = (cardsDrawFinishedCount >= cardHandView.Count);
        if (isAllCardAnimationFinished) {
            Ready = true;
            if (OnHandReady != null) {
                OnHandReady(this);
            }
        }
    }
    #endregion
}
