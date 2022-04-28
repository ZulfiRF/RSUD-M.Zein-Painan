namespace Core.Framework.Windows.Controls.Dialogs
{
    using System.Windows;

    /// <summary>
    ///     An internal control that represents a message dialog. Please use MetroWindow.ShowMessage instead!
    /// </summary>
    public partial class ProgressDialog : BaseMetroDialog
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

        public static readonly DependencyProperty IsCancelableProperty = DependencyProperty.Register(
            "IsCancelable",
            typeof(bool),
            typeof(ProgressDialog),
            new PropertyMetadata(
                default(bool),
                (s, e) =>
                {
                    ((ProgressDialog)s).PART_NegativeButton.Visibility = (bool)e.NewValue
                        ? Visibility.Visible
                        : Visibility.Hidden;
                }));

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message",
            typeof(string),
            typeof(ProgressDialog),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty NegativeButtonTextProperty =
            DependencyProperty.Register(
                "NegativeButtonText",
                typeof(string),
                typeof(ProgressDialog),
                new PropertyMetadata("Cancel"));

        #endregion

        #region Constructors and Destructors

        internal ProgressDialog(MetroWindow parentWindow)
            : base(parentWindow)
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Properties

        public bool IsCancelable
        {
            get
            {
                return (bool)this.GetValue(IsCancelableProperty);
            }
            set
            {
                this.SetValue(IsCancelableProperty, value);
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
    }
}