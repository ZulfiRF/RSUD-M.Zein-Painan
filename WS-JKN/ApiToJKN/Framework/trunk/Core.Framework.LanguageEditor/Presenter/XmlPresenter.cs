using System;
using System.Collections.Generic;
using System.Threading;
using Core.Framework.Helper.Presenters;
using Core.Framework.LanguageEditor.Contract;
using Core.Framework.LanguageEditor.Model;

namespace Core.Framework.LanguageEditor.Presenter
{
    public class XmlPresenter : BasePresenterViewCommand<IXmlView, IXmlCommand>
    {
        public XmlPresenter(IXmlView view, IXmlCommand command):base(view,command)
        {
        }

        public override void Initialize(params object[] parameters)
        {
            base.Initialize(parameters);
        }

        public List<Item> XmlSource()
        {
            var source = Command.XmlSource(View.Directory);
            return source;
        }

        public bool SaveXml()
        {
            return Command.SaveXml(View.Key, View.Translate, View.Directory);
        }

        public void LoadData()
        {
            ThreadPool.QueueUserWorkItem(LoadDataCallBack);
        }

        private void LoadDataCallBack(object state)
        {
            var result = Command.GetAllView();
            View.SetSource = result;
        }
    }
}
