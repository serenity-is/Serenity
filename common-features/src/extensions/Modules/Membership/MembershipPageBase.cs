using Serenity.Web.Providers;
using System.Reflection;

namespace Serenity.Extensions;

/// <summary>
/// Base class for membership pages that provides common helpers
/// for password hashing, salt generation, and user lookup.
/// </summary>
public abstract class MembershipPageBase<TUserRow> : Controller
    where TUserRow: class, IRow, IIdRow, IEmailRow, IPasswordRow, new()
{

    /// <summary>
    /// Returns a validation error view with the specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>The validation error view result.</returns>
    protected virtual ActionResult Error(string message)
    {
        return View("~/Views/Errors/ValidationError.cshtml", new ValidationError(message));
    }

    /// <summary>
    /// Generates a random salt of the configured size.
    /// </summary>
    /// <param name="settings">The membership settings.</param>
    /// <returns>The generated salt.</returns>
    protected virtual string GenerateSalt(MembershipSettings settings)
    {
        return IO.TemporaryFileHelper.RandomFileCode()[..settings.SaltSize];
    }

    /// <summary>
    /// Calculates the SHA-512 hash of the password combined with the salt.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="salt">The salt.</param>
    /// <returns>The computed hash.</returns>
    protected virtual string CalculateHash(string password, string salt)
    {
        return SiteMembershipProvider.ComputeSHA512(password + salt);
    }

    /// <summary>
    /// Computes a deterministic hash code for the specified string.
    /// </summary>
    /// <param name="str">The string to hash.</param>
    /// <returns>The deterministic hash code.</returns>
    public static int GetDeterministicHashCode(string str)
    {
        unchecked
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            for (int i = 0; i < str.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1)
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }

    /// <summary>
    /// Gets a nonce value for the specified user based on its update/insert date and password fields.
    /// </summary>
    /// <param name="user">The user row.</param>
    /// <returns>The nonce value.</returns>
    protected virtual int GetNonceFor(TUserRow user)
    {
        return GetDeterministicHashCode(
            ((user as IUpdateDateRow)?.UpdateDateField[user] ??
             (user as IInsertDateRow)?.InsertDateField[user] ??
             DateTime.Today).ToString("s") +
             (user as IPasswordRow)?.PasswordHashField[user] +
             (user as IPasswordRow)?.PasswordSaltField[user]);
    }

    /// <summary>
    /// Gets the connection key for the user row type.
    /// </summary>
    /// <returns>The connection key.</returns>
    protected virtual string GetConnectionKey() => typeof(TUserRow).GetCustomAttribute<ConnectionKeyAttribute>(inherit: false)?.Value ?? "Default";
}
