using System.Diagnostics.CodeAnalysis;
using BattleShipEngine;

namespace BattleShipStrategies.MartinF;

public class MartinStrategy : IGameStrategy
{
    private GameSetting settings;
    private Dictionary<Int2, HitResult> hits;
    private HashSet<Int2> possibleSpots; //Tiles where we can shoot and it makes sense
    private Int2 lastMove; //Move established by GetMove, which can then be used by the respond methods, is -1,-1 if no move has been made yet

    private Int2? lastBoatHitPoint;
    private Int2? boatDirection;
    private List<Int2> currentBoat;

    private HashSet<Int2> hitSpots;
    private HashSet<Int2> sunkSpots;

    public void Start(GameSetting setting) //Pseudo constructor
    {
        this.settings = setting;
        hits = new(settings.Width * settings.Height);
        possibleSpots = new(settings.Width * settings.Height);
        PopulatePossibleSpots();

        lastMove = new(-1, -1);

        lastBoatHitPoint = null;
        boatDirection = null;
        currentBoat = new();

        hitSpots = new();
        sunkSpots = new();
    }

    private void PopulatePossibleSpots()
    {
        //Add the spots "diagonally"
        //Every other tile, and then every other tile to fill the gaps

        //Add first diagonal set
        for (int y = 0; y < settings.Height; y += 1)
        {
            for (int x = y % 2; x < settings.Width; x += 2)
            {
                possibleSpots.Add(new Int2(x, y));
                //Console.SetCursorPosition(x * 4, y * 2);
                //Console.Write(i);
                //i++;
            }
        }

        //Add second diagonal set
        for (int y = 0; y < settings.Height; y += 1)
        {
            for (int x = (y + 1) % 2; x < settings.Width; x += 2)
            {
                possibleSpots.Add(new Int2(x, y));
                //Console.SetCursorPosition(x * 4, y * 2);
                //Console.Write(i);
                //i++;
            }
        }


        //Had worse results..
        //Add the spots "diagonally"
        /* Like this:
         * 0 3 5
         * 6 1 4
         * 9 7 2
        */

        //var i = 0;

        //for (int d = 0; d < settings.Width; d++)
        //{
        //    possibleSpots.Add(new(d, d));
        //    //Console.SetCursorPosition(d * 4, d * 2);
        //    //Console.Write(i);
        //    //i++;
        //}

        //// Diagonals above and below the middle diagonal
        //for (int d = 1; d < settings.Width; d++)
        //{
        //    // Diagonal above the middle diagonal
        //    for (int y = d, x = 0; y < settings.Height; y++, x++)
        //    {
        //        possibleSpots.Add(new(x, y));
        //        //Console.SetCursorPosition(x * 4, y * 2);
        //        //Console.Write(i);
        //        //i++;
        //    }

        //    // Diagonal below the middle diagonal
        //    for (int y = 0, x = d; x < settings.Width; y++, x++)
        //    {
        //        possibleSpots.Add(new(x, y));
        //        //Console.SetCursorPosition(x * 4, y * 2);
        //        //Console.Write(i);
        //        //i++;
        //    }
        //}
    }

