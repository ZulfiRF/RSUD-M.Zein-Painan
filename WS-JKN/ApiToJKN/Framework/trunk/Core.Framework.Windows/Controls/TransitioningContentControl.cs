// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Core.Framework.Windows.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;

    public class TransitioningContentControl : ContentControl
    {
        #region Constants

        public const string DefaultTransitionState = "DefaultTransition";

        internal const string CurrentContentPresentationSitePartName = "CurrentContentPresentationSite";

        internal const string NormalState = "Normal";

        internal const string PresentationGroup = "PresentationStates";

        internal const string PreviousContentPresentationSitePartName = "PreviousContentPresentationSite";

        #endregion

        #region Static Fields

        public static readonly DependencyProperty CustomVisualStatesProperty =
            DependencyProperty.Register(
                "CustomVisualStates",
                typeof(ObservableCollection<VisualState>),
                typeof(TransitioningContentControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IsTransitioningProperty =
            DependencyProperty.Register(
                "IsTransitioning",
                typeof(bool),
                typeof(TransitioningContentControl),
                new PropertyMetadata(OnIsTransitioningPropertyChanged));

        public static readonly DependencyProperty RestartTransitionOnContentChangeProperty =
            DependencyProperty.Register(
                "RestartTransitionOnContentChange",
                typeof(bool),
                typeof(TransitioningContentControl),
                new PropertyMetadata(false, OnRestartTransitionOnContentChangePropertyChanged));

        public static readonly DependencyProperty TransitionProperty = DependencyProperty.Register(
            "Transition",
            typeof(string),
            typeof(TransitioningContentControl),
            new PropertyMetadata(DefaultTransitionState, OnTransitionPropertyChanged));

        #endregion

        #region Fields

        private bool _allowIsTransitioningWrite;

        private Storyboard _currentTransition;

        #endregion

        #region Constructors and Destructors

        public TransitioningContentControl()
        {
            this.CustomVisualStates = new ObservableCollection<VisualState>();
            this.DefaultStyleKey = typeof(TransitioningContentControl);
        }

        #endregion

        #region Public Events

        public event RoutedEventHandler BeginAutoSizeWindow;
        public event RoutedEventHandler TransitionCompleted;

        #endregion

        #region Public Properties

        public bool AutoSizeWindow { get; set; }
        public ContentPresenter CurrentContentPresentationSite { get; set; }

        public ObservableCollection<VisualState> CustomVisualStates
        {
            get
            {
                return (ObservableCollection<VisualState>)this.GetValue(CustomVisualStatesProperty);
            }
            set
            {
                this.SetValue(CustomVisualStatesProperty, value);
            }
        }

        public bool IsTransitioning
        {
            get
            {
                return (bool)this.GetValue(IsTransitioningProperty);
            }
            private set
            {
                this._allowIsTransitioningWrite = true;
                this.SetValue(IsTransitioningProperty, value);
                this._allowIsTransitioningWrite = false;
            }
        }

        public ContentPresenter PreviousContentPresentationSite { get; set; }

        public bool RestartTransitionOnContentChange
        {
            get
            {
                return (bool)this.GetValue(RestartTransitionOnContentChangeProperty);
            }
            set
            {
                this.SetValue(RestartTransitionOnContentChangeProperty, value);
            }
        }

        public string Transition
        {
            get
            {
                return this.GetValue(TransitionProperty) as string;
            }
            set
            {
                this.SetValue(TransitionProperty, value);
            }
        }

        #endregion

        #region Properties

        private Storyboard CurrentTransition
        {
            get
            {
                return this._currentTransition;
            }
            set
            {
                // decouple event
                if (this._currentTransition != null)
                {
                    this._currentTransition.Completed -= this.OnTransitionCompleted;
                }

                this._currentTransition = value;

                if (this._currentTransition != null)
                {
                    this._currentTransition.Completed += this.OnTransitionCompleted;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        public void AbortTransition()
        {
            // go to normal state and release our hold on the old content.
            VisualStateManager.GoToState(this, NormalState, false);
            this.IsTransitioning = false;
            if (this.PreviousContentPresentationSite != null)
            {
                this.PreviousContentPresentationSite.Content = null;
            }
        }

        public override void OnApplyTemplate()
        {
            if (this.IsTransitioning)
            {
                this.AbortTransition();
            }

            if (this.CustomVisualStates != null && this.CustomVisualStates.Any())
            {
                VisualStateGroup presentationGroup = VisualStates.TryGetVisualStateGroup(this, PresentationGroup);
                if (presentationGroup != null)
                {
                    foreach (VisualState state in this.CustomVisualStates)
                    {
                        presentationGroup.States.Add(state);
                    }
                }
            }

            base.OnApplyTemplate();

            this.PreviousContentPresentationSite =
                this.GetTemplateChild(PreviousContentPresentationSitePartName) as ContentPresenter;
            this.CurrentContentPresentationSite =
                this.GetTemplateChild(CurrentContentPresentationSitePartName) as ContentPresenter;

            if (this.CurrentContentPresentationSite != null)
            {
                this.CurrentContentPresentationSite.Content = this.Content;
            }

            // hookup currenttransition
            Storyboard transition = this.GetStoryboard(this.Transition);
            this.CurrentTransition = transition;
            if (transition == null)
            {
                string invalidTransition = this.Transition;
                // revert to default
                this.Transition = DefaultTransitionState;

                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, "Temporary removed exception message", invalidTransition));
            }
            VisualStateManager.GoToState(this, NormalState, false);
        }

        public void OnBeginAutoSizeWindow(RoutedEventArgs e = null)
        {
            RoutedEventHandler handler = this.BeginAutoSizeWindow;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void ShowAnimation()
        {
            VisualStateManager.GoToState(this, NormalState, false);
            VisualStateManager.GoToState(this, this.Transition, true);
        }

        #endregion

        #region Methods

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            this.StartTransition(oldContent, newContent);
        }

        protected virtual void OnRestartTransitionOnContentChangeChanged(bool oldValue, bool newValue)
        {
        }

        private static void OnIsTransitioningPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (TransitioningContentControl)d;

            if (!source._allowIsTransitioningWrite)
            {
                source.IsTransitioning = (bool)e.OldValue;
                throw new InvalidOperationException();
            }
        }

        private static void OnRestartTransitionOnContentChangePropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((TransitioningContentControl)d).OnRestartTransitionOnContentChangeChanged(
                (bool)e.OldValue,
                (bool)e.NewValue);
        }

        private static void OnTransitionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (TransitioningContentControl)d;
            var oldTransition = e.OldValue as string;
            var newTransition = e.NewValue as string;

            if (source.IsTransitioning)
            {
                source.AbortTransition();
            }

            // find new transition
            Storyboard newStoryboard = source.GetStoryboard(newTransition);

            // unable to find the transition.
            if (newStoryboard == null)
            {
                // could be during initialization of xaml that presentationgroups was not yet defined
                if (VisualStates.TryGetVisualStateGroup(source, PresentationGroup) == null)
                {
                    // will delay check
                    source.CurrentTransition = null;
                }
                else
                {
                    // revert to old value
                    source.SetValue(TransitionProperty, oldTransition);

                    throw new ArgumentException(
                        string.Format(CultureInfo.CurrentCulture, "Temporary removed exception message", newTransition));
                }
            }
            else
            {
                source.CurrentTransition = newStoryboard;
            }
        }

        private Storyboard GetStoryboard(string newTransition)
        {
            VisualStateGroup presentationGroup = VisualStates.TryGetVisualStateGroup(this, PresentationGroup);
            Storyboard newStoryboard = null;
            if (presentationGroup != null)
            {
                newStoryboard =
                    presentationGroup.States.OfType<VisualState>()
                        .Where(state => state.Name == newTransition)
                        .Select(state => state.Storyboard)
                        .FirstOrDefault();
            }
            return newStoryboard;
        }

        private void OnTransitionCompleted(object sender, EventArgs e)
        {
            this.AbortTransition();
            RoutedEventHandler handler = this.TransitionCompleted;
            if (handler != null)
            {
                handler(this, new RoutedEventArgs());
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newContent",
            Justification = "Should be used in the future.")]
        private void StartTransition(object oldContent, object newContent)
        {
            // both presenters must be available, otherwise a transition is useless.
            if (this.CurrentContentPresentationSite != null && this.PreviousContentPresentationSite != null)
            {
                if (this.RestartTransitionOnContentChange)
                {
                    this.CurrentTransition.Completed -= this.OnTransitionCompleted;
                }

                this.CurrentContentPresentationSite.Content = newContent;

                this.PreviousContentPresentationSite.Content = oldContent;

                // and start a new transition
                if (!this.IsTransitioning || this.RestartTransitionOnContentChange)
                {
                    if (this.RestartTransitionOnContentChange)
                    {
                        this.CurrentTransition.Completed += this.OnTransitionCompleted;
                    }
                    this.IsTransitioning = true;
                    if (this.AutoSizeWindow)
                    {
                        this.OnBeginAutoSizeWindow();
                        this.ShowAnimation();
                    }
                    else
                    {
                        this.ShowAnimation();
                    }
                }
            }
        }

        #endregion
    }
}