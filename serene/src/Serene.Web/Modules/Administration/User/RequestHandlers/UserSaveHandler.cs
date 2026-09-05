using MyRow = Serene.Administration.UserRow;

namespace Serene.Administration;

public interface IUserSaveHandler : ISaveHandlerAsync<MyRow> { }

public class UserSaveHandler(IRequestContext context, IOptions<EnvironmentSettings> environmentOptions)
    : SaveRequestHandlerAsync<MyRow>(context), IUserSaveHandler
{
    private static MyRow.RowFields Fld { get { return MyRow.Fields; } }

    private string password;
    private readonly IOptions<EnvironmentSettings> environmentOptions = environmentOptions ??
        throw new ArgumentNullException(nameof(environmentOptions));

    protected override void GetEditableFields(HashSet<Field> editable)
    {
        base.GetEditableFields(editable);

        if (!Permissions.HasPermission(PermissionKeys.Security))
        {
            editable.Remove(Fld.Source);
            editable.Remove(Fld.IsActive);
        }
    }

    private static async Task<string> ValidateUsernameAsync(IDbConnection connection, string username,
        int? existingUserId, ITextLocalizer localizer, CancellationToken cancellationToken = default)
    {
        username = username.TrimToNull() ?? throw DataValidation.RequiredError(Fld.Username, localizer);
        if (!UserHelper.IsValidUsername(username))
            throw new ValidationError("InvalidUsername", "Username",
                "Usernames should start with letters, only contain letters and numbers!");

        var existing = await UserHelper.GetUserAsync(connection,
            new Criteria(Fld.Username) == username |
            new Criteria(Fld.Username) == username.Replace('I', 'İ'),
            cancellationToken).ConfigureAwait(false);

        if (existing != null && existingUserId != existing.UserId)
            throw new ValidationError("UniqueViolation", "Username",
                "A user with same name exists. Please choose another!");

        return username;
    }

    protected override async Task ValidateRequestAsync(CancellationToken cancellationToken = default)
    {
        await base.ValidateRequestAsync(cancellationToken).ConfigureAwait(false);

        if (IsUpdate)
        {
            environmentOptions.CheckPublicDemo(Row.UserId);

            if (Row.Username != Old.Username)
                Row.Username = await ValidateUsernameAsync(Connection, Row.Username, Old.UserId.Value,
                    Localizer, cancellationToken).ConfigureAwait(false);

            if (Row.DisplayName != Old.DisplayName)
                Row.DisplayName = UserHelper.ValidateDisplayName(Row.DisplayName, Localizer);
        }

        if (IsCreate)
        {
            Row.Username = await ValidateUsernameAsync(Connection, Row.Username, null,
                Localizer, cancellationToken).ConfigureAwait(false);
            Row.DisplayName = UserHelper.ValidateDisplayName(Row.DisplayName, Localizer);
        }

        if (IsCreate || (Row.IsAssigned(Fld.Password) && !Row.Password.IsEmptyOrNull()))
        {
            if (Row.IsAssigned(Fld.PasswordConfirm) && !Row.PasswordConfirm.IsEmptyOrNull() &&
                Row.Password != Row.PasswordConfirm)
                throw new ValidationError("PasswordConfirmMismatch", "PasswordConfirm",
                    ChangePasswordValidationTexts.PasswordConfirmMismatch.ToString(Localizer));

            password = Row.Password = UserHelper.ValidatePassword(Row.Password, Localizer);
        }
    }

    protected override async Task SetInternalFieldsAsync(CancellationToken cancellationToken = default)
    {
        await base.SetInternalFieldsAsync(cancellationToken).ConfigureAwait(false);

        if (IsCreate)
        {
            Row.Source = "site";
            Row.IsActive = Row.IsActive ?? 1;
        }

        if (IsCreate || !Row.Password.IsEmptyOrNull())
        {
            string salt = null;
            Row.PasswordHash = UserHelper.GenerateHash(password, ref salt);
            Row.PasswordSalt = salt;
        }
    }

    protected override async Task AfterSaveAsync(CancellationToken cancellationToken = default)
    {
        await base.AfterSaveAsync(cancellationToken).ConfigureAwait(false);

        Cache.InvalidateOnCommit(UnitOfWork, Fld);
    }
}
