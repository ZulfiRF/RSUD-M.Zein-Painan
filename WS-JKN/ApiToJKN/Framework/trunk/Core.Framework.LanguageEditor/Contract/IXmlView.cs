using System.Collections.Generic;
using System.IO;
using Core.Framework.Helper.Presenters;
using Core.Framework.LanguageEditor.Model;
using Core.Framework.LanguageEditor.Presenter;

namespace Core.Framework.LanguageEditor.Contract
{
    public interface IXmlView : IAttachPresenter<XmlPresenter>
    {
        string Key { get; }
        DirectoryInfo Names { get; }
        string DirectoryName { get; set; }
        string Directory { get; }
        string Translate { get; }
        List<FileView> SetSource { set; }
    }
}