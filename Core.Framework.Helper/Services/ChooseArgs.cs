using System;

namespace Core.Framework.Helper.Services
{
    public class ChooseArgs : EventArgs
    {
        #region Fields

        private readonly Message message;

        #endregion

        #region Constructors and Destructors

        public ChooseArgs(Message message)
        {
            message = message;
        }

        #endregion

        #region Public Properties

        public Message Message
        {
            get
            {
                return message;
            }
        }

        #endregion
    }
}