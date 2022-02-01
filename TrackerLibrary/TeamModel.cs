using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary
{
    /// <summary>
    /// Represents model of team.
    /// </summary>
    public class TeamModel
    {
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
