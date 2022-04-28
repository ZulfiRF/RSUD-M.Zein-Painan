#define PRINT2BUFFER
// Monitor event entry/exit
#define PRINT2OUTPUT
// Output interesting information to Visual Studio's Output window

namespace Core.Framework.Windows.Helper.DragDropFramework
{
    using System.Diagnostics;
    using System.Text;
    using System.Windows;

    /// <summary>
    ///     Manage drop events for IDataConsumers
    /// </summary>
    public class DropManager
    {
        private readonly FrameworkElement dropTarget;

        private readonly IDataConsumer[] dragDropConsumers;

        /// <summary>
        ///     Manage data that is dragged over and dropped on the <code>dropTarget</code>.
        ///     Supported data is defined as one or more classes that implement IDataConsumer.
        /// </summary>
        /// <param name="dropTarget">FrameworkElement monitored for drag events</param>
        /// <param name="dragDropConsumer">Supported data objects</param>
        public DropManager(FrameworkElement dropTarget, IDataConsumer dragDropConsumer)
            : this(dropTarget, new[] { dragDropConsumer })
        {
        }

        /// <summary>
        ///     Manage data that is dragged over and dropped on the <code>dropTarget</code>.
        ///     Supported data is defined as one or more classes that implement IDataConsumer.
        /// </summary>
        /// <param name="dropTarget">FrameworkElement monitored for drag events</param>
        /// <param name="dragDropConsumers">Array of supported data objects</param>
        public DropManager(FrameworkElement dropTarget, IDataConsumer[] dragDropConsumers)
        {
            this.dropTarget = dropTarget;
            Debug.Assert(dropTarget != null);

            this.dragDropConsumers = dragDropConsumers;
            Debug.Assert(dragDropConsumers != null);

            bool hookDragEnter = false;
            bool hookDragOver = false;
            bool hookDrop = false;
            bool hookDragLeave = false;

            // Determine which events to hook
            foreach (IDataConsumer dragDropConsumer in this.dragDropConsumers)
            {
                if ((dragDropConsumer.DataConsumerActions & DataConsumerActions.DragEnter) != 0)
                {
                    hookDragEnter = true;
                }
                if ((dragDropConsumer.DataConsumerActions & DataConsumerActions.DragOver) != 0)
                {
                    hookDragOver = true;
                }
                if ((dragDropConsumer.DataConsumerActions & DataConsumerActions.Drop) != 0)
                {
                    hookDrop = true;
                }
                if ((dragDropConsumer.DataConsumerActions & DataConsumerActions.DragLeave) != 0)
                {
                    hookDragLeave = true;
                }
            }

            if (hookDragEnter || hookDragOver || hookDrop || hookDragLeave)
            {
                this.dropTarget.AllowDrop = true;
            }

            // Hook only the events needed
            if (hookDragEnter)
            {
                this.dropTarget.DragEnter += this.DropTargetDragEnter;
            }
            if (hookDragOver)
            {
                this.dropTarget.DragOver += this.DropTargetDragOver;
            }
            if (hookDrop)
            {
                this.dropTarget.Drop += this.DropTargetDrop;
            }
            if (hookDragLeave)
            {
                this.dropTarget.DragLeave += this.DropTargetDragLeave;
            }
        }

        /// <summary>
        ///     Initial call, after DoDragDrop is called, has Effects and AllowedEffects set to
        ///     allowedEffects as passed to DoDragDrop.  Subsequent Effects and AllowedEffects
        ///     are set to the Effects returned by DragLeave.  Note that DragLeave can return
        ///     effects that are not defined in allowedEffects (as passed to DoDragDrop).
        ///     Source and Original source are set to dragSource as passed to DoDragDrop.
        /// </summary>
        private void DropTargetDragEnter(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException("DragEnter not implemented");
#if PRINT2BUFFER
            // ((Window1)Application.Current.MainWindow).buf0.Append('E');
#endif

            DragDropEffects effects = e.Effects;

#if TESTING
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
#else
            foreach (IDataConsumer dragDropConsumer in this.dragDropConsumers)
            {
                if ((dragDropConsumer.DataConsumerActions & DataConsumerActions.DragEnter) != 0)
                {
                    dragDropConsumer.DropTargetDragEnter(sender, e);
                    if (e.Handled)
                    {
                        break;
                    }
                }
            }

            if (!e.Handled)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
#endif

#if PRINT2OUTPUT
            Debug.WriteLine(
                "E Handled=" + e.Handled + " sender=" + sender.GetType() + " Source=" + e.Source.GetType()
                + " OriginalSource=" + e.OriginalSource.GetType() + " Effects=" + effects + " ReturnedEffects="
                + e.Effects + " AllowedEffects=" + e.AllowedEffects + this.DropObjectFormat(e));
#endif

#if PRINT2BUFFER
            //  ((Window1)Application.Current.MainWindow).buf1.Append('E');
#endif
        }

