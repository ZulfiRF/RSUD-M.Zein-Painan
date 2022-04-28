using System;
using System.Collections.Generic;
using Core.Framework.Helper;
using Core.Framework.Helper.Contracts;
using Core.Framework.Helper.Logging;
using Core.Framework.Model;
using Core.Framework.Model.Impl;
using Microsoft.Win32;

namespace Core.Framework.Connection
{
    public class RegistryConnection : ISettingRepository
    {
        #region Implementation of IConnectionString

        /// <summary>
        /// digunakan untuk mengambil data connection string dari sebuah repository
        /// </summary>
        /// <returns></returns>
        public ConnectionDomain GetIdentity(string projectName)
        {
            var result = new ConnectionDomain();
            if (projectName != null)
            {
                var openSubKey = Registry.CurrentUser.OpenSubKey(@"Software\Medifirst2000\" + projectName);
                if (openSubKey != null)
                {
                    result.ServerName = openSubKey.GetValue("Server Name").ToString();
                    result.Database = openSubKey.GetValue("Database Name").ToString();
                    result.UserName = openSubKey.GetValue("User Name").ToString();
                    result.Password = openSubKey.GetValue("Password Name").ToString();
                }

                var openKey = Registry.CurrentUser.OpenSubKey(@"Software\Medifirst2000\");
                if (openKey != null)
                {
                    result.Modul = openKey.GetValue("Modul Aplikasi").ToString();
                    result.KdProfile = openKey.GetValue("KdProfile").ToString();
                    result.Profile = openKey.GetValue("Profile").ToString();
                }
            }
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
                    currentRegistry.SetValue("Modul Aplikasi", model.Modul ?? "");
                }
            }
            if (openSubKey != null)
            {
                var currentRegistry = Registry.CurrentUser.OpenSubKey(@"Software\Medifirst2000", true);
                if (currentRegistry != null)
                {
                    currentRegistry.SetValue("Current", projectName);
                    currentRegistry.SetValue("Profile", model.Profile);
                    currentRegistry.SetValue("KdProfile", model.KdProfile);
                    currentRegistry.SetValue("Modul Aplikasi", model.Modul ?? "");
                }
                openSubKey.SetValue("Server Name", model.ServerName);
                openSubKey.SetValue("Database Name", model.Database);
                openSubKey.SetValue("User Name", model.UserName);
                openSubKey.SetValue("Password Name", model.Password);
            }
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
            var openSubKey = Registry.CurrentUser.OpenSubKey(@"Software\Medifirst2000");
            if (openSubKey != null)
                foreach (var subKeyName in openSubKey.GetSubKeyNames())
                {
                    yield return new ProjectItem()
                    {
                        Name = subKeyName
                    };
                }

        }

        /// <summary>
        /// digunkan untuk mengambil Project yang digunakan
        /// </summary>
        /// <returns></returns>
        public ProjectItem CurrentProject()
        {
            var openSubKey = Registry.CurrentUser.OpenSubKey(@"Software\Medifirst2000\");
            if (openSubKey != null)
            {
                if (openSubKey.GetValue("Current") != null)
                    return new ProjectItem()
                    {
                        Name = openSubKey.GetValue("Current").ToString()
                    };
                else
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// digunkan untuk mengambil Profile yang digunakan
        /// </summary>
        /// <returns></returns>
        public ProfileItem CurrentProfile()
        {
            var openSubKey = Registry.CurrentUser.OpenSubKey(@"Software\Medifirst2000\");
            if (openSubKey != null)
            {
                return new ProfileItem()
                {
                    CodeProfile = openSubKey.GetValue("KdProfile").ToString(),
                    Name = openSubKey.GetValue("Profile").ToString()
                };
            }
            return null;
        }

        /// <summary>
        /// digunkan untuk mengambil Modul yang digunakan
        /// </summary>
        /// <returns></returns>
        public ModuleItem CurrentModule()
        {
            var openSubKey = Registry.CurrentUser.OpenSubKey(@"Software\Medifirst2000\");
            if (openSubKey != null)
            {
                return new ModuleItem()
                {
                    CodeModule = openSubKey.GetValue("Modul Aplikasi").ToString()
                };
            }
            return new ModuleItem();
        }

        /// <summary>
        /// digunkan untuk mengambil Connection String
        /// </summary>
        /// <returns></returns>
        private string connectionstring;
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
