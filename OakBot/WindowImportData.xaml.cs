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

        public WindowImportData()
        {
            InitializeComponent();
        }

        private void btnImportAnkh_Click(object sender, RoutedEventArgs e)
        {
            if (Utils.ImportFromAnkhbot())
            {
                this.Close();
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
