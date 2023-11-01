namespace BattleShipEngine;

public readonly record struct GameSetting(
    int Height,
    int Width,
    int[] BoatCount
);
