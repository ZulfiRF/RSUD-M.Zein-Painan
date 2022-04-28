using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DbContext.Persistance;
using System.Linq.Expressions;
using System.Data.Common;
using System.Data.SqlClient;

namespace DbContext
{
    public static class Proccessing
    {
        private static string listAutoIncement = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static Dictionary<int, char> list = new Dictionary<int, char>();
        public static int count = 20;
        public static bool IsUseAllMethod = false;
        private static Connection conn = null;
        public static Exception Save(this object data)
        {
            return (data is IEnumerable<object>) ? Save(data as IEnumerable<object>, null) : Save(new object[] { data }, null);
        }
        public static Exception Save(this IEnumerable<object> data, IProccessing prosses)
        {
            try
            {
                string sql = "";
                if (prosses != null)
                {
                    prosses.Save(data);
                    return null;
                }
                else
                {
                    if (conn == null)
                    {
                        conn = string.IsNullOrEmpty(Connection.ConnectionString) ? new Connection(currentProvider) : new Connection(Connection.ConnectionString, currentProvider);
                    }
                    try
                    {
                        AutoGenerateKey(data);
                        try
                        {
                            {
                                if (!IsUseAllMethod)
                                {
                                    conn = string.IsNullOrEmpty(Connection.ConnectionString) ? new Connection(currentProvider) : new Connection(Connection.ConnectionString, currentProvider);
                                    conn.Open();
                                    conn.BeginTransaction();
                                }

                            }
                        }
                        catch (Exception)
                        {
                            if (!IsUseAllMethod)
                            {
                                conn.Close();
                                conn.Open();
                                conn.BeginTransaction();
                            }
                        }
                        foreach (var item in data)
                        {

                            var ListEnum = new List<string>();
                            var AutoIncrement = new List<string>();
                            System.Reflection.MemberInfo _data = item.GetType();
                            var attributes = _data.GetCustomAttributes(true);
                            foreach (object t in attributes)
                            {
                                if (t is PrimaryAttribute)
                                {
                                    var primaryAttribute = t as PrimaryAttribute;
                                    if (!string.IsNullOrEmpty(primaryAttribute.AutoIncrement))
                                    {
                                        AutoIncrement.AddRange(primaryAttribute.AutoIncrement.Split(new[] { ',' }));
                                    }

                                }
                                else
                                    if (t is EnumAttribute)
                                    {
                                        var primaryAttribute = t as EnumAttribute;
                                        ListEnum.AddRange(primaryAttribute.Value.Split(new[] { ',' }));
                                    }
                                //
                            }

                            string field = "(";
                            string value = "(";
                            foreach (
                            var info in item.GetType().GetProperties().Where(info => info.GetValue(item, null) != null && (!info.PropertyType.IsClass || info.PropertyType == typeof(string)) && !info.PropertyType.IsEnum))
                            {
                                if (((!info.PropertyType.IsClass && !info.PropertyType.IsEnum) || info.PropertyType.Name.Equals("String")) && !info.Name.Equals("DefaulTime"))
                                {
                                    if (AutoIncrement.Where(n => n.Equals(info.Name)).Count() == 0 && info.GetCustomAttributes(true).Where(n => n is PrimaryUpdateAttribute).Count() == 0)
                                    {
                                        field += info.Name + ",";
                                        value += (info.GetValue(item, null) is DateTime)
                                        ? "'" + ((DateTime)info.GetValue(item, null)).ToString(conn.FormateDate) + "',"
                                        : "'" + info.GetValue(item, null).ToString() + "',";
                                    }
                                }
                            }
                            if (field != string.Empty)
                            {
                                field = field.Substring(0, field.Length - 1) + ")";
                            }
                            if (value != string.Empty)
                            {
                                value = value.Substring(0, value.Length - 1) + ")";
                            }
                            string nameTable = "";
                            if (item.GetType().GetCustomAttributes(true).Where(n => n is TabelNameAttribute).Count() != 0)
                            {
                                nameTable = (item.GetType().GetCustomAttributes(true).FirstOrDefault(n => n is TabelNameAttribute) as TabelNameAttribute).Name;
                            }
                            else
                                nameTable = item.GetType().Name;
                            sql = "Insert into " + nameTable + " " + field + " VALUES " + value;
                            sql = sql.Replace("''", "null");
                            Console.WriteLine(sql);
                            conn.CommandText = sql;
                            conn.ExecuteNonQuery();
                        }
                        if (!IsUseAllMethod)
                        {
                            conn.Commit();
                            conn.Close();
                            conn = null;
                        }
                        return null;
                    }
                    catch (Exception _ex)
                    {
                        if (!IsUseAllMethod)
                        {
                            if (conn != null)
                            {
                                conn.RollBack();
                                conn.Close();
                            }
                        }
                        else
                        {
                            if (conn != null)
                            {
                                conn.RollBack();
                                conn.Close();
                            }
                            throw new ArgumentException(_ex.Message, _ex.InnerException);
                        }
                        //if (_ex.ToString().Contains("Violation of PRIMARY KEY"))
                        //    return Update(data, prosses);
                        return _ex;
                    }
                    finally
                    {
                        if (!IsUseAllMethod)
                            if (conn != null)
                            {
                                conn.Close();
                            }
                    }
                }


            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public static void AutoGenerateKey(IEnumerable<object> data)
        {

            var listChart = listAutoIncement.ToCharArray();
            if (list.Count == 0)
                for (int i = 0; i < listChart.Length; i++)
                {
                    list.Add(i, listChart[i]);
                }
            foreach (var item in data)
            {
                foreach (var info in item.GetType().GetProperties().Where(info => info.GetCustomAttributes(true).Where(attr => attr is CustomAttribute).Count() != 0))
                {
                    //autoIncrement
                    var attribute = info.GetCustomAttributes(true);
                    foreach (var subItem in attribute)
                    {
                        #region AutoGenerateDateTimeYYMMAttribute
                        if (subItem is AutoGenerateDateTimeYYMMAttribute)
                        {
                            var attr = subItem as AutoGenerateDateTimeYYMMAttribute;
                            attr.Property = info.Name;
                            DateTime? date = DateTime.Now;
                            try
                            {
                                date = (DateTime?)item.GetType().GetProperty("DefaultTime").GetValue(item, null);
                            }
                            catch (Exception)
                            {

                            }
                            if (date == null)
                            {
                                date = DateTime.Now;
                            }
                            try
                            {
                                var lastRecord = "";
                                string nameTable = "";
                                if (item.GetType().GetCustomAttributes(true).Where(n => n is TabelNameAttribute).Count() != 0)
                                {
                                    nameTable = (item.GetType().GetCustomAttributes(true).FirstOrDefault(n => n is TabelNameAttribute) as TabelNameAttribute).Name;
                                }
                                else
                                    nameTable = item.GetType().Name;
                                
                                switch (attr.TypeGenerate)
                                {
                                    case AutoGenerateDateTimeYYMMAttribute.TypeAutoGenerate.AutoFill:
                                        lastRecord = new object().LastRecordIndex(nameTable, info.Name);
                                        break;
                                    case AutoGenerateDateTimeYYMMAttribute.TypeAutoGenerate.LastIndex:
                                        lastRecord = new object().LastRecordIndex(nameTable, info.Name);
                                        break;
                                    default:
                                        break;
                                }

                                int countLasRecort = 0;
                                if (lastRecord == string.Empty)
                                    countLasRecort = 1;
                                else
                                {
                                    if (!lastRecord.Substring(0, lastRecord.Length - attr.Length).Equals(date.Value.ToString("yyMM")))
                                    {
                                        countLasRecort = 1;
                                    }
                                    else
                                        countLasRecort = Convert.ToInt32(lastRecord.Substring(lastRecord.Length - attr.Length)) + 1;
                                }
                                string hasil = date.Value.ToString("yyMM") + countLasRecort.ToString("D" + attr.Length);
                                info.SetValue(item, hasil, null);

                            }
                            catch (Exception)
                            {
                            }
                            finally
                            {
                                //.conn.CloseReader();

                            }

                        }
                        #endregion
                        #region CustomIntAutoIncrement
                        else if (subItem is CustomIntAutoIncrement)
                        {
                            var attr = subItem as CustomIntAutoIncrement;
                            try
                            {
                                int? lastRecord = 0;
                                try
                                {
                                    string nameTable = "";
                                    if (item.GetType().GetCustomAttributes(true).Where(n => n is TabelNameAttribute).Count() != 0)
                                    {
                                        nameTable = (item.GetType().GetCustomAttributes(true).FirstOrDefault(n => n is TabelNameAttribute) as TabelNameAttribute).Name;
                                    }
                                    else
                                        nameTable = item.GetType().Name;
                                    lastRecord = Convert.ToInt32(new object().LastRecordIndex(nameTable, info.Name));
                                }
                                catch (Exception)
                                {

                                }

                                lastRecord++;
                                info.SetValue(item, lastRecord, null);

                            }
                            catch (Exception)
                            {
                            }
                            finally
                            {
                                //.conn.CloseReader();

                            }
                        }
                        #endregion
                        #region CustomAutoIncrementAttribute
                        else if (subItem is CustomAutoIncrementAttribute)
                        {
                            var attr = subItem as CustomAutoIncrementAttribute;
                            try
                            {
                                var lastRecord = "";
                                string tempHasil = "";
                                if (attr.RelationProperty != null)
                                    foreach (var realtionProperty in attr.RelationProperty)
                                    {
                                        tempHasil += item.GetType().GetProperty(realtionProperty).GetValue(item, null).ToString();
                                    }

                                string nameTable = "";
                                if (item.GetType().GetCustomAttributes(true).Where(n => n is TabelNameAttribute).Count() != 0)
                                {
                                    nameTable = (item.GetType().GetCustomAttributes(true).FirstOrDefault(n => n is TabelNameAttribute) as TabelNameAttribute).Name;
                                }
                                else
                                    nameTable = item.GetType().Name;
                                lastRecord = new object().LastRecordIndex(nameTable, info.Name, tempHasil);
                                if (lastRecord == string.Empty)
                                    lastRecord = Convert.ToInt16(0).ToString("D" + (attr.Value - tempHasil.Length).ToString());
                                else
                                    lastRecord = lastRecord.Substring(tempHasil.Length);
                                int countLasRecort = 0;
                                if (lastRecord == string.Empty)
                                    countLasRecort = 'A';
                                else
                                {

                                    var index = list.FirstOrDefault(n => n.Value.ToString().ToUpper().Equals(lastRecord[lastRecord.Length - 1].ToString().ToUpper()));
                                    if (index.Key == list.Count - 1)
                                    {
                                    }

                                    var tempChar = lastRecord.ToCharArray();
                                    lastRecord = "";
                                    for (int i = 0; i < tempChar.Length - 1; i++)
                                    {
                                        lastRecord += tempChar[i];
                                    }
                                    if (index.Key == list.Max(n => n.Key))
                                    {
                                        string tempLastRecord = lastRecord;
                                        lastRecord = "";
                                        AutoIncrementLastRecordChar(tempLastRecord.ToCharArray().Reverse()).Reverse().ToList().ForEach(n => lastRecord += n);
                                        lastRecord += list[0].ToString();
                                    }
                                    else
                                        lastRecord += list[index.Key + 1].ToString();
                                }
                                if (info.ToString().Contains("Char"))
                                    info.SetValue(item, Convert.ToChar(lastRecord), null);
                                else
                                    info.SetValue(item, (tempHasil + lastRecord), null);

                            }
                            catch (Exception)
                            {
                            }
                            finally
                            {
                                //.conn.CloseReader();

                            }
                        }
                        #endregion
                    }
                }
            }
        }
        private static IEnumerable<char> AutoIncrementLastRecordChar(IEnumerable<char> iEnumerable)
        {
            var listChar = iEnumerable.ToArray();
            for (int i = 0; i < listChar.Length; i++)
            {
                if (listChar[i].Equals(list.Last().Value))
                {
                    listChar[i] = '0';
                    if (listChar[i + 1].Equals(list.Last().Value))
                    {
                        var tempList = AutoIncrementLastRecordChar(listChar.Skip(1)).ToArray();
                        for (int j = 0; j < tempList.Length; j++)
                        {
                            listChar[i + 1 + j] = tempList[j];
                        }
                    }
                    else
                        listChar[i + 1] = list[list.FirstOrDefault(n => n.Value.Equals(listChar[i + 1])).Key + 1];
                    break;
                }
                else
                {
                    listChar[i] = list[list.FirstOrDefault(n => n.Value.Equals(listChar[i])).Key + 1];
                    break;
                }
            }
            return listChar;
        }

        public static string ToHasil(this Exception e)
        {
            if (e == null)
                return "SUCCESS";
            else
                return e.ToString();
        }

        public static Exception Update(this object data)
        {
            return (data is IEnumerable<object>) ? Update(data as IEnumerable<object>, null) : Update(new object[] { data }, null);
        }
        public static Exception Update(this IEnumerable<object> data, IProccessing prosses)
        {
            try
            {
                if (prosses != null)
                {
                    prosses.Update(data);
                    return null;
                }
                else
                {
                    if (conn == null)
                    {
                        conn = string.IsNullOrEmpty(Connection.ConnectionString) ? new Connection(currentProvider) : new Connection(Connection.ConnectionString, currentProvider);
                    }
                    try
                    {
                        try
                        {
                            if (!IsUseAllMethod)
                            {
                                conn.Open();
                                conn.BeginTransaction();
                            }
                        }
                        catch (Exception)
                        {
                            if (!IsUseAllMethod)
                            {
                                conn.Close();
                                conn.Open();
                                conn.BeginTransaction();
                            }
                        }
                        foreach (var item in data)
                        {
                            var Primary = new List<string>();
                            var AutoIncrement = new List<string>();
                            var UpdatePrimary = new Dictionary<string, PrimaryUpdateAttribute>();
                            System.Reflection.MemberInfo _data = item.GetType();
                            var attributes = _data.GetCustomAttributes(true);
                            foreach (object t in attributes)
                            {
                                if (t is PrimaryAttribute)
                                {
                                    var primaryAttribute = t as PrimaryAttribute;
                                    Primary.AddRange(primaryAttribute.PrimaryKey.Split(new[] { ',' }));
                                    if (!string.IsNullOrEmpty(primaryAttribute.AutoIncrement))
                                    {
                                        AutoIncrement.AddRange(primaryAttribute.AutoIncrement.Split(new[] { ',' }));
                                    }
                                }

                            }
                            string where = "";
                            string value = "";

                            foreach (
                            var info in item.GetType().GetProperties().Where(info => info.GetValue(item, null) != null))
                            {
                                if ((!info.PropertyType.IsClass && !info.PropertyType.IsEnum) || info.PropertyType.Name.Equals("String"))
                                {
                                    string temp = (info.GetValue(item, null) is DateTime)
                                    ? "'" + ((DateTime)info.GetValue(item, null)).ToString(conn.FormateDate) + "'"
                                    : "'" + info.GetValue(item, null).ToString() + "'";
                                    ;
                                    if (Primary.Where(n => n.Equals(info.Name)).Count() != 0)
                                    {
                                        where += info.Name + " =" + temp + " AND ";
                                    }
                                    else
                                        if (AutoIncrement.Where(n => n.Equals(info.Name)).Count() != 0)
                                        {
                                        }
                                        else
                                            if (info.GetCustomAttributes(true).Where(n => n is PrimaryUpdateAttribute).Count() != 0)
                                            {
                                                foreach (var _item in info.GetCustomAttributes(true))
                                                {
                                                    if (_item is PrimaryUpdateAttribute)
                                                    {
                                                        value += (_item as PrimaryUpdateAttribute).FieldValue + " =" + temp + ",";
                                                    }
                                                }
                                            }
                                            else
                                            {

                                                value += info.Name + " =" + temp + ",";
                                            }
                                }

                            }
                            if (value != string.Empty)
                            {
                                value = value.Substring(0, value.Length - 1);
                            }
                            if (where != string.Empty)
                            {
                                where = where.Substring(0, where.Length - 4);
                            }
                            string nameTable = "";
                            if (item.GetType().GetCustomAttributes(true).Where(n => n is TabelNameAttribute).Count() != 0)
                            {
                                nameTable = (item.GetType().GetCustomAttributes(true).FirstOrDefault(n => n is TabelNameAttribute) as TabelNameAttribute).Name;
                            }
                            else
                                nameTable = item.GetType().Name;

                            string sql = "Update " + nameTable + " SET " + value + " Where " + where;
                            sql = sql.Replace("''", "null");
                            conn.CommandText = sql;
                            Console.WriteLine(sql);
                            conn.ExecuteNonQuery();
                        }
                        if (!IsUseAllMethod)
                        {
                            conn.Commit();
                            conn.Close();
                            conn = null;
                        }
                        return null;
                    }
                    catch (Exception _ex)
                    {
                        //Console.WriteLine(_ex);
                        if (!IsUseAllMethod)
                            if (conn != null)
                                conn.RollBack();
                        if (conn != null)
                        {
                            conn.RollBack();
                            conn.Close();
                        }
                        return _ex;
                    }
                    finally
                    {
                        if (!IsUseAllMethod)
                            if (conn != null)
                            {
                                conn.Close();
                            }
                    }
                }


            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public static Exception Delete(this object data)
        {
            return (data is IEnumerable<object>) ? Delete(data as IEnumerable<object>, null) : Delete(new object[] { data }, null);
        }
        public static Exception Delete(this IEnumerable<object> data, IProccessing prosses)
        {
            try
            {
                if (prosses != null)
                {
                    prosses.Delete(data);
                    return null;
                }
                else
                {
                    if (conn == null)
                    {
                        conn = string.IsNullOrEmpty(Connection.ConnectionString) ? new Connection(currentProvider) : new Connection(Connection.ConnectionString, currentProvider);
                    }
                    try
                    {
                        if (!IsUseAllMethod)
                            conn.Open();
                        if (!IsUseAllMethod)
                            conn.BeginTransaction();

                        foreach (var item in data)
                        {
                            var Primary = new List<string>();
                            var AutoIncrement = new List<string>();
                            System.Reflection.MemberInfo _data = item.GetType();
                            object[] attributes = _data.GetCustomAttributes(true);
                            foreach (object t in attributes)
                            {
                                if (t is PrimaryAttribute)
                                {
                                    var primaryAttribute = t as PrimaryAttribute;
                                    Primary.AddRange(primaryAttribute.PrimaryKey.Split(new[] { ',' }));
                                    if (!string.IsNullOrEmpty(primaryAttribute.AutoIncrement))
                                    {
                                        AutoIncrement.AddRange(primaryAttribute.AutoIncrement.Split(new[] { ',' }));
                                    }
                                }
                                //
                            }
                            string where = "";
                            foreach (
                            var info in item.GetType().GetProperties().Where(info => info.GetValue(item, null) != null))
                            {
                                if ((!info.PropertyType.IsClass && !info.PropertyType.IsEnum) || info.PropertyType.Name.Equals("String"))
                                {
                                    string temp = (info.GetValue(item, null) is DateTime)
                                    ? "'" + ((DateTime)info.GetValue(item, null)).ToString(conn.FormateDate) + "'"
                                    : "'" + info.GetValue(item, null).ToString() + "'";
                                    ;
                                    if (Primary.Where(n => n.Trim().Equals(info.Name)).Count() != 0)
                                    {
                                        where += info.Name + " =" + temp + " AND ";
                                    }
                                }

                            }
                            if (where != string.Empty)
                            {
                                where = where.Substring(0, where.Length - 4);
                            }
                            string nameTable = "";
                            if (item.GetType().GetCustomAttributes(true).Where(n => n is TabelNameAttribute).Count() != 0)
                            {
                                nameTable = (item.GetType().GetCustomAttributes(true).FirstOrDefault(n => n is TabelNameAttribute) as TabelNameAttribute).Name;
                            }
                            else
                                nameTable = item.GetType().Name;
                            string sql = "Delete From " + nameTable + " Where " + where;

                            conn.CommandText = sql;
                            conn.ExecuteNonQuery();
                        }
                        if (!IsUseAllMethod)
                        {
                            conn.Commit();
                            conn.Close();
                            conn = null;
                        }
                        return null;
                    }
                    catch (Exception _ex)
                    {
                        if (!IsUseAllMethod)
                            if (conn != null)
                                conn.RollBack();
                        return _ex;
                    }
                    finally
                    {
                        if (!IsUseAllMethod)
                            if (conn != null)
                            {
                                conn.Close();
                            }
                    }
                }


            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public static Exception DeleteFromPrimaryObject(this IEnumerable<object> data)
        {
            try
            {

                try
                {

                    foreach (var item in data)
                    {
                        if (conn == null)
                        {
                            conn = string.IsNullOrEmpty(Connection.ConnectionString) ? new Connection(currentProvider) : new Connection(Connection.ConnectionString, currentProvider);
                        }
                        conn.Open();
                        conn.BeginTransaction();
                        var Primary = new List<string>();
                        var AutoIncrement = new List<string>();
                        System.Reflection.MemberInfo _data = item.GetType();
                        object[] attributes = _data.GetCustomAttributes(true);
                        foreach (object t in attributes)
                        {
                            if (t is PrimaryAttribute)
                            {
                                var primaryAttribute = t as PrimaryAttribute;
                                Primary.AddRange(primaryAttribute.PrimaryKey.Split(new[] { ',' }));

                            }
                            //
                        }

                        string where = "";
                        foreach (
                        var info in item.GetType().GetProperties().Where(info => info.GetValue(item, null) != null))
                        {
                            if ((!info.PropertyType.IsClass && !info.PropertyType.IsEnum) || info.PropertyType.Name.Equals("String"))
                            {
                                string temp = (info.GetValue(item, null) is DateTime)
                                ? "'" + ((DateTime)info.GetValue(item, null)).ToString(conn.FormateDate) + "'"
                                : "'" + info.GetValue(item, null).ToString() + "'";
                                ;
                                if (Primary.Where(n => n.Equals(info.Name)).Count() != 0)
                                {
                                    where += info.Name + " =" + temp + " AND ";
                                }
                            }

                        }
                        if (where != string.Empty)
                        {
                            where = where.Substring(0, where.Length - 4);
                        }
                        string nameTable = "";
                        if (item.GetType().GetCustomAttributes(true).Where(n => n is TabelNameAttribute).Count() != 0)
                        {
                            nameTable = (item.GetType().GetCustomAttributes(true).FirstOrDefault(n => n is TabelNameAttribute) as TabelNameAttribute).Name;
                        }
                        else
                            nameTable = item.GetType().Name;
                        string sql = "Delete From " + nameTable + " Where " + where;
                        Console.WriteLine(sql);
                        conn.CommandText = sql;
                        conn.ExecuteNonQuery();
                        conn.Commit();
                        conn.Close();
                        conn = null;
                    }

                    return null;
                }
                catch (Exception _ex)
                {
                    if (!IsUseAllMethod)
                        if (conn != null)
                            conn.RollBack();
                    return _ex;
                }
                finally
                {
                    if (!IsUseAllMethod)
                        if (conn != null)
                        {
                            conn.Close();
                        }
                }



            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public static string LastRecordIndex(this object obj, string sql)
        {
            Connection conn = string.IsNullOrEmpty(Connection.ConnectionString) ? new Connection(currentProvider) : new Connection(Connection.ConnectionString, currentProvider);
            try
            {
                string s = "";
                var data = conn.GetDataFromSQL(sql);

                if (data != null)
                    if (!data.IsClosed)
                    {
                        try
                        {
                            while (data.Read())
                            {
                                s = data[0].ToString();
                            }
                        }
                        catch (Exception)
                        {

                        }

                    }

                return s;
            }
            finally
            {
                if (current != null)
                {
                    current.CloseReader();
                    current.Close();
                    current = null;
                }
                conn.CloseReader();
                if (!IsUseAllMethod)
                {
                    conn.Close();
                    conn = null;
                }
            }
        }
        public static string LastRecordIndex(this object obj, string table, string field)
        {
            Connection _conn = string.IsNullOrEmpty(Connection.ConnectionString) ? new Connection(currentProvider) : new Connection(Connection.ConnectionString, currentProvider);
            try
            {
                string s = "";
                current = _conn;
                var data = _conn.GetDataFromSQL("select " + field + " from " + table + " order by " + field);
                if (data != null)
                    if (!data.IsClosed)
                        try
                        {
                            while (data.Read())
                            {
                                s = data[0].ToString();
                            }
                            _conn.CloseReader();
                        }
                        catch (Exception)
                        {

                        }

                return s;
            }
            finally
            {
                if (current != null)
                {
                    current.CloseReader();
                    current.Close();
                    current = null;
                }
                if (_conn != null)
                {

                    _conn.Close();
                    _conn = null;
                }
            }
        }

        public static string LastRecordIndex(this object obj, string table, string field, string startWith)
        {
            Connection _conn = string.IsNullOrEmpty(Connection.ConnectionString) ? new Connection(currentProvider) : new Connection(Connection.ConnectionString, currentProvider);
            try
            {
                string s = "";
                current = _conn;
                var data = _conn.GetDataFromSQL("select " + field + " from " + table + " where " + field + " like '" + startWith + "%' order by " + field);

                if (data != null)
                    if (!data.IsClosed)
                        try
                        {
                            while (data.Read())
                            {
                                s = data[0].ToString();
                            }
                            _conn.CloseReader();
                        }
                        catch (Exception)
                        {

                        }

                return s;
            }
            finally
            {
                if (current != null)
                {
                    current.CloseReader();
                    current.Close();
                    current = null;
                }
                if (_conn != null)
                {

                    _conn.Close();
                    _conn = null;
                }
            }
        }
        public static int RowCountFromTable(this object obj, string sql)
        {
            Connection _conn = string.IsNullOrEmpty(Connection.ConnectionString) ? new Connection(currentProvider) : new Connection(Connection.ConnectionString, currentProvider);
            try
            {
                current = _conn;
                int count = 0;
                var data = _conn.GetDataFromSQL(sql);                
                if (data != null)
                    if (!data.IsClosed)
                        try
                        {
                            while (data.Read())
                            {
                                count++;
                            }
                            _conn.CloseReader();
                        }
                        catch (Exception)
                        {

                        }

                return count;
            }
            finally
            {
                if (current != null)
                {
                    current.CloseReader();
                    current.Close();
                    current = null;
                }
                if (!IsUseAllMethod)
                {
                    _conn.Close();
                    _conn = null;
                }
            }
        }
        public static object LastRecordIndexObject(this object obj, string sql)
        {
            Connection _conn = string.IsNullOrEmpty(Connection.ConnectionString) ? new Connection(currentProvider) : new Connection(Connection.ConnectionString, currentProvider);
            try
            {
                object s = null;
                current = _conn;
                var data = _conn.GetDataFromSQL(sql);
                if (data != null)
                    if (!data.IsClosed)
                    {
                        try
                        {
                            while (data.Read())
                            {
                                return data[0];
                                s = data[0];
                            }
                            _conn.CloseReader();

                        }
                        catch (Exception)
                        {

                        }

                    }
                return s;
            }
            finally
            {
                if (current != null)
                {
                    current.CloseReader();
                    current.Close();
                    current = null;
                }
                if (!IsUseAllMethod)
                {
                    _conn.Close();
                    _conn = null;
                }
            }

        }
        private static DbContext.Connection.DatabaseProvider currentProvider = Connection.DatabaseProvider.SQLServer;
        public static Exception AllArgument(this object obj, Action actions)
        {
            try
            {
                IsUseAllMethod = true;
                if (conn == null)
                {
                    conn = string.IsNullOrEmpty(Connection.ConnectionString) ? new Connection(currentProvider) : new Connection(Connection.ConnectionString, currentProvider);
                }
                conn.Open();
                conn.BeginTransaction();
                actions.Invoke();
                conn.Commit();
                IsUseAllMethod = false;
                conn.Close();
                return null;
            }
            catch (Exception ex)
            {
                if (conn != null)
                    conn.RollBack();
                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }
                return ex;
            }
            finally
            {

                if (conn != null)
                {
                    conn.CloseReader();
                    conn.Close();
                    conn = null;
                }
            }
        }
        private static Connection current = null;
        public static DbDataReader GetDataFromSQL(this object obj, string sql)
        {

            if (current == null)
            {
                current = string.IsNullOrEmpty(Connection.ConnectionString) ? new Connection(currentProvider) : new Connection(Connection.ConnectionString, currentProvider);
            }
            try
            {
                lock (current.CommandReader)
                {
                    try
                    {
                        //if (!IsUseAllMethod) 
                        {
                            current.Open();
                            current.BeginTransaction();
                        }
                    }
                    catch (Exception)
                    {
                        //  if (!IsUseAllMethod)
                        {
                            current.Close();
                            current.Open();
                            current.BeginTransaction();
                        }
                    }
                    current.CommandReader = sql;
                    return current.ExecuteReader();
                }
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {

            }


        }
        public static object MigrateObject(this object target, object value, params string[] field)
        {
            foreach (var item in field)
            {
                try
                {
                    target.GetType().GetProperty(item).SetValue(target, value.GetType().GetProperty(item).GetValue(value, null), null);
                }
                catch (Exception)
                {
                }

            }
            return target;
        }

        #region Comment


        //public static object GetOneData<T>(this object data)
        //{
        //    T item = Activator.CreateInstance<T>();
        //    try
        //    {
        //        if (conn == null)
        //            conn = string.IsNullOrEmpty(Connection.ConnectionString) ? new Connection(currentProvider) : new Connection(Connection.ConnectionString, currentProvider);

        //        if (!IsUseAllMethod) conn.Open();
        //        if (!IsUseAllMethod) conn.BeginTransaction();


        //        var Primary = new List<string>();
        //        System.Reflection.MemberInfo _data = item.GetType();
        //        var attributes = _data.GetCustomAttributes(true);
        //        foreach (object t in attributes)
        //        {
        //            if (t is PrimaryAttribute)
        //            {
        //                var primaryAttribute = t as PrimaryAttribute;
        //                Primary.AddRange(primaryAttribute.PrimaryKey.Split(new[] { ',' }));
        //            }
        //            //
        //        }
        //        string where = "";
        //        foreach (
        //        var info in data.GetType().GetProperties().Where(info => info.GetValue(item, null) != null))
        //        {
        //            string temp = (info.GetValue(item, null) is DateTime)
        //                             ? "'" + ((DateTime)info.GetValue(item, null)).ToString(conn.FormateDate) + "'"
        //                             : "'" + info.GetValue(item, null).ToString() + "'"; ;
        //            if (Primary.Where(n => n.Equals(info.Name)).Count() != 0)
        //            {
        //                where += info.Name + " =" + temp + " AND,";
        //            }
        //        }
        //        where = where.Substring(0, where.Length - 4);

        //        conn.CommandText = "Select * from " + item.GetType().Name + " Where " + where;
        //        var hasil = conn.ExecuteReader;
        //        while (hasil.Read())
        //        {
        //            for (int i = 0; i < hasil.FieldCount; i++)
        //            {
        //                Debug.Write(hasil.GetName(i));
        //            }

        //        }

        //        conn.Close();
        //        return item;
        //    }
        //    catch
        //    {
        //        return item;
        //    }


        //}
        #endregion

        public static T GetOneData<T>(this object _temp, T obj)
        {
            T data = Activator.CreateInstance<T>();
            var Primary = new List<string>();
            System.Reflection.MemberInfo _data = data.GetType();
            var attributes = _data.GetCustomAttributes(true);
            foreach (object t in attributes)
            {
                if (t is PrimaryAttribute)
                {
                    var primaryAttribute = t as PrimaryAttribute;
                    Primary.AddRange(primaryAttribute.PrimaryKey.Split(new[] { ',' }));
                }
                //
            }
            string where = "";
            foreach (
            var info in obj.GetType().GetProperties().Where(info => info.GetValue(obj, null) != null))
            {
                try
                {
                    string temp = (info.GetValue(obj, null) is DateTime)
                    ? "'" + ((DateTime)info.GetValue(obj, null)).ToString(conn.FormateDate) + "'"
                    : "'" + info.GetValue(obj, null).ToString() + "'";
                    ;
                    if (Primary.Where(n => n.Equals(info.Name)).Count() != 0)
                    {
                        where += info.Name + " =" + temp + " AND ";
                    }
                }
                catch (Exception)
                {

                }



            }

            try
            {


                where = where.Substring(0, where.Length - 4);
                string nameTable = "";
                if (data.GetType().GetCustomAttributes(true).Where(n => n is TabelNameAttribute).Count() != 0)
                {
                    nameTable = (data.GetType().GetCustomAttributes(true).FirstOrDefault(n => n is TabelNameAttribute) as TabelNameAttribute).Name;
                }
                else
                    nameTable = data.GetType().Name;
                string sql = "Select * from " + nameTable + " Where " + where;
                var hasil = new object().GetDataFromSQL(sql);
                if (hasil != null)
                {
                    hasil.Read();
                    for (int i = 0; i < hasil.FieldCount; i++)
                    {
                        try
                        {

                            switch (data.GetType().GetProperty(hasil.GetName(i)).PropertyType.ToString().ToLower())
                            {
                                case "system.nullable`1[system.int32]":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt32(hasil[i]), null);
                                    break;
                                case "system.nullable`1[system.int16]":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt16(hasil[i]), null);
                                    break;
                                case "system.nullable`1[system.int64]":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt64(hasil[i]), null);
                                    break;
                                case "system.nullable`1[system.byte]":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToByte(hasil[i]), null);

                                    break;
                                case "system.int32":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt32(hasil[i]), null);
                                    break;
                                case "system.int16":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt32(hasil[i]), null);
                                    break;
                                case "system.int64":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt64(hasil[i]), null);
                                    break;
                                case "system.byte":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToByte(hasil[i]), null);
                                    break;
                                case "system.string":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, hasil[i].ToString().Trim(), null);
                                    break;
                                case "system.nullable`1[system.bool]":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToBoolean(hasil[i]), null);
                                    break;
                                case "system.nullable`1[system.datetime]":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDateTime(hasil[i]), null);

                                    break;
                                case "system.nullable`1[system.double]":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDouble(hasil[i]), null);
                                    break;
                                case "system.nullable`1[system.char]":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToChar(hasil[i]), null);

                                    break;
                                case "system.nullable`1[system.decimal]":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDecimal(hasil[i]), null);
                                    break;
                                case "system.bool":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToBoolean(hasil[i]), null);
                                    break;
                                case "system.datetime":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDateTime(hasil[i]), null);
                                    break;
                                case "system.double":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDouble(hasil[i]), null);
                                    break;
                                case "system.char":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToChar(hasil[i]), null);
                                    break;
                                case "system.decimal":
                                    data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDecimal(hasil[i]), null);
                                    break;
                                default:
                                    break;
                            }

                        }
                        catch (Exception)
                        {

                        }
                    }
                    return data;
                }
                else
                {
                    return data;
                }
            }
            catch (Exception)
            {
                return data;
            }
            finally
            {
                if (current != null)
                {
                    current.CloseReader();
                    current.Close();
                    current = null;
                }
            }

        }

        public static object GetOneDataObject(this object _temp, object obj)
        {
            Connection conn = string.IsNullOrEmpty(Connection.ConnectionString) ? new Connection(currentProvider) : new Connection(Connection.ConnectionString, currentProvider);
            object data = Activator.CreateInstance(obj.GetType());
            var Primary = new List<string>();
            System.Reflection.MemberInfo _data = data.GetType();
            var attributes = _data.GetCustomAttributes(true);
            foreach (object t in attributes)
            {
                if (t is PrimaryAttribute)
                {
                    var primaryAttribute = t as PrimaryAttribute;
                    Primary.AddRange(primaryAttribute.PrimaryKey.Split(new[] { ',' }));
                }
                //
            }
            string where = "";
            int cekFillPrimaryCount = 0;
            foreach (
            var info in obj.GetType().GetProperties().Where(info => info.GetValue(obj, null) != null))
            {
                try
                {
                    string temp = (info.GetValue(obj, null) is DateTime)
                    ? "'" + ((DateTime)info.GetValue(obj, null)).ToString(conn.FormateDate) + "'"
                    : "'" + info.GetValue(obj, null).ToString() + "'";
                    ;
                    if (Primary.Where(n => n.Equals(info.Name)).Count() != 0)
                    {
                        cekFillPrimaryCount++;
                        where += info.Name + " =" + temp + " AND ";

                    }
                }
                catch (Exception)
                {

                }



            }
            if (Primary.Count != cekFillPrimaryCount)
            {
                return null;
            }
            try
            {

                if (!string.IsNullOrEmpty(where))
                    where = where.Substring(0, where.Length - 4);
                string nameTable = "";
                if (data.GetType().GetCustomAttributes(true).Where(n => n is TabelNameAttribute).Count() != 0)
                {
                    nameTable = (data.GetType().GetCustomAttributes(true).FirstOrDefault(n => n is TabelNameAttribute) as TabelNameAttribute).Name;
                }
                else
                    nameTable = data.GetType().Name;
                string sql = "Select * from " + nameTable + " Where " + where;
                var hasil = new object().GetDataFromSQL(sql);
                if (hasil != null)
                    if (hasil.HasRows)
                    {
                        hasil.Read();
                        for (int i = 0; i < hasil.FieldCount; i++)
                        {
                            try
                            {
                                string tempGetName = hasil.GetName(i).ToString();
                                switch (data.GetType().GetProperty(hasil.GetName(i)).PropertyType.ToString().ToLower())
                                {
                                    case "system.nullable`1[system.int32]":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt32(hasil[i]), null);
                                        break;
                                    case "system.nullable`1[system.int16]":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt16(hasil[i]), null);
                                        break;
                                    case "system.nullable`1[system.int64]":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt64(hasil[i]), null);
                                        break;
                                    case "system.nullable`1[system.byte]":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToByte(hasil[i]), null);

                                        break;
                                    case "system.int32":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt32(hasil[i]), null);
                                        break;
                                    case "system.int16":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt32(hasil[i]), null);
                                        break;
                                    case "system.int64":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt64(hasil[i]), null);
                                        break;
                                    case "system.byte":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToByte(hasil[i]), null);
                                        break;
                                    case "system.string":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, hasil[i].ToString().Trim(), null);
                                        break;
                                    case "system.nullable`1[system.bool]":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToBoolean(hasil[i]), null);
                                        break;
                                    case "system.nullable`1[system.datetime]":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDateTime(hasil[i]), null);

                                        break;
                                    case "system.nullable`1[system.double]":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDouble(hasil[i]), null);
                                        break;
                                    case "system.nullable`1[system.char]":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToChar(hasil[i]), null);

                                        break;
                                    case "system.nullable`1[system.decimal]":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDecimal(hasil[i]), null);
                                        break;
                                    case "system.bool":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToBoolean(hasil[i]), null);
                                        break;
                                    case "system.datetime":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDateTime(hasil[i]), null);
                                        break;
                                    case "system.double":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDouble(hasil[i]), null);
                                        break;
                                    case "system.char":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToChar(hasil[i]), null);
                                        break;
                                    case "system.decimal":
                                        data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDecimal(hasil[i]), null);
                                        break;
                                    default:
                                        break;
                                }

                            }
                            catch (Exception)
                            {

                            }
                        }
                        return data;
                    }
                    else
                    {
                        return null;
                    }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (current != null)
                {
                    current.CloseReader();
                    current.Close();
                    current = null;
                }
            }

        }

        public static IEnumerable<T> MappingSqlToObject<T>(this object _temp, string sql)
        {
            Connection conn = string.IsNullOrEmpty(Connection.ConnectionString) ? new Connection(currentProvider) : new Connection(Connection.ConnectionString, currentProvider);
            List<T> list = new List<T>();

            var hasil = new object().GetDataFromSQL(sql);

            try
            {
                if (hasil != null)
                    if (!hasil.IsClosed)
                        while (hasil.Read())
                        {
                            T data = Activator.CreateInstance<T>();
                            for (int i = 0; i < hasil.FieldCount; i++)
                            {
                                try
                                {
                                    switch (data.GetType().GetProperty(hasil.GetName(i)).PropertyType.ToString().ToLower())
                                    {
                                        case "system.nullable`1[system.int32]":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt32(hasil[i]), null);
                                            break;
                                        case "system.nullable`1[system.int16]":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt16(hasil[i]), null);
                                            break;
                                        case "system.nullable`1[system.int64]":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt64(hasil[i]), null);
                                            break;
                                        case "system.nullable`1[system.byte]":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToByte(hasil[i]), null);

                                            break;
                                        case "system.int32":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt32(hasil[i]), null);
                                            break;
                                        case "system.int16":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt32(hasil[i]), null);
                                            break;
                                        case "system.int64":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToInt64(hasil[i]), null);
                                            break;
                                        case "system.byte":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToByte(hasil[i]), null);
                                            break;
                                        case "system.string":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, hasil[i].ToString(), null);
                                            break;
                                        case "system.nullable`1[system.bool]":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToBoolean(hasil[i]), null);
                                            break;
                                        case "system.nullable`1[system.datetime]":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDateTime(hasil[i]), null);

                                            break;
                                        case "system.nullable`1[system.double]":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDouble(hasil[i]), null);
                                            break;
                                        case "system.nullable`1[system.char]":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToChar(hasil[i]), null);

                                            break;
                                        case "system.nullable`1[system.decimal]":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDecimal(hasil[i]), null);
                                            break;
                                        case "system.bool":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToBoolean(hasil[i]), null);
                                            break;
                                        case "system.datetime":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDateTime(hasil[i]), null);
                                            break;
                                        case "system.double":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDouble(hasil[i]), null);
                                            break;
                                        case "system.char":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToChar(hasil[i]), null);
                                            break;
                                        case "system.decimal":
                                            data.GetType().GetProperty(hasil.GetName(i)).SetValue(data, Convert.ToDecimal(hasil[i]), null);
                                            break;
                                        default:
                                            break;
                                    }

                                }
                                catch (Exception)
                                {
                                }
                            }
                            list.Add(data);
                        }

                return list;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (current != null)
                {
                    current.CloseReader();
                    current.Close();
                    current = null;
                }
            }
        }

        public static object Where<T>(this  object data, Expression<Func<T, bool>> fuct)
        {
            T obj = Activator.CreateInstance<T>();
            var variableInitialize = new string[fuct.Parameters.Count];
            var i = 0;
            foreach (var VARIABLE in fuct.Parameters)
            {
                variableInitialize[i] = VARIABLE.ToString();
            }
            string hasil = fuct.Body.ToString();

            variableInitialize.ToList().ForEach(n => hasil = hasil.Replace(n + ".", " ").Trim());
            hasil = hasil.Replace("\"", "'");
            hasil = hasil.Replace("==", "=");
            hasil = hasil.Replace(".Equals", " = ");
            hasil = hasil.Replace(".Contains", " Like ");
            hasil = "Select * from " + obj.GetType().Name + " Where " + hasil;
            Debug.Write(hasil);
            return data;
        }
        public static string AreEqual(this object data, string s)
        {
            return data.GetType().Name + "=" + s;
        }
    }


}
