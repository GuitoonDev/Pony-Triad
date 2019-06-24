using System.Collections.Generic;
using UnityEngine;
using Audio;
using AnimatorStateMachineLibrary;

[RequireComponent(typeof(Animator))]
public partial class GameController : MonoBehaviour
{
    private int nextStateTriggerId;
    private int gameOverTriggerId;

    private Animator animatorFSM;
    private Animator AnimatorFSM {
        get {
            if (animatorFSM == null) {
                animatorFSM = GetComponent<Animator>();
            }
            return animatorFSM;
        }
    }

    private int handsReadyCount = 0;
    private int cardPlayedCount = 0;
    private int cardsRotateCount = 0;
    private int cardsRotationFinishedCount = 0;
    private int cardsWonCount = 0;

    private bool isComboEnabled;

    private void Start() {
        nextStateTriggerId = Animator.StringToHash("NextState");
        gameOverTriggerId = Animator.StringToHash("GameOver");
    }

    [StateEnterMethod("Base Layer.Intro")]
    private void IntroState() {
        Debug.Log("GameManager::IntroState");

        int[] randomCardLevelArray = new int[5];
        for (int i = 0; i < randomCardLevelArray.Length; i++) {
            randomCardLevelArray[i] = Random.Range(0, cardsListArray.Length - 1);
        }

        CardDefinition[] playerCards = new CardDefinition[cardsPerPlayer];
        for (int i = 0; i < playerCards.Length; i++) {
            playerCards[i] = cardsListArray[randomCardLevelArray[i]].GetRandomCard();
        }
        playerOneCardsHand.Init(playerCards, false);
        playerOneCardsHand.OnHandReady += PlayerHandReady;

        playerCards = new CardDefinition[cardsPerPlayer];
        for (int i = 0; i < playerCards.Length; i++) {
            playerCards[i] = cardsListArray[randomCardLevelArray[i]].GetRandomCard();
        }
        playerTwoCardsHand.Init(playerCards, false);
        playerTwoCardsHand.OnHandReady += PlayerHandReady;

        cardHandByPlayer[PlayerNumber.One] = playerOneCardsHand;
        cardHandByPlayer[PlayerNumber.Two] = playerTwoCardsHand;

        for (int positionX = 0; positionX < selectableAreasList.GetLength(0); positionX++) {
            for (int positionY = 0; positionY < selectableAreasList.GetLength(1); positionY++) {
                CardBoardPartView newSelectableArea = verticalListableAreasList[positionX][positionY];
                newSelectableArea.OnCardPlayed += CardPlayed;
                newSelectableArea.OnCardAnimationFinished += CardAnimationFinished;
                newSelectableArea.BoardCoordinates = new Vector2Int(positionX, positionY);
                selectableAreasList[positionX, positionY] = newSelectableArea;
            }
        }

        currentPlayer = Random.Range(0, 2) < 1 ? PlayerNumber.One : PlayerNumber.Two;

        AudioManager.Instance.PlayGameMusic();
    }

    private void PlayerHandReady(PlayerView _cardHandReady) {
        _cardHandReady.OnHandReady -= PlayerHandReady;
        handsReadyCount++;

        if (handsReadyCount == cardHandByPlayer.Count) {
            randomArrow.OnAnimationComplete += BeginGame;
            switch (currentPlayer) {
                case PlayerNumber.One:
                    randomArrow.StartAnimation(PlayerNumber.Two);
                    break;

                case PlayerNumber.Two:
                    randomArrow.StartAnimation(PlayerNumber.One);
                    break;
            }
        }
    }

    private void BeginGame() {
        randomArrow.OnAnimationComplete -= BeginGame;
        AnimatorFSM.SetTrigger(nextStateTriggerId);
    }

    [StateEnterMethod("Base Layer.PickCard")]
    private void PickCardState() {
        Debug.Log("GameManager::PickCardState");

        switch (currentPlayer) {
            case PlayerNumber.One:
                CurrentPlayer = PlayerNumber.Two;
                break;

            case PlayerNumber.Two:
                CurrentPlayer = PlayerNumber.One;
                break;
        }
    }

    private void CardPlayed(CardBoardPartView _playedCardArea) {
        cardPlayedCount++;

        cardHandByPlayer[CurrentPlayer].RemoveCard(_playedCardArea.Card);
        cardHandByPlayer[CurrentPlayer].Enable(false);

        playedCardBoardArea = _playedCardArea;
        playedCardOpponentCardAreasByDirection = GetCardAreasAround(_playedCardArea);

        AnimatorFSM.SetTrigger(nextStateTriggerId);
    }


