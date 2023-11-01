using BattleShipEngine;

namespace BattleShipStrategies.Default;

public class DefaultGameStrategy : IGameStrategy
{
    public Int2 GetMove()
    {
        return new Int2(Random.Shared.Next(10), Random.Shared.Next(10));
    }

    public void RespondHit()
    {
    }

    public void RespondSunk()
    {
    }

    public void RespondMiss()
    {
    }
}