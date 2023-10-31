namespace BattleShipEngine;

public readonly record struct Participant(
    string Name,
    IBoardCreationStrategy BoardCreationStrategy,
    IGameStrategy GameStrategy);