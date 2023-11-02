using BattleShipEngine;

namespace BattleShipStrategies.Default;

public class DefaultGameStrategy : IGameStrategy
{
    private GameSetting _setting;
    
    public Int2 GetMove()
    {
        return new Int2(
            Random.Shared.Next(_setting.Width),
            Random.Shared.Next(_setting.Height)
        );
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

    public void Start(GameSetting setting)
    {
        _setting = setting;
    }
}