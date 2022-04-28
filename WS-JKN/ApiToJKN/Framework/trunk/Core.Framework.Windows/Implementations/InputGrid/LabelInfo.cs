using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Core.Framework.Helper;

namespace Core.Framework.Windows.Implementations.InputGrid
{
    /// <summary>
    /// Interaction logic for LabelInfo.xaml
    /// </summary>
    public partial class LabelInfo : UserControl
    {
        public LabelInfo()
        {
            InitializeComponent();
            Guid = this.GetType().GUID;
        }

        public Guid Guid { get; set; }

        public string Text
        {
            get
            {
                if (LimitShowText == -1)
                    return (string)GetValue(TextProperty);
                if (ToolTip == null)
                    return (string)GetValue(TextProperty);
                return ToolTip.ToString();
            }
            set { SetValue(TextProperty, value); }
        }



        public int LimitShowText
        {
            get { return (int)GetValue(LimitShowTextProperty); }
            set { SetValue(LimitShowTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LimitShowText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LimitShowTextProperty =
            DependencyProperty.Register("LimitShowText", typeof(int), typeof(LabelInfo), new UIPropertyMetadata(-1, ChangeShowLabel));

        private static void ChangeShowLabel(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var form = dependencyObject as LabelInfo;
            if (form != null)
            {
                if (e.NewValue == null)
                {
                    form.textBlock.Text = "";
                    form.ToolTip = "";
                }
                else
                {
                    if (form.LimitShowText == -1)
                        form.textBlock.Text = form.ToolTip.ToString();
                    else
                        form.textBlock.Text = form.ToolTip.ToString().Substring(form.LimitShowText) + "...";
                    //form.ToolTip = e.NewValue.ToString();
                }
            }
        }


        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LabelInfo), new UIPropertyMetadata("", TextChange));

        private static void TextChange(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var form = dependencyObject as LabelInfo;
            if (form != null)
            {
                if (e.NewValue == null)
                {
                    form.textBlock.Text = "";
                    form.ToolTip = "";
                }
                else
                {
                    if (form.LimitShowText == -1)
                        form.textBlock.Text = e.NewValue.ToString();
                    else
                        form.textBlock.Text = e.NewValue.ToString().Substring(form.LimitShowText) + "...";
                    form.ToolTip = e.NewValue.ToString();
                }
            }
        }
    }
}
