using MyRow = Serenity.Extensions.Entities.UserPreferenceRow;

namespace Serenity.Extensions.Repositories;

/// <summary>
/// Repository for managing user preferences.
/// </summary>
public class UserPreferenceRepository(IRequestContext context) : BaseRepository(context)
{
    /// <summary>
    /// Gets the id of the current user, converted to the type of the user ID field, or null when the user has no identifier,
    /// e.g. when not logged in, as preferences are kept per user.
    /// </summary>
    protected virtual object? GetUserId()
    {
        var identifier = Context.User?.GetIdentifier();
        return string.IsNullOrEmpty(identifier) ? null :
            MyRow.Fields.UserId.ConvertValue(identifier, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Updates or deletes a user preference of the current user.
    /// </summary>
    /// <param name="uow">The unit of work.</param>
    /// <param name="request">The update request.</param>
    /// <returns>The save response.</returns>
    /// <exception cref="ValidationError">The current user has no identifier, e.g. is not logged in, so the preference
    /// would be no one's.</exception>
    public SaveResponse Update(IUnitOfWork uow, UserPreferenceUpdateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Name);
        ArgumentNullException.ThrowIfNull(request.PreferenceType);

        var fld = MyRow.Fields;
        var userId = GetUserId() ?? throw new ValidationError("NotLoggedIn", null,
            CoreTexts.Authorization.NotLoggedIn.ToString(Context.Localizer));

        var criteria = fld.UserId == new ValueCriteria(userId) &
            fld.PreferenceType == request.PreferenceType &
            fld.Name == request.Name;

        if (string.IsNullOrEmpty(request.Value))
        {
            new SqlDelete(fld.TableName)
                .Where(criteria)
                .Execute(uow.Connection, ExpectedRows.ZeroOrOne);

            return new SaveResponse();
        }

        if (new SqlUpdate(fld.TableName)
                .Set(fld.Value, request.Value)
                .Where(criteria)
                .Execute(uow.Connection, ExpectedRows.ZeroOrOne) == 0)
        {
            try
            {
                new SqlInsert(fld.TableName)
                    .Set(fld.UserId, userId)
                    .Set(fld.PreferenceType, request.PreferenceType)
                    .Set(fld.Name, request.Name)
                    .Set(fld.Value, request.Value)
                    .Execute(uow.Connection);
            }
            catch
            {
                // retry update in case a race condition resulted in
                // unique index violation during insert
                if (new SqlUpdate(fld.TableName)
                    .Set(fld.Value, request.Value)
                    .Where(criteria)
                    .Execute(uow.Connection, ExpectedRows.ZeroOrOne) == 0)
                {
                    // re-throw if the issue was not actually that
                    throw;
                }
            }
        }

        return new SaveResponse();
    }

    /// <summary>
    /// Retrieves a user preference, which is empty for a user without an identifier.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="request">The retrieve request.</param>
    /// <returns>The retrieve response.</returns>
    public UserPreferenceRetrieveResponse Retrieve(IDbConnection connection, UserPreferenceRetrieveRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Name);
        ArgumentNullException.ThrowIfNull(request.PreferenceType);

        var fld = MyRow.Fields;
        var userId = GetUserId();
        if (userId is null)
            return new UserPreferenceRetrieveResponse();

        var row = connection.TryFirst<MyRow>(
            fld.UserId == new ValueCriteria(userId) &
            fld.PreferenceType == request.PreferenceType &
            fld.Name == request.Name);

        if (row == null)
            return new UserPreferenceRetrieveResponse();

        return new UserPreferenceRetrieveResponse
        {
            Value = row.Value
        };
    }
}