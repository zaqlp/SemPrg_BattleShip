using System.Diagnostics.CodeAnalysis;
using BattleShipEngine;

namespace BattleShipStrategies.MartinF;

public class MartinStrategy : IGameStrategy
{
    private GameSetting settings;
    private Dictionary<Int2, HitResult> hits;
    private HashSet<Int2> possibleSpots; //Tiles where we can shoot and it makes sense
    private Int2 lastMove; //Move established by GetMove, which can then be used by the respond methods, is -1,-1 if no move has been made yet


    public Int2 GetMove()
    {
        if (lastMove.X == -1)
        {
            //No move was done yet so fire at random
            lastMove = new Int2(
                Random.Shared.Next(0, settings.Width),
                Random.Shared.Next(0, settings.Height));
            return lastMove;
        }

        //Calculate the next move

        //If the tile we hit last time was a sunk, all tiles around it are water
        if (hits[lastMove] == HitResult.Sunk)
        {
            //Remove spots around the sunk tile from possible spots

            var neighbors = Extensions.GetNeighbors(lastMove);
            foreach (var neighbor in neighbors)
            {
                possibleSpots.Remove(neighbor);
            }
        }

        var move = possibleSpots.First();

        possibleSpots.Remove(move);
        lastMove = move;
        return move;
    }

    public void RespondHit()
    {
        hits[lastMove] = HitResult.Hit;
    }

    public void RespondSunk()
    {
        hits[lastMove] = HitResult.Sunk; //Override hit
    }

    public void RespondMiss()
    {
        hits[lastMove] = HitResult.Miss;
    }

    public void Start(GameSetting setting)
    {
        this.settings = setting;
        lastMove = new(-1, -1);

        hits = new(settings.Width * settings.Height);
        possibleSpots = new(settings.Width * settings.Height);
        //Populate possible spots with all tiles
        for (int x = 0; x < settings.Width; x++)
        {
            for (int y = 0; y < settings.Height; y++)
            {
                possibleSpots.Add(new(x, y));
            }
        }
    }

    private enum HitResult
    {
        Hit,
        Sunk,
        Miss
    }

    private static class Extensions
    {

        /// <returns>Points that are to the right, top right, top, ... in counterclockwise order starting from right</returns>
        [SuppressMessage("ReSharper", "WithExpressionModifiesAllMembers")]
        public static Int2[] GetNeighbors(Int2 ofInt)
        {
            return new Int2[]
            {
                ofInt with {X = ofInt.X + 1},
                ofInt with {X = ofInt.X + 1, Y = ofInt.Y + 1},
                ofInt with {Y = ofInt.Y + 1},
                ofInt with {X = ofInt.X - 1, Y = ofInt.Y + 1},
                ofInt with {X = ofInt.X - 1},
                ofInt with {X = ofInt.X - 1, Y = ofInt.Y - 1},
                ofInt with {Y = ofInt.Y - 1},
                ofInt with {X = ofInt.X + 1, Y = ofInt.Y - 1}
            };

        }
    }
}