    [StateEnterMethod("Base Layer.SameRule")]
    private void SameRuleState() {
        Debug.Log("GameManager::SameRuleState");

        isComboEnabled = false;
        cardsWonList.Clear();
        if (activeGameRules.HasFlag(GameRule.Same)) {
            foreach (KeyValuePair<CardDirection, CardBoardPartView> opponentCardBoardAreas in playedCardOpponentCardAreasByDirection) {
                CardDirection opponentCardOppositeDirection = GetOppositeDirection(opponentCardBoardAreas.Key);

                bool isSamePower = (playedCardBoardArea.Card.GetPowerByDirection(opponentCardBoardAreas.Key) == opponentCardBoardAreas.Value.Card.GetPowerByDirection(opponentCardOppositeDirection));
                if (isSamePower) {
                    cardsWonList.Add(new CardBoardAreaWon(opponentCardBoardAreas.Value, opponentCardOppositeDirection));
                }
            }

            if (cardsWonList.Count > 1) {
                isComboEnabled = true;

                sameRuleCardsWonList.AddRange(cardsWonList);

                foreach (CardBoardAreaWon cardWonItem in cardsWonList) {
                    cardWonItem.cardBoardArea.Card.StartShinyAnimation();
                }

                SpecialRuleText sameRuleText = Instantiate(sameRuleTextPrefab, uiCanvas.transform);
                sameRuleText.OnAnimationFinished += ProcessWonCardsList;
            }
            else {
                AnimatorFSM.SetTrigger(nextStateTriggerId);
            }
        }
        else {
            AnimatorFSM.SetTrigger(nextStateTriggerId);
        }
    }


    [StateEnterMethod("Base Layer.PlusRule")]
    private void PlusRuleState() {
        Debug.Log("GameManager::PlusRuleState");

        isComboEnabled = false;
        cardsWonList.Clear();

        if (activeGameRules.HasFlag(GameRule.Same)) {
            Dictionary<int, List<CardBoardAreaWon>> cardWonByAddPowers = new Dictionary<int, List<CardBoardAreaWon>>();

            foreach (KeyValuePair<CardDirection, CardBoardPartView> opponentCardBoardAreas in playedCardOpponentCardAreasByDirection) {
                CardDirection opponentCardOppositeDirection = GetOppositeDirection(opponentCardBoardAreas.Key);

                int powerAdd = ((int) playedCardBoardArea.Card.GetPowerByDirection(opponentCardBoardAreas.Key)) + ((int) opponentCardBoardAreas.Value.Card.GetPowerByDirection(opponentCardOppositeDirection));

                List<CardBoardAreaWon> powerAddCardWonList;
                if (!cardWonByAddPowers.TryGetValue(powerAdd, out powerAddCardWonList)) {
                    powerAddCardWonList = new List<CardBoardAreaWon>();
                    cardWonByAddPowers[powerAdd] = powerAddCardWonList;
                }

                powerAddCardWonList.Add(new CardBoardAreaWon(opponentCardBoardAreas.Value, opponentCardOppositeDirection));
            }

            foreach (List<CardBoardAreaWon> cardWonByAddPowerItem in cardWonByAddPowers.Values) {
                if (cardWonByAddPowerItem.Count > 1) {
                    cardsWonList.AddRange(cardWonByAddPowerItem);
                }
            }

            bool isEqualSameWonCards = false;
            if (cardsWonList.Count != 0 && sameRuleCardsWonList.Count == cardsWonList.Count) {
                isEqualSameWonCards = true;
                foreach (CardBoardAreaWon cardWonItem in cardsWonList) {
                    if (!sameRuleCardsWonList.Contains(cardWonItem)) {
                        isEqualSameWonCards = false;
                        break;
                    }
                }
            }

            if (!isEqualSameWonCards && cardsWonList.Count > 1) {
                isComboEnabled = true;

                foreach (CardBoardAreaWon cardWonItem in cardsWonList) {
                    cardWonItem.cardBoardArea.Card.StartShinyAnimation();
                }

                SpecialRuleText plusRuleText = Instantiate(plusRuleTextPrefab, uiCanvas.transform);
                plusRuleText.OnAnimationFinished += ProcessWonCardsList;
            }
            else {
                AnimatorFSM.SetTrigger(nextStateTriggerId);
            }
        }
        else {
            AnimatorFSM.SetTrigger(nextStateTriggerId);
        }

        sameRuleCardsWonList.Clear();
    }

    [StateEnterMethod("Base Layer.Fight")]
    private void FightState() {
        Debug.Log("GameManager::FightState");

        isComboEnabled = false;
        cardsWonList.Clear();

        List<CardBoardAreaWon> cardsWonInFight = CardFight(playedCardBoardArea);

        if (cardsWonInFight.Count > 0) {
            cardsWonList = cardsWonInFight;
            ProcessWonCardsList();
        }
        else {
            AnimatorFSM.SetTrigger(nextStateTriggerId);
        }
    }

    private void ProcessComboFightPhase() {
        isComboEnabled = false;
        List<CardBoardAreaWon> comboCardsList = new List<CardBoardAreaWon>(cardsWonList);
        cardsWonList.Clear();

        List<CardBoardAreaWon> cardsWonInFight = new List<CardBoardAreaWon>();
        foreach (CardBoardAreaWon comboCardItem in comboCardsList) {
            cardsWonInFight.AddRange(CardFight(comboCardItem.cardBoardArea));
        }

        if (cardsWonInFight.Count > 0) {
            isComboEnabled = true;

            cardsWonList = cardsWonInFight;
            SpecialRuleText comboRuleText = Instantiate(comboRuleTextPrefab, uiCanvas.transform);
            ProcessWonCardsList();
        }
        else {
            AnimatorFSM.SetTrigger(nextStateTriggerId);
        }
    }

