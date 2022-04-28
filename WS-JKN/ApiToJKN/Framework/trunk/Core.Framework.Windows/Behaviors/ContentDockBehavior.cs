using Core.Framework.Windows.Implementations;

namespace Core.Framework.Windows.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Core.Framework.Windows.Framework;
    using Core.Framework.Windows.Windows;

    /// <summary>
    ///     Illustrates the docking behavior for Content within DocumentContainer
    /// </summary>
    public class ContentDockBehavior : LogicalParentBehavior<DocumentContainer>
    {
        #region Fields

        private ObservableDependencyProperty _parentStateChangeObserver;

        private TabItem _tabItem;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Dock point
        /// </summary>
        public ContentDockPoint DockPoint { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        /// <exception cref="InvalidOperationException">WindowsManager does not exist in logical tree</exception>
        protected override void OnAttached()
        {
            base.OnAttached();

            this._tabItem = new TabItem();
            this._parentStateChangeObserver = new ObservableDependencyProperty(
                typeof(DocumentContainer),
                DocumentContainer.StateProperty,
                this.OnParentStateChanged);
            this._parentStateChangeObserver.AddValueChanged(this.LogicalParent);
            this.AssociatedObject.MouseEnter += this.OnMouseEnter;
            this.AssociatedObject.MouseUp += this.OnMouseUp;
            this.AssociatedObject.MouseLeave += this.OnMouseLeave;

            // This is important so content dock points can initialize their visibility
            this.OnParentStateChanged(this.LogicalParent.State);
        }

        /// <summary>
        ///     Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>Override this to unhook functionality from the AssociatedObject.</remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            this._tabItem = null;
            this.AssociatedObject.MouseEnter -= this.OnMouseEnter;
            this.AssociatedObject.MouseUp -= this.OnMouseUp;
            this.AssociatedObject.MouseLeave -= this.OnMouseLeave;
        }

        /// <summary>
        ///     Clears the illustrations
        /// </summary>
        private void ClearIllustrations()
        {
            if (this.LogicalParent.DockIllustrationPanel != null)
            {
                this.LogicalParent.DockIllustrationPanel.Children.Clear();
            }

            this.LogicalParent.Documents.Remove(this._tabItem);
        }

        /// <summary>
        ///     Detaches the dock pane from window manager
        /// </summary>
        /// <returns></returns>
        private DockPane DetachDockPaneFromWindowManager()
        {
            DockPane dockPane = WindowsManager.ActiveWindowsManager.DraggedPane;
            WindowsManager.ActiveWindowsManager.RemoveDockPane(dockPane);
            return dockPane;
        }

        /// <summary>
        ///     Illustrates the content dock
        /// </summary>
        private void IllustrateContentDock()
        {
            WindowsManager activeWindowsManager = WindowsManager.ActiveWindowsManager;
            this._tabItem.Style = WindowsManager.GetDockIllustrationContentStyle(activeWindowsManager);
            this.LogicalParent.Documents.Add(this._tabItem);

            if (this.LogicalParent.DocumentsTab != null)
            {
                this.LogicalParent.DocumentsTab.SelectedItem = this._tabItem;
            }
        }

        /// <summary>
        ///     Illustrates the document dock in split windows
        /// </summary>
        /// <param name="dock">The dock.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        private void IllustrateDocumentDock(Dock dock, double height, double width)
        {
            if (this.LogicalParent.DockIllustrationPanel == null)
            {
                return;
            }

            var dockPaneIllustratingGrid = new Grid();
            dockPaneIllustratingGrid.Width = width;
            dockPaneIllustratingGrid.Height = height;
            dockPaneIllustratingGrid.Style =
                WindowsManager.GetDockPaneIllustrationStyle(WindowsManager.ActiveWindowsManager);
            DockPanel.SetDock(dockPaneIllustratingGrid, dock);
            this.LogicalParent.DockIllustrationPanel.Children.Add(dockPaneIllustratingGrid);
        }

        /// <summary>
        ///     Determines whether document state is compatible with current dockpoint
        /// </summary>
        /// <param name="documentContainerState">State of the document container</param>
        /// <returns>
        ///     <c>true</c> if document state is compatible with current dockpoint; otherwise, <c>false</c>.
        /// </returns>
        private bool IsDocumentStateCompatible(DocumentContainerState documentContainerState)
        {
            switch (documentContainerState)
            {
                case DocumentContainerState.Empty:
                    return true;
                case DocumentContainerState.ContainsDocuments:
                    return (this.DockPoint == ContentDockPoint.Content);
                case DocumentContainerState.SplitHorizontally:
                    return (this.DockPoint == ContentDockPoint.Left) || (this.DockPoint == ContentDockPoint.Right);
                case DocumentContainerState.SplitVertically:
                    return (this.DockPoint == ContentDockPoint.Top) || (this.DockPoint == ContentDockPoint.Bottom);
                default:
                    break;
            }

            return false;
        }

        /// <summary>
        ///     Called when mouse enters the associated object
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs" /> instance containing the event data.</param>
        private void OnMouseEnter(object sender, MouseEventArgs args)
        {
            if (WindowsManager.ActiveWindowsManager == null)
            {
                return;
            }

            switch (this.DockPoint)
            {
                case ContentDockPoint.Top:
                    this.IllustrateDocumentDock(Dock.Top, this.LogicalParent.ActualHeight / 2, double.NaN);
                    break;
                case ContentDockPoint.Left:
                    this.IllustrateDocumentDock(Dock.Left, double.NaN, this.LogicalParent.ActualWidth / 2);
                    break;
                case ContentDockPoint.Right:
                    this.IllustrateDocumentDock(Dock.Right, double.NaN, this.LogicalParent.ActualWidth / 2);
                    break;
                case ContentDockPoint.Bottom:
                    this.IllustrateDocumentDock(Dock.Bottom, this.LogicalParent.ActualHeight / 2, double.NaN);
                    break;
                case ContentDockPoint.Content:
                    this.IllustrateContentDock();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        ///     Called when mouse leaves the associated object
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs" /> instance containing the event data.</param>
        private void OnMouseLeave(object sender, MouseEventArgs args)
        {
            if (WindowsManager.ActiveWindowsManager == null)
            {
                return;
            }

            this.ClearIllustrations();
        }

        /// <summary>
        ///     Called when mouse is up on the associated object
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs" /> instance containing the event data.</param>
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.ClearIllustrations();

            if ((WindowsManager.ActiveWindowsManager != null)
                && (WindowsManager.ActiveWindowsManager.DraggedPane != null))
            {
                this.LogicalParent.AddDockPane(this.DetachDockPaneFromWindowManager(), this.DockPoint);
            }
        }

        /// <summary>
        ///     Called when parent state has changed
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private void OnParentStateChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.OnParentStateChanged((DocumentContainerState)e.NewValue);
        }

        /// <summary>
        ///     Called when parent state has changed
        /// </summary>
        /// <param name="state">The state.</param>
        private void OnParentStateChanged(DocumentContainerState state)
        {
            if (this.IsDocumentStateCompatible(state))
            {
                this.AssociatedObject.Visibility = Visibility.Visible;
            }
            else
            {
                this.AssociatedObject.Visibility = Visibility.Hidden;
            }
        }

        #endregion
    }
}