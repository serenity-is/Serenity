﻿using System;
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
    }
}