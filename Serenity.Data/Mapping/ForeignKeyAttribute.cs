﻿using System;
using System.Linq;
using System.Reflection;

namespace Serenity.Data.Mapping
{
    /// <summary>
    /// Specifies that this property is a foreign key to another field in a primary key table.
    /// There is no need for foreign key to exist in database actually. It is not checked.
    /// This is mostly used for joins.
    /// </summary>
    public class ForeignKeyAttribute : Attribute
    {
        /// <summary>
        /// Specifies that this property is a foreign key to another field in a primary key table.
        /// </summary>
        /// <param name="table">Primary key table</param>
        /// <param name="field">Matching column in primary key table</param>
        public ForeignKeyAttribute(string table, string field = "Id")
        {
            Check.NotNull(table, nameof(table));
            Check.NotNull(field, nameof(field));

            this.Field = field;
            this.Table = table;
        }

        /// <summary>
        /// Specifies that this property is a foreign key to another field in a primary key table.
        /// </summary>
        /// <param name="rowType">Entity for primary key table. Row must have a [TableName] attribute.</param>
        /// <param name="field">If field parameter is not specified, the row type must have a field with 
        /// [Identity] attribute or single property with [PrimaryKey] attribute.
        /// (implementing IIdRow won't help)</param>
        public ForeignKeyAttribute(Type rowType, string field = null)
        {
            Check.NotNull(rowType, nameof(rowType));
            this.RowType = rowType;

            var attr = rowType.GetCustomAttribute<TableNameAttribute>(true);
            if (attr == null || string.IsNullOrEmpty(attr.Name))
                throw new ArgumentOutOfRangeException(nameof(rowType),
                    String.Format("Type '{0}' is specified for a ForeignKey attribute " +
                        "but it has no [TableName] attribute", rowType.FullName));

            this.Table = attr.Name;

            if (string.IsNullOrEmpty(field))
            {
                var identityOrPrimaryKey = rowType.GetProperties()
                    .Where(x => x.GetCustomAttribute<IdentityAttribute>() != null ||
                        x.GetCustomAttribute<PrimaryKeyAttribute>() != null).ToArray();

                if (identityOrPrimaryKey.Length == 0)
                    throw new ArgumentOutOfRangeException(nameof(rowType),
                        String.Format("Type '{0}' is specified for a ForeignKey attribute " + 
                            "but it has no property with [Identity] or [PrimaryKey] attribute",
                            rowType.FullName));

                if (identityOrPrimaryKey.Length > 1)
                {
                    var identity = identityOrPrimaryKey.Where(x =>
                        x.GetCustomAttribute<IdentityAttribute>() != null);

                    if (identity.Count() != 1)
                        throw new ArgumentOutOfRangeException(nameof(rowType),
                            String.Format("Type '{0}' is specified for a ForeignKey attribute " + 
                            "but it has multiple [Identity] or [PrimaryKey] attributes",
                            rowType.FullName));

                    identityOrPrimaryKey = identity.ToArray();
                }

                Field = identityOrPrimaryKey[0].GetCustomAttribute<ColumnAttribute>()?.Name ??
                    identityOrPrimaryKey[0].Name;
            }
            else
                Field = field;
        }

        public string Table { get; private set; }
        public string Field { get; private set; }
        public Type RowType { get; set; }
    }
}