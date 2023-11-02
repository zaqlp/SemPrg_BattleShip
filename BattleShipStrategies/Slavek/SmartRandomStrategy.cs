using BattleShipEngine;

namespace BattleShipStrategies.Slavek;

public class SmartRandomStrategy : IGameStrategy
{
    private List<Int2> _shots = new List<Int2>();
    private GameSetting _setting;
    
    public Int2 GetMove()
    {
        while (true)
        {
            Int2 shot = new Int2(
                Random.Shared.Next(_setting.Width),
                Random.Shared.Next(_setting.Height)
            );
            if (!_shots.Contains(shot))
            {
                _shots.Add(shot);
                return shot;
            }
        }
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
        _shots = new List<Int2>();
        _setting = setting;
    }
}