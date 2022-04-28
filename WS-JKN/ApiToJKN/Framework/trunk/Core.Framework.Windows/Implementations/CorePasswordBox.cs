using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Implementations
{
    public sealed class CorePasswordBox : CoreTextBox
    {

        #region Static Property
        public SecureString Password
        {
            get { return (SecureString)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
            "Password", typeof(SecureString), typeof(CorePasswordBox), new UIPropertyMetadata(new SecureString()));

        public string HiddenText
        {
            get { return (string)GetValue(HiddenTextProperty); }
            set { SetValue(HiddenTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HiddenTextProperty =
            DependencyProperty.Register("HiddenText", typeof(string), typeof(CorePasswordBox), new UIPropertyMetadata(string.Empty));
        #endregion


        private readonly DispatcherTimer maskTimer;

        public CorePasswordBox()
        {
            Manager.RegisterFormGrid(this);
            PreviewTextInput += OnPreviewTextInput;
            PreviewKeyDown += OnPreviewKeyDown;
            CommandManager.AddPreviewExecutedHandler(this, PreviewExecutedHandler);
            maskTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 1) };
            maskTimer.Tick += (sender, args) => MaskAllDisplayText();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            BorderBrush = new SolidColorBrush(Colors.Gray);
        }

        private void PreviewExecutedHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste)
                e.Handled = true;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var pressedKey = e.Key == System.Windows.Input.Key.System ? e.SystemKey : e.Key;
            switch (pressedKey)
            {
                case System.Windows.Input.Key.Space:
                    AddToSecureString(" ");
                    e.Handled = true;
                    break;
                case System.Windows.Input.Key.Back:
                case System.Windows.Input.Key.Delete:
                    if (SelectionLength > 0)
                        RemoveFromSecureString(SelectionStart, SelectionLength);
                    else if (pressedKey == System.Windows.Input.Key.Delete && CaretIndex < Text.Length)
                        RemoveFromSecureString(CaretIndex, 1);
                    else if (pressedKey == System.Windows.Input.Key.Back && CaretIndex > 0)
                    {
                        var caretIndex = CaretIndex;
                        if (CaretIndex > 0 && CaretIndex < Text.Length)
                            caretIndex = caretIndex - 1;
                        RemoveFromSecureString(CaretIndex - 1, 1);
                        CaretIndex = caretIndex;
                    }

                    e.Handled = true;
                    break;
            }
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "\r")
            {
                AddToSecureString(e.Text);
                e.Handled = true;
            }
        }

        private void AddToSecureString(string text)
        {
            if (SelectionLength > 0)
            {
                RemoveFromSecureString(SelectionStart, SelectionLength);
            }

            foreach (char c in text)
            {
                var caretIndex = CaretIndex;
                Password.InsertAt(caretIndex, c);
                HiddenText = HiddenText.Insert(caretIndex, c.ToString());
                MaskAllDisplayText();
                if (caretIndex == Text.Length)
                {
                    maskTimer.Stop();
                    maskTimer.Start();
                    Text = Text.Insert(caretIndex++, c.ToString());
                }
                else
                {
                    Text = Text.Insert(caretIndex++, "\u2022");
                }
                CaretIndex = caretIndex;
            }
        }

        private void RemoveFromSecureString(int startIndex, int trimLength)
        {
            var caretIndex = CaretIndex;
            for (var i = 0; i < trimLength; ++i)
            {
                Password.RemoveAt(startIndex);
                HiddenText = HiddenText.Remove(startIndex, 1);
            }

            Text = Text.Remove(startIndex, trimLength);
            CaretIndex = caretIndex;
        }

        private void MaskAllDisplayText()
        {
            maskTimer.Stop();
            var caretIndex = CaretIndex;
            Text = new string(Convert.ToChar("\u2022"), Text.Length);
            CaretIndex = caretIndex;
        }
    }
}
