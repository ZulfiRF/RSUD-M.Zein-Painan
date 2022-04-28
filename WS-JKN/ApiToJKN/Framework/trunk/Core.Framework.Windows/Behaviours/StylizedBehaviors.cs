namespace Core.Framework.Windows.Behaviours
{
    using System.Windows;
    using System.Windows.Interactivity;

    public class StylizedBehaviors
    {
        #region Static Fields

        public static readonly DependencyProperty BehaviorsProperty = DependencyProperty.RegisterAttached(
            @"Behaviors",
            typeof(StylizedBehaviorCollection),
            typeof(StylizedBehaviors),
            new FrameworkPropertyMetadata(null, OnPropertyChanged));

        private static readonly DependencyProperty OriginalBehaviorProperty =
            DependencyProperty.RegisterAttached(
                @"OriginalBehaviorInternal",
                typeof(Behavior),
                typeof(StylizedBehaviors),
                new UIPropertyMetadata(null));

        #endregion

        #region Public Methods and Operators

        public static StylizedBehaviorCollection GetBehaviors(DependencyObject uie)
        {
            return (StylizedBehaviorCollection)uie.GetValue(BehaviorsProperty);
        }

        public static void SetBehaviors(DependencyObject uie, StylizedBehaviorCollection value)
        {
            uie.SetValue(BehaviorsProperty, value);
        }

        #endregion

        #region Methods

        private static int GetIndexOf(BehaviorCollection itemBehaviors, Behavior behavior)
        {
            int index = -1;

            Behavior orignalBehavior = GetOriginalBehavior(behavior);

            for (int i = 0; i < itemBehaviors.Count; i++)
            {
                Behavior currentBehavior = itemBehaviors[i];

                if (currentBehavior == behavior || currentBehavior == orignalBehavior)
                {
                    index = i;
                    break;
                }

                Behavior currentOrignalBehavior = GetOriginalBehavior(currentBehavior);

                if (currentOrignalBehavior == behavior || currentOrignalBehavior == orignalBehavior)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private static Behavior GetOriginalBehavior(DependencyObject obj)
        {
            return obj.GetValue(OriginalBehaviorProperty) as Behavior;
        }

        private static void OnPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            var uie = dpo as UIElement;

            if (uie == null)
            {
                return;
            }

            BehaviorCollection itemBehaviors = Interaction.GetBehaviors(uie);

            var newBehaviors = e.NewValue as StylizedBehaviorCollection;
            var oldBehaviors = e.OldValue as StylizedBehaviorCollection;

            if (newBehaviors == oldBehaviors)
            {
                return;
            }

            if (oldBehaviors != null)
            {
                foreach (Behavior behavior in oldBehaviors)
                {
                    int index = GetIndexOf(itemBehaviors, behavior);

                    if (index >= 0)
                    {
                        itemBehaviors.RemoveAt(index);
                    }
                }
            }

            if (newBehaviors != null)
            {
                foreach (Behavior behavior in newBehaviors)
                {
                    int index = GetIndexOf(itemBehaviors, behavior);

                    if (index < 0)
                    {
                        var clone = (Behavior)behavior.Clone();
                        SetOriginalBehavior(clone, behavior);
                        itemBehaviors.Add(clone);
                    }
                }
            }
        }

        private static void SetOriginalBehavior(DependencyObject obj, Behavior value)
        {
            obj.SetValue(OriginalBehaviorProperty, value);
        }

        #endregion
    }
}