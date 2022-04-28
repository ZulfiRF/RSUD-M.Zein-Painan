using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Core.Framework.Helper.Contracts;
using Core.Framework.Helper.Presenters;
using System.Configuration;
using System.Diagnostics;

namespace Core.Framework.Helper
{
    public class HelperManager
    {

        private static readonly Dictionary<string, Type> ListModule = new Dictionary<string, Type>();
        private static readonly Dictionary<string, Type> ListTableItem = new Dictionary<string, Type>();
        private static readonly Dictionary<string, Type> ListContextManager = new Dictionary<string, Type>();
        private static readonly Dictionary<string, Type> ListAttachPresenter = new Dictionary<string, Type>();
        public static Dictionary<string, Type> GetListContextManager()
        {
            return ListContextManager;
        }
        public static Dictionary<string, Type> GetListTableItem()
        {
            return ListTableItem;
        }
        public static Dictionary<string, Type> GetListModule()
        {
            return ListModule;
        }

        public static Dictionary<string, Type> GetListAttachPresenter()
        {
            return ListAttachPresenter;
        }
        public static object GetContextManager(string key)
        {
            try
            {
                return Activator.CreateInstance(ListContextManager[key]);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static Type GetModule(string name)
        {
            try
            {
                Type type;
                if (ListModule.TryGetValue(name, out type))
                    return ListModule[name];
                return type;
            }
            catch (Exception)
            {
                return null;
            }

        }
        public static Type GetTableItem(string name)
        {
            try
            {
                Type type;
                if (ListTableItem.TryGetValue(name, out type))
                    return ListTableItem[name];
                return type;
            }
            catch (Exception)
            {
                return null;
            }

        }
        public static void RegisterModule(string path )
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
            string location = path;
            if (string.IsNullOrEmpty(location))
                location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (location != null)
            {
                foreach (var directory in Directory.GetDirectories(location))
                {
                    RegisterModuleFile(directory);
                }
                RegisterModuleFile(location);
            }
            BindAssembly(Assembly.GetExecutingAssembly());
        }

        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                //Console.WriteLine(args);
                //var directory = Path.GetDirectoryName(args.RequestingAssembly.Location);
                //if (Directory.Exists(directory))
                //{
                //    foreach (var varible in Directory.GetFiles(directory).Where(n => n.EndsWith(".dll")))
                //    {
                //        if (varible.Contains("Core.Framework.Windows")) continue;
                //        var assembly = Assembly.LoadFile(varible);
                //        if (assembly.FullName.Equals(args.Name))
                //        {
                //            return assembly;
                //        }
                //    }
                //    return FindAssembly(Path.GetDirectoryName(directory), args.Name);
                //}
                return null;
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        private static Assembly FindAssembly(string directory, string name)
        {
            if (Directory.Exists(directory))
            {
                foreach (var varible in Directory.GetFiles(directory).Where(n => n.EndsWith(".dll")))
                {
                    if (varible.Contains("Core.Framework.Windows")) continue;
                    var assembly = Assembly.LoadFile(varible);
                    if (assembly.FullName.Equals(name))
                    {
                        return assembly;
                    }
                }
                return FindAssembly(Path.GetDirectoryName(directory), name);
            }
            return null;
        }


        private static void RegisterModuleFile(string location)
        {
            foreach (var directory in Directory.GetDirectories(location))
            {
                RegisterModuleFile(directory);
            }
            foreach (var file in Directory.GetFiles(location).Where(n => n.EndsWith(".dll")))
            {
                try
                {
                    if (file.Contains("Framework.Windows")) continue;
                    var assembly = Assembly.LoadFile(file);
                    BindAssembly(assembly);
                }
                catch (Exception)
                {
                }
            }
        }

        private static void BindAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                Debug.WriteLine(type);
                if (!type.IsClass) continue;
                if (type.IsGenericType) continue;
                if (type.IsAbstract) continue;
                Type item;
                if (type.BaseType != null && type.BaseType.FullName.Equals("Core.Framework.Model.Impl.BaseConnectionManager"))
                {
                    if (type.FullName != null && !ListContextManager.TryGetValue(type.FullName, out item))
                    {
                        ListContextManager.Add(type.FullName, type);
                    }
                    else
                    {
                        if (type.FullName != null) ListContextManager[type.FullName] = type;
                    }
                }
                if (type != null && FintTableItem(type))
                {
                    if (type.FullName != null && !ListTableItem.TryGetValue(type.FullName, out item))
                    {

                        //  Assembly.LoadFrom(file);
                        // AppDomain.CurrentDomain.DefineDynamicAssembly(Assembly.LoadFile(file).GetName(true), AssemblyBuilderAccess.RunAndSave);
                        ListTableItem.Add(type.FullName, type);
                    }
                    else
                    {
                        if (type.FullName != null) ListTableItem[type.FullName] = type;
                    }
                }
                if (type.FullName != null && !ListModule.TryGetValue(type.FullName, out item))
                {
                    ListModule.Add(type.FullName, type);
                }
                else
                {
                    if (type.FullName != null) ListModule[type.FullName] = type;
                }
                if (typeof(IBasePresenter).IsAssignableFrom(type))
                {
                    item = null;
                    if (!ListAttachPresenter.TryGetValue(type.Name, out item))
                    {
                        ListAttachPresenter.Add(type.Name, type);
                    }
                    else
                    {
                        ListAttachPresenter[type.Name] = type;
                    }
                }
                if (type.IsClass)
                {
                    var constructor = type.GetConstructor(Type.EmptyTypes);
                    if (constructor != null)
                    {
                        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
                        if (constructor.ContainsGenericParameters) continue;
                        var instance = Activator.CreateInstance(type) as IFirstLoad;
                        if (instance != null)
                        {
                            try
                            {
                                instance.Initialize();
                            }
                            catch (Exception)
                            {
                                
                            }
                        }
                    }
                }
                //ThreadPool.QueueUserWorkItem(CallBack, type);
            }
        }

        private static bool FintTableItem(Type type)
        {

            while (type.BaseType != null)
            {
                var result = type.BaseType.FullName != null && type.BaseType.FullName.Equals("Core.Framework.Model.TableItem");
                type = type.BaseType;
                if (result)
                    return result;
            }
            return false;


        }

        public static string CodeMapping(object value, string keyDictionary)
        {
            try
            {
                var defaultItem = new[] { "" };
                foreach (var item in ConfigurationManager.AppSettings[keyDictionary].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var arr = item.Split(new[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length == 3)
                        defaultItem = arr;
                    else
                    {
                        var data = arr[1].Replace("'", "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (data.Any(n => n.ToString().ToLower().Equals(value.ToString().ToLower())))
                        {
                            return arr[0];
                        }

                    }


                }

                return defaultItem[0];
            }
            catch (Exception)
            {
                return "";

            }

        }

        public static object BindPengambilanObjekDariSource(object model, string displayMemberPath)
        {
            if (model is string)
                return model;
            if (displayMemberPath.Contains("."))
            {
                return displayMemberPath.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(model, (current, item) => current.GetType().GetProperty(item).GetValue(current, null));
            }
            return model.GetType().GetProperty(displayMemberPath).GetValue(model, null);

        }
    }
}
