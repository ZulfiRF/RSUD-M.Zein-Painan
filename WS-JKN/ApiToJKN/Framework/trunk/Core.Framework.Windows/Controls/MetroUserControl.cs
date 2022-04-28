using Core.Framework.Windows.Implementations;

namespace Core.Framework.Windows.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Animation;


    public class MetroUserControl : UserControl
    {
        #region Static Fields

        public static readonly DependencyProperty GlowBrushProperty = DependencyProperty.Register(
            "GlowBrush",
            typeof(SolidColorBrush),
            typeof(MetroUserControl),
            new PropertyMetadata(null));

        public static readonly DependencyProperty IgnoreTaskbarOnMaximizeProperty =
            DependencyProperty.Register(
                "IgnoreTaskbar",
                typeof(bool),
                typeof(MetroUserControl),
                new PropertyMetadata(false));

        public static readonly DependencyProperty SaveWindowPositionProperty =
            DependencyProperty.Register(
                "SaveWindowPosition",
                typeof(bool),
                typeof(MetroUserControl),
                new PropertyMetadata(false));

        public static readonly DependencyProperty ShowCloseButtonProperty =
            DependencyProperty.Register(
                "ShowCloseButton",
                typeof(bool),
                typeof(MetroUserControl),
                new PropertyMetadata(true));

        public static readonly DependencyProperty ShowIconOnTitleBarProperty =
            DependencyProperty.Register(
                "ShowIconOnTitleBar",
                typeof(bool),
                typeof(MetroUserControl),
                new PropertyMetadata(true));

        public static readonly DependencyProperty ShowMaxRestoreButtonProperty =
            DependencyProperty.Register(
                "ShowMaxRestoreButton",
                typeof(bool),
                typeof(MetroUserControl),
                new PropertyMetadata(true));

        public static readonly DependencyProperty ShowMinButtonProperty = DependencyProperty.Register(
            "ShowMinButton",
            typeof(bool),
            typeof(MetroUserControl),
            new PropertyMetadata(true));

        public static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register(
            "ShowTitleBar",
            typeof(bool),
            typeof(MetroUserControl),
            new PropertyMetadata(true));

        public static readonly DependencyProperty TitleCapsProperty = DependencyProperty.Register(
            "TitleCaps",
            typeof(bool),
            typeof(MetroUserControl),
            new PropertyMetadata(true));

        public static readonly DependencyProperty TitleForegroundProperty =
            DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(MetroUserControl));

        public static readonly DependencyProperty TitlebarHeightProperty = DependencyProperty.Register(
            "TitlebarHeight",
            typeof(int),
            typeof(MetroUserControl),
            new PropertyMetadata(30));

        public static readonly DependencyProperty WindowPlacementSettingsProperty =
            DependencyProperty.Register(
                "WindowPlacementSettings",
                typeof(IWindowPlacementSettings),
                typeof(MetroUserControl),
                new PropertyMetadata(null));

        #endregion

        #region Fields

        public TransparentAdorner _WindowAdorner;

        private bool isActiveWin = true;

        #endregion

        #region Constructors and Destructors

        static MetroUserControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(MetroUserControl),
                new FrameworkPropertyMetadata(typeof(MetroUserControl)));
        }

        public MetroUserControl()
        {
            this.Flyouts = new ObservableCollection<Flyout>();
            this.Loaded += this.MetroUserControlLoaded;
        }

        #endregion

        #region Enums

        private enum FadeDirection
        {
            FadeIn,

            FadeOut
        }

        #endregion

        #region Public Properties

        public ObservableCollection<Flyout> Flyouts { get; set; }

        public SolidColorBrush GlowBrush
        {
            get
            {
                return (SolidColorBrush)this.GetValue(GlowBrushProperty);
            }
            set
            {
                this.SetValue(GlowBrushProperty, value);
            }
        }

        public bool IgnoreTaskbarOnMaximize
        {
            get
            {
                return (bool)this.GetValue(IgnoreTaskbarOnMaximizeProperty);
            }
            set
            {
                this.SetValue(IgnoreTaskbarOnMaximizeProperty, value);
            }
        }

        public bool IsActiveWin
        {
            get
            {
                return this.isActiveWin;
            }

            set
            {
                if (this.isActiveWin != value)
                {
                    this.isActiveWin = value;

                    if (this.isActiveWin)
                    {
                        this.DettachWindowAdorner();
                    }
                    else
                    {
                        this.AttachWindowAdorner();
                    }
                }
            }
        }

        public bool SaveWindowPosition
        {
            get
            {
                return (bool)this.GetValue(SaveWindowPositionProperty);
            }
            set
            {
                this.SetValue(SaveWindowPositionProperty, value);
            }
        }

        public bool ShowCloseButton
        {
            get
            {
                return (bool)this.GetValue(ShowCloseButtonProperty);
            }
            set
            {
                this.SetValue(ShowCloseButtonProperty, value);
            }
        }

        public bool ShowIconOnTitleBar
        {
            get
            {
                return (bool)this.GetValue(ShowIconOnTitleBarProperty);
            }
            set
            {
                this.SetValue(ShowIconOnTitleBarProperty, value);
            }
        }

        public bool ShowMaxRestoreButton
        {
            get
            {
                return (bool)this.GetValue(ShowMaxRestoreButtonProperty);
            }
            set
            {
                this.SetValue(ShowMaxRestoreButtonProperty, value);
            }
        }

        public bool ShowMinButton
        {
            get
            {
                return (bool)this.GetValue(ShowMinButtonProperty);
            }
            set
            {
                this.SetValue(ShowMinButtonProperty, value);
            }
        }

        public bool ShowTitleBar
        {
            get
            {
                return (bool)this.GetValue(ShowTitleBarProperty);
            }
            set
            {
                this.SetValue(ShowTitleBarProperty, value);
            }
        }

        public bool TitleCaps
        {
            get
            {
                return (bool)this.GetValue(TitleCapsProperty);
            }
            set
            {
                this.SetValue(TitleCapsProperty, value);
            }
        }

        public Brush TitleForeground
        {
            get
            {
                return (Brush)this.GetValue(TitleForegroundProperty);
            }
            set
            {
                this.SetValue(TitleForegroundProperty, value);
            }
        }

        public int TitlebarHeight
        {
            get
            {
                return (int)this.GetValue(TitlebarHeightProperty);
            }
            set
            {
                this.SetValue(TitlebarHeightProperty, value);
            }
        }

        public WindowCommands WindowCommands { get; set; }

        public IWindowPlacementSettings WindowPlacementSettings
        {
            get
            {
                return (IWindowPlacementSettings)this.GetValue(WindowPlacementSettingsProperty);
            }
            set
            {
                this.SetValue(WindowPlacementSettingsProperty, value);
            }
        }

        #endregion

        #region Methods

        private void AttachWindowAdorner()
        {
            AdornerLayer parentAdorner = AdornerLayer.GetAdornerLayer(this.Content as UIElement);
            parentAdorner.Add(this._WindowAdorner);

            this.FadeAnimation(this.Content as PanelMetro, FadeDirection.FadeIn);
        }

        private void DettachWindowAdorner()
        {
            this.FadeAnimation(this.Content as PanelMetro, FadeDirection.FadeOut);

            AdornerLayer parentAdorner = AdornerLayer.GetAdornerLayer(this.Content as UIElement);
            parentAdorner.Remove(this._WindowAdorner);
        }

        private void FadeAnimation(PanelMetro border, FadeDirection fadeDirection)
        {
            var animFade = new DoubleAnimation();

            if (fadeDirection == FadeDirection.FadeIn)
            {
                animFade.From = 0;
                animFade.To = .10;
            }
            else
            {
                animFade.From = .10;
                animFade.To = 0;
            }

            animFade.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            var brush = new SolidColorBrush();
            brush.Color = Colors.Black;

            brush.BeginAnimation(Brush.OpacityProperty, animFade);

            border.Background = brush;
        }

        private void MetroUserControlLoaded(object sender, RoutedEventArgs e)
        {
            this._WindowAdorner = new TransparentAdorner(this.Content as UIElement);
        }

        #endregion
    }
}