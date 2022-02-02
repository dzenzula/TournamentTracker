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
        /// Save the list of strings to the text file.
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
    }
}
