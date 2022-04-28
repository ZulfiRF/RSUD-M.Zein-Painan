using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Core.Framework.Helper.Contracts;
using Microsoft.Win32;

namespace Core.Framework.Helper.Configuration
{
    public class SettingRegistry : ISetting
    {
        #region TypeFile enum

        public enum TypeFile
        {
            NameFile,
            PathFile
        }

        #endregion

        #region Constructor
        private string alamatRegistry;
        public SettingRegistry()
        {
            try
            {
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\PT JASAMEDIKA", true);


                if (regKey == null) // Key not existing, we need to create the key.
                {
                    // Use this if you are creating autorun on all users on the comp.
                    // Registry.LocalMachine.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");

                    // Use this if you are creating autorun on this user only.
                    Registry.LocalMachine.CreateSubKey("SOFTWARE\\PT JASAMEDIKA");
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

        private readonly string pathFile;
        private XElement xe;

        #region ISetting Members

        public string Save(string key, string value)
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

        public string Save(string key, string value, bool useEncryte)
        {
            throw new NotImplementedException();
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

        public string Save(string key, IEnumerable<string> values)
        {
            return "";
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

        public string PathFile
        {
            get { return pathFile; }
        }

        #endregion


        public IEnumerable<string> GetValues(string key)
        {

            IEnumerable<XElement> doc = xe.Descendants("item");

            foreach (var source in doc.Where(n => n.Attribute("key").Value.Equals(key)))
            {
                var xAttribute = source.Attribute("value");
                if (xAttribute != null) yield return xAttribute.Value;
            }

        }
    }
}