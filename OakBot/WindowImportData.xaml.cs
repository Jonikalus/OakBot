using System;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Media;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Diagnostics;

namespace OakBot
{
    /// <summary>
    /// Interaction logic for WindowImportData.xaml
    /// </summary>
    public partial class WindowImportData : Window
    {

        public WindowImportData()
        {
            InitializeComponent();
        }

        #region AnkhbotImport

        private void btnImportAnkh_Click(object sender, RoutedEventArgs e)
        {
            if (ImportFromAnkhbot())
            {
                this.Close();
            }            
        }

        /// <summary>
        /// Import the currency database file created by Ankhbot.
        /// </summary>
        /// <returns>True on success, false otherwise.</returns>
        private bool ImportFromAnkhbot()
        {
            // Create OpenFileDialog and set default file extention and filters
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".sqlite";
            dlg.Filter = "AnkhBot CurrencyDB|CurrencyDB.sqlite";
            dlg.InitialDirectory = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData) + "\\AnkhHeart\\AnkhBotR2\\Twitch\\Databases";

            // Show file dialog
            if (dlg.ShowDialog() == true)
            {
                int cntRecords = 0;

                // Set status
                lblStatusText.Content = "Reading Ankhbot Data and Writing to file. This window will close automatically when finished.";

                try
                {
                    // Create and open a new SQLite DB connection from the user selected file
                    SQLiteConnection dbConnection = new SQLiteConnection(string.Format("DataSource={0}; Version=3; Read Only=True;", dlg.FileName));
                    dbConnection.Open();

                    // Execute SQL command
                    SQLiteCommand sqlCmd = new SQLiteCommand("SELECT * FROM CurrencyUser", dbConnection);
                    SQLiteDataReader dataReader = sqlCmd.ExecuteReader();

                    // TODO close active chat connections
                    try
                    {
                        MainWindow.instance.DisconnectBot();
                        MainWindow.instance.DisconnectStreamer();
                    }
                    catch (Exception)
                    {

                    }

                    MainWindow.colViewers.Clear();
                    MainWindow.colDatabase.Clear();

                    // Itterate over found rows and insert a new TwitchViewer in colDatabase
                    while (dataReader.Read())
                    {
                        TwitchViewer viewer = new TwitchViewer((string)dataReader["Name"]);

                        viewer.Title = (string)dataReader["Rank"];
                        viewer.Points = (long)dataReader["Points"];
                        viewer.Raids = (long)dataReader["Raids"];

                        // .ToUniversalTime() is required to set DateTimeKind to universal and the result
                        // will else be converted to the local machine time which is what we don't want.
                        viewer.LastSeen = DateTime.Parse((string)dataReader["LastSeen"],
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();

                        // AnkhBot's time format is the following [d.]HH:MM:SS
                        // This is the value for timespan.toString(), .Parse() and .TryParse()
                        viewer.Watched = TimeSpan.Parse((string)dataReader["Hours"]);

                        // Add new TwitchViewer to colDatabase
                        MainWindow.colDatabase.Add(viewer);

                        // Counter for parsed records                      
                        cntRecords++;
                    }

                    dbConnection.Close();
                    dbConnection.Dispose();

                    // Open DBfile
                    dbConnection = new SQLiteConnection(string.Format("Data Source={0}; Version=3", ViewerDB.filename));
                    dbConnection.Open();

                    // Set status
                    lblStatusText.Content = "Clearing existing database...";

                    // Purge DBfile
                    sqlCmd = new SQLiteCommand("DELETE FROM `Viewers`", dbConnection);
                    sqlCmd.ExecuteNonQuery();

                    int cntWriteViewer = 0;

                    // Insert new TwitchViewer in `Viewers`
                    foreach (TwitchViewer viewer in MainWindow.colDatabase)
                    {
                        // Set status
                        //http://stackoverflow.com/questions/32680826/wpf-mvvm-thread-keep-running-and-show-progress-in-wpf-windows
                        //http://stackoverflow.com/questions/1952201/display-progress-bar-while-doing-some-work-in-c
                        lblStatusText.Content = string.Format("Writing viewer {0} of {1} to file...", cntWriteViewer, cntRecords);
                        pbStatus.Value = cntWriteViewer / cntRecords * 100;
                        cntWriteViewer++;

                        sqlCmd = new SQLiteCommand(
                            string.Format("INSERT INTO `Viewers` VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')",
                                viewer.UserName,
                                viewer.Points,
                                viewer.Spent,
                                viewer.Watched.ToString(),
                                viewer.LastSeen.ToString("o"),
                                viewer.Raids,
                                viewer.Title,
                                viewer.regular.ToString(),
                                viewer.IGN),
                            dbConnection);
                        sqlCmd.ExecuteNonQuery();
                    }

                    // Close DBfile
                    dbConnection.Close();

                    // Return success
                    return true;

                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show(string.Format("Could not open or read the selected sqlite database file.\n\n{0}", ex.ToString()),
                        "AnkhBot User Data Import", MessageBoxButton.OK, MessageBoxImage.Error);

                    lblStatusText.Content = "Error reading file.";

                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("The following program error has occured:\n\n{0}", ex.ToString()),
                        "AnkhBot User Data Import", MessageBoxButton.OK, MessageBoxImage.Error);

                    lblStatusText.Content = "Unknown error occured.";

                    return false;
                }
            }

            // User canceled the file selection
            return false;
        }

        #endregion

        private void btnImportDeep_Click(object sender, RoutedEventArgs e)
        {
            if(tbDeepSecret.Text != "Deepbot API Secret")
            {
                // TODO connect to deepbot websocket
            }
        }

        private void tbDeepSecret_GotFocus(object sender, RoutedEventArgs e)
        {
            tbDeepSecret.Clear();
            tbDeepSecret.Foreground = Brushes.Black;
        }

        private void tbDeepSecret_LostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tbDeepSecret.Text))
            {
                tbDeepSecret.Foreground = Brushes.LightGray;
                tbDeepSecret.Text = "Deepbot API Secret";
            }
        }
    }
}
