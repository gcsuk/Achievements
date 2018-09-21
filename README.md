# Achievements

## Setup

- Create a service bus namespace in Azure and note the Primary Connection String
- Create a queue called `unlockedachievements`
- Create a database in Azure and note the Connection String
- Run SQL scripts in `DatabaseCreationScripts.sql` in the repo root to create the database tables
- In appsettings.json set the Database and Service Bus connection strings