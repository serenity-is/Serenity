using System;
using System.Collections.Generic;
using System.Data;
using Serenity.Data;
using EntityType = System.String;
using Newtonsoft.Json;

namespace Serenity.Services
{
    public static class AuditLogService
    {
        public static AuditLogListResponse List(string schema, AuditLogListRequest request)
        {
            var fld = Dependency.Resolve<IAuditLogRow>(schema);

            Authorization.ValidateLoggedIn();

            var response = new AuditLogListResponse();

            using (var connection = SqlConnections.NewByKey("Default"))
            {
                response.Entities = new List<Row>();

                var row = ((Row)fld).CreateNew();

                if (request.Sort == null ||
                    request.Sort.Length == 0)
                    request.Sort = new SortBy[] { new SortBy(fld.DateField.Name, true) };

                var query = new SqlQuery().From(row)
                    .Select(
                        (Field)fld.IdField,
                        fld.EntityTypeIdField,
                        fld.EntityIdField,
                        fld.ParentTypeIdField,
                        fld.OldParentIdField,
                        fld.NewParentIdField,
                        fld.DateField,
                        (Field)fld.UserIdField,
                        fld.AuditTypeIdField,
                        fld.OldAuditDataField,
                        fld.NewAuditDataField)
                    .OrderBy(
                        (Field)fld.IdField)
                    .ApplySkipTakeAndCount(request.Skip, request.Take, request.ExcludeTotalCount)
                    .ApplySort(request.Sort);

                if (request.EntityTypeId != null &&
                    request.EntityId != null)
                {
                    var pEntityId = query.AddParam(request.EntityId);
                    var pEntityTypeId = query.AddParam(request.EntityTypeId);

                    query.Where(~(
                        ~(
                            new Criteria(0, fld.EntityTypeIdField) == pEntityTypeId &
                            new Criteria(0, fld.EntityIdField) == pEntityId) |
                        ~(
                            new Criteria(0, fld.ParentTypeIdField) == pEntityTypeId &
                            ~(
                                new Criteria(0, fld.OldParentIdField) == pEntityId |
                                new Criteria(0, fld.NewParentIdField) == pEntityId))));
                }
                else
                {
                    if (request.EntityTypeId != null)
                        query.WhereEqual(fld.EntityTypeIdField, request.EntityTypeId);//Convert.ToInt32(request.EntityTypeId.Value));

                    if (request.EntityId != null)
                        query.WhereEqual(fld.EntityIdField, request.EntityId.Value);
                }

                response.TotalCount = query.ForEach(connection, delegate()
                {
                    response.Entities.Add(row.Clone());
                });

                response.SetSkipTakeTotal(query);

                response.IdNameLookups = new Dictionary<EntityType, Dictionary<long, string>>();
                response.FieldTitles = new Dictionary<EntityType, Dictionary<string, string>>();
                response.ForeignEntityTypes = new Dictionary<EntityType, Dictionary<string, string>>();
                response.EntityTitles = new Dictionary<EntityType, string>();

                var lookups = response.IdNameLookups;
                var titles = response.FieldTitles;
                var foreigns = response.ForeignEntityTypes;
                var entities = response.EntityTitles;

                Action<EntityType, Int64> addLookup = (entityType, id) => {

                    Dictionary<long, string> lookup;
                    if (!lookups.TryGetValue(entityType, out lookup))
                    {
                        lookup = new Dictionary<long, string>();
                        lookups[entityType] = lookup;
                    }

                    if (!lookup.ContainsKey(id))
                        lookup[id] = null;
                };

                Action<EntityType, string> addTitle = (entityType, field) =>
                {

                    Dictionary<string, string> lookup;
                    if (!titles.TryGetValue(entityType, out lookup))
                    {
                        lookup = new Dictionary<string, string>();
                        titles[entityType] = lookup;
                    }

                    if (!lookup.ContainsKey(field))
                        lookup[field] = null;
                };

                Action<EntityType> addEntity = (entityType) =>
                {
                    if (!entities.ContainsKey(entityType))
                    {
                        //Row r;
                        String s = null;
                        // TODO: FIX!
                        //if (schema.TypeToTable.TryGetValue(entityType, out r))
                        //    s = LocalText.TryGet(1055, "Db." + r.Table + ".EntitySingular", false);
                         s = s ?? Enum.GetName(typeof(EntityType), entityType);
                        entities[entityType] = s;
                    }
                };

                Action<EntityType, string, EntityType> addForeign = (entityType, field, foreignType) =>
                {
                    Dictionary<string, string> foreign;
                    if (!foreigns.TryGetValue(entityType, out foreign))
                    {
                        foreign = new Dictionary<string, string>();
                        foreigns[entityType] = foreign;
                    }

                    if (!foreign.ContainsKey(field))
                        foreign[field] = Enum.GetName(typeof(EntityType), foreignType);
                };

                foreach (var entity in response.Entities)
                {
                    addEntity(fld.EntityTypeIdField[entity]);
                    addLookup(fld.EntityTypeIdField[entity], fld.EntityIdField[entity].Value);
                    if (fld.ParentTypeIdField[entity] != null)
                        addEntity(fld.ParentTypeIdField[entity]);

                    //if (entity.UserId != null)
                    //    addLookup(UserRow.TableName, entity.UserId.Value);

                    Row theRow;
                    if (((AuditType?)fld.AuditTypeIdField[entity] == AuditType.Insert ||
                         (AuditType?)fld.AuditTypeIdField[entity] == AuditType.Update) &&
                        (fld.OldAuditDataField[entity] != null || fld.NewAuditDataField[entity] != null))
                    {
                        theRow = RowRegistry.GetConnectionRow(RowRegistry.DefaultConnectionKey, fld.EntityTypeIdField[entity]);
                        if (theRow == null)
                            continue;

                        UpdateAuditDataDictionary ud = new UpdateAuditDataDictionary();
                        if (fld.OldAuditDataField[entity] != null)
                            ud.Old = JsonConvert.DeserializeObject<Dictionary<string, object>>(fld.OldAuditDataField[entity].TrimToNull() ?? "{}", JsonSettings.Tolerant);

                        if (fld.NewAuditDataField[entity] != null)
                            ud.New = JsonConvert.DeserializeObject<Dictionary<string, object>>(fld.OldAuditDataField[entity].TrimToNull() ?? "{}", JsonSettings.Tolerant);

                        for (var i = 0; i < 2; i++)
                        {
                            var d = (i == 0) ? ud.Old : ud.New;
                            if (d != null)
                                foreach (var p in d)
                                {
                                    addTitle(fld.EntityTypeIdField[entity], p.Key);

                                    if (p.Value != null &&
                                        p.Value is Int16 ||
                                        p.Value is Int32 ||
                                        p.Value is Int64)
                                    {
                                        var f = theRow.FindField(p.Key);
                                        if (f != null &&
                                            f.ForeignTable != null)
                                        {
                                            //EntityType foreignType;
                                            //if (schema.TableToType.TryGetValue(f.ForeignTable, out foreignType))
                                            {
                                                addForeign(fld.EntityTypeIdField[entity], p.Key, f.ForeignTable);
                                                addLookup(f.ForeignTable, Convert.ToInt64(p.Value));
                                            }
                                        }
                                    }
                                }
                        }
                    }
                }

                foreach (var pair in response.IdNameLookups)
                {
                    Row entity = RowRegistry.GetConnectionRow(RowRegistry.DefaultConnectionKey, pair.Key);
                    if (entity != null)
                    {
                        var idRow = entity as IIdRow;
                        var nameRow = entity as INameRow;
                        if (idRow != null &&
                            nameRow != null)
                        {
                            var lookup = pair.Value;
                            var idName = GetIdNameDictionary(connection, (IIdRow)entity, ((INameRow)entity).NameField, lookup.Keys);
                            foreach (var p in idName)
                                lookup[p.Key] = p.Value;
                        }
                    }
                }

                foreach (var pair in response.FieldTitles)
                {
                    Row entity = RowRegistry.GetConnectionRow(RowRegistry.DefaultConnectionKey, pair.Key);
                    if (entity != null)
                    {
                        var lookup = pair.Value;
                        var keys = new string[lookup.Keys.Count];
                        lookup.Keys.CopyTo(keys, 0);
                        foreach (var key in keys)
                        {
                            Field f;
                            if (key.EndsWith("Id"))
                            {
                                var s = key.Substring(0, key.Length - 2);
                                f = entity.FindField(s);
                                if (f != null)
                                {
                                    lookup[key] = f.Title;
                                    continue;
                                }
                            }

                            f = entity.FindField(key);
                            if (f != null)
                                lookup[key] = f.Title;
                        }
                    }
                }


                return response;
            }
        }

