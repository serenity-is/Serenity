using Serenity.Data;
using Serenity.Data.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if COREFX
using System;
#endif

namespace Serenity.Web
{
    public class RowLookupScript<TRow> : LookupScript
        where TRow: Row, new()
    {
        protected virtual void ApplyOrder(SqlQuery query)
        {
            var row = (Row)((query as ISqlQueryExtensible).FirstIntoRow);

            var nameField = row.GetNameField();
            if (!ReferenceEquals(null, nameField))
                query.OrderBy(nameField);
            else if (row is IIdRow)
                query.OrderBy((Field)((IIdRow)row).IdField);
        }

        protected virtual void PrepareQuery(SqlQuery query)
        {
            var row = (Row)((query as ISqlQueryExtensible).FirstIntoRow);

            if (row is IIdRow)
                query.Select((Field)((IIdRow)row).IdField);

            var nameField = row.GetNameField();
            if (!ReferenceEquals(null, nameField))
                query.Select(nameField);

            var list = new List<object>();

            foreach (var f in row.GetFields().Where(x => 
                x.CustomAttributes != null && 
                x.CustomAttributes.OfType<LookupIncludeAttribute>().Any()))
            {
                query.Select(f);
            }
        }

        public RowLookupScript()
            : base()
        {
            var row = new TRow();

            Field field;

            var idRow = row as IIdRow;
            if (idRow != null)
            {
                field = ((Field)idRow.IdField);
                this.IdField = field.PropertyName ?? field.Name;
            }

            var nameField = row.GetNameField();
            if (!ReferenceEquals(null, nameField))
            {
                this.TextField = nameField.PropertyName ?? nameField.Name;
            }

            var treeRow = row as IParentIdRow;
            if (treeRow != null)
            {
                field = ((Field)treeRow.ParentIdField);
                this.ParentIdField = field.PropertyName ?? field.Name;
            }

            var readPermission = typeof(TRow).GetCustomAttribute<ReadPermissionAttribute>(true);
            if (readPermission != null)
                this.Permission = readPermission.Permission ?? "?";

            this.GroupKey = row.GetFields().GenerationKey;
            this.getItems = GetItems;
        }

        protected virtual List<TRow> GetItems()
        {
            var list = new List<TRow>();
            var loader = new TRow();

            var query = new SqlQuery()
                .From(loader);

            PrepareQuery(query);
            ApplyOrder(query);

            using (var connection = SqlConnections.NewByKey(loader.GetFields().ConnectionKey))
            {
                query.ForEach(connection, delegate ()
                {
                    list.Add(loader.Clone());
                });
            }

            return list;
        }
    }
}