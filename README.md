# TournamentTracker
Tournament Tracker is a desktop application created in .NET with a view in WinForms and two way data storage. The appliaction handles storage in text files (CSV files) or an SQL database (using Dapper). TT was created for learning purposes.
Applications meet all conditions that I put up at the beginning:

- Two way storage for data;
- Creating new tournaments, teams, players and pool prizes;
- Loading unfinished tournaments, teams and players;
- Tracking current matchups, adding scores.

# Possible improvements
Add (any) graphic design would be nice. Buttons like "Edit Player", "Close Tournament without Winner", "Edit Team" would be nice, but i moved on to the next project.

# Instalation tips
### SQL
Use databaseScript.sql to re-create the database with default tournaments for TT and edit connectionStrings section in App.config providing your database name. Check DatabaseType in Program.cs if everything is correct you can use SQL database storage for your tournaments.

### TEXT
Check DatabaseType in Program.cs and if needed change from SQL to TextFile. Default .csv files in repo provide default tournament for tests.