        public static Dictionary<Int64, string> GetIdNameDictionary(IDbConnection connection, IIdRow row, StringField nameField,
            IEnumerable<Int64> idList)
        {
            var list = new List<Int64>(idList);
            var dictionary = new Dictionary<Int64, string>();
            if (list.Count <= 0)
                return dictionary;

            list.Sort();

            var theRow = (Row)row;

            Int64[] part = null;
            const int step = 100;

            var i = 0;
            while (i < list.Count)
            {
                var start = i;
                var end = start + step;
                if (end >= list.Count)
                    end = list.Count - 1;

                var len = end - start + 1;
                if (part == null || len != part.Length)
                    part = new Int64[len];

                list.CopyTo(start, part, 0, len);

                var query = new SqlQuery().Select(((Field)row.IdField).Name).Select(nameField.Name).From(theRow.Table);
                query.Where(new Criteria((Field)row.IdField).In(part));

                using (var reader = SqlHelper.ExecuteReader(connection, query))
                    while (reader.Read())
                        dictionary[reader.ToInt64(0).Value] = reader.AsString(1);

                i += step;
            }

            return dictionary;
        }

        public static void AuditInsert(IDbConnection connection, string schema, AuditSaveRequest request)
        {
            var fld = Dependency.Resolve<IAuditLogRow>(schema);

            var srcRow = (Row)request.NewEntity;
            var dstRow = srcRow.CreateNew();
            dstRow.TrackAssignments = true;

            foreach (var field in request.AuditFields)
                if (srcRow.IsAssigned(field) &&
                    !field.IsNull(srcRow))
                    field.Copy(srcRow, dstRow);

            if (request.FileFieldInfos != null)
            {
                if (request.FilesToDelete == null)
                    throw new ArgumentNullException("FilesToDelete");

                foreach (var file in request.FileFieldInfos)
                    if (dstRow.IsAssigned(file.Filename))
                        AuditFile(dstRow, file, request.FileSubFolder, request.FilesToDelete);
            }

            string auditData;
            if (dstRow.IsAnyFieldAssigned)
                auditData = dstRow.ToJson();
            else
                auditData = null;

            var audit = ((Row)fld).CreateNew();
            audit.TrackAssignments = true;
            fld.EntityTypeIdField[audit] = request.EntityType;
            fld.EntityIdField[audit] = request.NewEntity.IdField[srcRow];
            fld.ParentTypeIdField[audit] = request.ParentTypeId;
            fld.NewParentIdField[audit] = request.NewParentId;
            fld.DateField[audit] = DateTime.UtcNow;
            fld.AuditTypeIdField[audit] = (Int32?)AuditType.Insert;
            fld.OldAuditDataField[audit] = null;
            fld.NewAuditDataField[audit] = auditData;

            var loggingRow = request.NewEntity as ILoggingRow;
            if (loggingRow != null)
                fld.UserIdField[audit] = loggingRow.InsertUserIdField[(Row)request.NewEntity];
            if (fld.UserIdField[audit] == null)
                fld.UserIdField[audit] = Authorization.UserId.TryParseID().Value;

            connection.Insert(audit);
        }

