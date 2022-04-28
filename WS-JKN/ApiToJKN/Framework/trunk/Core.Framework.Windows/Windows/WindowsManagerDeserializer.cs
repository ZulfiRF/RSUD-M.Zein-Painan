///
/// Copyright(C) MixModes Inc. 2010
/// 

namespace Core.Framework.Windows.Windows
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Controls;

    using Core.Framework.Windows.Utilities;

    /// <summary>
    ///     Deserializes a window manager
    /// </summary>
    /// <remarks>Deserialization should be atomic operation and must not leave windows manager is an unstable state</remarks>
    public abstract class WindowsManagerDeserializer
    {
        #region Fields

        private Dictionary<Dock, DockedWindows> _dockedWindows;

        private List<DockPane> _floatingWindows;

        private DocumentContainer _rootContainer;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the dock positions.
        /// </summary>
        /// <value>The dock positions.</value>
        private List<Dock> DockPositions
        {
            get
            {
                return new List<Dock>(new[] { Dock.Left, Dock.Top, Dock.Right, Dock.Bottom });
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Deserializes the specified windows manager from the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="windowsManager">The windows manager.</param>
        /// <exception cref="ArgumentNullException">stream or windowsManager are null</exception>
        /// <exception cref="InvalidOperationException">stream is not readable</exception>
        public void Deserialize(Stream stream, WindowsManager windowsManager)
        {
            Validate.NotNull(stream, "stream");
            Validate.NotNull(windowsManager, "windowsManager");
            Validate.Assert<InvalidOperationException>(stream.CanRead);

            this._dockedWindows = new Dictionary<Dock, DockedWindows>();
            this._floatingWindows = new List<DockPane>();
            this._rootContainer = null;
            this.DockPositions.ForEach(dock => this._dockedWindows[dock] = new DockedWindows());

            // Initialize stream
            this.InitializeStream(stream);

            // Navigate windows manager
            this.NavigateWindowsManager();

            // Finalize deserialization
            this.FinalizeDeserialization();

            // Transfer contents to windows manager
            this.TransferWindowsManagerContents(windowsManager);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Finalizes the deserialization.
        /// </summary>
        protected abstract void FinalizeDeserialization();

        /// <summary>
        ///     Finalizes the document container.
        /// </summary>
        protected abstract void FinalizeDocumentContainer();

        /// <summary>
        ///     Finalizes the split.
        /// </summary>
        protected abstract void FinalizeSplit();

        /// <summary>
        ///     Initializes the document container.
        /// </summary>
        /// <param name="documentContainer">The document container.</param>
        /// <returns>Document container state</returns>
        protected abstract DocumentContainerState InitializeDocumentContainer(DocumentContainer documentContainer);

        /// <summary>
        ///     Initializes the split.
        /// </summary>
        /// <param name="parentContainer">The parent container.</param>
        protected abstract void InitializeSplit(DocumentContainer parentContainer);

        /// <summary>
        ///     Initializes the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        protected abstract void InitializeStream(Stream stream);

        /// <summary>
        ///     Reads the document containers within current split and sets the State as well as dimensions for read
        ///     DocumentContainer(s)
        /// </summary>
        /// <returns>Read document containers</returns>
        protected abstract IEnumerable<DocumentContainer> ReadDocumentContainers();

        /// <summary>
        ///     Reads the documents for current document container
        /// </summary>
        /// <param name="documentContainer">The document container.</param>
        protected abstract void ReadDocuments(DocumentContainer documentContainer);

        /// <summary>
        ///     Reads the floating panes.
        /// </summary>
        /// <returns>Floating panes</returns>
        protected abstract IEnumerable<DockPane> ReadFloatingPanes();

        /// <summary>
        ///     Reads the header panes.
        /// </summary>
        /// <param name="dock">Dock point</param>
        /// <returns>DockPanes in order within header pane with specified dock point</returns>
        protected abstract IEnumerable<DockPane> ReadHeaderPanes(Dock dock);

        /// <summary>
        ///     Reads the pinned panes.
        /// </summary>
        /// <param name="dock">Dock point</param>
        /// <returns>DockPanes in order within pinned pane with specified dock point</returns>
        protected abstract IEnumerable<DockPane> ReadPinnedPanes(Dock dock);

        /// <summary>
        ///     Reads the root document container and sets the State as well as dimensions for read DocumentContainer
        /// </summary>
        /// <returns>Read document container</returns>
        protected abstract DocumentContainer ReadRootDocumentContainer();

        /// <summary>
        ///     Reads the windows manager.
        /// </summary>
        protected abstract void ReadWindowsManager();

        /// <summary>
        ///     Navigates the document container
        /// </summary>
        /// <param name="documentContainer">The document container.</param>
        private void NavigateDocumentContainer(DocumentContainer documentContainer)
        {
            DocumentContainerState state = this.InitializeDocumentContainer(documentContainer);

            switch (state)
            {
                case DocumentContainerState.Empty:
                    // Do Nothing
                    break;
                case DocumentContainerState.ContainsDocuments:
                    this.ReadDocuments(documentContainer);
                    break;
                case DocumentContainerState.SplitHorizontally:
                    this.NavigateDocumentGrid(documentContainer, true);
                    break;
                case DocumentContainerState.SplitVertically:
                    this.NavigateDocumentGrid(documentContainer, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.FinalizeDocumentContainer();
        }

        /// <summary>
        ///     Navigates the document containers.
        /// </summary>
        private void NavigateDocumentContainers()
        {
            this._rootContainer = this.ReadRootDocumentContainer();
            this.NavigateDocumentContainer(this._rootContainer);
        }

        /// <summary>
        ///     Navigates the document grid.
        /// </summary>
        /// <param name="parentContainer">The parent container.</param>
        /// <param name="isHorizontal">if set to <c>true</c> orientation is horizontal otherwise orientation is vertical</param>
        private void NavigateDocumentGrid(DocumentContainer parentContainer, bool isHorizontal)
        {
            this.InitializeSplit(parentContainer);

            var childContainers = new List<DocumentContainer>();

            foreach (DocumentContainer documentContainer in this.ReadDocumentContainers())
            {
                this.NavigateDocumentContainer(documentContainer);
                childContainers.Add(documentContainer);
            }

            parentContainer.AddDocumentContainers(childContainers, isHorizontal);

            this.FinalizeSplit();
        }

        /// <summary>
        ///     Navigates the windows manager.
        /// </summary>
        private void NavigateWindowsManager()
        {
            // Read windows manager itself
            this.ReadWindowsManager();

            // Read header panes
            this.ReadHeaderPanes();

            // Read pinned panes
            this.ReadPinnedPanes();

            // Read floating panes
            this.ReadFloatingPanesBase();

            // Read document container
            this.NavigateDocumentContainers();
        }

        /// <summary>
        ///     Reads the floating panes base.
        /// </summary>
        private void ReadFloatingPanesBase()
        {
            foreach (DockPane pane in this.ReadFloatingPanes())
            {
                this._floatingWindows.Add(pane);
            }
        }

        /// <summary>
        ///     Reads the header panes.
        /// </summary>
        private void ReadHeaderPanes()
        {
            foreach (Dock dock in this.DockPositions)
            {
                DockedWindows dockedWindows = this._dockedWindows[dock];
                foreach (DockPane dockpane in this.ReadHeaderPanes(dock))
                {
                    dockedWindows.AutoHidePanes.Add(dockpane);
                }
            }
        }

        /// <summary>
        ///     Reads the pinned panes.
        /// </summary>
        private void ReadPinnedPanes()
        {
            foreach (Dock dock in this.DockPositions)
            {
                DockedWindows dockedWindows = this._dockedWindows[dock];
                foreach (DockPane dockpane in this.ReadPinnedPanes(dock))
                {
                    dockedWindows.PinnedPanes.Add(dockpane);
                }
            }
        }

        /// <summary>
        ///     Transfers the document grid from _rootContainer to the main document container of specified windows manager
        /// </summary>
        /// <param name="windowsManager">
        ///     The windows manager whose document content is the target of transferred child contents
        ///     from _rootContainer
        /// </param>
        /// <param name="isHorizontal">if set to <c>true</c> indicates horizontal orientation otherwise vertical orientation</param>
        private void TransferDocumentGrid(WindowsManager windowsManager, bool isHorizontal)
        {
            var contentGrid = this._rootContainer.Content as Grid;
            Validate.Assert<ArgumentNullException>(contentGrid != null);

            this._rootContainer.Clear();
            var documentContainers = new List<DocumentContainer>(contentGrid.Children.OfType<DocumentContainer>());
            contentGrid.Children.Clear();

            windowsManager.DocumentContainer.AddDocumentContainers(documentContainers, isHorizontal);
        }

        /// <summary>
        ///     Transfers the windows manager contents after desealization has finished
        /// </summary>
        /// <param name="windowsManager">The windows manager.</param>
        private void TransferWindowsManagerContents(WindowsManager windowsManager)
        {
            windowsManager.Clear();

            // Transfer auto hide and pinned windows for all dock points);
            foreach (Dock dockPosition in this.DockPositions)
            {
                DockedWindows dockedWindows = this._dockedWindows[dockPosition];

                foreach (DockPane pinnedPane in dockedWindows.PinnedPanes)
                {
                    windowsManager.AddPinnedWindow(pinnedPane, dockPosition);
                }

                foreach (DockPane autoHidePane in dockedWindows.AutoHidePanes)
                {
                    windowsManager.AddAutoHideWindow(autoHidePane, dockPosition);
                }
            }

            // Transfer floating windows
            foreach (DockPane floatingPane in this._floatingWindows)
            {
                windowsManager.AddFloatingWindow(floatingPane);
            }

            // Transfer document content
            switch (this._rootContainer.State)
            {
                case DocumentContainerState.Empty:
                    break;
                case DocumentContainerState.ContainsDocuments:
                    var documents = new List<object>(this._rootContainer.Documents);
                    this._rootContainer.Clear();
                    foreach (object document in documents)
                    {
                        if (document is DocumentContent)
                        {
                            var documentContent = (document as DocumentContent);
                            documentContent.DetachDockPane();
                            windowsManager.DocumentContainer.AddDocument(documentContent.DockPane);
                        }
                    }
                    break;
                case DocumentContainerState.SplitHorizontally:
                    this.TransferDocumentGrid(windowsManager, true);
                    break;
                case DocumentContainerState.SplitVertically:
                    this.TransferDocumentGrid(windowsManager, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        /// <summary>
        ///     Docked Windows
        /// </summary>
        private class DockedWindows
        {
            #region Fields

            /// <summary>
            ///     Auto hide panes
            /// </summary>
            public readonly List<DockPane> AutoHidePanes;

            /// <summary>
            ///     Pinned Panes
            /// </summary>
            public readonly List<DockPane> PinnedPanes;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="DockedWindows" /> class.
            /// </summary>
            public DockedWindows()
            {
                this.PinnedPanes = new List<DockPane>();
                this.AutoHidePanes = new List<DockPane>();
            }

            #endregion
        }

        // Private members
    }
}