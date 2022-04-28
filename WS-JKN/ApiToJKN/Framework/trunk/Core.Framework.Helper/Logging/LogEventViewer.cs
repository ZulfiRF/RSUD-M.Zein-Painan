using System;
using System.Diagnostics;
using Core.Framework.Helper.Contracts;

namespace Core.Framework.Helper.Logging
{
    public class LogEventViewer : ILogRepository
    {
        private readonly System.Diagnostics.EventLog SQLEventLog;
        public LogEventViewer(string source = null)
        {
            try
            {


                var log = "Core Framework";
                if (source != null)
                    log = source;
                if (!System.Diagnostics.EventLog.Exists(log))
                    System.Diagnostics.EventLog.CreateEventSource(log, log);
                SQLEventLog =
                    new System.Diagnostics.EventLog();

                SQLEventLog.Source = log;
                SQLEventLog.Log = log;

                SQLEventLog.Source = log;
                SQLEventLog.WriteEntry("The " + log + " was successfully initialize component.", EventLogEntryType.Information);
            }
            catch (Exception)
            {
            }

        }
        public void Error(string message)
        {
            try
            {
                SQLEventLog.WriteEntry("[" + DateTime.Now + "]" + message, EventLogEntryType.Error);

            }
            catch (Exception)
            {
            }
        }

        public void Error(Exception message)
        {
            try
            {
                SQLEventLog.WriteEntry("[" + DateTime.Now + "]" + message.ToString(), EventLogEntryType.Error);

            }
            catch (Exception)
            {
            }
        }

        public void Info(string message)
        {
            try
            {
                SQLEventLog.WriteEntry("[" + DateTime.Now + "]" + message, EventLogEntryType.Information);

            }
            catch (Exception)
            {
            }
        }

        public void Warning(string message)
        {
            SQLEventLog.WriteEntry("[" + DateTime.Now + "]" + message, EventLogEntryType.Warning);
        }
    }
}