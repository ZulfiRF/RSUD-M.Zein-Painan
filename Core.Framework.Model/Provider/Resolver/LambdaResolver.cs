using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Core.Framework.Helper;
using Core.Framework.Model.Attr;
using Core.Framework.Model.Helper;
using Core.Framework.Model.Provider.Builder;
using Core.Framework.Model.Provider.Resolver.ExpressionTree;
using Core.Framework.Model.Provider.ValueObjects;

namespace Core.Framework.Model.Provider.Resolver
{
    public class LambdaResolver
    {
        internal CoreDictionary<string, string> TableNamesDictionary;
        private readonly Dictionary<ExpressionType, string> _operationDictionary;
        private readonly Dictionary<LikeMethod, string> functionDictionary;

        public LambdaResolver(SqlQueryBuilder builder)
        {
            TableNamesDictionary = new CoreDictionary<string, string>();
            var dictionary = new Dictionary<ExpressionType, string>();
            dictionary.Add(ExpressionType.Equal, "=");
            dictionary.Add(ExpressionType.NotEqual, "!=");
            dictionary.Add(ExpressionType.GreaterThan, ">");
            dictionary.Add(ExpressionType.LessThan, "<");
            dictionary.Add(ExpressionType.GreaterThanOrEqual, ">=");
            dictionary.Add(ExpressionType.LessThanOrEqual, "<=");
            dictionary.Add(ExpressionType.Add, "+");
            dictionary.Add(ExpressionType.Subtract, "-");
            dictionary.Add(ExpressionType.Divide, "/");
            dictionary.Add(ExpressionType.Multiply, "*");

            var dictionaryFunction = new Dictionary<LikeMethod, string>();

            dictionaryFunction.Add(LikeMethod.In, "In");
            dictionaryFunction.Add(LikeMethod.NotIn, "Not In");
            _operationDictionary = dictionary;
            functionDictionary = dictionaryFunction;
            _builder = builder;
        }

        private SqlQueryBuilder _builder { get; set; }
        public string TablePrimary { get; set; }
        public Type TypeTablePrimary { get; set; }

        private void BuildSql(LikeNode node)
        {
            if (node.Method == LikeMethod.Equals)
            {
                _builder.QueryByField(node.MemberNode.TableName, node.MemberNode.FieldName,
                    _operationDictionary[ExpressionType.Equal], node.Value);
            }
            else if (node.Method == LikeMethod.In || node.Method == LikeMethod.NotIn)
            {
                _builder.QueryByField(node.MemberNode.TableName, node.MemberNode.FieldName,
                    functionDictionary[node.Method], node.Value);
            }
            else
            {
                var fieldValue = node.Value;
                switch (node.Method)
                {
                    case LikeMethod.StartsWith:
                        fieldValue = node.Value + "%";
                        break;

                    case LikeMethod.EndsWith:
                        fieldValue = "%" + node.Value;
                        break;
                    case LikeMethod.Contains:
                        fieldValue = "%" + node.Value + "%";
                        break;
                }
                _builder.QueryByFieldLike(node.MemberNode.TableName, node.MemberNode.FieldName, fieldValue);
            }
        }

        private void BuildSql(MemberNode memberNode)
        {
            //caun
            _builder.QueryByField(memberNode.TableName, memberNode.FieldName);
            //karena ga semua query harus di ada nilai pembanding
            //, _operationDictionary[ExpressionType.Equal],
            //    true);
        }

        private void BuildSql(Node node)
        {
            this.BuildSql((dynamic)node);
        }

        private void BuildSql(ValueNode node)
        {
            _builder.AddValue(node.Value);
        }

        private void BuildSql(OperationNode node)
        {
            this.BuildSql((dynamic)node.Left, (dynamic)node.Right, node.Operator);
        }

        private void BuildSql(SingleOperationNode node)
        {
            if (node.Operator == ExpressionType.Not)
            {
                _builder.Not();
            }
            BuildSql(node.Child);
        }

        private void BuildSql(MemberNode leftMember, MemberNode rightMember, ExpressionType op)
        {
            _builder.QueryByFieldComparison(leftMember.TableName, leftMember.FieldName, _operationDictionary[op],
                rightMember.TableName, rightMember.FieldName);
        }

        private void BuildSql(MemberNode memberNode, ValueNode valueNode, ExpressionType op)
        {
            if (valueNode.Value == null)
            {
                ResolveNullValue(memberNode, op);
            }
            else
            {
                _builder.QueryByField(memberNode.TableName, memberNode.FieldName, _operationDictionary[op],
                    valueNode.Value);
            }
        }

