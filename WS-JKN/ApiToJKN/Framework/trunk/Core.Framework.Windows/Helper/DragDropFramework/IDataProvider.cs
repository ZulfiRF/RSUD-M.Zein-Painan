using System.Windows;

namespace Core.Framework.Windows.Helper.DragDropFramework
{
   
    public interface IControlProvider<T> : IControlProvider
    {
      
        T Data { get; set; }
    }
    /// <summary>
    ///     A representation of a data object that is
    ///     dragged and dropped using this framework
    /// </summary>
    public interface IDataProvider
    {
        #region Public Properties

        /// <summary>
        ///     Return true to add an adorner to the dragged object
        /// </summary>
        bool AddAdorner { get; }

        /// <summary>
        ///     Returns the drag operations supported by this data object provider
        /// </summary>
        DragDropEffects AllowedEffects { get; }

        /// <summary>
        ///     Returns the actions used by this data object provider
        /// </summary>
        DataProviderActions DataProviderActions { get; }

        /// <summary>
        ///     The adorner (when used)
        /// </summary>
        DefaultAdorner DragAdorner { get; set; }

        /// <summary>
        ///     Return true to capture the mouse while dragging
        /// </summary>
        bool NeedsCaptureMouse { get; }

        /// <summary>
        ///     OriginalSource from MouseButtonEventArgs
        /// </summary>
        object OriginalSourceObject { get; set; }

        /// <summary>
        ///     Drag source container, e.g. TabControl.
        /// </summary>
        object SourceContainer { get; set; }

        /// <summary>
        ///     Drag source object, e.g. TabItem
        /// </summary>
        object SourceObject { get; set; }

        /// <summary>
        ///     Point where LeftMouseDown occurred,
        ///     relative to the drag source object's origin
        /// </summary>
        Point StartPosition { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Called after DragDrop.DoDragDrop() returns.
        ///     Typically during a file move, for example, the file is deleted here.
        ///     However, when moving a TabItem from one TabControl to another the
        ///     source TabItem must be unparented from the source TabControl
        ///     before it can be added to the destination TabControl.
        ///     So most of the time when moving items between item controls,
        ///     this method isn't used.
        ///     Provide your own method if you wish; making sure
        ///     to define DoDragDrop_Done in DataProviderActions.
        /// </summary>
        /// <param name="resultEffects">The drop operation that was performed</param>
        void DoDragDropDone(DragDropEffects resultEffects);

        /// <summary>
        ///     Provide your own method for displaying
        ///     the correct cursor during a drag.
        ///     Make sure to define GiveFeedback in DataProviderActions.
        /// </summary>
        /// <param name="sender">GiveFeedback event sender</param>
        /// <param name="e">GiveFeedback event arguments</param>
        void DragSourceGiveFeedback(object sender, GiveFeedbackEventArgs e);

        /// <summary>
        ///     Saves EscapePressed and KeyStates when
        ///     QueryContinueDrag is defined in DataProviderActions.
        ///     Provide your own method if you wish; making sure
        ///     to define QueryContinueDrag in DataProviderActions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DragSourceQueryContinueDrag(object sender, QueryContinueDragEventArgs e);

        /// <summary>
        ///     Called by drag-and-drop framework to initialize the class
        /// </summary>
        void Init();

        /// <summary>
        ///     Returns true when the specified source container, source object
        ///     and original source object are supported by this data object provider.
        ///     Saves the parameters in SourceContainer, SourceObject and
        ///     OriginalSourceObject, respectively, when initFlag is true.
        /// </summary>
        /// <param name="initFlag">When true, initialize the class and source/container values</param>
        /// <param name="dragSourceContainer">Mouse event <code>sender</code></param>
        /// <param name="dragSourceObject">Mouse event args <code>Source</code></param>
        /// <param name="dragOriginalSourceObject">Mouse event args <code>Source</code></param>
        /// <returns>True for a supported container and object; false otherwise</returns>
        bool IsSupportedContainerAndObject(
            bool initFlag,
            object dragSourceContainer,
            object dragSourceObject,
            object dragOriginalSourceObject);

        /// <summary>
        ///     Sets the data passed to WPF
        ///     DragDrop.DoDragDrop()
        /// </summary>
        /// <param name="data"></param>
        void SetData(ref DataObject data);

        /// <summary>
        ///     Provide your own method to remove the source object from its container.
        ///     This method is typically called when the source object is dropped and
        ///     must be removed from its container.
        /// </summary>
        void Unparent();

        #endregion
    }
}