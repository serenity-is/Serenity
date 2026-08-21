using System.Security.Cryptography;

namespace Serenity.Web.Providers;

/// <summary>
/// Used to be a membership provider but now only contains a SHA512 helper.
/// </summary>
public static class SiteMembershipProvider
{
    /// <summary>
    /// Computes the SHA512 hash of the given string.
    /// </summary>
    /// <param name="s">The string to hash.</param>
    /// <returns>The base64 encoded SHA512 hash with padding stripped.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null or empty.</exception>
    public static string ComputeSHA512(string s)
    {
        if (string.IsNullOrEmpty(s))
            throw new ArgumentNullException(nameof(s));

        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(s);
        buffer = SHA512.HashData(buffer);
        return Convert.ToBase64String(buffer)[..86]; // strip padding
    }
}