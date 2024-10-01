using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;

namespace Serenity;

/// <summary>
/// Extension methods for data protector to support tokens created via BinaryWriter
/// </summary>
public static class DataProtectorBinaryTokenExtensions
{
    /// <summary>
    /// Encrypts a token populated by the given callback and returns the encrypted token in 
    /// base64 URL encoded format
    /// </summary>
    /// <param name="protector">Data protector</param>
    /// <param name="callback">Callback to populate the writer</param>
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
    /// Decrypts the given token in base64 URL encoded format and return a binary reader with the decrypted data
    /// </summary>
    /// <param name="protector">Data protector</param>
    /// <param name="token">Encrypted token in base 64 URL encoded format</param>
    public static System.IO.BinaryReader UnprotectBinary(this IDataProtector protector, string token)
    {
        ArgumentNullException.ThrowIfNull(token);
        var tokenBytes = WebEncoders.Base64UrlDecode(token);
        var ticket = protector.Unprotect(tokenBytes);
        var ms = new System.IO.MemoryStream(ticket);
        return new System.IO.BinaryReader(ms);
    }
}