using System;

namespace Serenity.Data.Mapping
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class UpdatableExtensionAttribute : Attribute
    {
        public UpdatableExtensionAttribute(string alias, Type rowType)
        {
            Check.NotNullOrEmpty(alias, "ExtensionRelation.alias");
            Check.NotNull(rowType, "ExtensionRelation.rowType");

            this.Alias = alias;
            this.RowType = rowType;
            this.CheckChangesOnUpdate = true;
        }

        public Type RowType { get; private set; }

        /// <summary>
        /// The join alias in this row that brings in extension table fields as view fields
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// Name of the key field in this table. 
        /// If not specified, ID field of this table will be used.
        /// </summary>
        public string ThisKey { get; set; }

        /// <summary>
        /// Name of the key field in extension table. 
        /// If not specified, ID field of extension table is assumed,
        /// unless there is a field with matching name to ThisKey in extension table.
        /// </summary>
        public string OtherKey { get; set; }

        /// <summary>
        /// Only update if there are any changes to extension table fields.
        /// </summary>
        public bool CheckChangesOnUpdate { get; set; }

        /// <summary>
        /// This extension should only be inserted if this field is equal to PresenceValue
        /// For example, you might have a PersonType column in PersonRow and 
        /// student record should only be created (if not already) if PersonType = "Student".
        /// </summary>
        public string PresenceField { get; set; }

        /// <summary>
        /// This extension should only be inserted if PresenceField value is equal to this one.
        /// For example, you might have a PersonType column in PersonRow and 
        /// student record should only be created (if not already) if PersonType = "Student".
        /// </summary>
        public object PresenceValue { get; set; }

        /// <summary>
        /// Delete extension record if this record is deleted
        /// </summary>
        public bool CascadeDelete { get; set; }
    }
}