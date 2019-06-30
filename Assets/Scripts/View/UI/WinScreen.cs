using System.Collections;
using System.Collections.Generic;
using PonyTriad.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winText = null;
    [SerializeField] private Image choiceSelectionPanel = null;

    [Header("End Game Colors")]
    [SerializeField] private Color drawColor = default(Color);
    [SerializeField] private PlayersColorsDefinition playersColorsList = null;

    public void Show(PlayerNumber _playerWon) {
        gameObject.SetActive(true);

        switch (_playerWon) {
            case PlayerNumber.One:
                winText.text = string.Format("<#{0}>Blue</color>\nwins !", ColorUtility.ToHtmlStringRGB(playersColorsList.GetColorByPlayer(PlayerNumber.One)));
                break;

            case PlayerNumber.Two:
                winText.text = string.Format("<#{0}>Red</color>\nwins !", ColorUtility.ToHtmlStringRGB(playersColorsList.GetColorByPlayer(PlayerNumber.Two)));
                break;

            case PlayerNumber.None:
                winText.text = string.Format("<#{0}>Draw</color> !", ColorUtility.ToHtmlStringRGB(drawColor));

                if (Game.activeRules.HasFlag(GameRule.SuddenDeath)) {
                    choiceSelectionPanel.gameObject.SetActive(false);
                }
                break;
        }
    }
}
