using UnityEngine;

[CreateAssetMenu(fileName = "PlayersColorsList", menuName = "ProtoTriad/PlayersColorsList")]
public class PlayersColorsList : ScriptableObject
{
    [System.Serializable]
    public class PlayerColor
    {
        [SerializeField] private PlayerNumber player = default(PlayerNumber);
        public PlayerNumber Player => player;

        [SerializeField] private Color color = default(Color);
        public Color Color => color;
    }

    [SerializeField] private PlayerColor[] playerColorList = null;
    public Color GetColorByPlayer(PlayerNumber _player) {
        PlayerColor playerColorValue = null;
        for (int i = 0; playerColorValue == null && i < playerColorList.Length; i++) {
            if (playerColorList[i].Player == _player) {
                playerColorValue = playerColorList[i];
            }
        }

        return playerColorValue.Color;
    }
}
