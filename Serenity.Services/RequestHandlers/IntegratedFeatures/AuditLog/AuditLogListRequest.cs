using System;
using Serenity.Data;
using EntityType = System.String;

namespace Serenity.Services
{
    public class AuditLogListRequest : ListRequest
    {
        public EntityType EntityTypeId;
        public Int64? EntityId;
        public bool IncludeSubItems;
    }
}