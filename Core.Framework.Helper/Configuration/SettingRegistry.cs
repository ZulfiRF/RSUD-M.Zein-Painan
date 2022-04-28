namespace Core.Framework.Helper.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using Core.Framework.Helper.Contracts;

    using Microsoft.Win32;

    public class SettingRegistry : ISetting
    {
        #region Fields

        private readonly string pathFile;

        private string alamatRegistry;


        #endregion

        #region Constructors and Destructors

        public SettingRegistry()
        {
            try
            {
                pathFile = "";
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\PT JASAMEDIKA", true);

                if (regKey == null) // Key not existing, we need to create the key.
                {
                    // Use this if you are creating autorun on all users on the comp.
                    // Registry.LocalMachine.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");

                    // Use this if you are creating autorun on this user only.
                    Registry.CurrentUser.CreateSubKey("SOFTWARE\\PT JASAMEDIKA");
                }

                // Now lets add the autorun to the registry.

                // Replace ApplicationName and ApplictaionPath with the name of your program, and
                // the path of your program.
            }
            catch (Exception)
            {
            }
        }

        public SettingRegistry(string nameFile)
        {
            try
            {
                alamatRegistry = nameFile;
            }
            catch (Exception)
            {
            }
        }

        public SettingRegistry(string nameFile, TypeFile type)
        {
            try
            {
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
                return new string[1];
                //return xe.Descendants("item").Select(n => n.Attribute("key").Value).ToArray();
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
                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string GetKey(string value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetValues(string key)
        {
            yield break;
            //IEnumerable<XElement> doc = xe.Descendants("item");

            //foreach (XElement source in doc.Where(n => n.Attribute("key").Value.Equals(key)))
            //{
            //    XAttribute xAttribute = source.Attribute("value");
            //    if (xAttribute != null)
            //    {
            //        yield return xAttribute.Value;
            //    }
            //}
        }

        public string Log(string type, string value)
        {
            try
            {
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
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\PT JASAMEDIKA", true);
                if (regKey != null)
                {
                    object obj = regKey.GetValue(key);
                    regKey.SetValue(key, value, RegistryValueKind.String);
                }

                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string Save(string key, string value, bool useEncryte)
        {
            throw new NotImplementedException();
        }

        public string Save(string key, IEnumerable<string> values)
        {
            return "";
        }

        public string Update(string key, string newValue)
        {
            try
            {
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