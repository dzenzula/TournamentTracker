using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrackerUi
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Initialize the database connection.
            TrackerLibrary.GlobalConfig.InitializeConnections(true, true);
            Application.Run(new CreatePrizeForm());

            //Application.Run(new TournamentDashBoardForm());
        }
    }
}
