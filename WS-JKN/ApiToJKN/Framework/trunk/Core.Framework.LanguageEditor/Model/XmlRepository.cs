using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;
using Core.Framework.Helper.Logging;
using Core.Framework.LanguageEditor.Contract;

namespace Core.Framework.LanguageEditor.Model
{
    public class XmlRepository : IXmlRepository
    {
        /// <summary>
        /// method untuk menyimpan settingan pada file xml
        /// </summary>
        /// <param name="key">parameter string untuk Key</param>
        /// <param name="translate">parameter string untuk hasil Transalate</param>
        /// <param name="directory">parameter string untuk Directory</param>
        public bool SaveXml(string key, string translate, string directory)
        {
            try
            {
                var elements = Elements; //tampung data yang ada pada Property Elements ke variable elements
                foreach (var xElement in elements.ToList())
                {
                    var xAttributeKey = xElement.Attribute("key"); //ambil attribute key
                    if (xAttributeKey != null && xAttributeKey.Value == key) //jika value dari attribute sama dengan Property Key
                    {
                        xElement.RemoveAll();
                        elements.Remove(xElement);
                        var xml = new XElement("Setting",
                                              new XElement("item",
                                                           new XAttribute("key", key),
                                                           new XAttribute("value", translate)
                                                  )
                           );
                        xml.Add(elements);
                        xml.Save(directory);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Log.ThrowError(e, "400");
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// method untuk menampilkan isi dari xml
        /// </summary>
        /// <param name="directory">parameter string untuk alamat directory</param>
        /// <returns></returns>
        public List<Item> XmlSource(string directory)
        {
            Elements = XElement.Load(directory).Elements("item").ToList(); //Ambil element item dari xml
            var list = new List<Item>();
            foreach (var element in Elements)
            {
                var items = new Item();
                var xAttributeKey = element.Attribute("key"); //Ambil attribute key
                if (xAttributeKey != null)
                {
                    var key = xAttributeKey.Value.Split('-');

                    if (key[1] == "Tooltip")
                    {
                        items.Component = key[0] + "-" + key[1];
                        items.Key = key[2];
                    }
                    else
                    {
                        items.Component = key[0];
                        items.Key = key[1];
                    }
                }
                var xAttributeValue = element.Attribute("value"); //Ambil attribute value
                if (xAttributeValue != null)
                {
                    var value = xAttributeValue.Value;
                    items.Translate = value;
                }
                list.Add(items);
            }
            return list;
        }

        public List<FileView> GetAllView()
        {
            var dic = Path.GetDirectoryName(Application.ResourceAssembly.Location);
            var path = dic + @"\Language\";
            var list = new List<FileView>();
            var directoryInfo = new DirectoryInfo(path);
            foreach (var file in directoryInfo.GetFiles())
            {
                list.Add(new FileView()
                {
                    NamaFile = file.Name,
                    Path = file.FullName,
                    Tag = file
                });
            }
            return list;
        }

        /// <summary>
        /// property untuk menampung XElement
        /// </summary>
        public static List<XElement> Elements { get; set; }
        /// <summary>
        /// property untuk menampung pesan
        /// </summary>
        public string Msg { get; set; }
    }
}
