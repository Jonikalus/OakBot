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
                try
                {
                    int counter = 0;
                    string connString = string.Format("DataSource={0}; Version=3; Read Only=True;", dlg.FileName);
                    SQLiteConnection dbConnection = new SQLiteConnection(connString);
                    dbConnection.Open();
                    SQLiteCommand sqlCmd = new SQLiteCommand("SELECT * FROM CurrencyUser", dbConnection);
                    SQLiteDataReader dataReader = sqlCmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        TwitchUser viewer = new TwitchUser((string)dataReader["Name"]);
                        viewer.rank = (string)dataReader["Rank"];
                        viewer.points = (long)dataReader["Points"];
                        viewer.hours = (string)dataReader["Hours"];
                        viewer.raids = (long)dataReader["Raids"];
                        viewer.timeFirstSeen = DateTime.Parse((string)dataReader["LastSeen"]);
                        viewer.timeLastSeen = DateTime.Parse((string)dataReader["LastSeen"]);

                        _mW.viewerDatabase.Add(viewer);
                        counter++;
                    }

                    // Display message box
                    MessageBox.Show(string.Format("Completed import from AnkhBot.\nAdded {0} records.",counter),
                        "AnkhBot Import", MessageBoxButton.OK, MessageBoxImage.Information);

                    dbConnection.Close();
                    this.Close();

                }
                catch (SQLiteException ex)
                {
                    Trace.WriteLine(ex.ToString());
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
