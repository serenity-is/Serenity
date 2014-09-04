using System;
using System.Collections.Generic;
using Serenity.Data;
using EntityType = System.String;

namespace Serenity.Services
{
    public class AuditSaveRequest
    {
        public EntityType EntityType { get; private set; }
        public IIdRow OldEntity { get; private set; }
        public IIdRow NewEntity { get; private set; }
        public Field[] AuditFields { get; private set; }
        public EntityType ParentTypeId { get; set; }
        public Int64? OldParentId { get; set; }
        public Int64? NewParentId { get; set; }
        public AuditFileFieldInfo[] FileFieldInfos { get; set; }
        public string FileSubFolder { get; set; }
        public ICollection<string> FilesToDelete { get; set; }

        public AuditSaveRequest(EntityType entityType, IIdRow oldEntity, IIdRow newEntity, Field[] auditFields)
        {
            if (newEntity == null)
                throw new ArgumentNullException("newEntity");

            if (auditFields == null)
                throw new ArgumentNullException("auditFields");

            EntityType = entityType;
            OldEntity = oldEntity;
            NewEntity = newEntity;
            AuditFields = auditFields;
        }
    }
}