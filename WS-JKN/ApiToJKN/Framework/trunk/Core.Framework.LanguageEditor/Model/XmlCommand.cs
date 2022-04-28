using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Framework.Helper;
using Core.Framework.LanguageEditor.Contract;

namespace Core.Framework.LanguageEditor.Model
{
    public class XmlCommand : IXmlCommand
    {
        public List<Item> XmlSource(string directory)
        {
            return BaseDependency.Get<IXmlRepository>().XmlSource(directory);
        }

        public bool SaveXml(string key, string translate, string directory)
        {
            return BaseDependency.Get<IXmlRepository>().SaveXml(key, translate, directory);
        }

        public List<FileView> GetAllView()
        {
            return BaseDependency.Get<IXmlRepository>().GetAllView();
        }
    }
}
