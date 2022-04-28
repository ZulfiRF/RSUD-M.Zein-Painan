namespace Core.Framework.Helper.Aspect
{
    using System;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;

    public class InterceptSink : IMessageSink
    {
        #region Fields

        private readonly IMessageSink nextSink;

        #endregion

        #region Constructors and Destructors

        public InterceptSink(IMessageSink nextSink)
        {
            nextSink = nextSink;
        }

        #endregion

        #region Public Properties

        public IMessageSink NextSink
        {
            get
            {
                return nextSink;
            }
        }

        #endregion

        #region Public Methods and Operators

        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            IMessageCtrl rtnMsgCtrl = nextSink.AsyncProcessMessage(msg, replySink);
            return rtnMsgCtrl;
        }

        public IMessage SyncProcessMessage(IMessage msg)
        {
            var mcm = msg as IMethodCallMessage;
            PreProcess(ref mcm);
            IMethodReturnMessage mrm = default(IMethodReturnMessage);
            try
            {
                IMessage rtnMsg = nextSink.SyncProcessMessage(msg);

                mrm = rtnMsg as IMethodReturnMessage;

                return mrm;
            }
            catch (Exception)
            {
            }
            finally
            {
                PostProcess(mcm, ref mrm);
            }

            return null;
            //return msg;
        }

        #endregion

        #region Methods

        private void PostProcess(IMethodCallMessage callMsg, ref IMethodReturnMessage rtnMsg)
        {
            //PostProcessAttribute[] attrs
            //    = (PostProcessAttribute[])callMsg.MethodBase.GetCustomAttributes(typeof(PostProcessAttribute), true);
            //for (int i = 0; i < attrs.Length; i++)
            //    attrs[i].Processor.Process(callMsg, ref rtnMsg);
            foreach (IMethodCalling result in callMsg.MethodBase.GetCustomAttributes(true).OfType<IMethodCalling>())
            {
                result.PostExecute(callMsg, callMsg.MethodBase, rtnMsg.ReturnValue, callMsg.InArgs);
            }
        }

        private void PreProcess(ref IMethodCallMessage msg)
        {
            //PreProcessAttribute[] attrs
            //    = (PreProcessAttribute[])msg.MethodBase.GetCustomAttributes(typeof(PreProcessAttribute), true);
            //for (int i = 0; i < attrs.Length; i++)
            //    attrs[i].Processor.Process(ref msg);
            foreach (IMethodCalling result in msg.MethodBase.GetCustomAttributes(true).OfType<IMethodCalling>())
            {
                result.PreExecute(msg, msg.MethodBase, msg.InArgs);
            }
        }

        #endregion
    }
}