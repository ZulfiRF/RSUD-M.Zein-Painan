﻿namespace Core.Framework.Windows.Controls
{
    
    public enum FlyoutTheme
    {
        /// <summary>
        /// Adapts the Flyout's theme to the theme of its host window.
        /// </summary>
        Adapt,
        /// <summary>
        /// Adapts the Flyout's theme to the theme of its host window, but inverted.
        /// </summary>
        Inverse,
        /// <summary>
        /// The dark theme. This is the default theme.
        /// </summary>
        Dark,
        Light,

        /// <summary>
        /// The flyouts theme will match the host window's accent color.
        /// </summary>
        Accent
    }
}
