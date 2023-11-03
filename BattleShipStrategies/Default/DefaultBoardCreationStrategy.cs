using BattleShipEngine;

namespace BattleShipStrategies.Default;

public class DefaultBoardCreationStrategy : IBoardCreationStrategy
{
    public Int2[] GetBoatPositions(GameSetting setting)
    {
        List<Int2> boats =  new List<Int2>();
        int line = 0;
        int positionInLine = 0;
        for (int boatType = 0; boatType < setting.BoatCount.Length; boatType++)
        for (int boat = 0; boat < setting.BoatCount[boatType]; boat++)
        {
            if (positionInLine + boatType + 1 > setting.Width)
            {
                line += 2;
                positionInLine = 0;
            }
            for (int boatPart = 0; boatPart < boatType + 1; boatPart++)
                boats.Add(new Int2(line, positionInLine + boatPart));
            positionInLine += boatType + 2;
        }
        return boats.ToArray();
    }
}