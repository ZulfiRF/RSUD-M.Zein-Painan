namespace Core.Framework.Windows.Adorners
{
    using System;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    ///     Base class for adorners
    /// </summary>
    internal abstract class AdornerBase : Adorner, IDisposable
    {
        #region Fields

        protected VisualCollection _visualChildren;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes an Adorner
        /// </summary>
        /// <param name="adornedElement">The element to bind the adorner to</param>
        /// <exception cref="ArgumentNullException">adornedElement is null</exception>
        internal AdornerBase(UIElement adornedElement)
            : base(adornedElement)
        {
            this._visualChildren = new VisualCollection(this);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Number of visual child elements within this element
        /// </summary>
        /// <returns>The number of visual child elements for this element.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return this._visualChildren.Count;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Releases resources and disposes the object
        /// </summary>
        public void Dispose()
        {
            this._visualChildren.Clear();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection</param>
        /// <returns>The requested child element</returns>
        /// <exception cref="IndexOutOfBoundsException">Index out of bounds</exception>
        protected override Visual GetVisualChild(int index)
        {
            return this._visualChildren[index];
        }

        #endregion

        // To store and manage the adorner's visual children.
    }
}