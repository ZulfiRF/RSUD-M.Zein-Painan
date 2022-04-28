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
using System.Windows.Threading;
using Core.Framework.Windows.Implementations;

namespace Core.Framework.Windows.Ribbon.UI
{
    /// <summary>
    /// Interaction logic for ApplicationMenuDetailHeader.xaml
    /// </summary>
    public partial class ApplicationMenuDetailHeader
    {
        private DispatcherTimer timer;
        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (Parent != null)
            {
                if (Parent is CoreWrapPanel)
                {
                    timer.Start();
                    var key = e.Key.ToString();
                    if (key.ToLower().Equals("Space"))
                        key = " ";
                    var panel = Parent as CoreWrapPanel;
                    if (panel.Tag == null)
                        panel.Tag = key;
                    else
                    {
                        panel.Tag += key;
                    }
                    var applicationMenuButton = panel.Children.OfType<ApplicationMenuButton>().FirstOrDefault(n => n.Content.ToString().ToLower().Contains(panel.Tag.ToString().ToLower()));
                    bool focus = applicationMenuButton != null && applicationMenuButton.Focus();
                }
            }
            base.OnPreviewKeyDown(e);
        }
        public ApplicationMenuDetailHeader()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerOnTick;
            LbTitle.MouseLeftButtonDown+=LbTitleOnMouseLeftButtonDown;
        }

        private void LbTitleOnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Focus();
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            timer.Stop();
            if (Parent != null)
            {
                if (Parent is CoreWrapPanel)
                {
                    var panel = Parent as CoreWrapPanel;
                    panel.Tag = null;
                }
            }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ApplicationMenuDetailHeader), new UIPropertyMetadata("Header", PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var form = dependencyObject as ApplicationMenuDetailHeader;
            if (form != null)
            {
                form.LbTitle.Text = dependencyPropertyChangedEventArgs.NewValue.ToString();
            }
        }
    }
}
