#define PRINT2OUTPUT

namespace Core.Framework.Windows.Helper.DragDropFrameworkData
{
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Core.Framework.Windows.Helper.DragDropFramework;

    /// <summary>
    ///     This Data Provider represents TabItems.
    ///     Note that custom cursors are used.
    ///     When a TabItem is dragged within its
    ///     original container, the cursor is an arrow,
    ///     otherwise its a custom cursor with an
    ///     arrow and a page.
    /// </summary>
    /// <typeparam name="TContainer">Drag source container type</typeparam>
    /// <typeparam name="TObject">Drag source object type</typeparam>
    public class TabControlDataProvider<TContainer, TObject> : DataProviderBase<TContainer, TObject>, IDataProvider
        where TContainer : ItemsControl where TObject : TabItem
    {
        #region Static Fields

        private static Cursor MovePageCursor =
            new Cursor(
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("WpfDragAndDropSmorgasbord.Images.MovePage.cur"));

        private static Cursor MovePageNotCursor =
            new Cursor(
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("WpfDragAndDropSmorgasbord.Images.MovePageNot.cur"));

        #endregion

        #region Constructors and Destructors

        public TabControlDataProvider(string dataFormatString)
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
                    DragDropEffects.Move | // Move tab from one TabControl to another
                    DragDropEffects.Link | // Move tabs within the same TabControl
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
                // Move the tab to be the first in the list
                e.UseDefaultCursors = false;
                Mouse.OverrideCursor = MovePageCursor;
                e.Handled = true;
            }
            else if (e.Effects == DragDropEffects.Link)
            {
                // Drag tabs around
                e.UseDefaultCursors = false;
                Mouse.OverrideCursor = Cursors.Arrow;
                e.Handled = true;
            }
            else
            {
                e.UseDefaultCursors = false;
                Mouse.OverrideCursor = MovePageNotCursor;
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