        private static void AuditFile(Row row, AuditFileFieldInfo fileInfo, string fileSubFolder, ICollection<string> filesToDelete)
        {
            // TODO: fix
            //var historyFile = UploadHelper.CopyFileAndRelatedToHistory(UploadHelper.UploadFilePath(fileSubFolder, fileInfo.Filename[row]));

            //fileInfo.Filename[row] = historyFile;

            //if (fileInfo.OriginalName != null &&
            //    !row.IsAssigned(fileInfo.OriginalName))
            //    fileInfo.OriginalName.Copy(row, row);

            //if (fileInfo.Size != null &&
            //    !row.IsAssigned(fileInfo.Size))
            //    fileInfo.Size.Copy(row, row);

            //filesToDelete.Add(UploadHelper.UploadFilePath(fileUploadRoot, UploadHelper.HistoryFolder, historyFile));
        }

        public static Row PrepareAuditUpdate(string schema, AuditSaveRequest request)
        {
            var fld = Dependency.Resolve<IAuditLogRow>(schema);

            var data = new AuditUpdateData<Row>();

            var oldRow = (Row)request.OldEntity;
            var newRow = (Row)request.NewEntity;

            data.Old = oldRow.CreateNew();
            data.Old.TrackAssignments = true;
            data.New = oldRow.CreateNew();
            data.New.TrackAssignments = true;

            foreach (var field in request.AuditFields)
            {
                if (newRow.IsAssigned(field) &&
                    field.IndexCompare(oldRow, newRow) != 0)
                {
                    var strField = field as StringField;
                    if (strField == null ||
                        !strField[oldRow].IsTrimmedSame(strField[newRow]))
                    {
                        if (!field.IsNull(oldRow))
                            field.Copy(oldRow, data.Old);
                        if (!field.IsNull(newRow))
                            field.Copy(newRow, data.New);
                    }
                }
            }

            if (request.FileFieldInfos != null)
            {
                if (request.FilesToDelete == null)
                    throw new ArgumentNullException("FilesToDelete");

                foreach (var file in request.FileFieldInfos)
                {
                    if (file.Filename[oldRow].IsTrimmedSame(file.Filename[newRow]))
                        continue;

                    if (!file.Filename[oldRow].IsTrimmedEmpty())
                    {
                        file.OriginalName.Copy(oldRow, data.Old);
                        file.Size.Copy(oldRow, data.Old);
                        AuditFile(data.Old, file, request.FileSubFolder, request.FilesToDelete);
                    }

                    if (!file.Filename[newRow].IsTrimmedEmpty())
                    {
                        file.OriginalName.Copy(newRow, data.New);
                        file.Size.Copy(newRow, data.New);
                        AuditFile(data.New, file, request.FileSubFolder, request.FilesToDelete);
                    }
                }
            }

            if (!data.Old.IsAnyFieldAssigned)
                data.Old = null;

            if (!data.New.IsAnyFieldAssigned)
                data.New = null;

            string oldAuditData, newAuditData;
            if (data.Old == null)
                oldAuditData = null;
            else
                oldAuditData = data.Old.ToJson();

            if (data.New == null)
                newAuditData = null;
            else
                newAuditData = data.New.ToJson();

            var audit = ((Row)fld).CreateNew();
            audit.TrackAssignments = true;
            fld.EntityTypeIdField[audit] = request.EntityType;
            fld.EntityIdField[audit] = request.OldEntity.IdField[oldRow];
            fld.ParentTypeIdField[audit] = request.ParentTypeId;
            fld.OldParentIdField[audit] = request.OldParentId;
            fld.NewParentIdField[audit] = request.NewParentId;
            fld.DateField[audit] = DateTime.UtcNow;
            fld.AuditTypeIdField[audit] = (Int32)AuditType.Update;
            fld.OldAuditDataField[audit] = oldAuditData;
            fld.NewAuditDataField[audit] = newAuditData;

            var loggingRow = request.NewEntity as ILoggingRow;
            if (loggingRow != null)
                fld.UserIdField[audit] = loggingRow.UpdateUserIdField[(Row)request.NewEntity];
            if (fld.UserIdField[audit] == null)
                fld.UserIdField[audit] = (long)Authorization.UserId.TryParseID();

            return audit;
        }

