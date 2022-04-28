using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Core.Framework.Helper;
using Core.Framework.Helper.Collections;
using Core.Framework.Helper.Logging;
using Core.Framework.Model.Attr;

namespace Core.Framework.Model
{
    //public class ViewItem :INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    protected virtual void OnPropertyChanged(string propertyName)
    //    {
    //        PropertyChangedEventHandler handler = this.PropertyChanged;
    //        if (handler != null)
    //        {
    //            handler(this, new PropertyChangedEventArgs(propertyName));
    //        }
    //    }
    //}
    /// <summary>
    ///     Class TableItem
    /// </summary>
    [Serializable]
    public class TableItem : BaseItem, INotifyPropertyChanged, ILoadModel
    {
        public static List<object> currentDomain = new List<object>();
        private static List<Type> dictionary;
        public event EventHandler<ItemEventArgs<KeyValuePair<string, object>>> BeforeGetReference;
        private string tableName;

        public object Tag { get; set; }

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="TableItem" /> class.
        /// </summary>
        public TableItem()
        {
            IdPrimary = Guid.NewGuid().ToString();
            IsNew = true;
            if (dictionary == null)
            {
                dictionary = new List<Type>
                {
                    typeof (byte),
                    typeof (Int16),
                    typeof (Int32),
                    typeof (Int64),
                    typeof (Decimal),
                    typeof (float),
                    typeof (double),
                    typeof (string),
                    typeof (DateTime),
                    typeof (byte?),
                    typeof (Int16?),
                    typeof (Int32?),
                    typeof (Int64?),
                    typeof (Decimal?),
                    typeof (float?),
                    typeof (double?),
                    typeof (DateTime?)
                };
            }
        }

        #endregion Constructor

        //public override bool Equals(object obj)
        //{
        //    if (Dictionarys == null) return false;
        //    if (obj is TableItem)
        //    {
        //        var currentObject = obj as TableItem;
        //        var valid = false;
        //        foreach (var primaryKey in currentObject.PrimaryKeys)
        //        {
        //            try
        //            {
        //                var result =
        //                    currentObject.GetType().GetProperty(primaryKey).GetValue(currentObject, null).Equals(
        //                        this.GetType().GetProperty(primaryKey).GetValue(this, null));
        //                if (!result) return false;
        //            }
        //            catch (Exception)
        //            {
        //                return false;
        //            }
        //        }
        //        return true;
        //        //if (currentObject.Dictionarys == null) return false;
        //        //foreach (var dictionaryData in Dictionarys)
        //        //{
        //        //    object result;
        //        //    if (!currentObject.Dictionarys.TryGetValue(dictionaryData.Key, out result))
        //        //    {
        //        //        return false;
        //        //    }
        //        //    if (currentObject[dictionaryData.Key] != null && dictionaryData.Value != null)
        //        //    {
        //        //        if (dictionaryData.Value.Equals("null"))
        //        //        {
        //        //            if (!string.IsNullOrEmpty(currentObject[dictionaryData.Key].ToString())) 
        //        //                return false;
        //        //        }
        //        //        else
        //        //            if (!currentObject[dictionaryData.Key].Equals(dictionaryData.Value)) 
        //        //                return false;
        //        //    }
        //        //}
        //    }
        //    return false;
        //}

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is freeze.
        /// </summary>
        /// <value><c>true</c> if this instance is freeze; otherwise, <c>false</c>.</value>
        protected bool IsFreeze { get; set; }

        /// <summary>
        ///     Gets the fields.
        /// </summary>
        /// <value>The fields.</value>
        protected internal IEnumerable<FieldAttribute> Fields
        {
            get
            {
                // if (fields == null)
                {
                    var list = new List<FieldAttribute>();
                    foreach (var property in GetType().GetProperties(
                        BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.Public).Where(
                            n =>
                                n.GetCustomAttributes(true).OfType<FieldAttribute>().Count() != 0 ||
                                n.GetCustomAttributes(true).OfType<ReferenceAttribute>().Count() != 0)
                        .OrderBy(n => n.Name))
                    {
                        var temp =
                            property.GetCustomAttributes(true).OfType<FieldAttribute>().FirstOrDefault();
                        if (temp == null)
                        {
                            var tempRef =
                                property.GetCustomAttributes(true).OfType<ReferenceAttribute>().FirstOrDefault();
                            if (tempRef == null) continue;
                            temp = new FieldAttribute(property.Name) { IsReference = true };
                        }
                        temp.Info = property;
                        list.Add(temp);
                    }
                    fields = list;
                }
                return fields;
            }
        }

        /// <summary>
        ///     Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        protected internal TableItem Parent
        {
            get { return parent; }

            set
            {
                parent = value;
                OnPropertyChanged("Parent");
            }
        }

        /// <summary>
        ///     Gets the primary keys.
        /// </summary>
        /// <value>The primary keys.</value>
        protected internal virtual IEnumerable<string> PrimaryKeys
        {
            get
            {
                return primaryKeys ??
                       (primaryKeys =
                           GetType().GetProperties().Where(
                               n => n.GetCustomAttributes(true).OfType<FieldAttribute>().Count(c => c.IsPrimary) != 0).
                               Select(
                                   s => s.GetCustomAttributes(true).OfType<FieldAttribute>().FirstOrDefault().FieldName)
                               .ToArray());
            }
        }

