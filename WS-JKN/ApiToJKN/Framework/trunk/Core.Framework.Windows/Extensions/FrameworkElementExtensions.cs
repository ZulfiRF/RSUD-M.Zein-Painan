///
/// Copyright(C) MixModes Inc. 2010
/// 

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Framework.Windows.Extensions
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    ///     FrameworkElement extension methods
    /// </summary>
    public static class FrameworkElementExtensions
    {
        public static IEnumerable<TContainer> Containers<TContainer>(this ItemsControl itemsControl) where TContainer : class
        {

            var fieldInfo = typeof(ItemContainerGenerator).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
            var list = (IList)fieldInfo.GetValue(itemsControl.ItemContainerGenerator);
            for (var i = 0; i < list.Count; i++)
            {
                var container = itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as TContainer;
                if (container != null)
                    yield return container;
            }
        }

        public static IEnumerable<TObject> Except<TObject>(this IEnumerable<TObject> first, params TObject[] second)
        {
            return first.Except((IEnumerable<TObject>)second);
        }

        public static IEnumerable<object> LogicalTreeDepthFirstTraversal(this DependencyObject node)
        {
            if (node == null) yield break;
            yield return node;

            foreach (var child in LogicalTreeHelper.GetChildren(node).OfType<DependencyObject>()
                .SelectMany(depObj => depObj.LogicalTreeDepthFirstTraversal()))
                yield return child;
        }

        public static IEnumerable<object> VisualTreeDepthFirstTraversal(this DependencyObject node)
        {
            if (node == null) yield break;
            yield return node;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(node); i++)
            {
                var child = VisualTreeHelper.GetChild(node, i);
                foreach (var d in child.VisualTreeDepthFirstTraversal())
                {
                    yield return d;
                }
            }
        }

        /// <summary>
        /// Yields the visual ancestory (including the starting point).
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> VisualTreeAncestory(this DependencyObject dependencyObject)
        {
            if (dependencyObject == null) throw new ArgumentNullException("dependencyObject");

            while (dependencyObject != null)
            {
                yield return dependencyObject;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }
        }
        // Private members

        #region Constants

        private const string NotificationToolTipResourceName = "NotificationToolTip";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Clears the adorner layer for the element
        /// </summary>
        /// <param name="element">The element.</param>
        public static void ClearAdornerLayer(this FrameworkElement element)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(element);
            if (layer == null)
            {
                return;
            }

            Adorner[] adorners = layer.GetAdorners(element);

            if ((adorners == null) || (adorners.Length == 0))
            {
                return;
            }

            foreach (Adorner adorner in adorners)
            {
                layer.Remove(adorner);
            }
        }

        /// <summary>
        ///     This method ensures that the Widths and Heights are initialized.
        ///     Sizing to content produces Width and Height values of Double.NaN.
        ///     Because this Adorner explicitly resizes, the Width and Height
        ///     need to be set first.  It also sets the maximum size of the adorned element.
        /// </summary>
        /// <param name="elementToResize">Element to resize</param>
        public static void EnforceSize(this FrameworkElement elementToResize)
        {
            if (elementToResize.Width.Equals(Double.NaN))
            {
                elementToResize.Width = elementToResize.DesiredSize.Width;
            }

            if (elementToResize.Height.Equals(Double.NaN))
            {
                elementToResize.Height = elementToResize.DesiredSize.Height;
            }
        }

        /// <summary>
        ///     Gets the first logical parent of specified type or null if no parent of that type is found
        /// </summary>
        /// <typeparam name="T">Type of parent to search</typeparam>
        /// <param name="element">The element</param>
        /// <returns>First logical parent of specified type or null if no parent of that type is found</returns>
        public static T GetLogicalParent<T>(this FrameworkElement element) where T : FrameworkElement
        {
            DependencyObject parent = element;

            do
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }
            while ((parent != null) && (!(parent is T)));

            return parent as T;
        }

        /// <summary>
        ///     Gets the first visual parent of specified type or null if no parent of that type is found
        /// </summary>
        /// <typeparam name="T">Type of parent to search</typeparam>
        /// <param name="element">The element</param>
        /// <returns>First visual parent of specified type or null if no parent of that type is found</returns>
        public static T GetVisualParent<T>(this FrameworkElement element) where T : FrameworkElement
        {
            DependencyObject parent = element;

            do
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            while ((parent != null) && (!(parent is T)));

            return parent as T;
        }

        /// <summary>
        ///     Shows the notification tool tip on a FrameworkElement at the bottom
        /// </summary>
        /// <param name="element">FrameworkElement instance to display notification on</param>
        /// <param name="content">Notification content</param>
        public static void ShowNotificationToolTip(this FrameworkElement element, object content)
        {
            ShowNotificationToolTip(element, content, PlacementMode.Bottom);
        }

        /// <summary>
        ///     Shows the notification tool tip on a FrameworkElement at specified PlacementMode value
        /// </summary>
        /// <param name="element">FrameworkElement instance to display notification on</param>
        /// <param name="content">Notification content</param>
        /// <param name="placementMode">Placement mode.</param>
        public static void ShowNotificationToolTip(
            this FrameworkElement element,
            object content,
            PlacementMode placementMode)
        {
            var toolTip = Application.Current.Resources[NotificationToolTipResourceName] as ToolTip;

            if (toolTip == null)
            {
                throw new InvalidOperationException(
                    string.Format("ToolTip resource with key {0} not found.", NotificationToolTipResourceName),
                    null);
            }

            object contentClass = new { ToolTipContent = content };

            element.ToolTip = toolTip;

            toolTip.DataContext = contentClass;

            toolTip.PlacementTarget = element;
            toolTip.Placement = placementMode;
            toolTip.IsOpen = true;
        }

        #endregion
    }

   
}