using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Core.Framework.Windows.Controls;
using Core.Framework.Windows.Native;

namespace Core.Framework.Windows.Behaviours
{
    public class WindowSettings
    {
        #region Static Fields

        public static readonly DependencyProperty WindowPlacementSettingsProperty =
            DependencyProperty.RegisterAttached(
                "WindowPlacementSettings",
                typeof(IWindowPlacementSettings),
                typeof(WindowSettings),
                new FrameworkPropertyMetadata(OnWindowPlacementSettingsInvalidated));

        #endregion

        #region Fields

        private IWindowPlacementSettings _settings;

        private Window _window;

        #endregion

        #region Constructors and Destructors

        public WindowSettings(Window window, IWindowPlacementSettings windowPlacementSettings)
        {
            this._window = window;
            this._settings = windowPlacementSettings;
        }

        #endregion

        #region Public Methods and Operators

        public static void SetSave(DependencyObject dependencyObject, IWindowPlacementSettings windowPlacementSettings)
        {
            dependencyObject.SetValue(WindowPlacementSettingsProperty, windowPlacementSettings);
        }

        #endregion

        #region Methods

        protected virtual void LoadWindowState()
        {
            if (this._settings == null)
            {
                return;
            }
            this._settings.Reload();

            if (this._settings.Placement == null)
            {
                return;
            }

            try
            {
                WINDOWPLACEMENT wp = this._settings.Placement.Value;

                wp.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                wp.flags = 0;
                wp.showCmd = (wp.showCmd == Constants.SW_SHOWMINIMIZED ? Constants.SW_SHOWNORMAL : wp.showCmd);
                IntPtr hwnd = new WindowInteropHelper(this._window).Handle;
                UnsafeNativeMethods.SetWindowPlacement(hwnd, ref wp);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load window state:\r\n{0}", ex);
            }
        }

        protected virtual void SaveWindowState()
        {
            if (this._settings == null)
            {
                return;
            }
            WINDOWPLACEMENT wp;
            IntPtr hwnd = new WindowInteropHelper(this._window).Handle;
            UnsafeNativeMethods.GetWindowPlacement(hwnd, out wp);
            this._settings.Placement = wp;
            //   this._settings.Save();
        }

        private static void OnWindowPlacementSettingsInvalidated(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var window = dependencyObject as Window;
            if (window == null || !(e.NewValue is IWindowPlacementSettings))
            {
                return;
            }

            var windowSettings = new WindowSettings(window, (IWindowPlacementSettings)e.NewValue);
            windowSettings.Attach();
        }

        private void Attach()
        {
            if (this._window == null)
            {
                return;
            }
            this._window.Closing += this.WindowClosing;
            this._window.SourceInitialized += this.WindowSourceInitialized;
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            this.SaveWindowState();
            this._window.Closing -= this.WindowClosing;
            this._window.SourceInitialized -= this.WindowSourceInitialized;
            this._window = null;
            this._settings = null;
        }

        private void WindowSourceInitialized(object sender, EventArgs e)
        {
            this.LoadWindowState();
        }

        #endregion
    }
}