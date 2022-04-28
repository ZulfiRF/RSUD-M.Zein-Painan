using System;
using System.Windows;
using Core.Framework.Helper;
using Core.Framework.Security.Contracts;
using Core.Framework.Security.Control;
using Core.Framework.Windows.Contracts;

namespace Core.Framework.Security.Common
{
    public class CommonModuleForbidden : IGenericCalling, IPassingView
    {
        public string Title { get { return "Tidak Ada Akses"; } }
        public void InitView()
        {
            var view = BaseDependency.Get<IForbiddenView>();            
            var command = BaseDependency.Get<IForbiddenCommand>();
            var presenter = new ForbiddenPresenter(view, command);
            presenter.ShowForbiden -= PresenterOnShowForbiden;
            presenter.ShowForbiden += PresenterOnShowForbiden;
            var main = Application.Current.MainWindow as IMainWindow;
            if (main != null)
            {
                presenter.Initialize();
                view.PassingView = PassingView;
                view.SetAtasan(UserHead);
                main.SetContent(view as FrameworkElement, Title);
            }
        }

        private void PresenterOnShowForbiden(object sender, ItemEventArgs<IForbiddenView> e)
        {
            var main = Application.Current.MainWindow as IMainWindow;
            if (main != null)
            {
                main.ChangeContent(e.Item.ForbiddenVIew, e.Item.PassingView, "", null);
            }
        }

        public FrameworkElement PassingView { get; set; }
        public string UserHead { get; set; }
    }
}