        private void BuildSql(Node leftNode, Node rightNode, ExpressionType op)
        {
            _builder.BeginExpression();
            this.BuildSql((dynamic)leftNode);
            ResolveOperation(op);
            this.BuildSql((dynamic)rightNode);
            _builder.EndExpression();
        }

        private void BuildSql(Node leftMember, SingleOperationNode rightMember, ExpressionType op)
        {
            BuildSql(rightMember, leftMember, op);
        }

        private void BuildSql(SingleOperationNode leftMember, SingleOperationNode rightMember, ExpressionType op)
        {
            if (leftMember.Operator == ExpressionType.Not)
            {
                BuildSql((Node)leftMember, rightMember, op);
            }
            else
            {
                this.BuildSql((dynamic)leftMember.Child, (dynamic)rightMember, op);
            }
        }

        private void BuildSql(SingleOperationNode leftMember, Node rightMember, ExpressionType op)
        {
            if (leftMember.Operator == ExpressionType.Not)
            {
                BuildSql((Node)leftMember, rightMember, op);
            }
            else
            {
                this.BuildSql((dynamic)leftMember.Child, (dynamic)rightMember, op);
            }
        }

        private void BuildSql(ValueNode valueNode, MemberNode memberNode, ExpressionType op)
        {
            BuildSql(memberNode, valueNode, op);
        }

        private static BinaryExpression GetBinaryExpression(Expression expression)
        {
            if (!(expression is BinaryExpression))
            {
                throw new ArgumentException("Binary expression expected");
            }
            return (expression as BinaryExpression);
        }

        public static string GetColumnName(Expression expression)
        {
            var memberExpression = GetMemberExpression(expression);
            var attribute =
                memberExpression.Member.GetCustomAttributes(false)
                    .OfType<FieldAttribute>()
                    .FirstOrDefault<FieldAttribute>();
            if (attribute != null)
            {
                return attribute.FieldName;
            }


            return memberExpression.Member.Name;
        }

        public static string GetColumnName<T>(Expression<Func<T, object>> selector)
        {
            return GetColumnName(GetMemberExpression(selector.Body));
        }

        private object GetExpressionValue(Expression expression)
        {
            var nodeType = expression.NodeType;

            if (nodeType == ExpressionType.Call)
            {
                return ResolveMethodCall(expression as MethodCallExpression);
            }

            if (nodeType == ExpressionType.Lambda)
                return null;
            if (nodeType != ExpressionType.New)
                if (nodeType != ExpressionType.Constant)
                {
                    if (nodeType != ExpressionType.MemberAccess)
                    {
                        if (nodeType != ExpressionType.Quote)
                        {
                            if (nodeType == ExpressionType.Convert)
                            {
                                var unaryExpression = expression as UnaryExpression;
                                return ResolveQuery(unaryExpression);
                            }
                            if (nodeType == ExpressionType.NewArrayInit)
                            {
                                var e = expression as NewArrayExpression;
                                var stringBuilder = new StringBuilder();
                                var first = true;
                                foreach (var expression1 in e.Expressions)
                                {
                                    if (first)
                                    {
                                        stringBuilder.Append("'" + (expression1 as ConstantExpression).Value + "'");
                                        first = false;
                                    }
                                    else
                                    {
                                        stringBuilder.Append(",'" + (expression1 as ConstantExpression).Value + "'");
                                    }
                                }
                                return "(" + stringBuilder + ")";
                            }
                            throw new ArgumentException("Expected constant expression");
                        }
                        return ResolveQuery(expression as UnaryExpression);
                    }
                }
                else
                {
                    return (expression as ConstantExpression).Value;
                }
            var expression2 = expression as MemberExpression;
            object expressionValue;
            if (expression2.Expression.NodeType == ExpressionType.Parameter)
            {

                return expression2.Member.DeclaringType.GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault().TabelName + "." + expression2.Member.GetCustomAttributes(true).OfType<FieldAttribute>().FirstOrDefault().FieldName;
            }
            else
                expressionValue = GetExpressionValue(expression2.Expression);
            return this.ResolveValue((dynamic)expression2.Member, expressionValue);
        }

        private static MemberExpression GetMemberExpression(Expression expression)
        {
            var nodeType = expression.NodeType;
            if (nodeType != ExpressionType.Convert)
            {
                if (nodeType != ExpressionType.MemberAccess)
                {
                    throw new ArgumentException("Member expression expected");
                }
                return (expression as MemberExpression);
            }
            return GetMemberExpression((expression as UnaryExpression).Operand);
        }

