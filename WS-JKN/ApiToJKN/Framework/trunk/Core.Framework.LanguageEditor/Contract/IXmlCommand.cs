using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Framework.LanguageEditor.Model;

namespace Core.Framework.LanguageEditor.Contract
{
    public interface IXmlCommand
    {
        List<Item> XmlSource(string directory);
        bool SaveXml(string key, string translate, string directory);
        List<FileView> GetAllView();
    }
}
