namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    ///     Interaction logic for ContentMessage.xaml
    /// </summary>
    public partial class ContentMessage : UserControl
    {
        #region Constructors and Destructors

        public ContentMessage()
        {
            this.InitializeComponent();
            this.BtnYes.Click += this.BtnYesOnClick;
        }

        #endregion

        #region Public Events

        public event EventHandler YesResult;

        #endregion

        #region Public Methods and Operators

        public void OnYesResult(EventArgs e)
        {
            EventHandler handler = this.YesResult;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Methods

        private void BtnYesOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            this.OnYesResult(routedEventArgs);
        }

        #endregion
    }
}