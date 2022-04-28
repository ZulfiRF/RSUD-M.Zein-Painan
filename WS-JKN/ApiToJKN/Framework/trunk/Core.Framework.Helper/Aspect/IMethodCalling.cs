namespace Core.Framework.Helper.Aspect
{
    using System.Reflection;

    public interface IMethodCalling
    {
        #region Public Methods and Operators

        void PostExecute(object obj, MethodBase method, object result, params object[] args);

        void PreExecute(object obj, MethodBase method, params object[] args);

        #endregion
    }
}