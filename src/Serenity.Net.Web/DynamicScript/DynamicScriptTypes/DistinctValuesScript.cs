using Serenity.Data;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace Serenity.Web
{
    public class DistinctValuesScript<TRow> : LookupScript
       where TRow : Row, new()
    {
        private string propertyName;

        public DistinctValuesScript(string propertyName)
        {
            this.propertyName = propertyName;
            var row = new TRow();
            var field = GetFieldFrom(row);

            var readPermission = field.GetAttribute<ReadPermissionAttribute>() ??
                typeof(TRow).GetCustomAttribute<ReadPermissionAttribute>(true);

            if (readPermission != null)
                this.Permission = readPermission.Permission ?? "?";

            this.GroupKey = row.GetFields().GenerationKey;
            this.getItems = GetItems;

            this.TextField = "v";
            this.IdField = "v";
        }

        private Field GetFieldFrom(Row row)
        {
            Field field = row.FindFieldByPropertyName(propertyName) ??
                row.FindField(propertyName);

            if (ReferenceEquals(null, field))
            {
                throw new Exception(String.Format(
                    "Property '{0}' specified in a distinct values script on " +
                    "row type {1} is not found!", propertyName, row.GetType().FullName));
            }

            return field;
        }

        protected virtual void ApplyOrder(SqlQuery query)
        {
            var row = (Row)((query as ISqlQueryExtensible).FirstIntoRow);
            query.OrderBy(GetFieldFrom(row));
        }

        protected virtual void PrepareQuery(SqlQuery query)
        {
            var row = (Row)((query as ISqlQueryExtensible).FirstIntoRow);
            var field = GetFieldFrom(row);

            query.Select(field)
                .Distinct(true)
                .Where(field.IsNotNull());

            if (field is StringField)
                query.Where(field != "");
        }

        public override string GetScript()
        {
            return "Q.ScriptData.set(" + ("Lookup." + LookupKey).ToSingleQuoted() +
                ", new Q.Lookup(" + LookupParams.ToJson() + ", " + getItems().ToJson() + 
                ".map(function(x) { return { v: x }; })));";
        }

        protected virtual List<object> GetItems()
        {
            var loader = new TRow();
            var field = GetFieldFrom(loader);
            var list = new List<object>();

            var query = new SqlQuery()
                .From(loader);

            PrepareQuery(query);
            ApplyOrder(query);

            using (var connection = SqlConnections.NewByKey(loader.GetFields().ConnectionKey))
            {
                query.ForEach(connection, delegate ()
                {
                    list.Add(field.AsObject(loader));
                });
            }

            return list;
        }
    }
}