        public static void AuditUpdate(IDbConnection connection, string schema, AuditSaveRequest request)
        {
            var audit = PrepareAuditUpdate(schema, request);
            connection.Insert(audit);
        }

        public static void AuditDelete(IDbConnection connection, string schema, AuditDeleteRequest request)
        {
            var fld = Dependency.Resolve<IAuditLogRow>(schema);
            var audit = ((Row)fld).CreateNew();
            audit.TrackAssignments = true;
            fld.EntityTypeIdField[audit] = request.EntityType;
            fld.EntityIdField[audit] = request.EntityId;
            fld.ParentTypeIdField[audit] = request.ParentTypeId;
            fld.OldParentIdField[audit] = request.ParentId;
            fld.NewParentIdField[audit] = request.ParentId;
            fld.DateField[audit] = DateTime.UtcNow;
            fld.AuditTypeIdField[audit] = (Int32)AuditType.Delete;
            fld.UserIdField[audit] = (long)(request.UserId == null ? Authorization.UserId.TryParseID() : request.UserId.Value);

            connection.Insert(audit);
        }

        public static void AuditUndelete(IDbConnection connection, string schema, AuditUndeleteRequest request)
        {
            var fld = Dependency.Resolve<IAuditLogRow>(schema);
            var audit = ((Row)fld).CreateNew();
            audit.TrackAssignments = true;
            fld.EntityTypeIdField[audit] = request.EntityType;
            fld.EntityIdField[audit] = request.EntityId;
            fld.ParentTypeIdField[audit] = request.ParentTypeId;
            fld.OldParentIdField[audit] = request.ParentId;
            fld.NewParentIdField[audit] = request.ParentId;
            fld.DateField[audit] = DateTime.UtcNow;
            fld.AuditTypeIdField[audit] = (Int32)AuditType.UndoDelete;
            fld.UserIdField[audit] = (int)(request.UserId == null ? Authorization.UserId.TryParseID() : request.UserId.Value);

            connection.Insert(audit);
        }

        private class UpdateAuditDataDictionary
        {
            public IDictionary<string, object> Old { get; set; }
            public IDictionary<string, object> New { get; set; }
        }
    }
}