using System.Windows;

namespace OakBot.GameWisp_API
{
    /// <summary>
    /// Interaction logic for GameWispAuth.xaml
    /// </summary>
    public partial class GameWispAuth : Window
    {
        #region Public Constructors

        public GameWispAuth()
        {
            InitializeComponent();
        }

        #endregion Public Constructors

        #region Private Methods

        private void wbTwitchAuth_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
        }

        private void wbTwitchAuth_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        #endregion Private Methods
    }
}