using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Framework.Helper
{
    /// <summary>
    /// Class BaseDependency
    /// </summary>
    public class BaseDependency
    {
        /// <summary>
        /// The list class
        /// </summary>
        private static readonly Dictionary<object, object> listClass = new Dictionary<object, object>();
        private static readonly Dictionary<object, List<object>> listMultipeClass = new Dictionary<object, List<object>>();

        /// <summary>
        /// Adds the specified value.digunakan untuk  menambahkan Class Dependency      
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <example> contoh yang digunakan untuk  menambahkan Class Dependency        
        /// <code>
        /// public class TaskService : BaseDependency
        /// {        
        ///     public ITaskRepository TaskRepository { get; set; }
        /// }
        /// public class Main 
        /// {        
        ///     public Main()
        ///     {
        ///         BaseDependency.Add<![CDATA[<ITaskRepository>]]>(new TaskRepository());
        ///     }
        /// }
        ///   
        ///</code>
        ///</example>
        public static void Add<T>(T value)
        {
            var type = typeof(T);
            object obj;
            if (listClass.TryGetValue(type, out obj))
                listClass[type] = value;
            else listClass.Add(type, value);
        }


        public static void AddMultiple<T>(T value)
        {
            var type = typeof(T);
            List<object> obj;
            if (listMultipeClass.TryGetValue(type, out obj))
            {
                obj.Add(value);
            }
            else
            {
                obj = new List<object>();
                obj.Add(value);
                listMultipeClass.Add(type, obj);
            }
        }


        public static void RemoveMultiple<T>()
        {
            var type = typeof(T);
            List<object> obj;
            if (listMultipeClass.TryGetValue(type, out obj))
                listMultipeClass.Remove(type);
        }
        public static IEnumerable<T> GetMultiple<T>()
        {
            var type = typeof(T);
            List<object> obj;
            if (listMultipeClass.TryGetValue(type, out obj))
            {

                foreach (var o in obj)
                {
                    yield return (T) o;
                }
            }            
        }

        /// <summary>
        /// Removes this instance.digunakan untuk  menghapus Class Dependency        
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <example> contoh yang digunakan untuk  menghapus Class Dependency        
        /// <code>
        /// public class Main 
        /// {        
        ///     public Main()
        ///     {
        ///         BaseDependency.Remove<![CDATA[<ITaskRepository>]]>();
        ///     }
        /// }
        ///   
        ///</code>
        ///</example>
        public static void Remove<T>()
        {
            var type = typeof(T);
            object obj;
            if (listClass.TryGetValue(type, out obj))
                listClass.Remove(type);
        }

        /// <summary>
        /// Gets this instance.digunakan untuk  mengambil Class Dependency 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>``0.</returns>
        /// <example> contoh yang digunakan untuk  mengambil Class Dependency        
        /// <code>
        /// public class TaskService : BaseDependency
        /// {        
        ///     public ITaskRepository TaskRepository { get; set; }
        /// }
        /// public class Main 
        /// {        
        ///     public Main()
        ///     {
        ///         BaseDependency.Add<![CDATA[<ITaskRepository>]]>();
        ///     }
        ///     public Call()
        ///     {
        ///       var repository = BaseDependency.Get<![CDATA[<ITaskRepository>]]>();
        ///     }
        /// }
        /// 
        ///</code>
        ///</example>
        public static T Get<T>()
        {
            var type = typeof(T);
            object obj;
            if (listClass.TryGetValue(type, out obj))
            {
                return (T)obj;
            }
            return default(T);
        }
        public static T GetNewInstance<T>()
        {
            var type = typeof(T);
            object obj;
            if (listClass.TryGetValue(type, out obj))
            {
                foreach (var constructorInfo in obj.GetType().GetConstructors())
                {
                    var listType = new List<Type>();
                    var listValue = new List<object>();
                    foreach (var parameterInfo in constructorInfo.GetParameters())
                    {
                        listType.Add(parameterInfo.ParameterType);

                        var value = obj.GetType().GetProperty(
                            parameterInfo.Name[0].ToString().ToUpper() +
                            parameterInfo.Name.Substring(1, parameterInfo.Name.Length - 1),
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.CreateInstance);
                        if (value != null)
                        {
                            listValue.Add(value.GetValue(obj, null));
                        }
                        else
                        {
                            return default(T);
                        }
                    }
                    ConstructorInfo ctor = obj.GetType().GetConstructor(listType.ToArray());

                    return (T)ctor.Invoke(listValue.ToArray());
                }
                var data = (T)Activator.CreateInstance(obj.GetType());
                return data;
            }
            return default(T);
        }
        /// <summary>
        /// Gets the specified type.digunakan untuk  mengambil Class Dependency   
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        /// <example> contoh yang digunakan untuk  mengambil Class Dependency        
        /// <code>
        /// public class TaskService : BaseDependency
        /// {        
        ///     public ITaskRepository TaskRepository { get; set; }
        /// }
        /// public class Main 
        /// {        
        ///     public Main()
        ///     {
        ///         BaseDependency.Add<![CDATA[<ITaskRepository>]]>();
        ///     }
        ///     public Call()
        ///     {
        ///       var repository = BaseDependency.Get(Activator.CreateInstance<![CDATA[<ITaskRepository>]]>().GetType());
        ///     }
        /// }
        /// 
        ///</code>
        ///</example>
        public static object Get(Type type)
        {
            object obj;
            if (listClass.TryGetValue(type, out obj))
            {
                return obj;
            }
            return null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDependency"/> class.
        /// </summary>
        public BaseDependency()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected void Initialize()
        {
            foreach (var type in GetType().GetProperties().Where(n => n.PropertyType.IsInterface))
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
    }
}