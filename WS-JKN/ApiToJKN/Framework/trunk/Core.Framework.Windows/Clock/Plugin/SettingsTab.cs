/*
 * Copyright (c) 2010, Sergey Loktin (mailto://shadowconsp@gmail.com)
 * Licensed under The MIT License (http://www.opensource.org/licenses/mit-license.php)
*/

using System.Windows;
using System.Windows.Media;

namespace Core.Framework.Windows.Clock.Plugin
{
    public class SettingsTab
    {
        /// <summary>
        /// Tab header image
        /// </summary>
        public ImageSource HeaderImageSource 
        {
            get { return _headerImageSource; }
            set { _headerImageSource = value; } 
        }
        /// <summary>
        /// Tab header text
        /// </summary>
        public string HeaderText 
        {
            get { return _headerText; }
            set { _headerText = value; } 
        }
        /// <summary>
        /// Tab content UIElement
        /// </summary>
        public UIElement Content 
        {
            get { return _content; }
            set { _content = value; } 
        }

        ImageSource _headerImageSource = null;
        string _headerText = "Tab";
        UIElement _content = null;
    }
}
