using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Core.Framework.Helper;
using Core.Framework.Model;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Implementations
{
    public sealed class CoreTabItem : TabItem, IValidateControl, IControlToUseGenericCalling
    {
        #region Public Properties

        public string ControlNameSpace { get; set; }

        #endregion

        #region Public Methods and Operators

        public void ExecuteControl()
        {
            Manager.ExecuteGenericModule(this.ControlNameSpace);
        }

        #endregion
        #region Constructors and Destructors

        public CoreTabItem()
        {
            this.OnApplyTemplate();
            Loaded += OnLoaded;            
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                if (!string.IsNullOrEmpty(Ruangan))
                {
                    if (Ruangan.Contains(","))
                    {
                        var split = Ruangan.Split(',');
                        foreach (var ruangan in split)
                        {
                            var ruanganLogin = BaseDependency.Get<IHistoryLoginRepository>();
                            if (ruanganLogin.Ruangan == ruangan)
                            {
                                Visibility = Visibility.Visible;
                                break;
                            }
                            else
                            {
                                Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                    else
                    {
                        var ruanganLogin = BaseDependency.Get<IHistoryLoginRepository>();
                        if (ruanganLogin.Ruangan == Ruangan)
                        {
                            Visibility = Visibility.Visible;
                        }
                        else
                        {
                            Visibility = Visibility.Collapsed;
                        }
                    }                    
                }
            }
            catch (Exception)
            {

            }            
        }

        #endregion

        #region Public Events

        public event EventHandler<HandleArgs> BeforeValidate;

        #endregion

        #region Public Properties

        public bool IsError { get; set; }
        public bool IsNull { get; private set; }
        public bool IsRequired { get; set; }
        public bool SkipAutoFocus { get; set; }

        public string Ruangan
        {
            get { return (string)GetValue(RuanganProperty); }
            set { SetValue(RuanganProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RuanganProperty =
            DependencyProperty.Register("Ruangan", typeof(string), typeof(CoreTabItem), new PropertyMetadata(""));





        #endregion

        #region Public Methods and Operators

        public void ClearValueControl()
        {
        }

        public void FocusControl()
        {
        }

        #endregion

        private void OnBeforeValidate(HandleArgs e)
        {
            var handler = BeforeValidate;
            if (handler != null) handler(this, e);
        }
    }
}