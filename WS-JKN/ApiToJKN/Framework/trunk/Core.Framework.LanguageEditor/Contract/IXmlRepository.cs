using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Framework.LanguageEditor.Model;

namespace Core.Framework.LanguageEditor.Contract
{
    public interface IXmlRepository
    {
        bool SaveXml(string key, string translate, string directory);
        List<Item> XmlSource(string directory);
        List<FileView> GetAllView();
    }
}
