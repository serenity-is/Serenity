using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Serenity.Services;

/// <summary>
/// Abstract base class for save request handlers that share state and
/// mode neutral helper methods between synchronous and asynchronous
/// save request handlers.
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TSaveRequest">Save request type</typeparam>
/// <typeparam name="TSaveResponse">Save response type</typeparam>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="context">Request context</param>
/// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
public abstract class SaveRequestHandlerBase<TRow, TSaveRequest, TSaveResponse>(IRequestContext context) : ISaveRequestHandler
    where TRow : class, IRow, IIdRow, new()
    where TSaveResponse : SaveResponse, new()
    where TSaveRequest : SaveRequest<TRow>, new()
{
    private IUnitOfWork? unitOfWork;

    /// <summary>
    /// Gets the list of save behaviors.
    /// </summary>
    protected virtual IEnumerable<ISaveBehavior> GetBehaviors()
    {
        return Context.Behaviors.Resolve<TRow, ISaveBehavior>(GetType());
    }

    /// <summary>
    /// Clears assignment for fields that are marked as non table field,
    /// e.g. fields that are not mapped, view, expression etc.
    /// </summary>
    protected virtual void ClearNonTableAssignments()
    {
        foreach (var field in Row.GetFields())
        {
            if (Row.IsAssigned(field) && !field.IsTableField())
                Row.ClearAssignment(field);
        }
    }

    /// <summary>
    /// Gets the display order filter for current group, if the entity 
    /// implements <see cref="IDisplayOrderRow"/> interface
    /// </summary>
    protected virtual BaseCriteria GetDisplayOrderFilter()
    {
        return DisplayOrderFilterHelper.GetDisplayOrderFilterFor(Row);
    }

    /// <summary>
    /// Gets the list of editable fields. These are fields that have
    /// Insertable (if Create), or Updatable (if Update) flags,
    /// and have the insert or update permission based on the type
    /// of the current operation.
    /// </summary>
    /// <param name="editable"></param>
    protected virtual void GetEditableFields(HashSet<Field> editable)
    {
        var flag = IsCreate ? FieldFlags.Insertable : FieldFlags.Updatable;

        foreach (var field in Row.GetFields())
            if (field.Flags.HasFlag(flag))
            {
                // RowFieldsBase protects potentially sensitive fields by name, but IPasswordRow
                // fields may have arbitrary names, so don't allow editing them unless annotated.
                if (EntityFieldExtensions.IsSensitiveFieldBasedOnInterfaces(field, Row) &&
                    field.GetAttribute<MinSelectLevelAttribute>() == null)
                    continue;

                if ((IsCreate && (field.InsertPermission == null ||
                        Permissions.HasPermission(field.InsertPermission))) ||
                    (IsUpdate && (field.UpdatePermission == null ||
                        Permissions.HasPermission(field.UpdatePermission))))
                    editable.Add(field);
            }
    }

    /// <summary>
    /// Gets the list of required fields based on editable list, and <see cref="FieldFlags.NotNull"/>
    /// </summary>
    /// <param name="required">List of required fields to populate</param>
    /// <param name="editable">Editable fields returned from
    /// <see cref="GetEditableFields(HashSet{Field})"/></param>
    protected virtual void GetRequiredFields(HashSet<Field> required, HashSet<Field> editable)
    {
        foreach (var field in Row.GetFields())
        {
            if (editable.Contains(field) &&
                (field.Flags & FieldFlags.NotNull) == FieldFlags.NotNull &
                (field.Flags & FieldFlags.TrimToEmpty) != FieldFlags.TrimToEmpty)
            {
                required.Add(field);
            }
        }
    }

    /// <summary>
    /// Handles assignment to a non-editable field. If the field did not change
    /// in an update operation, it will be ignored by clearing the assignment.
    /// For non-table fields it will also be ignored.
    /// </summary>
    /// <param name="field">The field to handle.</param>
    protected virtual void HandleNonEditable(Field field)
    {
        if (IsUpdate && field.IndexCompare(Row, Old!, StringComparer.Ordinal) == 0)
        {
            field.CopyNoAssignment(Old!, Row);
            Row.ClearAssignment(field);
            return;
        }

        bool isNonTableField = ((field.Flags & FieldFlags.Foreign) == FieldFlags.Foreign) ||
              ((field.Flags & FieldFlags.Calculated) == FieldFlags.Calculated) ||
              ((field.Flags & FieldFlags.NotMapped) == FieldFlags.NotMapped);

        if (IsUpdate)
        {
            if ((field.Flags & FieldFlags.Reflective) != FieldFlags.Reflective)
            {
                if (!isNonTableField)
                    throw DataValidation.ReadOnlyError(field, Localizer);

                field.CopyNoAssignment(Old!, Row);
                Row.ClearAssignment(field);
            }
        }
        else if (IsCreate)
        {
            if (!field.IsNull(Row) &&
                (field.Flags & FieldFlags.Reflective) != FieldFlags.Reflective)
            {
                if (!isNonTableField)
                    throw DataValidation.ReadOnlyError(field, Localizer);

                field.AsObject(Row, null);
                Row.ClearAssignment(field);
            }
        }

        if (Row.IsAssigned(field))
            Row.ClearAssignment(field);
    }

    /// <summary>
    /// Sets the default value (<see cref="DefaultValueAttribute"/>) for the field
    /// </summary>
    /// <param name="field">Field</param>
    protected virtual void SetDefaultValue(Field field)
    {
        if (field.DefaultValue == null)
            return;

        field.AsInvariant(Row, field.DefaultValue);
    }

    /// <summary>
    /// Sets the default values for all fields (<see cref="DefaultValueAttribute"/>)
    /// </summary>
    protected virtual void SetDefaultValues()
    {
        foreach (var field in Row.GetTableFields())
        {
            if (Row.IsAssigned(field) || !field.IsNull(Row))
                continue;

            SetDefaultValue(field);
        }

        if (Row is IIsActiveRow isActiveRow &&
            !Row.IsAssigned(isActiveRow.IsActiveField))
            isActiveRow.IsActiveField[Row] = 1;
    }

    /// <summary>
    /// Sets values for <see cref="FieldFlags.TrimToEmpty"/> fields.
    /// </summary>
    protected virtual void SetTrimToEmptyFields()
    {
        foreach (var field in Row.GetFields())
            if (!Row.IsAssigned(field) &&
                field is StringField str &&
                (field.Flags & FieldFlags.Insertable) == FieldFlags.Insertable &
                (field.Flags & FieldFlags.NotNull) == FieldFlags.NotNull &
                (field.Flags & FieldFlags.TrimToEmpty) == FieldFlags.TrimToEmpty)
            {
                str[Row] = "";
            }
    }

    /// <summary>
    /// Validates editable fields, e.g. checks if the assigned fields
    /// are all in the editable set. Calls <see cref="HandleNonEditable(Field)"/>
    /// if not.
    /// </summary>
    /// <param name="editable">Set of editable fields</param>
    protected virtual void ValidateEditableFields(HashSet<Field> editable)
    {
        foreach (Field field in Row.GetFields())
        {
            if (IsUpdate && !Row.IsAssigned(field))
            {
                field.CopyNoAssignment(Old!, Row);
                Row.ClearAssignment(field);
                continue;
            }

            if (field is StringField stringField && Row.IsAssigned(field))
                DataValidation.AutoTrim(Row, stringField);

            if (!editable.Contains(field))
                HandleNonEditable(field);
        }
    }

    /// <summary>
    /// Captures the concurrency version used for the update, if the row implements
    /// <see cref="IConcurrencyVersionRow"/>, before editable field validation clears non
    /// editable assignments. The value sent by the client is used when present, otherwise
    /// the value loaded from the database. The captured value is later used to append the
    /// version to the update's WHERE clause.
    /// </summary>
    protected virtual void CaptureConcurrencyVersion()
    {
        concurrencyVersion = null;

        if (!IsUpdate || Row is not IConcurrencyVersionRow versionRow)
            return;

        var versionField = versionRow.ConcurrencyVersionField;
        if (versionField is null)
            return;

        // server side only opt out for internal services (client JSON cannot set it)
        if (!Request.IgnoreConcurrencyVersion)
        {
            // use the value sent by the client when present (full stale read detection),
            // otherwise fall back to the value loaded from the database (Old), so that a
            // concurrent write in the load -> update window is still detected. this mirrors
            // the server-tracked concurrency token behavior of EF Core / NHibernate.
            concurrencyVersion = Row.IsAssigned(versionField)
                ? versionField.AsObject(Row)
                : versionField.AsObject(Old!);
        }

        Row.ClearAssignment(versionField);
    }

    /// <summary>
    /// Validates editable fields.
    /// </summary>
    /// <returns>The set of editable fields.</returns>
    protected virtual HashSet<Field> ValidateEditable()
    {
        CaptureConcurrencyVersion();

        var editableFields = new HashSet<Field>();
        GetEditableFields(editableFields);
        ValidateEditableFields(editableFields);
        return editableFields;
    }

    /// <summary>
    /// Validates required fields
    /// </summary>
    /// <param name="editableFields">List of editable fields</param>
    protected virtual void ValidateRequired(HashSet<Field> editableFields)
    {
        var requiredFields = new HashSet<Field>();
        GetRequiredFields(required: requiredFields, editable: editableFields);

        if (IsUpdate)
            Row.ValidateRequiredIfModified(requiredFields, Localizer);
        else
            Row.ValidateRequired(requiredFields, Localizer);
    }

    /// <summary>
    /// Validates field values via <see cref="ICustomValidator"/> interface.
    /// </summary>
    /// <exception cref="ValidationError">One of the fields has an invalid value</exception>
    protected virtual void ValidateFieldValues()
    {
        var context = new RowValidationContext(Connection, Row, Localizer);

        foreach (var field in Row.GetFields())
        {
            if (!Row.IsAssigned(field))
                continue;

            var validators = field.CustomAttributes.OfType<ICustomValidator>();
            foreach (var validator in validators)
            {
                context.Value = field.AsObject(Row);

                var error = CustomValidate(context, field, validator);

                if (error != null)
                    throw new ValidationError("CustomValidationError", field.PropertyName ?? field.Name, error);
            }
        }
    }

    /// <summary>
    /// Calls custom validator.
    /// </summary>
    /// <param name="context">Validation context</param>
    /// <param name="field">Field</param>
    /// <param name="validator">Custom validator</param>
    /// <returns>The validation error message, or <c>null</c> if the value is valid.</returns>
    protected virtual string? CustomValidate(RowValidationContext context, Field field, ICustomValidator validator)
    {
        return validator.Validate(context);
    }

    /// <summary>
    /// Validates that record is not soft deleted
    /// </summary>
    protected virtual void ValidateIsActive()
    {
        if (Old is IIsActiveRow isActiveRow &&
            isActiveRow.IsActiveField[Old] < 0)
            throw DataValidation.RecordNotActive(Old, Localizer);

        if (Old is IIsDeletedRow isDeletedRow &&
            isDeletedRow.IsDeletedField[Old] == true)
            throw DataValidation.RecordNotActive(Old, Localizer);
    }

    /// <summary>
    /// Validates and clears the ID field
    /// </summary>
    protected virtual void ValidateAndClearIdField()
    {
        var idField = Row.GetIdField();
        if (Row.IsAssigned(idField))
            Row.ValidateRequired(idField, Localizer);

        if ((idField.Flags & FieldFlags.Updatable) != FieldFlags.Updatable)
        {
            // The id field is not updatable, so a client-supplied id cannot be applied. Only an
            // unassigned/null id, or the same value as the target record, is acceptable. The old
            // id is the one the request targets (Old is loaded by this same id), i.e. Request.EntityId
            // when present, otherwise the row's own id. A different non-null value would otherwise be
            // silently ignored (or re-key the row), so reject it instead.
            var newId = idField.AsObjectNoCheck(Row);
            var oldId = Request.EntityId != null
                ? idField.ConvertValue(Request.EntityId, System.Globalization.CultureInfo.InvariantCulture)
                : newId;

            if (newId != null && !Equals(newId, oldId))
                throw DataValidation.InvalidIdError(Row, idField, Localizer);

            // Keep the row id equal to the target record id so the update and response use it
            // consistently, then mark it unassigned so it is not included in the update SET.
            if (oldId != null)
                idField.AsObject(Row, oldId);

            Row.ClearAssignment(idField);
        }
    }

    /// <summary>
    /// Gets the id of the entity targeted by the current request, converted to the
    /// value type of the row's id field. Falls back to the row's own id when the
    /// request does not carry one. Used both for loading the previous entity and for
    /// building the error returned when it cannot be loaded, so that a caller cannot
    /// distinguish a missing record from one they are not allowed to touch.
    /// </summary>
    protected virtual object? GetRequestEntityId()
    {
        var idField = Row.GetIdField();
        if (Request.EntityId != null)
            return idField.ConvertValue(Request.EntityId, System.Globalization.CultureInfo.InvariantCulture);

        var rowId = idField.AsObject(Row);
        if (rowId == null && (idField.Flags & FieldFlags.NotNull) == FieldFlags.NotNull)
            ArgumentNullException.ThrowIfNull(Request.EntityId);

        return rowId;
    }

    /// <summary>
    /// Validates user permissions by checking <see cref="InsertPermissionAttribute"/>
    /// and <see cref="UpdatePermissionAttribute"/>, and <see cref="ModifyPermissionAttribute"/>
    /// or <see cref="ReadPermissionAttribute" /> if others are not found.
    /// </summary>
    /// <remarks>
    /// This check is per row type, not per record, so it does not prevent modifying records that
    /// belong to other users or tenants by their id. For record level authorization, implement a
    /// behavior that applies the required filter in <see cref="ISaveBehaviorSync.OnPrepareQuery"/>,
    /// or override this method. See the multitenancy tutorial in Serenity docs.
    /// </remarks>
    protected virtual void ValidatePermissions()
    {
        PermissionAttributeBase? attr = null;

        if (IsUpdate)
        {
            attr = typeof(TRow).GetCustomAttribute<UpdatePermissionAttribute>(true);
        }
        else if (IsCreate)
        {
            attr = typeof(TRow).GetCustomAttribute<InsertPermissionAttribute>(true);
        }

        attr ??= (PermissionAttributeBase?)typeof(TRow).GetCustomAttribute<ModifyPermissionAttribute>(true) ??
            typeof(TRow).GetCustomAttribute<ReadPermissionAttribute>(true);

        var permission = attr?.Permission ?? SpecialPermissionKeys.Deny;

        if (IsCreate)
        {
            Permissions.ValidatePermission(permission, Localizer);
            return;
        }

        if (!Permissions.HasPermission(permission))
            throw DataValidation.EntityNotFoundError(Row, GetRequestEntityId(), Localizer);
    }

    /// <summary>
    /// Attaches an handler to unit of work's OnCommit event to invalidate 
    /// the cache items related to this row's cache group
    /// </summary>
    protected virtual void InvalidateCacheOnCommit()
    {
        Cache.InvalidateOnCommit(UnitOfWork, Row);
    }

    /// <summary>
    /// Creates an exception indicating that the handler has not been initialized.
    /// </summary>
    /// <param name="property">The property being accessed.</param>
    /// <returns>The initialization error.</returns>
    private InvalidOperationException PropertyReadError([CallerMemberName] string? property = default)
    {
        return new InvalidOperationException($"Error reading '{property}' of {GetType().Name}. The handler has not been initialized.");
    }

    /// <summary>
    /// Gets the two level cache from the request context.
    /// </summary>
    public ITwoLevelCache Cache => Context.Cache;

    /// <summary>
    /// Gets the request context.
    /// </summary>
    public IRequestContext Context { get; } = context ?? throw new ArgumentNullException(nameof(context));

    /// <summary>
    /// Gets the localizer from the request context.
    /// </summary>
    public ITextLocalizer Localizer => Context.Localizer;

    /// <summary>
    /// Gets the permission service from the request context.
    /// </summary>
    public IPermissionService Permissions => Context.Permissions;

    /// <summary>
    /// Gets the current user from the request context.
    /// </summary>
    public ClaimsPrincipal? User => Context.User;

    /// <summary>
    /// Gets the current connection.
    /// </summary>
    public IDbConnection Connection { get => unitOfWork?.Connection ?? throw PropertyReadError(); }

    /// <summary>
    /// Gets the current unit of work.
    /// </summary>
    public IUnitOfWork UnitOfWork { get => unitOfWork ?? throw PropertyReadError(); protected set => unitOfWork = value; }

    /// <summary>
    /// Gets the old entity for update.
    /// </summary>
    public TRow? Old { get; protected set; }

    /// <summary>
    /// Gets the concurrency version used for the update operation, when the row implements
    /// <see cref="IConcurrencyVersionRow"/>. It is the value sent by the client when present,
    /// otherwise the value loaded from the database.
    /// </summary>
    protected object? concurrencyVersion;

    /// <summary>
    /// Gets the inserted entity for Create and the new entity for Update.
    /// </summary>
    public TRow Row { get => field ?? throw PropertyReadError(); protected set; }

    /// <summary>
    /// Returns true if this is a Create operation.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Old))]
    public bool IsCreate => Old == null;

    /// <summary>
    /// Returns true if this is an Update operation.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Old))]
    public bool IsUpdate => Old != null;

    /// <summary>
    /// Gets the request object.
    /// </summary>
    public TSaveRequest Request { get => field ?? throw PropertyReadError(); protected set; }

    /// <summary>
    /// Gets the response object.
    /// </summary>
    public TSaveResponse Response { get => field ?? throw PropertyReadError(); protected set; }

    /// <summary>
    /// A state bag for behaviors to preserve state among their methods.
    /// It will be cleared before each request, e.g. Process call.
    /// </summary>
    public IDictionary<string, object?> StateBag { get; } = new Dictionary<string, object?>();

    ISaveRequest ISaveRequestHandler.Request => Request;
    SaveResponse ISaveRequestHandler.Response => Response;
    IRow? ISaveRequestHandler.Old => Old;
    IRow ISaveRequestHandler.Row => Row;
}
