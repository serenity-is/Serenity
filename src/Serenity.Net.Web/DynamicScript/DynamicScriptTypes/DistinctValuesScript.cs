using Serenity.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serenity.Web
{
    public class DistinctValuesScript<TRow> : LookupScript
       where TRow : class, IRow, new()
    {
        private readonly IConnectionFactory connections;
        private readonly string propertyName;

        public DistinctValuesScript(IConnectionFactory connections, string propertyName)
        {
            this.connections = connections ?? throw new ArgumentNullException(nameof(connections));
            this.propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));

            var row = new TRow();
            var field = GetFieldFrom(row);

            var readPermission = field.GetAttribute<ReadPermissionAttribute>() ??
                typeof(TRow).GetCustomAttribute<ReadPermissionAttribute>(true);

            if (readPermission != null)
                Permission = readPermission.Permission ?? "?";

            GroupKey = row.GetFields().GenerationKey;

            TextField = "v";
            IdField = "v";
        }

        private Field GetFieldFrom(IRow row)
        {
            Field field = row.FindFieldByPropertyName(propertyName) ??
                row.FindField(propertyName);

            if (field is null)
            {
                throw new InvalidProgramException(string.Format(
                    "Property '{0}' specified in a distinct values script on " +
                    "row type {1} is not found!", propertyName, row.GetType().FullName));
            }

            return field;
        }

        protected virtual void ApplyOrder(SqlQuery query)
        {
            var row = (IRow)(query as ISqlQueryExtensible).FirstIntoRow;
            query.OrderBy(GetFieldFrom(row));
        }

        protected virtual void PrepareQuery(SqlQuery query)
        {
            var row = (IRow)(query as ISqlQueryExtensible).FirstIntoRow;
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
                ", new Q.Lookup(" + LookupParams.ToJson() + ", " + GetItems().ToJson() + 
                ".map(function(x) { return { v: x }; })));";
        }

        protected override IEnumerable GetItems()
        {
            var loader = new TRow();
            var field = GetFieldFrom(loader);
            var list = new List<object>();

            var query = new SqlQuery()
                .From(loader);

            PrepareQuery(query);
            ApplyOrder(query);

            using (var connection = connections.NewByKey(loader.Fields.ConnectionKey))
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