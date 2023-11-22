using BattleShipEngine;
using BattleShipStrategies.Default;
using BattleShipStrategies.MartinF;
using BattleShipStrategies.Slavek;


var participants = new List<Participant>()
{
    //new("Default", new DefaultBoardCreationStrategy(), new DefaultGameStrategy()),
    new("MartinStrategy", new DefaultBoardCreationStrategy(), new MartinStrategy()),
    new("SmartRandom", new SmartRandomBoardCreationStrategy(), new SmartRandomStrategy()),
    new("Slavek", new SmartRandomBoardCreationStrategy(), new DeathCrossStrategy()),
    //new("Interactive", new InteractiveBoardCreationStrategy(), new InteractiveGameStrategy())
};

//Setup scores
var competitorsScores = new Dictionary<Participant, int>(); // Key: Participant, Value: How many moves it took to sink all boats
foreach (var participant in participants)
    competitorsScores.Add(participant, 0);

//Play games
var settings = GameSetting.Default;

foreach (var participant in participants)
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