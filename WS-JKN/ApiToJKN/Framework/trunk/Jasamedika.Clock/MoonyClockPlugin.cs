/*
 * Copyright (c) 2010, Sergey Loktin (mailto://shadowconsp@gmail.com)
 * Licensed under The MIT License (http://www.opensource.org/licenses/mit-license.php)
*/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using Jasamedika.ClockPlugin;

namespace Jasamedika.Clock
{
    public class MoonyClockPlugin : IPlugin
    {
        public double DefaultAngle = -4;
        public double DefaultSize = 350;

        public string Name { get { return "MoonyClock"; } }
        public UIElement DesktopElement { get; private set; }

        public void Initialize()
        {
            DesktopElement = new MoonyClockCtrl();
        }

        public void Dispose()
        {
            DesktopElement = null;
        }

        public List<SettingsTab> GetNewSettingsTabs()
        {
            List<SettingsTab> tabs = new List<SettingsTab>();

            //color settings tab
            SettingsTab tab = new SettingsTab();
            tab.HeaderText = "Colors";
            tab.HeaderImageSource = new BitmapImage(new Uri("pack://application:,,,/MoonyClock;component/colors.png"));
            tab.Content = new ColorsSettings(DesktopElement as MoonyClockCtrl);
            tabs.Add(tab);

            //clock settings tab
            tab = new SettingsTab();
            tab.HeaderText = "Settings";
            tab.HeaderImageSource = new BitmapImage(new Uri("pack://application:,,,/MoonyClock;component/settings.png"));
            tab.Content = new ClockSettings(DesktopElement as MoonyClockCtrl);
            tabs.Add(tab);

            return tabs;
        }
    }
}
