﻿namespace Core.Framework.Windows.Helper
{
    using System;
    using System.Security.Permissions;
    using System.Windows.Threading;

    /// <summary>
    /// </summary>
    public static class DispatcherHelper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Simulate Application.DoEvents function of <see cref=" System.Windows.Forms.Application" /> class.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrames),
                frame);

            try
            {
                Dispatcher.PushFrame(frame);
            }
            catch (InvalidOperationException)
            {
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private static object ExitFrames(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;

            return null;
        }

        #endregion
    }
}