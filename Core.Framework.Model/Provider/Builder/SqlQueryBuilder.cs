using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Framework.Model.Provider.Adapter;
using Core.Framework.Model.Provider.ValueObjects;

namespace Core.Framework.Model.Provider.Builder
{
    public class SqlQueryBuilder
    {
        private const string PARAMETER_PREFIX = "Param";
        private bool isExists;
        private int top = int.MaxValue;
        private readonly List<string> _conditions = new List<string>();
        private readonly List<string> _groupingList = new List<string>();
        private readonly List<string> _havingConditions = new List<string>();
        private readonly List<string> _joinExpressions = new List<string>();
        private List<string> _selectionList = new List<string>();
        private List<string> _updateList = new List<string>();
        private readonly List<string> _sortList = new List<string>();
        private readonly List<string> _splitColumns = new List<string>();
        private readonly List<string> _tableNames = new List<string>();

        internal SqlQueryBuilder(string tableName, ISqlAdapter adapter)
        {
            _tableNames.Add(tableName);
            Adapter = adapter;
            Parameters = new ExpandoObject();
            CurrentParamIndex = 0;
        }

        internal ISqlAdapter Adapter { get; set; }

        private string Conditions
        {
            get
            {
                if (_conditions.Count == 0)
                {
                    return "";
                }
                return ("WHERE " + string.Join("", _conditions));
            }
        }

        public int CurrentParamIndex { get; private set; }

        public List<string> GroupByList
        {
            get { return _groupingList; }
        }

        private string Grouping
        {
            get
            {
                if (_groupingList.Count == 0)
                {
                    return "";
                }
                return ("GROUP BY " + string.Join(", ", _groupingList));
            }
        }

        private string Having
        {
            get
            {
                if (_havingConditions.Count == 0)
                {
                    return "";
                }
                return ("HAVING " + string.Join(" ", _havingConditions));
            }
        }

        public List<string> HavingConditions
        {
            get { return _havingConditions; }
        }

        public List<string> JoinExpressions
        {
            get { return _joinExpressions; }
        }

        private string Order
        {
            get
            {
                if (_sortList.Count == 0)
                {
                    return "";
                }
                return ("ORDER BY " + string.Join(", ", _sortList));
            }
        }

        public List<string> OrderByList
        {
            get { return _sortList; }
        }

        public IDictionary<string, object> Parameters { get; private set; }

        public string QueryString
        {
            get { return Adapter.QueryString(Selection, Source, Conditions, Grouping, Having, Order, isExists, top); }
        }

        public string QueryDelete
        {
            get
            {
                return Adapter.QueryDelete(Source, Conditions);
            }
        }

        public string QueryUpdate
        {
            get { return Adapter.QueryUpdate(Updated, Source, Conditions); }
        }

        public string Updated
        {
            get
            {
                return " " + string.Join(", ", _updateList);
            }
        }

        private string Selection
        {
            get
            {
                if (_selectionList.Count == 0)
                {
                    return string.Format("{0}.*", Adapter.Table(_tableNames.First()));
                }
                if (listSelection.Count != 0)
                {
                    foreach (var selection in _selectionList)
                    {
                        var tempSelection = selection;
                        var valid = false;
                        foreach (var names in typeof(SelectFunction).GetEnumNames())
                        {
                            if (tempSelection.Contains(names))
                            {
                                valid = true;
                                break;
                            }
                        }
                        if (valid) continue;
                        var temp = selection.Replace("[", "").Replace("]", "").Split(new[] { '.' });
                        if (!listSelection.Any(n => n.Key.Equals(temp[0]) && n.Value.Equals(temp[1])))
                        {
                            GroupBy(temp[0], temp[1]);
                        }
                    }

                }
                return string.Join(", ", _selectionList);
            }
        }

        public List<string> SelectionList
        {
            get { return _selectionList; }
            set { _selectionList = value; }
        }


        private string Source
        {
            get
            {
                var str = string.Join(" ", _joinExpressions);
                return string.Format("{0} {1}", Adapter.Table(_tableNames.First()), str);
            }
        }

        public List<string> SplitColumns
        {
            get { return _splitColumns; }
        }

        public List<string> TableNames
        {
            get { return _tableNames; }
        }

        public List<string> WhereConditions
        {
            get { return _conditions; }
        }

        public bool HaveGroup { get; set; }

        private void AddParameter(string key, object value)
        {
            if (!Parameters.ContainsKey(key))
            {
                Parameters.Add(key, value);
            }
        }

        public void And()
        {
            if (_conditions.Count > 0)
            {
                _conditions.Add(" AND ");
            }
        }

        public void BeginExpression()
        {
            _conditions.Add("(");
        }

