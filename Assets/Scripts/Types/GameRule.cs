[System.Flags]
public enum GameRule // see also here : https://ffxivtriad.com/rules
{
    None = 0,

    AllOpen = 1 << 0,
    ThreeOpen = 1 << 8,

    Same = 1 << 1,
    Plus = 1 << 2,

    Borderless = 1 << 3,
    SameWalls = 1 << 4,

    Reversed = 1 << 5,
    FallenAce = 1 << 11,

    Random = 1 << 6,
    Chaos = 1 << 9,
    Order = 1 << 10,

    SuddenDeath = 1 << 7,
}