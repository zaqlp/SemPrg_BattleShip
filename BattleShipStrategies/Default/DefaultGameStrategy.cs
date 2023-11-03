using BattleShipEngine;

namespace BattleShipStrategies.Default;

public class DefaultGameStrategy : IGameStrategy
{
    private GameSetting setting;
    
    public Int2 GetMove()
    {
        return new Int2(
            Random.Shared.Next(setting.Width),
            Random.Shared.Next(setting.Height)
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
        this.setting = setting;
    }
}