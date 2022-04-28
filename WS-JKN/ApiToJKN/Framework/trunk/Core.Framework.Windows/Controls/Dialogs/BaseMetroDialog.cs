using System.Windows.Controls;

namespace Core.Framework.Windows.Controls.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    public abstract class BaseMetroDialog : Control
    {
        #region Static Fields

        public static readonly DependencyProperty DialogBodyProperty = DependencyProperty.Register(
            "DialogBody",
            typeof(object),
            typeof(BaseMetroDialog),
            new PropertyMetadata(null));

        public static readonly DependencyProperty DialogBottomProperty = DependencyProperty.Register(
            "DialogBottom",
            typeof(object),
            typeof(BaseMetroDialog),
            new PropertyMetadata(null));

        public static readonly DependencyProperty DialogTopProperty = DependencyProperty.Register(
            "DialogTop",
            typeof(object),
            typeof(BaseMetroDialog),
            new PropertyMetadata(null));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(BaseMetroDialog),
            new PropertyMetadata(default(string)));

        #endregion

        #region Constructors and Destructors

        static BaseMetroDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(BaseMetroDialog),
                new FrameworkPropertyMetadata(typeof(BaseMetroDialog)));
        }

        /// <summary>
        ///     Initializes a new Core.Framework.Windows.Controls.BaseMetroDialog.
        /// </summary>
        /// <param name="owningWindow">The window that is the parent of the dialog.</param>
        public BaseMetroDialog(MetroWindow owningWindow)
        {
            switch (owningWindow.MetroDialogOptions.ColorScheme)
            {
                case MetroDialogColorScheme.Theme:
                    this.SetResourceReference(BackgroundProperty, "WhiteColorBrush");
                    break;
                case MetroDialogColorScheme.Accented:
                    this.SetResourceReference(BackgroundProperty, "AccentColorBrush");
                    this.SetResourceReference(ForegroundProperty, "IdealForegroundColorBrush");
                    break;
            }
        }

        /// <summary>
        ///     Initializes a new Core.Framework.Windows.Controls.BaseMetroDialog.
        /// </summary>
        public BaseMetroDialog()
        {
        }

        #endregion

        #region Public Properties

        public bool DialogResult { get; set; }

        /// <summary>
        ///     Gets/sets arbitrary content in the "message" area in the dialog.
        /// </summary>
        public object DialogBody
        {
            get
            {
                return this.GetValue(DialogBodyProperty);
            }
            set
            {
                this.SetValue(DialogBodyProperty, value);
            }
        }

        /// <summary>
        ///     Gets/sets arbitrary content below the dialog.
        /// </summary>
        public object DialogBottom
        {
            get
            {
                return this.GetValue(DialogBottomProperty);
            }
            set
            {
                this.SetValue(DialogBottomProperty, value);
            }
        }

        /// <summary>
        ///     Gets/sets arbitrary content on top of the dialog.
        /// </summary>
        public object DialogTop
        {
            get
            {
                return this.GetValue(DialogTopProperty);
            }
            set
            {
                this.SetValue(DialogTopProperty, value);
            }
        }

        /// <summary>
        ///     Gets/sets the dialog's title.
        /// </summary>
        public string Title
        {
            get
            {
                return (string)this.GetValue(TitleProperty);
            }
            set
            {
                this.SetValue(TitleProperty, value);
            }
        }

        #endregion

        #region Properties

        internal SizeChangedEventHandler SizeChangedHandler { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Waits for the dialog to become ready for interaction.
        /// </summary>
        /// <returns>A task that represents the operation and it's status.</returns>
        public Task WaitForLoadAsync()
        {
            this.Dispatcher.VerifyAccess();

            if (this.IsLoaded)
            {
                return new Task(() => { });
            }

            var tcs = new TaskCompletionSource<object>();

            RoutedEventHandler handler = null;
            handler = (sender, args) =>
            {
                this.Loaded -= handler;

                tcs.TrySetResult(null);
            };

            this.Loaded += handler;

            return tcs.Task;
        }

        #endregion


        #region Methods

        public virtual Task<MessageDialogResult> WaitForButtonPressAsync()
        {
            return null;
        }
        protected internal virtual void OnClose()
        {
        }

        protected internal virtual void OnShown()
        {
        }

        #endregion
    }
}