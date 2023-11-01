namespace BattleShipEngine;

public interface IBoardCreationStrategy
{
    public Int2[] GetBoatPositions(GameSetting setting);
}