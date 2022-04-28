using System.IO;
using System.Reflection;


namespace Core.Framework.Windows.Implementations
{
    using System;

    using Microsoft.Office.Interop.Excel;
    using Excel = Microsoft.Office.Interop.Excel;
    public class CoreUserControlUseExcel : CoreUserControl
    {
        #region Fields

        private string fileName;

        private Application myExcelApp;

        private Workbook myExcelWorkbook;

        private Workbooks myExcelWorkbooks;

        private Worksheet myExcelWorksheet;

        #endregion

        #region Constructors and Destructors

        public CoreUserControlUseExcel()
            : base()
        {
            //Initialized += OnInitialized;
            //this.MyGuid = Guid.NewGuid().ToString();
        }

        //private void OnInitialized(object sender, EventArgs e)
        //{
        //    OnInitialized(e);
        //}

        //private void OnInitialized(object sender, EventArgs e)
        //{
        //}

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        ~CoreUserControlUseExcel()
        {
            if (this.myExcelApp != null)
            {
                try
                {
                    this.myExcelApp.Visible = false;
                    this.myExcelApp.Quit();
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion

        #region Public Properties


        public Range Cells
        {
            get
            {
                try
                {
                    if (this.myExcelWorksheet.Cells != null)
                    {
                        return this.myExcelWorksheet.Cells;
                    }
                    return null;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }

            }
        }

        public string FileName
        {
            get
            {
                return this.fileName;
            }
            set
            {
                if (this.myExcelApp != null)
                {
                    this.myExcelApp.Visible = false;
                    this.myExcelApp.Quit();
                }
                string pathDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                fileName = pathDirectory + "\\" + value;
                object misValue = System.Reflection.Missing.Value;

                myExcelApp = new Excel.Application(); //Excel.ApplicationClass();
                myExcelApp.Visible = false;
                myExcelWorkbooks = myExcelApp.Workbooks;
                if (File.Exists(fileName))
                {
                    myExcelWorkbook = myExcelWorkbooks.Open(
                        fileName,
                        misValue,
                        misValue,
                        misValue,
                        misValue,
                        misValue,
                        misValue,
                        misValue,
                        misValue,
                        misValue,
                        misValue,
                        misValue,
                        misValue,
                        misValue,
                        misValue);
                    myExcelWorksheet = (Excel.Worksheet)myExcelWorkbook.ActiveSheet;
                }
            }
        }

        #endregion

        #region Public Methods and Operators


        public void Open()
        {
            this.myExcelApp.Visible = true;
        }

        public void Close()
        {
            this.myExcelApp.Visible = false;            
            this.myExcelApp.Quit();            
        }
        #endregion
    }
}