using System.Runtime.InteropServices;

namespace Core.Framework.Helper.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using Core.Framework.Helper.Contracts;

    public class LogFile : ILogRepository
    {
        public static readonly object objectShared = new object();
        #region Constructors and Destructors

        public LogFile()
        {
            try
            {
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\log"))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                                              "\\log");
                Path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\log\\" + DateTime.Now.ToString("yyyy-MM-dd");
                if (!File.Exists(Path))
                {
                    File.WriteAllText(Path, "");
                }
            }
            catch (Exception)
            {
            }

        }

        #endregion

        #region Public Properties

        public string Path { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Error(string message)
        {
            CallBackError(message);
        }

        public void Error(Exception message)
        {
            CallBackError(message);
        }

        public virtual void Info(string message)
        {
            try
            {
                string expectKeyword = ConfigurationManager.AppSettings["logExpectKeyword"];
                bool valid = false;
                if (expectKeyword != null)
                {
                    foreach (
                        string keyWord in
                            expectKeyword.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).AsParallel())
                    {
                        if (message.ToLower().Contains(keyWord.ToLower()))
                        {
                            valid = true;
                        }
                    }
                }
                //Console.WriteLine(message);
                if (!valid)
                {
                    ThreadPool.QueueUserWorkItem(CallBackInfo, message);
                }
            }
            catch (Exception)
            {
            }
        }

        public void Warning(string message)
        {
            try
            {
                string expectKeyword = ConfigurationManager.AppSettings["logExpectKeyword"];
                bool valid = false;
                if (expectKeyword != null)
                {
                    foreach (
                        string keyWord in
                            expectKeyword.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).AsParallel())
                    {
                        if (message.ToLower().Contains(keyWord.ToLower()))
                        {
                            valid = true;
                        }
                    }
                }
                Debug.WriteLine(message);
                if (!valid)
                {
                    ThreadPool.QueueUserWorkItem(CallBackWarning, message);
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Methods

        private void CallBackError(object state)
        {
            try
            {
                lock (objectShared)
                {
                    if (string.IsNullOrEmpty(Path)) return;
                    string[] data = File.ReadAllLines(Path);
                    var list = new List<string>(data);
                    list.Add("[Error]" + DateTime.Now + "\t\t" + state);
                    Console.WriteLine("[Error]" + DateTime.Now + "\t\t" + state);
                    File.WriteAllLines(Path, list);
                }
            }
            catch (Exception)
            {
            }
        }

        private void CallBackWarning(object state)
        {
            try
            {
                lock (objectShared)
                {
                    if (string.IsNullOrEmpty(Path)) return;
                    string[] data = File.ReadAllLines(Path);
                    var list = new List<string>(data);
                    list.Add("[Warning]" + DateTime.Now + "\t\t" + state);
                    File.WriteAllLines(Path, list);
                }
            }
            catch (Exception)
            {
            }
        }

        private void CallBackInfo(object state)
        {
            try
            {
                lock (objectShared)
                {
                    if (string.IsNullOrEmpty(Path)) return;
                    string[] data = File.ReadAllLines(Path);
                    var list = new List<string>(data);
                    list.Add("[Info]" + DateTime.Now + "\t\t" + state);
                    File.WriteAllLines(Path, list);
                }

            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }

        #endregion
    }
}