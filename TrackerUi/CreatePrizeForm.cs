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
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace TrackerUi
{
    public partial class CreatePrizeForm : Form
    {
        private readonly List<string> errorMsg = new List<string>();
        public CreatePrizeForm()
        {
            InitializeComponent();
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PrizeModel model = new PrizeModel(
                    placeNameValue.Text, 
                    placeNumberValue.Text, 
                    prizeAmountValue.Text,
                    prizePercentageValue.Text);

                GlobalConfig.Connection.CreatePrize(model);

                placeNameValue.Text = "";
                placeNumberValue.Text = "";
                prizeAmountValue.Text = "0";
                prizePercentageValue.Text = "0";
            }
            else
            {
                string message = string.Join(Environment.NewLine + Environment.NewLine + "   ", errorMsg);
                MessageBox.Show("   " + message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorMsg.Clear();
            }
        }

        private bool ValidateForm()
        {
            bool output = true;
            int placeNumber;

            if (!int.TryParse(placeNumberValue.Text, out placeNumber))
            {
                output = false;
                errorMsg.Add("Place number is invalid!");
            }

            if (placeNumber < 1)
            {
                output = false;
                errorMsg.Add("Place number must be greater than 0!");
            }

            if (placeNameValue.Text.Length == 0)
            {
                output = false;
                errorMsg.Add("Place name is empty!");
            }

            decimal prizeAmount = 0;
            double prizePercentage = 0;

            if(!decimal.TryParse(prizeAmountValue.Text, out prizeAmount) || !double.TryParse(prizePercentageValue.Text, out prizePercentage))
            {
                output = false;
                errorMsg.Add("Amount and percentage must be a number!");
            }

            if (prizeAmount <= 0 && prizePercentage <= 0)
            {
                output = false;
                errorMsg.Add("Amount and percentage can`t be 0 or less!");
            }

            if(prizePercentage < 0 || prizePercentage > 100)
            {
                output = false;
                errorMsg.Add("Percentage can`t be less than 0 and greater than 100!");
            }

            return output;
        }
    }
}
