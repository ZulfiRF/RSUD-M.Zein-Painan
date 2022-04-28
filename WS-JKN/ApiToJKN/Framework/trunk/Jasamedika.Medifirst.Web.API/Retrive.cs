using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Medifirst.PointOfService.Impl;

namespace Jasamedika.Medifirst.Web.API
{
    public class Retrive : IHttpHandler
    {
        public static Dictionary<string, string> ListView = new Dictionary<string, string>();

        #region IHttpHandler Members

        public virtual bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public virtual void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            XDocument doc;
            try
            {
                if (context.Request.QueryString["xml"] != null)
                {
                    doc = XDocument.Load(context.Server.MapPath("~/App_Data/log.xml"));
                    var data = doc.Descendants("item").Where(n => n.Attribute("key").Value.Contains(context.Request.Params["REMOTE_ADDR"].ToString()));
                    foreach (var item in data)
                    {
                        context.Response.Write(item.Attribute("value").Value + ";");
                    }
                    data.ToList().ForEach(b => b.Remove());
                    doc.Save(context.Server.MapPath("~/App_Data/log.xml"));
                }
                else if (context.Request.QueryString["conn"] != null)
                {
                    context.Response.Write(Connection.Conn);
                    context.Response.Write("<br/>");
                    context.Response.Write(PointService.GetServerPath() + "<br/>net.tcp://" + PointService.GetServer() + ":8020/ServiceExchange");
                }
                else if (context.Request.QueryString["Report"] != null)
                {
                    Report(context);
                }
                else if (context.Request.QueryString["c"] != null)
                {
                    context.Response.Write(context.Request.Headers);
                }
                else if (context.Request.QueryString["CekNoCM"] != null)
                {
                    CekNoCM(context);
                }
                else if (context.Request.QueryString["moduleName"] != null)
                {
                    ModuleName(context);
                }
                else if (context.Request.QueryString["IPAddress"] != null)
                {
                    context.Response.Write(context.Request.Params["REMOTE_ADDR"]);
                }
                else if (context.Request.QueryString["view"] != null)
                {
                    ViewQuery(context);
                }
                else
                {
                    Retrives(context);
                }
            }
            catch (Exception ex)
            {
                context.Response.Write(ex);
            }
        }

        private void Report(HttpContext context)
        {
            XElement doc = XElement.Load(context.Request.MapPath("~/App_Data/Report/" + context.Request.QueryString["Report"] + ".xml"));
            context.Response.Write(doc.ToString(SaveOptions.DisableFormatting));
        }

        private void Message(HttpContext context)
        {
        }

