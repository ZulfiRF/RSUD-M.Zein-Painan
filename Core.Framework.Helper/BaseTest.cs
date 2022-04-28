using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;
using Core.Framework.Helper.Attributes;
using Core.Framework.Helper.Logging;

namespace Core.Framework.Helper
{
    public class WriterOutput : TextWriter
    {
        private readonly Action<char> action;
        readonly StringBuilder stringBuilder = new StringBuilder();
        readonly Timer timer = new Timer();
        public WriterOutput()
        {
            //timer.Elapsed += TimerOnElapsed;
            //timer.Interval = 1000;
        }

        public WriterOutput(Action<char> action)
            : this()
        {
            action = action;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Log.Info(stringBuilder.ToString());
            stringBuilder.Clear();
        }
        public override void Write(char value)
        {
            base.Write(value);
            stringBuilder.Append(value.ToString());
            timer.Stop();
            timer.Start();
            if (action != null) action.Invoke(value);
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
    public class BaseTest
    {
        public BaseTest()
        {

        }
        public enum StatusChangeType
        {
            New,
            Add,
            Changes
        }
        public class ChangeItem
        {
            public KeyValuePair<string, object> Data { get; set; }
            public StatusChangeType Status { get; set; }
        }
        public event EventHandler<ItemEventArgs<string>> Initialize;
        public event EventHandler<ItemEventArgs<ChangeItem>> KeyChange;

        protected virtual void OnKeyChange(ItemEventArgs<ChangeItem> e)
        {
            EventHandler<ItemEventArgs<ChangeItem>> handler = KeyChange;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnInitialize(ItemEventArgs<string> e)
        {
            EventHandler<ItemEventArgs<string>> handler = Initialize;
            if (handler != null) handler(this, e);
        }

        protected CoreDictionary<string, object> InitializeData(MethodBase initalizeData)
        {
            OnInitialize(new ItemEventArgs<string>(DateTime.Now + "\t\t" + initalizeData.Name + " Execute"));

            var result = initalizeData.GetCustomAttributes(true)
                .OfType<InputTestAttribute>()
                .ToCoreDictionary(n => n.Name, m => m.Value);
            foreach (var o in result)
                OnKeyChange(new ItemEventArgs<ChangeItem>(new ChangeItem()
                {
                    Data = o,
                    Status = StatusChangeType.New
                }));
            return result;
        }
        protected CoreDictionary<string, object> CompareData(CoreDictionary<string, object> param, CoreDictionary<string, object> input)
        {
            if (input == null) return param;
            foreach (var o in input)
            {
                OnKeyChange(new ItemEventArgs<ChangeItem>(new ChangeItem()
                {
                    Data = o,
                    Status = StatusChangeType.Changes
                }));
                DateTime date;
                if (o.Value != null && DateTime.TryParse(o.Value.ToString(), out date))
                    param[o.Key] = date;
                else
                    param[o.Key] = o.Value;
            }
            return param;
        }
    }
}