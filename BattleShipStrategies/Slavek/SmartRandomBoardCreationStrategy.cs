using BattleShipEngine;

namespace BattleShipStrategies.Slavek;

public class SmartRandomBoardCreationStrategy : IBoardCreationStrategy
{
    public Int2[] GetBoatPositions(GameSetting setting)
    {
        while (true)
        {
            List<Int2> boats = new List<Int2>();
            for (int shipLength = setting.BoatCount.Length; shipLength > 0; shipLength--)
            {
                int timesTried = 0;
                for (int shipsPlaced = 0; shipsPlaced < setting.BoatCount[shipLength - 1];
                     timesTried++)
                {
                    bool valid = true;
                    Int2 boatStart = new Int2(
                        Random.Shared.Next(setting.Width),
                        Random.Shared.Next(setting.Height)
                    );
                    int direction = Random.Shared.Next(4);
                    for (int shipPart = 0; shipPart < shipLength; shipPart++)
                    {
                        Int2 nextSquare;
                        switch (direction)
                        {
                            case 0:
                                nextSquare = boatStart with { X = boatStart.X - shipPart };
                                break;
                            case 1:
                                nextSquare = boatStart with { Y = boatStart.Y - shipPart };
                                break;
                            case 2:
                                nextSquare = boatStart with { X = boatStart.X + shipPart };
                                break;
                            case 3:
                                nextSquare = boatStart with { Y = boatStart.Y + shipPart };
                                break;
                            default:
                                nextSquare = boatStart;
                                break;
                        }
                        if (nextSquare.X < 0 || nextSquare.X >= setting.Width ||
                            nextSquare.Y < 0 || nextSquare.Y >= setting.Height)
                        {
                            valid = false;
                            break;
                        }
                        for (int x = -1; x < 2; x++)
                            for (int y = -1; y < 2; y++)
                                if (boats.Contains(
                                        new Int2(nextSquare.X + x, nextSquare.Y + y)))
                                {
                                    valid = false;
                                    goto CheckValidity;
                                }
                    }
                    CheckValidity:
                    if (valid)
                    {
                        for (int shipPart = 0; shipPart < shipLength; shipPart++)
                        {
                            Int2 nextSquare;
                            switch (direction){
                                case 0:
                                    nextSquare = boatStart with { X = boatStart.X - shipPart };
                                    break;
                                case 1:
                                    nextSquare = boatStart with { Y = boatStart.Y - shipPart };
                                    break;
                                case 2:
                                    nextSquare = boatStart with { X = boatStart.X + shipPart };
                                    break;
                                case 3:
                                    nextSquare = boatStart with { Y = boatStart.Y + shipPart };
                                    break;
                                default:
                                    nextSquare = boatStart;
                                    break;
                            }
                            boats.Add(nextSquare);
                        }
                        shipsPlaced++;
                        timesTried = 0;
                    }
                    else if (timesTried > 1000)
                        goto NotUsableBoard;
                }
            }

            BoardTile[,] board = new BoardTile[setting.Width, setting.Height];
            foreach (var boat in boats)
            {
                board[boat.X, boat.Y] = BoardTile.Boat;
            }
            if (ValidateBoard(board, setting))
                return boats.ToArray();
            NotUsableBoard: continue;
        }
    }
    
#region Board Validation

    /// <summary>
    /// Checks whether is a board valid (depending on the rules) or not.
    /// </summary>
    private static bool ValidateBoard(BoardTile[,] board, GameSetting setting)
    {
        if (board.GetLength(0) != setting.Width ||
            board.GetLength(1) != setting.Height)
            return false;
        int[] ships = setting.BoatCount;
        int[] shipsFound = new int[ships.Length];
        for (int i = 0; i < ships.Length; i++)
            shipsFound[i] = 0;
        for (int i = 0; i < setting.Width; i++)
        {
            for (int j = 0; j < setting.Height; j++)
            {
                if (board[i,j] == BoardTile.Boat)
                {
                    int boatLength = GetBoatLength(board, new Int2(i, j), setting);
                    if (boatLength > ships.Length)
                        return false;
                    if (boatLength == -1)
                        return false;
                    shipsFound[boatLength-1] ++; // There aren't any boats with length 0.
                }
            }
        }
        
        for (int i = 0; i < ships.Length; i++)
        {
            if (shipsFound[i] != ships[i] * (i + 1)) // Longer ships are counted several times.
                return false;
        }
        
        return true;
    }

    private static int GetBoatLength(BoardTile[,] board, Int2 position, GameSetting setting)
    {
        int x = 0;
        int y = 0;
        for (int direction = 0; direction < 4; direction++)
        {
            int test = GetDirectionLength(board, position, direction, 0, setting);
            if (test == -1) // -1 means invalid ship shape.
                return -1;
            if (direction % 2 == 0)
                x += test;
            else
                y += test;
        }
        if (x > 0 && y > 0) // Boat should go in one direction only.
            return -1;
        return Math.Max(x, y) + 1;
    }
    
    private static int GetDirectionLength(BoardTile[,] board, Int2 position,
        int direction, int length, GameSetting setting)
    {
        // Checks if there is another boat touching this tile by the edge.
        if (direction % 2 == 0 && length > 0)
        {
            if (position.Y != 0 && board[position.X, position.Y-1] == BoardTile.Boat)
                return -1;
            if (position.Y != setting.Height-1 && board[position.X, position.Y+1] == BoardTile.Boat)
                return -1;
        }
        else if (length > 0)
        {
            if (position.X != 0 && board[position.X-1, position.Y] == BoardTile.Boat)
                return -1;
            if (position.X != setting.Width-1 && board[position.X+1, position.Y] == BoardTile.Boat)
                return -1;
        }
        if (direction == 0)
        {
            if (position.X == 0)
                return length;
            if (board[position.X-1,position.Y] == BoardTile.Boat)
                return GetDirectionLength(board, new Int2(position.X - 1, position.Y),
                    direction, length + 1, setting);
        }
        else if (direction == 1)
        {
            if (position.Y == 0)
                return length;
            if (board[position.X,position.Y-1] == BoardTile.Boat)
                return GetDirectionLength(board, new Int2(position.X, position.Y - 1),
                    direction, length + 1, setting);
        }
        else if (direction == 2)
        {
            if (position.X == setting.Width-1)
                return length;
            if (board[position.X+1,position.Y] == BoardTile.Boat)
                return GetDirectionLength(board, new Int2(position.X+1, position.Y),
                    direction, length + 1, setting);
        }
        else if (direction == 3)
        {
            if (position.Y == setting.Height-1) return length;
            if (board[position.X,position.Y+1] == BoardTile.Boat)
                return GetDirectionLength(board, new Int2(position.X, position.Y+1),
                    direction, length + 1, setting);
        }
        else return -1;
        // If we got here, it means the boat ends, so we also need to check the corners.
        if (direction % 2 == 0 && length > 0)
        {
            if (position.Y != 0 && board[position.X+direction-1, position.Y-1] == BoardTile.Boat)
                return -1;
            if (position.Y != setting.Height-1 && board[position.X+direction-1, position.Y+1]
                == BoardTile.Boat)
                return -1;
        }
        else if (length > 0)
        {
            if (position.X != 0 && board[position.X-1, position.Y+direction-2] == BoardTile.Boat)
                return -1;
            if (position.X != setting.Width-1 && board[position.X+1, position.Y+direction-2]
                == BoardTile.Boat)
                return -1;
        }

        // Everything was OK, this is the end of the ship.
        return length;
    }

#endregion

}