        private void ModuleName(HttpContext context)
        {
            using (SqlConnection conn = new SqlConnection(Connection.Conn))
            {
                try
                {
                    conn.Open();
                    if (context.Request.QueryString["KdProfile"] != null)
                    {
                        SqlCommand comnd = new SqlCommand("SELECT     ReportDisplay FROM         ModulAplikasi_S WHERE     (KdModulAplikasi = '" + context.Request.QueryString["moduleName"].ToString() + "') AND KdProfile='" + context.Request.QueryString["KdProfile"].ToString() + "'", conn);
                        var read = comnd.ExecuteReader();
                        while (read.Read())
                        {
                            context.Response.Write(read[0]);
                        }
                    }
                    else
                    {
                        SqlCommand comnd = new SqlCommand("SELECT     ReportDisplay FROM         ModulAplikasi_S WHERE     (KdModulAplikasi = '" + context.Request.QueryString["moduleName"].ToString() + "')", conn);
                        var read = comnd.ExecuteReader();
                        while (read.Read())
                        {
                            context.Response.Write(read[0]);
                        }
                    }
                }
                catch (Exception)
                {
                    context.Response.Write("");
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void Retrives(HttpContext context)
        {
            string temp = context.Request.RawUrl.Split(new char[] { '?' })[1];
            string[] tempArr = temp.Split(new char[] { '*' })[0].Split(new char[] { '/' });
            string table = tempArr[0];
            string where = "";
            string top = "";
            string select = "";
            for (int i = 1; i < tempArr.Length; i += 2)
            {
                if (tempArr[i].ToLower().Contains("select"))
                {
                    select = tempArr[i + 1];
                }
                else
                    if (tempArr[i].ToLower().Contains("top"))
                    {
                        top = tempArr[i + 1];
                    }
                    else
                    {
                        string field = tempArr[i];
                        string value = tempArr[i + 1];

                        //context.Response.Write();
                        if (where == string.Empty)
                        {
                            value = value.Replace('|', '%');
                            if (value.IndexOf('%') != -1)
                            {
                                where += field + " like '" + HttpUtility.HtmlDecode(value) + "'";
                            }
                            else
                            {
                                where += field + " = '" + HttpUtility.HtmlDecode(value) + "'";
                            }
                        }
                        else
                        {
                            switch (field[0])
                            {
                                case '|':
                                    if (value.IndexOf('%') != -1)
                                    {
                                        where += " OR " + field.Substring(1) + " like '" + HttpUtility.HtmlDecode(value) + "'";
                                    }
                                    else
                                    {
                                        where += " OR " + field.Substring(1) + " = '" + HttpUtility.HtmlDecode(value) + "'";
                                    }
                                    break;

                                default:
                                    if (value.IndexOf('%') != -1)
                                    {
                                        where += " AND " + field.Substring(1) + " like '" + HttpUtility.HtmlDecode(value) + "'";
                                    }
                                    else
                                    {
                                        where += " AND " + field + " = '" + HttpUtility.HtmlDecode(value) + "'";
                                    }
                                    break;
                            }
                        }
                    }
            }
            where = (where != string.Empty) ? " Where " + where : where;
            string sql = "";
            if (top != string.Empty)
            {
                sql = ("Select Top " + top + " * from " + table + where);
            }
            else
                sql = ("Select * from " + table + where);
            if (!string.IsNullOrEmpty(select))
            {
                sql = sql.Replace("*", select);
            }
            sql = sql.Replace("%20", " ");
            using (SqlConnection conn = new SqlConnection(Connection.Conn))
            {
                try
                {
                    //context.Response.Write(sql);
                    conn.Open();
                    SqlCommand comnd = new SqlCommand(sql, conn);
                    var read = comnd.ExecuteReader();
                    string Json = "[";
                    StringBuilder sbstr = new StringBuilder("[");

                    //int i = 0;
                    while (read.Read())
                    {
                        XElement xe = new XElement("List");
                        if (Json.Equals("["))
                            sbstr.Append("{");
                        else
                            sbstr.Append(",{");
                        Json = "";
                        for (int i = 0; i < read.FieldCount; i++)
                        {
                            string tempString = read[i].ToString();
                            if (read[i] is DateTime)
                            {
                                tempString = ((read[i] as DateTime?).HasValue) ? (read[i] as DateTime?).Value.ToBinary().ToString() : tempString;
                            }
                            if (tempString == string.Empty)
                                tempString = "null";
                            else
                                tempString = "\"" + tempString + "\"";
                            if (i == 0)
                                sbstr.Append("\"" + read.GetName(i) + "\":" + tempString.Replace(",", "^"));
                            else
                                sbstr.Append(",\"" + read.GetName(i) + "\":" + tempString.Replace(",", "^"));
                        }
                        sbstr.Append("}");
                    }
                    sbstr.Append("]");
                    context.Response.Write(sbstr);
                }
                catch (Exception ex)
                {
                    context.Response.Clear();
                    context.Response.Write(ex.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void ViewQuery(HttpContext context)
        {
            if (context.Request.QueryString["PathFile"] == null)
            {
                if (!File.Exists(context.Server.MapPath("~/App_Data/View.xml")))
                {
                    if (Directory.Exists(context.Server.MapPath("~/App_Data")))
                    {
                        Directory.CreateDirectory(context.Server.MapPath("~/App_Data"));
                    }
                    XElement _doc = new XElement("View");
                    _doc.Save(context.Server.MapPath("~/App_Data/View.xml"));
                }
            }
            else
                if (!File.Exists(context.Server.MapPath("~/App_Data" + context.Request.QueryString["PathFile"] + "/View.xml")))
                {
                    if (Directory.Exists(context.Server.MapPath("~/App_Data" + context.Request.QueryString["PathFile"])))
                    {
                        Directory.CreateDirectory(context.Server.MapPath("~/App_Data" + context.Request.QueryString["PathFile"]));
                    }
                    XElement _doc = new XElement("View");
                    _doc.Save(context.Server.MapPath("~/App_Data" + context.Request.QueryString["PathFile"] + "/View.xml"));
                }
            XDocument doc;
            if (context.Request.QueryString["PathFile"] == null)
                doc = XDocument.Load(context.Server.MapPath("~/App_Data/View.xml"));
            else
                doc = XDocument.Load(context.Server.MapPath("~/App_Data" + context.Request.QueryString["PathFile"] + "/View.xml"));
            var data = doc.Descendants("item").FirstOrDefault(n => n.Attribute("key").Value.ToLower().Equals(context.Request.QueryString["view"].ToLower()));
            if (data != null)
            {
                using (SqlConnection conn = new SqlConnection(Connection.Conn))
                {
                    SqlDataReader read = null;
                    try
                    {
                        //

                        string sql = "";
                        sql = data.Value;
                        if (context.Request.QueryString["a"] != null)
                        {
                            sql = sql.Replace("{0}", (context.Request.QueryString["a"] != null) ? " " + HttpUtility.HtmlDecode(context.Request.QueryString["a"]) : "");
                        }
                        else
                        {
                            sql = sql.Replace("{0}", (context.Request.QueryString["w"] != null) ? " AND " + HttpUtility.HtmlDecode(context.Request.QueryString["w"]) : "");
                        }

                        if (context.Request.QueryString["q"] != null)
                        {
                            sql = sql.Replace("{1}", (context.Request.QueryString["q"] != null) ? " " + HttpUtility.HtmlDecode(context.Request.QueryString["q"]) : "");
                        }
                        else
                        {
                            sql = sql.Replace("{1}", "");
                        }

                        sql = sql.Replace("|", "%");
                        sql = sql.Replace("!=", "<>");
                        if (context.Request.QueryString["top"] != null)
                            sql = sql.Replace("{2}", " Top " + context.Request.QueryString["top"]);
                        else
                            sql = sql.Replace("{2}", "");

                        // context.Response.Write(sql);
                        conn.Open();
                        if (context.Request.QueryString["print"] != null)
                            context.Response.Write(sql);
                        SqlCommand comnd = new SqlCommand(sql, conn);

                        //  comnd.CommandType = System.Data.CommandType.TableDirect;
                        read = comnd.ExecuteReader();
                        string Json = "[";
                        StringBuilder sbstr = new StringBuilder("[");

                        //int i = 0;
                        string[] select = null;
                        if (context.Request.QueryString["select"] != null)
                            select = context.Request.QueryString["select"].Split(new char[] { ',' });
                        while (read.Read())
                        {
                            XElement xe = new XElement("List");
                            if (Json.Equals("["))
                                sbstr.Append("{");
                            else
                                sbstr.Append(",{");
                            Json = "";
                            for (int i = 0; i < read.FieldCount; i++)
                            {
                                string tempString = read[i].ToString();
                                if (read[i] is DateTime)
                                {
                                    tempString = ((read[i] as DateTime?).HasValue) ? (read[i] as DateTime?).Value.ToBinary().ToString() : tempString;
                                }
                                if (tempString == string.Empty)
                                    tempString = "null";
                                else
                                    tempString = "\"" + tempString + "\"";
                                if (select == null)
                                {
                                    if (i == 0)
                                        sbstr.Append("\"" + read.GetName(i) + "\":" + tempString.Replace(",", "^"));
                                    else
                                        sbstr.Append(",\"" + read.GetName(i) + "\":" + tempString.Replace(",", "^"));
                                }
                                else
                                {
                                    var getName = read.GetName(i);
                                    if (select.Where(n => n.ToLower().Equals(read.GetName(i).ToLower())).Count() != 0)
                                        if (i == 0)
                                            sbstr.Append("\"" + read.GetName(i) + "\":" + tempString.Replace(",", "^"));
                                        else
                                            sbstr.Append(",\"" + read.GetName(i) + "\":" + tempString.Replace(",", "^"));
                                }
                            }
                            sbstr.Append("}");
                        }
                        sbstr.Append("]");
                        context.Response.Write(sbstr);
                    }
                    catch (Exception ex)
                    {
                        context.Response.Clear();
                        context.Response.Write(ex.ToString());
                    }
                    finally
                    {
                        read.Close();
                        conn.Close();
                    }
                }
            }
        }

        private void CekNoCM(HttpContext context)
        {
            using (SqlConnection conn = new SqlConnection(Connection.Conn))
            {
                try
                {
                    conn.Open();
                    SqlCommand comnd = new SqlCommand("SELECT     Count(*) FROM         Pasien_M WHERE     (NoCM = '" + context.Request.QueryString["CekNoCM"].ToString() + "')", conn);
                    var read = comnd.ExecuteReader();
                    while (read.Read())
                    {
                        context.Response.Write(read[0]);
                    }
                }
                catch (Exception)
                {
                    context.Response.Write("");
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        #endregion IHttpHandler Members
    }
}