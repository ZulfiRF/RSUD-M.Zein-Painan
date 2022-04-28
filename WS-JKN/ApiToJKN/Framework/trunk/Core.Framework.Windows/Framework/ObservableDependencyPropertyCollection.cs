///
/// Copyright(C) MixModes Inc. 2010
/// 

namespace Core.Framework.Windows.Framework
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Windows;

    /// <summary>
    ///     Observable dependency property collection
    /// </summary>
    /// <typeparam name="T">Type of objects whose property is desired to be monitored</typeparam>
    public class ObservableDependencyPropertyCollection<T> : ObservableCollection<T>
        where T : DependencyObject
    {
        #region Fields

        private readonly List<ObservableDependencyProperty> _descriptors = new List<ObservableDependencyProperty>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableDependencyPropertyCollection&lt;T&gt;" /> class.
        ///     All the dependency properties in the inheritance hierarchy is monitored for changes
        /// </summary>
        public ObservableDependencyPropertyCollection()
        {
            this.MonitorAllDependencyProperties();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableDependencyPropertyCollection&lt;T&gt;" /> class.
        ///     All the dependency properties in the inheritance hierarchy is monitored for changes
        /// </summary>
        /// <param name="list">Collection whose dependency properties are to be monitored</param>
        public ObservableDependencyPropertyCollection(List<T> list)
            : base(list)
        {
            this.MonitorAllDependencyProperties();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableDependencyPropertyCollection&lt;T&gt;" /> class.
        ///     Only dependency properties specified in properties are monitored
        /// </summary>
        /// <param name="properties">Dependency properties to monitor in collection</param>
        public ObservableDependencyPropertyCollection(params DependencyProperty[] properties)
        {
            this.MonitorExplicitDependencyProperties(properties);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableDependencyPropertyCollection&lt;T&gt;" /> class
        ///     Only dependency properties specified in properties are monitored
        /// </summary>
        /// <param name="list">Collection whose dependency properties are to be monitored</param>
        /// <param name="properties">Dependency properties to monitor in collection</param>
        public ObservableDependencyPropertyCollection(List<T> list, params DependencyProperty[] properties)
            : base(list)
        {
            this.MonitorExplicitDependencyProperties(properties);
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when dependency property has changed
        /// </summary>
        public event DependencyPropertyChangedEventHandler DependencyPropertyChanged;

        #endregion

        #region Methods

        /// <summary>
        ///     Raises the <see cref="E:CollectionChanged" /> event.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs" />
        ///     instance containing the event data.
        /// </param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            base.OnCollectionChanged(args);

            if (args.NewItems != null)
            {
                foreach (DependencyObject obj in args.NewItems)
                {
                    foreach (ObservableDependencyProperty descriptor in this._descriptors)
                    {
                        descriptor.AddValueChanged(obj);
                    }
                }
            }

            if (args.OldItems != null)
            {
                foreach (DependencyObject obj in args.OldItems)
                {
                    foreach (ObservableDependencyProperty descriptor in this._descriptors)
                    {
                        descriptor.RemoveValueChanged(obj);
                    }
                }
            }
        }

        /// <summary>
        ///     Called when dependency property of an item in collection has changed
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="args">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance
        ///     containing the event data.
        /// </param>
        protected void OnDependencyPropertyChanged(object item, DependencyPropertyChangedEventArgs args)
        {
            if (this.DependencyPropertyChanged != null)
            {
                this.DependencyPropertyChanged(item, args);
            }
        }

        /// <summary>
        ///     Creates the descriptor for property change callback
        /// </summary>
        /// <param name="property">Dependency property whose descriptor is to be created</param>
        private void CreateDescriptor(DependencyProperty property)
        {
            var descriptor = new ObservableDependencyProperty(typeof(T), property, this.OnDependencyPropertyChanged);
            this._descriptors.Add(descriptor);
        }

        /// <summary>
        ///     Monitors all dependency properties.
        /// </summary>
        private void MonitorAllDependencyProperties()
        {
            FieldInfo[] fieldInfos =
                typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                if (fieldInfo.FieldType == typeof(DependencyProperty))
                {
                    this.CreateDescriptor(fieldInfo.GetValue(null) as DependencyProperty);
                }
            }
        }

        /// <summary>
        ///     Monitors explicit dependency properties
        /// </summary>
        /// <param name="properties">Dependency properties to monitor in collection</param>
        private void MonitorExplicitDependencyProperties(params DependencyProperty[] properties)
        {
            foreach (DependencyProperty property in properties)
            {
                this.CreateDescriptor(property);
            }
        }

        #endregion

        // Private members
    }
}