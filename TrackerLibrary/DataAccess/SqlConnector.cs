using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class SqlConnector : IDataConnection
    {
        /// <summary>
        /// Saves a new prize to the database.
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public void CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Tournaments")))
            {
                var p = new DynamicParameters();
                p.Add("@PlaceNumber", model.PlaceNumber);
                p.Add("@PlaceName", model.PlaceName);
                p.Add("@PrizeAmount", model.PrizeAmount);
                p.Add("@PrizePercentage", model.PrizePercentage);
                p.Add("@id", 0, dbType: DbType.Int32, direction:ParameterDirection.Output);

                connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");
            }
        }
        /// <summary>
        /// Saves a new person to the database.
        /// </summary>
        /// <param name="model">Person information.</param>
        /// <returns>The person information, including unique identifier.</returns>
        public void CreatePerson(PersonModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Tournaments")))
            {
                var p = new DynamicParameters();
                p.Add("@FirstName", model.FirstName);
                p.Add("@LastName", model.LastName);
                p.Add("@EmailAddress", model.EmailAddress);
                p.Add("@CellphoneNumber", model.CellphoneNumber);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPeople_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");
            }
        }

        /// <summary>
        /// Saves a new team to the database.
        /// </summary>
        /// <param name="model">Team information.</param>
        /// <returns>Team information, including unique identifier.</returns>
        public void CreateTeam(TeamModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Tournaments")))
            {
                var p = new DynamicParameters();
                p.Add("@TeamName", model.TeamName);
                p.Add("id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTeam_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");

                foreach (PersonModel teamMember in model.TeamMembers)
                {
                    p = new DynamicParameters();
                    p.Add("@TeamId", model.Id);
                    p.Add("@PersonId", teamMember.Id);

                    connection.Execute("dbo.spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
                }
            }
        }

        /// <summary>
        /// Create new tournament to the database.
        /// </summary>
        /// <param name="model">Tournament information.</param>
        public void CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Tournaments")))
            {
                SaveTournament(connection, model);

                SaveTournamentPrizes(connection, model);

                SaveTournamentEntries(connection, model);

                SaveTournamentRounds(connection, model);

                TournamentLogic.UpdateTournamentResults(model);
            }
        }

        /// <summary>
        /// Saves rounds of tournament to the database.
        /// </summary>
        /// <param name="connection">Connection to the database.</param>
        /// <param name="model">Tournament information.</param>
        private void SaveTournamentRounds(IDbConnection connection, TournamentModel model)
        {
            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    SaveMatchup(connection, matchup, model);

                    foreach (MatchupEntryModel entry in matchup.Entries)
                    {
                        SaveEntryMantchup(connection, entry, matchup, model);
                    }
                }
            }
        }

        /// <summary>
        /// Save entry matchup to the database.
        /// </summary>
        /// <param name="connection">Current connection to the database.</param>
        /// <param name="entry">Entry matchup info.</param>
        /// <param name="matchup">Matchup info.</param>
        /// <param name="model">Tournament info.</param>
        private void SaveEntryMantchup(IDbConnection connection, MatchupEntryModel entry, MatchupModel matchup, TournamentModel model)
        {
            var p = new DynamicParameters();

            p.Add("@MatchupId", matchup.Id);
            if (entry.ParentMatchup == null)
            {
                p.Add("@ParentMatchupId", null);
            }
            else
            {
                p.Add("@ParentMatchupId", entry.ParentMatchup.Id);
            }
            if (entry.TeamCompeting == null)
            {
                p.Add("@TeamCompetingId", null);
            }
            else
            {
                p.Add("@TeamCompetingId", entry.TeamCompeting.Id);
            }
            p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("dbo.spMatchupEntries_Insert", p, commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Save matchup to the database.
        /// </summary>
        /// <param name="connection">Current connection to the database.</param>
        /// <param name="matchup">Matchup info.</param>
        /// <param name="model">Tournament info.</param>
        private void SaveMatchup(IDbConnection connection, MatchupModel matchup, TournamentModel model)
        {
            var p = new DynamicParameters();

            p.Add("@MatchupRound", matchup.MatchupRound);
            p.Add("@TournamentId", model.Id);
            p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("dbo.spMatchups_Insert", p, commandType: CommandType.StoredProcedure);

            matchup.Id = p.Get<int>("@id");
        }

        /// <summary>
        /// Saves tournament to the database.
        /// </summary>
        /// <param name="connection">Connection to the database.</param>
        /// <param name="model">Tournament information.</param>
        private void SaveTournament(IDbConnection connection, TournamentModel model)
        {
            var p = new DynamicParameters();
            p.Add("@TournamentName", model.TournamentName);
            p.Add("@EntryFee", model.EntryFee);
            p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("dbo.spTournaments_Insert", p, commandType: CommandType.StoredProcedure);
            model.Id = p.Get<int>("@id");
        }

        /// <summary>
        /// Saves prizes to the tournament table.
        /// </summary>
        /// <param name="connection">Connection to the database.</param>
        /// <param name="model">Tournament information.</param>
        private void SaveTournamentPrizes(IDbConnection connection, TournamentModel model)
        {
            foreach (var pz in model.Prizes)
            {
                var p = new DynamicParameters();
                p.Add("@TournamentId", model.Id);
                p.Add("@PrizeId", pz.Id);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentPrizes_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Saves teams that entered to the tournament.
        /// </summary>
        /// <param name="connection">Connection to the database.</param>
        /// <param name="model">Tournament information.</param>
        private void SaveTournamentEntries(IDbConnection connection, TournamentModel model)
        {
            foreach (var tm in model.EnteredTeams)
            {
                var p = new DynamicParameters();
                p.Add("@TournamentId", model.Id);
                p.Add("@TeamId", tm.Id);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentEntries_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Getting all teams from database.
        /// </summary>
        /// <returns>Information about all teams.</returns>
        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Tournaments")))
            {
                output = connection.Query<TeamModel>("dbo.spTeam_GetAll").ToList();


                foreach (var team in output)
                {
                    var p = new DynamicParameters();
                    p.Add("@TeamId", team.Id);
                    team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                }
            }

            return output;
        }

        /// <summary>
        /// Getting all people from database.
        /// </summary>
        /// <returns>Information about all people.</returns>
        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Tournaments")))
            {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
            }

            return output;
        }

        /// <summary>
        /// Getting all tournaments from database.
        /// </summary>
        /// <returns>Information about tournaments.</returns>
        public List<TournamentModel> GetTournament_All()
        {
            List<TournamentModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Tournaments")))
            {
                output = connection.Query<TournamentModel>("dbo.spTournaments_GetAll").ToList();

                foreach (TournamentModel t in output)
                {
                    //Populate prizes.
                    PopulatePrizes(connection, t);

                    //Populate teams.
                    PopulateTeams(connection, t);

                    foreach (TeamModel team in t.EnteredTeams)
                    {
                        //Populate team with their members for this tournament.
                        PopulateTeamWithMembers(connection, team);
                    }

                    //Populate rounds.
                    PopulateRounds(connection, t);
                }
            }

            return output;
        }

        /// <summary>
        /// Populate tournament with rounds.
        /// </summary>
        /// <param name="connection">Current connection to the database.</param>
        /// <param name="tournament">Tournament info.</param>
        private void PopulateRounds(IDbConnection connection, TournamentModel tournament)
        {
            var p = new DynamicParameters();
            p.Add("@TournamentId", tournament.Id);

            List<MatchupModel> matchups = connection.Query<MatchupModel>("dbo.spMatchups_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

            foreach (MatchupModel m in matchups)
            {
                p = new DynamicParameters();
                p.Add("@MatchupId", m.Id);
                m.Entries = connection.Query<MatchupEntryModel>("dbo.spMatchupEntries_GetByMatchup", p, commandType: CommandType.StoredProcedure).ToList();

                //Populate each matchup.
                List<TeamModel> allTeams = GetTeam_All();

                if (m.WinnerId > 0)
                    m.Winner = allTeams.First(x => x.Id == m.WinnerId);

                foreach (var me in m.Entries)
                {
                    if (me.TeamCompetingId > 0)
                        me.TeamCompeting = allTeams.First(x => x.Id == me.TeamCompetingId);

                    if (me.ParentMatchupId > 0)
                        me.ParentMatchup = matchups.FirstOrDefault(x => x.Id == me.ParentMatchupId);
                }
            }

            List<MatchupModel> currRow = new List<MatchupModel>();
            int currRound = 1;

            foreach (MatchupModel m in matchups)
            {
                if (m.MatchupRound > currRound)
                {
                    tournament.Rounds.Add(currRow);
                    currRow = new List<MatchupModel>();
                    currRound += 1;
                }

                currRow.Add(m);
            }

            tournament.Rounds.Add(currRow);
        }

        /// <summary>
        /// Populates team with members.
        /// </summary>
        /// <param name="connection">Current connection to the database.</param>
        /// <param name="team">Team info.</param>
        /// <returns>List of people for current team.</returns>
        private void PopulateTeamWithMembers(IDbConnection connection, TeamModel team)
        {
            var p = new DynamicParameters();
            p.Add("@TeamId", team.Id);
            team.TeamMembers = connection
                .Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure)
                .ToList();
        }

        /// <summary>
        /// Populates tournament with teams.
        /// </summary>
        /// <param name="connection">Current connection to the database.</param>
        /// <param name="tournament">Tournament info.</param>
        private void PopulateTeams(IDbConnection connection, TournamentModel tournament)
        {
            var p = new DynamicParameters();
            p.Add("@TournamentId", tournament.Id);
            tournament.EnteredTeams = connection
                .Query<TeamModel>("dbo.spTeam_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();
        }

        /// <summary>
        /// Populates tournament with prizes.
        /// </summary>
        /// <param name="connection">Current connection to the database.</param>
        /// <param name="tournament">Tournament info.</param>
        private void PopulatePrizes(IDbConnection connection, TournamentModel tournament)
        {
            var p = new DynamicParameters();
            p.Add("@TournamentId", tournament.Id);
            tournament.Prizes = connection
                .Query<PrizeModel>("dbo.spPrizes_GetByTournament", p, commandType: CommandType.StoredProcedure)
                .ToList();
        }

        /// <summary>
        /// Updating information about matchup in database.
        /// </summary>
        /// <param name="model">Matchup information.</param>
        public void UpdateMatchup(MatchupModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Tournaments")))
            {
                var p = new DynamicParameters();
                if (model.Winner != null)
                {
                    p.Add("@id", model.Id);
                    p.Add("@WinnerId", model.Winner.Id);

                    connection.Execute("dbo.spMatchups_Update", p, commandType: CommandType.StoredProcedure);
                }

                foreach (var me in model.Entries)
                {
                    if (me.TeamCompeting != null)
                    {
                        p = new DynamicParameters();
                        p.Add("@id", me.Id);
                        p.Add("@TeamCompetingId", me.TeamCompeting.Id);
                        p.Add("@Score", me.Score);

                        connection.Execute("dbo.spMatchupEntries_Update", p, commandType: CommandType.StoredProcedure); 
                    }
                }
            }
        }
    }
}
