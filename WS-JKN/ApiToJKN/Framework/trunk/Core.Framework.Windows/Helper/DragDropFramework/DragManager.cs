#define PRINT2BUFFER
// Monitor event entry/exit
#define PRINT2OUTPUT
// Output interesting information to Visual Studio's Output window

namespace Core.Framework.Windows.Helper.DragDropFramework
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    ///     Manage drag events for IDataProviders
    /// </summary>
    public class DragManager
    {
        #region Fields

        private readonly IDataProvider[] dragDropObjects;

        private readonly UIElement dragSource;

        private IDataProvider dragDropObject;

        private bool dragInProgress;

        private Point startPosition;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Manage dragging data object from <code>dragSource</code> FrameworkElement.
        ///     Hook various PreviewMouse* events in order to determine when a drag starts.
        /// </summary>
        /// <param name="dragSource">The FrameworkElement which contains objects to be dragged</param>
        /// <param name="dragDropObject">Object to be dragged, implementing IDataProvider</param>
        public DragManager(FrameworkElement dragSource, IDataProvider dragDropObject)
            : this(dragSource, new[] { dragDropObject })
        {
        }

        /// <summary>
        ///     Manage dragging data object from <code>dragSource</code> FrameworkElement.
        ///     Hook various PreviewMouse* events in order to determine when a drag starts.
        /// </summary>
        /// <param name="dragSource">The FrameworkElement which contains objects to be dragged</param>
        /// <param name="dragDropObjects">Array of objects to be dragged, implementing IDataProvider</param>
        public DragManager(FrameworkElement dragSource, IDataProvider[] dragDropObjects)
        {
            this.dragSource = dragSource;
            Debug.Assert(dragSource != null, "dragSource cannot be null");
            this.dragDropObjects = dragDropObjects;

            this.dragSource.PreviewMouseLeftButtonDown += this.DragSourcePreviewMouseLeftButtonDown;
            this.dragSource.PreviewMouseMove += this.DragSourcePreviewMouseMove;
            this.dragSource.PreviewMouseLeftButtonUp += this.DragSourcePreviewMouseLeftButtonUp;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called after DragDrop.DoDragDrop() returns.
        ///     Typically during a file move, for example, the file is deleted here.
        ///     However, when moving a TabItem from one TabControl to another the
        ///     source TabItem must be unparented from the source TabControl
        ///     before it can be added to the destination TabControl.
        ///     So most of the time when moving items between item controls,
        ///     this method isn't used.
        /// </summary>
        /// <param name="resultEffects">The drop operation that was performed</param>
        private void DoDragDropDone(DragDropEffects resultEffects)
        {
            if ((this.dragDropObject.DataProviderActions & DataProviderActions.DoDragDropDone) != 0)
            {
                this.dragDropObject.DoDragDropDone(resultEffects);
            }

#if PRINT2BUFFER
            // Debug.WriteLine("buf0: " + ((Window1)Application.Current.MainWindow).buf0.ToString());
            //Debug.WriteLine("buf1: " + ((Window1)Application.Current.MainWindow).buf1.ToString());
            // bool buffersSame = (((Window1)Application.Current.MainWindow).buf0.ToString().CompareTo(((Window1)Application.Current.MainWindow).buf1.ToString()) == 0);
            // if(buffersSame)
            //  Debug.WriteLine("buf0 and buf1 are the same");
            // Debug.Assert(buffersSame, "Possible reentrancy issue(s) -- make sure event code is short");
            // ((Window1)Application.Current.MainWindow).buf0 = new StringBuilder("");
            // ((Window1)Application.Current.MainWindow).buf1 = new StringBuilder("");
#endif
        }

        /// <summary>
        ///     Prepare for and begin a drag operation.
        ///     Hook the events needed by the data provider.
        /// </summary>
        private DragDropEffects DoDragDropStart(MouseEventArgs e)
        {
            var resultEffects = DragDropEffects.None;

            var data = new DataObject();
            this.dragDropObject.SetData(ref data);

            bool hookQueryContinueDrag = false;
            bool hookGiveFeedback = false;

            if ((this.dragDropObject.DataProviderActions & DataProviderActions.QueryContinueDrag) != 0)
            {
                hookQueryContinueDrag = true;
            }

            if ((this.dragDropObject.DataProviderActions & DataProviderActions.GiveFeedback) != 0)
            {
                hookGiveFeedback = true;
            }

            if (this.dragDropObject.AddAdorner)
            {
                hookGiveFeedback = true;
            }

            QueryContinueDragEventHandler queryContinueDrag = null;
            GiveFeedbackEventHandler giveFeedback = null;

            if (hookQueryContinueDrag)
            {
                queryContinueDrag = this.DragSourceQueryContinueDrag;
                this.dragSource.QueryContinueDrag += queryContinueDrag;
            }
            if (hookGiveFeedback)
            {
                giveFeedback = this.DragSourceGiveFeedback;
                this.dragSource.GiveFeedback += giveFeedback;
            }

            try
            {
                // NOTE:  Set 'dragSource' to desired value (dragSource or item being dragged)
                //		  'dragSource' is passed to QueryContinueDrag as Source and OriginalSource
                //dragSource = this._dragDropObject.Item;
                resultEffects = DragDrop.DoDragDrop(this.dragSource, data, this.dragDropObject.AllowedEffects);
            }
            catch
            {
                Debug.WriteLine("DragDrop.DoDragDrop threw an exception");
            }

            if (queryContinueDrag != null)
            {
                this.dragSource.QueryContinueDrag -= queryContinueDrag;
            }
            if (giveFeedback != null)
            {
                this.dragSource.GiveFeedback -= giveFeedback;
            }

            return resultEffects;
        }

        /// <summary>
        ///     Display the appropriate drag cursor based on
        ///     DragDropEffects returned within the DropManager
        /// </summary>
        private void DragSourceGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
#if PRINT2BUFFER
            // ((Window1)Application.Current.MainWindow).buf0.Append('g');
#endif

            if (this.dragDropObject.AddAdorner)
            {
                Point point = Utility.Win32GetCursorPos();
                DefaultAdorner dragAdorner = this.dragDropObject.DragAdorner;
                dragAdorner.SetMousePosition(dragAdorner.AdornedElement.PointFromScreen(point));
            }

            if ((this.dragDropObject.DataProviderActions & DataProviderActions.GiveFeedback) != 0)
            {
                this.dragDropObject.DragSourceGiveFeedback(sender, e);
            }

#if PRINT2OUTPUT
            Debug.WriteLine(
                "g handled=" + e.Handled + " sender=" + sender.GetType() + " Source=" + e.Source.GetType()
                + " OriginalSource=" + e.OriginalSource.GetType() + " Effects=" + e.Effects);
#endif

#if PRINT2BUFFER
            // ((Window1)Application.Current.MainWindow).buf1.Append('g');
#endif
        }

        /// <summary>
        ///     Check for a supported SourceContainer/SourceObject.
        ///     If found, get ready for a possible drag operation.
        /// </summary>
        private void DragSourcePreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (IDataProvider dropObject in this.dragDropObjects)
            {
                if (dropObject.IsSupportedContainerAndObject(true, sender, e.Source, e.OriginalSource))
                {
                    Debug.Assert(sender.Equals(this.dragSource));

                    this.dragDropObject = dropObject;

                    this.startPosition = e.GetPosition(sender as IInputElement);

                    this.dragDropObject.StartPosition = e.GetPosition(e.Source as IInputElement);

                    if (this.dragDropObject.NeedsCaptureMouse)
                    {
                        this.dragSource.CaptureMouse();
                    }

                    break;
                }
            }
        }

        /// <summary>
        ///     When MouseLeftButtonUp event occurs, abandon
        ///     any drag operation that may be in progress
        /// </summary>
        private void DragSourcePreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.dragDropObject != null)
            {
                if (this.dragDropObject.NeedsCaptureMouse)
                {
                    this.dragSource.ReleaseMouseCapture();
                }
                this.dragDropObject = null;
                this.dragInProgress = false;
            }
        }

        /// <summary>
        ///     If the mouse is moved (dragged) a minimum distance
        ///     over a supported SourceContainer/SourceObject,
        ///     initiate a drag operation.
        /// </summary>
        private void DragSourcePreviewMouseMove(object sender, MouseEventArgs e)
        {
            if ((this.dragDropObject != null) && !this.dragInProgress
                && this.dragDropObject.IsSupportedContainerAndObject(false, sender, e.Source, e.OriginalSource))
            {
                Point currentPosition = e.GetPosition(sender as IInputElement);
                if (((Math.Abs(currentPosition.X - this.startPosition.X)
                      > SystemParameters.MinimumHorizontalDragDistance)
                     || (Math.Abs(currentPosition.Y - this.startPosition.Y)
                         > SystemParameters.MinimumVerticalDragDistance)))
                {
                    // NOTE:
                    //      While dragging a ListBoxItem, another one can be selected
                    //      This doesn't seem to happen with TreeView or TabControl
                    if (sender is ListBox)
                    {
                        this.dragDropObject.SourceObject = e.Source;
                    }

                    this.dragInProgress = true;

                    if (this.dragDropObject.AddAdorner)
                    {
                        this.dragDropObject.DragAdorner =
                            new DefaultAdorner(
                                (UIElement)Application.Current.MainWindow.Content,
                                (UIElement)this.dragDropObject.SourceObject,
                                this.dragDropObject.StartPosition);
                        var visual = Application.Current.MainWindow.Content as Visual;
                        if (visual != null)
                        {
                            AdornerLayer.GetAdornerLayer(visual).Add(this.dragDropObject.DragAdorner);
                        }
                    }

                    DragDropEffects resultEffects = this.DoDragDropStart(e);

                    if (this.dragDropObject.NeedsCaptureMouse)
                    {
                        this.dragSource.ReleaseMouseCapture();
                    }

                    this.DoDragDropDone(resultEffects);

                    if (this.dragDropObject.AddAdorner)
                    {
                        if (Application.Current != null)
                        {
                            if (Application.Current.MainWindow != null)
                            {
                                if (Application.Current.MainWindow.Content != null)
                                {
                                    AdornerLayer.GetAdornerLayer((Visual)Application.Current.MainWindow.Content)
                                        .Remove(this.dragDropObject.DragAdorner);
                                }
                            }
                        }
                    }

                    Mouse.OverrideCursor = null;

                    this.dragDropObject = null;
                    this.dragInProgress = false;
                }
            }
        }

        /// <summary>
        ///     Gather keyboard key state information
        ///     and optionally abort a drag operation
        /// </summary>
        private void DragSourceQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
#if PRINT2BUFFER
            //((Window1)Application.Current.MainWindow).buf0.Append('q');
#endif

            if ((this.dragDropObject.DataProviderActions & DataProviderActions.QueryContinueDrag) != 0)
            {
                this.dragDropObject.DragSourceQueryContinueDrag(sender, e);
            }

#if PRINT2OUTPUT
            Debug.WriteLine(
                "q handled=" + e.Handled + " action=" + e.Action + " sender=" + sender.GetType() + " Source="
                + e.Source.GetType() + " OriginalSource=" + e.OriginalSource.GetType() + " KeyStates=" + e.KeyStates);
#endif

#if PRINT2BUFFER
            //    ((Window1)Application.Current.MainWindow).buf1.Append('q');
#endif
        }

        #endregion
    }
}