namespace Core.Framework.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Helper;

    public class Tile : Button, IControlToUseGenericCalling
    {
        #region Static Fields

        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(
            "Count",
            typeof(string),
            typeof(Tile),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty KeepDraggingProperty = DependencyProperty.Register(
            "KeepDragging",
            typeof(bool),
            typeof(Tile),
            new PropertyMetadata(true));

        public static readonly DependencyProperty TiltFactorProperty = DependencyProperty.Register(
            "TiltFactor",
            typeof(int),
            typeof(Tile),
            new PropertyMetadata(5));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(Tile),
            new PropertyMetadata(default(string)));

        #endregion

        #region Constructors and Destructors

        public Tile()
        {
            this.DefaultStyleKey = typeof(Tile);
            this.Cursor = Cursors.Hand;
        }

        #endregion

        #region Public Properties

        public string ControlNameSpace { get; set; }

        public string Count
        {
            get
            {
                return (string)this.GetValue(CountProperty);
            }
            set
            {
                this.SetValue(CountProperty, value);
            }
        }

        public bool KeepDragging
        {
            get
            {
                return (bool)this.GetValue(KeepDraggingProperty);
            }
            set
            {
                this.SetValue(KeepDraggingProperty, value);
            }
        }

        public int TiltFactor
        {
            get
            {
                return (Int32)this.GetValue(TiltFactorProperty);
            }
            set
            {
                this.SetValue(TiltFactorProperty, value);
            }
        }

        public string Title
        {
            get
            {
                return (string)this.GetValue(TitleProperty);
            }
            set
            {
                this.SetValue(TitleProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void ExecuteControl()
        {
            Manager.ExecuteGenericModule(this.ControlNameSpace);
        }

        #endregion

        #region Methods

        protected override void OnClick()
        {
            if (string.IsNullOrEmpty(this.ControlNameSpace))
            {
                base.OnClick();
            }
            else
            {
                this.ExecuteControl();
            }
        }

        #endregion
    }
}