# UNO-esque card game

This is an UNO-like multiplayer card game with 2-10 players with a few custom configurable rules. Each player can be either a human or an "AI".
The game has configurable saving, either as .json in the file system or SQLite database, this can be configured
in the Program.cs file in ConsoleApp and WebApp directories.

## Installation
Requirements: .NET SDK
* Clone this repository
* Execute the command "dotnet run" from inside the WebApp directory in the command prompt to play the game via web browser
   * Navigate to https://localhost:5223/ in your favourite web browser
* Execute the command "dotnet run" from inside the ConsoleApp directory in the command prompt to play the game via command line

## Technologies used
* ASP.NET Core
* Entity Framework Core
* Razor pages
* SQLite

## Author
* [Robert King](https://github.com/string-king)
