/*using Serenity.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Serenity.Data
{
    internal class RowPropertyDictionary
    {
        internal Dictionary<string, object> joins;
        internal Dictionary<string, PropertyInfo> rowProperties;
        internal Type rowType;
        internal string connectionKey;
        internal string tableName;
        internal string alias;
        internal string aliasDot;
        internal DialectExpressionSelector expressionSelector;

        public RowPropertyDictionary(Type rowType)
        {
            this.rowType = rowType;
            this.alias = "T0";
            this.aliasDot = "T0.";
            this.joins = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var tbn = this.rowType.GetCustomAttribute<TableNameAttribute>();
            if (tbn != null)
                tableName = tbn.Name;
            else
                tableName = "!!UNKNOWN!!";

            var connectionKeyAttr = rowType.GetCustomAttribute<ConnectionKeyAttribute>();
            if (connectionKeyAttr != null)
                connectionKey = connectionKeyAttr.Value;
            else
                connectionKey = "Default";

            expressionSelector = new DialectExpressionSelector(connectionKey);

            rowProperties = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (var pi in rowType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                rowProperties[pi.Name] = pi;

            foreach (var property in rowProperties.Values)
            {
                foreach (var attr in property.GetCustomAttributes<LeftJoinAttribute>())
                    if (attr.ToTable != null && attr.OnCriteria != null)
                        joins[attr.Alias] = property;
            }

            foreach (var attr in rowType.GetCustomAttributes<LeftJoinAttribute>())
                joins[attr.Alias] = attr;

            foreach (var attr in rowType.GetCustomAttributes<InnerJoinAttribute>())
                joins[attr.Alias] = attr;

            foreach (var attr in rowType.GetCustomAttributes<OuterApplyAttribute>())
                joins[attr.Alias] = attr;
        }

        public static IEnumerable<ExpressionAttribute> DetermineExpression(Dictionary<Type, RowPropertyDictionary> cache, Type rowType, 
            PropertyInfo property, int recursion)
        {
            var expressions = property.GetCustomAttributes<ExpressionAttribute>(false);
            if (expressions.Any())
                return expressions;

            var viewColumn = property.GetCustomAttribute<ViewColumnAttribute>();
            if (viewColumn == null)
                return new ExpressionAttribute[0];

            RowPropertyDictionary dictionary;
            if (!cache.TryGetValue(rowType, out dictionary))
            {
                dictionary = new RowPropertyDictionary(rowType);
                cache[rowType] = dictionary;
            }
        }
    }
}*/