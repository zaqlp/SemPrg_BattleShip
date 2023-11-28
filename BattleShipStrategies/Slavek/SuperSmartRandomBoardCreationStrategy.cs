using BattleShipEngine;
using BattleShipStrategies.MartinF;

namespace BattleShipStrategies.Slavek;

public class SuperSmartRandomBoardCreationStrategy : IBoardCreationStrategy
{
    private readonly IBoardCreationStrategy _smartStrategy;
    private readonly List<IGameStrategy> _gameStrategies;
    private readonly BoatPaster _paster = new BoatPaster();

    public SuperSmartRandomBoardCreationStrategy()
    {
        _smartStrategy = new SmartRandomBoardCreationStrategy();
        _gameStrategies = new List<IGameStrategy>();
        _gameStrategies.Add(new DeathCrossStrategy());
        _gameStrategies.Add(new MartinStrategy());
    }
    public Int2[] GetBoatPositions(GameSetting setting)
    {
        int mostMoves = 0;
        Int2[] bestResult = new Int2[] {};
        for (int i = 0; i < 10; i++)
        {
            Int2[] newResult = _smartStrategy.GetBoatPositions(setting);
            _paster.SetBoats(newResult);
            int newMoves = 0;
            Game game = new Game(_paster, setting);
            foreach (var gameStrategy in _gameStrategies)
            {
                newMoves += game.SimulateGame(gameStrategy);
            }
            if (newMoves > mostMoves)
            {
                bestResult = newResult;
                mostMoves = newMoves;
            }
        }
        return bestResult;
    }
}

internal class BoatPaster : IBoardCreationStrategy
{
    private Int2[] _boats = new Int2[]{};
    public void SetBoats(Int2[] boats)
    {
        _boats = boats;
    }
    
    public Int2[] GetBoatPositions(GameSetting setting)
    {
        return _boats;
    }
}
