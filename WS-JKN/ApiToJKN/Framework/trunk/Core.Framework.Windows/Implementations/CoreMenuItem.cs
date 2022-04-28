using Core.Framework.Windows.Controls;

namespace Core.Framework.Windows.Implementations
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Helper;

    public class CoreMenuItem : MenuItem, IControlToUseGenericCalling
    {
        #region Constructors and Destructors

        public CoreMenuItem()
        {
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Public Events

        [Category("Behavior")]
        public event RoutedEventHandler AfterExecuteControl;

        #endregion

        #region Public Properties

        public string ControlNameSpace { get; set; }
        public bool SortAutomatic { get; set; }

        #endregion

        #region Public Methods and Operators

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
            if (MetroWindow.Current != null)
                MetroWindow.Current.hotKeys = "";
        }

        public void ExecuteControl()
        {
            Manager.ExecuteGenericModule(this.ControlNameSpace, this, this.Tag);
            this.OnAfterExecuteControl();
        }

        public void OnAfterExecuteControl(RoutedEventArgs e = null)
        {
            RoutedEventHandler handler = this.AfterExecuteControl;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Methods

        protected override void OnClick()
        {
            this.Execute();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (Application.Current != null)
            {
                var style = Application.Current.Resources["MetroMenuItem"] as Style;
                this.Style = style;
            }
        }

        #endregion
    }
}