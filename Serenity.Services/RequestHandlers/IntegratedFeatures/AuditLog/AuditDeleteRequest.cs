using System;
using EntityType = System.String;

namespace Serenity.Services
{
    public class AuditDeleteRequest
    {
        public EntityType EntityType { get; private set; }
        public Int64 EntityId { get; private set; }
        public EntityType ParentTypeId { get; set; }
        public Int64? ParentId { get; set; }
        public Int32? UserId { get; set; }

        public AuditDeleteRequest(EntityType entityType, Int64 entityId)
        {
            EntityType = entityType;
            EntityId = entityId;
        }
    }
}