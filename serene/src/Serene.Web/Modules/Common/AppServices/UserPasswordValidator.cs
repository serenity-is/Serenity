using Microsoft.Extensions.Logging;
using Serene.Administration;

namespace Serene.AppServices;

public class UserPasswordValidator(ITwoLevelCache cache, ISqlConnections sqlConnections, IUserRetrieveService userRetriever,
    ILogger<UserPasswordValidator> log = null, IDirectoryService directoryService = null) : IUserPasswordValidator
{
    protected readonly ITwoLevelCache cache = cache ?? throw new ArgumentNullException(nameof(cache));
    protected readonly ISqlConnections sqlConnections = sqlConnections ?? throw new ArgumentNullException(nameof(sqlConnections));
    protected readonly IUserRetrieveService userRetriever = userRetriever ?? throw new ArgumentNullException(nameof(userRetriever));
    protected readonly IDirectoryService directoryService = directoryService;
    protected readonly ILogger<UserPasswordValidator> Log = log;

    public PasswordValidationResult Validate(ref string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            return PasswordValidationResult.EmptyUsername;

        if (string.IsNullOrEmpty(password))
            return PasswordValidationResult.EmptyPassword;

        username = username.TrimToEmpty();

        if (userRetriever.ByUsername(username) is UserDefinition user)
            return ValidateExistingUser(ref username, password, user);

        return ValidateFirstTimeUser(ref username, password);
    }

    private PasswordValidationResult ValidateExistingUser(ref string username, string password, UserDefinition user)
    {
        username = user.Username;

        if (user.IsActive != 1)
        {
            Log?.LogError("Inactive user login attempt: {username}", username);
            return PasswordValidationResult.InactiveUser;
        }

        // prevent more than 50 invalid login attempts in 30 minutes
        var throttler = new Throttler(cache.Memory, "ValidateUser:" + username.ToLowerInvariant(), TimeSpan.FromMinutes(30), 50);
        if (!throttler.Check())
            return PasswordValidationResult.Throttle;

        bool validatePassword() => UserHelper.CalculateHash(password, user.PasswordSalt)
            .Equals(user.PasswordHash, StringComparison.OrdinalIgnoreCase);

        if (user.Source == "site" || user.Source == "sign" || directoryService == null)
        {
            if (validatePassword())
            {
                throttler.Reset();
                return PasswordValidationResult.Valid;
            }

            return PasswordValidationResult.Invalid;
        }

        if (user.Source != "ldap")
            return PasswordValidationResult.UnknownSource;

        if (!string.IsNullOrEmpty(user.PasswordHash) &&
            user.LastDirectoryUpdate != null &&
            user.LastDirectoryUpdate.Value.AddHours(1) >= DateTime.Now)
        {
            if (validatePassword())
            {
                throttler.Reset();
                return PasswordValidationResult.Valid;
            }

            return PasswordValidationResult.Invalid;
        }

        DirectoryEntry entry;
        try
        {
            entry = directoryService.Validate(username, password);
            if (entry == null)
                return PasswordValidationResult.Invalid;

            throttler.Reset();
        }
        catch (Exception ex)
        {
            Log?.LogError(ex, "Error on directory access");

            // couldn't access directory. allow user to login with cached password
            if (!string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                if (validatePassword())
                {
                    throttler.Reset();
                    return PasswordValidationResult.Valid;
                }

                return PasswordValidationResult.Invalid;
            }

            throw;
        }

        try
        {
            string salt = user.PasswordSalt.TrimToNull();
            var hash = UserHelper.GenerateHash(password, ref salt);
            var displayName = entry.FirstName + " " + entry.LastName;
            var email = entry.Email.TrimToNull() ?? user.Email ?? (username + "@yourdefaultdomain.com");

            using var connection = sqlConnections.NewFor<UserRow>();
            using var uow = new UnitOfWork(connection);
            var fld = UserRow.Fields;
            new SqlUpdate(fld.TableName)
                .Set(fld.DisplayName, displayName)
                .Set(fld.PasswordHash, hash)
                .Set(fld.PasswordSalt, salt)
                .Set(fld.Email, email)
                .Set(fld.LastDirectoryUpdate, DateTime.Now)
                .WhereEqual(fld.UserId, user.UserId)
                .Execute(connection, ExpectedRows.One);

            uow.Commit();

            userRetriever.RemoveCachedUser(user, cache);

            return PasswordValidationResult.Valid;
        }
        catch (Exception ex)
        {
            Log?.LogError(ex, "Error while updating directory user");
            return PasswordValidationResult.Valid;
        }
    }

    private PasswordValidationResult ValidateFirstTimeUser(ref string username, string password)
    {
        var throttler = new Throttler(cache.Memory, "ValidateUser:" + username.ToLowerInvariant(), TimeSpan.FromMinutes(30), 50);
        if (!throttler.Check())
            return PasswordValidationResult.Throttle;

        if (directoryService == null)
            return PasswordValidationResult.Invalid;

        DirectoryEntry entry;
        try
        {
            entry = directoryService.Validate(username, password);
            if (entry == null)
                return PasswordValidationResult.Invalid;

            throttler.Reset();
        }
        catch (Exception ex)
        {
            Log?.LogError(ex, "Error on directory first time authentication");
            return PasswordValidationResult.DirectoryError;
        }

        try
        {
            string salt = null;
            var hash = UserHelper.GenerateHash(password, ref salt);
            var displayName = entry.FirstName + " " + entry.LastName;
            var email = entry.Email.TrimToNull() ?? (username + "@yourdefaultdomain.com");
            username = entry.Username.TrimToNull() ?? username;

            using var connection = sqlConnections.NewFor<UserRow>();
            using var uow = new UnitOfWork(connection);
            var userId = (int)connection.InsertAndGetID(new UserRow
            {
                Username = username,
                Source = "ldap",
                DisplayName = displayName,
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                IsActive = 1,
                InsertDate = DateTime.Now,
                InsertUserId = 1,
                LastDirectoryUpdate = DateTime.Now
            });

            uow.Commit();

            userRetriever.RemoveCachedUser(userId.ToInvariant(), username, cache);

            return PasswordValidationResult.Valid;
        }
        catch (Exception ex)
        {
            Log?.LogError(ex, "Error while importing directory user");
            return PasswordValidationResult.DirectoryError;
        }
    }
}