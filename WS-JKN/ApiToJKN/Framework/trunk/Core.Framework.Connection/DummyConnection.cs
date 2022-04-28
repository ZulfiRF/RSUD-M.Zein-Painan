using System;
using System.Collections.Generic;
using Core.Framework.Helper;
using Core.Framework.Helper.Contracts;
using Core.Framework.Model;
using Core.Framework.Model.Impl;
using Microsoft.Win32;

namespace Core.Framework.Connection
{
    public class DummyConnection : ISettingRepository
    {
        #region Implementation of IConnectionString

        /// <summary>
        /// digunakan untuk mengambil data connection string dari sebuah repository
        /// </summary>
        /// <returns></returns>
        public ConnectionDomain GetIdentity(string projectName)
        {
            var result = new ConnectionDomain();
            //result.ServerName = @"serveroltp\ss2008r2_de";
            //result.ServerName = @"192.168.0.1\ss2008_dev";

            result.ServerName = @"192.168.0.1\ss2008r2_de";
           // result.ServerName = @".\ss2008r2";
            //result.ServerName = @"JASAMEDIKA-PC\SS2008R2";
            result.Database = "Medifirst2000";
            //result.Database = "Medifirst2000_SIMRS_DE";
            //result.Database = "Medifirst2000_MR";
            //result.Database = "Medifirst2000_SIMRS2";
            result.UserName = "sa";
            result.Password = "j4s4medik4";
            return result;
        }

        public void SetIdentity(string projectName, ConnectionDomain model)
        {
            if (string.IsNullOrEmpty(projectName))
                return;
            var openSubKey = Registry.CurrentUser.OpenSubKey(@"Software\Medifirst2000\" + projectName, true);
            if (openSubKey == null)
            {
                Registry.CurrentUser.CreateSubKey(@"Software\Medifirst2000\" + projectName);
                openSubKey = Registry.CurrentUser.OpenSubKey(@"Software\Medifirst2000\" + projectName, true);
                var currentRegistry = Registry.CurrentUser.OpenSubKey(@"Software\Medifirst2000", true);
                if (currentRegistry != null)
                {
                    currentRegistry.SetValue("Current", projectName);
                    currentRegistry.SetValue("Profile", model.Profile);
                    currentRegistry.SetValue("KdProfile", model.KdProfile);
                    currentRegistry.SetValue("Modul Aplikasi", model.Modul);
                }
            }
            if (openSubKey != null)
            {
                openSubKey.SetValue("Server Name", model.ServerName);
                openSubKey.SetValue("Database Name", model.Database);
                openSubKey.SetValue("User Name", model.UserName);
                openSubKey.SetValue("Password Name", model.Password);
            }
        }

        /// <summary>
        /// digunakan untuk mengetahui jika connection telah benar atau tidak
        /// </summary>
        /// <returns></returns>
        public bool CekConnection()
        {
            return CekConnection(GetIdentity(CurrentProject().Name));
        }

        /// <summary>
        /// digunakan untuk mengetahui jika connection telah benar atau tidak
        /// </summary>
        /// <returns></returns>
        public bool CekConnection(ConnectionDomain model)
        {
            try
            {
                var setting = BaseDependency.Get<ISetting>();
                var engine = HelperManager.GetContextManager(setting.GetValue("contextManagerEngine")) as BaseConnectionManager;

                if (engine != null)
                {
                    engine.ConnectionString = engine.CreateConnectioString(model);
                    using (var manager = new ContextManager(engine.CreateConnectioString(model), engine))
                    {
                        var reader = manager.ExecuteQuery("SELECT 1");
                        if (reader == null)
                            return false;
                        return true;
                    }
                }
                return false;
            }
            catch (Exception exception)
            {
                throw;
            }

        }

        /// <summary>
        /// digunkan untuk mengambil database dari sebuah repository
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DatabaseItem> GetDatabase(ConnectionDomain connection = null)
        {
            var setting = BaseDependency.Get<ISetting>();
            var engine = HelperManager.GetContextManager(setting.GetValue("contextManagerEngine")) as BaseConnectionManager;
            if (engine != null)
            {
                var domainConnection = GetIdentity(CurrentProject().Name);
                domainConnection.Database = "master";
                using (var contextManager = new ContextManager(engine.CreateConnectioString(domainConnection),
                                                        engine))
                {
                    var reader = contextManager.ExecuteQuery("select * from sys.databases ");
                    if (reader != null)
                        while (reader.Read())
                        {
                            yield return new DatabaseItem
                            {
                                Name = reader[0].ToString()
                            };
                        }
                }

            }
        }

        /// <summary>
        /// digunkan untuk mengambil List Module yang terdaftar
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProjectItem> GetProject()
        {

            yield return new ProjectItem
            {
                Name = "Rekam Medis"
            };

        }

        /// <summary>
        /// digunkan untuk mengambil Project yang digunakan
        /// </summary>
        /// <returns></returns>
        public ProjectItem CurrentProject()
        {

            return new ProjectItem
            {
                Name = "Rekam Medis"
            };
        }

        /// <summary>
        /// digunkan untuk mengambil Profile yang digunakan
        /// </summary>
        /// <returns></returns>
        public ProfileItem CurrentProfile()
        {

            return new ProfileItem
            {
                CodeProfile = "0",
                Name = "Rekam Medis"
            };
        }

        /// <summary>
        /// digunkan untuk mengambil Modul yang digunakan
        /// </summary>
        /// <returns></returns>
        public ModuleItem CurrentModule()
        {
            return new ModuleItem
            {
                CodeModule = ""
            };
        }

        /// <summary>
        /// digunkan untuk mengambil Connection String
        /// </summary>
        /// <returns></returns>
        public string ConnectionString
        {
            get
            {
                var setting = BaseDependency.Get<ISetting>();
                if (setting != null)
                {
                    var engine =
                        HelperManager.GetContextManager(setting.GetValue("contextManagerEngine")) as
                        BaseConnectionManager;
                    if (engine != null)
                        return engine.CreateConnectioString(GetIdentity(CurrentProject().Name));
                }
                //return @"Data Source=JASAMEDIKA-PC\SS2008R2;Initial Catalog=Medisirst2000_MR;User ID=sa;Password=j4s4medik4";
                //return @"Data Source=192.168.0.1\SS2008R2_de;Initial Catalog=Medifirst2000_SIMRS_DE;User ID=sa;Password=j4s4medik4";
                return @"Data Source=192.168.0.1\SS2008R2;Initial Catalog=Medifirst2000;User ID=sa;Password=j4s4medik4";

            }
        }

        #endregion
    }
}