        public string GetTableName<T>()
        {
            return GetTableName(typeof(T));
        }

        private string GetTableName(MemberExpression expression)
        {
            if (expression == null)
                return "";
            return GetTableName(expression.Member.DeclaringType);
        }

        public string GetTableName(Type type)
        {
            var tableItem = Activator.CreateInstance(type) as TableItem;
            if (tableItem != null)
            {
                if (TableNamesDictionary.ContainsKey(tableItem.TableName))
                    return TableNamesDictionary[tableItem.TableName];
                return tableItem.TableName;
            }
            var attribute =
                type.GetCustomAttributes(false).OfType<TableAttribute>().FirstOrDefault<TableAttribute>();
            if (attribute != null)
            {
                return attribute.TabelName;
            }
            return type.Name;
        }

        public void GroupBy<T>(Expression<Func<T, object>> expression)
        {
            GroupBy<T>(GetMemberExpression(expression.Body));
        }

        private void GroupBy<T>(MemberExpression expression)
        {
            var columnName = GetColumnName(GetMemberExpression(expression));
            _builder.GroupBy(GetTableName<T>(), columnName);
        }

        public void Join<T1, T2>(Expression<Func<T1, T2, bool>> expression)
        {
            var binaryExpression = GetBinaryExpression(expression.Body);
            var memberExpression = GetMemberExpression(binaryExpression.Left);
            var rightExpression = GetMemberExpression(binaryExpression.Right);
            Join<T1, T2>(memberExpression, rightExpression);
        }

        public void Join<T1, T2>(MemberExpression leftExpression, MemberExpression rightExpression)
        {
            _builder.Join(GetTableName<T1>(), GetTableName<T2>(), GetColumnName(leftExpression),
                GetColumnName(rightExpression));
        }

        public void Join<T1, T2, TKey>(Expression<Func<T1, TKey>> leftExpression,
            Expression<Func<T1, TKey>> rightExpression)
        {
            Join<T1, T2>(GetMemberExpression(leftExpression.Body), GetMemberExpression(rightExpression.Body));
        }

        public void OrderBy<T>(Expression<Func<T, object>> expression, bool desc = false)
        {
            var columnName = GetColumnName(GetMemberExpression(expression.Body));
            _builder.OrderBy(GetTableName<T>(), columnName, desc);
        }

        public void QueryByIsIn<T>(Expression<Func<T, object>> expression, SqlLamBase sqlQuery)
        {
            var columnName = GetColumnName(expression);
            _builder.QueryByIsIn(GetTableName<T>(), columnName, sqlQuery);
        }

        public void QueryByIsIn<T>(Expression<Func<T, object>> expression, IEnumerable<object> values)
        {
            var columnName = GetColumnName(expression);
            _builder.QueryByIsIn(GetTableName<T>(), columnName, values);
        }

        public void QueryByNotIn<T>(Expression<Func<T, object>> expression, SqlLamBase sqlQuery)
        {
            var columnName = GetColumnName(expression);
            _builder.Not();
            _builder.QueryByIsIn(GetTableName<T>(), columnName, sqlQuery);
        }

        public void QueryByNotIn<T>(Expression<Func<T, object>> expression, IEnumerable<object> values)
        {
            var columnName = GetColumnName(expression);
            _builder.Not();
            _builder.QueryByIsIn(GetTableName<T>(), columnName, values);
        }

        private object ResolveMethodCall(MethodCallExpression callExpression)
        {
            var source = callExpression.Arguments.Select(GetExpressionValue).ToArray();
            if (!source.Any(n => !n.Equals(null)))
            {
                //   callExpression.Method.Invoke(callExpression.Object)
            }
            var obj2 = (callExpression.Object != null)
                ? GetExpressionValue(callExpression.Object)
                : source.FirstOrDefault();
            //if (obj2 == null)
            //    return callExpression.Method.Invoke();
            //else
            return callExpression.Method.Invoke(obj2, source);
        }

        private void ResolveNullValue(MemberNode memberNode, ExpressionType op)
        {
            var type = op;
            if (type != ExpressionType.Equal)
            {
                if (type != ExpressionType.NotEqual)
                {
                    return;
                }
            }
            else
            {
                _builder.QueryByFieldNull(memberNode.TableName, memberNode.FieldName);
                return;
            }
            _builder.QueryByFieldNotNull(memberNode.TableName, memberNode.FieldName);
        }

