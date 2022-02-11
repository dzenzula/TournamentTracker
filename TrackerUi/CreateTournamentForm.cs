using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUi
{
    public partial class CreateTournamentForm : Form, IPrizeRequester, ITeamRequester
    {
        private readonly List<TeamModel> _availableTeams = GlobalConfig.Connection.GetTeam_All();
        private readonly List<TeamModel> _selectedTeams = new List<TeamModel>();
        private readonly List<PrizeModel> _selectedPrizes = new List<PrizeModel>();
        public CreateTournamentForm()
        {
            InitializeComponent();

            WireUpLists();
        }

        //Refresh and add teams and prizes.
        private void WireUpLists()
        {
            selectTeamDropDown.DataSource = null;
            selectTeamDropDown.DataSource = _availableTeams;
            selectTeamDropDown.DisplayMember = "TeamName";

            tournamentTeamsListBox.DataSource = null;
            tournamentTeamsListBox.DataSource = _selectedTeams;
            tournamentTeamsListBox.DisplayMember = "TeamName";

            prizesListBox.DataSource = null;
            prizesListBox.DataSource = _selectedPrizes;
            prizesListBox.DisplayMember = "PlaceName";
        }

        private void addTeamButton_Click(object sender, EventArgs e)
        {
            var t = (TeamModel) selectTeamDropDown.SelectedItem;

            if (t == null) return;
            _availableTeams.Remove(t);
            _selectedTeams.Add(t);

            WireUpLists();
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            //Call CreatePrizeForm
            CreatePrizeForm frm = new CreatePrizeForm(this);
            frm.Show();
        }

        private void createNewTeamLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateTeamForm frm = new CreateTeamForm(this);
            frm.Show();
        }

        //Get back from the form a PrizeModel
        public void PrizeComplete(PrizeModel model)
        {
            //Take the PrizeModel and put it into out list of selected prizes
            _selectedPrizes.Add(model);
            WireUpLists();
        }

        public void TeamComplete(TeamModel model)
        {
            _selectedTeams.Add(model);
            WireUpLists();
        }

        private void deleteSelectTeamButton_Click(object sender, EventArgs e)
        {
            var t = (TeamModel) tournamentTeamsListBox.SelectedItem;

            if(t == null) return;
            _availableTeams.Add(t);
            _selectedTeams.Remove(t);
            WireUpLists();
        }

        private void deleteSelectedPrizeButton_Click(object sender, EventArgs e)
        {
            var p = (PrizeModel) prizesListBox.SelectedItem;

            if(p == null) return;
            _selectedPrizes.Remove(p);
            WireUpLists();
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            //Validate data
            bool feeAcceptable = decimal.TryParse(entryFeeValue.Text, out decimal fee);
            if (!feeAcceptable)
            {
                MessageBox.Show("You need to entry valid Entry Fee.", "Invalid Fee", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            TournamentModel tm = new TournamentModel();
            tm.TournamentName = tournamentNameValue.Text;
            tm.EntryFee = fee;
            tm.Prizes = _selectedPrizes;
            tm.EnteredTeams = _selectedTeams;

            //Wire our matchups
            TournamentLogic.CreateRounds(tm);

            GlobalConfig.Connection.CreateTournament(tm);

            TournamentLogic.UpdateTournamentResults(tm);

            this.Close();
        }
    }
}
