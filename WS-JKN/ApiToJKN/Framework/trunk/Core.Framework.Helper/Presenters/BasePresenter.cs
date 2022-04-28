using System;

namespace Core.Framework.Helper.Presenters
{
    public class BasePresenter : IBasePresenter
    {
      
        #region Public Methods and Operators

        public virtual void Initialize(params object[] parameters)
        {
            Parameters = parameters;
        }

        #endregion

        public object[] Parameters { get;protected set; }

       
    }
}