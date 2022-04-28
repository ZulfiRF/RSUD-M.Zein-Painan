using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Core.Framework.Helper.Contracts;
using Core.Framework.Helper.Logging;

namespace Core.Framework.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    /// <summary>
    ///     Class BaseDependency
    /// </summary>
    public class BaseDependency
    {
        #region Static Fields

        /// <summary>
        ///     The list class
        /// </summary>
        private static readonly Dictionary<object, object> listClass = new Dictionary<object, object>();

        private static readonly Dictionary<object, List<object>> listMultipeClass =
            new Dictionary<object, List<object>>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseDependency" /> class.
        /// </summary>
        public BaseDependency()
        {
            Initialize();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the specified value.digunakan untuk  menambahkan Class Dependency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <example>
        ///     contoh yang digunakan untuk  menambahkan Class Dependency
        ///     <code>
        ///  public class TaskService : BaseDependency
        ///  {        
        ///      public ITaskRepository TaskRepository { get; set; }
        ///  }
        ///  public class Main 
        ///  {        
        ///      public Main()
        ///      {
        ///          BaseDependency.Add<![CDATA[<ITaskRepository>]]>(new TaskRepository());
        ///      }
        ///  }
        ///    
        /// </code>
        /// </example>
        public static void Add<T>(T value)
        {
            Type type = typeof(T);
            object obj;
            //if (type.Name.Contains("IMappingMasterView"))
            //{
            //    Debug.Print("Bingo");
            //}
            if (listClass.TryGetValue(type.ToString(), out obj))
            {
                //listClass[type.ToString()] = value;
            }
            else
            {
                listClass.Add(type.ToString(), value);
            }
        }

        public static void Add(object type, object value)
        {
            if (type == null) return;
            if (value == null) return;

            object obj;
            if (listClass.TryGetValue(type.ToString(), out obj))
            {
                listClass[type.ToString()] = value;
            }
            else
            {
                listClass.Add(type.ToString(), value);
            }
        }
        public static void Add<T>(object value)
        {
            Type type = typeof(T);
            object obj;
            //if (type.Name.Contains("IMappingMasterView"))
            //{
            //    Debug.Print("Bingo");
            //}
            if (listClass.TryGetValue(type.ToString(), out obj))
            {
                listClass[type.ToString()] = value;
            }
            else
            {
                listClass.Add(type.ToString(), value);
            }
        }

        public static List<object> ListObjectReference = new List<object>();
        private static string useLog;



        public static void AddByReference<T>(Type value)
        {
            Type type = typeof(T);
            object obj;
            //if (type.Name.Contains("IMappingMasterView"))
            //{
            //    Debug.Print("Bingo");
            //}
            if (listClass.TryGetValue(type.ToString(), out obj))
            {
                listClass[type.ToString()] = value;
            }
            else
            {
                listClass.Add(type.ToString(), value);
            }
        }
        public static void AddMultiple<T>(T value)
        {
            Type type = typeof(T);
            List<object> obj;
            if (listMultipeClass.TryGetValue(type.ToString(), out obj))
            {
                obj.Add(value);
            }
            else
            {
                obj = new List<object>();
                obj.Add(value);
                listMultipeClass.Add(type.ToString(), obj);
            }
        }

        /// <summary>
        ///     Gets this instance.digunakan untuk  mengambil Class Dependency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>``0.</returns>
        /// <example>
        ///     contoh yang digunakan untuk  mengambil Class Dependency
        ///     <code>
        ///  public class TaskService : BaseDependency
        ///  {        
        ///      public ITaskRepository TaskRepository { get; set; }
        ///  }
        ///  public class Main 
        ///  {        
        ///      public Main()
        ///      {
        ///          BaseDependency.Add<![CDATA[<ITaskRepository>]]>();
        ///      }
        ///      public Call()
        ///      {
        ///        var repository = BaseDependency.Get<![CDATA[<ITaskRepository>]]>();
        ///      }
        ///  }
        ///  
        /// </code>
        /// </example>
        public static T Get<T>()
        {
            var notAuthentication = true;
            try
            {
                Type type = typeof(T);
                var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var filePath = location + "\\" +
                               "CoreInjenction.diconfig";
                if (File.Exists(filePath))
                {
                    var xdocument = XElement.Load(filePath);
                    foreach (var descendant in xdocument.Descendants("dependency").Where(n => n.Attribute("contract").Value.ToString().Equals(type.FullName)))
                    {
                        try
                        {
                            if (descendant.Attribute("usecondition") != null)
                            {
                                var valid = new List<bool>();
                                foreach (var xElement in descendant.Descendants("condition"))
                                {
                                    var value = CacheHelper.GetCache(xElement.Attribute("Key").Value.ToString());
                                    if (value.ToString() != string.Empty)
                                        valid.Add(value.ToString().Equals(xElement.Attribute("Value").Value.ToString()));
                                }
                                if (valid.Count(n => n) != descendant.Descendants("condition").Count())
                                {
                                    notAuthentication = false;
                                    throw new Exception("Tidak memiliki Access");
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            Log.Error(exception);
                            throw;
                        }

                    }
                }
                object obj;
                if (listClass.TryGetValue(type.ToString(), out obj))
                {
                    if (obj is Type)
                    {
                        var data = Activator.CreateInstance((Type)obj);
                        return (T)data;
                    }
                    if (obj is T)
                        return (T)obj;

                    Log.Info(type.ToString() + " Kosong");
                    return default(T);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                if (notAuthentication)
                    return default(T);
                throw;
            }

            return default(T);
        }

        /// <summary>
        ///     Gets the specified type.digunakan untuk  mengambil Class Dependency
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        /// <example>
        ///     contoh yang digunakan untuk  mengambil Class Dependency
        ///     <code>
        ///  public class TaskService : BaseDependency
        ///  {        
        ///      public ITaskRepository TaskRepository { get; set; }
        ///  }
        ///  public class Main 
        ///  {        
        ///      public Main()
        ///      {
        ///          BaseDependency.Add<![CDATA[<ITaskRepository>]]>();
        ///      }
        ///      public Call()
        ///      {
        ///        var repository = BaseDependency.Get(Activator.CreateInstance<![CDATA[<ITaskRepository>]]>().GetType());
        ///      }
        ///  }
        ///  
        /// </code>
        /// </example>
        public static object Get(Type type)
        {
            object obj;
            if (listClass.TryGetValue(type.ToString(), out obj))
            {
                return obj;
            }
            return null;
        }

        public static IEnumerable<T> GetMultiple<T>()
        {
            Type type = typeof(T);
            List<object> obj;
            if (listMultipeClass.TryGetValue(type.ToString(), out obj))
            {
                foreach (object o in obj)
                {
                    yield return (T)o;
                }
            }
        }

        public static T GetNewInstance<T>()
        {
            Type type = typeof(T);
            object obj;
            if (listClass.TryGetValue(type.ToString(), out obj))
            {
                var objectBuild = obj;
                if (obj is Type)
                {

                }
                else
                {
                    objectBuild = obj.GetType();
                    foreach (ConstructorInfo constructorInfo in obj.GetType().GetConstructors())
                    {
                        var listType = new List<Type>();
                        var listValue = new List<object>();
                        foreach (ParameterInfo parameterInfo in constructorInfo.GetParameters())
                        {
                            listType.Add(parameterInfo.ParameterType);

                            PropertyInfo value =
                                obj.GetType()
                                    .GetProperty(
                                        parameterInfo.Name[0].ToString().ToUpper()
                                        + parameterInfo.Name.Substring(1, parameterInfo.Name.Length - 1),
                                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                        | BindingFlags.CreateInstance);
                            if (value != null)
                            {
                                listValue.Add(value.GetValue(obj, null));
                            }
                            else
                            {
                                Log.Info(type + " Tidak ada implementasi nya");
                                return default(T);
                            }
                        }
                        ConstructorInfo ctor = obj.GetType().GetConstructor(listType.ToArray());

                        return (T)ctor.Invoke(listValue.ToArray());
                    }
                }
                if (objectBuild != null)
                {
                    var data = (T)Activator.CreateInstance(objectBuild as Type);
                    return data;
                }
            }
            var itemObject = ListObjectReference.FirstOrDefault(n => n.Equals(type));
            if (itemObject == null)
            {
                Log.Info(type + " Tidak ada implementasi nya");
                return default(T);
            }
            var objectBuild2 = itemObject;
            obj = itemObject;
            if (obj is Type)
            {

            }
            else
            {
                objectBuild2 = obj.GetType();
                foreach (ConstructorInfo constructorInfo in obj.GetType().GetConstructors())
                {
                    var listType = new List<Type>();
                    var listValue = new List<object>();
                    foreach (ParameterInfo parameterInfo in constructorInfo.GetParameters())
                    {
                        listType.Add(parameterInfo.ParameterType);

                        PropertyInfo value =
                            obj.GetType()
                                .GetProperty(
                                    parameterInfo.Name[0].ToString().ToUpper()
                                    + parameterInfo.Name.Substring(1, parameterInfo.Name.Length - 1),
                                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                    | BindingFlags.CreateInstance);
                        if (value != null)
                        {
                            listValue.Add(value.GetValue(obj, null));
                        }
                        else
                        {
                            Log.Info(type + " Tidak ada implementasi nya");
                            return default(T);
                        }
                    }
                    ConstructorInfo ctor = obj.GetType().GetConstructor(listType.ToArray());

                    return (T)ctor.Invoke(listValue.ToArray());
                }
            }
            if (objectBuild2 != null)
            {
                var data = (T)Activator.CreateInstance(objectBuild2 as Type);
                return data;
            }
            Log.Info(type + " Tidak ada implementasi nya");
            return default(T);
        }

        /// <summary>
        ///     Removes this instance.digunakan untuk  menghapus Class Dependency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <example>
        ///     contoh yang digunakan untuk  menghapus Class Dependency
        ///     <code>
        ///  public class Main 
        ///  {        
        ///      public Main()
        ///      {
        ///          BaseDependency.Remove<![CDATA[<ITaskRepository>]]>();
        ///      }
        ///  }
        ///    
        /// </code>
        /// </example>
        public static void Remove<T>()
        {
            Type type = typeof(T);
            object obj;
            if (listClass.TryGetValue(type.ToString(), out obj))
            {
                listClass.Remove(type.ToString());
            }
        }

        public static void RemoveMultiple<T>()
        {
            Type type = typeof(T);
            List<object> obj;
            if (listMultipeClass.TryGetValue(type.ToString(), out obj))
            {
                listMultipeClass.Remove(type.ToString());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        protected void Initialize()
        {
            foreach (PropertyInfo type in GetType().GetProperties().Where(n => n.PropertyType.IsInterface))
            {
                object obj;
                if (listClass.TryGetValue(type.PropertyType, out obj))
                {
                    try
                    {
                        type.SetValue(this, obj, null);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        #endregion

        //private static bool valid;
        public static void ReadConfiguration(string configurationFile = "")
        {
            useLog = Get<ISetting>().GetValue("useLog");
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = location + "\\" +
                           (string.IsNullOrEmpty(configurationFile) ? "CoreInjenction.diconfig" : configurationFile);
            if (File.Exists(filePath))
            {
                if (string.IsNullOrEmpty(configurationFile))
                {
                    foreach (var file in Directory.GetFiles(location, "*.diconfig", SearchOption.AllDirectories))
                    {
                        //if (file.Contains("Jasamedika"))
                        //valid = true;
                        RegisterFile(file);


                    }
                }
                else
                {
                    RegisterFile(filePath);
                }

            }
        }

        private static void RegisterFile(string filePath)
        {
            var xdocument = XElement.Load(filePath);
            foreach (var descendant in xdocument.Descendants("dependency"))
            {
                try
                {
                    if (descendant.Attribute("usecondition") != null)
                    {
                        var valid = new List<bool>();
                        foreach (var xElement in descendant.Descendants("condition"))
                        {
                            var value = CacheHelper.GetCache(xElement.Attribute("Key").Value.ToString());
                            if (value.ToString() != string.Empty)
                                valid.Add(value.ToString().Equals(xElement.Attribute("Value").Value.ToString()));
                        }
                        if (valid.Count(n => n) == descendant.Descendants("condition").Count())
                        {
                            //if (descendant.Attribute("contract").Value.Contains("Master"))
                            //{
                            //    Debug.Print("Aha");
                            //}
                            var typeContract = HelperManager.GetType(descendant.Attribute("contract").Value);
                            //if (descendant.Attribute("contract").Value.Contains("IKartuStokRepository"))
                            //{
                            //    Debug.Print("");
                            //}
                            var implementation = HelperManager.GetType(descendant.Attribute("implementation").Value);
                            //if (implementation == null)
                            //    Log.Info(descendant.Attribute("contract").Value + " Tidak memiliki Implementation");
                            Add(typeContract, implementation);
                        }
                    }
                    else
                    {
                        //if (descendant.Attribute("contract").Value.Contains("Master"))
                        //{
                        //    Debug.Print("Aha");
                        //}
                        if (useLog.Equals("3"))
                            Log.Info("Add Contract " + descendant.Attribute("contract").Value + " - " + descendant.Attribute("implementation").Value);
                        var typeContract = HelperManager.GetType(descendant.Attribute("contract").Value);
                        //if (descendant.Attribute("contract").Value.Contains("IKartuStokRepository"))
                        //{
                        //    Debug.Print("");
                        //}
                        var implementation = HelperManager.GetType(descendant.Attribute("implementation").Value);
                        //if (implementation == null)
                        //    Log.Info(descendant.Attribute("contract").Value + " Tidak memiliki Implementation");
                        Add(typeContract, implementation);
                    }
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
            }
        }

        public static XElement SaveConfigurationFromChace()
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = location + "\\CoreInjenction.diconfig";
            var xElement = new XElement("DependencyInjections");
            foreach (var contract in HelperManager.GetTypes().Where(n => n.IsInterface && n.FullName.ToLower().Contains("medifirst")))
            {
                foreach (var type in HelperManager.GetTypes().Where(n => n.IsClass))
                {

                    var interfaces = type.GetInterfaces();
                    foreach (var @interface in interfaces)
                    {
                        if (@interface.FullName != null && @interface.FullName.Equals(contract.FullName))
                        {
                            Add(contract, type);
                        }

                    }
                }
            }
            foreach (var o in listClass.OrderBy(n => n.Key.ToString()))
            {
                var element = new XElement("dependency");
                element.Add(new XAttribute("contract", o.Key.ToString()));
                element.Add(new XAttribute("implementation", o.Value.ToString()));
                xElement.Add(element);
            }
            xElement.Save(filePath);
            return xElement;
        }

        public static string MergerConfigurationFromChace(string path, XElement elementNew)
        {

            var xPathElement = XElement.Load(path);
            var xElement = new XElement("DependencyInjections");
            foreach (var descendant in xPathElement.Descendants("dependency"))
            {
                var element = new XElement("dependency");
                if (xElement.Descendants("dependency").FirstOrDefault(n => n.Attribute("contract").Value == descendant.Attribute("contract").Value) != null) continue;
                element.Add(new XAttribute("contract", descendant.Attribute("contract").Value.ToString()));
                element.Add(new XAttribute("implementation", descendant.Attribute("implementation").Value.ToString()));
                xElement.Add(element);
            }
            foreach (var descendant in elementNew.Descendants("dependency"))
            {
                var element = new XElement("dependency");
                if (xElement.Descendants("dependency").FirstOrDefault(n => n.Attribute("contract").Value == descendant.Attribute("contract").Value) != null) continue;
                element.Add(new XAttribute("contract", descendant.Attribute("contract").Value.ToString()));
                element.Add(new XAttribute("implementation", descendant.Attribute("implementation").Value.ToString()));
                xElement.Add(element);
            }
            xElement.Save(path);
            return xElement.ToString();
        }
    }
}