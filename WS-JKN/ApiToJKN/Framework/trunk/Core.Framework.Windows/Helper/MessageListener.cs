using Core.Framework.Helper.Extention;

namespace Core.Framework.Windows.Helper
{
    using System.Diagnostics;
    using System.Windows;

    /// <summary>
    ///     Message listener, singlton pattern.
    ///     Inherit from DependencyObject to implement DataBinding.
    /// </summary>
    public class MessageListener : DependencyObject
    {
        #region Static Fields

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message",
            typeof(string),
            typeof(MessageListener),
            new UIPropertyMetadata(null));

        public static readonly DependencyProperty PercentaceProgressProperty =
            DependencyProperty.Register(
                "PercentaceProgress",
                typeof(double),
                typeof(MessageListener),
                new UIPropertyMetadata(0.0));

        public static readonly DependencyProperty SumProgressProperty =
          DependencyProperty.Register(
              "SumProgress",
              typeof(double),
              typeof(MessageListener),
              new UIPropertyMetadata(0.0));

        /// <summary>
        /// </summary>
        private static MessageListener mInstance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        private MessageListener()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Get MessageListener instance
        /// </summary>
        public static MessageListener Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new MessageListener();
                }
                return mInstance;
            }
        }

        /// <summary>
        ///     Get or set received message
        /// </summary>
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

        public string Percent
        {
            get { return (string)GetValue(PercentProperty); }
            set { SetValue(PercentProperty, value); }
        }

        public static readonly DependencyProperty PercentProperty =
            DependencyProperty.Register("Percent", typeof(string), typeof(MessageListener), new PropertyMetadata("0%"));



        public double PercentaceProgress
        {
            get
            {
                return (double)this.GetValue(PercentaceProgressProperty);
            }
            set
            {
                this.SetValue(PercentaceProgressProperty, value);
            }
        }

        public double SumProgress
        {
            get
            {
                return (double)this.GetValue(SumProgressProperty);
            }
            set
            {
                this.SetValue(SumProgressProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        public void ReceiveMessage(string message)
        {
            this.Message = message;
            //Debug.WriteLine(this.Message);
            DispatcherHelper.DoEvents();
        }

        public void UpdatePrecentace(double value)
        {
            this.PercentaceProgress = value;
            Debug.WriteLine(this.Message);
            DispatcherHelper.DoEvents();
        }

        #endregion
    }
}