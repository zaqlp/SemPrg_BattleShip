# BattleShipEngine
This is the BattleShip guessing game engine to compete against other programmers.

## How to use

### How to run
To compile, .NET 7 is required.
When complete, run BattleShipAnalytics.

### New strategy
You can write Your strategy and put it to the BattleShipStrategies.
To make everything work properly, fulfill the requirements of GameStrategy/BoardCreationStrategy (look at the interfaces).
After adding the code, You can add a new competitor into the tournament in the file Program.cs in BattleShipAnalytics.

## Projects

### BattleShipEngine
This project contains the main part, the game with its rules and functioning.

### BattleShipAnalytics
This project contains Program, the class which starts everything.

### BattleShipStrategies
Here is the place for the strategies. They will compete against each other in the tournaments.

### BattleShipExternalStrategies
Everyone is welcome, so You can write Your strategy in some other language and run it externally.
This is the part to communicate with those other programs.
