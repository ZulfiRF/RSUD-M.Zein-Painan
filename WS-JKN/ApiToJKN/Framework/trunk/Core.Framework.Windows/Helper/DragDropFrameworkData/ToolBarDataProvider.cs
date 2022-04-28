#define PRINT2BUFFER
#define PRINT2OUTPUT

namespace Core.Framework.Windows.Helper.DragDropFrameworkData
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    using Core.Framework.Windows.Helper.DragDropFramework;

    /// <summary>
    ///     This Data Provider represents items found on a ToolBar.
    ///     Note that text specific to the object is also added to the drag data;
    ///     this allows canvas items to be dragged onto the Rich Text Box.
    /// </summary>
    /// <typeparam name="TContainer">Drag source container type</typeparam>
    /// <typeparam name="TObject">Drag source object type</typeparam>
    public class ToolBarDataProvider<TContainer, TObject> : DataProviderBase<TContainer, TObject>, IDataProvider
        where TContainer : ItemsControl where TObject : UIElement
    {
        #region Constructors and Destructors

        public ToolBarDataProvider(string dataFormatString)
            : base(dataFormatString)
        {
        }

        #endregion

        #region Public Properties

        public override DragDropEffects AllowedEffects
        {
            get
            {
                return //DragDropEffects.Copy |
                    //DragDropEffects.Scroll |
                    DragDropEffects.Move | DragDropEffects.Link | // Indicates a ToolBar "insert"
                    DragDropEffects.None;
            }
        }

        public override DataProviderActions DataProviderActions
        {
            get
            {
                return //DragDropDataProviderActions.QueryContinueDrag |
                    DataProviderActions.GiveFeedback | //DragDropDataProviderActions.DoDragDrop_Done |
                    DataProviderActions.None;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void DragSourceGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (e.Effects == DragDropEffects.Move)
            {
                e.UseDefaultCursors = true;
                e.Handled = true;
            }
            else if (e.Effects == DragDropEffects.Link)
            {
                e.UseDefaultCursors = true;
                e.Handled = true;
            }
        }

        public override bool IsSupportedContainerAndObject(
            bool initFlag,
            object dragSourceContainer,
            object dragSourceObject,
            object dragOriginalSourceObject)
        {
            var sourceObject = dragSourceObject as TObject;
            // When an image button is clicked,
            // most of the time the image is the <code>e.Source</code>.
            // So when _SourceObject is null, search for a TObject parent.
            if (sourceObject == null)
            {
                sourceObject = Utility.FindParentControlIncludingMe<TObject>(dragSourceObject as DependencyObject);
            }

            if (initFlag)
            {
                // Init DataProvider variables
                this.Init();
                this.SourceContainer = dragSourceContainer;
                this.SourceObject = sourceObject;
                this.OriginalSourceObject = dragOriginalSourceObject;
            }

            return (dragSourceContainer is TContainer) && (sourceObject != null);
        }

        public override void Unparent()
        {
            var item = this.SourceObject as TObject;
            var container = (TContainer)this.SourceContainer;

            Debug.Assert(item != null, "Unparent expects a non-null item");
            Debug.Assert(container != null, "Unparent expects a non-null container");

            if ((container != null) && (item != null))
            {
                container.Items.Remove(item);
            }
        }

        #endregion
    }
}