using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary
{
    /// <summary>
    /// Represents prize of the tournament.
    /// </summary>
    public class PrizeModel
    {
        /// <summary>
        /// The uniqe identifier for the prize.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Place for the prize.
        /// </summary>
        public int PlaceNumber { get; set; }

        /// <summary>
        /// Name of the prize place.
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// Amount of the prize.
        /// </summary>
        public decimal PrizeAmount { get; set; }

        /// <summary>
        /// Percentage of the prize.
        /// </summary>
        public double PrizePercentage { get; set; }

        public PrizeModel(){}

        public PrizeModel(string placeName, string placeNumber, string prizeAmount, string prizePercentage)
        {
            PlaceName = placeName;

            int placeNumberValue = 0;
            int.TryParse(placeNumber, out placeNumberValue);
            PlaceNumber = placeNumberValue;

            decimal prizeAmountValue = 0;
            decimal.TryParse(prizeAmount, out prizeAmountValue);
            PrizeAmount = prizeAmountValue;

            double prizePercentageValue = 0;
            double.TryParse(prizePercentage, out prizePercentageValue);
            PrizePercentage = prizePercentageValue;
        }
    }
}
