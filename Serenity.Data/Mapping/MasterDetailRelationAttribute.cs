using System;
using Serenity.Services;

namespace Serenity.Data.Mapping
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class MasterDetailRelationAttribute : Attribute
    {
        public MasterDetailRelationAttribute(string foreignKey)
        {
            Check.NotNullOrEmpty(foreignKey, "MasterDetailRelation.ForeignKey");

            this.ForeignKey = foreignKey;
            this.CheckChangesOnUpdate = true;
            this.DeleteNonPresentLines = true;
            this.ColumnSelection = ColumnSelection.List;
        }

        public string ForeignKey { get; private set; }
        public bool CheckChangesOnUpdate { get; set; }
        public bool DeleteNonPresentLines { get; set; }
        public ColumnSelection ColumnSelection { get; set; }
        public string IncludeColumns { get; set; }
        public string FilterField { get; set; }
        public object FilterValue { get; set; }

        /// <summary>
        /// Optional: override the default behaviour and use a different id field (i.e. from a unique constraint)
        /// </summary>
        public string MasterKeyField { get; set; }

        /// <summary>
        /// Forces deletion of linking row records even if master record uses soft delete.
        /// If false (default) this doesn't delete linking records, as master record might be undeleted.
        /// </summary>
        public bool ForceCascadeDelete { get; set; }
    }
}
