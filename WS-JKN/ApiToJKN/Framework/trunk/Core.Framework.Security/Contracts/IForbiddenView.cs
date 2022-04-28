using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Core.Framework.Helper.Presenters;
using Core.Framework.Security.Control;

namespace Core.Framework.Security.Contracts
{
    public interface IForbiddenView : IAttachPresenter<ForbiddenPresenter>
    {
        string Username { get; }
        string Passowrd { get; }

        Brush SetUsernames { set; }
        Brush SetPasswords { set; }
        Brush SetRuangans { set; }
        bool SetBtnLogin { set; }

        void ToMainWindow();
        void ClearAll();

        FrameworkElement PassingView { get; set; }
        FrameworkElement ForbiddenVIew { get; }

        void SetAtasan(string atasan);
    }
}
