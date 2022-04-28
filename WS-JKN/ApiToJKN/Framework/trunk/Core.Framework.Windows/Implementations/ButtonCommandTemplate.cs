using System.Windows.Media;
using Core.Framework.Windows.Extensions;
using Core.Framework.Windows.Windows;

namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Helper;

    /// <summary>
    ///     Interaction logic for ButtonCommand.xaml
    /// </summary>
    public partial class ButtonCommandTemplate
    {
        #region Fields

        private bool isBusy;

        #endregion

        #region Constructors and Destructors

        public ButtonCommandTemplate()
        {
            this.InitializeComponent();
            this.BtnBatal.Click += this.BtnBatalOnClick;
            this.BtnSimpan.Click += this.BtnSimpanOnClick;
            this.BtnHapus.Click += this.BtnHapusOnClick;
            this.BtnTutup.Click += this.BtnTutupOnClick;
        }

        #endregion

        #region Public Events

        public event EventHandler DeleteItem;

        public event EventHandler ResetItem;
        public event EventHandler SaveItem;

        #endregion

        #region Public Properties

        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }
            set
            {
                this.isBusy = value;
                foreach (CoreButton button in this.ContentPanel.Children.OfType<CoreButton>())
                {
                    button.IsBusy = value;
                }
            }
        }

        #endregion

        #region Properties

        protected PanelMetro FormGrid { get; set; }

        protected CoreButton PartBatalButton
        {
            get
            {
                return this.BtnBatal;
            }
        }

        protected CoreButton PartDeleteButton
        {
            get
            {
                return this.BtnHapus;
            }
        }

        protected CoreButton PartSaveButton
        {
            get
            {
                return this.BtnSimpan;
            }
        }

        protected CoreButton PartTutupButton
        {
            get
            {
                return this.BtnTutup;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void ClearItem()
        {
            IEnumerable<IValidateControl> children =
                Manager.FindVisualChildren<FrameworkElement>(this.FormGrid).OfType<IValidateControl>();
            foreach (IValidateControl validateControl in children)
            {
                validateControl.ClearValueControl();
                validateControl.IsError = false;
            }
        }

        public void OnDeleteItem(EventArgs e)
        {
            EventHandler handler = this.DeleteItem;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnResetItem(EventArgs e)
        {
            EventHandler handler = this.ResetItem;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnSaveItem(EventArgs e)
        {
            EventHandler handler = this.SaveItem;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void SetFormSubmit(PanelMetro form)
        {
            //   BtnHapus.FormGrid = form;
            this.BtnSimpan.FormGrid = form;
            //     BtnBatal.FormGrid = form;
            this.FormGrid = form;
        }

        #endregion

        #region Methods

        private void BtnBatalOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            this.ClearItem();
            this.OnResetItem(EventArgs.Empty);
        }

        private void BtnHapusOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            this.OnDeleteItem(EventArgs.Empty);
        }

        private void BtnSimpanOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            this.OnSaveItem(EventArgs.Empty);
        }

        private void BtnTutupOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            object parent = Manager.FindVisualParent<FrameworkElement>(this);
            if (parent != null)
            {
                while (!(parent is ICloseControl))
                {
                    var frameworkElement = parent as FrameworkElement;
                    if (frameworkElement != null)
                    {
                        parent = Manager.FindVisualParent<FrameworkElement>(frameworkElement);

                    }
                    else
                    {
                        var control = Application.Current.MainWindow as IMainWindow;
                        if (control != null)
                        {
                            var geDocumentCount = control.GetDocumentCount();
                            if (geDocumentCount == 0)
                                control.ClearAll = true;
                        }
                        break;
                    }
                }
                if (parent != null)
                {
                    (parent as ICloseControl).CloseControl();

                }
            }
        }

        #endregion
    }
}