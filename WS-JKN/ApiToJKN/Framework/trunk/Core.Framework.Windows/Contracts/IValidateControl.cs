using System;

namespace Core.Framework.Windows.Contracts
{
    public interface IValidateControl : IFocusControl
    {
        event EventHandler<HandleArgs> BeforeValidate;

        bool IsRequired { get; set; }

        bool IsNull { get; }

        bool IsError { get; set; }

        void ClearValueControl();

        bool SkipAutoFocus { get; set; }
    }
}