namespace Core.Framework.Helper.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;

    using Core.Framework.Helper.Contracts;
    using Core.Framework.Helper.Security;

    public class SettingXml : ISetting
    {
        #region Fields

        private readonly string pathFile;

        private XElement xe;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// untuk spesifik
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public SettingXml(string path, string name)
        {
            try
            {
                if (!path.EndsWith("\\"))
                {
                    pathFile = path + "\\" + name + ".xml";
                }
                else
                {
                    pathFile = path + "" + name + ".xml";
                }
                if (!File.Exists(pathFile))
                {
                    xe = new XElement("Setting");
                    xe.Save(pathFile);
                }
                else
                {
                    xe = XElement.Load(pathFile);
                }
            }
            catch (Exception)
            {
            }
        }

        public SettingXml()
        {
            try
            {
                var fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
                if (!fileInfo.DirectoryName.EndsWith("\\"))
                {
                    pathFile = fileInfo.DirectoryName + "\\setting.xml";
                }
                else
                {
                    pathFile = fileInfo.DirectoryName + "setting.xml";
                }
                if (!File.Exists(pathFile))
                {
                    xe = new XElement("Setting");
                    xe.Save(pathFile);
                }
                else
                {
                    xe = XElement.Load(pathFile);
                }
            }
            catch (Exception)
            {
            }
        }

        public SettingXml(string nameFile)
        {
            try
            {
                pathFile = Directory.GetCurrentDirectory() + "\\" + nameFile + ".xml";
                if (!File.Exists(pathFile))
                {
                    xe = new XElement("Setting");
                    xe.Save(pathFile);
                }
                else
                {
                    xe = XElement.Load(pathFile);
                }
            }
            catch (Exception)
            {
            }
        }

        public SettingXml(string nameFile, TypeFile type)
        {
            try
            {
                switch (type)
                {
                    case TypeFile.NameFile:
                        pathFile = Directory.GetCurrentDirectory() + "\\" + nameFile + ".xml";
                        break;
                    case TypeFile.PathFile:
                        pathFile = nameFile;
                        break;
                    default:
                        break;
                }

                if (!File.Exists(pathFile))
                {
                    xe = new XElement("Setting");
                    xe.Save(pathFile);
                }
                else
                {
                    xe = XElement.Load(pathFile);
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Enums

        public enum TypeFile
        {
            NameFile,

            PathFile
        }

        #endregion

        #region Public Properties

        public string PathFile
        {
            get
            {
                return pathFile;
            }
        }

        #endregion

        #region Public Methods and Operators

        public string Delete(string key)
        {
            try
            {
                IEnumerable<XElement> doc = xe.Descendants("item");
                XElement node = doc.FirstOrDefault(n => n.Attribute("key").Value.Equals(key));
                if (node != null)
                {
                    node.Remove();
                }
                xe.Save(pathFile);
                xe = XElement.Load(pathFile);
                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string[] GetAllKey()
        {
            try
            {
                return xe.Descendants("item").Select(n => n.Attribute("key").Value).ToArray();
            }
            catch (Exception)
            {
            }
            return null;
        }

        public string GetPathFile()
        {
            return pathFile;
        }

        public string GetValue(string key)
        {
            try
            {
                IEnumerable<XElement> doc = xe.Descendants("item");
                XElement node = doc.FirstOrDefault(n => n.Attribute("key").Value.Equals(key));

                if (node != null)
                {
                    if (node.Attribute("flag") != null)
                    {
                        if (node.Attribute("flag").Value.ToLower().Equals("true"))
                        {
                            return Cryptography.FuncAesDecrypt(node.Attribute("value").Value);
                        }
                    }
                    return node.Attribute("value").Value;
                }
                //xe.Add(new XElement("item", new XAttribute("key", key), new XAttribute("value", "")));
                //xe.Save(pathFile);
                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string GetKey(string value)
        {
            try
            {
                IEnumerable<XElement> doc = xe.Descendants("item");
                XElement node = doc.FirstOrDefault(n => n.Attribute("value").Value.Equals(value));

                if (node != null)
                {
                    if (node.Attribute("flag") != null)
                    {
                        if (node.Attribute("flag").Value.ToLower().Equals("true"))
                        {
                            return Cryptography.FuncAesDecrypt(node.Attribute("key").Value);
                        }
                    }
                    return node.Attribute("key").Value;
                }
                //xe.Add(new XElement("item", new XAttribute("key", key), new XAttribute("value", "")));
                //xe.Save(pathFile);
                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public IEnumerable<string> GetValues(string key)
        {
            IEnumerable<XElement> doc = xe.Descendants("item");

            foreach (XElement source in doc.Where(n => n.Attribute("key").Value.Equals(key)))
            {
                XAttribute xAttribute = source.Attribute("value");
                if (xAttribute != null)
                {
                    yield return xAttribute.Value;
                }
            }
        }

        public string Log(string type, string value)
        {
            try
            {
                xe.Add(
                    new XElement(
                        "Log",
                        new XAttribute("Type", type),
                        new XElement("Value", value),
                        new XAttribute("Date", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"))));
                xe.Save(pathFile);
                xe = XElement.Load(pathFile);
                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string Save(string key, string value)
        {
            try
            {
                IEnumerable<XElement> doc = xe.Descendants("item");
                XElement node = doc.FirstOrDefault(n => n.Attribute("key").Value.Equals(key));
                if (node != null)
                {
                    node.Attribute("value").Value = value;
                }
                else
                {
                    xe.Add(new XElement("item", new XAttribute("key", key), new XAttribute("value", value)));
                }
                xe.Save(pathFile);
                xe = XElement.Load(pathFile);
                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string Save(string key, string value, bool useEncryte)
        {
            try
            {
                IEnumerable<XElement> doc = xe.Descendants("item");
                XElement node = doc.FirstOrDefault(n => n.Attribute("key").Value.Equals(key));
                if (useEncryte)
                {
                    value = Cryptography.FuncAesEncrypt(value);
                }
                if (node != null)
                {
                    node.Attribute("value").Value = value;
                }
                else
                {
                    xe.Add(
                        new XElement(
                            "item",
                            new XAttribute("key", key),
                            new XAttribute("value", value),
                            new XAttribute("flag", useEncryte)));
                }
                xe.Save(pathFile);
                xe = XElement.Load(pathFile);
                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string Save(string key, IEnumerable<string> values)
        {
            try
            {
                IEnumerable<XElement> doc = xe.Descendants("item");
                IEnumerable<XElement> nodes = doc.Where(n => n.Attribute("key").Value.Equals(key));
                nodes.Remove();
                foreach (string value in values)
                {
                    xe.Add(new XElement("item", new XAttribute("key", key), new XAttribute("value", value)));
                }

                xe.Save(pathFile);
                xe = XElement.Load(pathFile);
                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string Update(string key, string newValue)
        {
            try
            {
                IEnumerable<XElement> doc = xe.Descendants("item");
                XElement node = doc.FirstOrDefault(n => n.Attribute("key").Value.Equals(key));
                if (node != null)
                {
                    if (node.Attribute("flag") != null && node.Attribute("flag").Value.ToLower().Equals("true"))
                    {
                        newValue = Cryptography.FuncAesDecrypt(newValue);
                    }
                    node.Attribute("value").Value = newValue;
                }
                xe.Save(pathFile);
                xe = XElement.Load(pathFile);
                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        #endregion
    }
}