using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayersColorsData", menuName = "Pony Triad/Players Colors Data")]
public class PlayersColorsData: ScriptableObject
{
    [SerializeField] private Color nonePlayerColor = default(Color);
    [SerializeField] private Color playerOneColor = default(Color);
    [SerializeField] private Color playerTwoColor = default(Color);

    private Dictionary<PlayerNumber, Color> colorByPlayer = null;

    public Color GetColorByPlayer(PlayerNumber _targetPlayer)
    {
        if (colorByPlayer == null)
        {
            colorByPlayer = new Dictionary<PlayerNumber, Color>
            {
                [PlayerNumber.None] = nonePlayerColor,
                [PlayerNumber.One] = playerOneColor,
                [PlayerNumber.Two] = playerTwoColor
            };
        }

        return colorByPlayer[_targetPlayer];
    }
}
