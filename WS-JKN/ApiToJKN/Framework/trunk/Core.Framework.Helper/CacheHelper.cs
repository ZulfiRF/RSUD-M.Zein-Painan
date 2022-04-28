using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Core.Framework.Helper.Logging;

namespace Core.Framework.Helper
{
    public class CacheHelper
    {
        private static Timer timer;
        private static readonly Dictionary<object, object> DictionaryChache = new Dictionary<object, object>();
        private static readonly Dictionary<Guid, KeyValuePair<object, Action<object>>> SubscribeList = new Dictionary<Guid, KeyValuePair<object, Action<object>>>();
        private static readonly Dictionary<object, DateTime> DictionaryTime = new Dictionary<object, DateTime>();


        public static Guid SubscrbeModule(object key, Action<object> invoke)
        {
            var guid = Guid.NewGuid();
            SubscribeList.Add(guid, new KeyValuePair<object, Action<object>>(key, invoke));
            return guid;
        }
        public static void UnSubscrbeModule(Guid guid)
        {
            SubscribeList.Remove(guid);
        }

        public static void RegisterCache(object key, object value)
        {
            SettingTimer();
            RegisterCache(key, value, 10);
        }
        public static void RegisterCache(object key, object value, int second)
        {



            SettingTimer();
            if (second != 0)
            {
                try
                {
                    object obj;
                    if (!DictionaryChache.TryGetValue(key, out obj))
                    {
                        if (second == -1)
                            DictionaryTime.Add(key, DateTime.MaxValue);
                        else
                            DictionaryTime.Add(key, DateTime.Now + TimeSpan.FromSeconds(second));
                        DictionaryChache.Add(key, value);
                    }
                    else
                    {
                        DictionaryChache[key] = value;
                        if (second == -1)
                            DictionaryTime[key] = DateTime.MaxValue;
                        else
                            DictionaryTime[key] = DateTime.Now + TimeSpan.FromSeconds(second);
                    }
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
                if (!stateIsCek)
                    DictionaryTimeTemp = DictionaryTime.ToDictionary(n => n.Key, n => n.Value);
            }
            try
            {
                Dictionary<Guid, KeyValuePair<object, Action<object>>> localList = new Dictionary<Guid, KeyValuePair<object, Action<object>>>(SubscribeList);
                foreach (var method in localList.Where(n => n.Value.Key.Equals(key)))
                {
                    method.Value.Value.Invoke(value);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                throw;
            }
        }

        public static Dictionary<object, DateTime> DictionaryTimeTemp { get; set; }

        public static void RemoveCache(object key)
        {
            object obj;
            if (DictionaryChache.TryGetValue(key, out obj))
                DictionaryChache.Remove(key);

            DateTime time;
            if (DictionaryTime.TryGetValue(key, out time))
                DictionaryTime.Remove(key);
        }
        public static object GetCache(object key)
        {
            try
            {
                if (key == null)
                    return "";
                object obj;
                if (DictionaryChache.TryGetValue(key, out obj))
                    return obj;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }

            return "";
        }
        public static T GetCache<T>(object key)
        {
            if (key == null)
                return default(T);
            object obj;
            if (DictionaryChache.TryGetValue(key, out obj))
                return (T)obj;
            return default(T);
        }
        private static void SettingTimer()
        {
            if (timer == null)
            {
                timer = new Timer();
                timer.Interval = 100;
                timer.Elapsed += TimerOnTick;
                timer.Start();
            }
        }
        private static readonly object ObjectReadOnly = new object();
        static bool stateIsCek;
        private static void TimerOnTick(object sender, EventArgs eventArgs)
        {
            try
            {
                lock (ObjectReadOnly)
                {
                    stateIsCek = true;
                    if (DictionaryTimeTemp != null)
                    {
                        var dicionary = new Dictionary<object, DateTime>(DictionaryTimeTemp);
                        foreach (var o in dicionary)
                        {
                            if (o.Value < DateTime.Now)
                            {
                                DictionaryChache.Remove(o.Key);
                                DictionaryTime.Remove(o.Key);
                                //Debug.WriteLine(o.Key + " has Remove From Cache");
                            }
                        }
                    }
                    stateIsCek = false;
                }

            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }
    }
}