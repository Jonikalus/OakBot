using System;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Media;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;

using System.Diagnostics;

namespace OakBot
{
    /// <summary>
    /// Interaction logic for WindowImportData.xaml
    /// </summary>
    public partial class WindowImportData : Window
    {
        private MainWindow _mW;

        public WindowImportData(MainWindow mW)
        {
            _mW = mW;
            InitializeComponent();
        }

        private void btnImportAnkh_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog and set default file extention and filters
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".sqlite";
            dlg.Filter = "AnkhBot CurrencyDB|CurrencyDB.sqlite";
            dlg.InitialDirectory = @"%appdata%\AnkhHeart\AnkhBotR2\Twitch\Databases";

            // Show file dialog
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source=CurrencyDB.sqlite; Version=3; Read Only=True;"));
                SQLiteCommand sqlCmd = new SQLiteCommand("SELECT * FROM CurrencyUser", dbConnection);
                SQLiteDataReader dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    // TODO override and add records from Ankhbot
                    Trace.WriteLine(string.Format("User: {0} with {1d} points", dataReader["Name"], dataReader["Points"]));
                }

            }
        }

        private void btnImportDeep_Click(object sender, RoutedEventArgs e)
        {
            if(tbDeepbotSecret.Text != "Deepbot API Secret")
            {
                // TODO connect to deepbot websocket
            }
        }

        private void tbDeepbotSecret_GotFocus(object sender, RoutedEventArgs e)
        {
            tbDeepbotSecret.Clear();
            tbDeepbotSecret.Foreground = Brushes.Black;
        }

        private void tbDeepbotSecret_LostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tbDeepbotSecret.Text))
            {
                tbDeepbotSecret.Foreground = Brushes.LightGray;
                tbDeepbotSecret.Text = "Deepbot API Secret";
            }
        }
    }
}
