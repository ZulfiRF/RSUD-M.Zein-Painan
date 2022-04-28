/*
 * Copyright (c) 2010, Sergey Loktin (mailto://shadowconsp@gmail.com)
 * Licensed under The MIT License (http://www.opensource.org/licenses/mit-license.php)
*/

using System.Collections.Generic;
using System.Windows;

namespace Core.Framework.Windows.Clock.Plugin
{
    public interface IPlugin
    {
        /// <summary>
        /// Plugin name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// UIElemet that will be shown on desktop
        /// </summary>
        UIElement DesktopElement { get; }

        /// <summary>
        /// Initialize plugin
        /// </summary>
        void Initialize();
        /// <summary>
        /// Dispose plugin
        /// </summary>
        void Dispose();
        /// <summary>
        /// Get settings tabs for plugin that will be show in settings window tab for this plugin 
        /// </summary>
        /// <returns></returns>
        List<SettingsTab> GetNewSettingsTabs();
    }
}
