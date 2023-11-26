namespace BattleShipEngine;

/// <summary>
/// Represents a point in space. A 2 Dimensional vector.
/// </summary>
/// <param name="X"></param>
/// <param name="Y"></param>
public readonly record struct Int2(int X, int Y)
{
    public static Int2 operator +(Int2 a, Int2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Int2 operator -(Int2 a, Int2 b) => new(a.X - b.X, a.Y - b.Y);
};