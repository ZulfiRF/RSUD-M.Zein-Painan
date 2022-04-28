using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Framework.Model.Attr;
using Core.Framework.Model.Helper.Odata;

namespace Core.Framework.Model.Impl.SqlLite
{
    /// <summary>
    /// Class SqlConnectionManager
    /// </summary>
    public sealed class SqlConnectionManager : BaseConnectionManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseConnectionManager" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqlConnectionManager(string connectionString)
            : base(connectionString)
        {
            ConnectionString = connectionString;
        }

        public override string ConvertField(string field)
        {
            return "[" + base.ConvertField(field) + "]";
        }
        #region Implementation of IConnectionManager

        /// <summary>
        /// The connection
        /// </summary>
        private IDbConnection connection;

        /// <summary>
        /// Gets the connection. digunakan untuk Connection saat mengakses database
        /// </summary>
        /// <value>The connection.</value>
        /// <exception cref="System.ArgumentNullException">Connection String is Null or Empty</exception>
        public override IDbConnection Connection
        {
            get
            {
                if (string.IsNullOrEmpty(ConnectionString))
                    throw new ArgumentNullException("Connection String is Null or Empty");
                connection = new SQLiteConnection(ConnectionString);
                return connection;
            }
        }

        /// <summary>
        /// Gets the command. digunakan untuk mengelola sql query
        /// </summary>
        /// <value>The command.</value>
        public override IDbCommand Command
        {
            get
            {
                return new SQLiteCommand();
            }
        }

        /// <summary>
        /// Gets the transaction.  digunakan untuk melakukan transaction dan roll back
        /// </summary>
        /// <value>The transaction.</value>
        /// <exception cref="System.ArgumentNullException">Conection Manager is Null</exception>
        public override IDbTransaction Transaction
        {
            get
            {
                if (connection != null)
                    return connection.BeginTransaction();
                throw new ArgumentNullException("Conection Manager is Null");
            }
        }

        /// <summary>
        /// Gets or sets the connection string. digunakan untuk menyeting connection string
        /// </summary>
        /// <value>The connection string.</value>
        public override string ConnectionString { get; set; }

        /// <summary>
        /// Gets the formate date. digunakan untuk konversi date time ke bentuk string
        /// </summary>
        /// <value>The formate date.</value>
        public override string FormateDate
        {
            get { return "yyyy-MM-dd hh:mm:00"; }
        }