        public void EndExpression()
        {
            _conditions.Add(")");
        }

        public void AddValue(object value)
        {
            _conditions.Add(value.ToString());
        }
        public void Update(string fieldName, string value)
        {
            _updateList.Add(Adapter.Function(fieldName, value));
        }
        public void GroupBy(string tableName, string fieldName)
        {
            _groupingList.Add(Adapter.Field(tableName, fieldName));
        }

        public void Join(string originalTableName, string joinTableName, string leftField, string rightField)
        {
            var item = string.Format("JOIN {0} ON {1} = {2}", Adapter.Table(joinTableName),
                Adapter.Field(originalTableName, leftField), Adapter.Field(joinTableName, rightField));
            _tableNames.Add(joinTableName);
            _joinExpressions.Add(item);
            _splitColumns.Add(rightField);
        }
        private List<string> originalTableHasAdd = new List<string>();
        public void JoinMultiple(string originalTableName, string joinTableName, params string[] query)
        {
            if (originalTableHasAdd.Any(n => n.Equals(originalTableName + "+" + joinTableName))) return;
            originalTableHasAdd.Add(originalTableName + "+" + joinTableName);
            var item = string.Format(" INNER JOIN {0} ON ", Adapter.Table(joinTableName));
            var firstLoad = false;
            foreach (var s in query)
            {
                var relation = s.Split('=');
                if (!firstLoad)
                    item += "[" + originalTableName + "].[" + relation[0] + "] = " + "[" + joinTableName + "].[" +
                            relation[1] + "]";
                else
                    item += " AND [" + originalTableName + "].[" + relation[0] + "] = " + "[" + joinTableName + "].[" +
                            relation[1] + "]";
                firstLoad = true;
            }
            _tableNames.Add(joinTableName);
            _joinExpressions.Add(item);
            //_splitColumns.Add(rightField);
        }

        private string NextParamId()
        {
            CurrentParamIndex++;
            return ("Param" + CurrentParamIndex.ToString(CultureInfo.InvariantCulture));
        }

        public void Not()
        {
            _conditions.Add(" NOT ");
        }

        public void Or()
        {
            if (_conditions.Count > 0)
            {
                _conditions.Add(" OR ");
            }
        }
        public void Equal()
        {
            if (_conditions.Count > 0)
            {
                _conditions.Add(" = ");
            }
        }

        public void OrderBy(string tableName, string fieldName, bool desc = false)
        {
            var item = Adapter.Field(tableName, fieldName);
            if (desc)
            {
                item = item + " DESC";
            }
            _sortList.Add(item);
        }
        public void QueryByField(string tableName, string fieldName)
        {
            var parameterId = NextParamId();
            string item = null;

            item = string.Format("{0}  ", Adapter.Field(tableName, fieldName));
            _conditions.Add(item);
        }
        public void QueryByField(string tableName, string fieldName, string op, object fieldValue)
        {
            var parameterId = NextParamId();
            string item = null;
            if (fieldValue != null)
            {
                item = string.Format("{0} {1} {2}", Adapter.Field(tableName, fieldName, fieldValue.GetType().Name), op,
                    Adapter.Parameter(parameterId));
            }
            else
                item = string.Format("{0} {1} {2}", Adapter.Field(tableName, fieldName), op,
                    Adapter.Parameter(parameterId));
            _conditions.Add(item);

            DateTime dateField;
            if (fieldValue is DateTime)
            {
                dateField = Convert.ToDateTime(fieldValue);
                fieldValue = dateField.ToString("yyyy/MM/dd HH:mm:ss");
            }
            // di comment masih salah kalau datetime ?
            //if (DateTime.TryParse(fieldValue as string, out dateField))
            //    fieldValue = dateField.ToString("yyyy/MM/dd");
            //fieldValue = (fieldValue is DateTime ? ((DateTime)fieldValue).ToString("yyyy/MM/dd") : fieldValue);

            AddParameter(parameterId, fieldValue);
        }

        public void QueryByFieldComparison(string leftTableName, string leftFieldName, string op, string rightTableName,
            string rightFieldName)
        {
            var item = string.Format("{0} {1} {2}", Adapter.Field(leftTableName, leftFieldName), op,
                Adapter.Field(rightTableName, rightFieldName));
            _conditions.Add(item);
        }

