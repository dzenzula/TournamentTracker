using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public static class TextConnectorProcessor
    {
        /// <summary>
        /// Get file path.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Path of file.</returns>
        public static string FullFilePath(this string fileName)
        {
            return $"{ConfigurationManager.AppSettings["filePath"]}\\{ fileName }";
        }
        
        /// <summary>
        /// Load the text file
        /// </summary>
        /// <param name="file">Full path of the file.</param>
        /// <returns>Return List of strings from file</returns>
        public static List<string> LoadFile(this string file)
        {
            if (!File.Exists(file))
                return new List<string>();
            return File.ReadAllLines(file).ToList();
        }

        /// <summary>
        /// Convert list of strings to PrizeModel.
        /// </summary>
        /// <param name="lines">List of strings from file.</param>
        /// <returns>List of PrizeModel.</returns>
        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach (string line in lines)
            {
                PrizeModel p = new PrizeModel();
                string[] cols = line.Split(',');
                p.Id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2];
                p.PrizeAmount = decimal.Parse(cols[3]);
                p.PrizePercentage = double.Parse(cols[4]);
                output.Add(p);
            }

            return output;
        }

        /// <summary>
        /// Convert list of strings to PersonModel.
        /// </summary>
        /// <param name="lines">List of strings from file.</param>
        /// <returns>List of PersonModel.</returns>
        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();

            foreach (string line in lines)
            {
                PersonModel p = new PersonModel();
                string[] cols = line.Split(',');
                p.Id = int.Parse(cols[0]);
                p.FirstName = cols[1];
                p.LastName = cols[2];
                p.EmailAddress = cols[3];
                p.CellphoneNumber = cols[4];
                output.Add(p);
            }

            return output;
        }

        /// <summary>
        /// Converts file to the TeamModel.
        /// </summary>
        /// <param name="lines">List of lines.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Returns Model of Team.</returns>
        public static List<TeamModel> ConvertToTeamModel(this List<string> lines, string fileName)
        {
            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = fileName.FullFilePath().LoadFile().ConvertToPersonModels();

            foreach (string line in lines)
            {
                TeamModel t = new TeamModel();

                string[] cols = line.Split(',');

                t.Id = int.Parse(cols[0]);
                t.TeamName = cols[1];

                string[] personIds = cols[2].Split('|');

                foreach (string id in personIds)
                {
                    t.TeamMembers.Add(people.First(x => x.Id == int.Parse(id)));
                }

                output.Add(t);
            }

            return output;
        }

        /// <summary>
        /// Converts file to the TournamentModel.
        /// </summary>
        /// <param name="lines">List of lines.</param>
        /// <param name="teamFileName">Name of team file.</param>
        /// <param name="peopleFile">Name of people file.</param>
        /// <param name="prizeFileName">Name of prize file.</param>
        /// <returns>Returns model of tournament.</returns>
        public static List<TournamentModel> ConvertToTournamentModels(this List<string> lines, 
            string teamFileName, 
            string peopleFile, 
            string prizeFileName)
        {
            //id, TournamentName, EntryFee, (id|id|id - Entered Teams), (id|id|id - Prizes), (Rounds - id^id^id|id^id^id|id^id^id)
            List<TournamentModel> output = new List<TournamentModel>();
            List<TeamModel> teams = GlobalConfig.TeamFile.FullFilePath().LoadFile().ConvertToTeamModel(peopleFile);
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();
            

            foreach (var line in lines)
            {
                TournamentModel tm = new TournamentModel();

                string[] cols = line.Split(',');

                tm.Id = int.Parse(cols[0]);
                tm.TournamentName = cols[1];
                tm.EntryFee = decimal.Parse(cols[2]);

                string[] teamIds = cols[3].Split('|');
                foreach (string id in teamIds)
                    tm.EnteredTeams.Add(teams.First(x => x.Id == int.Parse(id)));

                if(cols[4].Length > 0)
                {
                    string[] prizeIds = cols[4].Split('|');
                    foreach (string id in prizeIds)
                        tm.Prizes.Add(prizes.First(x => x.Id == int.Parse(id)));
                }

                string[] rounds = cols[5].Split('|');
                
                foreach (string round in rounds)
                {
                    string[] msText = round.Split('^');
                    List<MatchupModel> ms = new List<MatchupModel>();

                    foreach (string matchupModelTextId in msText)
                    {
                        ms.Add(matchups.First(x => x.Id == int.Parse(matchupModelTextId)));
                    }

                    tm.Rounds.Add(ms);
                }

                output.Add(tm);
            }

            return output;
        }

        public static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines)
        {
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();

            foreach (var line in lines)
            {
                MatchupEntryModel me = new MatchupEntryModel();

                string[] cols = line.Split(',');
                me.Id = int.Parse(cols[0]);
                if (cols[1].Length == 0)
                    me.TeamCompeting = null;
                else
                    me.TeamCompeting = LookUpTeamById(int.Parse(cols[1]));
                me.Score = double.Parse(cols[2]);
                int parentId = 0;
                if (int.TryParse(cols[3], out parentId))
                    me.ParentMatchup = LookUpMatchupById(parentId);
                else
                    me.ParentMatchup = null;

                output.Add(me);
            }

            return output;
        }

        private static List<MatchupEntryModel> ConvertStringToMatchupEntryModels(string input)
        {
            string[] ids = input.Split('|');
            List<string> entries = GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile();
            List<string> matchingEntries = new List<string>();

            foreach (string id in ids)
            {
                foreach (string entry in entries)
                {
                    string[] cols = entry.Split(',');

                    if(cols[0] == id)
                        matchingEntries.Add(entry);
                }
            }

            List<MatchupEntryModel> output = matchingEntries.ConvertToMatchupEntryModels();
            return output;
        }

        private static TeamModel LookUpTeamById(int id)
        {
            List<string> teams = GlobalConfig.TeamFile.FullFilePath().LoadFile();
            foreach (var team in teams)
            {
                string[] cols = team.Split(',');
                if (cols[0] == id.ToString())
                {
                    List<string> matchingTeams = new List<string>();
                    matchingTeams.Add(team);
                    return matchingTeams.ConvertToTeamModel(GlobalConfig.PeopleFile).First();
                }
            }

            return null;
        }

        private static MatchupModel LookUpMatchupById(int id)
        {
            List<string> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile();
            foreach (var matchup in matchups)
            {
                string[] cols = matchup.Split(',');
                if (cols[0] == id.ToString())
                {
                    List<string> matchingMatchups = new List<string>();
                    matchingMatchups.Add(matchup);
                    return matchingMatchups.ConvertToMatchupModels().First();
                }
            }

            return null;
        }

        public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            
            foreach (string line in lines)
            {
                MatchupModel m = new MatchupModel();
                string[] cols = line.Split(',');
                m.Id = int.Parse(cols[0]);
                m.Entries = ConvertStringToMatchupEntryModels(cols[1]);
                if (cols[2].Length == 0)
                    m.Winner = null;
                else
                    m.Winner = LookUpTeamById(int.Parse(cols[2]));
                m.MatchupRound = int.Parse(cols[3]);
                output.Add(m);
            }

            return output;
        }

        /// <summary>
        /// Converts team model to the text.
        /// </summary>
        /// <param name="teams">Model of team.</param>
        /// <returns>Returns string of team model.</returns>
        private static string ConvertTeamListToString(List<TeamModel> teams)
        {
            string output = "";

            if(teams.Count == 0)
                return "";

            foreach (var t in teams)
                output += $"{t.Id}|";

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        /// <summary>
        /// Converts prize model to the text.
        /// </summary>
        /// <param name="prizes">Model of prize.</param>
        /// <returns>Returns string of prize model.</returns>
        private static string ConvertPrizeListToString(List<PrizeModel> prizes)
        {
            string output = "";

            if (prizes.Count == 0)
                return "";

            foreach (var p in prizes)
                output += $"{p.Id}|";

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        /// <summary>
        /// Converts list of rounds to the text.
        /// </summary>
        /// <param name="rounds">Model of round.</param>
        /// <returns>Returns strings of round list.</returns>
        private static string ConvertRoundListToString(List<List<MatchupModel>> rounds)
        {
            string output = "";

            if (rounds.Count == 0)
                return "";

            foreach (var r in rounds)
                output += $"{ConvertMatchupListToString(r)}|";

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        /// <summary>
        /// Converts mutchups to the text.
        /// </summary>
        /// <param name="matchups">Model of mutchups</param>
        /// <returns>Returns strings of mutchups</returns>
        private static string ConvertMatchupListToString(List<MatchupModel> matchups)
        {
            string output = "";

            if (matchups.Count == 0)
                return "";
            foreach (var m in matchups)
                output += $"{m.Id}^";

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        /// <summary>
        /// Converts list of people to the text.
        /// </summary>
        /// <param name="people">List of people.</param>
        /// <returns>Returns strings of people</returns>
        private static string ConvertPeopleListToString(List<PersonModel> people)
        {
            string output = "";

            if (people.Count == 0)
            {
                return "";
            }

            foreach (PersonModel p in people)
            {
                output += $"{ p.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertMatchupEntryListToString(List<MatchupEntryModel> entries)
        {
            string output = "";

            if (entries.Count == 0)
            {
                return "";
            }

            foreach (MatchupEntryModel e in entries)
            {
                output += $"{ e.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }


        /// <summary>
        /// Save the list of strings to the prize text file.
        /// </summary>
        /// <param name="models">List of PrizeModel.</param>
        /// <param name="fileName">Name of the file to save.</param>
        public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (PrizeModel p in models)
                lines.Add($"{ p.Id },{ p.PlaceNumber },{ p.PlaceName },{ p.PrizeAmount },{ p.PrizePercentage }");

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        /// <summary>
        /// Save the list of strings to the people text file.
        /// </summary>
        /// <param name="models">List of PersonModel.</param>
        /// <param name="fileName">Name of the file to save.</param>
        public static void SaveToPeopleFile(this List<PersonModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (PersonModel p in models)
                lines.Add($"{p.Id},{p.FirstName},{p.LastName},{p.EmailAddress},{p.CellphoneNumber}");

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        /// <summary>
        /// Save list of strings to the team text file.
        /// </summary>
        /// <param name="models">List of team model.</param>
        /// <param name="fileName">Name of the file to save.</param>
        public static void SaveToTeamFile(this List<TeamModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (TeamModel t in models)
            {
                lines.Add($"{ t.Id },{ t.TeamName },{ ConvertPeopleListToString(t.TeamMembers) }");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        /// <summary>
        /// Save list of strings to the tournament text file.
        /// </summary>
        /// <param name="models">List of tournament model.</param>
        /// <param name="fileName">Name of the file to save.</param>
        public static void SaveToTournamentFile(this List<TournamentModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (var tm in models)
            {
                lines.Add($"{tm.Id}," +
                          $"{tm.TournamentName}," +
                          $"{tm.EntryFee}," +
                          $"{ConvertTeamListToString(tm.EnteredTeams)}," +
                          $"{ConvertPrizeListToString(tm.Prizes)}," +
                          $"{ConvertRoundListToString(tm.Rounds)}");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        /// <summary>
        /// Save list of matchups to the files.
        /// </summary>
        /// <param name="model">Tournament model.</param>
        /// <param name="matchupFile">Name of file to save.</param>
        /// <param name="matchupEntryFile">Name of file to save.</param>
        public static void SaveRoundsToFile(this TournamentModel model, string matchupFile, string matchupEntryFile)
        {
            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    matchup.SaveMatchupToFile(matchupFile, matchupEntryFile);
                }
            }
        }

        /// <summary>
        /// Saves matchups to the file.
        /// </summary>
        /// <param name="matchup">Matchup model.</param>
        /// <param name="matchupFile">Name of file to save.</param>
        /// <param name="matchupEntryFile">Name of entries file to save.</param>
        public static void SaveMatchupToFile(this MatchupModel matchup, string matchupFile, string matchupEntryFile)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            int currId = 1;
            if(matchups.Count > 0)
                currId = matchups.OrderByDescending(x => x.Id).First().Id + 1;

            matchup.Id = currId;

            matchups.Add(matchup);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.SaveEntryToFile(matchupEntryFile);
            }

            List<string> lines = new List<string>();

            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                    winner = m.Winner.Id.ToString();
                lines.Add($"{m.Id},{ConvertMatchupEntryListToString(m.Entries)},{winner},{m.MatchupRound}");
            }

            File.WriteAllLines(GlobalConfig.MatchupFile.FullFilePath(), lines);
        }

        /// <summary>
        /// Saves entries to the file.
        /// </summary>
        /// <param name="entry">Matchup entry model.</param>
        /// <param name="matchupEntryFile">Name of file to save.</param>
        public static void SaveEntryToFile(this MatchupEntryModel entry, string matchupEntryFile)
        {
            List<MatchupEntryModel> entries =
                GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

            int currId = 1;
            if (entries.Count > 0)
                currId = entries.OrderByDescending(x => x.Id).First().Id + 1;

            entry.Id = currId;
            entries.Add(entry);

            List<string> lines = new List<string>();

            foreach (MatchupEntryModel e in entries)
            {
                string parent = "";
                if(e.ParentMatchup != null)
                    parent = e.ParentMatchup.Id.ToString();
                string teamCompeting = "";
                if(e.TeamCompeting != null)
                    teamCompeting = e.TeamCompeting.Id.ToString();
                lines.Add($"{e.Id},{teamCompeting},{e.Score},{parent}");
            }

            File.WriteAllLines(GlobalConfig.MatchupEntryFile.FullFilePath(), lines);
        }

        public static void UpdateMatchupToFile(this MatchupModel matchup)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();
            MatchupModel oldMatchup = new MatchupModel();

            foreach (MatchupModel m in matchups)
            {
                if (m.Id == matchup.Id)
                    oldMatchup = m;
            }

            matchups.Remove(oldMatchup);

            matchups.Add(matchup);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.UpdateEntryToFile();
            }

            List<string> lines = new List<string>();

            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                    winner = m.Winner.Id.ToString();
                lines.Add($"{m.Id},{ConvertMatchupEntryListToString(m.Entries)},{winner},{m.MatchupRound}");
            }

            File.WriteAllLines(GlobalConfig.MatchupFile.FullFilePath(), lines);
        }

        public static void UpdateEntryToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries =
                GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();
            MatchupEntryModel oldEntry = new MatchupEntryModel();

            foreach (MatchupEntryModel e in entries)
            {
                if (e.Id == entry.Id)
                    oldEntry = e;
            }

            entries.Remove(oldEntry);

            entries.Add(entry);

            List<string> lines = new List<string>();

            foreach (MatchupEntryModel e in entries)
            {
                string parent = "";
                if (e.ParentMatchup != null)
                    parent = e.ParentMatchup.Id.ToString();
                string teamCompeting = "";
                if (e.TeamCompeting != null)
                    teamCompeting = e.TeamCompeting.Id.ToString();
                lines.Add($"{e.Id},{teamCompeting},{e.Score},{parent}");
            }

            File.WriteAllLines(GlobalConfig.MatchupEntryFile.FullFilePath(), lines);
        }
    }
}
