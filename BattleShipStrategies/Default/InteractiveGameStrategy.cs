using BattleShipEngine;

namespace BattleShipStrategies.Default;

public class InteractiveGameStrategy : IGameStrategy
{
    public Int2 GetMove()
    {
        while (true)
        {
            Console.WriteLine("Enter your move (in the format x,y):");
            
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
            return new Int2(row, column);
        }
    }

    public void RespondHit()
    {
        Console.WriteLine("Hit!");
    }

    public void RespondSunk()
    {
        Console.WriteLine("Sunk!!");
    }

    public void RespondMiss()
    {
        Console.WriteLine("Miss.");
    }

    public void Start(GameSetting setting)
    {
        Console.WriteLine("An interactive game starts.");
        Console.WriteLine($"There is {setting.Width} columns and {setting.Height} rows.");
        Console.WriteLine("There are also these boats:");
        for (int i = 0; i < setting.BoatCount.Length; i++)
        {
            Console.WriteLine($"    {setting.BoatCount[i]} times {i + 1} squares long");
        }
    }
}