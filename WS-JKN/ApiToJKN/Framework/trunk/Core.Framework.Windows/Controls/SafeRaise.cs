namespace Core.Framework.Windows.Controls
{
    using System;

    internal static class SafeRaise
    {
        #region Delegates

        public delegate T GetEventArgs<T>() where T : EventArgs;

        #endregion

        #region Public Methods and Operators

        public static void Raise(EventHandler eventToRaise, object sender)
        {
            if (eventToRaise != null)
            {
                eventToRaise(sender, EventArgs.Empty);
            }
        }

        public static void Raise(EventHandler<EventArgs> eventToRaise, object sender)
        {
            Raise(eventToRaise, sender, EventArgs.Empty);
        }

        public static void Raise<T>(EventHandler<T> eventToRaise, object sender, T args) where T : EventArgs
        {
            if (eventToRaise != null)
            {
                eventToRaise(sender, args);
            }
        }

        public static void Raise<T>(EventHandler<T> eventToRaise, object sender, GetEventArgs<T> getEventArgs)
            where T : EventArgs
        {
            if (eventToRaise != null)
            {
                eventToRaise(sender, getEventArgs());
            }
        }

        #endregion
    }
}