using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using Core.Framework.Helper;

namespace Core.Framework.Model
{
    public class BaseItem
    {
        /// <summary>
        ///     Gets or sets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        private bool isNew;

        /// <summary>
        ///     The manager
        /// </summary>
        private ContextManager manager;

        /// <summary>
        ///     Gets or sets the dictionarys.
        /// </summary>
        /// <value>The dictionarys.</value>
        protected internal CoreDictionary<string, object> Dictionarys { get; set; }

        /// <summary>
        ///     Gets or sets the dictionary reference.
        /// </summary>
        /// <value>The dictionary reference.</value>
        protected static Dictionary<string, object> DictionaryReference { get; set; }

        public bool IsNew
        {
            get { return isNew; }
            set
            {
                isNew = value;
                if (isNew == false)
                    OnPropertyChanged();
            }
        }

        protected internal virtual ContextManager Manager
        {
            get { return manager; }
            set
            {
                manager = value;
                OnChangeManager(new ItemEventArgs<ContextManager>(manager));
            }
        }

        /// <summary>
        ///     Gets or sets the GUID id.
        /// </summary>
        /// <value>The GUID id.</value>
        internal string GuidId { get; set; }

        protected bool IsUpdate { get; set; }

        /// <summary>
        ///     Gets or sets the previous.
        /// </summary>
        /// <value>The previous.</value>
        protected internal CoreDictionary<string, object> Previous { get; set; }

        public virtual string GetTableName()
        {
            return "";
        }

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<ItemEventArgs<ContextManager>> ChangeManager;

        protected void OnChangeManager(ItemEventArgs<ContextManager> e)
        {
            var handler = ChangeManager;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        ///     Gets or sets the manager.
        /// </summary>
        /// <value>The manager.</value>
        public ContextManager GetContextManager()
        {
            return manager;
        }

        public void SetContextManager(ContextManager contextManager)
        {
            Manager = contextManager;
        }

        /// <summary>
        ///     Called when [property changed].
        /// </summary>
        /// <param name="name">The name.</param>
        protected void OnPropertyChanged(string name = "")
        {
            if (PropertyChanged != null && !string.IsNullOrEmpty(name))
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            if (Manager != null)
                if (!IsNew)
                {
                    if (GuidId == null)
                        GuidId = Guid.NewGuid().ToString();
                    Manager.FreezeUpdate = true;
                    Manager.Update(this);
                    IsUpdate = true;
                    Manager.FreezeUpdate = false;
                }
        }

        /// <summary>
        ///     Gets or sets the <see cref="System.Object" /> with the specified key. digunakan untuk mengambil data dari
        ///     dictionary ajar lebih optimize
        /// </summary>
        /// <param name="key">berisikan proeprty atau key yang ada di object atau dictionary</param>
        /// <returns>System.Object.</returns>
        /// <example>
        ///     contoh yang digunakan untuk mengambil satu data dari database
        ///     <code>
        ///  var context = new DomainContext(connectionString);
        ///  var oneData = context.ContactPeopleInImmediatelies.FirstOrDefault();
        ///  var ID =oneData["ID"];
        /// </code>
        /// </example>
        public void OnInit(IDataReader read, ContextManager contextManager)
        {
            IsNew = false;
            if (Dictionarys == null)
                Dictionarys = new CoreDictionary<string, object>();
            if (Previous == null)
                Previous = new CoreDictionary<string, object>();
            Manager = contextManager;
            for (var i = 0; i < read.FieldCount; i++)
            {
                if (read[i] is SByte)
                {
                    object result;
                    if (!Dictionarys.TryGetValue(read.GetName(i).ToLower(), out result))
                    {
                        Dictionarys.Add(read.GetName(i).ToLower(), Convert.ToByte(read[i]));
                        //OnPropertyChanged(read.GetName(i));
                    }
                    if (!Previous.TryGetValue(read.GetName(i).ToLower(), out result))
                        Previous.Add(read.GetName(i).ToLower(), Convert.ToByte(read[i]));
                }
                else
                {
                    object result;
                    if (!Dictionarys.TryGetValue(read.GetName(i).ToLower(), out result))
                    {
                        Dictionarys.Add(read.GetName(i).ToLower(), read[i]);
                        //OnPropertyChanged(read.GetName(i));
                    }
                    if (!Previous.TryGetValue(read.GetName(i).ToLower(), out result))
                        Previous.Add(read.GetName(i).ToLower(), read[i]);
                }
            }
        }
    }
}