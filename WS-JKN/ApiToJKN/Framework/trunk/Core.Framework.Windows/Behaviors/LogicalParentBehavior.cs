///
/// Copyright(C) MixModes Inc. 2010
/// 

namespace Core.Framework.Windows.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Interactivity;

    /// <summary>
    ///     Behavior that is attached to a logical parent of specified type that may or may not be immidiate parent of the
    ///     behavior
    /// </summary>
    /// <typeparam name="T">Type of logical parent</typeparam>
    /// <remarks>
    ///     1. The first parent that matches the specified type is assumed to be the logical parent that behavior shall use
    ///     2. Do not use AssociatedObject but rather LogicalParent when using derived behaviors of this type
    /// </remarks>
    public abstract class LogicalParentBehavior<T> : Behavior<FrameworkElement>
        where T : FrameworkElement
    {
        #region Properties

        /// <summary>
        ///     Logical parent
        /// </summary>
        protected T LogicalParent { get; private set; }

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
            this.FindLogicalParent();
        }

        /// <summary>
        ///     Finds the logical parent
        /// </summary>
        /// <exception cref="InvalidOperationException">Parent does not exist in logical tree</exception>
        private void FindLogicalParent()
        {
            DependencyObject currentElement = this.AssociatedObject;

            while (currentElement != null)
            {
                currentElement = LogicalTreeHelper.GetParent(currentElement);

                if (currentElement is T)
                {
                    this.LogicalParent = currentElement as T;
                    return;
                }
                if ((currentElement is FrameworkElement) && ((currentElement as FrameworkElement).TemplatedParent is T))
                {
                    this.LogicalParent = (currentElement as FrameworkElement).TemplatedParent as T;
                    return;
                }
            }

            throw new InvalidOperationException("No parent found in logical tree");
        }

        #endregion
    }
}