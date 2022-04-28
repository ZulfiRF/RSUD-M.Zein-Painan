using System;
using System.Windows.Media.Animation;

namespace Core.Framework.Windows.Implementations.Drag
{
    internal class StoryboardCompletionListener
    {
        private readonly Storyboard _storyboard;
        private readonly Action<Storyboard> _continuation;

        public StoryboardCompletionListener(Storyboard storyboard, Action<Storyboard> continuation)
        {
            if (storyboard == null) throw new ArgumentNullException("storyboard");
            if (continuation == null) throw new ArgumentNullException("continuation");

            _storyboard = storyboard;
            _continuation = continuation;

            _storyboard.Completed += StoryboardOnCompleted;
        }

        private void StoryboardOnCompleted(object sender, EventArgs eventArgs)
        {
            _storyboard.Completed -= StoryboardOnCompleted;
            _continuation(_storyboard);
        }
    }
}