using System;
using System.Threading.Tasks;
using System.Windows;

namespace Core.Framework.Windows.Controls.Dialogs
{
    /// <summary>
    ///     A class for manipulating an open ProgressDialog.
    /// </summary>
    public class ProgressDialogController
    {
        //No spiritdead, you can't change this.

        #region Constructors and Destructors

        internal ProgressDialogController(ProgressDialog dialog, Func<Task> closeCallBack)
        {
            this.WrappedDialog = dialog;
            this.CloseCallback = closeCallBack;

            this.IsOpen = dialog.IsVisible;

            this.WrappedDialog.PART_NegativeButton.Dispatcher.Invoke(
                new Action(() => { this.WrappedDialog.PART_NegativeButton.Click += this.PART_NegativeButton_Click; }));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets if the Cancel button has been pressed.
        /// </summary>
        public bool IsCanceled { get; private set; }

        /// <summary>
        ///     Gets if the wrapped ProgressDialog is open.
        /// </summary>
        public bool IsOpen { get; private set; }

        #endregion

        #region Properties

        private Func<Task> CloseCallback { get; set; }
        private ProgressDialog WrappedDialog { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Begins an operation to close the ProgressDialog.
        /// </summary>
        /// <returns>A task representing the operation.</returns>
        public Task CloseAsync()
        {
            Action action = () =>
                                {
                                    if (!this.IsOpen)
                                    {
                                        throw new InvalidOperationException();
                                    }
                                    this.WrappedDialog.Dispatcher.VerifyAccess();
                                    this.WrappedDialog.PART_NegativeButton.Click -= this.PART_NegativeButton_Click;
                                };

            if (this.WrappedDialog.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                this.WrappedDialog.Dispatcher.Invoke(action);
            }

            return
                this.CloseCallback()
                    .ContinueWith(x => this.WrappedDialog.Dispatcher.Invoke(new Action(() => { this.IsOpen = false; })));
        }

        /// <summary>
        ///     Sets if the Cancel button is visible.
        /// </summary>
        /// <param name="value"></param>
        public void SetCancelable(bool value)
        {
            if (this.WrappedDialog.Dispatcher.CheckAccess())
            {
                this.WrappedDialog.IsCancelable = value;
            }
            else
            {
                this.WrappedDialog.Dispatcher.Invoke(new Action(() => { this.WrappedDialog.IsCancelable = value; }));
            }
        }

        /// <summary>
        ///     Sets the ProgressBar's IsIndeterminate to true. To set it to false, call SetProgress.
        /// </summary>
        public void SetIndeterminate()
        {
            if (this.WrappedDialog.Dispatcher.CheckAccess())
            {
                this.WrappedDialog.PART_ProgressBar.IsIndeterminate = true;
            }
            else
            {
                this.WrappedDialog.Dispatcher.Invoke(
                    new Action(() => { this.WrappedDialog.PART_ProgressBar.IsIndeterminate = true; }));
            }
        }

        /// <summary>
        ///     Sets the dialog's message content.
        /// </summary>
        /// <param name="message">The message to be set.</param>
        public void SetMessage(string message)
        {
            if (this.WrappedDialog.Dispatcher.CheckAccess())
            {
                this.WrappedDialog.Message = message;
            }
            else
            {
                this.WrappedDialog.Dispatcher.Invoke(new Action(() => { this.WrappedDialog.Message = message; }));
            }
        }

        /// <summary>
        ///     Sets the dialog's progress bar value and sets IsIndeterminate to false.
        /// </summary>
        /// <param name="value">The percentage to set as the value.</param>
        public void SetProgress(double value)
        {
            if (value < 0.0 || value > 1.0)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            Action action = () =>
                                {
                                    this.WrappedDialog.PART_ProgressBar.IsIndeterminate = false;
                                    this.WrappedDialog.PART_ProgressBar.Value = value;
                                    this.WrappedDialog.PART_ProgressBar.Maximum = 1.0;
                                    this.WrappedDialog.PART_ProgressBar.ApplyTemplate();
                                };

            if (this.WrappedDialog.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                this.WrappedDialog.Dispatcher.Invoke(action);
            }
        }

        #endregion

        #region Methods

        private void PART_NegativeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WrappedDialog.Dispatcher.CheckAccess())
            {
                this.IsCanceled = true;

                this.WrappedDialog.PART_NegativeButton.IsEnabled = false;
            }
            this.WrappedDialog.Dispatcher.Invoke(
                new Action(
                    () =>
                        {
                            this.IsCanceled = true;

                            this.WrappedDialog.PART_NegativeButton.IsEnabled = false;
                        }));

            //Close();
        }

        #endregion
    }
}