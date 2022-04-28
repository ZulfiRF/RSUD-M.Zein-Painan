using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Core.Framework.Windows.Windows
{
    public partial class GrowlNotifiactions
    {
        #region Constants

        private const byte MAX_NOTIFICATIONS = 4;

        #endregion

        #region Fields

        public Notifications Notifications = new Notifications();

        private readonly Notifications buffer = new Notifications();

        private int count;

        #endregion

        #region Constructors and Destructors

        public GrowlNotifiactions()
        {
            this.InitializeComponent();
            this.NotificationsControl.DataContext = this.Notifications;
        }

        #endregion

        #region Public Methods and Operators

        public void AddNotification(Notification notification)
        {
            Helper.Manager.Timeout(Dispatcher, () =>
                                                   {

                                                       notification.Id = this.count++;
                                                       if (this.Notifications.Count + 1 > MAX_NOTIFICATIONS)
                                                       {
                                                           this.buffer.Add(notification);
                                                       }
                                                       else
                                                       {
                                                           this.Notifications.Add(notification);
                                                       }

                                                       //Show window if there're notifications
                                                       if (this.Notifications.Count > 0 && !this.IsActive)
                                                       {
                                                           this.Show();
                                                       }

                                                   });
        }

        public void RemoveNotification(Notification notification)
        {
            if (this.Notifications.Contains(notification))
            {
                this.Notifications.Remove(notification);
            }

            if (this.buffer.Count > 0)
            {
                this.Notifications.Add(this.buffer[0]);
                this.buffer.RemoveAt(0);
            }

            //Close window if there's nothing to show
            if (this.Notifications.Count < 1)
            {
                this.Hide();
            }
        }

        #endregion

        #region Methods

        private void NotificationWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Height != 0.0)
            {
                return;
            }
            var element = sender as Grid;
            this.RemoveNotification(this.Notifications.First(n => n.Id == Int32.Parse(element.Tag.ToString())));
        }

        #endregion
    }
}