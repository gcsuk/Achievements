# Achievements

The achievements repository is a microservice designed to listen to Azure Service bus for "Achievement Unlocked Events", process the event (saves it to the database) and emits a message to listening clients via SignalR

The idea behind this service is to allow any web site to "fire-and-forget" an event and let this take care of storage and management, and of telling users of the achievement.

There is also a management API for CRUD operations against Achievements and User Achievements

**This is a small learning project for using Service Bus and SignalR together in .NET Core. It works, but it isn't optimised for production use. I will make changes over time but it is not "supported." Having said that, and PRs opened against it will be reviewed and merged if they add value.**

## Pre-requisites

- Visual Studio 2017 Update 5+
- .NET Core 2.1.4 Runtime
- Knowledge of .NET Core configuration

## Setup

- Clone the repository
- Create a service bus namespace in Azure
  - Note the Primary Connection String in `Shared access policies` > `RootManageSharedAccessKey`
  - Create a queue called `unlockedachievements`
- Create a database in Azure
    - Note the Connection String in `Connection Strings`
    - Create a database user and note the username and password
- Run SQL scripts in `DatabaseCreationScripts.sql` in the repo root to create the database tables
- In appsettings.json set the Database and Service Bus connection strings - *do not commit these to your forked repo*
- Create a Web App in Azure
    - Add both connection string entries to the ConnectionStrings section of Application Settings
    - Set `Web Sockets` to `On` in Application Settings

## Running

- Run the application.
- Navigate to index.html
  - You should see a `Start` button. Click that.
  - Open the console (F12) you should see that SignalR is connected
- Open a new tab, navigate to /swagger/ui
  - Create an achievement using the POST endpoint in Achievements section.
  - Open the Demo section and executre POST /demo/events/send
  - Return to the other tab and in the console you should see an Achievement Unlocked message in the console.
  - Use the GET endpoint of the User Achievements section of Swager to verify the achievement is in the database

## Notes and future improvements

- UserId is currently hard coded to `1` in demo sender
- Use the UserAchievements CRUD endpoints to remove Achievement ID 1 for User ID 1 in between calling the demo event sender, as otherwise it will attempt to insert a duplicate database entry and fall over
