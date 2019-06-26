using System.Collections.Generic;
using UnityEngine;
using AnimatorStateMachineLibrary;
using PonyTriad.Model;
using PonyTriad.Audio;

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
        Debug.Log("GameManager::IntroState");

        bool isOpenRuleActive = activeGameRules.HasFlag(GameRule.AllOpen);

        game = new Game(cardsListArray, cardsPerPlayer, activeGameRules);

        playerOneView.Init(game.GetPlayerByNumber(PlayerNumber.One), false, isOpenRuleActive);
        playerOneView.OnHandReady += PlayerHandReady;

        playerTwoView.Init(game.GetPlayerByNumber(PlayerNumber.Two), false, isOpenRuleActive);
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

    private void CardPlayed(CardBoardPartView _playedCardBoardArea) {
        playerViewByNumber[CurrentPlayer].RemoveCard(_playedCardBoardArea.Card);
        playerViewByNumber[CurrentPlayer].Enable(false);

        turnResultByPhase = game.PlayCard(CurrentPlayer, _playedCardBoardArea.Card.Model, _playedCardBoardArea.BoardPosition);

        AnimatorFSM.SetTrigger(nextStateTriggerId);
    }


    [StateEnterMethod("Base Layer.SameRule")]
    private void SameRuleState() {
        Debug.Log("GameManager::SameRuleState");

        if (turnResultByPhase.TryGetValue(GamePhase.Same, out currentPhaseResult)) {
            SpecialRuleText sameRuleText = Instantiate(sameRuleTextPrefab, uiCanvas.transform);
            sameRuleText.OnAnimationFinished += () => {
                ProcessWonCardsList(currentPhaseResult.cardsWonList);
            };
        }
        else {
            AnimatorFSM.SetTrigger(nextStateTriggerId);
        }
    }


    [StateEnterMethod("Base Layer.PlusRule")]
    private void PlusRuleState() {
        Debug.Log("GameManager::PlusRuleState");

        if (turnResultByPhase.TryGetValue(GamePhase.Plus, out currentPhaseResult)) {
            SpecialRuleText plusRuleText = Instantiate(plusRuleTextPrefab, uiCanvas.transform);
            plusRuleText.OnAnimationFinished += () => {
                ProcessWonCardsList(currentPhaseResult.cardsWonList);
            };
        }
        else {
            AnimatorFSM.SetTrigger(nextStateTriggerId);
        }
    }

    [StateEnterMethod("Base Layer.Fight")]
    private void FightState() {
        Debug.Log("GameManager::FightState");

        if (turnResultByPhase.TryGetValue(GamePhase.Normal, out currentPhaseResult)) {
            ProcessWonCardsList(currentPhaseResult.cardsWonList);
        }
        else {
            AnimatorFSM.SetTrigger(nextStateTriggerId);
        }
    }

    private void ProcessWonCardsList(List<CardOnBoardWon> _cardsWonList) {
        cardsRotationFinishedCount = 0;
        foreach (CardOnBoardWon cardWonItem in _cardsWonList) {
            // if (cardWonItem.card.PlayerOwner != cardWonItem.previousPlayerOwner) {
            Debug.LogFormat("Card won : {0}", cardWonItem.card.BoardPosition);
            Vector2Int cardWonBoardPosition = cardWonItem.card.BoardPosition.Value;
            selectableAreasList[cardWonBoardPosition.x, cardWonBoardPosition.y].Card.ChangePlayerOwner(cardWonItem.direction, CurrentPlayer);
            cardsRotateCount++;
            // }
        }
    }

    private void CardAnimationFinished(CardBoardPartView _cardTarget) {
        cardsRotationFinishedCount++;

        if (cardsRotationFinishedCount == cardsRotateCount) {
            cardsRotationFinishedCount = 0;
            cardsRotateCount = 0;

            if (currentPhaseResult.comboCardList != null && currentPhaseResult.comboCardList.Count > 0) {
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
        Debug.Log("GameManager::BetweenTurnState");

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
        Debug.Log("GameManager::GameOverState");

        CurrentPlayer = PlayerNumber.None;

        winScreen.gameObject.SetActive(true);

        if (playerViewByNumber[PlayerNumber.One].CurrentPlayerScore > playerViewByNumber[PlayerNumber.Two].CurrentPlayerScore) {
            winText.text = string.Format("<#{0}>Blue</color>\nwins !", ColorUtility.ToHtmlStringRGB(playersColorsList.GetColorByPlayer(PlayerNumber.One)));
            AudioManager.Instance.PlayVictoryMusic();
        }
        else if (playerViewByNumber[PlayerNumber.One].CurrentPlayerScore < playerViewByNumber[PlayerNumber.Two].CurrentPlayerScore) {
            winText.text = string.Format("<#{0}>Red</color>\nwins !", ColorUtility.ToHtmlStringRGB(playersColorsList.GetColorByPlayer(PlayerNumber.Two)));
            AudioManager.Instance.PlayVictoryMusic();
        }
        else {
            winText.text = string.Format("<#{0}>Draw</color> !", ColorUtility.ToHtmlStringRGB(drawColor));
        }
    }
}
