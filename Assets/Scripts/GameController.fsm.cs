using System.Collections.Generic;
using UnityEngine;
using AnimatorStateMachineLibrary;
using PonyTriad.Model;
using PonyTriad.Audio;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public partial class GameController : MonoBehaviour
{
    private Animator animatorFSM;
    private Animator AnimatorFSM {
        get {
            if (animatorFSM == null) {
                animatorFSM = GetComponent<Animator>();
            }
            return animatorFSM;
        }
    }
    private int nextStateTriggerId;
    private int gameOverTriggerId;

    private PlayerNumber currentPlayer = PlayerNumber.None;
    private PlayerNumber CurrentPlayer {
        get {
            return currentPlayer;
        }
        set {
            if (currentPlayer != value && value != PlayerNumber.None) {
                if (currentPlayer != PlayerNumber.None) {
                    playerViewByNumber[currentPlayer].Enable(false);
                }

                currentPlayer = value;

                playerViewByNumber[currentPlayer].Enable(true);
            }
            else if (value == PlayerNumber.None) {
                foreach (PlayerView handItem in playerViewByNumber.Values) {
                    handItem.Enable(false);
                }
            }
        }
    }

    private CardBoardPartView[,] selectableAreasList = new CardBoardPartView[3, 3];
    private Dictionary<PlayerNumber, PlayerView> playerViewByNumber = new Dictionary<PlayerNumber, PlayerView>();


    private Game game;
    Dictionary<GamePhase, ResultPhaseData> turnResultByPhase;
    ResultPhaseData currentPhaseResult;

    private int handsReadyCount = 0;
    private int cardsRotateCount = 0;
    private int cardsRotationFinishedCount = 0;

    private void Start() {
        nextStateTriggerId = Animator.StringToHash("NextState");
        gameOverTriggerId = Animator.StringToHash("GameOver");
    }

    [StateEnterMethod("Base Layer.Intro")]
    private void IntroState() {
        GameRule? customGameRules = CustomGameHolder.NextGameRules;

        randomRules = randomRules || CustomGameHolder.IsRandomRules;

        if (randomRules && !CustomGameHolder.IsSuddenDeathNewGame) {
            activeGameRules = default(GameRule);

            float openRuleRandomValue = Random.value;
            if (openRuleRandomValue > 0.5) {
                activeGameRules |= GameRule.AllOpen;
            }
            else if (openRuleRandomValue > 0.25) {
                activeGameRules |= GameRule.ThreeOpen;
            }

            activeGameRules |= Random.value > 0.4 ? GameRule.Same : GameRule.None;
            activeGameRules |= Random.value > 0.4 ? GameRule.Plus : GameRule.None;

            float borderRuleRandomValue = Random.value;
            if (borderRuleRandomValue > 0.8) {
                activeGameRules |= GameRule.Borderless;
            }
            else if (borderRuleRandomValue > 0.6 && activeGameRules.HasFlag(GameRule.Same)) {
                activeGameRules |= GameRule.SameWalls;

            }

            activeGameRules |= Random.value > 0.9 ? GameRule.Reversed : GameRule.None;
            activeGameRules |= Random.value > 0.9 ? GameRule.FallenAce : GameRule.None;

            activeGameRules |= Random.value > 0.45 ? GameRule.SuddenDeath : GameRule.None;
        }
        else if (customGameRules.HasValue) {
            activeGameRules = customGameRules.Value;
        }

        ruleBarHolder.Init(activeGameRules);
        game = new Game(cardsListArray, cardsPerPlayer, activeGameRules);

        playerOneView.Init(game.GetPlayerByNumber(PlayerNumber.One), false);
        playerOneView.OnHandReady += PlayerHandReady;

        playerTwoView.Init(game.GetPlayerByNumber(PlayerNumber.Two), false);
        playerTwoView.OnHandReady += PlayerHandReady;

        playerViewByNumber[PlayerNumber.One] = playerOneView;
        playerViewByNumber[PlayerNumber.Two] = playerTwoView;

        for (int positionX = 0; positionX < selectableAreasList.GetLength(0); positionX++) {
            for (int positionY = 0; positionY < selectableAreasList.GetLength(1); positionY++) {
                CardBoardPartView newSelectableArea = verticalListableAreasList[positionX][positionY];
                newSelectableArea.OnCardPlayed += CardPlayed;
                newSelectableArea.OnCardAnimationFinished += CardAnimationFinished;
                newSelectableArea.BoardPosition = new Vector2Int(positionX, positionY);
                selectableAreasList[positionX, positionY] = newSelectableArea;
            }
        }

        currentPlayer = Random.Range(0, 2) < 1 ? PlayerNumber.One : PlayerNumber.Two;

        AudioManager.Instance.PlayGameMusic();
    }

    private void PlayerHandReady(PlayerView _cardHandReady) {
        _cardHandReady.OnHandReady -= PlayerHandReady;
        handsReadyCount++;

        if (handsReadyCount == playerViewByNumber.Count) {
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
        Debug.LogFormat("GameController::PickCard");
        switch (currentPlayer) {
            case PlayerNumber.One:
                CurrentPlayer = PlayerNumber.Two;
                break;

            case PlayerNumber.Two:
                CurrentPlayer = PlayerNumber.One;
                break;
        }
    }

    private void CardPlayed(CardBoardPartView _playedCardBoardArea) {
        Debug.LogFormat("GameController::CardPlayed");
        playerViewByNumber[CurrentPlayer].RemoveCard(_playedCardBoardArea.Card);
        playerViewByNumber[CurrentPlayer].Enable(false);

        turnResultByPhase = game.PlayCard(CurrentPlayer, _playedCardBoardArea.Card.Model, _playedCardBoardArea.BoardPosition);

        AnimatorFSM.SetTrigger(nextStateTriggerId);
    }


    [StateEnterMethod("Base Layer.SameRule")]
    private void SameRuleState() {
        Debug.LogFormat("GameController::SameRuleState");
        if (turnResultByPhase.TryGetValue(GamePhase.Same, out currentPhaseResult)) {
            foreach (CardOnBoardWon cardWonItem in currentPhaseResult.cardWonList) {
                Vector2Int cardWonBoardPosition = cardWonItem.card.BoardPosition.Value;
                CardView targetCardViewItem = selectableAreasList[cardWonBoardPosition.x, cardWonBoardPosition.y].Card;
                targetCardViewItem.StartShinyAnimation();
            }

            SpecialRuleText sameRuleText = Instantiate(sameRuleTextPrefab, uiCanvas.transform);
            sameRuleText.OnAnimationFinished += () => {
                ProcessWonCardsList(currentPhaseResult.cardWonList);
            };
        }
        else {
            AnimatorFSM.SetTrigger(nextStateTriggerId);
        }
    }


    [StateEnterMethod("Base Layer.PlusRule")]
    private void PlusRuleState() {
        Debug.LogFormat("GameController::PlusRuleState");
        if (turnResultByPhase.TryGetValue(GamePhase.Plus, out currentPhaseResult)) {
            foreach (CardOnBoardWon cardWonItem in currentPhaseResult.cardWonList) {
                Vector2Int cardWonBoardPosition = cardWonItem.card.BoardPosition.Value;
                CardView targetCardViewItem = selectableAreasList[cardWonBoardPosition.x, cardWonBoardPosition.y].Card;
                targetCardViewItem.StartShinyAnimation();
            }

            SpecialRuleText plusRuleText = Instantiate(plusRuleTextPrefab, uiCanvas.transform);
            plusRuleText.OnAnimationFinished += () => {
                ProcessWonCardsList(currentPhaseResult.cardWonList);
            };
        }
        else {
            AnimatorFSM.SetTrigger(nextStateTriggerId);
        }
    }

    [StateEnterMethod("Base Layer.Fight")]
    private void FightState() {
        Debug.LogFormat("GameController::FightState");
        if (turnResultByPhase.TryGetValue(GamePhase.Normal, out currentPhaseResult)) {
            ProcessWonCardsList(currentPhaseResult.cardWonList);
        }
        else {
            AnimatorFSM.SetTrigger(nextStateTriggerId);
        }
    }

    private void ProcessWonCardsList(List<CardOnBoardWon> _cardsWonList) {
        Debug.LogFormat("GameController::ProcessWonCardsList");
        cardsRotationFinishedCount = 0;
        foreach (CardOnBoardWon cardWonItem in _cardsWonList) {
            Debug.LogFormat("Card won : {0}", cardWonItem.card.BoardPosition);
            Vector2Int cardWonBoardPosition = cardWonItem.card.BoardPosition.Value;
            CardView targetCardViewItem = selectableAreasList[cardWonBoardPosition.x, cardWonBoardPosition.y].Card;
            if (targetCardViewItem.PlayerOwner != CurrentPlayer) {
                targetCardViewItem.ChangePlayerOwner(cardWonItem.direction, CurrentPlayer);
                cardsRotateCount++;
            }
        }
    }

    private void CardAnimationFinished(CardBoardPartView _cardTarget) {
        cardsRotationFinishedCount++;

        if (cardsRotationFinishedCount == cardsRotateCount) {
            cardsRotationFinishedCount = 0;
            cardsRotateCount = 0;

            if (currentPhaseResult.comboCardList != null && currentPhaseResult.comboCardList.Count > 0) {
                Debug.LogFormat("GameController::CardAnimationFinished -> COMBO");
                SpecialRuleText comboRuleText = Instantiate(comboRuleTextPrefab, uiCanvas.transform);
                ProcessWonCardsList(currentPhaseResult.comboCardList.Dequeue());
            }
            else {
                AnimatorFSM.SetTrigger(nextStateTriggerId);
            }
        }
    }

    [StateEnterMethod("Base Layer.BetweenTurns")]
    private void BetweenTurnState() {
        Debug.LogFormat("GameController::BetweenTurnState");
        playerViewByNumber[PlayerNumber.One].CurrentPlayerScore = game.GetPlayerByNumber(PlayerNumber.One).Score;
        playerViewByNumber[PlayerNumber.Two].CurrentPlayerScore = game.GetPlayerByNumber(PlayerNumber.Two).Score;

        if (game.IsOver) {
            AnimatorFSM.SetTrigger(gameOverTriggerId);
        }
        else {
            AnimatorFSM.SetTrigger(nextStateTriggerId);
        }
    }

    [StateEnterMethod("Base Layer.GameOver")]
    private void GameOverState() {
        CurrentPlayer = PlayerNumber.None;

        PlayerNumber playerWon = PlayerNumber.None;
        if (playerViewByNumber[PlayerNumber.One].CurrentPlayerScore > playerViewByNumber[PlayerNumber.Two].CurrentPlayerScore) {
            playerWon = PlayerNumber.One;
            AudioManager.Instance.PlayVictoryMusic();
        }
        else if (playerViewByNumber[PlayerNumber.One].CurrentPlayerScore < playerViewByNumber[PlayerNumber.Two].CurrentPlayerScore) {
            playerWon = PlayerNumber.Two;
            AudioManager.Instance.PlayVictoryMusic();
        }
        else {
            if (Game.activeRules.HasFlag(GameRule.SuddenDeath)) {
                CustomGameHolder.NextCardDeckByPlayer = game.GetCurrentOwnedCardByPlayer();
                CustomGameHolder.NextGameRules = Game.activeRules;
                CustomGameHolder.IsSuddenDeathNewGame = true;
                DOVirtual.DelayedCall(3, SelectNewGame);
            }
        }

        winScreen.Show(playerWon);
    }
}
