using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents model of team.
    /// </summary>
    public class TeamModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Represents list of the team member.
        /// </summary>
        public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();

        /// <summary>
        /// Represents name of the team.
        /// </summary>
        public string TeamName { get; set; }

    }
}
