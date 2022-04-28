namespace Core.Framework.Windows.Controls.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    ///     An internal control that represents a message dialog. Please use MetroWindow.ShowMessage instead!
    /// </summary>
    public partial class MessageDialog : BaseMetroDialog
    {
        //private const string PART_AffirmativeButton = "PART_AffirmativeButton";
        //private const string PART_NegativeButton = "PART_NegativeButton";

        //private Button AffirmativeButton = null;
        //private Button NegativeButton = null;

        //static MessageDialog()
        //{
        //    //DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageDialog), new FrameworkPropertyMetadata(typeof(MessageDialog)));
        //}

        #region Static Fields

        public static readonly DependencyProperty AffirmativeButtonTextProperty =
            DependencyProperty.Register(
                "AffirmativeButtonText",
                typeof(string),
                typeof(MessageDialog),
                new PropertyMetadata("OK"));

        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(
            "ButtonStyle",
            typeof(MessageDialogStyle),
            typeof(MessageDialog),
            new PropertyMetadata(
                MessageDialogStyle.Affirmative,
                (s, e) =>
                {
                    var md = (MessageDialog)s;

                    SetButtonState(md);
                }));

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message",
            typeof(string),
            typeof(MessageDialog),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty NegativeButtonTextProperty =
            DependencyProperty.Register(
                "NegativeButtonText",
                typeof(string),
                typeof(MessageDialog),
                new PropertyMetadata("Cancel"));

        #endregion

        #region Constructors and Destructors

        internal MessageDialog(MetroWindow parentWindow)
            : base(parentWindow)
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Properties

        public string AffirmativeButtonText
        {
            get
            {
                return (string)this.GetValue(AffirmativeButtonTextProperty);
            }
            set
            {
                this.SetValue(AffirmativeButtonTextProperty, value);
            }
        }

        public MessageDialogStyle ButtonStyle
        {
            get
            {
                return (MessageDialogStyle)this.GetValue(ButtonStyleProperty);
            }
            set
            {
                this.SetValue(ButtonStyleProperty, value);
            }
        }

        

        public string Message
        {
            get
            {
                return (string)this.GetValue(MessageProperty);
            }
            set
            {
                this.SetValue(MessageProperty, value);
            }
        }

        public string NegativeButtonText
        {
            get
            {
                return (string)this.GetValue(NegativeButtonTextProperty);
            }
            set
            {
                this.SetValue(NegativeButtonTextProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void OnApplyTemplate()
        {
            //AffirmativeButton = GetTemplateChild(PART_AffirmativeButton) as Button;
            //NegativeButton = GetTemplateChild(PART_NegativeButton) as Button;

            base.OnApplyTemplate();
        }

        #endregion

        #region Methods

        public override Task<MessageDialogResult> WaitForButtonPressAsync()
        {
            this.Dispatcher.BeginInvoke(
                new Action(
                    () =>
                    {
                        this.Focus();

                        //kind of acts like a selective 'IsDefault' mechanism.
                        if (this.ButtonStyle == MessageDialogStyle.Affirmative)
                        {
                            this.PART_AffirmativeButton.Focus();
                        }
                        else if (this.ButtonStyle == MessageDialogStyle.AffirmativeAndNegative)
                        {
                            this.PART_NegativeButton.Focus();
                        }
                    }));

            var tcs = new TaskCompletionSource<MessageDialogResult>();

            RoutedEventHandler negativeHandler = null;
            KeyEventHandler negativeKeyHandler = null;

            RoutedEventHandler affirmativeHandler = null;
            KeyEventHandler affirmativeKeyHandler = null;

            Action cleanUpHandlers = () =>
            {
                this.PART_NegativeButton.Click -= negativeHandler;
                this.PART_AffirmativeButton.Click -= affirmativeHandler;

                this.PART_NegativeButton.KeyDown -= negativeKeyHandler;
                this.PART_AffirmativeButton.KeyDown -= affirmativeKeyHandler;
            };

            negativeKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.Negative);
                }
            };

            affirmativeKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.Affirmative);
                }
            };

            negativeHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(MessageDialogResult.Negative);
                this.DialogResult = false;
                e.Handled = true;
            };

            affirmativeHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(MessageDialogResult.Affirmative);
                this.DialogResult = true;
                e.Handled = true;
            };

            this.PART_NegativeButton.KeyDown += negativeKeyHandler;
            this.PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;

            this.PART_NegativeButton.Click += negativeHandler;
            this.PART_AffirmativeButton.Click += affirmativeHandler;

            return tcs.Task;
        }

        private static void SetButtonState(MessageDialog md)
        {
            if (md.PART_AffirmativeButton == null)
            {
                return;
            }

            switch (md.ButtonStyle)
            {
                case MessageDialogStyle.Affirmative:
                {
                    md.PART_AffirmativeButton.Visibility = Visibility.Visible;
                    md.PART_NegativeButton.Visibility = Visibility.Collapsed;
                }
                    break;
                case MessageDialogStyle.AffirmativeAndNegative:
                {
                    md.PART_AffirmativeButton.Visibility = Visibility.Visible;
                    md.PART_NegativeButton.Visibility = Visibility.Visible;
                }
                    break;
            }
        }

        private void Dialog_Loaded(object sender, RoutedEventArgs e)
        {
            SetButtonState(this);
        }

        #endregion
    }
}