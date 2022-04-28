using System;
using System.Data.SqlClient;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using Medifirst.Exchange.ServiceExchange;
using Medifirst.PointOfService.Impl;
using Medifirst.QueryBuilder;
using Medifirst.QueryBuilder.Impl;
using Medifirst.Security;
using Medifirst.Exchange;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Jasamedika.Medifirst.Web.API
{
    public class LoginAuth : IHttpHandler, IRequiresSessionState
    {
        public static object obj = new object();
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
            context.Response.Clear();
            if (context.Request.QueryString["current"] != null)
            {
                context.Response.Write(CurrentSubModuleNameClose);
            }
            else if (context.Request.QueryString["Securitylogin"] != null)
            {
                Securitylogin(context);
            }
            else if (context.Request.QueryString["login"] != null)
            {
                Login(context);
            }
            else if (context.Request.QueryString["updateUser"] != null)
            {
                Update(context);
            }
            else if (context.Request.QueryString["logout"] != null)
            {
                Logout(context);
            }
            else if (context.Request.QueryString["open"] != null)
            {
                OpenModule(context);
            }
            else if (context.Request.QueryString["Remember"] != null)
            {
                Remember(context);
            }
            else if (context.Request.QueryString["GetRemember"] != null)
            {
                GetRemember(context);
            }
            else if (context.Request.QueryString["close"] != null)
            {
                CloseModule(context);
            }
            else if (context.Request.QueryString["KdHistoryLogin"] != null)
            {
                string[] query = context.Request.QueryString["KdHistoryLogin"].Split(new char[] { '/' });
                context.Response.Write(GetKdHistoryLogin(query[0], query[1], query[2]));
            }
            else if (context.Request.RawUrl.ToLower().Contains("ceklogin"))
            {
                try
                {
                    if (context.Session["Login"] != null)
                    {
                        context.Response.Write(true);
                    }
                    else
                    {
                        context.Response.Write(false);
                    }
                }
                catch (Exception)
                {

                }

            }
            else
            {
                HttpContext.Current.Session["b"] = "1111";

            }


        }

        private void GetRemember(HttpContext context)
        {
            {
                if (!Directory.Exists(context.Server.MapPath("~/App_Data")))
                    Directory.CreateDirectory(context.Server.MapPath("~/Log"));
                string path = context.Server.MapPath("~/App_Data/Remember.xml");
             
                var message = XDocument.Load(path);
                XDocument _doc = XDocument.Load(path);
                var data = _doc.Descendants("Remember").FirstOrDefault();
                if (data != null)
                {
                    var item = data.Descendants("Item").FirstOrDefault(n => n.Element("UserAgent").Value.Equals(context.Request.UserAgent) && n.Element("IP").Value.Equals(context.Request.Params["REMOTE_ADDR"]));
                    if (item != null)
                        context.Response.Write(item.Element("Login").Value);
                }

                //   context.Response.Write(value);

            }
        }

        private void Remember(HttpContext context)
        {
            //  lock (context)
            {
                if (!Directory.Exists(context.Server.MapPath("~/App_Data")))
                    Directory.CreateDirectory(context.Server.MapPath("~/Log"));
                string path = context.Server.MapPath("~/App_Data/Remember.xml");
                if (!File.Exists(path))
                {
                    XDocument doc = new XDocument();
                    XElement xe = new XElement("Remember");
                    doc.Add(xe);
                    doc.Save(path);
                }
                var message = XDocument.Load(path);
                XDocument _doc = XDocument.Load(path);
                var data = _doc.Descendants("Remember").FirstOrDefault();
                if (data != null)
                {
                    var item = data.Descendants("Item").FirstOrDefault(n => n.Element("UserAgent").Value.Equals(context.Request.UserAgent) && n.Element("IP").Value.Equals(context.Request.Params["REMOTE_ADDR"]));
                    if (item == null)
                        data.Add(new XElement("Item", new XElement("UserAgent", context.Request.UserAgent)
                            , new XElement("IP", context.Request.Params["REMOTE_ADDR"]), new XElement("Login", context.Request.QueryString["Remember"])));
                    else
                        item.Element("Login").Value = context.Request.QueryString["Remember"];
                    _doc.Save(path);
                }

                //   context.Response.Write(value);

            }
        }

        private void Securitylogin(HttpContext context)
        {
            //  lock (context)
            try
            {
                bool valUsername = false;
                bool valModul = false;
                bool valRuangan = false;
                bool valAdmin = false;
                //bool validated = false;
                string ip = context.Request.Params["REMOTE_ADDR"];
                string result;
                string kdpegawai = "";
                string statuslogin;
                string kdkelompokuser;
                using (SqlConnection conn = new SqlConnection(Connection.Conn))
                {
                    conn.Open();
                    string[] queryString = context.Request.QueryString["Securitylogin"].Split(new char[] { '/' });
                    string namaruangan;
                    string username = queryString[0];
                    string password = queryString[1];
                    string kdruangan = queryString[2];
                    string kdprofile = queryString[3];
                    string kdmodulaplikasi = queryString[4];
                    //string namahost = "";
                    IQueryBuilder queryUsername = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);

                    queryUsername.SelectTable("loginuser_s");
                    queryUsername.AddField("kdpegawai");
                    queryUsername.AddField("convert(varchar(50), namauser)");
                    queryUsername.AddField("convert(varchar(50), katasandi)");
                    queryUsername.AddField("kdkelompokuser");
                    queryUsername.AddField("statuslogin");
                    queryUsername.AddField("statusenabled");
                    queryUsername.AddCriteria(queryUsername.Criteria.Equal("namauser", Cryptography.funcAESEncrypt(queryString[0])));
                    queryUsername.AddCriteria(queryUsername.Criteria.Equal("kdprofile", kdprofile));
                    queryUsername.AddCriteria(queryUsername.Criteria.Equal("statusenabled", 1));
                    string sql = queryUsername.BuildQuery();
                    SqlCommand comd = new SqlCommand(sql, conn);
                    var dtUsername = comd.ExecuteReader();
                    while (dtUsername.Read())
                    {
                        kdpegawai = dtUsername[0].ToString();
                        result = Cryptography.funcAESDecrypt(dtUsername[2].ToString());
                        kdkelompokuser = dtUsername[3].ToString();
                        statuslogin = dtUsername[4].ToString();

                        valUsername = result == password && statuslogin == "0" ? true : false;
                        valAdmin = kdkelompokuser == "0" ? true : false;

                        //=====

                        if (valUsername)
                        {
                            IQueryBuilder queryModul = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
                            queryModul.SelectTable("mappegawaitomodulaplikasi_s");
                            queryModul.AddField("kdmodulaplikasi");
                            queryModul.AddCriteria(queryModul.Criteria.Equal("kdmodulaplikasi", kdmodulaplikasi));
                            queryModul.AddCriteria(queryModul.Criteria.Equal("kdpegawai", kdpegawai));

                            //queryModul.AddJoin(queryModul.Join.InnerIJoin("modulaplikasi_s", "mappegawaitomodulaplikasi_s", "kdmodulaplikasi", "kdmodulaplikasi"));
                            //queryModul.AddField("mappegawaitomodulaplikasi_s.kdpegawai");
                            //queryModul.AddField("modulaplikasi_s.kdmodulaplikasi");
                            //queryModul.AddField("modulaplikasi_s.modulaplikasi");

                            //queryModul.AddCriteria(queryModul.Criteria.Equal("mappegawaitomodulaplikasi_s.kdpegawai", kdpegawai));
                            //queryModul.AddCriteria(queryModul.Criteria.Equal("upper(modulaplikasi_s.modulaplikasi)", modulaplikasi));

                            string queryRes = queryModul.BuildQuery();
                            conn.Close();
                            conn.Open();
                            SqlCommand comd1 = new SqlCommand(queryRes, conn);

                            var temo = comd1.ExecuteReader();
                            valModul = false;
                            while (temo.Read())
                            {
                                valModul = true;
                                kdmodulaplikasi = temo[0].ToString();
                            }

                        }

                        //=====

                        if (valModul)
                        {
                            IQueryBuilder queryRuangan = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
                            queryRuangan.SelectTable("maploginusertoruangan_s");
                            queryRuangan.AddJoin(queryRuangan.Join.InnerJoin("ruangan_m", "maploginusertoruangan_s", "kdruangan", "kdruangan"));
                            queryRuangan.AddField("ruangan_m.kdruangan");
                            queryRuangan.AddField("ruangan_m.namaruangan");

                            queryRuangan.AddCriteria(queryRuangan.Criteria.Equal("maploginusertoruangan_s.kdpegawai", kdpegawai));
                            queryRuangan.AddCriteria(queryRuangan.Criteria.Equal("ruangan_m.kdruangan", kdruangan));
                            conn.Close();
                            conn.Open();
                            string sqlRuangan = queryRuangan.BuildQuery();
                            SqlCommand comd2 = new SqlCommand(sqlRuangan, conn);
                            var ruanganExecute = comd2.ExecuteReader();
                            valRuangan = false;
                            while (ruanganExecute.Read())
                            {
                                valRuangan = true;
                                namaruangan = ruanganExecute[1].ToString();
                            }


                        }

                        //=====

                        if (valRuangan)
                        {
                            IQueryBuilder queryLoginUser = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
                            queryLoginUser.SelectTable("LoginUser_S");
                            queryLoginUser.AddField("LoginUser_S.KdKelompokUser");
                            queryLoginUser.AddCriteria(queryLoginUser.Criteria.Equal("LoginUser_S.KdPegawai", kdpegawai));
                            sql = queryLoginUser.BuildQuery();
                            conn.Close();
                            conn.Open();
                            comd = new SqlCommand(sql, conn);
                            var execute = comd.ExecuteReader();
                            while (execute.Read())
                            {
                                IQueryBuilder queryMapObjectModulAplikasi = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);

                                queryMapObjectModulAplikasi.SelectTable("MapObjekModulToKelompokUser_S");
                                queryMapObjectModulAplikasi.AddCriteria(queryMapObjectModulAplikasi.Criteria.Equal("MapObjekModulToKelompokUser_S.KdKelompokUser", execute[0]));
                                queryMapObjectModulAplikasi.AddCriteria(queryMapObjectModulAplikasi.Criteria.Equal("MapObjekModulToKelompokUser_S.KdObjekModulAplikasi", queryString[5]));
                                sql = queryMapObjectModulAplikasi.BuildQuery();
                                conn.Close();
                                conn.Open();
                                var comd1 = new SqlCommand(sql, conn);
                                var executeMapObjekModulAplikasi = comd1.ExecuteReader();
                                while (executeMapObjekModulAplikasi.Read())
                                {
                                    context.Response.Write("sj3889skk349klsko349");
                                }
                                break;
                            }
                        }
                        break;
                    }

                    conn.Close();
                }

            }
            catch (System.Exception ex)
            {
            }

        }
        public static string CurrentSubModuleNameClose = "";
        private void CloseModule(HttpContext context)
        {
            // lock (CurrentSubModuleNameClose)
            {


                string[] query = context.Request.QueryString["close"].Split(new char[] { '/' });
                // open=Registrasi Pelayanan/kodeHistoriLogin
                if (!CurrentSubModuleNameClose.Equals(query[2]))
                {
                    SubModulMessage subModul = new SubModulMessage();
                    subModul.IP = context.Request.Params["REMOTE_ADDR"];
                    subModul.SubModuleName = query[2];
                    CurrentSubModuleNameClose = query[2];
                    using (SqlConnection conn = new SqlConnection(Connection.Conn))
                    {
                        IQueryBuilder queryHistory = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);

                        queryHistory.SelectTable("HistoryLoginModulAplikasi_s");
                        queryHistory.AddCriteria(queryHistory.Criteria.Equal("HistoryLoginModulAplikasi_s.kdProfile", query[0]));
                        queryHistory.AddCriteria(queryHistory.Criteria.Equal("HistoryLoginModulAplikasi_s.KdHistoryLogin", query[1]));
                        queryHistory.AddJoin(queryHistory.Join.InnerJoin("Ruangan_M", "HistoryLoginModulAplikasi_s", "KdRuanganUser", "KdRuangan"));
                        queryHistory.AddJoin(queryHistory.Join.InnerJoin("ModulAplikasi_S", "HistoryLoginModulAplikasi_s", "KdModulAplikasi", "KdModulAplikasi"));
                        queryHistory.AddField("Ruangan_M.NamaRuangan");
                        queryHistory.AddField("ModulAplikasi_S.ReportDisplay");
                        var sql = queryHistory.BuildQuery();
                        conn.Open();
                        var comd = new SqlCommand(sql, conn);
                        var execute = comd.ExecuteReader();
                        while (execute.Read())
                        {
                            lock (obj)
                            {
                                string module = execute[1].ToString();
                                string ruangan = execute[0].ToString();
                                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
                                {
                                    try
                                    {
                                        Exchange msg = new Exchange(new EndpointAddress(new Uri("net.tcp://" + PointService.GetServer() + ":8020/ServiceExchange")), PointService.SetNetTCP());
                                        subModul.ModuleName = module;
                                        subModul.Ruangan = ruangan;
                                        subModul.Type = MessageType.CloseSubModul;
                                        msg.UnSubscribeModul(subModul);
                                    }
                                    catch (Exception)
                                    {


                                    }
                                }));
                            }

                        }
                    }
                }
            }
        }

        private void OpenModule(HttpContext context)
        {
            try
            {
                string[] query = context.Request.QueryString["open"].Split(new char[] { '/' });
                // open=Registrasi Pelayanan/kodeHistoriLogin
                SubModulMessage subModul = new SubModulMessage();
                //subModul.IP = "192.168.0.24";
                subModul.IP = context.Request.Params["REMOTE_ADDR"];
                CurrentSubModuleNameClose = "";
                subModul.SubModuleName = query[2];
                using (SqlConnection conn = new SqlConnection(Connection.Conn))
                {
                    try
                    {
                        IQueryBuilder queryHistory = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);

                        queryHistory.SelectTable("HistoryLoginModulAplikasi_s");
                        queryHistory.AddCriteria(queryHistory.Criteria.Equal("HistoryLoginModulAplikasi_s.kdProfile", query[0]));
                        queryHistory.AddCriteria(queryHistory.Criteria.Equal("HistoryLoginModulAplikasi_s.KdHistoryLogin", query[1]));
                        queryHistory.AddJoin(queryHistory.Join.InnerJoin("Ruangan_M", "HistoryLoginModulAplikasi_s", "KdRuanganUser", "KdRuangan"));
                        queryHistory.AddJoin(queryHistory.Join.InnerJoin("ModulAplikasi_S", "HistoryLoginModulAplikasi_s", "KdModulAplikasi", "KdModulAplikasi"));
                        queryHistory.AddField("Ruangan_M.NamaRuangan");
                        queryHistory.AddField("ModulAplikasi_S.ReportDisplay");
                        var sql = queryHistory.BuildQuery();
                        conn.Open();
                        var comd = new SqlCommand(sql, conn);
                        var execute = comd.ExecuteReader();
                        while (execute.Read())
                        {
                            lock (execute)
                            {
                                string module = execute[1].ToString();
                                string ruangan = execute[0].ToString();
                                ThreadPool.QueueUserWorkItem(new WaitCallback(
                                delegate
                                {
                                    try
                                    {
                                        Exchange msg = new Exchange(new EndpointAddress(new Uri("net.tcp://" + PointService.GetServer() + ":8020/ServiceExchange")), PointService.SetNetTCP());
                                        subModul.ModuleName = module;
                                        subModul.Ruangan = ruangan;
                                        subModul.Type = MessageType.NewSubModul;
                                        msg.SubscribeModul(subModul);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }));
                            }

                        }
                        context.Response.Write("SUCCESS");
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        conn.Close();
                    }


                }
            }
            catch (Exception ex)
            {

                context.Response.Write(ex);
            }

        }

        private void Logout(HttpContext context)
        {
            context.Session["Login"] = null;
            using (SqlConnection conn = new SqlConnection(Connection.Conn))
            {
                try
                {
                    conn.Open();
                    //string ip = "192.168.0.24";
                    string ip = context.Request.Params["REMOTE_ADDR"];
                    string[] query = context.Request.QueryString["logout"].Split(new char[] { '/' });
                    string sql = "update HistoryLoginModulAplikasi_s set TglLogout='" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss") + "' where kdProfile='" + query[0] + "' and KdHistoryLogin='" + query[1] + "'";
                    SqlCommand comd = new SqlCommand(sql, conn);
                    var execute = comd.ExecuteReader();
                    conn.Close();
                    IQueryBuilder queryHistory = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);

                    queryHistory.SelectTable("HistoryLoginModulAplikasi_s");
                    queryHistory.AddCriteria(queryHistory.Criteria.Equal("HistoryLoginModulAplikasi_s.kdProfile", query[0]));
                    queryHistory.AddCriteria(queryHistory.Criteria.Equal("HistoryLoginModulAplikasi_s.KdHistoryLogin", query[1]));
                    queryHistory.AddJoin(queryHistory.Join.InnerJoin("Ruangan_M", "HistoryLoginModulAplikasi_s", "KdRuanganUser", "KdRuangan"));
                    queryHistory.AddJoin(queryHistory.Join.InnerJoin("ModulAplikasi_S", "HistoryLoginModulAplikasi_s", "KdModulAplikasi", "KdModulAplikasi"));
                    queryHistory.AddField("Ruangan_M.NamaRuangan");
                    queryHistory.AddField("ModulAplikasi_S.ReportDisplay");
                    sql = queryHistory.BuildQuery();
                    conn.Open();
                    comd = new SqlCommand(sql, conn);
                    execute = comd.ExecuteReader();
                    while (execute.Read())
                    {
                        lock (obj)
                        {
                            string modulName = execute[1].ToString();
                            string Ruangan = execute[0].ToString();
                            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
                            {
                                try
                                {
                                    Exchange msg = new Exchange(new EndpointAddress(new Uri("net.tcp://" + PointService.GetServer() + ":8020/ServiceExchange")), PointService.SetNetTCP());
                                    msg.UnSubscribe(new ModulMessage()
                                    {
                                        IP = ip,
                                        ModuleName = modulName,
                                        Ruangan = Ruangan,
                                        Type = MessageType.UnSubscribe
                                    });
                                }
                                catch (Exception)
                                {


                                }
                            }));
                        }

                    }

                    conn.Close();
                    context.Response.Write(true);
                }
                catch (System.Exception)
                {
                    context.Response.Write(false);
                }

            }
        }

        private void Update(HttpContext context)
        {
            try
            {
                string[] queryString = context.Request.QueryString["updateUser"].Split(new char[] { '/' });
                string kdPegawai = queryString[0];
                string username = queryString[1];
                string kataSandi = queryString[2];
                using (SqlConnection conn = new SqlConnection(Connection.Conn))
                {
                    try
                    {
                        conn.Open();
                        string sql = "update loginuser_s set namauser='" + Cryptography.funcAESEncrypt(username) + "',katasandi='" + Cryptography.funcAESEncrypt(kataSandi) + "' where kdPegawai='" + kdPegawai + "'";
                        SqlCommand comd = new SqlCommand(sql, conn);
                        var execute = comd.ExecuteReader();

                        conn.Close();

                        context.Response.Write(true);
                    }
                    catch (System.Exception)
                    {
                        context.Response.Write(false);
                    }

                }
            }
            catch (System.Exception ex)
            {
                context.Response.Write(ex);
            }
        }

        void Login(HttpContext context)
        {
            // lock (context)
            try
            {
                bool valUsername = false;
                bool valModul = false;
                bool valRuangan = false;
                bool valAdmin = false;
                bool validated = false;
                string ip = context.Request.Params["REMOTE_ADDR"];
                string result;
                string kdpegawai = "";
                string statuslogin;
                string kdkelompokuser;
                using (SqlConnection conn = new SqlConnection(Connection.Conn))
                {
                    conn.Open();
                    string[] queryString = context.Request.QueryString["login"].Split(new char[] { '/' });
                    string namaruangan;
                    string username = queryString[0];
                    string password = queryString[1];
                    string kdruangan = queryString[2];
                    string kdprofile = queryString[3];
                    string kdmodulaplikasi = queryString[4];
                    //string namahost = "";
                    IQueryBuilder queryUsername = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);

                    queryUsername.SelectTable("loginuser_s");
                    queryUsername.AddField("kdpegawai");
                    queryUsername.AddField("convert(varchar(50), namauser)");
                    queryUsername.AddField("convert(varchar(50), katasandi)");
                    queryUsername.AddField("kdkelompokuser");
                    queryUsername.AddField("statuslogin");
                    queryUsername.AddField("statusenabled");
                    queryUsername.AddCriteria(queryUsername.Criteria.Equal("namauser", Cryptography.funcAESEncrypt(queryString[0])));
                    queryUsername.AddCriteria(queryUsername.Criteria.Equal("kdprofile", kdprofile));
                    queryUsername.AddCriteria(queryUsername.Criteria.Equal("statusenabled", 1));
                    string sql = queryUsername.BuildQuery();
                    SqlCommand comd = new SqlCommand(sql, conn);
                    var dtUsername = comd.ExecuteReader();
                    while (dtUsername.Read())
                    {
                        kdpegawai = dtUsername[0].ToString();
                        result = Cryptography.funcAESDecrypt(dtUsername[2].ToString());
                        kdkelompokuser = dtUsername[3].ToString();
                        statuslogin = dtUsername[4].ToString();

                        valUsername = result == password && statuslogin == "0" ? true : false;
                        valAdmin = kdkelompokuser == "0" ? true : false;

                        //=====

                        if (valUsername)
                        {
                            IQueryBuilder queryModul = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
                            queryModul.SelectTable("mappegawaitomodulaplikasi_s");
                            queryModul.AddField("kdmodulaplikasi");
                            queryModul.AddCriteria(queryModul.Criteria.Equal("kdmodulaplikasi", kdmodulaplikasi));
                            queryModul.AddCriteria(queryModul.Criteria.Equal("kdpegawai", kdpegawai));

                            //queryModul.AddJoin(queryModul.Join.InnerIJoin("modulaplikasi_s", "mappegawaitomodulaplikasi_s", "kdmodulaplikasi", "kdmodulaplikasi"));
                            //queryModul.AddField("mappegawaitomodulaplikasi_s.kdpegawai");
                            //queryModul.AddField("modulaplikasi_s.kdmodulaplikasi");
                            //queryModul.AddField("modulaplikasi_s.modulaplikasi");

                            //queryModul.AddCriteria(queryModul.Criteria.Equal("mappegawaitomodulaplikasi_s.kdpegawai", kdpegawai));
                            //queryModul.AddCriteria(queryModul.Criteria.Equal("upper(modulaplikasi_s.modulaplikasi)", modulaplikasi));

                            string queryRes = queryModul.BuildQuery();
                            conn.Close();
                            conn.Open();
                            SqlCommand comd1 = new SqlCommand(queryRes, conn);

                            var temo = comd1.ExecuteReader();
                            valModul = false;
                            while (temo.Read())
                            {
                                valModul = true;
                                kdmodulaplikasi = temo[0].ToString();
                            }

                        }

                        //=====

                        if (valModul)
                        {
                            IQueryBuilder queryRuangan = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
                            queryRuangan.SelectTable("maploginusertoruangan_s");
                            queryRuangan.AddJoin(queryRuangan.Join.InnerJoin("ruangan_m", "maploginusertoruangan_s", "kdruangan", "kdruangan"));
                            queryRuangan.AddField("ruangan_m.kdruangan");
                            queryRuangan.AddField("ruangan_m.namaruangan");

                            queryRuangan.AddCriteria(queryRuangan.Criteria.Equal("maploginusertoruangan_s.kdpegawai", kdpegawai));
                            queryRuangan.AddCriteria(queryRuangan.Criteria.Equal("ruangan_m.kdruangan", kdruangan));
                            conn.Close();
                            conn.Open();
                            string sqlRuangan = queryRuangan.BuildQuery();
                            SqlCommand comd2 = new SqlCommand(sqlRuangan, conn);
                            var ruanganExecute = comd2.ExecuteReader();
                            valRuangan = false;
                            while (ruanganExecute.Read())
                            {
                                valRuangan = true;
                                namaruangan = ruanganExecute[1].ToString();
                            }


                        }

                        //=====

                        if (valRuangan)
                        {
                            try
                            {
                                conn.Close();
                                conn.Open();
                                ulong intMaxLogin = 0;
                                SqlCommand _comd2 = new SqlCommand("select max(KdHistoryLogin) from HistoryLoginModulAplikasi_S", conn);
                                var modulExecute = _comd2.ExecuteReader();
                                while (modulExecute.Read())
                                {
                                    if (modulExecute[0] == DBNull.Value)
                                        intMaxLogin = 1;
                                    else
                                        intMaxLogin = Convert.ToUInt64(modulExecute[0]) + 1;
                                }
                                conn.Close();
                                conn.Open();
                                string hostName = "";
                                try
                                {
                                    hostName = Dns.GetHostByAddress(ip).HostName;
                                }
                                catch (Exception)
                                {
                                    hostName = "Undifined";
                                }

                                string insertSQL = "INSERT INTO HistoryLoginModulAplikasi_S (KdProfile, KdHistoryLogin, KdModulAplikasi, KdUser, KdRuanganUser, NamaHost, TglLogin)" +
                                "VALUES     (" + kdprofile + ", " + intMaxLogin + ", '" + kdmodulaplikasi + "', '" + kdpegawai + "', '" + kdruangan + "', '" + hostName + "','" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss") + "')";
                                _comd2 = new SqlCommand(insertSQL, conn);
                                _comd2.ExecuteReader();
                                validated = true;
                            }
                            catch (Exception)
                            {


                            }

                        }
                        break;
                    }

                    conn.Close();
                    if (validated)
                    {
                        context.Session.Add("Login", "true");

                        IQueryBuilder queryModulAplikasi = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
                        queryModulAplikasi.SelectTable("ModulAplikasi_S");
                        queryModulAplikasi.AddField("ReportDisplay");
                        queryModulAplikasi.AddCriteria(queryModulAplikasi.Criteria.Equal("KdModulAplikasi", kdmodulaplikasi));
                        string sqlModul = queryModulAplikasi.BuildQuery();
                        conn.Open();
                        SqlCommand _comd2 = new SqlCommand(sqlModul, conn);
                        var modulExecute = _comd2.ExecuteReader();
                        string modul = "";
                        while (modulExecute.Read())
                        {
                            modul = modulExecute[0].ToString();

                        }
                        queryModulAplikasi = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
                        queryModulAplikasi.SelectTable("Ruangan_M");
                        queryModulAplikasi.AddField("NamaRuangan");
                        queryModulAplikasi.AddCriteria(queryModulAplikasi.Criteria.Equal("KdRuangan", kdruangan));
                        sqlModul = queryModulAplikasi.BuildQuery();
                        conn.Close();
                        conn.Open();
                        _comd2 = new SqlCommand(sqlModul, conn);
                        modulExecute = _comd2.ExecuteReader();
                        while (modulExecute.Read())
                        {
                            lock (obj)
                            {
                                string temp = modulExecute[0].ToString();
                                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
                                {
                                    try
                                    {
                                        Exchange msg = new Exchange(new EndpointAddress(new Uri("net.tcp://" + PointService.GetServer() + ":8020/ServiceExchange")), PointService.SetNetTCP());
                                        msg.Subscribe(new ModulMessage()
                                        {
                                            IP = ip,
                                            //  IP = "192.168.0.24",
                                            ModuleName = modul,
                                            NamaUser = username,
                                            Ruangan = temp,
                                            Type = MessageType.Subscribe
                                        }, TypeSubcribe.APLIKASI);
                                    }
                                    catch (Exception)
                                    {

                                    }

                                }));
                            }
                            //                            Dns.GetHostByAddress(ip).HostName.Split(new char[] { '.' })[0].ToUpper();

                        }
                    }
                    context.Response.Write((validated) ? kdpegawai : "@$##^%$^&@%&^^*&#@^&(*&#@&*(&*#@(&");
                }

            }
            catch (System.Exception ex)
            {
                context.Response.Write(ex.ToString());
            }
        }

        public string GetKdHistoryLogin(string kdpegawai, string kdmodulaplikasi, string kdruangan)
        {
            try
            {
                string ip = HttpContext.Current.Request.Params["REMOTE_ADDR"];
                IQueryBuilder query = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
                query.SelectTable("HistoryLoginModulAplikasi_S");
                //query.AddField("kdhistorylogin");
                query.AddAggregate(query.Aggregate.Max("kdhistorylogin"));
                query.AddCriteria(query.Criteria.Equal("kduser", kdpegawai));
                query.AddCriteria(query.Criteria.Equal("kdmodulaplikasi", kdmodulaplikasi));
                query.AddCriteria(query.Criteria.Equal("kdruanganuser", kdruangan));
                string hostName = "";
                try
                {
                    hostName = Dns.GetHostByAddress(ip).HostName;
                }
                catch (Exception)
                {
                    hostName = "Undifined";
                }
                query.AddCriteria(query.Criteria.Equal("namahost", hostName));
                using (SqlConnection conn = new SqlConnection(Connection.Conn))
                {

                    conn.Open();
                    string sql = query.BuildQuery();
                    SqlCommand _comd2 = new SqlCommand(sql, conn);
                    var modulExecute = _comd2.ExecuteReader();
                    while (modulExecute.Read())
                    {
                        return modulExecute[0].ToString();
                    }

                    conn.Close();
                }
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

//   public string GetKdHistoryLogin(ref IServiceDataUI service, string kdpegawai, string kdmodulaplikasi, string kdruangan, string namahost)
//{
//    try
//    {
//        IQueryBuilder query = QueryBuilderDB.CreateDatabaseQuery(ActiveDatabase.pActiveDB);
//        query.SelectTable("HistoryLoginModulAplikasi_S");
//        //query.AddField("kdhistorylogin");
//        query.AddAggregate(query.Aggregate.Max("kdhistorylogin"));
//        query.AddCriteria(query.Criteria.Equal("kduser", kdpegawai));
//        query.AddCriteria(query.Criteria.Equal("kdmodulaplikasi", kdpegawai));
//        query.AddCriteria(query.Criteria.Equal("kdruanganuser", kdruangan));
//        query.AddCriteria(query.Criteria.Equal("namahost", namahost));

//        string result = service.Find(query.BuildQuery()).Rows[0][0].ToString();
//        return result;
//    }
//    catch (Exception ex)
//    {

//        throw new Exception(ex.ToString());
//    }
//}

//public ArrayList GetGlobalVar(ref IServiceDataUI service, string username, string kdruangan)
//{
//    try
//    {
//        ArrayList result = new ArrayList();
//        DataTable tempDT;

//        IQueryBuilder queryLoginUser = QueryBuilderDB.CreateDatabaseQuery(ActiveDatabase.pActiveDB);
//        queryLoginUser.SelectTable("LoginUser_S");
//        queryLoginUser.AddField("KdPegawai");
//        queryLoginUser.AddField("convert(varchar(50), NamaUser)");
//        queryLoginUser.AddField("KdKelompokUser");
//        queryLoginUser.AddCriteria(queryLoginUser.Criteria.Equal("convert(varchar(50), NamaUser)", username));

//        tempDT = service.Find(queryLoginUser.BuildQuery());
//        result.Add(tempDT.Rows[0][0].ToString());
//        result.Add(tempDT.Rows[0][1].ToString());
//        result.Add(tempDT.Rows[0][2].ToString());
//        tempDT.Clear();

//        IQueryBuilder queryDataPegawai = QueryBuilderDB.CreateDatabaseQuery(ActiveDatabase.pActiveDB);
//        queryDataPegawai.SelectTable("Pegawai_M");
//        queryDataPegawai.AddField("NamaPegawai");
//        queryDataPegawai.AddCriteria(queryDataPegawai.Criteria.Equal("KdPegawai", result[0].ToString()));

//        tempDT = service.Find(queryDataPegawai.BuildQuery());
//        result.Add(tempDT.Rows[0][0].ToString());
//        tempDT.Clear();

//        IQueryBuilder queryRuangan = QueryBuilderDB.CreateDatabaseQuery(ActiveDatabase.pActiveDB);
//        queryRuangan.SelectTable("Ruangan_M");
//        queryRuangan.AddField("NamaRuangan");
//        queryRuangan.AddField("KdDepartemen");
//        queryRuangan.AddCriteria(queryRuangan.Criteria.Equal("KdRuangan", kdruangan));

//        tempDT = service.Find(queryRuangan.BuildQuery());
//        result.Add(tempDT.Rows[0][0].ToString());
//        result.Add(tempDT.Rows[0][1].ToString());
//        tempDT.Clear();

//        return result;
//    }
//    catch (Exception ex)
//    {

//        throw new Exception(ex.ToString());
//    }
//}

//public bool AuthenticateDesktop(ref IServiceDataUI service, string username, string password, string kdruangan, string kdprofile, string kdmodulaplikasi, string namahost)
//{
//    try
//    {
//        bool valUsername = false;
//        bool valModul = false;
//        bool valRuangan = false;
//        bool valAdmin = false;
//        bool validated = false;

//        string result;
//        string kdpegawai;
//        string statuslogin;
//        string kdkelompokuser;

//        LoginUser_S loginuser = new LoginUser_S();

//        IQueryBuilder queryUsername = QueryBuilderDB.CreateDatabaseQuery(ActiveDatabase.pActiveDB);

//        queryUsername.SelectTable("loginuser_s");
//        queryUsername.AddField("kdpegawai");
//        queryUsername.AddField("convert(varchar(50), katasandi)");
//        queryUsername.AddField("kdkelompokuser");
//        queryUsername.AddField("statuslogin");

//        queryUsername.AddCriteria(queryUsername.Criteria.Equal("namauser", username));
//        queryUsername.AddCriteria(queryUsername.Criteria.Equal("kdprofile", kdprofile));
//        queryUsername.AddCriteria(queryUsername.Criteria.Equal("statusenabled", 1));

//        DataTable dtUsername = service.Find(queryUsername.BuildQuery());

//        kdpegawai = dtUsername.Rows[0][0].ToString();
//        result = Cryptography.funcAESDecrypt(dtUsername.Rows[0][2].ToString());
//        kdkelompokuser = dtUsername.Rows[0][3].ToString();
//        statuslogin = dtUsername.Rows[0][4].ToString();

//        valUsername = result == password && statuslogin == "0" ? true : false;
//        valAdmin = kdkelompokuser == "0" ? true : false;

//        //=====

//        if (valUsername)
//        {
//            IQueryBuilder queryModul = QueryBuilderDB.CreateDatabaseQuery(ActiveDatabase.pActiveDB);

//            queryModul.SelectTable("mappegawaitomodulaplikasi_s");
//            queryModul.AddField("kdmodulaplikasi");
//            queryModul.AddCriteria(queryModul.Criteria.Equal("kdmodulaplikasi", kdmodulaplikasi));
//            queryModul.AddCriteria(queryModul.Criteria.Equal("kdpegawai", kdpegawai));

//            string queryRes = queryModul.BuildQuery();
//            DataTable dtModul = service.Find(queryRes);

//            if (dtModul.Rows.Count == 0)
//            {
//                valModul = false;
//            }
//            else
//            {
//                valModul = true;
//                kdmodulaplikasi = dtModul.Rows[0][0].ToString();
//            }
//        }

//        //=====

//        if (valModul)
//        {
//            IQueryBuilder queryRuangan = QueryBuilderDB.CreateDatabaseQuery(ActiveDatabase.pActiveDB);

//            queryRuangan.SelectTable("maploginusertoruangan_s");
//            queryRuangan.AddJoin(queryRuangan.Join.InnerIJoin("ruangan_m", "maploginusertoruangan_s", "kdruangan", "kdruangan"));
//            queryRuangan.AddField("ruangan_m.kdruangan");

//            queryRuangan.AddCriteria(queryRuangan.Criteria.Equal("maploginusertoruangan_s.kdpegawai", kdpegawai));
//            queryRuangan.AddCriteria(queryRuangan.Criteria.Equal("ruangan_m.kdruangan", kdruangan));

//            DataTable dtMap = service.Find(queryRuangan.BuildQuery());

//            if (dtMap.Rows.Count == 0)
//            {
//                valRuangan = false;
//            }
//            else
//            {
//                valRuangan = true;
//            }
//        }

//        //=====

//        if (valRuangan)
//        {
//            UpdateHistoryLogin(ref service, kdprofile, kdmodulaplikasi, kdpegawai, kdruangan, namahost);
//            validated = true;
//        }

//        return validated;
//    }
//    catch (Exception ex)
//    {

//        throw new Exception(ex.ToString());
//    }
//}

//public void LogoutUser(ref IServiceDataUI service, string kdhistorylogin, string kdpegawai, string kdprofile)
//{
//    HistoryLoginModulAplikasi_S history = new HistoryLoginModulAplikasi_S();
//    history.KdProfile = Convert.ToInt32(kdprofile);
//    history.KdHistoryLogin = Convert.ToInt32(kdhistorylogin);
//    history.TglLogout = DateTime.Now;

//    HistoryLoginModulAplikasi_S updatedHistory = (HistoryLoginModulAplikasi_S)service.GetOneData(history);
//    service.Save(updatedHistory);
//    //=====
//    LoginUser_S login = new LoginUser_S();
//    login.KdProfile = Convert.ToInt32(kdprofile);
//    login.KdPegawai = kdpegawai;
//    login.StatusLogin = 0;

//    LoginUser_S updatedLogin = (LoginUser_S)service.GetOneData(login);
//    service.Save(updatedLogin);
//}

//#endregion

//private void UpdateHistoryLogin(ref IServiceDataUI service, string kdprofile, string kdmodulaplikasi, string kduser, string kdruangan, string namahost)
//{

//    HistoryLoginModulAplikasi_S history = new HistoryLoginModulAplikasi_S();

//    history.KdProfile = Convert.ToInt32(kdprofile);
//    history.KdModulAplikasi = kdmodulaplikasi;
//    history.KdUser = kduser;
//    history.KdRuanganUser = kdruangan;
//    history.NamaHost = namahost;
//    history.TglLogin = DateTime.Now;

//    service.Save(history);
//}
