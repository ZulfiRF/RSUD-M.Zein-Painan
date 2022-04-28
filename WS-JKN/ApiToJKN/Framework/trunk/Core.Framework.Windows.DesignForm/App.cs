using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Core.Framework.Helper;
using Core.Framework.Helper.Configuration;
using Core.Framework.Helper.Contracts;

namespace Core.Framework.Windows.DesignForm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Core.Framework.Windows;component/Styles/Colours.xaml", UriKind.RelativeOrAbsolute) });
            //Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Core.Framework.Windows;component/Styles/Accents/Blue.xaml", UriKind.RelativeOrAbsolute) });
            //Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Core.Framework.Windows;component/Styles/Fonts.xaml", UriKind.RelativeOrAbsolute) });
            //Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Core.Framework.Windows;component/Styles/Controls.xaml", UriKind.RelativeOrAbsolute) });
            //Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Core.Framework.Windows;component/Styles/Controls.AnimatedSingleRowTabControl.xaml", UriKind.RelativeOrAbsolute) });
            //Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Core.Framework.Windows;component/Resources/Icons.xaml", UriKind.RelativeOrAbsolute) });
            //Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Core.Framework.Windows;component/Styles/Accents/BaseLight.xaml", UriKind.RelativeOrAbsolute) });
            //Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/Core.Framework.Windows;component/Styles/Controls.DisplayWithCheckBox.xaml", UriKind.RelativeOrAbsolute) });
            //Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/Core.Framework.Windows;component/Styles/Controls.DisplayWithDateTime.xaml", UriKind.RelativeOrAbsolute) });




            var setting = new SettingXml();
            BaseDependency.Add<ISetting>(setting);
        }
    }
}
