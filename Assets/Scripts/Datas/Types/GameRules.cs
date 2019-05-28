[System.Flags]
public enum GameRules
{
    None = 0,

    Open = 1 << 0,

    Same = 1 << 1,
    Plus = 1 << 2,

    Borderless = 1 << 3,
    AceWalls = 1 << 4,

    Battle = 1 << 5,
}