        /// <summary>
        ///     Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        /// <exception cref="System.ArgumentNullException">Table Name Kosong</exception>
        public string TableName
        {
            get
            {
                var type = GetType();
                if (!string.IsNullOrEmpty(tableName)) return tableName;
                var firstOrDefault =
                    type.GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
                if (firstOrDefault != null)
                    return firstOrDefault.TabelName;
                return type.Name;
            }
            protected set { tableName = value; }
        }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        protected internal RowType Type { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [will commit].
        /// </summary>
        /// <value><c>true</c> if [will commit]; otherwise, <c>false</c>.</value>
        protected internal bool WillCommit { get; set; }

        /// <summary>
        ///     Gets the auto drop table.
        /// </summary>
        /// <value>The auto drop table.</value>
        public UpdateTableType AutoDropTable
        {
            get
            {
                var firstOrDefault =
                    GetType().GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
                if (firstOrDefault != null)
                    return firstOrDefault.AutoDrop;
                //  throw new ArgumentNullException("Table Name Kosong");
                return UpdateTableType.NothingAction;
            }
        }

        public virtual bool IsTransaction
        {
            get { return false; }
        }

        public virtual bool IsRebindFromServer { get; set; }

        public bool IsAutoIncrement
        {
            get
            {
                var firstOrDefault =
                    GetType().GetCustomAttributes(true).OfType<TableAutoIncrement>().FirstOrDefault();
                if (firstOrDefault != null)
                {
                    return true;
                }
                return false;
            }
        }

        internal string IdPrimary { get; set; }

        public override string GetTableName()
        {
            return TableName;
        }

        /// <summary>
        ///     Creates the table.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        protected internal bool CreateTable()
        {
            return true;
        }

        /// <summary>
        ///     Clones this instance.
        /// </summary>
        /// <returns>System.Object.</returns>
        public virtual object Clone()
        {
            return this;
        }

        /// <summary>
        ///     Clones this instance.
        /// </summary>
        /// <returns>System.Object.</returns>
        public virtual object Clone(IDictionary<string, object> cloneDictionary)
        {
            Dictionarys = new CoreDictionary<string, object>(cloneDictionary);
            return this;
        }

        public void InitializeManager(ContextManager contextManager)
        {
            Manager = contextManager;
        }

        public void RebindDataToPrevious()
        {
            Previous = new CoreDictionary<string, object>(Dictionarys);
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            if (Dictionarys == null) return base.ToString();
            var i = 0;
            result.Append("[" + TableName + "]");
            var searchDisplay =
                Fields.FirstOrDefault(n => n.Info.GetCustomAttributes(true).OfType<SearchAttribute>().Any());
            if (searchDisplay != null)
            {
                var title = searchDisplay.Info.GetValue(this, null);
                if (title != null)
                    return title.ToString();
            }
            foreach (var primaryKey in PrimaryKeys)
            {
                if (i == 0)
                    result.Append(primaryKey + " -- " + Dictionarys[primaryKey.ToLower()] + "");
                else
                    result.Append(primaryKey + " -- " + Dictionarys[primaryKey.ToLower()]);
            }
            return result.ToString();
        }

        #region Local Variable

        /// <summary>
        ///     The fields
        /// </summary>
        private IEnumerable<FieldAttribute> fields;

        /// <summary>
        ///     The parent
        /// </summary>
        private TableItem parent;

        /// <summary>
        ///     The primary keys
        /// </summary>
        private IEnumerable<string> primaryKeys;

        #endregion Local Variable

        #region Protected

        private IDataRecord dataRecord;
        private bool isLoad;
        protected Guid GuidNewData { get; set; }


        /// <summary>
        ///     Gets or sets the tag after insert.
        /// </summary>
        /// <value>The tag after insert.</value>
        protected internal List<object> TagAfterInsert { get; set; }

        /// <summary>
        ///     Gets or sets the tag before insert.
        /// </summary>
        /// <value>The tag before insert.</value>
        protected internal List<object> TagBeforeInsert { get; set; }

        /// <summary>
        ///     Gets or sets the GUID id.
        /// </summary>
        /// <value>The GUID id.</value>
        public virtual object this[string key]
        {
            get
            {
                object obj = null;
                try
                {
                    var temp = key.ToLower();
                    //if (tempObj.ContainsKey(key))
                    //    return tempObj[key];
                    if (Dictionarys != null)
                        Dictionarys.TryGetValue(temp, out obj);
                    //if (Skip && obj != null)
                    //{
                    //    if (obj == DBNull.Value)
                    //        return SelectFunction.Sum;
                    //    return obj;
                    //}
                    FieldAttribute typeProperty;
                    if (Dictionarys == null)
                    {
                        typeProperty = Fields.FirstOrDefault(n => n.FieldName.ToLower().Equals(temp));
                        if (typeProperty == null)
                        {
                            obj = null;
                            return null;
                        }
                        var customAttribute =
                            typeProperty.Info.GetCustomAttributes(true).OfType<DefaultValueAttribute>().FirstOrDefault();
                        if (customAttribute != null)
                        {
                            if (obj == null)
                            {
                                obj = DefaultValue(customAttribute);
                                return obj;
                            }
                        }

                        if (typeProperty.Info.PropertyType.IsValueType)
                        {
                            obj = Activator.CreateInstance(typeProperty.Info.PropertyType);
                            return obj;
                        }
                        obj = null;
                        return null;
                    }

                    //if (obj == null) return SelectFunction.Sum;
                    //return obj;
                    //try
                    //{
                    //    if (obj != null && obj.ToString().Equals("null"))
                    //    {
                    //        return string.Empty;
                    //    }
                    //    if (obj != null && obj.ToString().Contains("="))
                    //    {
                    //        var firstOrDefault =
                    //            Fields.FirstOrDefault(
                    //                n =>
                    //                n.FieldName.Equals(key) &&
                    //                n.Info.GetCustomAttributes(true).OfType<EncrypteAttribute>().Count() != 0);
                    //        if (firstOrDefault != null)
                    //        {
                    //            var fieldEncrypte = firstOrDefault.Info;
                    //            if (fieldEncrypte != null && !IsNew)
                    //            {
                    //                var encrypteAttribute = fieldEncrypte.GetCustomAttributes(true).OfType<EncrypteAttribute>().FirstOrDefault();
                    //                var descrypte = obj.ToString();
                    //                var password = "";
                    //                if (encrypteAttribute != null)
                    //                {
                    //                    if (encrypteAttribute.RelationProperty != null)
                    //                    {
                    //                        foreach (var relation in encrypteAttribute.RelationProperty)
                    //                        {
                    //                            if (Previous != null)
                    //                                password += (Previous[relation.ToLower()] != null
                    //                                                 ? password + Previous[relation.ToLower()]
                    //                                                 : password +
                    //                                                   Fields.FirstOrDefault(
                    //                                                       n => n.FieldName.Equals(relation)).Info.
                    //                                                       GetValue(this, null));
                    //                            else
                    //                                password +=
                    //                                    Fields.FirstOrDefault(n => n.FieldName.Equals(relation)).Info.
                    //                                        GetValue(this, null).ToString();
                    //                        }
                    //                    }

                    //                    if (encrypteAttribute.Aesblock == null && encrypteAttribute.Aeskey == null)
                    //                    {
                    //                        descrypte = descrypte.Decrypt(encrypteAttribute.Key + password);
                    //                    }
                    //                    else
                    //                    {
                    //                        descrypte = Cryptography.FuncAesDecrypt(descrypte);
                    //                    }
                    //                    //password = encrypteAttribute.RelationProperty.Aggregate(password, (current, relation) => ((Previous[relation] != null) ? current + Previous[relation] : current + Fields.FirstOrDefault(n => n.FieldName.Equals(relation)).Info.GetValue(this, null).ToString()));

                    //                }

                    //                return descrypte;
                    //            }
                    //        }
                    //    }
                    //}
                    //catch (Exception)
                    //{
                    //    return null;
                    //}
                    Dictionarys.TryGetValue(temp, out obj);
                    //return Convert.ChangeType(obj.ToString(), Fields.FirstOrDefault(n => n.FieldName.Equals(key)).Info.PropertyType);
                    if (obj == DBNull.Value)
                    {
                        obj = null;
                        return null;
                    }
                    if (obj == null)
                    {
                        typeProperty = Fields.FirstOrDefault(n => n.FieldName.ToLower().Equals(temp));
                        if (typeProperty == null)
                        {
                            return null;
                        }
                        if (Nullable.GetUnderlyingType(typeProperty.Info.PropertyType) != null)
                        {
                            return null;
                        }
                        var customAttribute =
                            typeProperty.Info.GetCustomAttributes(true).OfType<DefaultValueAttribute>().FirstOrDefault();
                        if (customAttribute != null)
                        {
                            obj = DefaultValue(customAttribute);
                            return obj;
                        }

                        if (typeProperty.Info.PropertyType.IsValueType)
                        {
                            if (dictionary.All(n => n != typeProperty.Info.PropertyType))
                                return Activator.CreateInstance(typeProperty.Info.PropertyType);
                            if (typeProperty.IsAllowNull == SpesicicationType.NotAllowNull)
                            {
                                try
                                {
                                    obj = Activator.CreateInstance(typeProperty.Info.PropertyType);
                                    return obj;
                                }
                                catch (Exception exception)
                                {
                                    Log.Error(exception);
                                }
                            }
                        }
                        var skipAttribute =
                            typeProperty.Info.GetCustomAttributes(true).OfType<SkipAttribute>().FirstOrDefault();
                        if (skipAttribute != null)
                        {
                            if (typeProperty.Info.PropertyType.FullName.ToLower().Contains("string"))
                                obj = "";
                            return "";
                        }
                    }
                    typeProperty = Fields.FirstOrDefault(n => n.FieldName.ToLower().Equals(temp));
                    if (typeProperty != null && typeProperty.Info.PropertyType.Name.Contains("Char"))
                        return (obj != null) ? obj.ToString()[0] : char.MaxValue;
                    if (typeProperty != null && typeProperty.IsReference)
                    {
                        if (typeProperty.Info.PropertyType.IsClass)
                        {
                            var data = typeProperty.Info.GetValue(this, null);
                            obj = data;
                            return data;
                        }
                    }
                    return obj;
                }
                finally
                {
                    lock (this)
                    {
                        object data;
                        if (!tempObj.TryGetValue(key, out data))
                            tempObj.Add(key, obj);
                    }
                }
            }

            set
            {
                if (Dictionarys == null)
                    Dictionarys = new CoreDictionary<string, object>();
                object val;
                if (tempObj.ContainsKey(key))
                    tempObj.Remove(key);
                var temp = key.ToLower();
                if (!Dictionarys.TryGetValue(temp, out val))
                    Dictionarys.Add(temp, value);
                else
                    Dictionarys[temp] = value;

                // if (!IsNew)
                OnPropertyChanged(key);
            }
        }

        private readonly Dictionary<string, object> tempObj = new Dictionary<string, object>();

        public void OnInitLoad(IDataRecord read)
        {
            IsNew = false;
            if (Dictionarys == null)
                Dictionarys = new CoreDictionary<string, object>();
            if (Previous == null)
                Previous = new CoreDictionary<string, object>();
            Manager = ContextManager.Current;
            dataRecord = read;
        }

        public bool IsLoad
        {
            get { return isLoad; }
            set
            {
                isLoad = value;
                if (value)
                {
                    try
                    {
                        if (dataRecord != null)
                            for (var i = 0; i < dataRecord.FieldCount; i++)
                            {
                                if (dataRecord[i] is SByte)
                                {
                                    object result;
                                    if (!Dictionarys.TryGetValue(dataRecord.GetName(i).ToLower(), out result))
                                        Dictionarys.Add(dataRecord.GetName(i).ToLower(), Convert.ToByte(dataRecord[i]));
                                    if (!Previous.TryGetValue(dataRecord.GetName(i).ToLower(), out result))
                                        Previous.Add(dataRecord.GetName(i).ToLower(), Convert.ToByte(dataRecord[i]));
                                }
                                else
                                {
                                    object result;
                                    if (!Dictionarys.TryGetValue(dataRecord.GetName(i).ToLower(), out result))
                                        Dictionarys.Add(dataRecord.GetName(i).ToLower(), dataRecord[i]);
                                    if (!Previous.TryGetValue(dataRecord.GetName(i).ToLower(), out result))
                                        Previous.Add(dataRecord.GetName(i).ToLower(), dataRecord[i]);
                                }
                            }
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception);
                    }
                }
            }
        }

