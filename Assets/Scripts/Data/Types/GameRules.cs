[System.Flags]
public enum GameRule
{
    None = 0,

    Open = 1 << 0,

    Same = 1 << 1,
    Plus = 1 << 2,

    Borderless = 1 << 3,
    SameWalls = 1 << 4,

    Reversed = 1 << 5,

    Random = 1 << 6,

    Deathmatch = 1 << 7,
}