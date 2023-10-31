namespace BattleShipEngine;

/// <summary>
/// The game of battleships
/// A Game exists for a board and multiple strategies can simulate it.
/// The rules for the board can be found here: https://www.codewars.com/kata/52bb6539a4cf1b12d90005b7
/// 
/// </summary>
public sealed class Game
{
    private readonly BoardTile[,] _boardTemplate;

    /// <summary>
    /// Creates a new game with the given positions. They are used only as a template and one Game can simulate multiple games.
    /// </summary>
    public Game(IBoardCreationStrategy boardCreationStrategy)
    {
        var boatPositions = boardCreationStrategy.GetBoatPositions();
        this._boardTemplate = GenerateBoardFromBoats(boatPositions);
    }

    /// <summary>
    /// Plays the game with on a board with a strategy. Calculates how many moves the strategy took to sink all boats.
    /// </summary>
    /// <returns> How many moves the strategy took to sink all boats. </returns>
    public int SimulateGame(IGameStrategy strategy)
    {
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

    private static BoardTile[,] GenerateBoardFromBoats(Int2[] boatPositions)
    {
        var board = new BoardTile[10, 10];
        foreach (var boatPosition in boatPositions)
        {
            board[boatPosition.X, boatPosition.Y] = BoardTile.Boat;
        }

        //Validate that the board is correct and throw if not

        return board;
    }

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