﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Core.Framework.Windows.Implementations
{    
    public class VisualFeedbackContentControl : ContentControl
    {
        static VisualFeedbackContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VisualFeedbackContentControl), new FrameworkPropertyMetadata(typeof(VisualFeedbackContentControl)));
        }

        public VisualFeedbackContentControl()
        {   
            MouseMove += OnMouseMove;                        
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this);
            MouseLeftButtonDownX = position.X;
            MouseLeftButtonDownY = position.Y;

            base.OnPreviewMouseLeftButtonDown(e);
        }

        private void OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            var position = mouseEventArgs.GetPosition(this);
            MouseX = position.X;
            MouseY = position.Y;
        }

        private static readonly DependencyPropertyKey MouseXPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "MouseX", typeof (double), typeof (VisualFeedbackContentControl),
                new PropertyMetadata(default(double)));

        public static readonly DependencyProperty MouseXProperty =
            MouseXPropertyKey.DependencyProperty;

        public double MouseX
        {
            get { return (double) GetValue(MouseXProperty); }
            private set { SetValue(MouseXPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey MouseYPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "MouseY", typeof (double), typeof (VisualFeedbackContentControl),
                new PropertyMetadata(default(double)));

        public static readonly DependencyProperty MouseYProperty =
            MouseYPropertyKey.DependencyProperty;

        public double MouseY
        {
            get { return (double) GetValue(MouseYProperty); }
            private set { SetValue(MouseYPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey MouseLeftButtonDownXPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "MouseLeftButtonDownX", typeof (double), typeof (VisualFeedbackContentControl),
                new PropertyMetadata(default(double)));

        public static readonly DependencyProperty MouseLeftButtonDownXProperty =
            MouseLeftButtonDownXPropertyKey.DependencyProperty;

        public double MouseLeftButtonDownX
        {
            get { return (double) GetValue(MouseLeftButtonDownXProperty); }
            private set { SetValue(MouseLeftButtonDownXPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey MouseLeftButtonDownYPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "MouseLeftButtonDownY", typeof (double), typeof (VisualFeedbackContentControl),
                new PropertyMetadata(default(double)));

        public static readonly DependencyProperty MouseLeftButtonDownYProperty =
            MouseLeftButtonDownYPropertyKey.DependencyProperty;

        public double MouseLeftButtonDownY
        {
            get { return (double) GetValue(MouseLeftButtonDownYProperty); }
            private set { SetValue(MouseLeftButtonDownYPropertyKey, value); }
        }

    }
}
