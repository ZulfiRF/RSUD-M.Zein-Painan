#define PRINT2BUFFER
#define PRINT2OUTPUT

namespace Core.Framework.Windows.Helper.DragDropFrameworkData
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Shapes;

    using Core.Framework.Windows.Helper.DragDropFramework;

    /// <summary>
    ///     This Data Provider represents items found on a StackPanel,
    ///     allowing them to be drag data.
    ///     Note that text specific to the object is also added to the drag data;
    ///     this allows StackPanel items to be dragged onto the Rich Text Box.
    /// </summary>
    /// <typeparam name="TContainer">Drag source container type</typeparam>
    /// <typeparam name="TObject">Drag source object type</typeparam>
    public class StackPanelDataProvider<TContainer, TObject> : DataProviderBase<TContainer, TObject>, IDataProvider
        where TContainer : StackPanel where TObject : UIElement
    {
        #region Constructors and Destructors

        public StackPanelDataProvider(string dataFormatString)
            : base(dataFormatString)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Return true so an addorner is added when an item is dragged
        /// </summary>
        public override bool AddAdorner
        {
            get
            {
                return true;
            }
        }

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
                // Image buttons can return the image as the source, so look for the button
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

        /// <summary>
        ///     Not only add the DataProvider class, also add a string
        /// </summary>
        public override void SetData(ref DataObject data)
        {
            // Set default data
            Debug.Assert(data.GetDataPresent(this.SourceDataFormat) == false, "Shouldn't set data more than once");
            data.SetData(this.SourceDataFormat, this);

            // Look for a System.String
            string textString = null;

            if (this.SourceObject is Rectangle)
            {
                var rect = (Rectangle)this.SourceObject;
                if (rect.Fill != null)
                {
                    textString = rect.Fill.ToString();
                }
            }
            else if (this.SourceObject is TextBlock)
            {
                var textBlock = (TextBlock)this.SourceObject;
                textString = textBlock.Text;
            }
            else if (this.SourceObject is Button)
            {
                var button = (Button)this.SourceObject;
                if (button.ToolTip != null)
                {
                    textString = button.ToolTip.ToString();
                }
            }

            if (textString != null)
            {
                data.SetData(textString);
            }
        }

        public override void Unparent()
        {
            var item = this.SourceObject as TObject;
            var panel = this.SourceContainer as TContainer;

            Debug.Assert(item != null, "Unparent expects a non-null item");
            Debug.Assert(panel != null, "Unparent expects a non-null panel");

            if ((panel != null) && (item != null))
            {
                panel.Children.Remove(item);
            }
        }

        #endregion
    }
}