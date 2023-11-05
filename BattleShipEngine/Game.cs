namespace BattleShipEngine;

/// <summary>
/// The game of battleships
/// A Game exists for a board and multiple strategies can simulate it.
/// </summary>
public sealed class Game
{
    private readonly BoardTile[,] _boardTemplate;
    private readonly GameSetting _setting;

    /// <summary>
    /// Creates a new game with the given positions. They are used only as a template and one Game can simulate multiple games.
    /// </summary>
    public Game(IBoardCreationStrategy boardCreationStrategy, GameSetting setting)
    {
        var boatPositions = boardCreationStrategy.GetBoatPositions(setting);
        this._boardTemplate = GenerateBoardFromBoats(boatPositions, setting);
        this._setting = setting;
    }

    /// <summary>
    /// Plays the game with on a board with a strategy. Calculates how many moves the strategy took to sink all boats.
    /// </summary>
    /// <returns> How many moves the strategy took to sink all boats. </returns>
    public int SimulateGame(IGameStrategy strategy)
    {
        strategy.Start(_setting);
        //Copy the board, so this can be reused
        var board = (BoardTile[,])this._boardTemplate.Clone();

        //Todo: optimize with span
        var ammOfMoves = 0;
        while (true)
        {
            ammOfMoves++;

            var move = strategy.GetMove();
            if (board[move.X, move.Y] == BoardTile.Boat)
            {
                //Player hit a boat
                board[move.X, move.Y] = BoardTile.DamagedBoat;
                strategy.RespondHit();

                //Check if the boat was sunk (all tiles are hit)
                if (BoatIsDead(board, move, Direction.All, _setting))
                {
                    strategy.RespondSunk();
                }
            }
            else
            {
                strategy.RespondMiss();
            }

            if (IsGameOver(board))
                break;
        }

        return ammOfMoves;
    }

    private static bool BoatIsDead(BoardTile[,] board, Int2 position,
        Direction direction, GameSetting setting)
    {
        if (direction == Direction.All)
        {
            bool result = true;
            for (int i = 0; i < 4; i++)
                result = result && BoatIsDead(board, position, (Direction) i, setting);
            return result;
        }
        if (board[position.X, position.Y] == BoardTile.Boat)
            return false;
        if (board[position.X, position.Y] == BoardTile.Water)
            return true;
        if ((direction == Direction.Left && position.X != 0 && !BoatIsDead(
                board, position with {X = position.X - 1}, direction, setting)) ||
            (direction == Direction.Up && position.Y != 0 && !BoatIsDead(
                board, position with {Y = position.Y - 1}, direction, setting)) ||
            (direction == Direction.Right && position.X != setting.Width - 1 && !BoatIsDead(
                board, position with {X = position.X + 1}, direction, setting)) ||
            (direction == Direction.Down && position.Y != setting.Height - 1 && !BoatIsDead(
                board, position with {Y = position.Y + 1}, direction, setting)))
            return false;
        return true;
    }

    private static BoardTile[,] GenerateBoardFromBoats(Int2[] boatPositions, GameSetting setting)
    {
        var board = new BoardTile[setting.Width, setting.Height];
        foreach (var boatPosition in boatPositions)
        {
            board[boatPosition.X, boatPosition.Y] = BoardTile.Boat;
        }

        //Validate that the board is correct and throw if not
        if (!ValidateBoard(board, setting))
            throw new Exception("The board is not valid!");

        return board;
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

    private static bool IsGameOver(BoardTile[,] board)
    {
        foreach (var tile in board)
        {
            if (tile == BoardTile.Boat)
                return false;
        }
        return true;
    }
}