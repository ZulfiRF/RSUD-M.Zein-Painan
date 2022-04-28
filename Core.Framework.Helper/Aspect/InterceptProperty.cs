namespace Core.Framework.Helper.Aspect
{
    using System;
    using System.Runtime.Remoting.Contexts;
    using System.Runtime.Remoting.Messaging;

    public class InterceptProperty : IContextProperty, IContributeObjectSink
    {
        #region Constructors and Destructors

        public InterceptProperty()
            : base()
        {
        }

        #endregion

        #region Public Properties

        public string Name
        {
            get
            {
                return "Intercept";
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Freeze(Context newContext)
        {
        }

        public IMessageSink GetObjectSink(MarshalByRefObject obj, IMessageSink nextSink)
        {
            return new InterceptSink(nextSink);
        }

        public bool IsNewContextOK(Context newCtx)
        {
            var p = newCtx.GetProperty("Intercept") as InterceptProperty;
            if (p == null)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}