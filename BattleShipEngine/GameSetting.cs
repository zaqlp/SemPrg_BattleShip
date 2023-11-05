namespace BattleShipEngine;

/// <summary>
/// Represents the setting of a game
/// </summary>
/// <param name="Height">Height of the board</param>
/// <param name="Width">Width of the board</param>
///<param name="BoatCount">The 0th element is the amount of <c>1 tile long boats</c>, the 1th element is the amount of <c>2 tile long boats</c>, ...</param>
public readonly record struct GameSetting(
    int Height,
    int Width,
    int[] BoatCount)
{
    /// <summary>
    /// Settings that can be found here <see href="https://www.codewars.com/kata/52bb6539a4cf1b12d90005b7"></see>
    /// </summary>
    public static GameSetting Default = new(
        10, 
        10, 
        new[] {4, 3, 2, 1});
}
