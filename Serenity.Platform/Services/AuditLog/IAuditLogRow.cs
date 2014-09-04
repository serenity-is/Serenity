using System;
using Newtonsoft.Json;

namespace Serenity.Data
{
    public interface IAuditLogRow : IIdRow
    {
        StringField EntityTypeIdField { get; }
        Int64Field EntityIdField { get; }
        StringField ParentTypeIdField { get; }
        Int64Field OldParentIdField { get; }
        Int64Field NewParentIdField { get; }
        DateTimeField DateField { get; }
        IIdField UserIdField { get; }
        Int32Field AuditTypeIdField { get; }
        StringField OldAuditDataField { get; }
        StringField NewAuditDataField { get; }
    }

    public enum AuditType
    {
        Insert = 1,
        Update = 2,
        Delete = 3,
        UndoDelete = 4
    }
}