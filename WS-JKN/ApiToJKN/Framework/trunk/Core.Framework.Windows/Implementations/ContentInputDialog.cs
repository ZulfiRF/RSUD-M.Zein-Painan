namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    ///     Interaction logic for ContentConfirmation.xaml
    /// </summary>
    public partial class ContentInputDialog : UserControl
    {
        #region Constructors and Destructors

        public ContentInputDialog()
        {
            this.InitializeComponent();
            this.BtnNo.Click += this.BtnNoOnClick;
            this.BtnYes.Click += this.BtnYesOnClick;
        }

        #endregion

        #region Public Events

        public event EventHandler NoResult;

        public event EventHandler YesResult;

        #endregion

        #region Public Methods and Operators

        public void OnNoResult(EventArgs e)
        {
            EventHandler handler = this.NoResult;
            if (handler != null)
            {
                handler(this, e);
            }
        }

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

        private void BtnNoOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            this.OnNoResult(routedEventArgs);
        }

        private void BtnYesOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            this.OnYesResult(routedEventArgs);
        }

        #endregion

        public string result { get; set; }
    }
}