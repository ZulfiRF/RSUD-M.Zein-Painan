using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Core.Framework.Helper.Logging
{
    using System;

    using Core.Framework.Helper.Contracts;

    public class Log
    {
        #region Public Methods and Operators

        public static string CurrentError;
        public static void Error(string message)
        {
            Debug.WriteLine("[" + DateTime.Now.ToString("G") + "] \t " + message);
            CurrentError = message;
            var log = BaseDependency.Get<ILogRepository>();
            if (log != null)
            {
                log.Error(message);
            }
        }

        public static void ThrowError(Exception e, string code)
        {
            Error(e);
            var error = ErrorTemplate.GetError(code);
            throw new Exception(error);
        }

        public static string ThrowGetMessageError(Exception e, string code)
        {
            Error(e);
            var error = ErrorTemplate.GetError(code);
            return error;
        }

        public static string ThrowGetMessage(string code)
        {            
            var error = ErrorTemplate.GetError(code);
            var log = BaseDependency.Get<ILogRepository>();
            if (log != null)
            {
                log.Error(error);
            }
            else
            {
                BaseDependency.Add<ILogRepository>(new LogFile());
                log = BaseDependency.Get<ILogRepository>();
                log.Error(error);
            }
            return error;
        }

        public static void Error(Exception message)
        {
            try
            {
                Debug.WriteLine("[" + DateTime.Now.ToString("G") + "] \t " + message);
                CurrentError = message.ToString();
                var log = BaseDependency.Get<ILogRepository>();
                if (log != null)
                {
                    log.Error(message.Message);
                    log.Error(message.GetBaseException().ToString());
                }
                else
                {
                    BaseDependency.Add<ILogRepository>(new LogFile());
                    log = BaseDependency.Get<ILogRepository>();
                    log.Error(message.GetBaseException().ToString());
                }
            }
            catch (Exception exception)
            {
                Error(exception);
            }

        }

        public static void Info(string message)
        {
            Debug.WriteLine("[" + DateTime.Now.ToString("G") + "] \t " + message);
            var log = BaseDependency.Get<ILogRepository>();
            if (log != null)
            {
                log.Info(message);
            }
        }

        #endregion
    }
}