using System.Collections.Generic;
using PonyTriad.Model;

public class CustomGameHolder
{
    private static Dictionary<PlayerNumber, List<Card>> nextCardDeckByPlayer;
    public static Dictionary<PlayerNumber, List<Card>> NextCardDeckByPlayer {
        get {
            Dictionary<PlayerNumber, List<Card>> value = nextCardDeckByPlayer;
            nextCardDeckByPlayer = null;
            return value;
        }
        set {
            nextCardDeckByPlayer = value;
        }
    }

    private static GameRule? nextGameRules;
    public static GameRule? NextGameRules {
        get {
            GameRule? value = nextGameRules;
            nextGameRules = null;
            return value;
        }
        set {
            nextGameRules = value;
        }
    }

    private static bool isSuddenDeathNewGame;
    public static bool IsSuddenDeathNewGame {
        get {
            bool value = isSuddenDeathNewGame;
            isSuddenDeathNewGame = false;
            return value;
        }
        set {
            isSuddenDeathNewGame = value;
        }
    }

    private static bool isRandomRules;
    public static bool IsRandomRules {
        get {
            bool value = isRandomRules;
            isRandomRules = false;
            return value;
        }
        set {
            isRandomRules = value;
        }
    }
}