using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Helper;
using Core.Framework.Windows.Implementations;

namespace Core.Framework.Windows.Ribbon.UI
{
    /// <summary>
    /// Interaction logic for ApplicationMenuButton.xaml
    /// </summary>
    public class ApplicationMenuButton : Button, IControlToUseGenericCalling
    {
        private DispatcherTimer timer;
        private string controlNameSpace;

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
        #region Construction

        static ApplicationMenuButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApplicationMenuButton),
             new FrameworkPropertyMetadata(typeof(ApplicationMenuButton)));
        }

        public ApplicationMenuButton()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerOnTick;
            if (Application.Current != null)
                Style = Application.Current.Resources["ApplicationMenuButtonStyle"] as Style;
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

        #endregion

        #region Implementation of IControlToUseGenericCalling

        public string ControlNameSpace
        {
            get { return controlNameSpace; }
            set { controlNameSpace = value; }
        }
        protected override void OnClick()
        {
            this.Execute();
        }
        public void ExecuteControl()
        {
            Manager.ExecuteGenericModule(this.ControlNameSpace, this, this.Tag);
            this.OnAfterExecuteControl();
        }
        [Category("Behavior")]
        public event RoutedEventHandler AfterExecuteControl;

        public void OnAfterExecuteControl(RoutedEventArgs e = null)
        {
            RoutedEventHandler handler = this.AfterExecuteControl;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public void Execute()
        {
            if (!string.IsNullOrEmpty(this.ControlNameSpace))
            {
                this.ExecuteControl();
            }
            else
            {
                base.OnClick();
            }
        }
        #endregion
    }
}
