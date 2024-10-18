namespace Serenity.Demo.Northwind;

public class NotesBehavior(IUserRetrieveService userRetriever,
    IServiceResolver<INoteListHandler> listHandlerResolver,
    IServiceResolver<INoteSaveHandler> saveHandlerResolver,
    IServiceResolver<INoteDeleteHandler> deleteHandlerResolver) : BaseSaveDeleteBehavior, IImplicitBehavior, IRetrieveBehavior, IFieldBehavior
{
    public Field Target { get; set; }

    private readonly IUserRetrieveService userRetriever = userRetriever ?? throw new ArgumentNullException(nameof(userRetriever));
    private readonly IServiceResolver<INoteListHandler> listHandlerResolver = listHandlerResolver ?? throw new ArgumentNullException(nameof(listHandlerResolver));
    private readonly IServiceResolver<INoteSaveHandler> saveHandlerResolver = saveHandlerResolver ?? throw new ArgumentNullException(nameof(saveHandlerResolver));
    private readonly IServiceResolver<INoteDeleteHandler> deleteHandlerResolver = deleteHandlerResolver ?? throw new ArgumentNullException(nameof(deleteHandlerResolver));

    public bool ActivateFor(IRow row)
    {
        if (Target is null)
            return false;

        var attr = Target.GetAttribute<NotesEditorAttribute>();
        if (attr == null)
            return false;

        if (Target.ValueType != typeof(List<NoteRow>))
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, 
                "Field '{0}' in row type '{1}' has a NotesEditorAttribute " +
                "but its property type is not a List<NoteRow>!",
                Target.PropertyName ?? Target.Name, row.GetType().FullName));
        }

        return true;
    }

    public void OnAfterExecuteQuery(IRetrieveRequestHandler handler) { }
    public void OnBeforeExecuteQuery(IRetrieveRequestHandler handler) { }
    public void OnPrepareQuery(IRetrieveRequestHandler handler, SqlQuery query) { }
    public void OnValidateRequest(IRetrieveRequestHandler handler) { }

    public void OnReturn(IRetrieveRequestHandler handler)
    {
        if (Target is null ||
            !handler.AllowSelectField(Target) ||
            !handler.ShouldSelectField(Target))
            return;

        var idField = (handler.Row as IIdRow).IdField;
        var fld = NoteRow.Fields;

        var listRequest = new ListRequest
        {
            ColumnSelection = ColumnSelection.List,
            EqualityFilter = new Dictionary<string, object>
            {
                { fld.EntityType.PropertyName, handler.Row.Table },
                { fld.EntityId.PropertyName, idField.AsObject(handler.Row) ?? -1 }
            }
        };

        var notes = listHandlerResolver.Resolve().List(handler.Connection, listRequest).Entities;

        var userIdList = notes.Where(x => x.InsertUserId != null)
            .Select(x => x.InsertUserId.Value).Distinct();

        if (userIdList.Any())
        {
            var userDisplayNames = userIdList.ToDictionary(x => x,
                x => userRetriever.ById(x.ToString(
                    CultureInfo.InvariantCulture))?.DisplayName);

            foreach (var x in notes)
                if (x.InsertUserId != null &&
                    userDisplayNames.TryGetValue(x.InsertUserId.Value, out string s))
                    x.InsertUserDisplayName = s;
        }

        Target.AsObject(handler.Row, notes);
    }

    private void SaveNote(IUnitOfWork uow, NoteRow note, string entityType, 
        long entityId, long? noteId)
    {
        note = note.Clone();
        note.NoteId = noteId;
        note.EntityType = entityType;
        note.EntityId = entityId;
        note.InsertDate = null;
        note.ClearAssignment(NoteRow.Fields.InsertDate);

        var saveRequest = new SaveRequest<NoteRow> { Entity = note };

        if (noteId == null)
            saveHandlerResolver.Resolve().Create(uow, saveRequest);
        else
            saveHandlerResolver.Resolve().Update(uow, saveRequest);
    }

    private void DeleteNote(IUnitOfWork uow, long noteId)
    {
        deleteHandlerResolver.Resolve().Delete(uow, new DeleteRequest { EntityId = noteId });
    }

    private void NoteListSave(IUnitOfWork uow, string entityType, long entityId, 
        List<NoteRow> oldList, List<NoteRow> newList)
    {
        var row = oldList.Count > 0 ? oldList[0] : 
            (newList.Count > 0) ? newList[0] : null;

        if (row == null)
            return;

        if (oldList.Count == 0)
        {
            foreach (var note in newList)
                SaveNote(uow, note, entityType, entityId, null);

            return;
        }

        var rowIdField = (row as IIdRow).IdField;

        if (newList.Count == 0)
        {
            foreach (var note in oldList)
                DeleteNote(uow, Convert.ToInt64(rowIdField.AsObject(note), 
                    CultureInfo.InvariantCulture));

            return;
        }

        var oldById = new Dictionary<long, NoteRow>(oldList.Count);
        foreach (var item in oldList)
            oldById[Convert.ToInt64(rowIdField.AsObject(item), CultureInfo.InvariantCulture)] = item;

        var newById = new Dictionary<long, NoteRow>(newList.Count);
        foreach (var item in newList)
        {
            var id = rowIdField.AsObject(item);
            if (id != null)
                newById[Convert.ToInt64(id, CultureInfo.InvariantCulture)] = item;
        }

        foreach (var item in oldList)
        {
            var id = Convert.ToInt64(rowIdField.AsObject(item), CultureInfo.InvariantCulture);
            if (!newById.ContainsKey(id))
                DeleteNote(uow, id);
        }

        foreach (var item in newList)
        {
            var id = rowIdField.AsObject(item);

            if (id == null || !oldById.TryGetValue(Convert.ToInt64(id, 
                CultureInfo.InvariantCulture), out NoteRow old))
                continue;

            bool anyChanges = false;
            foreach (var field in item.GetFields())
            {
                if (item.IsAssigned(field) &&
                    (field.Flags & FieldFlags.Updatable) == FieldFlags.Updatable &
                    field.IndexCompare(old, item) != 0)
                {
                    anyChanges = true;
                    break;
                }
            }

            if (!anyChanges)
                continue;

            SaveNote(uow, item, entityType, entityId, Convert.ToInt64(id, 
                CultureInfo.InvariantCulture));
        }

        foreach (var item in newList)
        {
            var id = rowIdField.AsObject(item);
            if (id == null || !oldById.ContainsKey(Convert.ToInt64(id, 
                CultureInfo.InvariantCulture)))
                SaveNote(uow, item, entityType, entityId, null);
        }
    }

    public override void OnAfterSave(ISaveRequestHandler handler)
    {
        if (Target.AsObject(handler.Row) is not List<NoteRow> newList)
            return;

        var idField = (handler.Row as IIdRow).IdField;
        var entityId = Convert.ToInt64(idField.AsObject(handler.Row),
            CultureInfo.InvariantCulture);

        if (handler.IsCreate)
        {
            foreach (var note in newList)
                SaveNote(handler.UnitOfWork, note, handler.Row.Table, entityId, null);

            return;
        }

        var fld = NoteRow.Fields;
        var listRequest = new ListRequest
        {
            ColumnSelection = ColumnSelection.List,
            EqualityFilter = new Dictionary<string, object>
            {
                { fld.EntityType.PropertyName, handler.Row.Table },
                { fld.EntityId.PropertyName, entityId }
            }
        };
        
        var oldList = listHandlerResolver.Resolve().List(handler.Connection, listRequest).Entities;
        NoteListSave(handler.UnitOfWork, handler.Row.Table, entityId, oldList, newList);
    }

    public override void OnBeforeDelete(IDeleteRequestHandler handler)
    {
        if (Target is null ||
            (Target.Flags & FieldFlags.Updatable) != FieldFlags.Updatable)
            return;

        var idField = (handler.Row as IIdRow).IdField;
        var row = new NoteRow();
        var fld = NoteRow.Fields;

        var deleteList = new List<long>();
        new SqlQuery()
                .From(row)
                .Select(fld.NoteId)
                .Where(
                    fld.EntityType == handler.Row.Table &
                    fld.EntityId == Convert.ToInt64(idField.AsObject(handler.Row), 
                        CultureInfo.InvariantCulture))
                .ForEach(handler.Connection, () =>
                {
                    deleteList.Add(row.NoteId.Value);
                });

        foreach (var id in deleteList)
            DeleteNote(handler.UnitOfWork, id);
    }
}