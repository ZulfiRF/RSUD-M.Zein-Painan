/*
 * Copyright (c) 2010, Sergey Loktin (mailto://shadowconsp@gmail.com)
 * Licensed under The MIT License (http://www.opensource.org/licenses/mit-license.php)
*/

using System.Windows;
using System.Windows.Controls;

namespace Core.Framework.Windows.Clock
{
    /// <summary>
    /// Interaction logic for ColorsSettings.xaml
    /// </summary>
    public partial class ColorsSettings : UserControl
    {
        MoonyClockCtrl _clock;

        public ColorsSettings(MoonyClockCtrl DesktopElement)
        {
            InitializeComponent();
            _clock = DesktopElement;
            this.DataContext = _clock;
            if (_clock.ColorNum == 0)
                LightColor.IsChecked = true;
            else
                DarkColor.IsChecked = true;
        }

        private void LightColor_Checked(object sender, RoutedEventArgs e)
        {
            if (_clock == null)
                return;
            _clock.SetLightColors();
        }

        private void DarkColor_Checked(object sender, RoutedEventArgs e)
        {
            if (_clock == null)
                return;
            _clock.SetDarkColors();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.color = _clock.ColorNum;
            Properties.Settings.Default.Save();
        }
    }
}
