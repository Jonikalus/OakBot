using System;
using System.Linq;
using System.Windows;

namespace OakBot
{
    /// <summary>
    /// Interaction logic for WindowDatabaseCleanup.xaml
    /// </summary>
    public partial class WindowDatabaseCleanup : Window
    {
        #region Public Constructors

        public WindowDatabaseCleanup()
        {
            InitializeComponent();
        }

        #endregion Public Constructors

        #region Private Methods

        private void btnClean_Click(object sender, RoutedEventArgs e)
        {
            if (rbTrim.IsChecked == true)
            {
                int watchedCutoff;
                int pointsCutoff;
                int daysCutoff;
                DateTime currentDate = DateTime.UtcNow;

                if (Int32.TryParse(tbWatched.Text, out watchedCutoff) &&
                    Int32.TryParse(tbPoints.Text, out pointsCutoff) &&
                    Int32.TryParse(tbDays.Text, out daysCutoff))
                {
                    foreach (Viewer viewer in MainWindow.colDatabase.Reverse())
                    {
                        if (cbHours.IsChecked == true && viewer.Minutes < watchedCutoff ||
                            cbPoints.IsChecked == true && viewer.Points < pointsCutoff ||
                            cbLastSeen.IsChecked == true && currentDate.Subtract(viewer.LastSeen).TotalDays > daysCutoff)
                        {
                            MainWindow.colDatabase.Remove(viewer);
                        }
                    }

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Use numberic values only.", "Oakbot Database Cleanup", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (rbPurge.IsChecked == true)
            {
                MessageBox.Show("Purge, not implemented yet.");
            }
        }

        #endregion Private Methods
    }
}