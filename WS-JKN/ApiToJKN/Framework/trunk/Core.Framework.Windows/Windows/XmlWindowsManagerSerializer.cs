///
/// Copyright(C) MixModes Inc. 2010
/// 

namespace Core.Framework.Windows.Windows
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Controls;
    using System.Xml;

    using Core.Framework.Windows.Utilities;

    /// <summary>
    ///     Xml window manager serializer
    /// </summary>
    public class XmlWindowsManagerSerializer : WindowsManagerSerializer
    {
        #region Fields

        private readonly Action<XmlElement, DockPane> _dockPaneWriter;

        private readonly XmlDocument _document = new XmlDocument();

        private readonly Func<DocumentContent, string> _documentWriter;

        private readonly Stack<XmlElement> _elementStack = new Stack<XmlElement>();

        private Stream _stream;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="XmlWindowsManagerSerializer" /> class.
        /// </summary>
        /// <param name="dockPaneWriter">The dock pane writer.</param>
        /// <param name="documentWriter">The document writer.</param>
        public XmlWindowsManagerSerializer(
            Action<XmlElement, DockPane> dockPaneWriter,
            Func<DocumentContent, string> documentWriter)
        {
            Validate.NotNull(dockPaneWriter, "dockPaneWriter");
            Validate.NotNull(documentWriter, "documentWriter");
            this._dockPaneWriter = dockPaneWriter;
            this._documentWriter = documentWriter;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Finalizes the document container.
        /// </summary>
        /// <param name="documentContainer">The document container.</param>
        protected override void FinalizeDocumentContainer(DocumentContainer documentContainer)
        {
            this.FinalizeElement("DocumentContainer");
        }

        /// <summary>
        ///     Finalizes the headers.
        /// </summary>
        protected override void FinalizeHeaders()
        {
            this.FinalizeElement("Headers");
        }

        /// <summary>
        ///     Finalizes the pinned panes.
        /// </summary>
        protected override void FinalizePinnedPanes()
        {
            this.FinalizeElement("PinnedPanes");
        }

        /// <summary>
        ///     Finalizes the serialization.
        /// </summary>
        protected override void FinalizeSerialization()
        {
            this._document.Save(this._stream);
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
        protected override void InitializeDocumentContainer(DocumentContainer documentContainer)
        {
            this.ValidateStackNotEmpty();
            XmlElement documentContainerElement = this._document.CreateElement("DocumentContainer");
            documentContainerElement.SetAttribute(
                "State",
                Enum.GetName(typeof(DocumentContainerState), documentContainer.State));
            documentContainerElement.SetAttribute("Row", Grid.GetRow(documentContainer).ToString());
            documentContainerElement.SetAttribute("Column", Grid.GetColumn(documentContainer).ToString());
            this._elementStack.Peek().AppendChild(documentContainerElement);
            this._elementStack.Push(documentContainerElement);
        }

        /// <summary>
        ///     Initializes the headers.
        /// </summary>
        protected override void InitializeHeaders()
        {
            this.ValidateStackNotEmpty();
            Validate.NotNull(this._document.DocumentElement, "WindowsManager");
            XmlElement rootElement = this._elementStack.Peek();
            Validate.Assert<InvalidOperationException>(
                (rootElement == this._document.DocumentElement) && (rootElement.Name == "WindowsManager"));
            XmlElement headersElement = this._document.CreateElement("Headers");
            this._elementStack.Push(headersElement);
            rootElement.AppendChild(headersElement);
        }

        /// <summary>
        ///     Initializes the pinned panes.
        /// </summary>
        protected override void InitializePinnedPanes()
        {
            this.ValidateStackNotEmpty();
            Validate.NotNull(this._document.DocumentElement, "WindowsManager");
            XmlElement rootElement = this._elementStack.Peek();
            Validate.Assert<InvalidOperationException>(
                (rootElement == this._document.DocumentElement) && (rootElement.Name == "WindowsManager"));
            XmlElement pinnedPanes = this._document.CreateElement("PinnedPanes");
            this._elementStack.Push(pinnedPanes);
            rootElement.AppendChild(pinnedPanes);
        }

        /// <summary>
        ///     Initializes the split.
        /// </summary>
        protected override void InitializeSplit()
        {
            this.ValidateStackNotEmpty();
            XmlElement splitElement = this._document.CreateElement("Split");
            this._elementStack.Peek().AppendChild(splitElement);
            this._elementStack.Push(splitElement);
        }

        /// <summary>
        ///     Initializes the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        protected override void InitializeStream(Stream stream)
        {
            this._stream = stream;
        }

        /// <summary>
        ///     Writes the documents within a document container
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="documents">The documents.</param>
        protected override void WriteDocuments(DocumentContainer container, IEnumerable<DocumentContent> documents)
        {
            this.ValidateStackNotEmpty();
            XmlElement documentContainerElement = this._elementStack.Peek();
            Validate.Assert<InvalidOperationException>(documentContainerElement.Name == "DocumentContainer");
            foreach (DocumentContent document in documents)
            {
                XmlElement documentElement = this._document.CreateElement("Document");
                documentElement.SetAttribute("Data", this._documentWriter(document));
                documentContainerElement.AppendChild(documentElement);
            }
        }

        /// <summary>
        ///     Writes the floating panes.
        /// </summary>
        /// <param name="floatingPanes">The floating panes.</param>
        protected override void WriteFloatingPanes(IEnumerable<DockPane> floatingPanes)
        {
            this.ValidateStackNotEmpty();
            Validate.NotNull(this._document.DocumentElement, "WindowsManager");
            XmlElement rootElement = this._elementStack.Peek();
            Validate.Assert<InvalidOperationException>(
                (rootElement == this._document.DocumentElement) && (rootElement.Name == "WindowsManager"));
            XmlElement floatingWindows = this._document.CreateElement("FloatingWindows");
            rootElement.AppendChild(floatingWindows);

            foreach (XmlElement floatingWindow in this.WriteDockPanes(floatingPanes))
            {
                floatingWindows.AppendChild(floatingWindow);
            }
        }

        /// <summary>
        ///     Writes the header panes within a docked header
        /// </summary>
        /// <param name="panes">The panes.</param>
        /// <param name="headerDock">Header dock.</param>
        protected override void WriteHeaderPanes(IEnumerable<DockPane> panes, Dock headerDock)
        {
            this.ValidateStackNotEmpty();
            XmlElement headersElement = this._elementStack.Peek();
            Validate.Assert<InvalidOperationException>(headersElement.Name == "Headers");
            XmlElement headerElement = this._document.CreateElement("Header");
            headerElement.SetAttribute("Dock", Enum.GetName(typeof(Dock), headerDock));

            foreach (XmlElement dockElement in this.WriteDockPanes(panes))
            {
                headerElement.AppendChild(dockElement);
            }

            headersElement.AppendChild(headerElement);
        }

        /// <summary>
        ///     Writes the pinned panes.
        /// </summary>
        /// <param name="panes">The panes.</param>
        /// <param name="headerDock">The header dock.</param>
        protected override void WritePinnedPanes(IEnumerable<DockPane> panes, Dock headerDock)
        {
            this.ValidateStackNotEmpty();
            XmlElement pinnedPanesElement = this._elementStack.Peek();
            Validate.Assert<InvalidOperationException>(pinnedPanesElement.Name == "PinnedPanes");
            XmlElement pinnedPaneElement = this._document.CreateElement("PinnedPane");
            pinnedPaneElement.SetAttribute("Dock", Enum.GetName(typeof(Dock), headerDock));

            foreach (XmlElement dockElement in this.WriteDockPanes(panes))
            {
                pinnedPaneElement.AppendChild(dockElement);
            }

            pinnedPanesElement.AppendChild(pinnedPaneElement);
        }

        /// <summary>
        ///     Writes the windows manager.
        /// </summary>
        /// <param name="windowsManager">The windows manager.</param>
        protected override void WriteWindowsManager(WindowsManager windowsManager)
        {
            XmlElement xmlElement = this._document.CreateElement("WindowsManager");
            this._document.AppendChild(xmlElement);
            this._elementStack.Push(xmlElement);
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
        ///     Validates that the stack is not empty.
        /// </summary>
        private void ValidateStackNotEmpty()
        {
            Validate.Assert<InvalidOperationException>(this._elementStack.Count > 0);
        }

        /// <summary>
        ///     Writes the dock pane.
        /// </summary>
        /// <param name="dockPane">The dock pane.</param>
        /// <returns>XMLElement corresponding to the dock pane</returns>
        private XmlElement WriteDockPane(DockPane dockPane)
        {
            XmlElement dockPaneElement = this._document.CreateElement("DockPane");
            dockPaneElement.SetAttribute("Height", dockPane.ActualHeight.ToString());
            dockPaneElement.SetAttribute("Width", dockPane.ActualWidth.ToString());

            if (dockPane.DockPaneState == DockPaneState.Floating)
            {
                dockPaneElement.SetAttribute("Top", Canvas.GetTop(dockPane).ToString());
                dockPaneElement.SetAttribute("Left", Canvas.GetLeft(dockPane).ToString());
            }

            this._dockPaneWriter(dockPaneElement, dockPane);
            return dockPaneElement;
        }

        /// <summary>
        ///     Writes the dock panes.
        /// </summary>
        /// <param name="dockPanes">The dock panes.</param>
        /// <returns>XMLElement(s) corresponding to the dock panes in order</returns>
        private IEnumerable<XmlElement> WriteDockPanes(IEnumerable<DockPane> dockPanes)
        {
            foreach (DockPane dockPane in dockPanes)
            {
                yield return this.WriteDockPane(dockPane);
            }
        }

        #endregion

        // Private members
    }
}