using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
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
                string[] cols = line.Split(',');
                PrizeModel p = new PrizeModel();
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
        /// Convert list of strings to PersonModel.
        /// </summary>
        /// <param name="lines">List of strings from file.</param>
        /// <returns>List of PersonModel.</returns>
        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                PersonModel p = new PersonModel();
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

        public static List<TeamModel> ConvertToTeamModel(this List<string> lines)
        {
            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = TextConnector.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                TeamModel t = new TeamModel();
                t.Id = int.Parse(cols[0]);
                t.TeamName = cols[1];

                string[] personIds = cols[2].Split('|');

                foreach (string id in personIds)
                {
                    t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
                }

                output.Add(t);
            }

            return output;
        }

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

        public static void SaveToTeamFile(this List<TeamModel> models)
        {
            List<string> lines = new List<string>();

            foreach (TeamModel t in models)
            {
                lines.Add($"{ t.Id },{ t.TeamName },{ ConvertPeopleListToString(t.TeamMembers) }");
            }

            File.WriteAllLines(TextConnector.TeamFile.FullFilePath(), lines);
        }
    }
}
