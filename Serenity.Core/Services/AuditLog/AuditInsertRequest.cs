using System;
using Serenity.Data;
using System.Collections.Generic;
using Serenity.Web;
using EntityType = System.String;

namespace Serenity.Services
{
    public class AuditInsertRequest
    {
        public EntityType EntityType { get; private set; }
        public IIdRow Entity { get; private set;  }
        public Field[] AuditFields { get; private set; }
        public EntityType ParentTypeId { get; set; }
        public Int64? ParentId { get; set; }
        public AuditFileFieldInfo[] FileFieldInfos { get; set; }
        public string FileSubFolder { get; set; }
        public ICollection<string> FilesToDelete { get; set; }

        public AuditInsertRequest(EntityType entityType, IIdRow entity, Field[] auditFields)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (auditFields == null)
                throw new ArgumentNullException("auditFields");

            EntityType = entityType;
            Entity = entity;
            AuditFields = auditFields;
        }
    }
}