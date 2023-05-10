using System.Security.Cryptography;

namespace Serenity.Web.Providers;

/// <summary>
/// Used to be a membership provider but now only contains a SHA512 helper
/// </summary>
public static class SiteMembershipProvider
{
    /// <summary>
    /// Computes SHA512 of the string
    /// </summary>
    /// <param name="s">String</param>
    /// <exception cref="ArgumentNullException">String is null or empty</exception>
    public static string ComputeSHA512(string s)
    {
        if (string.IsNullOrEmpty(s))
            throw new ArgumentNullException(nameof(s));

        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(s);
        var sha512 = SHA512.Create();
        buffer = sha512.ComputeHash(buffer);
        return Convert.ToBase64String(buffer).Substring(0, 86); // strip padding
    }
}