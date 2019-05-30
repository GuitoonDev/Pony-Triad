[System.Flags]
public enum GameRule
{
    None = 0,

    Open = 1 << 0,

    Same = 1 << 1,
    Plus = 1 << 2,

    Borderless = 1 << 3,
    SameAceWalls = 1 << 4,

    Battle = 1 << 5,

    Random = 1 << 6,
    Reversed = 1 << 7
}