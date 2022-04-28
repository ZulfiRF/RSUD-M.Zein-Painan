namespace Core.Framework.Windows
{
    using System;
    using System.Windows;

    /// <summary>
    ///     Helper to show or close given splash window
    /// </summary>
    public static class Splasher
    {
        #region Static Fields

        /// <summary>
        /// </summary>
        private static Window mSplash;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Get or set the splash screen window
        /// </summary>
        public static Window Splash
        {
            get
            {
                return mSplash;
            }
            set
            {
                mSplash = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Close splash screen
        /// </summary>
        public static void CloseSplash()
        {
            if (mSplash != null)
            {
                mSplash.Close();

                if (mSplash is IDisposable)
                {
                    (mSplash as IDisposable).Dispose();
                }
            }
            IsComplated = true;
        }

        /// <summary>
        ///     Show splash screen
        /// </summary>
        public static void ShowSplash()
        {
            if (mSplash != null)
            {
                mSplash.Show();
            }
        }

        #endregion

        public static bool IsComplated { get; set; }
    }
}