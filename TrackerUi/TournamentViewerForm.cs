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
    public partial class TournamentViewerForm : Form
    {
        private readonly TournamentModel _tournament;
        private readonly BindingList<int> _rounds = new BindingList<int>();
        private readonly BindingList<MatchupModel> _selectedMatchups = new BindingList<MatchupModel>();

        public TournamentViewerForm(TournamentModel tournamentModel)
        {
            InitializeComponent();

            _tournament = tournamentModel;

            _tournament.OnTournamentComplete += _tournament_OnTournamentComplete;

            WireUpLists();

            LoadFormData();

            LoadRounds();
        }

        private void _tournament_OnTournamentComplete(object sender, DateTime e)
        {
            this.Close();
        }

        private void LoadFormData()
        {
            tournamentName.Text = _tournament.TournamentName;
        }

        private void WireUpLists()
        {
            roundDropDown.DataSource = _rounds;

            matchupListBox.DataSource = _selectedMatchups;
            matchupListBox.DisplayMember = "DisplayName";
        }

        private void LoadRounds()
        {
            _rounds.Clear();

            _rounds.Add(1);
            int currRound = 1;

            foreach (var matchups in _tournament.Rounds)
            {
                if (matchups.First().MatchupRound > currRound)
                {
                    currRound = matchups.First().MatchupRound;
                    _rounds.Add(currRound);
                }
            }

            LoadMatchups(1);
        }

        private void roundDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropDown.SelectedItem);
        }

        private void LoadMatchups(int round)
        {
            foreach (var matchups in _tournament.Rounds)
            {
                if (matchups.First().MatchupRound == round)
                {
                    _selectedMatchups.Clear();
                    foreach (var m in matchups)
                    {
                        if (m.Winner == null || !unplayedOnlyCheckbox.Checked)
                        {
                            _selectedMatchups.Add(m); 
                        }
                    }
                }
            }

            if (_selectedMatchups.Count > 0)
                LoadMatchup(_selectedMatchups.First());

            DisplayMatchupInfo();
        }

        private void DisplayMatchupInfo()
        {
            bool isVisible = (_selectedMatchups.Count > 0);

            teamOneName.Visible = isVisible;
            teamOneScoreLabel.Visible = isVisible;
            teamOneScoreValue.Visible = isVisible;
            teamTwoName.Visible = isVisible;
            teamTwoScoreLabel.Visible = isVisible;
            teamTwoScoreValue.Visible = isVisible;
            versusLabel.Visible = isVisible;
            scoreButton.Visible = isVisible;
        }

        private void LoadMatchup(MatchupModel m)
        {
            if (m != null)
            {
                for (int i = 0; i < m.Entries.Count; i++)
                {
                    if (i == 0)
                    {
                        if (m.Entries[0].TeamCompeting != null)
                        {
                            teamOneName.Text = m.Entries[0].TeamCompeting.TeamName;
                            teamOneScoreValue.Text = m.Entries[0].Score.ToString();

                            teamTwoName.Text = "<bye>";
                            teamTwoScoreValue.Text = "0";
                        }
                        else
                        {
                            teamOneName.Text = "Not yet set";
                            teamOneScoreValue.Text = "0";
                        }

                    }

                    if (i == 1)
                    {
                        if (m.Entries[1].TeamCompeting != null)
                        {
                            teamTwoName.Text = m.Entries[1].TeamCompeting.TeamName;
                            teamTwoScoreValue.Text = m.Entries[1].Score.ToString();
                        }
                        else
                        {
                            teamTwoName.Text = "Not yet set";
                            teamTwoScoreValue.Text = "0";
                        }

                    }
                } 
            }
        }

        private void matchupListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchup((MatchupModel)matchupListBox.SelectedItem);
        }

        private void unplayedOnlyCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropDown.SelectedItem);
        }

        private string ValidateData()
        {
            string output = "";
            bool scoreOneValid = double.TryParse(teamOneScoreValue.Text, out double teamOneScore);
            bool scoreTwoValid = double.TryParse(teamTwoScoreValue.Text, out double teamTwoScore);

            if (!scoreOneValid || !scoreTwoValid)
                output = "The score value is not a valid number.";
            else if (teamOneScore == teamTwoScore)
                output = "We do not allow ties in this application.";

            return output;
        }

        private void scoreButton_Click(object sender, EventArgs e)
        {
            string errorMsg = ValidateData();
            if (errorMsg.Length > 0)
            {
                MessageBox.Show($"Input error: {errorMsg}");
                return;
            }

            MatchupModel m = (MatchupModel) matchupListBox.SelectedItem;

            for (int i = 0; i < m.Entries.Count; i++)
            {
                if (i == 0)
                {
                    if (m.Entries[0].TeamCompeting != null)
                    {
                        teamOneName.Text = m.Entries[0].TeamCompeting.TeamName;
                        m.Entries[0].Score = double.Parse(teamOneScoreValue.Text);
                    }
                }

                if (i == 1)
                {

                    if (m.Entries[1].TeamCompeting != null)
                    {
                        teamTwoName.Text = m.Entries[1].TeamCompeting.TeamName;
                        m.Entries[1].Score = double.Parse(teamTwoScoreValue.Text);
                    }
                }
            }

            try
            {
                int currentRound = TournamentLogic.CheckCurrentRound(_tournament);
                int lastRound = _rounds.Last();

                TournamentLogic.UpdateTournamentResults(_tournament);

                if (currentRound == lastRound)
                {
                    TournamentResultForm frm = new TournamentResultForm();
                    frm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The application had the following error: {ex.Message}");
                return;
            }

            LoadMatchups((int)roundDropDown.SelectedItem);
        }
    }
}
