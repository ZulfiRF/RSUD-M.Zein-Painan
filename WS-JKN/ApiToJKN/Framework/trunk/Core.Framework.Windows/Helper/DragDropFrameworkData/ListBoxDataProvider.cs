#define PRINT2BUFFER
#define PRINT2OUTPUT

namespace Core.Framework.Windows.Helper.DragDropFrameworkData
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    using Core.Framework.Windows.Helper.DragDropFramework;

    /// <summary>
    ///     This Data Provider represents ListBoxItems.
    /// </summary>
    /// <typeparam name="TContainer">Drag source container type</typeparam>
    /// <typeparam name="TObject">Drag source object type</typeparam>
    public class ListBoxDataProvider<TContainer, TObject> : DataProviderBase<TContainer, TObject>, IDataProvider
        where TContainer : ItemsControl where TObject : FrameworkElement
    {
        #region Constructors and Destructors

        public ListBoxDataProvider(string dataFormatString)
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
                    DragDropEffects.Move | // Move ListBoxItem
                    DragDropEffects.Link | // Needed for moving ListBoxItem as a TreeView sibling
                    DragDropEffects.None;
            }
        }

        public override DataProviderActions DataProviderActions
        {
            get
            {
                return DataProviderActions.QueryContinueDrag | // Need Shift key info (for TreeView)
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
                // ... when Shift key is pressed
                e.UseDefaultCursors = true;
                e.Handled = true;
            }
        }

        public override void Unparent()
        {
            var item = this.SourceObject as TObject;
            var container = this.SourceContainer as TContainer;

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