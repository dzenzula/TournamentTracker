using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public static class TournamentLogic
    {
        public static string winnerTeam;
        public static string runnerUpTeam;
        public static string tournamentName;
        public static string prizeWinner;
        public static string prizeRunnerUp;
        /// <summary>
        /// Create rounds logic.
        /// </summary>
        /// <param name="model">Tournament info.</param>
        public static void CreateRounds(TournamentModel model)
        {
            List<TeamModel> randomizedTeams = RandomizeTeamOrder(model.EnteredTeams);
            int rounds = FindNumberOfRounds(randomizedTeams.Count);
            int byes = NumberOfByes(rounds, randomizedTeams.Count);

            model.Rounds.Add(CreateFirstRound(byes, randomizedTeams));

            CreateOtherRounds(model, rounds);
        }

        /// <summary>
        /// Updates information about tournament.
        /// </summary>
        /// <param name="model"></param>
        public static void UpdateTournamentResults(TournamentModel model)
        {
            int startingRound = model.CheckCurrentRound();
            List<MatchupModel> toScore = new List<MatchupModel>();

            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel rm in round)
                {
                    if (rm.Winner == null && rm.Entries.Any(x => x.Score != 0) || rm.Entries.Count == 1)
                        toScore.Add(rm);
                }
            }

            MarkWinnerInMatchups(toScore);

            AdvanceWinners(toScore, model);

            toScore.ForEach(x => GlobalConfig.Connection.UpdateMatchup(x));

            int enddingRound = model.CheckCurrentRound();
        }

        /// <summary>
        /// Advance winners.
        /// </summary>
        /// <param name="models">List of matchups.</param>
        /// <param name="tournament">Tournament info.</param>
        private static void AdvanceWinners(List<MatchupModel> models, TournamentModel tournament)
        {
            foreach (MatchupModel m in models)
            {
                foreach (List<MatchupModel> round in tournament.Rounds)
                {
                    foreach (MatchupModel rm in round)
                    {
                        foreach (MatchupEntryModel me in rm.Entries)
                        {
                            if (me.ParentMatchup != null)
                            {
                                if (me.ParentMatchup.Id == m.Id)
                                {
                                    me.TeamCompeting = m.Winner;
                                    GlobalConfig.Connection.UpdateMatchup(rm);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Marks a winner from matchup.
        /// </summary>
        /// <param name="models">Tournament info.</param>
        /// <exception cref="Exception">If score is tie, will be exception.</exception>
        private static void MarkWinnerInMatchups(List<MatchupModel> models)
        {
            //greater or lesser
            string greaterWins = ConfigurationManager.AppSettings["greaterWins"];

            foreach (MatchupModel m in models)
            {
                //Cheks for bye week entry
                if (m.Entries.Count == 1)
                {
                    m.Winner = m.Entries[0].TeamCompeting;
                    continue;
                }

                //0 means false, or low score wins
                if (greaterWins == "0")
                {
                    if (m.Entries[0].Score < m.Entries[1].Score)
                        m.Winner = m.Entries[0].TeamCompeting;
                    else if (m.Entries[1].Score < m.Entries[0].Score)
                        m.Winner = m.Entries[1].TeamCompeting;
                    else
                        throw new Exception("We do not allow ties in this application.");
                }
                else
                {
                    //1 mean true, or high score wins
                    if (m.Entries[0].Score > m.Entries[1].Score)
                        m.Winner = m.Entries[0].TeamCompeting;
                    else if (m.Entries[1].Score > m.Entries[0].Score)
                        m.Winner = m.Entries[1].TeamCompeting;
                    else
                        throw new Exception("We do not allow ties in this application.");
                } 
            }
        }

        /// <summary>
        /// Create other rounds exepct first.
        /// </summary>
        /// <param name="model">Tournament info.</param>
        /// <param name="rounds">Number of rounds.</param>
        private static void CreateOtherRounds(TournamentModel model, int rounds)
        {
            //Starts with second round.
            int round = 2;
            List<MatchupModel> previousRound = model.Rounds[0];
            List<MatchupModel> currRound = new List<MatchupModel>();
            MatchupModel currMatchup = new MatchupModel();

            while (round <= rounds)
            {
                foreach (var match in previousRound)
                {
                    currMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match});

                    if (currMatchup.Entries.Count > 1)
                    {
                        currMatchup.MatchupRound = round;
                        currRound.Add(currMatchup);
                        currMatchup = new MatchupModel();
                    }
                }

                model.Rounds.Add(currRound);
                previousRound = currRound;
                currRound = new List<MatchupModel>();
                round += 1;
            }
        }

        /// <summary>
        /// Create first round for the tournament.
        /// </summary>
        /// <param name="byes">If there is bye.</param>
        /// <param name="teams">List of teams.</param>
        /// <returns>List of matchups for first round.</returns>
        private static List<MatchupModel> CreateFirstRound(int byes, List<TeamModel> teams)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            MatchupModel curr = new MatchupModel();

            foreach (var team in teams)
            {
                curr.Entries.Add(new MatchupEntryModel{ TeamCompeting = team});

                if (byes > 0 || curr.Entries.Count > 1)
                {
                    curr.MatchupRound = 1;
                    output.Add(curr);
                    curr = new MatchupModel();

                    if (byes > 0)
                    {
                        byes -= 1;
                    }
                }
            }

            return output;
        }


        /// <summary>
        /// Calculating if there is team without enemy(?).
        /// </summary>
        /// <param name="rounds">Number of rounds.</param>
        /// <param name="numberOfTeams">Number of teams.</param>
        /// <returns></returns>
        private static int NumberOfByes(int rounds, int numberOfTeams)
        {
            int output = 0;
            int totalTeams = 1;

            for (int i = 1; i <= rounds; i++)
            {
                totalTeams *= 2;
            }

            output = totalTeams - numberOfTeams;
            return output;
        }

        /// <summary>
        /// Calculating number of rounds.
        /// </summary>
        /// <param name="teamCount">Number of teams.</param>
        /// <returns>Number of rounds for tournament.</returns>
        private static int FindNumberOfRounds(int teamCount)
        {
            int output = 1;
            int val = 2;

            while (val < teamCount)
            {
                output += 1;
                val *= 2;
            }
            return output;
        }

        /// <summary>
        /// Checks current round.
        /// </summary>
        /// <param name="model">Tournament info.</param>
        /// <returns>Current round.</returns>
        public static int CheckCurrentRound(this TournamentModel model)
        {
            int output = 1;

            foreach (List<MatchupModel> round in model.Rounds)
            {
                if (round.All(x => x.Winner != null))
                {
                    output += 1;
                }
                else
                {
                    return output;
                }
            }

            //Tournament is complete.
            CompleteTournament(model);
            return output - 1;
        }

        /// <summary>
        /// Complete tournament.
        /// </summary>
        /// <param name="model">Tournament info.</param>
        private static void CompleteTournament(TournamentModel model)
        {
            GlobalConfig.Connection.CompleteTournament(model);
            TeamModel winners = model.Rounds.Last().First().Winner;
            TeamModel runnerUp = model.Rounds.Last().First().Entries.First(x => x.TeamCompeting != winners).TeamCompeting;

            decimal winnerPrize = 0;
            decimal runnerUpPrize = 0;

            if (model.Prizes.Count > 0)
            {
                decimal totalIncome = model.EnteredTeams.Count * model.EntryFee;

                PrizeModel firstPlacePrize = model.Prizes.FirstOrDefault(x => x.PlaceNumber == 1);
                PrizeModel secondPlacePrize = model.Prizes.FirstOrDefault(x => x.PlaceNumber == 2);

                if (firstPlacePrize != null)
                {
                    winnerPrize = firstPlacePrize.CalculatePrizePayout(totalIncome);
                    prizeWinner = winnerPrize.ToString();
                }

                if (secondPlacePrize != null)
                {
                    runnerUpPrize = secondPlacePrize.CalculatePrizePayout(totalIncome);
                    prizeRunnerUp = runnerUpPrize.ToString();
                }
            }

            winnerTeam = winners.TeamName;
            runnerUpTeam = runnerUp.TeamName;
            tournamentName = model.TournamentName;

            model.CompleteTournament();
        }

        /// <summary>
        /// Calculate prize.
        /// </summary>
        /// <param name="prize">Prize info.</param>
        /// <param name="totalIncome">Total income.</param>
        /// <returns>Prize payout.</returns>
        private static decimal CalculatePrizePayout(this PrizeModel prize, decimal totalIncome)
        {
            decimal output = 0;

            if (prize.PrizeAmount > 0)
                output = prize.PrizeAmount;
            else
                output = Decimal.Multiply(totalIncome, Convert.ToDecimal(prize.PrizePercentage / 100));

            return output;
        }

        /// <summary>
        /// Randomize teams order.
        /// </summary>
        /// <param name="teams">List of teams.</param>
        /// <returns>Randomized list of teams.</returns>
        private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
        {
            return teams.OrderBy(x => Guid.NewGuid()).ToList();
        }

    }
}