        /// <summary>
        ///     Occurs when mouse is over the area occupied
        ///     by the dropTarget (specified in the constructor).
        ///     You must likely will provide your own method; make sure
        ///     to define DragOver in DataConsumerActions.
        /// </summary>
        private void DropTargetDragOver(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException("DragOver not implemented");
#if PRINT2BUFFER
            //((Window1)Application.Current.MainWindow).buf0.Append('O');
#endif

            DragDropEffects effects = e.Effects;

#if TESTING
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
#else
            foreach (IDataConsumer dragDropConsumer in this.dragDropConsumers)
            {
                if ((dragDropConsumer.DataConsumerActions & DataConsumerActions.DragOver) != 0)
                {
                    dragDropConsumer.DropTargetDragOver(sender, e);
                    if (e.Handled)
                    {
                        break;
                    }
                }
            }

            if (!e.Handled)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
#endif

#if PRINT2OUTPUT
            Debug.WriteLine(
                "O Handled=" + e.Handled + " sender=" + sender.GetType() + " Source=" + e.Source.GetType()
                + " OriginalSource=" + e.OriginalSource.GetType() + " Effects=" + effects + " ReturnedEffects="
                + e.Effects + " AllowedEffects=" + e.AllowedEffects + this.DropObjectFormat(e));
#endif

#if PRINT2BUFFER
            //    ((Window1)Application.Current.MainWindow).buf1.Append('O');
#endif
        }

        /// <summary>
        ///     Occurs when the left mouse button is released in the area
        ///     occupied by the dropTarget (specified in the constructor).
        ///     You must likely will provide your own method; make sure
        ///     to define Drop in DataConsumerActions.
        ///     See DropTarget_DragEnter in DropManager for additional comments.
        /// </summary>
        private void DropTargetDrop(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException("Drop not implemented");
#if PRINT2BUFFER
            //((Window1)Application.Current.MainWindow).buf0.Append('D');
#endif

            DragDropEffects effects = e.Effects;

#if TESTING
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
#else
            foreach (IDataConsumer dragDropConsumer in this.dragDropConsumers)
            {
                if ((dragDropConsumer.DataConsumerActions & DataConsumerActions.Drop) != 0)
                {
                    dragDropConsumer.DropTargetDrop(sender, e);
                    if (e.Handled)
                    {
                        break;
                    }
                }
            }

            if (!e.Handled)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
#endif

#if PRINT2OUTPUT
            Debug.WriteLine(
                "D Handled=" + e.Handled + " sender=" + sender.GetType() + " Source=" + e.Source.GetType()
                + " OriginalSource=" + e.OriginalSource.GetType() + " Effects=" + effects + " ReturnedEffects="
                + e.Effects + " AllowedEffects=" + e.AllowedEffects + this.DropObjectFormat(e));
#endif

#if PRINT2BUFFER
            // ((Window1)Application.Current.MainWindow).buf1.Append('D');
#endif
        }

        /// <summary>
        ///     Retured effects are passed to *_DragEnter in both Effects and AllowedEffects;
        ///     even effects not included in DoDragDrop's allowedEffects can be used.
        /// </summary>
        private void DropTargetDragLeave(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException("DragLeave not implemented");
#if PRINT2BUFFER
            // ((Window1)Application.Current.MainWindow).buf0.Append('L');
#endif

            DragDropEffects effects = e.Effects;

#if TESTING
            e.Effects = DragDropEffects.Link;
            e.Handled = true;
#else
            foreach (IDataConsumer dragDropConsumer in this.dragDropConsumers)
            {
                if ((dragDropConsumer.DataConsumerActions & DataConsumerActions.DragLeave) != 0)
                {
                    dragDropConsumer.DropTargetDragLeave(sender, e);
                    if (e.Handled)
                    {
                        break;
                    }
                }
            }

            if (!e.Handled)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
#endif

#if PRINT2OUTPUT
            Debug.WriteLine(
                "L Handled=" + e.Handled + " sender=" + sender.GetType() + " Source=" + e.Source.GetType()
                + " OriginalSource=" + e.OriginalSource.GetType() + " Effects=" + effects + " ReturnedEffects="
                + e.Effects + " AllowedEffects=" + e.AllowedEffects + this.DropObjectFormat(e));
#endif

#if PRINT2BUFFER
            // ((Window1)Application.Current.MainWindow).buf1.Append('L');
#endif
        }

#if PRINT2OUTPUT
        /// <summary>
        ///     Returns a string of the data formats contained in the drag data
        /// </summary>
        /// <param name="e">DragEvent argument containing the drag data</param>
        /// <returns>String of drag data object formats</returns>
        private string DropObjectFormat(DragEventArgs e)
        {
            var buffer = new StringBuilder();

            buffer.Append(" DropObjectFormat=");
            string[] dataFormats = e.Data.GetFormats();
            int count = dataFormats.Length;
            for (int i = 0; i < count; ++i)
            {
                if (i > 0)
                {
                    buffer.Append(", ");
                }
                buffer.Append(dataFormats[i]);
            }

            return buffer.ToString();
        }
#endif
    }
}