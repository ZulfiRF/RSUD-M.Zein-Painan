///
/// Copyright(C) MixModes Inc. 2010
/// 

namespace Core.Framework.Windows.Framework
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    ///     Wrapper class that creates a wrapper for DependencyPropertyDescriptor
    /// </summary>
    public class ObservableDependencyProperty
    {
        #region Fields

        private readonly DependencyProperty _dependencyProperty;

        private readonly DependencyPropertyDescriptor _descriptor;

        private readonly DependencyPropertyChangedEventHandler _onDependencyPropertyChanged;

        private bool _changeEventInProgress;

        private object _oldValue;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="ObservableDependencyPropertyCollection&lt;T&gt;.ObservableDependencyProperty" /> class.
        /// </summary>
        /// <param name="targetType">Type of the target</param>
        /// <param name="dependencyProperty">Dependency property.</param>
        /// <param name="OnDependencyPropertyChanged">Dependency property changed callback</param>
        public ObservableDependencyProperty(
            Type targetType,
            DependencyProperty dependencyProperty,
            DependencyPropertyChangedEventHandler OnDependencyPropertyChanged)
        {
            this._descriptor = DependencyPropertyDescriptor.FromProperty(dependencyProperty, targetType);
            this._dependencyProperty = dependencyProperty;
            this._onDependencyPropertyChanged = OnDependencyPropertyChanged;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Enables property monitoring for a dependency object
        /// </summary>
        /// <param name="dependencyObject">The dependency object</param>
        public void AddValueChanged(DependencyObject dependencyObject)
        {
            this._oldValue = dependencyObject.GetValue(this._dependencyProperty);
            this._descriptor.AddValueChanged(dependencyObject, this.OnValueChanged);
        }

        /// <summary>
        ///     Disables property monitoring for a dependency object
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        public void RemoveValueChanged(DependencyObject dependencyObject)
        {
            this._descriptor.RemoveValueChanged(dependencyObject, this.OnValueChanged);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when value of dependency property has changed
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnValueChanged(object sender, EventArgs args)
        {
            if (this._changeEventInProgress)
            {
                return;
            }

            this._changeEventInProgress = true;

            object oldValue = this._oldValue;
            this._oldValue = (sender as DependencyObject).GetValue(this._dependencyProperty);

            this._onDependencyPropertyChanged(
                sender,
                new DependencyPropertyChangedEventArgs(this._dependencyProperty, oldValue, this._oldValue));

            this._changeEventInProgress = false;
        }

        #endregion

        // Private members
    }
}