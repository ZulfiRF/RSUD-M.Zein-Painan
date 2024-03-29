﻿namespace Core.Framework.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    public partial class RevealImage
    {
        #region Static Fields

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            "Image",
            typeof(ImageSource),
            typeof(RevealImage),
            new UIPropertyMetadata(null));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(RevealImage),
            new UIPropertyMetadata(""));

        #endregion

        #region Constructors and Destructors

        public RevealImage()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Properties

        public ImageSource Image
        {
            get
            {
                return (ImageSource)this.GetValue(ImageProperty);
            }
            set
            {
                this.SetValue(ImageProperty, value);
            }
        }

        public string Text
        {
            get
            {
                return (string)this.GetValue(TextProperty);
            }
            set
            {
                this.SetValue(TextProperty, value);
            }
        }

        #endregion

        #region Methods

        private static void TypewriteTextblock(string textToAnimate, TextBlock txt, TimeSpan timeSpan)
        {
            var story = new Storyboard { FillBehavior = FillBehavior.HoldEnd };

            DiscreteStringKeyFrame discreteStringKeyFrame;
            var stringAnimationUsingKeyFrames = new StringAnimationUsingKeyFrames { Duration = new Duration(timeSpan) };

            string tmp = string.Empty;
            foreach (char c in textToAnimate)
            {
                discreteStringKeyFrame = new DiscreteStringKeyFrame { KeyTime = KeyTime.Paced };
                tmp += c;
                discreteStringKeyFrame.Value = tmp;
                stringAnimationUsingKeyFrames.KeyFrames.Add(discreteStringKeyFrame);
            }

            Storyboard.SetTargetName(stringAnimationUsingKeyFrames, txt.Name);
            Storyboard.SetTargetProperty(stringAnimationUsingKeyFrames, new PropertyPath(TextBlock.TextProperty));
            story.Children.Add(stringAnimationUsingKeyFrames);
            story.Begin(txt);
        }

        private void GridMouseEnter(object sender, MouseEventArgs e)
        {
            TypewriteTextblock(this.Text.ToUpper(), this.textBlock, TimeSpan.FromSeconds(.25));
        }

        #endregion
    }
}