﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OakBot.GameWisp_API
{
    /// <summary>
    /// Interaction logic for GameWispAuth.xaml
    /// </summary>
    public partial class GameWispAuth : Window
    {
        public GameWispAuth()
        {
            InitializeComponent();
        }

        private void wbTwitchAuth_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }

        private void wbTwitchAuth_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {

        }
    }
}