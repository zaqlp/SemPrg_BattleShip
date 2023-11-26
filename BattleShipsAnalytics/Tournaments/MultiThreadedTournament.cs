using System.Diagnostics;
using BattleShipEngine;

namespace BattleShipsAnalytics.Tournaments;

public class MultiThreadedTournament : ITournament
{
    private readonly List<Participant> _participants;
    private readonly int _gamesPerBoard;
    public MultiThreadedTournament(List<Participant> participants, int gamesPerBoard)
    {
        _participants = participants;
        _gamesPerBoard = gamesPerBoard;
    }

    public void PlayAndPrint(GameSetting settings)
    {
        Console.WriteLine("Waiting for all games to finish...");

        Task.Run(() => PlayAndPrintAsync(settings)).Wait();
    }

    private async Task PlayAndPrintAsync(GameSetting settings)
    {
        //Setup scores
        var competitorsScores = new Dictionary<Participant, int>(); // Key: Participant, Value: How many moves it took to sink all boats
        foreach (var participant in _participants)
        {
            competitorsScores.Add(participant, 0);
        }

        var tasks = new Task[_participants.Count];
        for (var i = 0; i < _participants.Count; i++)
        {
            var participant = _participants[i];
            var task = Task.Run(() =>
            {
                //Calculate how others did against this board
                foreach (var competitor in _participants)
                {
                    if (competitor == participant)
                        continue; //The player shouldn't play against himself

                    var strategyCopy = Activator.CreateInstance(competitor.GameStrategy.GetType()) as IGameStrategy;

                    //Simulate games on this board
                    for (int gI = 0; gI < _gamesPerBoard; gI++)
                    {
                        //Assume boards are valid (lol) (it's faster)
                        var game = new Game(participant.BoardCreationStrategy, settings);

                        Debug.Assert(strategyCopy != null, nameof(strategyCopy) + " != null");
                        var ammOfMoves = game.SimulateGame(strategyCopy);
                        competitorsScores[competitor] += ammOfMoves;
                    }
                }
            });
            tasks[i] = task;
        }

        await Task.WhenAll(tasks);

        DrawResultTable(competitorsScores);
    }

    private void DrawResultTable(Dictionary<Participant, int> competitorsScores)
    {
        //Final results
        //Draw it as a table with -+| and stuff
        const int nameWidth = 20;
        const int avgWidth = 10;
        const int totalWidth = 20;

        Console.WriteLine("\nTotal amount of moves needed to solve all the opponents' boards:");
        Console.WriteLine($"{"Name",-nameWidth}|{"Total",-totalWidth}|{"Avg",-avgWidth}");
        Console.WriteLine($"{"".PadRight(nameWidth, '-')}+{"".PadRight(totalWidth, '-')}+{"".PadRight(avgWidth, '-')}");
        foreach (var participant in
                 competitorsScores.OrderBy(x => x.Value))
        {
            var avg = participant.Value / (double)_gamesPerBoard / (_participants.Count - 1); // -1 because we don't count the participant himself
            Console.WriteLine(
                $"{participant.Key.Name,-nameWidth}|{participant.Value,-totalWidth}|{avg}");
        }
    }
}