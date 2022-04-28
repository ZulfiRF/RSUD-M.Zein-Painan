///
/// Copyright(C) MixModes Inc. 2010
/// 

namespace Core.Framework.Windows.Windows
{
    using System.Windows.Input;

    /// <summary>
    ///     Document content
    /// </summary>
    public class DocumentContent
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DocumentContent" /> class.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="closeCommand">The close command.</param>
        public DocumentContent(DockPane pane, ICommand closeCommand)
        {
            this.Header = pane.Header;
            this.Content = pane.Content;
            this.DockPane = pane;
            this.CloseCommand = closeCommand;
            pane.Tag = this;
            pane.Header = null;
            pane.Content = null;
        }

        public void SetHeader(string header)
        {
            this.Header = header;
        }
        #endregion

        #region Public Properties

        public ICommand CloseAllTabCommand { get; set; }

        /// <summary>
        ///     Gets or sets the close command.
        /// </summary>
        /// <value>The close command.</value>
        public ICommand CloseCommand { get; private set; }

        /// <summary>
        ///     Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content { get;  set; }

        /// <summary>
        ///     Gets or sets the dock pane.
        /// </summary>
        /// <value>The dock pane.</value>
        public DockPane DockPane { get; private set; }

        public DocumentContainer DocumentContainer { get; set; }

        /// <summary>
        ///     Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public object Header { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Detaches the dock pane.
        /// </summary>
        public void DetachDockPane()
        {
            this.DockPane.Header = this.Header;
            this.DockPane.Content = this.Content;
        }

        #endregion
    }
}