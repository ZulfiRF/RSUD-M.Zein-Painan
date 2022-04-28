///
/// Copyright(C) MixModes Inc. 2010
/// 

namespace Core.Framework.Windows.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Interactivity;

    using Core.Framework.Windows.Windows;

    public abstract class WindowsManagerBehavior : Behavior<FrameworkElement>
    {
        #region Properties

        /// <summary>
        ///     Windows manager
        /// </summary>
        protected WindowsManager WindowsManager { get; private set; }

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
            this.FindWindowsManager();
        }

        /// <summary>
        ///     Finds the windows manager
        /// </summary>
        /// <exception cref="InvalidOperationException">WindowsManager does not exist in logical tree</exception>
        private void FindWindowsManager()
        {
            DependencyObject currentElement = this.AssociatedObject;

            while (currentElement != null)
            {
                currentElement = LogicalTreeHelper.GetParent(currentElement);

                if (currentElement is WindowsManager)
                {
                    this.WindowsManager = currentElement as WindowsManager;
                    return;
                }
            }

            throw new InvalidOperationException("No WindowsManager found in logical tree");
        }

        #endregion
    }
}