        private void ResolveOperation(ExpressionType op)
        {
            switch (op)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    _builder.And();
                    return;

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    _builder.Or();
                    return;
                case ExpressionType.Equal:
                    _builder.Equal();
                    return;
                case ExpressionType.Add:
                    _builder.Add();
                    return;
                case ExpressionType.Divide:
                    _builder.Divide();
                    return;
                case ExpressionType.Subtract:
                    _builder.Subtract();
                    return;
                case ExpressionType.Multiply:
                    _builder.Multiply();
                    return;
                case ExpressionType.LessThan:
                    _builder.LessThan();
                    return;
                case ExpressionType.LessThanOrEqual:
                    _builder.LessThanOrEqual();
                    return;
                case ExpressionType.GreaterThan:
                    _builder.GreaterThan();
                    return;
                case ExpressionType.GreaterThanOrEqual:
                    _builder.GreaterThanOrEqual();
                    return;
            }
            throw new ArgumentException(string.Format("Unrecognized binary expression operation '{0}'", op));
        }

        private Node ResolveQuery(BinaryExpression binaryExpression)
        {
            return new OperationNode
            {
                Left = (Node)this.ResolveQuery((dynamic)binaryExpression.Left),
                Operator = binaryExpression.NodeType,
                Right = (Node)this.ResolveQuery((dynamic)binaryExpression.Right)
            };
        }

        private Node ResolveQuery(ConstantExpression constantExpression)
        {
            return new ValueNode { Value = constantExpression.Value };
        }

        private void ResolveQuery(Expression expression)
        {
            throw new ArgumentException(string.Format("The provided expression '{0}' is currently not supported",
                expression.NodeType));
        }

        private Node ResolveQuery(MethodCallExpression callExpression)
        {
            LikeMethod method;
            FunctionMethod functionMethod;
            var valueExpression = "";
            if (Enum.TryParse(callExpression.Method.Name, true, out method))
            {
                var expression = callExpression.Object as MemberExpression;
                Expression expressionArgument = null;
                if (expression == null)
                {
                    object obj = ResolveMethodCall(callExpression);
                    Console.WriteLine();
                }
                if (method == LikeMethod.Between)
                {
                    expression = callExpression.Arguments[0] as MemberExpression;
                    valueExpression = _builder.Adapter.ResolveValue(method, callExpression.Arguments.Select(n => GetExpressionValue(n)));
                }
                else
                    if (callExpression.Arguments.Count == 1)
                    expressionArgument = callExpression.Arguments.FirstOrDefault();
                else
                {

                    var methodCallExpression = callExpression.Arguments[0] as UnaryExpression;
                    if (methodCallExpression != null)
                        expression = methodCallExpression.Operand as MemberExpression;
                    else if (callExpression.Arguments[0] is MemberExpression)
                    {
                        expression = callExpression.Arguments[0] as MemberExpression;
                    }
                    expressionArgument = callExpression.Arguments.LastOrDefault();
                }
                object expressionValue;
                if (!string.IsNullOrEmpty(valueExpression))
                {
                    expressionValue = valueExpression;

                }
                else
                {
                    expressionValue = GetExpressionValue(expressionArgument);

                }
                var node = new LikeNode();
                var tableReference = GetTableName(expression);
                var node2 = new MemberNode
                {
                    TableName = tableReference,
                    FieldName = GetColumnName(expression)
                };
                if (!GetTableName(expression).Equals(TablePrimary))
                {
                    var tablePrimary = Activator.CreateInstance(TypeTablePrimary);
                    foreach (var propertyInfo in tablePrimary.GetType().GetProperties())
                    {
                        foreach (
                            var customAttribute in propertyInfo.GetCustomAttributes(true).OfType<ReferenceAttribute>())
                        {
                            if (customAttribute.TableName.Equals(tableReference))
                            {
                                _builder.JoinMultiple(TablePrimary, tableReference,
                                    customAttribute.ReferenecKey.ToArray());
                            }
                        }
                    }
                }
                node.MemberNode = node2;
                node.Method = method;
                node.Value = string.IsNullOrEmpty(valueExpression) ? _builder.Adapter.ResolveValue(method, expressionValue) : valueExpression;
                return node;

            }
            if (Enum.TryParse(callExpression.Method.Name, true, out functionMethod))
            {
            }
            var obj2 = ResolveMethodCall(callExpression);
            return new ValueNode { Value = obj2 };
        }