        public bool Skip { get; set; }

        /// <summary>
        ///     Converts the char. digunakan untuk merubah object to char
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Char.</returns>
        protected char ConvertChar(object value)
        {
            if (value == null)
                return char.MinValue;
            if (value is char)
                return value.ToString()[0];
            return char.MinValue;
        }

        /// <summary>
        ///     Converts the bool. digunakan untuk merubah ke bool
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     <c>true</c> jika value bernilai true atau value bernilai 0<c>false</c> jika value bernilai false atau value
        ///     bernilai selain  0
        /// </returns>
        protected bool ConvertBool(object value)
        {
            if (value == null)
                return false;
            if (value is bool)
                return (bool)value;
            return Convert.ToInt16(value) == 0;
        }

        /// <summary>
        ///     Bindings the property string. digunakan untuk validasi inputan yang di kirim
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <param name="allowCharacterSpesial">The allow character spesial.</param>
        /// <exception cref="System.ArgumentException">
        ///     Panjang Karakter  + property +  melebihi batas
        ///     or
        ///     Value  + property +  hanya boleh Angka
        ///     or
        ///     Value  + property +  hanya boleh Angka dan Character
        ///     or
        ///     Value  + property +  tidak boleh mengandung Charackter Spesial
        ///     or
        ///     Value  + property +  tidak boleh mengandung Charackter Spesial selain '_.-'
        /// </exception>
        protected void BindingPropertyString(string property, string value,
            TypeValueString allowCharacterSpesial = TypeValueString.Text)
        {
            var propertyInfo = GetType().GetProperty(property,
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance);
            var atribute =
                propertyInfo.GetCustomAttributes(true).FirstOrDefault(n => n is FieldAttribute) as FieldAttribute;
            this[property] = value;
            if (atribute != null && atribute.Length > 0)
                if (value.Length > atribute.Length)
                    throw new ArgumentException("Panjang Karakter " + property + " melebihi batas");
            Regex regexItem;
            switch (allowCharacterSpesial)
            {
                case TypeValueString.Number:
                    regexItem = new Regex("^[0-9]*$");
                    if (!regexItem.IsMatch(value))
                    {
                        throw new ArgumentException("Value " + property + " hanya boleh Angka");
                    }
                    break;

                case TypeValueString.NumberWithCharacter:
                    regexItem = new Regex("^[0-9 -]*$");
                    if (!regexItem.IsMatch(value))
                    {
                        throw new ArgumentException("Value " + property + " hanya boleh Angka dan Character");
                    }
                    break;

                case TypeValueString.Text:
                    break;

                case TypeValueString.UpperText:
                    this[property] = value.ToUpper();
                    break;

                case TypeValueString.LowerText:
                    this[property] = value.ToLower();
                    break;

                case TypeValueString.OnlyTextAndNumber:
                    regexItem = new Regex("^[a-zA-Z0-9 ]*$");
                    if (!regexItem.IsMatch(value))
                    {
                        throw new ArgumentException("Value " + property + " tidak boleh mengandung Charackter Spesial");
                    }
                    break;

                case TypeValueString.TextWithoutCharacterSpecial:
                    regexItem = new Regex("^[a-zA-Z0-9 _.-]*$");
                    if (!regexItem.IsMatch(value))
                    {
                        throw new ArgumentException("Value " + property +
                                                    " tidak boleh mengandung Charackter Spesial selain '_.-' ");
                    }
                    break;
            }
        }


        public void OnInit(IDataRecord read, ContextManager contextManager)
        {
            IsNew = false;
            if (Dictionarys == null)
                Dictionarys = new CoreDictionary<string, object>();
            if (Previous == null)
                Previous = new CoreDictionary<string, object>();
            Manager = contextManager;
            dataRecord = read;
            IsLoad = true;
        }


        /// <summary>
        ///     Called when [init].
        /// </summary>
        public virtual void OnInit()
        {
            foreach (var fieldAttribute in Fields)
            {
                if (fieldAttribute.IsAllowNull == SpesicicationType.NotAllowNull)
                {
                    if (fieldAttribute.Info == null) continue;
                    var obj = fieldAttribute.Info.GetValue(this, null);
                    if (obj == null)
                    {
                        var customAttribute =
                            fieldAttribute.Info.GetCustomAttributes(true).OfType<DefaultValueAttribute>().FirstOrDefault
                                ();
                        if (customAttribute != null)
                        {
                            if (fieldAttribute.Info.PropertyType.ToString().Contains("DateTime") &&
                                customAttribute.Value.ToString().ToLower().Equals("now"))
                            {
                                if (!fieldAttribute.Info.GetCustomAttributes(true).Any(n => n is SkipAttribute))
                                    fieldAttribute.Info.SetValue(this, DateTime.Now, null);
                            }
                            else
                                fieldAttribute.Info.SetValue(this, customAttribute.Value, null);
                        }
                    }
                }
            }
            // InitOnLoad.Initialise(this);
        }

        public virtual void OnInitByPass(Dictionary<string, object> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                var key = data.Select(n => n.Key).ToArray()[i];
                var dict = new KeyValuePair<string, object>(key, data[key]);
                object result = null;
                //OnInit();
                if (Dictionarys == null)
                    Dictionarys = new CoreDictionary<string, object>();
                if (!Dictionarys.TryGetValue(dict.Key.ToLower(), out result))
                {
                    Dictionarys.Add(key, data[key]);
                }
                else
                    Dictionarys[key] = data[key];
            }
        }

