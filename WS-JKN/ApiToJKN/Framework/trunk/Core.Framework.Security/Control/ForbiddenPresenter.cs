using System;
using System.Windows;
using Core.Framework.Helper;
using Core.Framework.Helper.Presenters;
using Core.Framework.Security.Contracts;
using Core.Framework.Windows.Contracts;

namespace Core.Framework.Security.Control
{
    public class ForbiddenPresenter : BasePresenterViewCommand<IForbiddenView, IForbiddenCommand>
    {
        public ForbiddenPresenter(IForbiddenView view, IForbiddenCommand command)
            : base(view, command)
        {
        }

        public override void Initialize(params object[] parameters)
        {
            base.Initialize(parameters);
        }

        public bool Login(bool isNewLogin = false)
        {
            if (Command.Authentication(View.Username, View.Passowrd, isNewLogin))
                return true;
            else
                return false;
        }

        public event EventHandler<ItemEventArgs<IForbiddenView>> ShowForbiden;
        protected virtual void OnShowForbiden(ItemEventArgs<IForbiddenView> e)
        {
            var handler = ShowForbiden;
            if (handler != null) handler(this, e);
        }

        public void ChangedView()
        {
            OnShowForbiden(new ItemEventArgs<IForbiddenView>(View));            
        }

        public bool IsHavingAccess(string namaUserIntervace, bool newLogin = false)
        {
            var repository = BaseDependency.Get<IOtorisasiLogin>();
            return repository.isHavingAccess(namaUserIntervace, newLogin);
        }

        public string MsgException()
        {
            return Command.MsgException;
        }

        
    }
}