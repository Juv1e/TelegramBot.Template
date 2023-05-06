# Telegram Bot Template

This is a template for creating a modular Telegram bot using C# and the Telegram.Bot library.

## Adding a Command Module

To add a new command module, follow these steps:

1. Right-click on the `Commands` folder in the solution explorer and select "Add" > "Class"
2. Name the class using PascalCase and appending "Command" at the end of the name; for example: `ExampleCommand`
3. Implement the `ICommandModule` interface in the class
4. Add the command you would like to handle in the `Command` property of your command module class
5. Add a description of the command in the `Description` property of your command module class
6. Write the code for handling the command in the `HandleCommandAsync` method of your command module class
7. Build and run the application

Remember to remove or modify the example commands when creating your own command modules.
