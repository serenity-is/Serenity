using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;

namespace Serenity;

/// <summary>
/// Extension methods for <see cref="IDataProtector"/> to support tokens
/// created via a <see cref="System.IO.BinaryWriter"/>.
/// </summary>
public static class DataProtectorBinaryTokenExtensions
{
    /// <summary>
    /// Encrypts a token populated by the given callback and returns the
    /// encrypted token in base64 URL encoded format.
    /// </summary>
    /// <param name="protector">The data protector.</param>
    /// <param name="callback">The callback used to populate the writer.</param>
    /// <returns>The protected token in base64 URL encoded format.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="callback"/> is <c>null</c>.</exception>
    public static string ProtectBinary(this IDataProtector protector, Action<System.IO.BinaryWriter> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        byte[] bytes;
        using var ms = new System.IO.MemoryStream();
        using var bw = new System.IO.BinaryWriter(ms);
        callback(bw);
        bw.Flush();
        bytes = ms.ToArray();
        var protectedBytes = protector.Protect(bytes);
        return WebEncoders.Base64UrlEncode(protectedBytes);
    }

    /// <summary>
    /// Decrypts the given token in base64 URL encoded format and returns a
    /// binary reader over the decrypted data.
    /// </summary>
    /// <param name="protector">The data protector.</param>
    /// <param name="token">The encrypted token in base64 URL encoded format.</param>
    /// <returns>A <see cref="System.IO.BinaryReader"/> over the decrypted data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="token"/> is <c>null</c>.</exception>
    public static System.IO.BinaryReader UnprotectBinary(this IDataProtector protector, string token)
    {
        ArgumentNullException.ThrowIfNull(token);
        var tokenBytes = WebEncoders.Base64UrlDecode(token);
        var ticket = protector.Unprotect(tokenBytes);
        var ms = new System.IO.MemoryStream(ticket);
        return new System.IO.BinaryReader(ms);
    }
}