        /// <summary>
        /// Converts the boolean. digunakan untuk konversi boolean ke bentuk string
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>System.String.</returns>
        public override string ConvertBoolean(bool value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Creates the table.digunakan untuk membuat table
        /// </summary>
        /// <param name="item">berisikan object yang akan di buat table</param>
        /// <returns><c>true</c> jika berhasil melakakukan penyimpanan pada database, <c>false</c> jika terjadi kesalahan pada proses penyimpanan pada database</returns>
        public override bool CreateTable(TableItem item)
        {
            bool valid = false;
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                using (var command = new SQLiteCommand())
                {
                    command.Connection = conn;
                    command.CommandText = "SELECT * FROM sqlite_master  WHERE name = '" + item.TableName + "'";
                    var read = command.ExecuteReader();
                    while (read.Read())
                    {
                        valid = true;
                    }
                }
            }
            string query = "CREATE TABLE \"" + item.TableName + "\"( \n";
            foreach (var field in item.Fields)
            {
                query += " \"" + field.FieldName + "\" " + MatchType(field) + " " + (item.PrimaryKeys.Any(n => n.Equals(field.FieldName)) ? "PRIMARY KEY" : "") + " " + ((field.IsAllowNull == SpesicicationType.AllowNull) ? "NULL" : "NOT NULL") + ", \n";
            }


            query = query.Substring(0, query.Length - 3);
            string checkExist = "";
            switch (item.AutoDropTable)
            {
                case UpdateTableType.AutoDropAction:
                    checkExist += " drop table " + item.TableName + " \n";
                    checkExist += query + "\n";
                    break;

                case UpdateTableType.NothingAction:
                    if (!valid)
                        checkExist += query + ")";
                    break;

                case UpdateTableType.AutoRevision:
                    break;
            }




            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                using (var command = new SQLiteCommand())
                {
                    command.Connection = conn;
                    command.CommandText = checkExist;
                    command.ExecuteNonQuery();
                }
            }
            return true;
        }

        /// <summary>
        /// Matches the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.String.</returns>
        private string MatchType(FieldAttribute type)
        {
            if (type.Info.PropertyType.Name.Contains("String"))
            {
                if (type.Length == -1)
                    return "[Text]";
                return "[varchar] (" + ((type.Length == null || type.Length == 0) ? "255" : type.Length.ToString()) +
                       ")";
            }
            if (type.Info.PropertyType.IsEnum)
            {
                return "[varchar] (1)";
            }
            switch (type.Info.PropertyType.Name)
            {
                case "System.Int16":
                case "Int16":
                    return "[INTEGER]";
                case "System.Int32":
                case "Int32":
                    return "[int]";
                case "System.Int64":
                case "Int64":
                    return "[INTEGER]";

                case "System.Boolean":
                case "Boolean":
                    return "[BOOL]";

                case "System.DateTime":
                case "DateTime":
                    return "[DATETIME]";
                case "System.Byte":
                case "Byte":
                    return "[INTEGER]";
                case "System.Decimal":
                case "Decimal":
                    return "[DOUBLE]";

                case "System.Double":
                case "Double":
                case "System.Single":
                case "Single":
                    return "[DOUBLE]";
            }
            switch (type.Info.ToString().Split(new[] { ' ' })[0])
            {
                case "System.Nullable`1[System.Int16]":
                case "System.Nullable`1[Int16]":
                    return "[INTEGER]";
                case "System.Nullable`1[System.Int32]":
                case "System.Nullable`1[Int32]":
                    return "[int]";
                case "System.Nullable`1[System.Int64]":
                case "System.Nullable`1[Int64]":
                    return "[INTEGER]";

                case "System.Nullable`1[System.Boolean]":
                case "System.Nullable`1[Boolean]":
                    return "[BOOL]";

                case "System.Nullable`1[System.DateTime]":
                case "System.Nullable`1[DateTime]":
                    return "[DATETIME]";
                case "System.Nullable`1[System.Byte]":
                case "System.Nullable`1[Byte]":
                    return "[INTEGER]";
                case "System.Nullable`1[System.Decimal]":
                case "System.Nullable`1[Decimal]":
                    return "[DECIMAL]";

                case "System.Nullable`1[System.Double]":
                case "System.Nullable`1[Double]":
                case "System.Nullable`1[System.Single]":
                case "System.Nullable`1[Single]":
                    return "[DOUBLE]";
            }
            return "";
        }

        /// <summary>
        /// Creates the database.
        /// </summary>
        /// <returns><c>true</c> jika berhasil melakakukan pembuatan pada database, <c>false</c> jika terjadi kesalahan pada proses pembuatan database</returns>
        public override bool CreateDatabase()
        {
            try
            {
                var dictionary = new Dictionary<string, string>();
                foreach (var key in ConnectionString.Split(new[] { ';' }))
                {
                    var arr = key.Split(new[] { '=' });
                    if (arr.Length == 2)
                        dictionary.Add(arr[0], arr[1]);
                }
                string connection = "";
                foreach (var value in dictionary)
                {
                    if (!value.Key.ToLower().Equals("initial catalog"))
                    {
                        connection += value.Key + "=" + value.Value + ";";
                    }
                }
                connection += "Initial Catalog=master;";
                using (var conn = new SQLiteConnection(connection))
                {
                    conn.ConnectionString = connection;
                    conn.Open();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        #endregion Implementation of IConnectionManager

        #region Implementation of IFilterQuery

        /// <summary>
        /// The dictionary
        /// </summary>
        private readonly Dictionary<string, string> dictionary = new Dictionary<string, string>();

        /// <summary>
        /// Equals the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public override string Equal(string input)
        {
            var split = Regex.Split(input, " eq ");
            if (split.Length > 2)
            {
                foreach (var dict in dictionary.OrderByDescending(n => n.Value.Length))
                {
                    if (input.Contains(dict.Key))
                        input = input.Replace(dict.Key, dict.Value);
                }
            }
            split = Regex.Split(input, " eq ");
            if (split.Length == 2)
                return split[0] + " = '" + split[1] + "'";
            return input;
        }

        /// <summary>
        /// Nulls the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public override string Null(string input)
        {
            var split = Regex.Split(input, "null");
            if (split[0].Trim().EndsWith("="))
                input = input.Replace("=", "is");
            else if (split[0].Trim().EndsWith("<>"))
            {
                input = input.Replace("<>", "is");
                var tempSplit = input.Split(new[] { ' ' });
                int index = 0;
                input = "";
                if (tempSplit.Length == 3)
                {
                    input = " not " + tempSplit[0] + " " + tempSplit[1] + " " + tempSplit[2];
                }
                else
                    foreach (var s in tempSplit)
                    {
                        if (index == 1)
                            input += " not ";
                        input += s + " ";
                        index++;
                    }
            }

            return input;
        }

        /// <summary>
        /// Nots the equal.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public override string NotEqual(string input)
        {
            var split = Regex.Split(input, " ne ");
            if (split.Length > 2)
            {
                foreach (var dict in dictionary.OrderByDescending(n => n.Value.Length))
                {
                    if (input.Contains(dict.Key))
                        input = input.Replace(dict.Key, dict.Value);
                }
            }
            split = Regex.Split(input, " ne ");
            if (split.Length == 2)
                return split[0] + " <> '" + split[1] + "'";
            return input;
        }

        /// <summary>
        /// Greaters the than.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public override string GreaterThan(string input)
        {
            var split = Regex.Split(input, " gt ");
            if (split.Length > 2)
            {
                foreach (var dict in dictionary.OrderByDescending(n => n.Value.Length))
                {
                    if (input.Contains(dict.Key))
                        input = input.Replace(dict.Key, dict.Value);
                }
            }
            split = Regex.Split(input, " gt ");
            if (split.Length == 2)
                return split[0] + " > " + split[1] + "";
            return input;
        }

        /// <summary>
        /// Greaters the than or equal.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public override string GreaterThanOrEqual(string input)
        {
            var split = Regex.Split(input, " ge ");
            if (split.Length > 2)
            {
                foreach (var dict in dictionary.OrderByDescending(n => n.Value.Length))
                {
                    if (input.Contains(dict.Key))
                        input = input.Replace(dict.Key, dict.Value);
                }
            }
            split = Regex.Split(input, " ge ");
            if (split.Length == 2)
                return split[0] + " >= " + split[1] + "";
            return input;
        }

        /// <summary>
        /// Lesses the than.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public override string LessThan(string input)
        {
            var split = Regex.Split(input, " lt ");
            if (split.Length > 2)
            {
                foreach (var dict in dictionary.OrderByDescending(n => n.Value.Length))
                {
                    if (input.Contains(dict.Key))
                        input = input.Replace(dict.Key, dict.Value);
                }
            }
            split = Regex.Split(input, " lt ");
            if (split.Length == 2)
                return split[0] + " < " + split[1] + "";
            return input;
        }

        /// <summary>
        /// Lesses the than or equal.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public override string LessThanOrEqual(string input)
        {
            var split = Regex.Split(input, " le ");
            if (split.Length > 2)
            {
                foreach (var dict in dictionary.OrderByDescending(n => n.Value.Length))
                {
                    if (input.Contains(dict.Key))
                        input = input.Replace(dict.Key, dict.Value);
                }
            }
            split = Regex.Split(input, " le ");
            if (split.Length == 2)
                return split[0] + " <= " + split[1] + "";
            return input;
        }

        /// <summary>
        /// Ands the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public override string And(string input)
        {
            var split = Regex.Split(input, " and ");
            if (split.Length > 2)
            {
                foreach (var dict in dictionary.OrderByDescending(n => n.Value.Length))
                {
                    if (input.Contains(dict.Key))
                        input = input.Replace(dict.Key, dict.Value);
                }
            }
            split = Regex.Split(input, " and ");
            if (split.Length == 2)
                return split[0] + " and " + split[1] + "";
            return input;
        }

        /// <summary>
        /// Ors the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public override string Or(string input)
        {
            var split = Regex.Split(input, " or ");
            if (split.Length > 2)
            {
                foreach (var dict in dictionary.OrderByDescending(n => n.Value.Length))
                {
                    if (input.Contains(dict.Key))
                        input = input.Replace(dict.Key, dict.Value);
                }
            }
            split = Regex.Split(input, " or ");
            if (split.Length == 2)
                return split[0] + " or " + split[1] + "";
            return input;
        }

        /// <summary>
        /// Nots the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public override string Not(string input)
        {
            var split = Regex.Split(input, " not ");
            if (split.Length > 2)
            {
                foreach (var dict in dictionary.OrderByDescending(n => n.Value.Length))
                {
                    if (input.Contains(dict.Key))
                        input = input.Replace(dict.Key, dict.Value);
                }
            }
            split = Regex.Split(input, "not");
            if (split.Length == 2)
                return split[0] + " not " + split[1] + "";
            return input;
        }

        /// <summary>
        /// Startswithes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public override string Startswith(string input)
        {
            if (input.StartsWith("startswith"))
            {
                var split = Regex.Split(input, "eq");
                if (split.Length == 2)
                {
                    var keyvalueSplit = Regex.Split(Regex.Match(split[0], @"\(([^)]*)\)").Groups[1].Value, ",");
                    if (split[1].Trim().ToLower().Equals("true"))
                        return keyvalueSplit[0] + " LIKE '" + keyvalueSplit[1].Replace("'", "") + "%'";
                }
            }
            return input;
        }

        /// <summary>
        /// Endwithes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public override string Endwith(string input)
        {
            if (input.StartsWith("endswith"))
            {
                var split = Regex.Split(input, "eq");
                if (split.Length == 2)
                {
                    var keyvalueSplit = Regex.Split(Regex.Match(split[0], @"\(([^)]*)\)").Groups[1].Value, ",");
                    if (split[1].Trim().ToLower().Equals("true"))
                        return keyvalueSplit[0] + " LIKE '%" + keyvalueSplit[1].Replace("'", "") + "'";
                }
            }
            return input;
        }

        /// <summary>
        /// Determines whether [contains] [the specified input].
        /// </summary>
        /// <param name="input">The input.</param>
        public override string Contains(string input)
        {
            if (input.StartsWith("contains"))
            {
                var split = Regex.Split(input, "eq");
                if (split.Length == 2)
                {
                    var keyvalueSplit = Regex.Split(Regex.Match(split[0], @"\(([^)]*)\)").Groups[1].Value, ",");
                    if (split[1].Trim().ToLower().Equals("true"))
                        return keyvalueSplit[0] + " LIKE '%" + keyvalueSplit[1].Replace("'", "") + "%'";
                    if (split[1].Trim().ToLower().StartsWith("true"))
                    {
                        return keyvalueSplit[0] + " LIKE '%" + keyvalueSplit[1].Replace("'", "") + "%'" + split[1].Trim().Substring(4);
                    }
                }
            }
            return input;
        }

        /// <summary>
        /// Creates the filter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>ResultQuery.</returns>
        public override ResultQuery CreateFilter(string input)
        {
            var data = Split(input, '[', ']');
            input = input.Replace("[", "").Replace("]", "");
            input = data.Aggregate(input, (current, wordRelation) => current.Replace(wordRelation, ""));
            var result = "";
            foreach (var query in input.Split(new[] { " and ", " or " }, StringSplitOptions.RemoveEmptyEntries))
            {
                var operate = input.IndexOf(query);
                var temp = Startswith(query.Trim());
                temp = Contains(temp);
                temp = Endwith(temp);
                temp = Equal(temp);
                temp = GreaterThan(temp);
                temp = GreaterThanOrEqual(temp);
                temp = LessThan(temp);
                temp = LessThanOrEqual(temp);
                temp = And(temp);
                temp = Or(temp);
                temp = Not(temp);
                temp = NotEqual(temp);
                temp = Null(temp);
                if (operate != 0)
                {
                    var operatCheck = input.Substring(operate - 4, 3);
                    if (operatCheck.Contains("or"))
                        result += " or ";
                    else if (operatCheck.Contains("and"))
                        result += " and ";
                }
                result += temp;
            }

            List<string> strList = new List<string>();
            foreach (var str in data)
            {
                strList.Add(CreateFilter(str).Filter);
            }
            return new ResultQuery()
            {
                Filter = result,
                Relation = strList
            };
        }

        /// <summary>
        /// Splits the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="bracketUp">The bracket up.</param>
        /// <param name="bracketDown">The bracket down.</param>
        /// <returns>List{System.String}.</returns>
        private List<string> Split(string input, char bracketUp, char bracketDown)
        {
            int indexOpenBracket = 0, indexCharacterOpen = 0;
            var hasUseIndex = new List<int>();
            var listString = new List<string>();
            foreach (var chareacter in input.ToCharArray().Reverse())
            {
                if (chareacter.Equals(bracketUp))
                {
                    indexOpenBracket++;
                    for (int i = input.Length - indexCharacterOpen; i < input.Length; i++)
                    {
                        if (input[i].Equals(bracketDown))
                        {
                            if (hasUseIndex.Count(n => n == i) == 0)
                            {
                                hasUseIndex.Add(i);
                                string temp = input.Substring(input.Length - indexCharacterOpen, i - (input.Length - indexCharacterOpen));
                                string result = temp;

                                // result = CreateFilter(result);
                                try
                                {
                                    var operate = input.Substring(input.Length - indexCharacterOpen - 5, 4);
                                    foreach (var opr in operate.Split(new[] { ' ' }))
                                    {
                                        if (opr.Equals("and") || opr.Equals("or") || opr.Equals("ne"))
                                        {
                                            listString.Add(opr + " " + result.Trim());
                                            break;
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    listString.Add(result.Trim());
                                }

                                break;
                            }
                        }
                    }
                }
                indexCharacterOpen++;
            }
            return listString;
        }

        #endregion Implementation of IFilterQuery
    }
}