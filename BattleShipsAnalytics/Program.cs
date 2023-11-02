using BattleShipEngine;
using BattleShipStrategies.Default;
using BattleShipStrategies.Slavek;


var participants = new List<Participant>()
{
    new("Default1", new DefaultBoardCreationStrategy(), new DefaultGameStrategy()),
    new("Default2", new DefaultBoardCreationStrategy(), new DefaultGameStrategy()),
    new("RandomBySlavek", new DefaultBoardCreationStrategy(), new SmartRandomStrategy())
};

var competitorsScores = new Dictionary<Participant, int>(); // Key: Participant, Value: How many moves it took to sink all boats
foreach (var participant in participants)
    competitorsScores.Add(participant, 0);
foreach (var participant in participants)
{
    var game = new Game(participant.BoardCreationStrategy, new GameSetting(
        10, 10, new [] {4, 3, 2, 1}));
    
    Console.WriteLine($"The Board of {participant.Name} was cleared by ");

    //Calculate how others did against this board
    foreach (var competitor in participants)
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
Console.WriteLine("Total amount of moves needed to solve all the opponents' boards:");
foreach (var participant in
         competitorsScores.OrderBy(x => x.Value))
{
    Console.WriteLine($"\t-{participant.Key.Name}: {participant.Value} moves");
}