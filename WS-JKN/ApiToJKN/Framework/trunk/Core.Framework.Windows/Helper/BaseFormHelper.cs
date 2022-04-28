using System;
using System.Windows.Media;
using Core.Framework.Helper;
using Core.Framework.Helper.Contracts;
using Core.Framework.Helper.Presenters;
using Core.Framework.Windows.Controls;

namespace Core.Framework.Windows.Helper
{
    using System.Windows;
    using System.Windows.Controls;

    using Core.Framework.Model.Impl;
    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Implementations;

    public class BaseFormHelper : UserControl, IGenericForm
    {
        // Using a DependencyProperty as the backing store for HeaderContent.  This enables animation, styling, binding, etc...

        #region Static Fields

        public static readonly DependencyProperty FooterContentProperty = DependencyProperty.Register(
            "FooterContent",
            typeof(Panel),
            typeof(BaseFormHelper),
            new UIPropertyMetadata());

        public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.Register(
            "HeaderContent",
            typeof(Panel),
            typeof(BaseFormHelper),
            new UIPropertyMetadata());

        public static readonly DependencyProperty InModeSearchProperty = DependencyProperty.Register(
            "InModeSearch",
            typeof(bool),
            typeof(BaseFormHelper),
            new UIPropertyMetadata(false));

        // Using a DependencyProperty as the backing store for MainContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MainContentProperty = DependencyProperty.Register(
            "MainContent",
            typeof(Panel),
            typeof(BaseFormHelper),
            new UIPropertyMetadata());

        #endregion

        #region Public Properties

        public string ConnectionString { get; set; }

        public Panel FooterContent
        {
            get
            {
                return (Panel)this.GetValue(FooterContentProperty);
            }
            set
            {
                this.SetValue(FooterContentProperty, value);
            }
        }

        public Panel HeaderContent
        {
            get
            {
                return (Panel)this.GetValue(HeaderContentProperty);
            }
            set
            {
                this.SetValue(HeaderContentProperty, value);
            }
        }

        public bool InModeSearch
        {
            get
            {
                return (bool)this.GetValue(InModeSearchProperty);
            }
            set
            {
                this.SetValue(InModeSearchProperty, value);
            }
        }

        public Panel MainContent
        {
            get
            {
                return (Panel)this.GetValue(MainContentProperty);
            }
            set
            {
                this.SetValue(MainContentProperty, value);
            }
        }

        public BaseConnectionManager ManagerConnection { get; set; }

        #endregion

        // Using a DependencyProperty as the backing store for FooterContent.  This enables animation, styling, binding, etc...

        #region Properties

        protected CoreTextBox PartTextBox { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void OnApplyTemplate()
        {
            this.PartTextBox = this.GetTemplateChild("TbTextbox") as CoreTextBox;
            base.OnApplyTemplate();
            if (this.PartTextBox != null)
            {
                this.PartTextBox.LostFocus += this.PartTextBoxOnLostFocus;
            }
        }

        #endregion

        #region Methods

        private void PartTextBoxOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            //this.InModeSearch = false;
        }

        #endregion        
    }
}