        private Node ResolveQuery(UnaryExpression unaryExpression)
        {
            Node child = null;
            if (unaryExpression.Operand.NodeType == ExpressionType.New)
            {
                //dynamic arguments = unaryExpression.Operand;
                var methodCallExpression = (NewExpression)unaryExpression.Operand;
                var listArgumenst = methodCallExpression.Arguments;
                var listObject = new List<object>();
                var listType = new List<Type>();
                foreach (var expression in listArgumenst)
                {
                    listObject.Add(((ConstantExpression)expression).Value);
                    listType.Add(((ConstantExpression)expression).Value.GetType());
                }
                var objectNew = Activator.CreateInstance(unaryExpression.Operand.Type, listObject.ToArray());
                if (objectNew is DateTime)
                    return new ValueNode
                    {
                        Value =
                            _builder.Adapter.ConvertDateTime(
                                objectNew)
                    };
                return new ValueNode
                {
                    Value = objectNew
                };
            }
            child = this.ResolveQuery((dynamic)unaryExpression.Operand);
            return new SingleOperationNode
            {
                Operator = unaryExpression.NodeType,
                Child = child
            };
        }

        public void ResolveQuery<T>(Expression<Func<T, int>> expression, string r, string l)
        {
            var key = Expression.Invoke(expression, expression.Parameters.ToArray());
            var intR = Convert.ToInt32(r);
            var intL = Convert.ToInt32(l);
            var lowerBound = Expression.GreaterThanOrEqual(key, Expression.Constant(intR));
            var upperBound = Expression.LessThanOrEqual(key, Expression.Constant(intL));
            var and = Expression.AndAlso(lowerBound, upperBound);
            var lambda = Expression.Lambda<Func<T, bool>>(and, expression.Parameters);
            object obj2 = this.ResolveQuery((dynamic)lambda.Body);
            this.BuildSql((dynamic)obj2);
        }

        public void ResolveQuery<T>(Expression<Func<T, bool>> expression)
        {
            object obj2 = this.ResolveQuery((dynamic)expression.Body);
            this.BuildSql((dynamic)obj2);
        }       

        private Node ResolveQuery(MemberExpression memberExpression, MemberExpression rootExpression = null)
        {
            rootExpression = rootExpression ?? memberExpression;
            if (memberExpression.Expression != null)
            {
                switch (memberExpression.Expression.NodeType)
                {
                    case ExpressionType.Call:
                    case ExpressionType.Constant:
                        return new ValueNode { Value = GetExpressionValue(rootExpression) };

                    case ExpressionType.MemberAccess:
                        return ResolveQuery(memberExpression.Expression as MemberExpression, rootExpression);

                    case ExpressionType.Parameter:
                        {
                            //update 24 06 2015 Chandra
                            // tambahan untuk left join ketika property table item di panggil
                            FindRelation(rootExpression);

                            return new MemberNode
                            {
                                TableName = GetTableName(rootExpression),
                                FieldName = GetColumnName(rootExpression)
                            };
                        }
                }
            }
            else
            {
                var value = Activator.CreateInstance(rootExpression.Member.ReflectedType);

                if (value is DateTime)
                    return new ValueNode
                    {
                        Value =
                            _builder.Adapter.ConvertDateTime(
                                (DateTime)value.GetType().GetProperty(rootExpression.Member.Name).GetValue(value, null))
                    };

                return new ValueNode
                {
                    Value = value.GetType().GetProperty(rootExpression.Member.Name).GetValue(value, null)
                };
            }
            throw new ArgumentException("Expected member expression");
        }

