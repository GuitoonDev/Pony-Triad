using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayersColorsList", menuName = "ProtoTriad/PlayersColorsList")]
public class PlayersColorsDefinition : ScriptableObject
{
    [SerializeField] private Color nonePlayerColor = default(Color);
    [SerializeField] private Color playerOneColor = default(Color);
    [SerializeField] private Color playerTwoColor = default(Color);

    private Dictionary<PlayerNumber, Color> colorByPlayer = null;

    public Color GetColorByPlayer(PlayerNumber _targetPlayer) {
        if (colorByPlayer == null) {
            PopulateColorListByPlayer(out colorByPlayer);
        }
        return colorByPlayer[_targetPlayer];
    }

    private void PopulateColorListByPlayer(out Dictionary<PlayerNumber, Color> _colorByPlayer) {
        _colorByPlayer = new Dictionary<PlayerNumber, Color>();

        _colorByPlayer[PlayerNumber.None] = nonePlayerColor;
        _colorByPlayer[PlayerNumber.One] = playerOneColor;
        _colorByPlayer[PlayerNumber.Two] = playerTwoColor;
    }
}