    public Int2 GetMove()
    {
        Int2 move;

        if (lastMove.X == -1)
        {
            //No move was done yet so go with the first possible spot
            
            move = possibleSpots.First();
            goto commitMove;
        }

        //We already shot somewhere

        var lastHitResult = hits[lastMove];

        //Search for a boat that we hit
        if (lastBoatHitPoint is not null)
        {
            //We shot to the right, is there something?
            switch (lastHitResult)
            {
                case HitResult.Hit:
                    //We hit the boat again, so continue in that direction
                    //But if it leads us out of bounds, we need to turn around
                    
                    hitSpots.Add(lastMove);

                    lastBoatHitPoint = lastMove;
                    currentBoat.Add(lastMove);
                    
                    var nextPos = lastMove + boatDirection!.Value;
                    if (possibleSpots.Contains(nextPos))
                    {
                        move = nextPos;
                        goto commitMove;
                    }
                    else //Out of bounds
                    {
                        //We're out of bounds, so we need to turn around from the first hit point
                        boatDirection = new Int2(-boatDirection!.Value.X, -boatDirection.Value.Y);

                        move = currentBoat[0] + boatDirection!.Value;
                        goto commitMove;
                    }
                    break;
                case HitResult.Sunk:
                    //We sunk the boat, remove it's outline from the possible spots
                    var outline = Extensions.GetOutline(currentBoat);
                    foreach (var point in outline)
                    {
                        possibleSpots.Remove(point);
                    }

                    //Reset the boat
                    lastBoatHitPoint = null;
                    boatDirection = null;
                    currentBoat.Clear();

                    sunkSpots.UnionWith(currentBoat); //Debug
                    break;
                case HitResult.Miss:
                    //We missed, so the boat is not going to the right (or whatever direction we were going)
                    //If we already established a correct direction (we already have at least 2 boat pieces), we ran off the edge of the boat, so we need to turn around
                    //If we didn't establish a direction yet (we only have 1 boat piece), the boat is going another direction completely, 
                    if (currentBoat.Count >= 2)
                    {
                        //Return to the first hit point and shoot in the opposite direction
                        boatDirection = new Int2(-boatDirection!.Value.X, -boatDirection.Value.Y);


                        move = currentBoat[0] + boatDirection!.Value;
                        goto commitMove;
                    }
                    else //if (currentBoat.Count == 1)
                    {
                        //Rotate the direction 90 degrees counterclockwise
                        boatDirection = DirectionRotatedAndPossible(boatDirection!.Value, lastBoatHitPoint.Value);

                        //Shoot from last hit point in the new direction
                        move = lastBoatHitPoint.Value + boatDirection.Value;
                        goto commitMove;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //If the tile we hit last time was sunk, all tiles around it are water
        //This happens when we hit a size 1 boat
        if (lastHitResult == HitResult.Sunk)
        {
            //Remove spots around the sunk tile from possible spots
            sunkSpots.Add(lastMove);

            var neighbors = Extensions.GetNeighbors(lastMove);
            foreach (var neighbor in neighbors)
            {
                possibleSpots.Remove(neighbor);
            }
        }

        if (lastHitResult == HitResult.Hit)
        {
            hitSpots.Add(lastMove);

            //We hit a ship, so we need to find the direction of that boat and follow it
            lastBoatHitPoint = lastMove;
            currentBoat.Add(lastMove);

            //Assume boat direction
            //If there is a wall to the right, go down instead, etc.
            boatDirection = DirectionRotatedAndPossible(new Int2(1, 0), lastMove); //Start with right
            
            move = lastMove + boatDirection.Value;
            goto commitMove;
        }

        //No move was committed yet, so just continue with the next possible spot
        move = possibleSpots.First();
        goto commitMove;

        commitMove:
        if (!possibleSpots.Contains(move))
        {
            //Why are we shooting here?
            throw new Exception("Trying to shoot at a spot that is not possible");
        }
        possibleSpots.Remove(move);
        lastMove = move;
        return move;
    }
    private Int2 DirectionRotatedAndPossible(Int2 startDirection, Int2 fromPosition)
    {
        var direction = startDirection;
        while (true)
        {
            var nextMove = fromPosition + direction;
            if (!possibleSpots.Contains(nextMove))
            {
                direction = Extensions.Rotate90DegreesCounterclockwise(direction);

                if (direction == startDirection) //We went full circle
                {
                    //This shouldn't happen, because we should always have possible spots around
                    //Throw instead of crashing with an infinite while...
                    throw new Exception("Went full circle when assuming boat direction");
                }
                continue;
            }

            //It is a possible spot
            break;
        }

        return direction;
    }

    public void RespondHit()
    {
        hits[lastMove] = HitResult.Hit;
        hitSpots.Add(lastMove);
    }

    public void RespondSunk()
    {
        hits[lastMove] = HitResult.Sunk; //Override hit
        sunkSpots.Add(lastMove);
    }

    public void RespondMiss()
    {
        hits[lastMove] = HitResult.Miss;
    }

    private void DrawStrategyMap()
    {
        //Draw the map
        //Indicate which points are possible
        for (int y = 0; y < settings.Height; y++)
        {
            for (int x = 0; x < settings.Width; x++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(" ");

                var point = new Int2(x, y);
                if (sunkSpots.Contains(point))
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Write("S");
                    continue;
                }
                if (hitSpots.Contains(point))
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.Write("H");
                    continue;
                }

                if (possibleSpots.Contains(point))
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write("P");
                    continue;
                }
                
                //Impossible point
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write("x");
                    continue;
                }
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();
        }

        Console.WriteLine("-----\n");
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

        /// <returns>Points that are to the right, top, left, bottom in counterclockwise order starting from right</returns>
        public static Int2[] GetNeighborsOrthogonal(Int2 ofInt)
        {
            return new Int2[]
            {
                ofInt with {X = ofInt.X + 1},
                ofInt with {Y = ofInt.Y + 1},
                ofInt with {X = ofInt.X - 1},
                ofInt with {Y = ofInt.Y - 1},
            };
        }

        /// <returns>The outline of the contiguous points</returns>
        public static Int2[] GetOutline(ICollection<Int2> ofContiguousPoints)
        {
            var outline = new List<Int2>();
            foreach (var point in ofContiguousPoints)
            {
                var neighbors = GetNeighbors(point);
                foreach (var neighbor in neighbors)
                {
                    if (!ofContiguousPoints.Contains(neighbor))
                    {
                        outline.Add(neighbor);
                    }
                }
            }

            return outline.ToArray();
        }

        public static Int2 Rotate90DegreesCounterclockwise(Int2 ofInt)
        {
            return new Int2(-ofInt.Y, ofInt.X);
        }
    }
}