        private void FindRelation(MemberExpression rootExpression)
        {
            var tableName = GetTableName(rootExpression);
            if (!tableName.Equals(TablePrimary))
            {
                var expression = (rootExpression.Expression as MemberExpression);
                var listMemberExpression = new List<MemberExpression>();
                while (expression != null)
                {
                    listMemberExpression.Add(expression);
                    expression = (expression.Expression as MemberExpression);
                }
                listMemberExpression.Reverse();
                var typeCurrent = TypeTablePrimary;
                foreach (var memberExpression1 in listMemberExpression)
                {
                    var tableReference = (Activator.CreateInstance(memberExpression1.Type) as TableItem).TableName;
                    //var current = memberExpression1.Expression as MemberExpression;
                    //var tablePrimary = Activator.CreateInstance(memberExpression1.Member.DeclaringType) as TableItem;
                    var tablePrimary = Activator.CreateInstance(typeCurrent) as TableItem;
                    var property = tablePrimary.GetType().GetProperty(memberExpression1.Member.Name);
                    typeCurrent = property.PropertyType;
                    if (property != null)
                    {
                        foreach (var propertyInfo in tablePrimary.GetType().GetProperties())
                        {
                            var costumAttribuReference =
                                propertyInfo.GetCustomAttributes(true)
                                    .FirstOrDefault(n => n is ReferenceAttribute);
                            if (costumAttribuReference != null)
                            {
                                var customAttribute = (costumAttribuReference as ReferenceAttribute);
                                if (customAttribute != null &&
                                    customAttribute.TableName.Equals(tableReference))
                                {
                                    _builder.JoinMultiple(tablePrimary.TableName, tableReference,
                                        customAttribute.ReferenecKey.ToArray());
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ResolveSingleOperation(ExpressionType op)
        {
            if (op == ExpressionType.Not)
            {
                _builder.Not();
            }
        }

        private object ResolveValue(object field, object obj)
        {
            return field;
        }

        private object ResolveValue(FieldInfo field, object obj)
        {
            if (field.FieldType == typeof(DateTime) || field.FieldType == typeof(Nullable<DateTime>))
            {
                var format = ContextManager.Current.ConnectionManager.FormateDate;
                return ((DateTime)field.GetValue(obj)).ToString(format);
            }
            return field.GetValue(obj);
        }

        private object ResolveValue(PropertyInfo property, object obj)
        {
            return property.GetValue(obj, null);
        }

        private void Select<T>(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberInit:
                    return;
                case ExpressionType.Convert:
                case ExpressionType.MemberAccess:
                    Select<T>(GetMemberExpression(expression));
                    return;

                case ExpressionType.New:
                    foreach (object expression2 in (expression as NewExpression).Arguments)
                    {
                        if (expression2 is MemberExpression)
                            Select<T>(expression2 as MemberExpression);
                        else if (expression2 is MethodCallExpression)
                        {
                            var data = expression2 as MethodCallExpression;
                            //Select<T>(expression2 as MethodCallExpression);
                            if (data.Method.ReturnType == typeof(SelectFunction))
                            {
                                var function = data.Method.Name.ToUpper();
                                var tableName = GetTableName((data.Arguments[0] as MemberExpression).Expression.Type);
                                var fieldName = GetColumnName(data.Arguments[0]);
                                SelectFunction func;
                                Enum.TryParse(function, out func);
                                _builder.HaveGroup = true;
                                _builder.Select(tableName, fieldName, func);
                                _builder.HaveGroup = false;
                                //_builder.GroupBy(tableName, fieldName);
                            }

                        }
                    }
                    return;

                case ExpressionType.Parameter:
                    _builder.Select(GetTableName(expression.Type));
                    return;
            }
            throw new ArgumentException("Invalid expression");
        }

        private void Select<T>(MemberExpression expression)
        {
            if (expression.Type.IsClass && (expression.Type != typeof(string)))
            {
                _builder.Select(GetTableName(expression.Type));
            }
            else
            {
                if (expression.Member.DeclaringType == typeof(T))
                    _builder.Select(GetTableName<T>(), GetColumnName(expression));
                else
                {
                    _builder.Select(GetTableName(expression.Member.DeclaringType), GetColumnName(expression));
                    FindRelation(expression);

                }

            }
        }
        public void Select<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            Select<T>(expression.Body);
        }
        public void Select<T>(Expression<Func<T, object>> expression)
        {
            Select<T>(expression.Body);
        }

        //public void SelectFunctionOnly<T>(SelectFunction selectFunction)
        //{
        //    _builder.Select(GetTableName<T>(), "*", SelectFunction.COUNT);
        //}

        private void SelectWithFunction<T>(Expression expression, SelectFunction selectFunction)
        {
            var columnName = GetColumnName(GetMemberExpression(expression));
            _builder.Select(GetTableName<T>(), columnName, selectFunction);
        }

        public void SelectWithFunction<T>(Expression<Func<T, object>> expression, SelectFunction selectFunction)
        {
            SelectWithFunction<T>(expression.Body, selectFunction);
        }

        internal void RemoveExpression<T>(Expression<Func<T, object>> expression, SelectFunction selectFunction)
        {
            RemoveWithFunction<T>(expression.Body, selectFunction);
        }

        private void RemoveWithFunction<T>(Expression expression, SelectFunction selectFunction)
        {
            var columnName = GetColumnName(GetMemberExpression(expression));
            _builder.Remove(GetTableName<T>(), columnName, selectFunction);
        }
    }
}