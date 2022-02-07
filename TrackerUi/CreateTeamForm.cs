using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace TrackerUi
{
    public partial class CreateTeamForm : Form
    {
        private List<PersonModel> _availableTeamMembers = GlobalConfig.Connection.GetPerson_All();
        private readonly List<PersonModel> _selectedTeamMembers = new List<PersonModel>();
        private readonly List<string> _errorMsg = new List<string>();
        private readonly ITeamRequester _callingForm;
        public CreateTeamForm(ITeamRequester caller)
        {
            InitializeComponent();

            _callingForm = caller;

            WireUpLists();
        }

        /// <summary>
        /// Refresh lists.
        /// </summary>
        private void WireUpLists()
        {
            selectTeamMemberDropDown.DataSource = null;
            selectTeamMemberDropDown.DataSource = _availableTeamMembers;
            selectTeamMemberDropDown.DisplayMember = "FullName";

            teamMembersListBox.DataSource = null;
            teamMembersListBox.DataSource = _selectedTeamMembers;
            teamMembersListBox.DisplayMember = "FullName";
        }

        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PersonModel p = new PersonModel
                {
                    FirstName = firstNameValue.Text,
                    LastName = lastNameValue.Text,
                    EmailAddress = emailValue.Text,
                    CellphoneNumber = cellphoneValue.Text
                };
                GlobalConfig.Connection.CreatePerson(p);

                _availableTeamMembers = GlobalConfig.Connection.GetPerson_All();
                WireUpLists();

                firstNameValue.Text = "";
                lastNameValue.Text = "";
                emailValue.Text = "";
                cellphoneValue.Text = "";
            }
            else
            {
                string message = string.Join(Environment.NewLine + Environment.NewLine + "   ", _errorMsg);
                MessageBox.Show("   " + message,"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _errorMsg.Clear();
            }
        }

        private bool ValidateForm()
        {
            bool output = true;
            Regex regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Regex regexPhone = new Regex(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}");

            if (firstNameValue.Text.Length == 0 || lastNameValue.Text.Length == 0 || emailValue.Text.Length == 0)
            {
                output = false;
                _errorMsg.Add("Please type in first name, last name and email!");
            }


            if (firstNameValue.Text.Any(char.IsDigit) || lastNameValue.Text.Any(char.IsDigit))
            {
                output = false;
                _errorMsg.Add("Please don`t use digits in your name and last name.");
            }


            if (!regexEmail.IsMatch(emailValue.Text))
            {
                output = false;
                _errorMsg.Add("Email is invalid!");
            }

            if (!regexPhone.IsMatch(cellphoneValue.Text) && cellphoneValue.Text.Length != 0)
            {
                output = false;
                _errorMsg.Add("Your cellphone is invalid!");
            }

            return output;
        }

        private void addTeamMemberButton_Click(object sender, EventArgs e)
        {
            var p = (PersonModel) selectTeamMemberDropDown.SelectedItem;

            if (p == null) return;
            _availableTeamMembers.Remove(p);
            _selectedTeamMembers.Add(p);
            WireUpLists();
        }

        private void deleteSelectTeamButton_Click(object sender, EventArgs e)
        {
            var p = (PersonModel) teamMembersListBox.SelectedItem;

            if (p == null) return;
            _availableTeamMembers.Add(p);
            _selectedTeamMembers.Remove(p);
            WireUpLists();
        }

        private void createTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = new TeamModel();

            if (ValidateTeam())
            {
                t.TeamName = teamNameValue.Text;

                t.TeamMembers = _selectedTeamMembers;

                GlobalConfig.Connection.CreateTeam(t);

                _callingForm.TeamComplete(t);

                this.Close();
            }
            else
            {
                string message = string.Join(Environment.NewLine + Environment.NewLine + "   ", _errorMsg);
                MessageBox.Show("   " + message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _errorMsg.Clear();
            }

        }

        private bool ValidateTeam()
        {
            bool output = true;

            if (teamNameValue.Text.Length == 0)
            {
                output = false;
                _errorMsg.Add("Please enter a valid team name.");
            }

            if (_selectedTeamMembers.Count == 0)
            {
                output = false;
                _errorMsg.Add("Please enter at least one team member.");
            }

            return output;
        }
    }
}
