namespace Core.Framework.Windows.Behaviours
{
    using System.Windows;
    using System.Windows.Interactivity;

    using Core.Framework.Windows.Controls;

    public class GlowWindowBehavior : Behavior<Window>
    {
        #region Fields

        private GlowWindow bottom;

        private GlowWindow left, right, top;

        #endregion

        #region Public Methods and Operators

        public void Hide()
        {
            this.left.Hide();
            this.right.Hide();
            this.bottom.Hide();
            this.top.Hide();
        }

        public void Show()
        {
            this.left.Show();
            this.right.Show();
            this.top.Show();
            this.bottom.Show();
        }

        #endregion

        #region Methods

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Loaded += (sender, e) =>
            {
                this.left = new GlowWindow(this.AssociatedObject, GlowDirection.Left);
                this.right = new GlowWindow(this.AssociatedObject, GlowDirection.Right);
                this.top = new GlowWindow(this.AssociatedObject, GlowDirection.Top);
                this.bottom = new GlowWindow(this.AssociatedObject, GlowDirection.Bottom);

                this.Show();

                this.left.Update();
                this.right.Update();
                this.top.Update();
                this.bottom.Update();
            };

            this.AssociatedObject.Closed += (sender, args) =>
            {
                if (this.left != null)
                    this.left.Close();
                if (this.right != null)
                    this.right.Close();
                if (this.top != null)
                    this.top.Close();
                if (this.bottom != null)
                    this.bottom.Close();
            };
        }

        #endregion
    }
}