using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml.Linq;
using Medifirst.QueryBuilder;
using Medifirst.QueryBuilder.Impl;
using Medifirst.Setting;
using DbContext;

namespace Jasamedika.Medifirst.Web.API
{
    public class Setting : IHttpHandler
    {
        public static object obj = new object();
        public static ISetting setting;
        public static ISettingGroup settingGroup;
        public static string Path { get; set; }
        #region IHttpHandler Members

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            lock (obj)
            {
                context.Response.Clear();
                try
                {
                    if (context.Request.QueryString["save"] != null)
                    {
                        Save(context);
                    }
                    else if (context.Request.QueryString["TabelDefault"] != null)
                    {
                        TabelDefault(context);
                    }
                    else if (context.Request.QueryString["dns"] != null)
                    {
                        DnsName(context);
                    }
                    else if (context.Request.QueryString["update"] != null)
                    {
                        Update(context);
                    }
                    else if (context.Request.QueryString["get"] != null)
                    {
                        GetKey(context);
                    }
                    else if (context.Request.QueryString["msg"] != null)
                    {
                        GetMessage(context);
                    }
                    else if (context.Request.QueryString["Message"] != null)
                    {
                        Message(context);
                    }
                    else if (context.Request.QueryString["HakAkses"] != null)
                    {
                        HakAkses(context);
                    }
                    else if (context.Request.QueryString["Log"] != null)
                    {
                        LogError(context);
                    }
                    else if (context.Request.QueryString["Bahasa"] != null)
                    {
                        Bahasa(context);
                    }
                    else if (context.Request.QueryString["Error"] != null)
                    {
                        Error(context);
                    }
                    ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
                    {
                        Path = context.Server.MapPath("~/App_Data/Setting.xml");
                        setting = new SettingXml(Path, SettingXml.TypeFile.PathFile);
                    }));
                }
                catch (System.Exception ex)
                {
                    Path = null;
                    context.Response.Write(ex);
                }

            }
        }

        private void TabelDefault(HttpContext context)
        {
            DbContext.Connection.ConnectionString = Connection.Conn;
            var tabelDefault = context.Request.QueryString["TabelDefault"];
            string pathFile = "";
            string[] parameter = null;
            if (context.Request.QueryString["p"] != null)
                parameter = context.Request.QueryString["p"].Split(new[] { ',' });
            if (context.Request.QueryString["PathFile"] != null)
                pathFile = context.Request.QueryString["PathFile"] + ".Template";
            else
                pathFile = tabelDefault + ".Template";
            XDocument doc = XDocument.Load(context.Server.MapPath("~/App_Data/" + pathFile));
            var data = doc.Descendants("Item");
            var asm = Assembly.LoadFile(context.Request.MapPath("~/bin/Jasamedika.Medifirst.Domain.dll"));
            var type = asm.GetTypes().FirstOrDefault(n => n.Name.Equals(tabelDefault));
            var listSave = new List<object>();
            foreach (var item in data)
            {
                var obj = Activator.CreateInstance(type);
                foreach (var subItem in item.Descendants())
                {
                    var name = subItem.Name.LocalName;
                    if (!string.IsNullOrEmpty(subItem.Value))
                    {
                        string value = subItem.Value;
                        if (parameter != null)
                            for (int i = 0; i < parameter.Length; i++)
                            {
                                value = value.Replace("{" + i.ToString() + "}", parameter[i]);
                            }
                        switch (obj.GetType().GetProperty(name).PropertyType.ToString().ToLower())
                        {
                            case "system.nullable`1[system.int32]":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToInt32(value), null);
                                break;
                            case "system.nullable`1[system.int16]":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToInt16(value), null);
                                break;
                            case "system.nullable`1[system.int64]":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToInt64(value), null);
                                break;
                            case "system.nullable`1[system.byte]":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToByte(value), null);

                                break;
                            case "system.int32":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToInt32(value), null);
                                break;
                            case "system.int16":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToInt32(value), null);
                                break;
                            case "system.int64":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToInt64(value), null);
                                break;
                            case "system.byte":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToByte(value), null);
                                break;
                            case "system.string":
                                obj.GetType().GetProperty(name).SetValue(obj, value.ToString().Trim(), null);
                                break;
                            case "system.nullable`1[system.bool]":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToBoolean(value), null);
                                break;
                            case "system.nullable`1[system.datetime]":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToDateTime(value), null);

                                break;
                            case "system.nullable`1[system.double]":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToDouble(value), null);
                                break;
                            case "system.nullable`1[system.char]":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToChar(value), null);

                                break;
                            case "system.nullable`1[system.decimal]":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToDecimal(value), null);
                                break;
                            case "system.bool":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToBoolean(value), null);
                                break;
                            case "system.datetime":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToDateTime(value), null);
                                break;
                            case "system.double":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToDouble(value), null);
                                break;
                            case "system.char":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToChar(value), null);
                                break;
                            case "system.decimal":
                                obj.GetType().GetProperty(name).SetValue(obj, Convert.ToDecimal(value), null);
                                break;
                            default:
                                break;
                        }
                        
                    }
                }
                var dataUpdate = this.GetOneDataObject(obj);
                if (dataUpdate == null)
                    listSave.Add(obj);
            }
            context.Response.Write(listSave.Save().ToHasil());
        }

        private void Error(HttpContext context)
        {
            var Namefile = context.Request.QueryString["Error"];
            List<string> listTitleHasAdd = new List<string>();
            XElement output = new XElement("Erros");

            if (File.Exists(context.Server.MapPath("~/App_Data/" + Namefile + ".xml")))
            {
                XDocument doc = XDocument.Load(context.Server.MapPath("~/App_Data/" + Namefile + ".xml"));
                var data = doc.Descendants("item");
                foreach (var subItem in data)
                {
                    if (listTitleHasAdd.Where(n => n.Equals(subItem.Attribute("Key").Value)).Count() == 0)
                    {
                        output.Add(subItem);
                        listTitleHasAdd.Add(subItem.Attribute("Key").Value);
                    }
                }

            }
            context.Response.Write(output.ToString());
        }

        private void Bahasa(HttpContext context)
        {
            var listFile = context.Request.QueryString["Bahasa"].Split(new[] { ',' });
            listFile.ToList().ForEach(n => n.Trim());
            List<string> listTitleHasAdd = new List<string>();
            XElement output = new XElement("Bahasa");
            foreach (var item in listFile)
            {
                if (File.Exists(context.Server.MapPath("~/Bahasa/" + item + ".xml")))
                {
                    XDocument doc = XDocument.Load(context.Server.MapPath("~/Bahasa/" + item + ".xml"));
                    var data = doc.Descendants("Item");
                    foreach (var subItem in data)
                    {
                        if (listTitleHasAdd.Where(n => n.Equals(subItem.Attribute("LabelID").Value)).Count() == 0)
                        {
                            output.Add(subItem);
                            listTitleHasAdd.Add(subItem.Attribute("LabelID").Value);
                        }
                    }
                }
            }
            context.Response.Write(output.ToString());
        }

        private void DnsName(HttpContext context)
        {
            try
            {
                context.Response.Write(Dns.GetHostByAddress(IPAddress.Parse(context.Request.QueryString["dns"])).HostName.Split(new[] { '.', ' ' })[0]);
            }
            catch (Exception)
            {

                context.Response.Write(context.Request.QueryString["dns"]);
            }

        }

        private void LogError(HttpContext context)
        {
            lock (context)
            {
                if (!Directory.Exists(context.Server.MapPath("~/Log")))
                    Directory.CreateDirectory(context.Server.MapPath("~/Log"));
                string path = context.Server.MapPath("~/Log/" + DateTime.Now.ToString("dd-MM-yyyy") + ".xml");
                if (!File.Exists(path))
                {
                    XDocument doc = new XDocument();
                    XElement xe = new XElement("Logging");
                    doc.Add(xe);
                    doc.Save(path);
                }
                var message = XDocument.Load(path);
                XDocument _doc = XDocument.Load(path);
                var data = _doc.Descendants("Logging").FirstOrDefault();
                data.Add(new XElement("Log", new XElement("Message", context.Request.QueryString["Log"])
                    , new XElement("Date", DateTime.Now)
                    , new XElement("Host", context.Request.Params["REMOTE_ADDR"])));
                _doc.Save(path);
                path = context.Server.MapPath("~/App_Data/ListError.xml");
                _doc = XDocument.Load(path);
                var listError = _doc.Descendants("item");
                string value = "";
                foreach (var item in listError)
                {
                    int count = item.Attribute("Key").Value.Split(new char[] { ',' }).Count();
                    int hasCount = 0;
                    foreach (var subItem in item.Attribute("Key").Value.Split(new char[] { ',' }))
                    {
                        if (context.Request.QueryString["Log"].ToLower().Contains(subItem.ToLower()))
                        {
                            value = item.Attribute("Value").Value;
                            hasCount++;
                        }
                    }
                    if (hasCount == count)
                        break;

                }
                context.Response.Write(value);

            }
        }

        private void HakAkses(HttpContext context)
        {
            using (SqlConnection conn = new SqlConnection(Connection.Conn))
            {
                try
                {
                    string query = context.Request.QueryString["HakAkses"];
                    IQueryBuilder queryLoginUser = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);

                    queryLoginUser.SelectTable("LoginUser_S");
                    queryLoginUser.AddField("LoginUser_S.KdKelompokUser");
                    queryLoginUser.AddCriteria(queryLoginUser.Criteria.Equal("LoginUser_S.KdPegawai", query));
                    var sql = queryLoginUser.BuildQuery();
                    conn.Open();
                    var comd = new SqlCommand(sql, conn);
                    var execute = comd.ExecuteReader();
                    while (execute.Read())
                    {
                        IQueryBuilder queryMapObjekModulToKelompokUser = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);

                        queryMapObjekModulToKelompokUser.SelectTable("MapObjekModulToKelompokUser_S");
                        queryMapObjekModulToKelompokUser.AddField("MapObjekModulToKelompokUser_S.KdObjekModulAplikasi");
                        queryMapObjekModulToKelompokUser.AddCriteria(queryMapObjekModulToKelompokUser.Criteria.Equal("MapObjekModulToKelompokUser_S.KdKelompokUser", execute[0]));
                        sql = queryMapObjekModulToKelompokUser.BuildQuery();
                        conn.Close();
                        conn.Open();
                        SqlCommand comnd = new SqlCommand(sql, conn);
                        var read = comnd.ExecuteReader();
                        StringBuilder hasil = new StringBuilder();
                        while (read.Read())
                        {
                            hasil.Append(read[0].ToString() + ",");
                        }

                        context.Response.Write(hasil.ToString());
                        break;
                    }

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

        private void Message(HttpContext context)
        {
            var message = XDocument.Load(context.Server.MapPath("~/App_Data/Message.xml"));            
            context.Response.Write(message.Descendants("item").FirstOrDefault(n => n.Attribute("key").Value.ToLower().Equals(context.Request.QueryString["Message"].ToString().ToLower())).Attribute("value").Value);
        }

        private void GetMessage(HttpContext context)
        {
            FirstLoad(context);
            string msg = "";
            foreach (var item in setting.GetAllKey().Where(n => n.StartsWith("msg")))
            {
                msg += "[" + item + "," + setting.GetValue(item) + "]-";
            }
            msg = msg.Substring(0, msg.Length - 1);
            context.Response.Write(msg);
        }

        private void GetKey(HttpContext context)
        {
            FirstLoad(context);
            if (context.Request.QueryString["q"] != null)
                context.Response.Write(settingGroup.GetValue(context.Request.QueryString["get"], context.Request.QueryString["q"]));
            else
                context.Response.Write(setting.GetValue(context.Request.QueryString["get"]));

        }

        private void Update(HttpContext context)
        {
            FirstLoad(context);
            string[] temp = context.Request.QueryString["update"].Split(new char[] { '/' });
            context.Response.Write(setting.Update(temp[0], temp[1]));
        }

        private void FirstLoad(HttpContext context)
        {
            if (Path == string.Empty || Path == null)
            {
                Path = context.Server.MapPath("~/App_Data/Setting.xml");
                setting = new SettingXml(Path, SettingXml.TypeFile.PathFile);
                settingGroup = new SettingXml(Path, SettingXml.TypeFile.PathFile);
            }
        }

        private void Save(HttpContext context)
        {
            FirstLoad(context);
            string[] temp = context.Request.QueryString["save"].Split(new char[] { '/' });
            context.Response.Write(setting.Save(temp[0], temp[1]));
        }

        #endregion
    }
}
