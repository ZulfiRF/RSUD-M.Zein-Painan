﻿namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    ///     Interaction logic for ContentError.xaml
    /// </summary>
    public partial class ContentError : UserControl
    {
        #region Constructors and Destructors

        public ContentError()
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