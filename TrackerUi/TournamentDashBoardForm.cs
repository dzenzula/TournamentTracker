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
    public partial class TournamentDashBoardForm : Form
    {
        private readonly List<TournamentModel> _tournaments = GlobalConfig.Connection.GetTournament_All();
        public TournamentDashBoardForm()
        {
            InitializeComponent();

            WireUpDashBoardLists();
        }

        private void WireUpDashBoardLists()
        {
            loadExistingTournamentDropDown.DataSource = null;
            loadExistingTournamentDropDown.DataSource = _tournaments;
            loadExistingTournamentDropDown.DisplayMember = "TournamentName";
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            CreateTournamentForm frm = new CreateTournamentForm();
            frm.ShowDialog();
            WireUpDashBoardLists();
        }

        private void loadTournamentButton_Click(object sender, EventArgs e)
        {
            if (loadExistingTournamentDropDown.SelectedItem != null)
            {
                TournamentModel tm = (TournamentModel)loadExistingTournamentDropDown.SelectedItem;
                TournamentViewerForm frm = new TournamentViewerForm(tm);
                frm.ShowDialog();
                WireUpDashBoardLists();
            }
            else
            {
                MessageBox.Show("List is empty. Please create new tournament.");
            }
        }
    }
}
