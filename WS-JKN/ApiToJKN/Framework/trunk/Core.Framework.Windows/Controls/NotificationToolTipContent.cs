///
/// Copyright(C) MixModes Inc. 2010
/// 

namespace Core.Framework.Windows.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    ///     Interaction logic for NotificationTooltipContent.xaml
    /// </summary>
    public partial class NotificationToolTipContent : UserControl
    {
        #region Fields

        /// <summary>
        ///     Description property
        /// </summary>
        public DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description",
            typeof(string),
            typeof(NotificationToolTipContent));

        /// <summary>
        ///     Icon Property
        /// </summary>
        public DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(string),
            typeof(NotificationToolTipContent));

        /// <summary>
        ///     Title property
        /// </summary>
        public DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(NotificationToolTipContent));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotificationToolTipContent" /> class.
        /// </summary>
        public NotificationToolTipContent()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get
            {
                return this.GetValue(this.DescriptionProperty) as string;
            }
            set
            {
                this.SetValue(this.DescriptionProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        public string Icon
        {
            get
            {
                return this.GetValue(this.IconProperty) as string;
            }
            set
            {
                this.SetValue(this.IconProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get
            {
                return this.GetValue(this.TitleProperty) as string;
            }
            set
            {
                this.SetValue(this.TitleProperty, value);
            }
        }

        #endregion
    }
}