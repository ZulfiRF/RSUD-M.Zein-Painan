///
/// Copyright(C) MixModes Inc. 2010
/// 

namespace Core.Framework.Windows.Adorners
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using Core.Framework.Windows.Extensions;

    /// <summary>
    ///     Content adorner base class that can be populated with any visual
    /// </summary>
    internal abstract class ContentAdornerBase : AdornerBase
    {
        #region Fields

        private readonly ContentControl _contentControl;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes an Adorner
        /// </summary>
        /// <param name="adornedElement">The element to bind the adorner to</param>
        /// <exception cref="ArgumentNullException">adornedElement is null</exception>
        internal ContentAdornerBase(UIElement adornedElement, FrameworkElement content)
            : base(adornedElement)
        {
            this._contentControl = new ContentControl();
            this._contentControl.Content = content;
            this._contentControl.ApplyTemplate();
            this._visualChildren.Add(this._contentControl);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     When overridden in a derived class, positions child elements and
        ///     determines a size for a <see cref="T:System.Windows.FrameworkElement" /> derived class.
        /// </summary>
        /// <param name="finalSize">
        ///     The final area within the parent that this element
        ///     should use to arrange itself and its children.
        /// </param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            (this.AdornedElement as FrameworkElement).EnforceSize();

            this._contentControl.Arrange(new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height));

            return finalSize;
        }

        /// <summary>
        ///     Finds content with specified name
        /// </summary>
        /// <param name="name">Name of the element to find</param>
        /// <returns>An element with matching name if one exists; null otherwise</returns>
        protected T FindElement<T>(string name) where T : FrameworkElement
        {
            var contentPresenter = VisualTreeHelper.GetChild(this._contentControl, 0) as ContentPresenter;
            FrameworkElement content = null;

            if ((contentPresenter == null) || ((content = contentPresenter.Content as FrameworkElement) == null)
                || (name == null))
            {
                return null;
            }

            var searchStack = new Stack<FrameworkElement>();
            searchStack.Push(content);

            while (searchStack.Count > 0)
            {
                FrameworkElement element = searchStack.Pop();

                if (name.Equals(element.Tag))
                {
                    return element as T;
                }

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {
                    var childElement = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                    if (childElement != null)
                    {
                        searchStack.Push(childElement);
                    }
                }
            }

            return null;
        }

        #endregion

        // Private members
    }
}