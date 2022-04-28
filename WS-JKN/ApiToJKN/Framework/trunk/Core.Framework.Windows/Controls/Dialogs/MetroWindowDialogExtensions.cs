﻿using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Controls.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;


    public static class MetroWindowDialogExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Hides a visible Metro Dialog instance.
        /// </summary>
        /// <param name="window">The window with the dialog that is visible.</param>
        /// <param name="dialog">The dialog instance to hide.</param>
        /// <returns>A task representing the operation.</returns>
        /// <exception cref="InvalidOperationException">
        ///     The <paramref name="dialog" /> is not visible in the window.
        ///     This happens if <see cref="ShowMetroDialogAsync" /> hasn't been called before.
        /// </exception>
        public static Task HideMetroDialogAsync(this MetroWindow window, BaseMetroDialog dialog)
        {
            window.Dispatcher.VerifyAccess();
            if (!window.messageDialogContainer.Children.Contains(dialog))
            {
                throw new InvalidOperationException("The provided dialog is not visible in the specified window.");
            }

            window.SizeChanged -= dialog.SizeChangedHandler;

            window.messageDialogContainer.Children.Remove(dialog); //remove the dialog from the container

            return window.HideOverlayAsync();
        }

        /// <summary>
        ///     Creates a MessageDialog inside of the current window.
        /// </summary>
        /// <param name="title">The title of the MessageDialog.</param>
        /// <param name="message">The message contained within the MessageDialog.</param>
        /// <param name="style">The type of buttons to use.</param>
        /// <returns>A task promising the result of which button was pressed.</returns>
        public static Task<MessageDialogResult> ShowMessageAsync(
            this MetroWindow window,
            string title,
            string message,
            MessageDialogStyle style = MessageDialogStyle.Affirmative,
            Action<bool> competeAction = null,BaseMetroDialog dialogControl = null)
        {
            window.Dispatcher.VerifyAccess();
            return window.ShowOverlayAsync().ContinueWith(
                i =>
                {
                    return (Task<MessageDialogResult>)window.Dispatcher.Invoke(
                        new Func<Task<MessageDialogResult>>(
                            () =>
                            {
                                //create the dialog control
                                BaseMetroDialog dialog=null;
                                if (dialogControl == null)
                                {
                                   var messageDialog=  new MessageDialog(window);
                                   ((MessageDialog)messageDialog).Message = message;
                                   messageDialog.Title = title;
                                   ((MessageDialog)messageDialog).ButtonStyle = style;

                                   ((MessageDialog)messageDialog).AffirmativeButtonText = window.MetroDialogOptions.AffirmativeButtonText;
                                   ((MessageDialog)messageDialog).NegativeButtonText = window.MetroDialogOptions.NegativeButtonText;
                                    dialog = messageDialog;
                                }
                                else
                                {
                                    dialog = dialogControl;
                                }
                                

                                SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, dialog);
                                dialog.SizeChangedHandler = sizeHandler;

                                return dialog.WaitForLoadAsync().ContinueWith(
                                    x =>
                                    {
                                        return dialog.WaitForButtonPressAsync().ContinueWith(
                                            y =>
                                            {
                                                //once a button as been clicked, begin removing the dialog.

                                                dialog.OnClose();

                                                return ((Task)window.Dispatcher.Invoke(
                                                    new Func<Task>(
                                                        () =>
                                                        {
                                                            window.SizeChanged -= sizeHandler;

                                                            window.messageDialogContainer.Children.Remove(dialog);
                                                                //remove the dialog from the container

                                                            return window.HideOverlayAsync().ContinueWith(
                                                                n => Manager.Timeout(
                                                                    window.Dispatcher,
                                                                    () =>
                                                                    {
                                                                        if (competeAction != null)
                                                                        {
                                                                            competeAction.Invoke(
                                                                                dialog.DialogResult);
                                                                        }
                                                                    }));
                                                            //window.overlayBox.Visibility = System.Windows.Visibility.Hidden; //deactive the overlay effect
                                                        }))).ContinueWith(y3 => y).Unwrap();
                                            }).Unwrap();
                                    }).Unwrap();
                            }));
                }).Unwrap();
        }

        /// <summary>
        ///     Adds a Metro Dialog instance to the specified window and makes it visible.
        /// </summary>
        /// <param name="window">The owning window of the dialog.</param>
        /// <param name="title">The title to be set in the dialog.</param>
        /// <param name="dialog">The dialog instance itself.</param>
        /// <returns>A task representing the operation.</returns>
        /// <exception cref="InvalidOperationException">The <paramref name="dialog" /> is already visible in the window.</exception>
        public static Task ShowMetroDialogAsync(this MetroWindow window, BaseMetroDialog dialog)
        {
            window.Dispatcher.VerifyAccess();
            if (window.messageDialogContainer.Children.Contains(dialog))
            {
                throw new InvalidOperationException("The provided dialog is already visible in the specified window.");
            }

            return window.ShowOverlayAsync().ContinueWith(
                z =>
                {
                    dialog.Dispatcher.Invoke(
                        new Action(
                            () =>
                            {
                                SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, dialog);
                                dialog.SizeChangedHandler = sizeHandler;
                            }));
                }).ContinueWith(y => ((Task)dialog.Dispatcher.Invoke(new Func<Task>(() => dialog.WaitForLoadAsync()))));
        }

        /// <summary>
        ///     Creates a ProgressDialog inside of the current window.
        /// </summary>
        /// <param name="title">The title of the ProgressDialog.</param>
        /// <param name="message">The message within the ProgressDialog.</param>
        /// <param name="isCancelable">Determines if the cancel button is visible.</param>
        /// <returns>A task promising the instance of ProgressDialogController for this operation.</returns>
        public static Task<ProgressDialogController> ShowProgressAsync(
            this MetroWindow window,
            string title,
            string message,
            bool isCancelable = false)
        {
            window.Dispatcher.VerifyAccess();

            return window.ShowOverlayAsync().ContinueWith(
                z =>
                {
                    return
                        ((Task<ProgressDialogController>)
                            window.Dispatcher.Invoke(
                                new Func<Task<ProgressDialogController>>(
                                    () =>
                                    {
                                        //create the dialog control
                                        var dialog = new ProgressDialog(window);
                                        dialog.Message = message;
                                        dialog.Title = title;
                                        dialog.IsCancelable = isCancelable;
                                        dialog.NegativeButtonText = window.MetroDialogOptions.NegativeButtonText;
                                        SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, dialog);
                                        dialog.SizeChangedHandler = sizeHandler;

                                        return dialog.WaitForLoadAsync().ContinueWith(
                                            x =>
                                            {
                                                return new ProgressDialogController(
                                                    dialog,
                                                    () =>
                                                    {
                                                        dialog.OnClose();

                                                        return (Task)window.Dispatcher.Invoke(
                                                            new Func<Task>(
                                                                () =>
                                                                {
                                                                    window.SizeChanged -= sizeHandler;

                                                                    window.messageDialogContainer.Children.Remove(
                                                                        dialog); //remove the dialog from the container

                                                                    return window.HideOverlayAsync();
                                                                    //window.overlayBox.Visibility = System.Windows.Visibility.Hidden; //deactive the overlay effect
                                                                }));
                                                    });
                                            });
                                    })));
                }).Unwrap();
        }

        #endregion

        #region Methods

        private static SizeChangedEventHandler SetupAndOpenDialog(MetroWindow window, BaseMetroDialog dialog)
        {
            dialog.SetValue(Panel.ZIndexProperty, (int)window.overlayBox.GetValue(Panel.ZIndexProperty) + 1);
            dialog.MinHeight = window.ActualHeight / 4.0;

            SizeChangedEventHandler sizeHandler = null; //an event handler for auto resizing an open dialog.
            sizeHandler = (sender, args) => { dialog.MinHeight = window.ActualHeight / 4.0; };

            window.SizeChanged += sizeHandler;

            //window.overlayBox.Visibility = Visibility.Visible; //activate the overlay effect

            window.messageDialogContainer.Children.Add(dialog); //add the dialog to the container

            dialog.OnShown();

            if (window.TextBlockStyle != null && !dialog.Resources.Contains(typeof(TextBlock)))
            {
                dialog.Resources.Add(typeof(TextBlock), window.TextBlockStyle);
            }
            return sizeHandler;
        }

        #endregion
    }
}