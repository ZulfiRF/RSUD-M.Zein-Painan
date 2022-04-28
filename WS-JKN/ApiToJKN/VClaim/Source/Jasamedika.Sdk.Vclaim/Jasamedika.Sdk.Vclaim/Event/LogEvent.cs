using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jasamedika.Sdk.Vclaim.Event
{
    public class LogEvent : EventArgs
    {
        public string Log { get; set; }
        public LogEvent(string log)
        {
            Log = log;
        }
    }

    public class XcodeEvent : EventArgs
    {
        public string Xcode { get; set; }
        public XcodeEvent(string xcode)
        {
            Xcode = xcode;
        }
    }
}