        /// <summary>
        ///     Called when [init]. digunkan untuk mingisi object melalui dictionaryData
        /// </summary>
        /// <param name="dictionaryData">The dictionaryData.</param>
        /// <param name="isUpdate"> </param>
        /// <param name="byPassData"></param>
        public virtual void OnInit(Dictionary<string, object> dictionaryData, bool isUpdate = false, bool byPassData = false)
        {
            if (Manager != null)
                Manager.FreezeUpdate = false;

            OnInit();
            if (Dictionarys == null)
                Dictionarys = new CoreDictionary<string, object>();
            if (!IsNew)
            {
                OnPropertyChanged();
            }
            //Parallel.For(0, dictionaryData.Count, delegate(int i)
            for (var i = 0; i < dictionaryData.Count; i++)
            {
                try
                {
                    var key = dictionaryData.Select(n => n.Key).ToArray()[i];
                    var dict = new KeyValuePair<string, object>(key, dictionaryData[key]);
                    object result;
                    object valueObject = null; //Edit 25 Agustus 2014
                    if (dict.Value != null) valueObject = dict.Value;
                    object objectBefore;
                    if (Previous != null && Previous.TryGetValue(dict.Key.ToLower(), out objectBefore))
                    {
                        if (objectBefore != null && objectBefore.Equals(dict.Value))
                            //return;
                            continue;
                    }
                    //var propertyInfo = GetType().GetProperty(dict.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (byPassData)
                    {
                        if (!Dictionarys.TryGetValue(dict.Key.ToLower(), out result))
                        {
                            Dictionarys.Add(key, dictionaryData[key]);
                        }
                        else
                            Dictionarys[key] = dictionaryData[key];
                        //continue;
                    }

                    if (Fields.Any())
                    {
                        PropertyInfo propertyInfo = null;
                        foreach (var n in Fields)
                        {
                            if (
                            (n.FieldName != null && n.FieldName.ToLower().Equals(dict.Key.ToLower()))
                                || (n.Info != null && n.Info.Name.ToLower().Equals(dict.Key.ToLower()))
                            )
                                propertyInfo = n.Info;
                        }

                        if (propertyInfo != null)
                            if (Dictionarys.TryGetValue(dict.Key.ToLower(), out result))
                            {
                                if (propertyInfo.ToString().Contains("DateTime"))
                                {
                                    DateTime? date;
                                    date = valueObject != null ? (DateTime?)DateTime.MinValue : null;

                                    if (propertyInfo.PropertyType == typeof (DateTime?))
                                    {
                                        if (date == DateTime.MinValue)
                                            date = null;
                                    }

                                    if (valueObject is DateTime)
                                    {
                                        date = (DateTime)valueObject;
                                    }
                                    else
                                    {
                                        if (valueObject != null)
                                            if (!string.IsNullOrEmpty(valueObject.ToString()))
                                            {
                                                var arr = valueObject.ToString().Split('-');
                                                if (arr.Length == 3)
                                                {
                                                    date = new DateTime(Convert.ToInt16(arr[2]), Convert.ToInt16(arr[0]),
                                                                        Convert.ToInt16(arr[1]));
                                                }
                                                else if (arr.Length == 1)
                                                {
                                                    arr = valueObject.ToString().Split(' ')[0].Split('/');
                                                    if (arr.Length == 3)
                                                        date = new DateTime(Convert.ToInt16(arr[2]),
                                                                            Convert.ToInt16(arr[0]),
                                                                            Convert.ToInt16(arr[1]));
                                                }
                                            }
                                    }

                                    Dictionarys[dict.Key.ToLower()] = date;
                                    if (!propertyInfo.GetCustomAttributes(true).Any(n => n is SkipAttribute))
                                        propertyInfo.SetValue(this, date, null);
                                }
                                else if (propertyInfo.PropertyType.IsEnum)
                                {
                                    var values = Enum.GetValues(propertyInfo.PropertyType);

                                    // var val = values.ToArray()[Convert.ToInt16(valueObject.ToString())];
                                    var count = 0;
                                    foreach (var val in values)
                                    {
                                        if (valueObject != null &&
                                            count == (Convert.ToInt16(valueObject.ToString().Trim())))
                                        {
                                            Dictionarys[dict.Key.ToLower()] = Enum.Parse(propertyInfo.PropertyType,
                                                                                         val.ToString());
                                            if (!propertyInfo.GetCustomAttributes(true).Any(n => n is SkipAttribute))
                                                propertyInfo.SetValue(this,
                                                                      Enum.Parse(propertyInfo.PropertyType,
                                                                                 val.ToString()),
                                                                      null);
                                            break;
                                        }
                                        count++;
                                    }
                                }
                                else
                                {
                                    if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null)
                                    {
                                        var genericType = propertyInfo.PropertyType.GetGenericArguments();
                                        if (genericType[0].Name.Contains("String"))
                                        {
                                            if (valueObject != null)
                                            {
                                                Dictionarys[dict.Key.ToLower()] = valueObject.ToString();
                                                if (
                                                    !propertyInfo.GetCustomAttributes(true).Any(n => n is SkipAttribute) ||
                                                    isUpdate)
                                                    propertyInfo.SetValue(this,
                                                                          valueObject.ToString(),
                                                                          BindingFlags.Public | BindingFlags.NonPublic |
                                                                          BindingFlags.Instance, null, null,
                                                                          CultureInfo.InvariantCulture);
                                            }
                                        }
                                        else
                                        {
                                            if (valueObject == null &&
                                                (genericType[0] == typeof(byte) || genericType[0] == typeof(Int16) ||
                                                 genericType[0] == typeof(Int32) || genericType[0] == typeof(Int64)))
                                                valueObject = null;
                                            else if (valueObject != null && string.IsNullOrEmpty(valueObject.ToString()) &&
                                                     genericType[0] == typeof(char)) valueObject = Char.MinValue;
                                            if (valueObject == null)
                                                Dictionarys[dict.Key.ToLower()] = null;
                                            else
                                            {
                                                if (!string.IsNullOrEmpty(valueObject.ToString()))
                                                {
                                                    Dictionarys[dict.Key.ToLower()] =
                                                        Convert.ChangeType(valueObject.ToString(),
                                                                           genericType[0]);
                                                    if (
                                                        !propertyInfo.GetCustomAttributes(true).Any(
                                                            n => n is SkipAttribute))
                                                        propertyInfo.SetValue(this,
                                                                              Convert.ChangeType(valueObject.ToString(),
                                                                                                 genericType[0]),
                                                                              BindingFlags.Public |
                                                                              BindingFlags.NonPublic |
                                                                              BindingFlags.Instance, null, null,
                                                                              CultureInfo.InvariantCulture);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (propertyInfo.PropertyType.Name.Contains("String"))
                                        {
                                            if (valueObject != null)
                                            {
                                                if (string.IsNullOrEmpty(valueObject.ToString()))
                                                {
                                                    valueObject = null;
                                                    Dictionarys[dict.Key.ToLower()] = valueObject;
                                                    if (
                                                        !propertyInfo.GetCustomAttributes(true).Any(
                                                            n => n is SkipAttribute))
                                                        propertyInfo.SetValue(this,
                                                                              valueObject,
                                                                              BindingFlags.Public |
                                                                              BindingFlags.NonPublic |
                                                                              BindingFlags.Instance, null, null,
                                                                              CultureInfo.InvariantCulture);
                                                }
                                                else
                                                {
                                                    Dictionarys[dict.Key.ToLower()] = valueObject.ToString();
                                                    if (
                                                        !propertyInfo.GetCustomAttributes(true).Any(
                                                            n => n is SkipAttribute) ||
                                                        isUpdate)
                                                        propertyInfo.SetValue(this,
                                                                              valueObject.ToString(),
                                                                              BindingFlags.Public |
                                                                              BindingFlags.NonPublic |
                                                                              BindingFlags.Instance, null, null,
                                                                              CultureInfo.InvariantCulture);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (valueObject != null)
                                            {
                                                if (string.IsNullOrEmpty(valueObject.ToString()) &&
                                                    (propertyInfo.PropertyType == typeof(byte) ||
                                                     propertyInfo.PropertyType == typeof(Int16) ||
                                                     propertyInfo.PropertyType == typeof(Int32) ||
                                                     propertyInfo.PropertyType == typeof(Int64))) valueObject = 0;
                                                else if (string.IsNullOrEmpty(valueObject.ToString()) &&
                                                         propertyInfo.PropertyType == typeof(char))
                                                    valueObject = Char.MinValue;
                                                Dictionarys[dict.Key.ToLower()] =
                                                    Convert.ChangeType(valueObject.ToString(),
                                                                       propertyInfo.PropertyType);
                                                if (
                                                    !propertyInfo.GetCustomAttributes(true).Any(n => n is SkipAttribute) ||
                                                    isUpdate)
                                                    propertyInfo.SetValue(this,
                                                                          Convert.ChangeType(valueObject.ToString(),
                                                                                             propertyInfo.PropertyType),
                                                                          BindingFlags.Public | BindingFlags.NonPublic |
                                                                          BindingFlags.Instance, null, null,
                                                                          CultureInfo.InvariantCulture);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (propertyInfo.ToString().Contains("DateTime"))
                                {
                                    DateTime? date = null;
                                    if (valueObject is DateTime)
                                    {
                                        date = (DateTime)valueObject;
                                    }
                                    else
                                    {
                                        if (valueObject != null && string.IsNullOrEmpty(valueObject.ToString()))
                                        {
                                            if (propertyInfo.PropertyType.IsGenericType)
                                            {
                                            }
                                            else
                                                date = DateTime.MinValue;
                                        }
                                        else
                                        {
                                            if (valueObject != null)
                                                if (!string.IsNullOrEmpty(valueObject.ToString()))
                                                {
                                                    var arr = valueObject.ToString().Split('-');
                                                    if (arr.Length == 3)
                                                        date = new DateTime(Convert.ToInt16(arr[2]),
                                                                            Convert.ToInt16(arr[0]),
                                                                            Convert.ToInt16(arr[1]));
                                                    else if (arr.Length == 1)
                                                    {
                                                        arr = valueObject.ToString().Split(' ')[0].Split('/');
                                                        if (arr.Length == 3)
                                                            date = new DateTime(Convert.ToInt16(arr[2]),
                                                                                Convert.ToInt16(arr[0]),
                                                                                Convert.ToInt16(arr[1]));
                                                    }
                                                }
                                        }
                                    }

                                    Dictionarys.Add(dict.Key.ToLower(), date);
                                    if (!propertyInfo.GetCustomAttributes(true).Any(n => n is SkipAttribute))
                                        propertyInfo.SetValue(this, date, null);
                                }
                                else if (propertyInfo.PropertyType.IsEnum)
                                {
                                    var values = Enum.GetValues(propertyInfo.PropertyType);
                                    var count = 0;
                                    foreach (var val in values)
                                    {
                                        if (valueObject != null &&
                                            count == (Convert.ToInt16(valueObject.ToString().Trim())))
                                        {
                                            Dictionarys[dict.Key.ToLower()] = Enum.Parse(propertyInfo.PropertyType,
                                                                                         val.ToString());
                                            if (!propertyInfo.GetCustomAttributes(true).Any(n => n is SkipAttribute))
                                                propertyInfo.SetValue(this,
                                                                      Enum.Parse(propertyInfo.PropertyType,
                                                                                 val.ToString()),
                                                                      null);
                                            break;
                                        }
                                        count++;
                                    }
                                }
                                else
                                {
                                    if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null)
                                    {
                                        var genericType = propertyInfo.PropertyType.GetGenericArguments();
                                        if (genericType[0].Name.Contains("String"))
                                        {
                                            if (valueObject != null)
                                            {
                                                Dictionarys[dict.Key.ToLower()] = valueObject.ToString();
                                                if (!propertyInfo.GetCustomAttributes(true).Any(n => n is SkipAttribute))
                                                    propertyInfo.SetValue(this, valueObject.ToString(),
                                                                          BindingFlags.Public | BindingFlags.NonPublic |
                                                                          BindingFlags.Instance, null, null,
                                                                          CultureInfo.InvariantCulture);
                                            }
                                        }
                                        else
                                        {
                                            if (valueObject != null)
                                                if (string.IsNullOrEmpty(valueObject.ToString()) &&
                                                    (genericType[0] == typeof(byte) || genericType[0] == typeof(Int16) ||
                                                     genericType[0] == typeof(Int32) ||
                                                     genericType[0] == typeof(Int64)))
                                                    valueObject = null;
                                                else if (string.IsNullOrEmpty(valueObject.ToString()) &&
                                                         genericType[0] == typeof(char)) valueObject = Char.MinValue;
                                            if (valueObject == null)
                                                Dictionarys[dict.Key.ToLower()] = null;
                                            else
                                            {
                                                Dictionarys[dict.Key.ToLower()] = Convert.ChangeType(
                                                    valueObject.ToString(), genericType[0]);
                                                if (!propertyInfo.GetCustomAttributes(true).Any(n => n is SkipAttribute))
                                                    propertyInfo.SetValue(this,
                                                                          Convert.ChangeType(valueObject.ToString(),
                                                                                             genericType[0]),
                                                                          BindingFlags.Public | BindingFlags.NonPublic |
                                                                          BindingFlags.Instance, null, null,
                                                                          CultureInfo.InvariantCulture);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (propertyInfo.PropertyType.Name.Contains("String"))
                                        {
                                            if (valueObject != null)
                                            {
                                                if (string.IsNullOrEmpty(valueObject.ToString()))
                                                {
                                                    valueObject = null;
                                                    Dictionarys[dict.Key.ToLower()] = valueObject;
                                                    if (
                                                        !propertyInfo.GetCustomAttributes(true).Any(
                                                            n => n is SkipAttribute))
                                                        propertyInfo.SetValue(this,
                                                                              valueObject,
                                                                              BindingFlags.Public |
                                                                              BindingFlags.NonPublic |
                                                                              BindingFlags.Instance, null, null,
                                                                              CultureInfo.InvariantCulture);
                                                }
                                                else
                                                {
                                                    Dictionarys[dict.Key.ToLower()] = valueObject.ToString();
                                                    if (
                                                        !propertyInfo.GetCustomAttributes(true).Any(
                                                            n => n is SkipAttribute) ||
                                                        isUpdate)
                                                        propertyInfo.SetValue(this,
                                                                              valueObject.ToString(),
                                                                              BindingFlags.Public |
                                                                              BindingFlags.NonPublic |
                                                                              BindingFlags.Instance, null, null,
                                                                              CultureInfo.InvariantCulture);
                                                }
                                                //Dictionarys[dict.Key.ToLower()] =
                                                //    valueObject.ToString();
                                                //if (!propertyInfo.GetCustomAttributes(true).Any(n => n is SkipAttribute))
                                                //    propertyInfo.SetValue(this,
                                                //        valueObject.ToString(),
                                                //        BindingFlags.Public | BindingFlags.NonPublic |
                                                //        BindingFlags.Instance, null, null,
                                                //        CultureInfo.InvariantCulture);
                                            }
                                        }
                                        else
                                        {
                                            if (valueObject != null)
                                            {
                                                if (string.IsNullOrEmpty(valueObject.ToString()) &&
                                                    (propertyInfo.PropertyType == typeof(byte) ||
                                                     propertyInfo.PropertyType == typeof(Int16) ||
                                                     propertyInfo.PropertyType == typeof(Int32) ||
                                                     propertyInfo.PropertyType == typeof(Int64))) valueObject = 0;
                                                else if (string.IsNullOrEmpty(valueObject.ToString()) &&
                                                         propertyInfo.PropertyType == typeof(char))
                                                    valueObject = Char.MinValue;
                                                Dictionarys.Add(dict.Key.ToLower(),
                                                                Convert.ChangeType(valueObject.ToString(),
                                                                                   propertyInfo.PropertyType));
                                                if (
                                                    !propertyInfo.GetCustomAttributes(true).Any(n => n is SkipAttribute) ||
                                                    isUpdate)
                                                    propertyInfo.SetValue(this,
                                                                          Convert.ChangeType(valueObject.ToString(),
                                                                                             propertyInfo.PropertyType),
                                                                          BindingFlags.Public | BindingFlags.NonPublic |
                                                                          BindingFlags.Instance, null, null,
                                                                          CultureInfo.InvariantCulture);
                                            }
                                        }
                                    }
                                }
                            }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            } //);

            OnInit();
            if (Manager != null)
                Manager.FreezeUpdate = true;
            if (!IsNew)
            {
                if (GuidId == null)
                    GuidId = Guid.NewGuid().ToString();
                if (Manager == null)
                {
                    Manager = ContextManager.Current;
                }
                Manager.FreezeUpdate = true;
                Manager.Update(this);
                Manager.FreezeUpdate = false;
            }
        }


        //public override bool Equals(object obj)
        //{
        //    if (obj == null) return false;

        //    return obj.ToString().Equals(this.ToString());
        //}
        public CoreDictionary<string, object> GetDictionary()
        {
            //if (Dictionarys == null)
            //{
            var model = new CoreDictionary<string, object>();
            //foreach (var fieldAttribute in Fields)
            //{
            //    Debug.Print(fieldAttribute.FieldName.ToLower() + " " + fieldAttribute.FieldName);
            //    model.Add(fieldAttribute.FieldName.ToLower(), this[fieldAttribute.FieldName]);
            //}
            if (Dictionarys != null)
                foreach (var type in Dictionarys)
                {
                    object result;
                    if (!model.TryGetValue(type.Key.ToLower(), out result))
                        model.Add(type.Key.ToLower(), type.Value);
                }
            return model;
            //new CoreDictionary<string, object>(Fields.ToDictionary(n => n.FieldName, n => this[n.FieldName]));
            //}
            //return Dictionarys;
        }

        private object DefaultValue(DefaultValueAttribute propertyInfo)
        {
            if (propertyInfo.Value.Equals("Now"))
                return DateTime.Now;
            return propertyInfo.Value;
        }


        /// <summary>
        ///     Gets the reference. digunakan untuk mengambil object relation dari database
        /// </summary>
        /// <typeparam name="T">berisikan Class yang akan di cast</typeparam>
        /// <param name="tableType">berisikan nama table yang berelasi</param>
        /// <param name="related">berisikan where kondisi </param>
        /// <returns>T</returns>
        /// <example>
        ///     contoh yang digunakan untuk mengambil satu data dari database
        ///     <code>
        ///  var model = GetReference<![CDATA[<mEmploye>]]>("mEmploye", "ID=EmployeID");
        ///   return model;
        /// </code>
        /// </example>
        protected T GetReference<T>(Type tableType, params string[] related)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var item = Activator.CreateInstance(tableType) as TableItem;
            if (item != null)
            {
                var tableKey = item.TableName;
                object value;
                if (tableKey.Contains("="))
                {
                    var tempRelated = related;
                    related = new string[1 + related.Length];
                    related[0] = tableKey;
                    var i = 1;
                    foreach (var s in tempRelated)
                    {
                        related[i] = s;
                        i++;
                    }
                    var tableItem = Activator.CreateInstance<T>() as TableItem;
                    if (tableItem != null)
                        tableKey = tableItem.TableName;
                }
                if (Type == RowType.CannotLoad)
                {
                    if (DictionaryReference == null)
                        DictionaryReference = new CoreDictionary<string, object>();
                    //Trace.WriteLine("get : " + tableKey);
                    if (!DictionaryReference.TryGetValue(tableKey, out value))
                    {
                        return default(T);
                    }
                    var result = (T)value;
                    //if (tableItem != null) tableItem.Type = RowType.CannotLoad;
                    return result;
                }
                var list = new List<string>();
                foreach (var s in related)
                {
                    var temp = s.Split('=');
                    if (temp.Length == 2)
                    {
                        try
                        {
                            var firstOrDefault =
                                GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic |
                                                        BindingFlags.Instance).FirstOrDefault(
                                                            n =>
                                                                n.GetCustomAttributes(true)
                                                                    .OfType<FieldAttribute>()
                                                                    .Count(
                                                                        x => x.FieldName.Equals(temp[1])) != 0);
                            if (firstOrDefault != null)
                                list.Add(temp[0] + "='" + firstOrDefault.GetValue(this, null) + "'");
                            else
                            {
                                list.Add(temp[0] + "='" + temp[1] + "'");
                            }
                        }
                        catch (Exception)
                        {
                            list.Add(s);
                        }
                    }
                    else
                    {
                        list.Add(s);
                    }
                }
                if (DictionaryReference == null)
                    DictionaryReference = new CoreDictionary<string, object>();
                if (ContextManager.Current != null)
                {
                    if (Manager == null || Manager.ConnectionManager == null)
                        Manager = ContextManager.Current;
                }
                else if (Manager == null || Manager.ConnectionManager == null)
                    return default(T);
                var where = Manager.ConnectionManager.CreateRelationQuery(list.ToArray());
                if (!DictionaryReference.TryGetValue("One-" + tableKey + @where, out value))
                    OnBeforeGetReference(new ItemEventArgs<KeyValuePair<string, object>>(new KeyValuePair<string, object>("One-" + tableKey + where, this)));
                //Trace.WriteLine("get : " + tableKey + @where);
                if (!DictionaryReference.TryGetValue("One-" + tableKey + @where, out value))
                {
                    var data = (T)Manager.GetOneData(Activator.CreateInstance<T>(), @where);
                    if (data == null)
                        return default(T);
                    if (data != null)
                    {
                        var tableItem = Activator.CreateInstance<T>() as TableItem;
                        if (tableItem != null && !tableItem.IsTransaction)
                        {
                            //Trace.WriteLine(tableKey + where);
                            //if (!IsRebindFromServer)
                            DictionaryReference.Add("One-" + tableKey + where, data);
                        }
                    }
                    else
                    {
                        return default(T);
                    }
                    if (data is TableItem)
                        (data as TableItem).IsRebindFromServer = IsRebindFromServer;
                    stopwatch.Stop();
                    Trace.WriteLine("New - " + data + " [" + TimeSpan.FromTicks(stopwatch.ElapsedTicks) + "] ");

                    return data;
                }
                //else if (IsRebindFromServer)
                //{
                //    var data = (T)Manager.GetOneData(Activator.CreateInstance<T>(), @where);
                //    if (data == null)
                //        return default(T);
                //    if (data != null)
                //    {
                //        var tableItem = Activator.CreateInstance<T>() as TableItem;
                //        if (tableItem != null && !tableItem.IsTransaction)
                //        {
                //            //Trace.WriteLine(tableKey + where);
                //            //if (!IsRebindFromServer)
                //            DictionaryReference.Add("One-" + tableKey + where, data);
                //        }
                //    }
                //    else
                //    {
                //        return default(T);
                //    }
                //    if (data is TableItem)
                //        (data as TableItem).IsRebindFromServer = IsRebindFromServer;
                //    stopwatch.Stop();
                //    Trace.WriteLine("New - " + data + " [" + TimeSpan.FromTicks(stopwatch.ElapsedTicks) + "] ");

                //    return data;
                //}
                stopwatch.Stop();
                Trace.WriteLine("Old - " + value + " [" + TimeSpan.FromTicks(stopwatch.ElapsedTicks) + "] ");
                return (T)value;
            }
            return default(T);
        }


        /// <summary>
        ///     Gets the reference. digunakan untuk mengambil object relation dari database
        /// </summary>
        /// <typeparam name="T">berisikan Class yang akan di cast</typeparam>
        /// <param name="tableKey">berisikan nama table yang berelasi</param>
        /// <param name="related">berisikan where kondisi </param>
        /// <returns>T</returns>
        /// <example>
        ///     contoh yang digunakan untuk mengambil satu data dari database
        ///     <code>
        ///  var model = GetReference<![CDATA[<mEmploye>]]>("mEmploye", "ID=EmployeID");
        ///   return model;
        /// </code>
        /// </example>
        protected T GetReference<T>(string tableKey, params string[] related)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            object value;
            if (tableKey.Contains("="))
            {
                var tempRelated = related;
                related = new string[1 + related.Length];
                related[0] = tableKey;
                var i = 1;
                foreach (var s in tempRelated)
                {
                    related[i] = s;
                    i++;
                }
                var tableItem = Activator.CreateInstance<T>() as TableItem;
                if (tableItem != null)
                    tableKey = tableItem.TableName;
            }
            if (Type == RowType.CannotLoad)
            {
                if (DictionaryReference == null)
                    DictionaryReference = new CoreDictionary<string, object>();
                //Trace.WriteLine("get : " + tableKey);
                if (!DictionaryReference.TryGetValue("One-" + tableKey, out value))
                {
                    return default(T);
                }
                var result = (T)value;
                //if (tableItem != null) tableItem.Type = RowType.CannotLoad;
                return result;
            }
            var list = new List<string>();
            foreach (var s in related)
            {
                var temp = s.Split('=');
                if (temp.Length == 2)
                {
                    try
                    {
                        var firstOrDefault =
                            GetType()
                                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .FirstOrDefault(
                                    n =>
                                        n.GetCustomAttributes(true).OfType<FieldAttribute>().Count(
                                            x => x.FieldName.Equals(temp[1])) != 0);
                        if (firstOrDefault != null)
                            list.Add(temp[0] + "='" + firstOrDefault.GetValue(this, null) + "'");
                        else
                        {
                            list.Add(temp[0] + "='" + temp[1] + "'");
                        }
                    }
                    catch (Exception)
                    {
                        list.Add(s);
                    }
                }
                else
                {
                    list.Add(s);
                }
            }
            if (DictionaryReference == null)
                DictionaryReference = new CoreDictionary<string, object>();
            if (ContextManager.Current != null)
            {
                if (Manager == null || Manager.ConnectionManager == null)
                    Manager = ContextManager.Current;
            }
            else if (Manager == null || Manager.ConnectionManager == null)
                return default(T);
            var where = Manager.ConnectionManager.CreateRelationQuery(list.ToArray());
            //Trace.WriteLine("get : " + tableKey + @where);
            if (!DictionaryReference.TryGetValue("One-" + tableKey + @where, out value))
                OnBeforeGetReference(new ItemEventArgs<KeyValuePair<string, object>>(new KeyValuePair<string, object>("One-" + tableKey + where, this)));
            if (!DictionaryReference.TryGetValue("One-" + tableKey + where, out value) || IsRebindFromServer)
            {
                var data = (T)Manager.GetOneData(Activator.CreateInstance<T>(), where);
                if (data is TableItem)
                    (data as TableItem).IsRebindFromServer = IsRebindFromServer;
                //Trace.WriteLine("New - " + data);
                if (data != null)
                {
                    var tableItem = Activator.CreateInstance<T>() as TableItem;
                    if (tableItem != null && !tableItem.IsTransaction)
                    {
                        stopwatch.Stop();
                        Trace.WriteLine("New - " + data + " [" + TimeSpan.FromTicks(stopwatch.ElapsedTicks) + "] ");
                        //Trace.WriteLine(tableKey + where);
                        if (!IsRebindFromServer)
                            DictionaryReference.Add("One-" + tableKey + where, data);
                    }
                }
                else
                {
                    return default(T);
                }
                stopwatch.Stop();
                Trace.WriteLine("Old - " + data + " [" + TimeSpan.FromTicks(stopwatch.ElapsedTicks) + "] ");
                return data;
            }
            //Trace.WriteLine("Old - " + value);
            return (T)value;
        }

        /// <summary>
        ///     Gets the references.digunakan untuk mengambil object relation dari database
        /// </summary>
        /// <typeparam name="T">berisikan Class yang akan di cast</typeparam>
        /// <param name="tableKey">berisikan nama table yang berelasi</param>
        /// <param name="related">berisikan where kondisi </param>
        /// <returns>List T</returns>
        /// <example>
        ///     contoh yang digunakan untuk mengambil satu data dari database
        ///     <code>
        ///  var model = GetReferences<![CDATA[<mEmploye>]]>("mEmploye", "ID=EmployeID");
        ///   return model;
        /// </code>
        /// </example>
        protected IEnumerable<T> GetReferences<T>(string tableKey, params string[] related)
        {
            object value;
            var list = new List<string>();
            if (DictionaryReference == null)
                DictionaryReference = new CoreDictionary<string, object>();
            foreach (var s in related)
            {
                var temp = s.Split('=');
                if (temp.Length == 2)
                {
                    try
                    {
                        var firstOrDefault =
                            GetType()
                                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .FirstOrDefault(
                                    n =>
                                        n.GetCustomAttributes(true).OfType<FieldAttribute>().Count(
                                            x => x.FieldName.Equals(temp[1])) != 0);
                        if (firstOrDefault != null)
                            list.Add(temp[0] + "='" + firstOrDefault.GetValue(this, null) + "'");
                        else
                        {
                            list.Add(temp[0] + "='" + temp[1] + "'");
                        }
                    }
                    catch (Exception)
                    {
                        list.Add(s);
                    }
                }
                else
                {
                    list.Add(s);
                }
            }
            if (IsFreeze)
            {
                //Trace.WriteLine("get : " + tableKey);
                if (!DictionaryReference.TryGetValue(tableKey, out value))
                {
                    foreach (
                        object item in
                            Manager.GetRow(Activator.CreateInstance<T>(),
                                Manager.ConnectionManager.CreateRelationQuery(list.ToArray())))
                    {
                        var tableItem = (T)item as TableItem;
                        if (tableItem != null)
                        {
                            tableItem.Type = RowType.CannotLoad;
                            tableItem.IsFreeze = true;
                        }
                        yield return (T)item;
                    }
                }
                yield break;
            }
            //Trace.WriteLine("get : " + tableKey);
            if (!DictionaryReference.TryGetValue(tableKey, out value))
            {
                if (Manager == null)
                    yield break;
                foreach (
                    object item in
                        Manager.GetRow(Activator.CreateInstance<T>(),
                            Manager.ConnectionManager.CreateRelationQuery(list.ToArray())))
                {
                    yield return (T)item;
                }
            }
            else
            {
                foreach (var item in (IEnumerable<T>)value)
                {
                    yield return item;
                }
            }
        }


        /// <summary>
        ///     Gets the references.digunakan untuk mengambil object relation dari database
        /// </summary>
        /// <typeparam name="T">berisikan Class yang akan di cast</typeparam>
        /// <param name="tableType">berisikan type table yang berelasi</param>
        /// <param name="related">berisikan where kondisi </param>
        /// <returns>List T</returns>
        /// <example>
        ///     contoh yang digunakan untuk mengambil satu data dari database
        ///     <code>
        ///  var model = GetReferences<![CDATA[<mEmploye>]]>("mEmploye", "ID=EmployeID");
        ///   return model;
        /// </code>
        /// </example>
        protected CoreQueryable<T> GetReferences<T>(Type tableType, params string[] related) where T : TableItem
        {
            var modelItem = Activator.CreateInstance(tableType) as TableItem;
            if (modelItem == null) return null;
            var tableKey = modelItem.TableName;
            object value;
            var list = new List<string>();
            if (DictionaryReference == null)
                DictionaryReference = new CoreDictionary<string, object>();
            foreach (var s in related)
            {
                var temp = s.Split('=');
                if (temp.Length == 2)
                {
                    try
                    {
                        var firstOrDefault =
                            GetType()
                                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .FirstOrDefault(
                                    n =>
                                        n.GetCustomAttributes(true).OfType<FieldAttribute>().Count(
                                            x => x.FieldName.Equals(temp[1])) != 0);
                        if (firstOrDefault != null)
                            list.Add(temp[0] + "='" + firstOrDefault.GetValue(this, null) + "'");
                        else
                        {
                            list.Add(temp[0] + "='" + temp[1] + "'");
                        }
                    }
                    catch (Exception)
                    {
                        list.Add(s);
                    }
                }
                else
                {
                    list.Add(s);
                }
            }
            if (Manager == null) Manager = ContextManager.Current;
            if (Manager == null) return null;
            var filter = Manager.ConnectionManager.CreateRelationQuery(list.ToArray());
            if (IsFreeze)
            {
                //Trace.WriteLine("get : " + tableKey + filter);
                if (!DictionaryReference.TryGetValue(tableKey + filter, out value))
                {
                    return new CoreQueryable<T>(Manager, Activator.CreateInstance<T>(), related);
                    //foreach (object item in Manager.GetRow(Activator.CreateInstance<T>(), filter))
                    //{
                    //    var tableItem = (T)item as TableItem;
                    //    if (tableItem != null)
                    //    {
                    //        tableItem.Type = RowType.CannotLoad;
                    //        tableItem.IsFreeze = true;
                    //    }

                    //    yield return (T)item;
                    //}
                }
                //yield break;
            }
            //Trace.WriteLine("get : " + tableKey + filter);
            //foreach (var key in DictionaryReference.Keys)
            //{
            //    Trace.WriteLine(key);
            //}
            if (!DictionaryReference.TryGetValue(tableKey + filter, out value) || IsRebindFromServer)
                OnBeforeGetReference(new ItemEventArgs<KeyValuePair<string, object>>(new KeyValuePair<string, object>(tableKey + filter, this)));
            if (!DictionaryReference.TryGetValue(tableKey + filter, out value) || IsRebindFromServer)
            {
                Trace.WriteLine(tableKey + filter);
                if (Manager == null)
                    return null;
                return new CoreQueryable<T>(Manager, Activator.CreateInstance<T>(), related);
                //var listTemp = Manager.GetRow(Activator.CreateInstance<T>(), filter).OfType<T>();
                //var tableItem = Activator.CreateInstance<T>() as TableItem;
                //if (tableItem != null)
                //{
                //    tableItem.Skip = true;
                //    if (!tableItem.IsTransaction)
                //    {
                //        //Trace.WriteLine(tableKey + filter);
                //        if (!IsRebindFromServer)
                //            DictionaryReference.Add(tableKey + filter, listTemp);
                //    }
                //}

                ////foreach (T item in listTemp)
                ////{
                ////    yield return item;
                ////}
                //return new CoreQueryable<T>(listTemp);
            }
            return new CoreQueryable<T>((IEnumerable<T>)value);
            //foreach (T item in (IEnumerable<T>)value)
            //{
            //    yield return item;
            //}
        }

        public void SetLocalReference(string key, object obj)
        {
            object objLocal = null;
            if (!DictionaryReference.TryGetValue(key, out objLocal))
                DictionaryReference.Add(key, obj);
        }
        /// <summary>
        ///     Sets the reference.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="related"> </param>
        protected void SetReference(string key, TableItem value, params string[] related)
        {
            //if (TagBeforeInsert == null)
            //    TagBeforeInsert = new List<object>();
            //if (TagBeforeInsert.Count(n => n.Equals(value)) == 0)
            //    TagBeforeInsert.Add(value);
            if (DictionaryReference == null)
                DictionaryReference = new CoreDictionary<string, object>();
            object val;
            var list = new List<string>();
            foreach (var s in related)
            {
                var temp = s.Split('=');
                if (temp.Length == 2)
                {
                    try
                    {
                        var firstOrDefault =
                            GetType()
                                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .FirstOrDefault(
                                    n =>
                                        n.GetCustomAttributes(true).OfType<FieldAttribute>().Count(
                                            x => x.FieldName.Equals(temp[1])) != 0);
                        if (firstOrDefault != null)
                            list.Add(temp[0] + "='" + firstOrDefault.GetValue(this, null) + "'");
                    }
                    catch (Exception)
                    {
                        list.Add(s);
                    }
                }
                else
                {
                    list.Add(s);
                }
            }
            if (DictionaryReference == null)
                DictionaryReference = new CoreDictionary<string, object>();
            if (ContextManager.Current != null)
            {
                if (Manager == null || Manager.ConnectionManager == null)
                    Manager = ContextManager.Current;
            }
            var where = Manager.ConnectionManager.CreateRelationQuery(list.ToArray());
            //Trace.WriteLine("get : " + key + @where);
            if (!DictionaryReference.TryGetValue("One-" + key + @where, out val))
            {
                //Trace.WriteLine(key + @where);
                DictionaryReference.Add("One-" + key + @where, value);
            }
            DictionaryReference["One-" + key + @where] = value;
            OnPropertyChanged(key);
        }

        protected void SetReferences<T>(Type type, IEnumerable<T> values, params string[] related)
        {
            var modelItem = Activator.CreateInstance(type) as TableItem;
            if (modelItem == null) return;
            var tableKey = modelItem.TableName;
            if (TagAfterInsert == null)
                TagAfterInsert = new List<object>();
            foreach (var linkItem in values)
            {
                if (TagAfterInsert.Count(n => n.Equals(linkItem)) == 0)
                    TagAfterInsert.Add(linkItem);
            }

            if (DictionaryReference == null)
                DictionaryReference = new CoreDictionary<string, object>();
            object value;
            //if (!DictionaryReference.TryGetValue(tableKey, out value))
            //{
            //    DictionaryReference.Add(tableKey, values);
            //}
            var list = new List<string>();
            foreach (var s in related)
            {
                var temp = s.Split('=');
                if (temp.Length == 2)
                {
                    try
                    {
                        var firstOrDefault =
                            GetType()
                                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .FirstOrDefault(
                                    n =>
                                        n.GetCustomAttributes(true).OfType<FieldAttribute>().Count(
                                            x => x.FieldName.Equals(temp[1])) != 0);
                        if (firstOrDefault != null)
                            list.Add(temp[0] + "='" + firstOrDefault.GetValue(this, null) + "'");
                    }
                    catch (Exception)
                    {
                        list.Add(s);
                    }
                }
                else
                {
                    list.Add(s);
                }
            }
            if (DictionaryReference == null)
                DictionaryReference = new CoreDictionary<string, object>();
            if (ContextManager.Current != null)
            {
                if (Manager == null || Manager.ConnectionManager == null)
                    Manager = ContextManager.Current;
            }
            if (Manager == null) return;
            if (Manager.ConnectionManager == null) return;
            var where = Manager.ConnectionManager.CreateRelationQuery(list.ToArray());
            //Trace.WriteLine("get : " + tableKey + @where);
            if (!DictionaryReference.TryGetValue(tableKey + @where, out value))
            {
                //Trace.WriteLine(tableKey + @where);
                DictionaryReference.Add(tableKey + @where, values);
            }
            DictionaryReference[tableKey + @where] = values;
        }

        /// <summary>
        ///     Sets the references.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        protected void SetReferences(string key, IEnumerable<TableItem> values)
        {
            if (TagAfterInsert == null)
                TagAfterInsert = new List<object>();
            foreach (var linkItem in values)
            {
                if (TagAfterInsert.Count(n => n.Equals(linkItem)) == 0)
                    TagAfterInsert.Add(linkItem);
            }

            if (DictionaryReference == null)
                DictionaryReference = new CoreDictionary<string, object>();
            object value;
            //Trace.WriteLine("get : " + key);
            if (!DictionaryReference.TryGetValue(key, out value))
            {
                //Trace.WriteLine(key);
                DictionaryReference.Add(key, values);
            }
        }

        #endregion Protected

        #region Delete

        /// <summary>
        ///     Called when [before delete]. digunakan untuk memangggil trigger yang di pasang di dalam Domain
        /// </summary>
        internal void OnBeforeDelete()
        {
            BeforeDeleteTrigger(Manager, Previous);
        }

        /// <summary>
        ///     Befores the delete trigger. digunakan untuk melakukan trigger sebelum penghapusan tabel
        /// </summary>
        /// <param name="contextManager">The context manager.</param>
        /// <param name="fieldsBefore">The fields before.</param>
        protected virtual void BeforeDeleteTrigger(ContextManager contextManager,
            IDictionary<string, object> fieldsBefore)
        {
        }

        #endregion Delete

        #region Update

        /// <summary>
        ///     Called when [before update]. digunakan untuk memangggil trigger yang di pasang di dalam Domain
        /// </summary>
        internal void OnBeforeUpdate()
        {
            BeforeUpdateTrigger(Manager, Previous, Dictionarys);
        }

        /// <summary>
        ///     Befores the update trigger.digunakan untuk melakukan trigger sebelum perubahan tabel
        /// </summary>
        /// <param name="contextManager">The context manager.</param>
        /// <param name="fieldsBefore">The fields before.</param>
        /// <param name="fieldsAfter">The fields after.</param>
        protected virtual void BeforeUpdateTrigger(ContextManager contextManager,
            IDictionary<string, object> fieldsBefore,
            IDictionary<string, object> fieldsAfter)
        {
        }

        #endregion Update

        #region MyRegion

        /// <summary>
        ///     Called when [before insert]. digunakan untuk memangggil trigger yang di pasang di dalam Domain
        /// </summary>
        internal void OnBeforeInsert()
        {
            BeforeInsertTrigger(Manager, Dictionarys);
        }

        /// <summary>
        ///     Befores the insert trigger.digunakan untuk melakukan trigger sebelum penyimpanan tabel
        /// </summary>
        /// <param name="contextManager">The context manager.</param>
        /// <param name="fieldsAfter">The fields after.</param>
        protected virtual void BeforeInsertTrigger(ContextManager contextManager,
            IDictionary<string, object> fieldsAfter)
        {
        }

        #endregion MyRegion

        #region Implementation of INotifyPropertyChanged

        #endregion Implementation of INotifyPropertyChanged

        protected virtual void OnBeforeGetReference(ItemEventArgs<KeyValuePair<string, object>> e)
        {
            var handler = BeforeGetReference;
            if (handler != null) handler(this, e);
        }
    }
}