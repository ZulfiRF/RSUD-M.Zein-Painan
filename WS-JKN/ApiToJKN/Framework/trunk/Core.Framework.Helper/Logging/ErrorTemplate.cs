using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using Core.Framework.Helper.Contracts;

namespace Core.Framework.Helper.Logging
{
    public class ErrorTemplate : IErrorTemplate
    {
        public static string PathFile { get; set; }

        public ErrorTemplate()
        {
            PathFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Error.Tempalet.xml";
        }

        public static string GetError(string code)
        {
            XDocument xElemenet = null;
            if (string.IsNullOrEmpty(PathFile))
            {
                PathFile = Path.GetDirectoryName(Application.ResourceAssembly.Location) + @"\Error.Template.xml";
            }
            xElemenet = XDocument.Load(PathFile);
            var firstElement = xElemenet.Element("Error");
            if (firstElement != null)
            {
                var element = firstElement.Elements("item").FirstOrDefault(n=>n.Attribute("code").Value == code);
                if (element != null)
                {
                    var elementValue = element.Attribute("code").Value;
                    if (elementValue == code)
                    {
                        return code + " - " + element.Attribute("error").Value;
                    }
                }
            }
            return null;
        }
    }
}
