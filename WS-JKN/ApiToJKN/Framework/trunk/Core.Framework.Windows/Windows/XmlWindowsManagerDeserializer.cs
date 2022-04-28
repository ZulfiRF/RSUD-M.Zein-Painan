
namespace Core.Framework.Windows.Windows
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Controls;
    using System.Xml;

    using Core.Framework.Windows.Utilities;

    /// <summary>
    ///     Xml window manager deserializer
    /// </summary>
    public class XmlWindowsManagerDeserializer : WindowsManagerDeserializer
    {
        #region Fields

        private readonly Action<DockPane, string> _dockPaneReader;

        private readonly XmlDocument _document = new XmlDocument();

        private readonly Dictionary<DocumentContainer, XmlElement> _documentContainerToDefinitionMapping =
            new Dictionary<DocumentContainer, XmlElement>();


        private readonly Stack<XmlElement> _elementStack = new Stack<XmlElement>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="XmlWindowsManagerDeserializer" /> class.
        /// </summary>
        public XmlWindowsManagerDeserializer(Action<DockPane, string> dockPaneReader)
        {
            Validate.NotNull(dockPaneReader, "dockPaneReader");
            this._dockPaneReader = dockPaneReader;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Finalizes the deserialization.
        /// </summary>
        protected override void FinalizeDeserialization()
        {
            this._elementStack.Pop();
        }

        /// <summary>
        ///     Finalizes the document container.
        /// </summary>
        protected override void FinalizeDocumentContainer()
        {
            this.FinalizeElement("DocumentContainer");
        }

        /// <summary>
        ///     Finalizes the split.
        /// </summary>
        protected override void FinalizeSplit()
        {
            this.FinalizeElement("Split");
        }

        /// <summary>
        ///     Initializes the document container.
        /// </summary>
        /// <param name="documentContainer">The document container.</param>
        /// <returns>Document container state</returns>
        protected override DocumentContainerState InitializeDocumentContainer(DocumentContainer documentContainer)
        {
            this.ValidateStackNotEmpty();

            XmlElement documentContainerElement;
            Validate.Assert<InvalidOperationException>(
                this._documentContainerToDefinitionMapping.TryGetValue(documentContainer, out documentContainerElement)
                && (documentContainerElement.Name == "DocumentContainer"));

            Grid.SetRow(documentContainer, int.Parse(documentContainerElement.GetAttribute("Row")));
            Grid.SetColumn(documentContainer, int.Parse(documentContainerElement.GetAttribute("Column")));

            this._elementStack.Push(documentContainerElement);

            return
                (DocumentContainerState)
                    Enum.Parse(typeof(DocumentContainerState), documentContainerElement.GetAttribute("State"));
        }

        /// <summary>
        ///     Initializes the split.
        /// </summary>
        /// <param name="parentContainer">The parent container.</param>
        protected override void InitializeSplit(DocumentContainer parentContainer)
        {
            this.ValidateStackNotEmpty();

            XmlElement documentContainerElement = this._elementStack.Peek();
            Validate.Assert<InvalidOperationException>(documentContainerElement.Name == "DocumentContainer");

            var splitElement = documentContainerElement.FirstChild as XmlElement;
            Validate.Assert<NullReferenceException>(splitElement != null);
            Validate.Assert<InvalidOperationException>(splitElement.Name == "Split");

            this._elementStack.Push(splitElement);
        }

        /// <summary>
        ///     Initializes the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        protected override void InitializeStream(Stream stream)
        {
            this._document.Load(stream);
        }

        /// <summary>
        ///     Reads the document containers within current split and sets the State as well as dimensions for read
        ///     DocumentContainer(s)
        /// </summary>
        /// <returns>Read document containers</returns>
        protected override IEnumerable<DocumentContainer> ReadDocumentContainers()
        {
            this.ValidateStackNotEmpty();
            XmlElement parentElement = this._elementStack.Peek();
            foreach (XmlElement documentContainer in parentElement.SelectNodes("DocumentContainer").OfType<XmlElement>()
                )
            {
                var container = new DocumentContainer();
                this._documentContainerToDefinitionMapping[container] = documentContainer;
                yield return container;
            }
        }

        /// <summary>
        ///     Reads the documents for current document container
        /// </summary>
        /// <param name="documentContainer">The document container.</param>
        protected override void ReadDocuments(DocumentContainer documentContainer)
        {
            this.ValidateStackNotEmpty();

            XmlElement documentContainerElement = this._elementStack.Peek();
            Validate.Assert<InvalidOperationException>(documentContainerElement.Name == "DocumentContainer");

            foreach (
                XmlElement documentElement in documentContainerElement.SelectNodes(@"Document").OfType<XmlElement>())
            {
                var dockPane = new DockPane();
                this._dockPaneReader(dockPane, documentElement.GetAttribute("Data"));
                documentContainer.AddDocument(dockPane);
            }
        }

        /// <summary>
        ///     Reads the floating panes.
        /// </summary>
        /// <returns>Floating panes</returns>
        protected override IEnumerable<DockPane> ReadFloatingPanes()
        {
            XmlElement rootElement = this._elementStack.Peek();
            Validate.Assert<InvalidOperationException>(rootElement.Name == "WindowsManager");
            XmlNodeList dockPaneList = rootElement.SelectNodes(@"FloatingWindows/DockPane");
            foreach (XmlElement dockPaneElement in dockPaneList.OfType<XmlElement>())
            {
                yield return this.ReadDockPane(dockPaneElement);
            }
        }

        /// <summary>
        ///     Reads the header panes.
        /// </summary>
        /// <param name="dock">Dock point</param>
        /// <returns>DockPanes in order within header pane with specified dock point</returns>
        protected override IEnumerable<DockPane> ReadHeaderPanes(Dock dock)
        {
            XmlElement rootElement = this._elementStack.Peek();
            Validate.Assert<InvalidOperationException>(rootElement.Name == "WindowsManager");
            XmlNodeList dockPaneList =
                rootElement.SelectNodes(
                    string.Format(@"Headers/Header[@Dock='{0}']/DockPane", Enum.GetName(typeof(Dock), dock)));
            foreach (XmlElement dockPaneElement in dockPaneList.OfType<XmlElement>())
            {
                yield return this.ReadDockPane(dockPaneElement);
            }
        }

        /// <summary>
        ///     Reads the pinned panes.
        /// </summary>
        /// <param name="dock">Dock point</param>
        /// <returns>DockPanes in order within pinned pane with specified dock point</returns>
        protected override IEnumerable<DockPane> ReadPinnedPanes(Dock dock)
        {
            XmlElement rootElement = this._elementStack.Peek();
            Validate.Assert<InvalidOperationException>(rootElement.Name == "WindowsManager");
            XmlNodeList dockPaneList =
                rootElement.SelectNodes(
                    string.Format(@"PinnedPanes/PinnedPane[@Dock='{0}']/DockPane", Enum.GetName(typeof(Dock), dock)));
            foreach (XmlElement dockPaneElement in dockPaneList.OfType<XmlElement>())
            {
                yield return this.ReadDockPane(dockPaneElement);
            }
        }

        /// <summary>
        ///     Reads the root document container and sets the State as well as dimensions for read DocumentContainer
        /// </summary>
        /// <returns>Read document container</returns>
        protected override DocumentContainer ReadRootDocumentContainer()
        {
            XmlElement rootElement = this._elementStack.Peek();
            Validate.Assert<InvalidOperationException>(rootElement.Name == "WindowsManager");
            return this.ReadDocumentContainers().First();
        }

        /// <summary>
        ///     Reads the windows manager.
        /// </summary>
        protected override void ReadWindowsManager()
        {
            XmlElement windowsManagerElement = this._document.DocumentElement;
            Validate.Assert<NullReferenceException>(windowsManagerElement != null);
            Validate.Assert<InvalidOperationException>(windowsManagerElement.Name == "WindowsManager");
            this._elementStack.Push(windowsManagerElement);
        }

        /// <summary>
        ///     Finalizes the element.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <exception cref="InvalidOperationException">Top element on the stack does not match element name</exception>
        private void FinalizeElement(string elementName)
        {
            this.ValidateStackNotEmpty();
            Validate.Assert<InvalidOperationException>(this._elementStack.Peek().Name == elementName);
            this._elementStack.Pop();
        }

        /// <summary>
        ///     Reads the dock pane from the xml element
        /// </summary>
        /// <param name="dockPaneElement">The dock pane xml element.</param>
        /// <returns>Newly constructed DockPane</returns>
        private DockPane ReadDockPane(XmlElement dockPaneElement)
        {
            var dockPane = new DockPane();
            this._dockPaneReader(dockPane, dockPaneElement.GetAttribute("Data"));

            double height = double.Parse(dockPaneElement.GetAttribute("Height"));
            dockPane.Height = height;

            double width = double.Parse(dockPaneElement.GetAttribute("Width"));
            dockPane.Width = width;

            var topAttribute = dockPaneElement.Attributes.GetNamedItem("Top") as XmlAttribute;
            if (topAttribute != null)
            {
                Canvas.SetTop(dockPane, double.Parse(topAttribute.Value));
            }

            var leftAttribute = dockPaneElement.Attributes.GetNamedItem("Left") as XmlAttribute;
            if (leftAttribute != null)
            {
                Canvas.SetLeft(dockPane, double.Parse(leftAttribute.Value));
            }

            return dockPane;
        }

        /// <summary>
        ///     Validates that the stack is not empty.
        /// </summary>
        private void ValidateStackNotEmpty()
        {
            Validate.Assert<InvalidOperationException>(this._elementStack.Count > 0);
        }

        #endregion
    }
}