    private List<CardBoardAreaWon> CardFight(CardBoardPartView _targetCardBoardArea) {
        List<CardBoardAreaWon> cardsWonInFight = new List<CardBoardAreaWon>();

        Dictionary<CardDirection, CardBoardPartView> opponentCardBoardAreas = GetCardAreasAround(_targetCardBoardArea);
        foreach (KeyValuePair<CardDirection, CardBoardPartView> opponentCardBoardAreaItem in opponentCardBoardAreas) {
            CardDirection opponentCardOppositeDirection = GetOppositeDirection(opponentCardBoardAreaItem.Key);

            int powerDiff = ((int) _targetCardBoardArea.Card.GetPowerByDirection(opponentCardBoardAreaItem.Key)) - ((int) opponentCardBoardAreaItem.Value.Card.GetPowerByDirection(opponentCardOppositeDirection));

            if (opponentCardBoardAreaItem.Value.Card.PlayerOwner != _targetCardBoardArea.Card.PlayerOwner) {
                bool isTargetCardWon;
                if (activeGameRules.HasFlag(GameRule.Reversed)) {
                    isTargetCardWon = powerDiff < 0;
                }
                else {
                    isTargetCardWon = powerDiff > 0;
                }

                if (isTargetCardWon) {
                    cardsWonInFight.Add(new CardBoardAreaWon(opponentCardBoardAreaItem.Value, opponentCardOppositeDirection));
                }
            }
        }

        return cardsWonInFight;
    }

    private void ProcessWonCardsList() {
        cardsRotationFinishedCount = 0;
        foreach (CardBoardAreaWon cardWonItem in cardsWonList) {
            if (cardWonItem.cardBoardArea.Card.PlayerOwner != playedCardBoardArea.Card.PlayerOwner) {
                Debug.LogFormat("Card won : " + cardWonItem.cardBoardArea.name + "," + cardWonItem.cardBoardArea.transform.parent.name);
                cardWonItem.cardBoardArea.Card.ChangePlayerOwner(cardWonItem.direction, CurrentPlayer);
                cardsRotateCount++;
                cardsWonCount++;
            }
        }
    }

    private void CardAnimationFinished(CardBoardPartView _cardTarget) {
        cardsRotationFinishedCount++;

        if (cardsRotationFinishedCount == cardsRotateCount) {
            cardsRotationFinishedCount = 0;
            cardsRotateCount = 0;

            if (isComboEnabled) {
                ProcessComboFightPhase();
            }
            else {
                AnimatorFSM.SetTrigger(nextStateTriggerId);
            }
        }
    }

    [StateEnterMethod("Base Layer.BetweenTurns")]
    private void BetweenTurnState() {
        Debug.Log("GameManager::BetweenTurnState");

        isComboEnabled = false;
        playedCardOpponentCardAreasByDirection.Clear();

        switch (CurrentPlayer) {
            case PlayerNumber.One:
                cardHandByPlayer[PlayerNumber.One].CurrentPlayerScore += cardsWonCount;
                cardHandByPlayer[PlayerNumber.Two].CurrentPlayerScore -= cardsWonCount;
                break;

            case PlayerNumber.Two:
                cardHandByPlayer[PlayerNumber.Two].CurrentPlayerScore += cardsWonCount;
                cardHandByPlayer[PlayerNumber.One].CurrentPlayerScore -= cardsWonCount;
                break;
        }

        cardsWonCount = 0;

        bool isGameOver = (cardPlayedCount == selectableAreasList.GetLength(0) * selectableAreasList.GetLength(1));
        if (isGameOver) {
            AnimatorFSM.SetTrigger(gameOverTriggerId);
        }
        else {
            AnimatorFSM.SetTrigger(nextStateTriggerId);
        }
    }

    [StateEnterMethod("Base Layer.GameOver")]
    private void GameOverState() {
        Debug.Log("GameManager::GameOverState");

        CurrentPlayer = PlayerNumber.None;

        winScreen.gameObject.SetActive(true);

        if (cardHandByPlayer[PlayerNumber.One].CurrentPlayerScore > cardHandByPlayer[PlayerNumber.Two].CurrentPlayerScore) {
            winText.text = string.Format("<#{0}>Blue</color>\nwins !", ColorUtility.ToHtmlStringRGB(playersColorsList.GetColorByPlayer(PlayerNumber.One)));
            AudioManager.Instance.PlayVictoryMusic();
        }
        else if (cardHandByPlayer[PlayerNumber.One].CurrentPlayerScore < cardHandByPlayer[PlayerNumber.Two].CurrentPlayerScore) {
            winText.text = string.Format("<#{0}>Red</color>\nwins !", ColorUtility.ToHtmlStringRGB(playersColorsList.GetColorByPlayer(PlayerNumber.Two)));
            AudioManager.Instance.PlayVictoryMusic();
        }
        else {
            winText.text = string.Format("<#{0}>Draw</color> !", ColorUtility.ToHtmlStringRGB(drawColor));
        }
    }
}
