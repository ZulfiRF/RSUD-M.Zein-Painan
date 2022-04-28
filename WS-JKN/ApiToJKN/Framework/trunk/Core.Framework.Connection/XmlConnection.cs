using System;
using System.Collections.Generic;
using System.Linq;
using Core.Framework.Helper;
using Core.Framework.Helper.Configuration;
using Core.Framework.Helper.Contracts;
using Core.Framework.Helper.Logging;
using Core.Framework.Model;
using Core.Framework.Model.Impl;

namespace Core.Framework.Connection
{
    public class XmlConnection : ISettingRepository
    {
        #region Implementation of IConnectionString

        /// <summary>
        /// digunakan untuk mengambil data connection string dari sebuah repository
        /// </summary>
        /// <returns></returns>
        public ConnectionDomain GetIdentity(string projectName)
        {
            var context = BaseDependency.Get<ISetting>();
            var result = new ConnectionDomain();

            result.ServerName = context.GetValue("Server Name");
            result.Database = context.GetValue("Database Name");
            result.UserName = context.GetValue("User Name");
            result.Password = context.GetValue("Password Name");

            result.Modul = context.GetValue("Modul Aplikasi");
            result.KdProfile = context.GetValue("KdProfile");
            result.Profile = context.GetValue("Profile");

            return result;
        }

        public void SetIdentity(string projectName, ConnectionDomain model)
        {
            if (string.IsNullOrEmpty(projectName))
                return;
            var context = BaseDependency.Get<ISetting>();
            context.Save("Current", projectName);
            context.Save("Profile", model.Profile);
            context.Save("KdProfile", model.KdProfile);
            context.Save("Modul Aplikasi", model.Modul ?? "");

            context.Save("Server Name", model.ServerName);
            context.Save("Database Name", model.Database);
            context.Save("User Name", model.UserName);
            context.Save("Password Name", model.Password);
            connectionstring = "";
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
                var context = HelperManager.GetContextManager(setting.GetValue("contextManagerEngine"));
                var engine = context as BaseConnectionManager;

                if (engine != null)
                {
                    engine.ConnectionString = engine.CreateConnectioString(model);
                    connectionstring = engine.ConnectionString;
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

        private ConnectionDomain tempConnection;
        /// <summary>
        /// digunkan untuk mengambil database dari sebuah repository
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DatabaseItem> GetDatabase(ConnectionDomain connection = null)
        {
            var setting = BaseDependency.Get<ISetting>();
            var context = HelperManager.GetContextManager(setting.GetValue("contextManagerEngine"));
            var engine = context as BaseConnectionManager;
            if (engine != null)
            {
                ConnectionDomain domainConnection;
                if (connection == null) domainConnection = GetIdentity(CurrentProject().Name);
                else
                {
                    domainConnection = connection;
                    tempConnection = new ConnectionDomain()
                    {
                        Database = connection.Database,
                        Password = connection.Password,
                        ServerName = connection.ServerName,
                        UserName = connection.UserName,
                    };
                }
                domainConnection.Database = "Medifirst2000";
                using (var contextManager = new ContextManager(engine.CreateConnectioString(domainConnection),
                                                        engine))
                {
                    connectionstring = engine.CreateConnectioString(domainConnection);
                    var reader = contextManager.ExecuteQuery("select * from sys.databases ");
                    if (reader != null)
                        while (reader.Read())
                        {
                            yield return new DatabaseItem()
                            {
                                Name = reader[0].ToString()
                            };
                        }
                }

            }
            yield break;
        }

        /// <summary>
        /// digunkan untuk mengambil List Module yang terdaftar
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProjectItem> GetProject()
        {
            var context = BaseDependency.Get<ISetting>();
            if (context != null)
                return new[] { new ProjectItem() { Name = context.GetValue("Project") } };
            return null;
        }

        /// <summary>
        /// digunkan untuk mengambil Project yang digunakan
        /// </summary>
        /// <returns></returns>
        public ProjectItem CurrentProject()
        {
            var context = BaseDependency.Get<ISetting>();
            return new ProjectItem() { Name = context.GetValue("Current") };
        }

        /// <summary>
        /// digunkan untuk mengambil Project yang digunakan
        /// </summary>
        /// <returns></returns>
        public ProfileItem CurrentProfile()
        {
            var context = BaseDependency.Get<ISetting>();
            return new ProfileItem()
            {
                Name = context.GetValue("Profile"),
                CodeProfile = context.GetValue("KdProfile")
            };
        }

        /// <summary>
        /// digunkan untuk mengambil Modul yang digunakan
        /// </summary>
        /// <returns></returns>
        public ModuleItem CurrentModule()
        {
            var context = BaseDependency.Get<ISetting>();
            return new ModuleItem()
            {
                CodeModule = context.GetValue("Modul Aplikasi")
            };
        }

        private string connectionstring;
        /// <summary>
        /// digunkan untuk mengambil Connection String
        /// </summary>
        /// <returns></returns>
        public string ConnectionString
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(connectionstring)) return connectionstring;
                    var setting = BaseDependency.Get<ISetting>();
                    var context = HelperManager.GetContextManager(setting.GetValue("contextManagerEngine"));
                    var baseType = context.GetType().BaseType;
                    if (!(context is BaseConnectionManager)) return "";
                    var engine = (BaseConnectionManager)context;
                    if (engine != null)
                    {
                        if (tempConnection != null)
                            return engine.CreateConnectioString(tempConnection);
                        var project = CurrentProject();
                        if (project != null)
                        {
                            if (project.Name != null)
                            {
                                var identity = GetIdentity(project.Name);
                                if (identity != null)
                                    return connectionstring = engine.CreateConnectioString(identity);
                            }
                        }
                    }
                    return "";
                }
                catch (Exception e)
                {
                    Log.ThrowError(e, "200");
                    throw;
                }
            }
        }

        #endregion
    }
}
