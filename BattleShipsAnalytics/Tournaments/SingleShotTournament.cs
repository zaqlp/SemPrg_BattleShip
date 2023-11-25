using BattleShipEngine;

namespace BattleShipsAnalytics.Tournaments;

public class SingleShotTournament : ITournament
{
    private readonly List<Participant> _participants;
    public SingleShotTournament(List<Participant> participants)
    {
        _participants = participants;
    }

    public void PlayAndPrint(GameSetting settings)
    {
        //Setup scores
        var competitorsScores = new Dictionary<Participant, int>(); // Key: Participant, Value: How many moves it took to sink all boats
        foreach (var participant in _participants)
            competitorsScores.Add(participant, 0);

        foreach (var participant in _participants)
        {
            Game game;
            try
            {
                game = new Game(participant.BoardCreationStrategy, settings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine($"The {participant.Name}'s board was not valid. Skipping.");
                continue;
            }

            Console.WriteLine($"The Board of {participant.Name} was cleared by ");

            //Calculate how others did against this board
            foreach (var competitor in _participants)
            {
                if (competitor == participant)
                    continue; //The player shouldn't play against himself

                //Simulate game on this board
                var ammOfMoves = game.SimulateGame(competitor.GameStrategy);

                competitorsScores[competitor] += ammOfMoves;

                //Some logging for this board
                Console.WriteLine($"\t-{competitor.Name} in {ammOfMoves} moves");
            }
        }

        //Final results
        DrawResultTable(competitorsScores);
    }

    private void DrawResultTable(Dictionary<Participant, int> competitorsScores)
    {
        //Final results
        //Draw it as a table with -+| and stuff
        const int nameWidth = 20;
        const int totalWidth = 20;

        Console.WriteLine("\nTotal amount of moves needed to solve all the opponents' boards:");
        Console.WriteLine($"{"Name",-nameWidth}|{"Total",-totalWidth}");
        Console.WriteLine($"{"".PadRight(nameWidth, '-')}+{"".PadRight(totalWidth, '-')}");
        foreach (var participant in
                 competitorsScores.OrderBy(x => x.Value))
        {
            Console.WriteLine(
                $"{participant.Key.Name,-nameWidth}|{participant.Value,-totalWidth}");
        }
    }
}