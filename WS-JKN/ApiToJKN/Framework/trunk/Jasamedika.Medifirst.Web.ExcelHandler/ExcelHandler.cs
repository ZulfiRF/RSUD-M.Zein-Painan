using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Medifirst.PointOfService.Impl;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Globalization;
namespace Jasamedika.Medifirst.Web
{
    public class ExcelHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }
        private string _connection = "";
        private Dictionary<string, string> ListViewDictionary(string nameFile)
        {
            XDocument doc = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/Report/view.xml"));
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var item in doc.Descendants("item").Where(n => n.Attribute("Relation") != null).Where(n => n.Attribute("Relation").Value.Contains(nameFile)))
            {
                dict.Add(item.Attribute("key").Value, item.Value);
            }
            return dict;
        }
        private string FillCellExcel(string value, DateTime from, DateTime until, Dictionary<string, string> listView, Excel.Range range)
        {
            if (value.ToLower().Contains("$from"))
            {
                value = value.Replace("<", "").Replace(">", "");
                var parameter = value.Split(new[] { ':' });
                if (parameter.Length == 2)
                    return from.ToString(parameter[1]);
                else
                    return from.ToString();
            }
            else if (value.ToLower().Contains("$until"))
            {
                value = value.Replace("<", "").Replace(">", "");
                var parameter = value.Split(new[] { ':' });
                if (parameter.Length == 2)
                    return until.ToString(parameter[1]);
                else
                    return until.ToString();
            }
            else if (value.ToLower().Contains("$view"))
            {
                value = value.Replace("<", "").Replace(">", "").Replace("$", "");
                var parameter = value.Split(new[] { ';' });
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                foreach (var item in parameter)
                {
                    var result = item.Split(new[] { ':' });
                    dictionary.Add(result[0].ToLower(), result[1]);
                }
                return ReadDataFromSql(dictionary, listView, range);
            }
            return "";
        }

        private string ReadDataFromSql(Dictionary<string, string> dictionary, Dictionary<string, string> listView, Excel.Range range)
        {

            using (SqlConnection conn = new SqlConnection(_connection))
            {
                try
                {
                    conn.Open();
                    string sql = listView[dictionary["view"]];
                    string s = sql;
                    string keyWord = "{0}";
                    string textInsert = "";
                    if (dictionary.TryGetValue("where", out textInsert))
                        textInsert = dictionary["where"];
                    int indexCellWhere = textInsert.IndexOf("Cell", 0);
                    if (indexCellWhere != -1)
                    {
                        int indexSikuTutup = textInsert.IndexOf("]", 0);
                        string getCell = textInsert.Substring(indexCellWhere + 4, indexSikuTutup - (indexCellWhere + 3));
                        getCell = getCell.Replace("[", "").Replace("]", "");
                        string cut = textInsert.Substring(indexCellWhere, indexSikuTutup - indexCellWhere + 1);
                        string[] cells = getCell.Split(new char[] { ',' });
                        textInsert = textInsert.Replace(cut, (range.Cells[Convert.ToInt16(cells[0]), Convert.ToInt16(cells[1])] as Excel.Range).Value2.ToString());
                    }
                    int index = s.IndexOf(keyWord, 0);
                    string subString = s.Substring(index + keyWord.Length).Trim();
                    if (subString.Contains("=") || subString.Contains(">") | subString.Contains("<"))
                        textInsert += " AND ";
                    string result = s.Insert(index + keyWord.Length, textInsert);
                    result = result.Replace("{0}", "");
                    SqlCommand command = new SqlCommand(result, conn);
                    var data = command.ExecuteReader();
                    while (data.Read())
                    {
                        try
                        {
                            string select;
                            if (dictionary.TryGetValue("select", out select))
                                return data[select].ToString();
                            return data[0].ToString();
                        }
                        catch (Exception)
                        {
                            return "";
                        }

                    }
                }
                finally
                {
                    conn.Close();

                }

                return "";
            }

        }
        public void ProcessRequest(HttpContext context)
        {

            string nameFile = context.Request.RawUrl.Split(new[] { '/' })[context.Request.RawUrl.Split(new[] { '/' }).Length - 1];
            Excel.Application excelApp = new Excel.Application();
            if (context.Request.QueryString["from"] != null && context.Request.QueryString["until"] != null)
            {
                string s = PointService.GetServer() + PointService.GetServerPath();
                _connection = new PointService().GetSetting("adoConnectionSIMKES");
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime from = DateTime.ParseExact(context.Request.QueryString["from"], "dd-MM-yyyy", provider);
                DateTime until = DateTime.ParseExact(context.Request.QueryString["until"], "dd-MM-yyyy", provider);
                string filePath = HttpContext.Current.Server.MapPath("~/File/" + nameFile);
                string fileExcelDuplicate = Guid.NewGuid().ToString().Replace("-", "_");
                File.Copy(filePath, HttpContext.Current.Server.MapPath("~/File/" + fileExcelDuplicate + ".xlsx"));
                fileExcelDuplicate = HttpContext.Current.Server.MapPath("~/File/" + fileExcelDuplicate + ".xlsx");
                Excel.Workbook excelWorkBook = excelApp.Workbooks.Open(filePath, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, Type.Missing, Type.Missing);
                try
                {
                    var listView = ListViewDictionary(nameFile);
                    Excel.Sheets excelSheets = excelWorkBook.Worksheets;
                    Excel.Worksheet excelSheet = (Excel.Worksheet)excelSheets.get_Item(1); // open Sheet1.
                    var ShtRange = excelSheet.UsedRange;
                    string fileTemplate = HttpContext.Current.Server.MapPath("~/App_Data/Report/TemplateExcel.xml");
                    XElement doc = XElement.Load(fileTemplate);
                    var parentElement = doc.Descendants("file").FirstOrDefault(n => n.Attribute("name").Value.Equals(nameFile));
                    foreach (var item in parentElement.Elements())
                    {
                        (ShtRange.Cells[Convert.ToInt16(item.Attribute("Y").Value), Convert.ToInt16(item.Attribute("X").Value)] as Excel.Range).Value2 = FillCellExcel((ShtRange.Cells[Convert.ToInt16(item.Attribute("Y").Value), Convert.ToInt16(item.Attribute("X").Value)] as Excel.Range).Value2.ToString(), from, until, listView, ShtRange);
                    }
                    excelWorkBook.Save();
                }
                catch (Exception ex)
                {
                    string error = ex.ToString();
                }
                finally
                {
                    if (excelWorkBook != null)
                    {
                        //excelWorkBook.Close(false, Type.Missing, Type.Missing);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkBook);


                        excelWorkBook = null;
                    }
                    if (excelApp != null)
                    {
                        //excelApp.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                        excelApp = null;
                    }
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                try
                {
                    File.ReadAllBytes(fileExcelDuplicate);
                }
                finally
                {
                    File.Delete(fileExcelDuplicate);

                }
            }
            else
            {
                string filePath = HttpContext.Current.Server.MapPath("~/File/" + nameFile);
                Excel.Workbook excelWorkBook = excelApp.Workbooks.Open(filePath, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, Type.Missing, Type.Missing);
                try
                {

                    Excel.Sheets excelSheets = excelWorkBook.Worksheets;
                    Excel.Worksheet excelSheet = (Excel.Worksheet)excelSheets.get_Item(1); // open Sheet1.
                    var ShtRange = excelSheet.UsedRange;
                    int rowCount = ShtRange.Rows.Count, columnCount = ShtRange.Columns.Count;
                    StringBuilder str = new StringBuilder();
                    str.Append("[");
                    for (int Rnum = 1; Rnum <= rowCount; Rnum++)
                    {
                        //Reading Each Column value From sheet to datatable Colunms                  
                        for (int Cnum = 1; Cnum <= columnCount; Cnum++)
                        {
                            ExcelItem item = new ExcelItem();
                            item.IndexX = Cnum;
                            item.IndexY = Rnum;
                            item.RowSpan = 1;
                            item.ColumnSpan = 1;
                            item.FontSize = 11;
                            int msg = 0;
                            try
                            {
                                //str.Append("[X1" + Rnum + "," + Cnum + "]");
                                var cell = (ShtRange.Cells[Rnum, Cnum] as Excel.Range);
                                if (cell != null)
                                    if ((ShtRange.Cells[Rnum, Cnum] as Excel.Range).Value2 != null)
                                    {
                                        msg = 1;
                                        //str.Append("[X2" + Rnum + "," + Cnum + "]");
                                        // (double)(ShtRange.Cells[Rnum, Cnum] as Excel.Range).Font.Size;
                                        if ((ShtRange.Cells[Rnum, Cnum] as Excel.Range).Value2 != null)
                                            if ((ShtRange.Cells[Rnum, Cnum] as Excel.Range).Value2.ToString() != string.Empty)
                                            {
                                                //str.Append("[X3" + Rnum + "," + Cnum + "]");
                                                msg = 2;
                                                item.TextValue = (ShtRange.Cells[Rnum, Cnum] as Excel.Range).Value.ToString();
                                                item.FontSize = (double)(ShtRange.Cells[Rnum, Cnum] as Excel.Range).Font.Size;
                                                msg = 3;
                                                if (cell.MergeCells.Equals(true))
                                                {
                                                    msg = 4;
                                                    //str.Append("[X4" + Rnum + "," + Cnum + "]");
                                                    item.RowSpan = cell.MergeArea.Rows.Count;
                                                    item.ColumnSpan = cell.MergeArea.Columns.Count;
                                                    str.Append("{X=\"" + item.IndexX + "\",Y=\"" + item.IndexY + "\",Text=\"" + item.TextValue + "\",RowSpan=\"" + item.RowSpan + "\",ColSpan=\"" + item.ColumnSpan + "\"}");
                                                }
                                                else
                                                {
                                                    msg = 5;
                                                    //str.Append("[X5" + Rnum + "," + Cnum + "]");
                                                    str.Append("{X=\"" + item.IndexX + "\",Y=\"" + item.IndexY + "\",Text=\"" + item.TextValue + "\"}");
                                                    item.RowSpan = 1;
                                                    item.ColumnSpan = 1;
                                                }
                                            }

                                    }
                                    else
                                        item.TextValue = "";

                            }
                            catch (Exception ex)
                            {
                                str.Append(ex.Message);
                                excelSheets = excelWorkBook.Worksheets;
                                excelSheet = (Excel.Worksheet)excelSheets.get_Item(1); // open Sheet1.
                                ShtRange = excelSheet.UsedRange;
                                //item.TextValue = ex.Message;
                                //excelWorkBook = excelApp.Workbooks.Open(filePath, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, Type.Missing, Type.Missing);
                                //excelSheets = excelWorkBook.Worksheets;
                                //excelSheet = (Excel.Worksheet)excelSheets.get_Item(1); // open Sheet1.
                                //ShtRange = excelSheet.UsedRange;
                            }
                            //listExcel.Add(item);
                        }
                    }
                    str.Remove(str.Length - 1, 1);
                    str.Append("]");
                    context.Response.Write(str.ToString());
                }
                catch (Exception ex)
                {
                    context.Response.Write(ex.Message);
                }
                finally
                {
                    if (excelWorkBook != null)
                    {
                       // excelWorkBook.Close(false, Type.Missing, Type.Missing);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkBook);
                        excelWorkBook = null;
                    }
                    if (excelApp != null)
                    {
                        excelApp.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                        excelApp = null;
                    }
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

            }
        }
    }
    public class ExcelItem
    {
        public string TextValue { get; set; }
        public int IndexX { get; set; }
        public int IndexY { get; set; }

        public double FontSize { get; set; }
        public int RowSpan { get; set; }
        public int ColumnSpan { get; set; }
    }
}
