using BattleShipEngine;

namespace BattleShipStrategies.Default;

public class InteractiveBoardCreationStrategy : IBoardCreationStrategy
{
    public Int2[] GetBoatPositions(GameSetting setting)
    {
        Console.WriteLine("Create a board.");
        Console.WriteLine($"There is {setting.Width} columns and {setting.Height} rows.");
        Console.WriteLine("There also should be these boats:");
        int boatSquaresSum = 0;
        for (int i = 0; i < setting.BoatCount.Length; i++)
        {
            Console.WriteLine($"    {setting.BoatCount[i]} times {i + 1} squares long");
            boatSquaresSum += (i + 1) * setting.BoatCount[i];
        }
        Console.WriteLine($"Now write {boatSquaresSum} positions of boats (in the format x,y).");
        List<Int2> boats =  new List<Int2>();
        while (boatSquaresSum > 0)
        {
            var input = Console.ReadLine() ?? String.Empty;
            var parts = input.Split(',');
            
            if (parts.Length != 2)
            {
                Console.WriteLine("Invalid input.");
                continue;
            }
            if (!int.TryParse(parts[0], out var row))
            {
                Console.WriteLine("Invalid X.");
                continue;
            }
            if (!int.TryParse(parts[1], out var column))
            {
                Console.WriteLine("Invalid Y.");
                continue;
            }
            boats.Add(new Int2(row, column));
            boatSquaresSum--;
        }
        return boats.ToArray();
    }
}