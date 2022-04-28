using System;
using System.Collections.Generic;
using System.Windows.Media.Animation;

namespace Core.Framework.Windows.Implementations.Drag
{
    internal static class StoryboardCompletionListenerExtension
    {
        private static readonly IDictionary<Storyboard, Action<Storyboard>> ContinuationIndex = new Dictionary<Storyboard, Action<Storyboard>>();        

        public static void WhenComplete(this Storyboard storyboard, Action<Storyboard> continuation)
        {
// ReSharper disable once ObjectCreationAsStatement
            new StoryboardCompletionListener(storyboard, continuation);
        }        
    }
}