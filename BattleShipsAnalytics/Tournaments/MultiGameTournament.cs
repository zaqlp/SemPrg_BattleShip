using BattleShipEngine;

namespace BattleShipsAnalytics.Tournaments;

internal class MultiGameTournament : ITournament
{
    private readonly List<Participant> _participants;
    private readonly int _gamesPerBoard;
    public MultiGameTournament(List<Participant> participants, int gamesPerBoard)
    {
        _participants = participants;
        _gamesPerBoard = gamesPerBoard;
    }

    public void PlayAndPrint(GameSetting settings)
    {
        //Setup scores
        var competitorsScores = new Dictionary<Participant, int>(); // Key: Participant, Value: How many moves it took to sink all boats
        foreach (var participant in _participants)
        {
            competitorsScores.Add(participant, 0);
        }

        foreach (var participant in _participants)
        {
            Console.WriteLine($"The {_gamesPerBoard} boards of {participant.Name}:");

            //Calculate how others did against this board
            foreach (var competitor in _participants)
            {
                if (competitor == participant)
                    continue; //The player shouldn't play against himself

                var currentCompetitorTotalMoves = 0;
                //Simulate games on this board
                for (int i = 0; i < _gamesPerBoard; i++)
                {
                    //Assume boards are valid (lol) (it's faster)
                    var game = new Game(participant.BoardCreationStrategy, settings);

                    var ammOfMoves = game.SimulateGame(competitor.GameStrategy);
                    competitorsScores[competitor] += ammOfMoves;
                    currentCompetitorTotalMoves += ammOfMoves;
                }

                Console.WriteLine($"\t-{competitor.Name}: {currentCompetitorTotalMoves} moves - avg: {currentCompetitorTotalMoves / (double)_gamesPerBoard}");
            }

            Console.WriteLine();
        }

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
            var avg = participant.Value / (double)_gamesPerBoard / (_participants.Count-1); // -1 because we don't count the participant himself
            Console.WriteLine(
                $"{participant.Key.Name,-nameWidth}|{participant.Value,-totalWidth}|{avg}");
        }
    }
}