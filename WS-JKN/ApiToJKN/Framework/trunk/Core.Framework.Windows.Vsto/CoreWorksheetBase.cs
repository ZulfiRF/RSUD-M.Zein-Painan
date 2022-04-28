using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Framework.Windows.Vsto.Contracts;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Excel;

namespace Core.Framework.Windows.Vsto
{
    public class CoreWorkbookBase : WorkbookBase
    {
        public static CoreWorksheetBase CurrentSheet;
        public CoreWorkbookBase(Factory factory, IServiceProvider serviceProvider, string primaryCookie, string identifier)
            : base(factory, serviceProvider, primaryCookie, identifier)
        {
            SheetActivate += OnSheetActivated;
            Startup += CoreWorkbookBaseStartup;

        }

        void CoreWorkbookBaseStartup(object sender, EventArgs e)
        {
            dynamic currentSheet = ActiveSheet;
            CurrentSheet = CoreWorksheetBase.GetSheet(currentSheet._CodeName);
            RefreshView();
        }



        private void OnSheetActivated(object sh)
        {
            dynamic currentSheet = sh;
            CurrentSheet = CoreWorksheetBase.GetSheet(currentSheet._CodeName);
            RefreshView();
        }

        private static void RefreshView()
        {
            if (CurrentSheet is IView)
            {
                (CurrentSheet as IView).InitView();
            }
        }
    }
    public class CoreWorksheetBase : WorksheetBase
    {
        private static Dictionary<string, CoreWorksheetBase> currentList = new Dictionary<string, CoreWorksheetBase>();
        public CoreWorksheetBase(Factory factory, IServiceProvider serviceProvider, string primaryCookie, string identifier)
            : base(factory, serviceProvider, primaryCookie, identifier)
        {
            CoreWorksheetBase result;
            if (!currentList.TryGetValue(primaryCookie, out result))
                currentList.Add(primaryCookie, this);
        }
        public static CoreWorksheetBase GetSheet(string key)
        {
            return currentList[key];
        }
    }
}
