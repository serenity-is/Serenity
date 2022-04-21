using System.Collections;

namespace Serenity.Web
{
    public class RowLookupScript<TRow> : LookupScript
        where TRow: class, IRow, new()
    {
        private readonly ISqlConnections connections;

        protected virtual void ApplyOrder(SqlQuery query)
        {
            var row = (IRow)(query as ISqlQueryExtensible).FirstIntoRow;

            if (row.NameField is object)
                query.OrderBy(row.NameField);
            else if (row.IdField is object)
                query.OrderBy(row.IdField);
        }

        protected virtual void PrepareQuery(SqlQuery query)
        {
            var row = (IRow)(query as ISqlQueryExtensible).FirstIntoRow;

            if (row.IdField is object)
                query.Select(row.IdField);

            if (row.NameField is object)
                query.Select(row.NameField);

            var list = new List<object>();

            foreach (var f in row.GetFields().Where(x => 
                x.CustomAttributes.OfType<LookupIncludeAttribute>().Any()))
            {
                query.Select(f);
            }
        }

        public RowLookupScript(ISqlConnections connections)
            : base()
        {
            this.connections = connections ?? throw new ArgumentNullException(nameof(connections));

            var row = new TRow();

            if (row.IdField is object)
            {
                IdField = row.IdField.PropertyName ?? row.IdField.Name;
            }

            if (row.NameField is Field nameField)
            {
                TextField = nameField.PropertyName ?? nameField.Name;
            }

            if (row is IParentIdRow treeRow)
            {
                ParentIdField = treeRow.ParentIdField.PropertyName ?? treeRow.ParentIdField.Name;
            }

            var readPermission = typeof(TRow).GetCustomAttribute<ReadPermissionAttribute>(true);
            if (readPermission != null)
                Permission = readPermission.Permission ?? "?";

            GroupKey = row.GetFields().GenerationKey;
        }

        protected override IEnumerable GetItems()
        {
            var list = new List<TRow>();
            var loader = new TRow();

            var query = new SqlQuery()
                .From(loader);

            PrepareQuery(query);
            ApplyOrder(query);

            using (var connection = connections.NewByKey(loader.Fields.ConnectionKey))
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