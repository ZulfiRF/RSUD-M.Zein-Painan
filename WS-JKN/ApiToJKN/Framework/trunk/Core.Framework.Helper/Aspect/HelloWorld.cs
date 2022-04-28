namespace Core.Framework.Helper.Aspect
{
    using System;
    using System.Reflection;

    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property,
        AllowMultiple = true)]
    public class HelloWorld : Attribute, IMethodCalling
    {
        #region Constructors and Destructors

        public HelloWorld()
        {

        }

        #endregion

        #region Public Methods and Operators

        public void PostExecute(object obj, MethodBase method, object result, params object[] args)
        {
        }

        public void PreExecute(object obj, MethodBase method, params object[] args)
        {
            method.Invoke(obj, args);
        }

        #endregion
    }
}