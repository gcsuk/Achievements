# Achievements

The achievements repository is a microservice designed to listen to Azure Service bus for "Achievement Unlocked" events, process the event (saves it to azure table storage database) and emits a message to listening clients via SignalR

The idea behind this service is to allow any web site to "fire-and-forget" an event and let this take care of storage and management, and telling users of the achievement.

There is also a management API for CRUD operations against Achievements and Unlocked Achievements

**This is a small learning project for using Service Bus and SignalR together in .NET5. It works, but it isn't optimised for production use. I will make changes over time but it is not "supported." Having said that, and PRs opened against it will be reviewed and merged if they add value.**

*Credit to [@davidfowl](https://github.com/davidfowl) for help when I was completely stuck getting the Service Bus handler to talk to SignalR* [(Stack Overflow)](https://stackoverflow.com/questions/52470647/how-to-access-signalr-connection-from-azure-service-bus-event-handler)

## Pre-requisites

- Visual Studio 2019 (16.8)
- .NET Core 5.0 Runtime (wherever you deploy it)
- Knowledge of .NET configuration

## Setup

- Clone the repository
- Create a service bus namespace in Azure
  - Note the Primary Connection String in `Shared access policies` > `RootManageSharedAccessKey`
  - Create a queue called `unlockedachievements`
- Create a storage account in Azure
    - Note the Connection String in `Access Keys`
    - Use Storage Explorer to add a table called `Achievements`
    - Use Storage Explorer to add a table called `AchievementsUnlocked`
- In a user secrets file, set the `TableStorage` and `Service Bus` connection strings - *do not fill in the entries in app.settings and commit these to your forked repo, they are there to be overridden by Azure Configuration transformations not development. Look here https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets for more info*
- In a CLI navigate to the wwwroot directory and run `npm install`
- Create a Web App in Azure
    - Add both connection string entries to the ConnectionStrings section of Application Settings
    - Ensure `Stack` is set to `.NET` and `.NET Framework Version` is set to `.NET 5` in `Configuration > General Settings`
    - Set `Web Sockets` to `On` in Application Settings

## Running

- Run the application.
- Navigate to index.html
  - You should see a `Start` button. Click that.
  - Open the console (F12) you should see that SignalR is connected.
- Open a new tab, navigate to /swagger
  - Create an achievement called "Learner" using the POST endpoint in Achievements section.
  - Open the Demo section and execute POST /demo/events/send.
  - Return to the other tab and in the console you should see the achievement details printed to the screen - this pops up then disappears so be quick!
  - Use the GET endpoint of the User Achievements section of Swagger to verify the achievement is in the database.

## Notes and future improvements

- UserId is currently hard coded to `Learner` in demo sender.
- Use the UserAchievements CRUD endpoints to remove Achievement "Learner" for User "1" in between calling the demo event sender, as otherwise it will think there is nothing new to add and nothing will happen.