        public void QueryByFieldLike(string tableName, string fieldName, string fieldValue)
        {
            var parameterId = NextParamId();
            var item = "";

            if (fieldValue.Contains("%"))
                item = string.Format("{0} LIKE {1}", Adapter.Field(tableName, fieldName),
                         Adapter.Parameter(parameterId));
            else if (fieldValue.Contains("and"))
                item = string.Format("{0} between {1}", Adapter.Field(tableName, fieldName),
                Adapter.Parameter(parameterId));
            else
                item = string.Format("{0} LIKE {1}", Adapter.Field(tableName, fieldName),
                         Adapter.Parameter(parameterId));

            //if (fieldValue.Contains("and"))
            //    item = string.Format("{0} between {1}", Adapter.Field(tableName, fieldName),
            //    Adapter.Parameter(parameterId));
            //else
            //    item = string.Format("{0} LIKE {1}", Adapter.Field(tableName, fieldName),
            //         Adapter.Parameter(parameterId));
            _conditions.Add(item);
            AddParameter(parameterId, fieldValue);
        }

        public void QueryByFieldNotNull(string tableName, string fieldName)
        {
            _conditions.Add(string.Format("{0} IS NOT NULL", Adapter.Field(tableName, fieldName)));
        }

        public void QueryByFieldNull(string tableName, string fieldName)
        {
            _conditions.Add(string.Format("{0} IS NULL", Adapter.Field(tableName, fieldName)));
        }

        public void QueryByIsIn(string tableName, string fieldName, SqlLamBase sqlQuery)
        {
            var queryString = sqlQuery.QueryString;
            foreach (var pair in sqlQuery.QueryParameters)
            {
                var replacement = "Inner" + pair.Key;
                queryString = Regex.Replace(queryString, pair.Key, replacement);
                AddParameter(replacement, pair.Value);
            }
            var item = string.Format("{0} IN ({1})", Adapter.Field(tableName, fieldName), queryString);
            _conditions.Add(item);
        }

        public void QueryByIsIn(string tableName, string fieldName, IEnumerable<object> values)
        {
            var enumerable = values.Select(delegate (object x)
            {
                var key = NextParamId();
                AddParameter(key, x);
                return Adapter.Parameter(key);
            });
            var item = string.Format("{0} IN ({1})", Adapter.Field(tableName, fieldName),
                string.Join(",", enumerable));
            _conditions.Add(item);
        }

        public string QueryStringPage(int pageSize, int? pageNumber = new int?())
        {
            if (!pageNumber.HasValue)
            {
                return Adapter.QueryStringPage(Source, Selection, Conditions, Order, pageSize);
            }
            if (_sortList.Count == 0)
            {
                throw new Exception("Pagination requires the ORDER BY statement to be specified");
            }
            return Adapter.QueryStringPage(Source, Selection, Conditions, Order, pageSize, pageNumber.Value);
        }

        public void Select(string tableName)
        {
            var item = string.Format("{0}.*", Adapter.Table(tableName));

            _selectionList.Add(item);
        }
        public List<KeyValuePair<string, string>> listSelection = new List<KeyValuePair<string, string>>();
        public void Select(string tableName, string fieldName)
        {
            _selectionList.Add(Adapter.Field(tableName, fieldName));
        }

        public void Select(string tableName, string fieldName, SelectFunction selectFunction)
        {
            listSelection.Add(new KeyValuePair<string, string>(tableName, fieldName));
            var item = string.Format("{0}({1})", selectFunction, Adapter.Field(tableName, fieldName));
            _selectionList.Add(item);
        }

        public void Exists()
        {
            isExists = true;
        }

        public void Top(int topData)
        {
            top = topData;
        }

        public void Remove(string tableName, string fieldName, SelectFunction selectFunction)
        {
            var item = string.Format("{0}({1})", selectFunction, Adapter.Field(tableName, fieldName));
            _selectionList.Remove(item);
        }

        public string QueryInsertInto(string tableName)
        {
            return Adapter.QueryInsertInto(tableName);
        }

        public void Multiply()
        {
            if (_conditions.Count > 0)
            {
                _conditions.Add(" * ");
            }
        }

        public void Add()
        {
            if (_conditions.Count > 0)
            {
                _conditions.Add(" * ");
            }
        }

        public void Divide()
        {
            if (_conditions.Count > 0)
            {
                _conditions.Add(" * ");
            }
        }

        public void Subtract()
        {
            if (_conditions.Count > 0)
            {
                _conditions.Add(" * ");
            }
        }

        public void LessThan()
        {
            if (_conditions.Count > 0)
            {
                _conditions.Add(" < ");
            }
        }

        public void LessThanOrEqual()
        {
            if (_conditions.Count > 0)
            {
                _conditions.Add(" <= ");
            }
        }

        public void GreaterThan()
        {
            if (_conditions.Count > 0)
            {
                _conditions.Add(" > ");
            }
        }

        public void GreaterThanOrEqual()
        {
            if (_conditions.Count > 0)
            {
                _conditions.Add(" >= ");
            }
        }
    }
}