/*
 * Copyright (c) 2010, Sergey Loktin (mailto://shadowconsp@gmail.com)
 * Licensed under The MIT License (http://www.opensource.org/licenses/mit-license.php)
*/

using System.Windows;
using System.Windows.Controls;

namespace Jasamedika.Clock
{
    /// <summary>
    /// Interaction logic for WeatherSettings.xaml
    /// </summary>
    public partial class ClockSettings : UserControl
    {
        MoonyClockCtrl _clockCtrl = null;

        public ClockSettings(MoonyClockCtrl DesktopElement)
        {
            InitializeComponent();
            _clockCtrl = DesktopElement;
            this.DataContext = _clockCtrl;
            rb24.IsChecked = _clockCtrl.Hours24;
            rb12.IsChecked = !_clockCtrl.Hours24;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _clockCtrl.Hours24 = true;
        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            _clockCtrl.Hours24 = false;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.hours24 = _clockCtrl.Hours24;
            Properties.Settings.Default.Save();
        }
    }
}
