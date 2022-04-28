using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using Core.Framework.Helper.Attributes;
using Core.Framework.Helper.Logging;

namespace Core.Framework.Helper
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Contracts;
    using Presenters;

    public class HelperManager
    {
        #region Static Fields

        private static readonly Dictionary<string, Type> ListAttachPresenter = new Dictionary<string, Type>();

        private static readonly Dictionary<string, Type> ListContextManager = new Dictionary<string, Type>();

        private static readonly Dictionary<string, Type> ListModule = new Dictionary<string, Type>();

        private static readonly Dictionary<string, Type> ListSearchFramework = new Dictionary<string, Type>();

        private static readonly Dictionary<string, Type> ListTableItem = new Dictionary<string, Type>();

        #endregion

        #region Public Methods and Operators

        public static void AddListSearchFramework(Type type)
        {
            Type item;
            Activator.CreateInstance(type);
            if (type.FullName != null && !ListSearchFramework.TryGetValue(type.FullName, out item))
            {

                ListSearchFramework.Add(type.FullName, type);
            }
            else
            {
                if (type.FullName != null)
                {
                    ListSearchFramework[type.FullName] = type;
                }
            }
        }
        public static object BindPengambilanObjekDariSource(object model, string displayMemberPath)
        {
            try
            {
                if (model is string)
                {
                    return model;
                }
                if (displayMemberPath.Contains("."))
                {
                    var current = model;
                    foreach (var s in displayMemberPath.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (current == null)
                            return current;
                        current = current.GetType().GetProperty(s).GetValue(current, null);
                    }
                    return current;
                    //return 
                    //    .Aggregate(
                    //        model,
                    //        (current, item) => current.GetType().GetProperty(item).GetValue(current, null));
                }
                if (model == DBNull.Value)
                    return null;
                if (model == null)
                    return null;
                var property = model.GetType().GetProperty(displayMemberPath);
                if (property == null)
                    return null;
                var values = property.GetValue(model, null);
                return values ?? string.Empty;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static string CodeMapping(object value, string keyDictionary)
        {
            try
            {
                var defaultItem = new[] { "" };
                foreach (
                    string item in
                        ConfigurationManager.AppSettings[keyDictionary].Split(
                            new[] { ';' },
                            StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] arr = item.Split(new[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length == 3)
                    {
                        defaultItem = arr;
                    }
                    else
                    {
                        string[] data = arr[1].Replace("'", "")
                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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

        public static Dictionary<string, Type> GetListAttachPresenter()
        {
            return ListAttachPresenter;
        }

        public static Dictionary<string, Type> GetListContextManager()
        {
            return ListContextManager;
        }

        public static Dictionary<string, Type> GetListModule()
        {
            return ListModule;
        }

        public static Dictionary<string, Type> GetListTableItem()
        {
            return ListTableItem;
        }

        public static Type GetModule(string name)
        {
            try
            {
                Type type;
                if (ListModule.TryGetValue(name, out type))
                {
                    return ListModule[name];
                }
                return type;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// untuk mendapatkan type dari domain yang dimaksud
        /// </summary>
        /// <param name="domain">parameter string domain name space</param>
        /// <returns>type</returns>
        public static Type GetListSearchFramework(string domain)
        {
            if (domain != null)
            {
                try
                {
                    string[] arr = domain.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length != 2)
                    {
                        return null;
                    }

                    Type type;
                    if (ListSearchFramework.TryGetValue(arr[0], out type))
                    {
                        var context = Activator.CreateInstance(type);
                        var method = context.GetType().GetMethod(arr[1]);
                        var infomationAttribute = method.GetCustomAttributes(true).Where(n => n is InformationModule);
                        foreach (var o in infomationAttribute)
                        {
                        }
                        var typeMethod = method.ReturnType.GetGenericArguments().SingleOrDefault();
                        return typeMethod;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            return null;
        }

        public static IEnumerable GetSearchFramwork(string keyDomain, string value, string field = "", object tag = null)
        {
            if (string.IsNullOrEmpty(keyDomain))
            {
                return new List<object>();
            }
            string[] arr = keyDomain.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length != 2)
            {
                return null;
            }
            try
            {
                Type type;
                if (ListSearchFramework.TryGetValue(arr[0], out type))
                {
                    object context = Activator.CreateInstance(type);
                    MethodInfo method = context.GetType().GetMethod(arr[1]);


                    if (method != null)
                    {
                        var parameters = method.GetParameters();
                        var count = parameters.Count();
                        if (count == 1)
                            return method.Invoke(context, new object[] { value }) as IEnumerable;
                        if (parameters[1].ParameterType == typeof(string))
                            return method.Invoke(context, new object[] { value, field }) as IEnumerable;
                        else
                            return method.Invoke(context, new object[] { value, tag }) as IEnumerable;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        public static void SetTableItem(Type value)
        {
            Type type;

            if (!ListTableItem.TryGetValue(value.FullName, out type))
            {
                ListTableItem.Add(value.FullName, value);
            }
        }

        public static Type GetTableItem(string name)
        {
            try
            {
                Type type;
                if (string.IsNullOrEmpty(name))
                    return null;
                if (ListTableItem.TryGetValue(name, out type))
                {
                    return ListTableItem[name];
                }
                return type;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void RegisterModule(string path = "")
        {
            useLog = BaseDependency.Get<ISetting>().GetValue("useLog");
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
            string location = path;
            if (string.IsNullOrEmpty(location))
            {
                location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (IsTesting)
                    location = Path.GetDirectoryName(Path.GetDirectoryName(location));
            }
            if (!Directory.Exists(location))
            {
                location = Path.GetDirectoryName(location);
            }
            if (location != null)
            {
                if (Directory.Exists(location))
                {
                    foreach (string directory in Directory.GetDirectories(location))
                    {
                        RegisterModuleFile(directory);
                    }
                    RegisterModuleFile(location);
                }
            }
            BindAssembly(Assembly.GetExecutingAssembly());
            BindAssembly(Assembly.GetCallingAssembly());
            BindAssembly(Assembly.GetEntryAssembly());
            BindAssembly();

        }


        #endregion

        #region Methods

        private static void BindAssembly(Assembly assembly)
        {
            try
            {
                if (assembly == null)
                    return;
                if (useLog.Equals("3"))
                    Log.Info("Add Assembly " + Path.GetFileName(assembly.Location));
                //if (assembly.FullName.StartsWith("Core.Framework."))return;
                if (assembly.FullName.StartsWith("Core.Framework.") && 
                    !assembly.FullName.Contains("Core.Framework.Model") && 
                    !assembly.FullName.Contains("Core.Framework.Helper") && 
                    !assembly.FullName.Contains("Core.Framework.Connection") &&
                    !assembly.FullName.Contains("Core.Framework.LogManagement") &&
                    !assembly.FullName.Contains("Core.Framework.LanguageEditor") &&
                    !assembly.FullName.Contains("Core.Framework.Security")) 
                    return;

                var arrayType = assembly.GetExportedTypes().ToArray();
                //Parallel.For(0, arrayType.Length, delegate(int i)
                ListType.AddRange(arrayType);
                ListType.AddRange(assembly.GetTypes().Where(n => n.IsInterface));
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }
        static readonly List<Type> ListType = new List<Type>();
        private static bool cekData = false;
        private class TaskItem
        {
            public string Path { get; set; }
            public TimeSpan LongTime { get; set; }
        }

        public static Type GetType(string name)
        {
            return ListType.FirstOrDefault(n => n.FullName.Equals(name));
        }

        public static List<Type> GetTypes()
        {
            return ListType;
        }
        private static void BindAssembly()
        {
            var i = 0;
            List<TaskItem> list = new List<TaskItem>();
            var types = ListType.Where(n => n.IsClass && !n.IsGenericType && !n.IsAbstract).Distinct().ToList();
            var typse = ListType.Where(n => n.FullName.Contains("ProdukDetailRepository"));

            foreach (Type type in types)
            {

                Stopwatch sp = new Stopwatch();
                sp.Start();
                OnUpdateLoad(new ItemEventArgs<string>(type.Name));
                OnReportProgress(new ItemEventArgs<ReportItem>(new ReportItem()
                {
                    Position = i,
                    Sum = types.Count
                }));
                if (useLog.Equals("3"))
                    Log.Info("Add Type " + type.FullName);
                i++;
                //var type = arrayType[i];
                if (!type.IsClass)
                {
                    continue;
                }
                if (type.IsGenericType)
                {
                    continue;
                }
                if (type.IsAbstract)
                {
                    continue;
                }
                Type item;
                if (type.BaseType != null
                    && type.BaseType.FullName.Equals("Core.Framework.Model.Impl.BaseConnectionManager"))
                {
                    if (type.FullName != null && !ListContextManager.TryGetValue(type.FullName, out item))
                    {
                        ListContextManager.Add(type.FullName, type);
                    }
                    else
                    {
                        if (type.FullName != null)
                        {
                            ListContextManager[type.FullName] = type;
                        }
                    }
                }
                var valid = false;
                if (type != null && FintTableItem(type))
                {
                    if (type.FullName != null && !ListTableItem.TryGetValue(type.FullName, out item))
                    {

                        ListTableItem.Add(type.FullName, type);
                    }
                    else
                    {
                        if (type.FullName != null)
                        {
                            ListTableItem[type.FullName] = type;
                        }
                    }
                    valid = true;
                }

                if (typeof(IBaseSearchFramework).IsAssignableFrom(type))
                {
                    Activator.CreateInstance(type);
                    if (type.FullName != null && !ListSearchFramework.TryGetValue(type.FullName, out item))
                    {

                        ListSearchFramework.Add(type.FullName, type);
                    }
                    else
                    {
                        if (type.FullName != null)
                        {
                            ListSearchFramework[type.FullName] = type;
                        }
                    }
                    valid = true;
                }
                if (type.FullName != null && !ListModule.TryGetValue(type.FullName, out item))
                {

                    ListModule.Add(type.FullName, type);
                }
                else
                {
                    if (type.FullName != null)
                    {
                        ListModule[type.FullName] = type;
                    }
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
                    valid = true;

                }
                if (type.IsClass && !valid)
                {
                    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                    if (constructor != null)
                    {
                        try
                        {

                            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
                            //AppDomain.CurrentDomain.ResourceResolve += CurrentDomainOnResourceResolve;
                            if (constructor.ContainsGenericParameters)
                            {
                                continue;
                            }
                            if ((type.FullName.EndsWith("View")
                                 && type.Module.ToString().Contains("Form"))
                                ||
                                (type.FullName.ToString().Contains("Report") && !type.FullName.Contains("First")))
                            //|| (type.FullName.ToString().Contains("Report")))
                            {
                                continue;
                            }

                            if (type.FullName.Contains("Forms") || type.FullName.Contains("Views"))
                                continue;

                            if (type.Name.Contains("Testing"))
                            {
                                continue;
                            }

                            if (!type.Name.StartsWith("FirstLoad"))
                            {
                                Thread.Sleep(TimeSpan.FromTicks(100));
                                continue;
                            }

                            var instance = Activator.CreateInstance(type) as IFirstLoad;
                            if (instance != null)
                            {
                                instance.Initialize();
                            }
                        }
                        catch (Exception exception)
                        {
                            Log.Error("Terjadi Kesalahan [" + type + "] " + exception);
                        }
                    }
                }
                //sp.Stop();

                list.Add(new TaskItem()
                {
                    Path = type.Name,
                    LongTime = TimeSpan.FromTicks(sp.ElapsedTicks)
                });
                cekData = false;
                Thread.Sleep(TimeSpan.FromTicks(100));
            }

        }

        //private static Assembly CurrentDomainOnResourceResolve(object sender, ResolveEventArgs args)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(pathLoadAssembly))
        //        {
        //            return Assembly.LoadFile(pathLoadAssembly);
        //        }
        //        return null;
        //    }
        //    finally
        //    {
        //        pathLoadAssembly = "";
        //    }

        //}

        static Dictionary<string, Assembly> listAssemblyCache = new Dictionary<string, Assembly>();
        private static readonly List<string> datas = new List<string>();
        public static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                Assembly assembly;
                string key = args.Name.Split(new[] { ',' })[0];
                if (ListAssemblyCache.TryGetValue(key, out assembly))
                    return assembly;
                else
                {
                    if (args.RequestingAssembly != null)
                    {
                        key = args.RequestingAssembly.FullName.Split(new[] { ',' })[0] + "resource";
                        if (ListAssemblyCache.TryGetValue(key, out assembly))
                            return assembly;
                    }
                }
                if (useLog.Equals("3"))
                    Log.Info("Find Assembly " + args.Name);
                if (args.RequestingAssembly == null)
                {
                    return null;
                }
                if (string.IsNullOrEmpty(args.RequestingAssembly.Location)) return null;
                string directory = Path.GetDirectoryName(args.RequestingAssembly.Location);
                if (directory != null && Directory.Exists(directory))
                {
                    foreach (string varible in Directory.GetFiles(directory).Where(n => n.EndsWith(".dll")))
                    {
                        if (cekData)
                        {
                            datas.Add(varible);
                        }
                        if (varible.Contains("Core.Framework.Windows")) { continue; }
                        //if (Path.GetFileNameWithoutExtension(varible).Contains("Core") || Path.GetFileNameWithoutExtension(varible).Contains("Jasamedika") || Path.GetFileNameWithoutExtension(varible).Contains("Medifirst"))
                        {
                            if (!File.Exists(varible)) continue;
                            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
                            assembly = Assembly.LoadFile(varible);
                            if (useLog.Equals("3"))
                                Log.Info("Find Assembly " + args.Name + " >> " + assembly.FullName);
                            if (assembly.FullName.Split(new[] { ',' })[0].Equals(args.Name.Split(new[] { ',' })[0]))
                            {
                                try
                                {

                                    return assembly;
                                }
                                finally
                                {
                                    key = args.Name.Split(new[] { ',' })[0];
                                    Assembly asm;
                                    if (!ListAssemblyCache.TryGetValue(key, out asm))
                                        ListAssemblyCache.Add(key, assembly);
                                }


                            }
                            if (useLog.Equals("3"))
                                Log.Info("Find Assembly " + args.RequestingAssembly.FullName + " >> " + assembly.FullName);
                            if (args.Name.Contains("resources") &&
                                assembly.FullName.Split(new[] { ',' })[0].Equals(args.RequestingAssembly.FullName.Split(new[] { ',' })[0]))
                            {
                                var externalBaml = Assembly.LoadFile(varible);
                                Stream resourceStream = externalBaml.GetManifestResourceStream(externalBaml.GetName().Name + ".g.resources");
                                using (ResourceReader resourceReader = new ResourceReader(resourceStream))
                                {
                                    try
                                    {
                                        return resourceReader.GetType().Assembly;
                                    }
                                    finally
                                    {
                                        key = args.RequestingAssembly.FullName.Split(new[] { ',' })[0] + "resource";
                                        Assembly asm;
                                        if (!ListAssemblyCache.TryGetValue(key, out asm))
                                            ListAssemblyCache.Add(key, resourceReader.GetType().Assembly);
                                    }


                                }

                            }
                            //if (args.Name.Contains("resources") &&
                            //    assembly.FullName.Equals(args.RequestingAssembly.FullName))
                            //{
                            //    pathLoadAssembly = varible;
                            //    var asm = Assembly.LoadFile(varible);
                            //    string resourceName = asm.GetName().Name + ".resources";
                            //    if (args.Name.Contains(resourceName))
                            //        foreach (var fileStream in asm.GetFiles(true))
                            //        {
                            //            int i = 0;
                            //            var bytes = new List<byte>();
                            //            while (fileStream.Position != fileStream.Length)
                            //            {
                            //                var b = fileStream.ReadByte();
                            //                try
                            //                {
                            //                    if (b == -1)
                            //                        b = 0;
                            //                    bytes.Add(Convert.ToByte(b));
                            //                }
                            //                catch (Exception)
                            //                {
                            //                }

                            //            }
                            //            var y = bytes.Where(n => n != 0);
                            //            return Assembly.Load(bytes.ToArray());

                            //        }

                            //}
                        }
                    }

                    //if (directory != null)// GetDirectories(directory))
                    //{
                    //    foreach (var dir in Directory.GetDirectories(directory))
                    //    {
                    //        foreach (var source in Directory.GetFiles(dir).Where(n => n.EndsWith(".dll")))
                    //        {
                    //            if (source != null &&
                    //           (source.Contains("Core")
                    //           || source.Contains("Jasamedika")
                    //           || source.Contains("Medifirst")))
                    //            {
                    //                Assembly assembly = Assembly.LoadFile(source);
                    //                if (assembly.FullName.Equals(source))
                    //                {
                    //                    return assembly;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    return FindAssembly(Path.GetDirectoryName(directory), args);
                }
                return null;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                return null;
            }
        }

        private static Assembly FindAssembly(string directory, ResolveEventArgs name)
        {
            if (Directory.Exists(directory))
            {
                foreach (string varible in Directory.GetFiles(directory).Where(n => n.EndsWith(".dll")))
                {

                    if (varible.Contains("Core.Framework.Windows")) { continue; }
                    //if (Path.GetFileNameWithoutExtension(varible).Contains("Core") || Path.GetFileNameWithoutExtension(varible).Contains("Jasamedika") || Path.GetFileNameWithoutExtension(varible).Contains("Medifirst"))
                    {
                        //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
                        Assembly assembly = Assembly.LoadFile(varible);
                        if (assembly.FullName.Split(new[] { ',' })[0].Equals(name.Name.Split(new[] { ',' })[0]))
                        {
                            Debug.Print(assembly.GetName().Name);
                            return assembly;
                        }
                    }
                }

                if (directory != null)// GetDirectories(directory))
                {
                    foreach (var dir in Directory.GetDirectories(directory))
                    {
                        foreach (var subDirectory in Directory.GetDirectories(dir))
                        {
                            var asm = FindAssembly(subDirectory, name);
                            if (asm != null)
                                return asm;
                        }
                        foreach (var source in Directory.GetFiles(dir).Where(n => n.EndsWith(".dll")))
                        {
                            if (source != null &&
                           (source.Contains("Core")
                           || source.Contains("Jasamedika")
                           || source.Contains("Medifirst")))
                            {
                                Assembly assembly = Assembly.LoadFile(source);
                                if (useLog.Equals("3"))
                                    Log.Info("Find Assemblys " + name.Name + " >>" + assembly.FullName);
                                if (assembly.FullName.Split(new[] { ',' })[0].Equals(name.Name.Split(new[] { ',' })[0]))
                                {
                                    return assembly;
                                }
                                if (name.Name.Contains("resources") &&
                                assembly.FullName.Split(new[] { ',' })[0].Equals(name.RequestingAssembly.FullName.Split(new[] { ',' })[0]))
                                {
                                    var externalBaml = Assembly.LoadFile(source);
                                    Stream resourceStream = externalBaml.GetManifestResourceStream(externalBaml.GetName().Name + ".g.resources");
                                    using (ResourceReader resourceReader = new ResourceReader(resourceStream))
                                    {
                                        return resourceReader.GetType().Assembly;
                                    }

                                }
                            }
                        }
                    }
                }

                Debug.Print(directory + " , Name => " + name.Name);
                return FindAssembly(Path.GetDirectoryName(directory), name);
            }
            return null;
        }

        public static bool FintTableItem(Type type)
        {
            return FindType(type, "Core.Framework.Model.TableItem");
        }

        public static bool FindType(Type type, string nameClass)
        {
            while (type.BaseType != null)
            {
                bool result = type.BaseType.FullName != null && type.BaseType.FullName.Equals(nameClass);
                type = type.BaseType;
                if (result)
                {
                    return result;
                }
            }
            return false;
        }
        public class ReportItem
        {
            public int Position { get; set; }
            public int Sum { get; set; }
        }
        public static event EventHandler<ItemEventArgs<string>> UpdateLoad;
        public static event EventHandler<ItemEventArgs<ReportItem>> ReportProgress;

        private static void OnReportProgress(ItemEventArgs<ReportItem> e)
        {
            EventHandler<ItemEventArgs<ReportItem>> handler = ReportProgress;
            if (handler != null) handler(null, e);
        }

        private static void OnUpdateLoad(ItemEventArgs<string> e)
        {
            EventHandler<ItemEventArgs<string>> handler = UpdateLoad;
            if (handler != null) handler(null, e);
        }

        static List<string> hasAddFile = new List<string>();
        private static string useLog;

        private static void RegisterModuleFile(string location)
        {
            foreach (string directory in Directory.GetDirectories(location))
            {
                RegisterModuleFile(directory);
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (string file in Directory.GetFiles(location).Where(n => n.ToLower().EndsWith(".dll")))
            {
                if (hasAddFile.Any(n => n.Equals(file)))
                {
                    continue;
                }
                if (useLog.Equals("3"))
                    Log.Info("Add Library " + Path.GetFileName(file));
                hasAddFile.Add(file);
                try
                {

                    if (file.Contains("Framework.Windows"))
                    {
                        continue;
                    }
                    if (Path.GetFileNameWithoutExtension(file).Contains("Core")
                        || Path.GetFileNameWithoutExtension(file).Contains("Jasamedika")
                        || Path.GetFileNameWithoutExtension(file).Contains("Medifirst"))
                    {
                        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
                        Assembly assembly = Assembly.LoadFile(file);
                        BindAssembly(assembly);
                    }
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
            }
            stopwatch.Stop();
            //Log.Info(location + " Mengahbiskan waktu " + TimeSpan.FromTicks(stopwatch.ElapsedTicks));
        }

        #endregion

        public static bool IsTesting { get; set; }

        public static Dictionary<string, Assembly> ListAssemblyCache
        {
            get
            {
                return listAssemblyCache;
            }

            set
            {
                listAssemblyCache = value;
            }
        }
    }
}