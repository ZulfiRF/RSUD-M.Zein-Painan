using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.ServiceModel;
using System.Text;
using System.Web;
using Medifirst.Service.Impl;
using Medifirst.Service.Impl.ServiceQueue;
using Medifirt.Queue.Handler;

namespace Jasamedika.Medifirst.Web.API
{
    public class Antrian : IHttpHandler
    {
        public static List<Guid> guid;
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
            if (context.Request.QueryString["call"] != null)
            {
                string post = context.Request.ContentEncoding.GetString(context.Request.BinaryRead(context.Request.ContentLength));
                context.Response.Write(post);
                Handler Handle = new Handler();                
                var q = new QueueService(new InstanceContext(Handle));
                if (guid == null)
                {
                    guid = new List<Guid>();
                    Guid gd = Guid.NewGuid();
                    guid.Add(gd);
                    q.Subscribe(gd,Message.TypeMessage.UpdatePasien);
                    gd = Guid.NewGuid();
                    guid.Add(gd);
                    q.Subscribe(gd, Message.TypeMessage.NewPasien);
                }
                if (post != string.Empty)
                {
                    q.Send(new Message()
                    {
                    Content = post
                    ,
                    Type = Message.TypeMessage.UpdatePasien
                    });
                    q.Send(new Message()
                    {
                    Content = post,
                    Type = Message.TypeMessage.CountingPasien
                    });
                }

            }
            else
            {
                RetriveData(context);
            }
        }

      

        private void RetriveData(HttpContext context)
        {
            using (SqlConnection conn = new SqlConnection(Connection.Conn))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT  dbo.AntrianPasienRegistrasi_T.NoRegistrasi, dbo.AntrianPasienRegistrasi_T.KdProfile, dbo.AntrianPasienRegistrasi_T.KdAntrian, dbo.AntrianPasienRegistrasi_T.TglAntrian, dbo.AntrianPasienRegistrasi_T.NoAntrian, dbo.AntrianPasienRegistrasi_T.StatusPasien, dbo.AntrianPasienRegistrasi_T.NoCM,  dbo.AntrianPasienRegistrasi_T.KdRuangan, dbo.Ruangan_M.NamaRuangan FROM         dbo.AntrianPasienRegistrasi_T INNER JOIN dbo.Ruangan_M ON dbo.AntrianPasienRegistrasi_T.KdRuangan = dbo.Ruangan_M.KdRuangan";
                    SqlCommand comnd = new SqlCommand(sql + " where  CONVERT(VARCHAR(10),  dbo.AntrianPasienRegistrasi_T.TglAntrian, 103) ='" + DateTime.Now.ToString("dd/MM/yyyy") + "'", conn);
                    var read = comnd.ExecuteReader();
                    string Json = "[";
                    StringBuilder sbstr = new StringBuilder("[");
                    //int i = 0;
                    while (read.Read())
                    {
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
                                sbstr.Append("\"" + read.GetName(i) + "\":" + tempString);
                            else
                                sbstr.Append(",\"" + read.GetName(i) + "\":" + tempString);
                        }
                        sbstr.Append("}");
                    }
                    sbstr.Append("]");
                    context.Response.Write(sbstr);
                }
                catch (Exception ex)
                {
                    context.Response.Write(ex);
                }
                finally
                {
                    conn.Close();
                }

            }
        }

        #endregion
    }
}
