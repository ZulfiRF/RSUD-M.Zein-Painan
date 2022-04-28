using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Core.Framework.Helper;
using Core.Framework.LanguageEditor.Contract;
using Core.Framework.LanguageEditor.Presenter;
using Core.Framework.Windows.Contracts;

namespace Core.Framework.LanguageEditor.Common
{
    public class CommonModuleLanguage : IGenericCalling
    {
        public string Title { get { return "Editor Bahasa"; } }
        public void InitView()
        {
            var view = BaseDependency.GetNewInstance<IXmlView>();
            var command = BaseDependency.GetNewInstance<IXmlCommand>();
            var presenter = new XmlPresenter(view, command);
            var main = Application.Current.MainWindow as IMainWindow;
            if (main != null)
            {
                presenter.Initialize();
                main.SetContent(view as FrameworkElement, Title);
            }
        }
    }
}
