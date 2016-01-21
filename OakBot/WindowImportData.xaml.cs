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
            try
            {
                MessageBox.Show(string.Format("Completed import from AnkhBot.\nAdded {0} records.", Config.ImportFromAnkhbot(_mW)),
                        "AnkhBot Import", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (SQLiteException ex)
            {
                Trace.WriteLine(ex.ToString());
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
