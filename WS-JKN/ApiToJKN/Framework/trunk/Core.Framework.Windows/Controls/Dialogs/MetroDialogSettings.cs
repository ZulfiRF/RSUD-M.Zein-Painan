namespace Core.Framework.Windows.Controls.Dialogs
{
    /// <summary>
    ///     A class that represents the settings used by Metro Dialogs.
    /// </summary>
    public class MetroDialogSettings
    {
        #region Constructors and Destructors

        internal MetroDialogSettings()
        {
            this.AffirmativeButtonText = "OK";
            this.NegativeButtonText = "Cancel";

            this.ColorScheme = MetroDialogColorScheme.Theme;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets/sets the text used for the Affirmative button. For example: "OK" or "Yes".
        /// </summary>
        public string AffirmativeButtonText { get; set; }

        /// <summary>
        ///     Gets/sets whether the metro dialog should use the default black/white appearance (theme) or try to use the current
        ///     accent.
        /// </summary>
        public MetroDialogColorScheme ColorScheme { get; set; }

        /// <summary>
        ///     Gets/sets the text used for the Negative bytton. For example: "Cancel" or "No".
        /// </summary>
        public string NegativeButtonText { get; set; }

        #endregion
    }
}