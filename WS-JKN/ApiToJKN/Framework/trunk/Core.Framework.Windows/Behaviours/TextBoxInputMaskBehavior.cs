using System.Diagnostics;

namespace Core.Framework.Windows.Behaviours
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    /// <summary>
    ///     InputMask for Textbox with 2 Properties: <see cref="InputMask" />, <see cref="PromptChar" />.
    /// </summary>
    public class TextBoxInputMaskBehavior : Behavior<TextBox>, IDisposable
    {
        #region Static Fields

        public static readonly DependencyProperty InputMaskProperty = DependencyProperty.Register(
            "InputMask",
            typeof(string),
            typeof(TextBoxInputMaskBehavior),
            null);

        public static readonly DependencyProperty PromptCharProperty = DependencyProperty.Register(
            "PromptChar",
            typeof(char),
            typeof(TextBoxInputMaskBehavior),
            new PropertyMetadata('_'));

        #endregion

        #region Fields


        #endregion

        #region Constructors and Destructors

        public TextBoxInputMaskBehavior()
        {
        }
        
        public TextBoxInputMaskBehavior(string inputMask, TextBox control, char prompchar = '_')
        {
            this.InputMask = inputMask;
            this.ControlTextBox = control;
            this.PromptChar = prompchar;
            this.BindingMask();
            this.ControlTextBox.PreviewTextInput += this.ControlTextBoxPreviewTextInput;
            this.ControlTextBox.PreviewKeyDown += this.ControlTextBoxPreviewKeyDown;
            this.ControlTextBox.GotFocus += ControlTextBoxOnGotFocus;
            this.ControlTextBox.LostFocus += this.ControlTextBoxOnLostFocus;
            DataObject.AddPastingHandler(this.ControlTextBox, this.Pasting);
        }

        private void ControlTextBoxOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            this.ControlTextBox.Text =
                    this.ControlTextBox.Text.Replace(this.PromptChar.ToString(CultureInfo.InvariantCulture), " ");
        }

        #endregion

        #region Public Properties

        public TextBox ControlTextBox { get; set; }

        public string InputMask
        {
            get
            {
                return (string)this.GetValue(InputMaskProperty);
            }
            set
            {
                this.SetValue(InputMaskProperty, value);
            }
        }

        public char PromptChar
        {
            get
            {
                return (char)this.GetValue(PromptCharProperty);
            }
            set
            {
                this.SetValue(PromptCharProperty, value);
            }
        }

        public MaskedTextProvider Provider { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void BindingMask()
        {
            if (this.ControlTextBox == null)
            {
                return;
            }
            try
            {
                this.Provider = new MaskedTextProvider(this.InputMask, CultureInfo.CurrentCulture);
                this.Provider.Set(this.ControlTextBox.Text);
                this.Provider.PromptChar = this.PromptChar;
                this.ControlTextBox.Text = this.Provider.ToDisplayString();

                //seems the only way that the text is formatted correct, when source is updated
                DependencyPropertyDescriptor textProp = DependencyPropertyDescriptor.FromProperty(
                    TextBox.TextProperty,
                    typeof(TextBox));
                if (textProp != null)
                {
                    textProp.AddValueChanged(this.ControlTextBox, (s, args) => this.UpdateText());
                }
            }
            catch (Exception)
            {
            }
        }

        public void Clear()
        {
            if (this.ControlTextBox == null)
            {
                return;
            }
            this.ControlTextBox.PreviewTextInput -= this.ControlTextBoxPreviewTextInput;
            this.ControlTextBox.PreviewKeyDown -= this.ControlTextBoxPreviewKeyDown;
            this.ControlTextBox.LostFocus -= this.ControlTextBoxOnLostFocus;
            DataObject.RemovePastingHandler(this.ControlTextBox, this.Pasting);
            //DependencyPropertyDescriptor textProp = DependencyPropertyDescriptor.FromProperty(TextBox.TextProperty,
            //                                                                                     typeof(TextBox));
            //if (textProp != null)
            //{
            //    textProp.RemoveValueChanged(ControlTextBox, (s, args) => {});
            //}
            try
            {
                this.Provider = null;

                //seems the only way that the text is formatted correct, when source is updated
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this.ControlTextBox != null)
            {
                
                this.ControlTextBox.PreviewTextInput -= this.ControlTextBoxPreviewTextInput;
                this.ControlTextBox.PreviewKeyDown -= this.ControlTextBoxPreviewKeyDown;
                this.ControlTextBox.LostFocus -= this.ControlTextBoxOnLostFocus;
                DataObject.RemovePastingHandler(this.ControlTextBox, this.Pasting);
                this.InputMask = null;
                this.ControlTextBox = null;                
            }
        }

        #endregion

        #region Methods

        protected override void OnAttached()
        {
            base.OnAttached();
            if (this.ControlTextBox != null)
            {
                this.ControlTextBox.Loaded += this.ControlTextBoxLoaded;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            //ControlTextBox.Loaded -= ControlTextBoxLoaded;
            //ControlTextBox.PreviewTextInput -= ControlTextBoxPreviewTextInput;
            //ControlTextBox.PreviewKeyDown -= ControlTextBoxPreviewKeyDown;

            //DataObject.RemovePastingHandler(ControlTextBox, Pasting);
        }

        /*
        Mask Character  Accepts  Required?  
        0  Digit (0-9)  Required  
        9  Digit (0-9) or space  Optional  
        #  Digit (0-9) or space  Required  
        L  Letter (a-z, A-Z)  Required  
        ?  Letter (a-z, A-Z)  Optional  
        &  Any character  Required  
        C  Any character  Optional  
        A  Alphanumeric (0-9, a-z, A-Z)  Required  
        a  Alphanumeric (0-9, a-z, A-Z)  Optional  
           Space separator  Required 
        .  Decimal separator  Required  
        ,  Group (thousands) separator  Required  
        :  Time separator  Required  
        /  Date separator  Required  
        $  Currency symbol  Required  

        In addition, the following characters have special meaning:

        Mask Character  Meaning  
        <  All subsequent characters are converted to lower case  
        >  All subsequent characters are converted to upper case  
        |  Terminates a previous < or >  
        \  Escape: treat the next character in the mask as literal text rather than a mask symbol  

        */

        private void ControlTextBoxLoaded(object sender, RoutedEventArgs e)
        {
            this.BindingMask();
        }

        private void ControlTextBoxOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                this.ControlTextBox.Text =
                    this.ControlTextBox.Text.Replace(this.PromptChar.ToString(CultureInfo.InvariantCulture), " ");
            }
            catch (Exception)
            {
            }
        }

        private void ControlTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space) //handle the space
                {
                    this.TreatSelectedText();

                    int position = this.GetNextCharacterPosition(this.ControlTextBox.SelectionStart);

                    if (this.Provider.InsertAt(" ", position))
                    {
                        this.RefreshText(position);
                    }

                    e.Handled = true;
                }

                if (e.Key == Key.Back) //handle the back space
                {
                    if (this.TreatSelectedText())
                    {
                        this.RefreshText(this.ControlTextBox.SelectionStart);
                    }
                    else
                    {
                        if (this.ControlTextBox.SelectionStart != 0)
                        {
                            if (this.Provider.RemoveAt(this.ControlTextBox.SelectionStart - 1))
                            {
                                this.RefreshText(this.ControlTextBox.SelectionStart - 1);
                            }
                        }
                    }

                    e.Handled = true;
                }

                if (e.Key == Key.Delete) //handle the delete key
                {
                    //treat selected text
                    if (this.TreatSelectedText())
                    {
                        this.RefreshText(this.ControlTextBox.SelectionStart);
                    }
                    else
                    {
                        if (this.Provider.RemoveAt(this.ControlTextBox.SelectionStart))
                        {
                            this.RefreshText(this.ControlTextBox.SelectionStart);
                        }
                    }

                    e.Handled = true;
                }
            }
            catch (Exception)
            {
            }
        }

        private void ControlTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                this.TreatSelectedText();

                int position = this.GetNextCharacterPosition(this.ControlTextBox.SelectionStart);

                if (Keyboard.IsKeyToggled(Key.Insert))
                {
                    if (this.Provider.Replace(e.Text, position))
                    {
                        position++;
                    }
                }
                else
                {
                    if (this.Provider.InsertAt(e.Text, position))
                    {
                        position++;
                    }
                }

                position = this.GetNextCharacterPosition(position);

                this.RefreshText(position);

                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }

        private int GetNextCharacterPosition(int startPosition)
        {
            int position = this.Provider.FindEditPositionFrom(startPosition, true);

            if (position == -1)
            {
                return startPosition;
            }
            return position;
        }

        /// <summary>
        ///     Pasting prüft ob korrekte Daten reingepastet werden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var pastedText = (string)e.DataObject.GetData(typeof(string));

                this.TreatSelectedText();

                int position = this.GetNextCharacterPosition(this.ControlTextBox.SelectionStart);

                if (this.Provider.InsertAt(pastedText, position))
                {
                    this.RefreshText(position);
                }
            }

            e.CancelCommand();
        }

        private void RefreshText(int position)
        {
            this.SetText(this.Provider.ToDisplayString());
            this.ControlTextBox.SelectionStart = position;
        }

        private void SetText(string text)
        {
            this.ControlTextBox.Text = String.IsNullOrWhiteSpace(text) ? String.Empty : text;
        }

        /// <summary>
        ///     Falls eine Textauswahl vorliegt wird diese entsprechend behandelt.
        /// </summary>
        /// <returns>true Textauswahl behandelt wurde, ansonsten falls </returns>
        private bool TreatSelectedText()
        {
            if (this.ControlTextBox.SelectionLength > 0)
            {
                return this.Provider.RemoveAt(
                    this.ControlTextBox.SelectionStart,
                    this.ControlTextBox.SelectionStart + this.ControlTextBox.SelectionLength - 1);
            }
            return false;
        }

        private void UpdateText()
        {
            if (this.Provider == null)
            {
                return;
            }
            if (this.ControlTextBox == null)
            {
                return;
            }
            //check Provider.Text + TextBox.Text
            if (this.Provider.ToDisplayString().Equals(this.ControlTextBox.Text))
            {
                return;
            }

            //use provider to format
            bool success = this.Provider.Set(this.ControlTextBox.Text);

            //ui and mvvm/codebehind should be in sync
            this.SetText(success ? this.Provider.ToDisplayString() : this.ControlTextBox.Text);
        }

        #endregion
    }
}