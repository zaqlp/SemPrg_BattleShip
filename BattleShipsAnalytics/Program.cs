using System.Runtime.CompilerServices;
using BattleShipEngine;
using BattleShipStrategies.Default;


var participants = new List<Participant>()
{
    new("Default", new DefaultBoardCreationStrategy(), new DefaultGameStrategy())
};

foreach (var participant in participants)
{
    var game = new Game(participant.BoardCreationStrategy);

    //Calculate how others did against this board
    var competitorsScores = new Dictionary<Participant, int>(); // Key: Participant, Value: How many moves it took to sink all boats
    foreach (var competitor in participants)
    {
        if (competitor == participant)
            continue; //The player shouldn't play against himself

        //Simulate game on this board
        var ammOfMoves = game.SimulateGame(competitor.GameStrategy);

        competitorsScores.Add(competitor, ammOfMoves);
    }

    //Some logging for this board
    Console.WriteLine($"The Board of {participant.Name} was cleared by ");
    foreach (var competitor in competitorsScores)
    {
        Console.WriteLine($"\t-{competitor.Key.Name} in {competitor.Value} moves");
    }
}