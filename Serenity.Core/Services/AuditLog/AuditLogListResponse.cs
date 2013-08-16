using System;
using System.Collections.Generic;
using Serenity.Data;
using EntityType = System.String;

namespace Serenity.Services
{
    public class AuditLogListResponse : ListResponse<Row>
    {
        public Dictionary<EntityType, Dictionary<Int64, string>> IdNameLookups;
        // JSON.NET nedense Dictionary<string, EntityType> tipindeki değerin value larını sayı olarak serialize ediyor, çözene kadar böyle kalsın
        public Dictionary<EntityType, Dictionary<string, string>> ForeignEntityTypes;
        public Dictionary<EntityType, string> EntityTitles;
        public Dictionary<EntityType, Dictionary<string, string>> FieldTitles;
    }
}