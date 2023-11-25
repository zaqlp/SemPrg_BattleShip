using BattleShipEngine;
using BattleShipsAnalytics.Tournaments;
using BattleShipStrategies.MartinF;
using BattleShipStrategies.Slavek;


var participants = new List<Participant>()
{
    new("MartinStrategy", new SmartRandomBoardCreationStrategy(), new MartinStrategy()),
    new("SmartRandom", new SmartRandomBoardCreationStrategy(), new SmartRandomStrategy()),
    new("Slavek", new SmartRandomBoardCreationStrategy(), new DeathCrossStrategy()),
    //new("Interactive", new InteractiveBoardCreationStrategy(), new InteractiveGameStrategy())
};

var settings = GameSetting.Default;

var tournament = new SingleShotTournament(participants);
//var tournament = new MultiGameTournament(participants, 1000);
//var tournament = new MultiThreadedTournament(participants, 1000); //Might be faster, but not sure (lol)

tournament.PlayAndPrint(settings);
