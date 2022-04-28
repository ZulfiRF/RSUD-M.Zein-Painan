using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace Core.Framework.Windows.Helper
{
    public class VisibilityControl
    {
        public string FileName { get; set; }
        public string Control { get; set; }
        public string DefaultValue { get; set; }
        public bool IsVisible { get; set; }
    }
}