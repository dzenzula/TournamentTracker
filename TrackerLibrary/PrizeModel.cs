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
    }
}
