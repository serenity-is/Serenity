using Microsoft.AspNetCore.WebUtilities;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Serenity.Web;

/// <summary>
/// Default implementation for <see cref="IScriptContent"/>
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="content">Content</param>
/// <param name="time">Time</param>
/// <param name="compressionLevel">Suggested compression level</param>
/// <exception cref="ArgumentNullException"></exception>
public class ScriptContent(byte[] content, DateTime time, CompressionLevel compressionLevel) : IScriptContent
{
    private readonly CompressionLevel compressionLevel = compressionLevel;
    private string hash;
    private readonly byte[] content = content ?? throw new ArgumentNullException(nameof(content));
    private byte[] gzipContent;
    private byte[] brotliContent;

    /// <summary>
    /// Gets script generated time
    /// </summary>
    public DateTime Time { get; private set; } = time;

    /// <summary>
    /// Gets script hash
    /// </summary>
    public string Hash 
    { 
        get
        {
            if (hash == null)
            {
                byte[] result = MD5.HashData(content);
                hash = WebEncoders.Base64UrlEncode(result);
            }

            return hash;
        }
    }

    /// <inheritdoc/>
    public byte[] Content => content;

    /// <inheritdoc/>
    public bool CanCompress => compressionLevel != CompressionLevel.NoCompression;

    /// <inheritdoc/>
    public byte[] CompressedContent
    {
        get
        {
            if (!CanCompress)
                throw new InvalidOperationException("Script does not allow compression!");

            if (gzipContent == null)
            {
                using var cs = new MemoryStream(content.Length);
                using (var gz = new GZipStream(cs, compressionLevel))
                {
                    gz.Write(content, 0, content.Length);
                    gz.Flush();
                }

                gzipContent = cs.ToArray();
            }

            return gzipContent;
        }
    }

    /// <inheritdoc/>
    public byte[] BrotliContent
    {
        get
        {
            if (!CanCompress)
                throw new InvalidOperationException("Script does not allow compression!");

            if (brotliContent == null)
            {
                CompressionLevel brotliLevel;
                if (Environment.Version.Major >= 7)
                {
                    // .NET 7 does not allow custom levels
                    brotliLevel = compressionLevel switch
                    {
                        CompressionLevel.Optimal => CompressionLevel.Optimal,
                        // CompressionLevel.SmallestSize is too slow with Brotli
                        CompressionLevel.SmallestSize => CompressionLevel.Optimal, 
                        _ => CompressionLevel.Fastest
                    };
                }
                else
                {
                    brotliLevel = compressionLevel switch
                    {
                        CompressionLevel.Optimal => (CompressionLevel)4,
                        // level 5-9 almost same compression, and 10+ is much slower
                        CompressionLevel.SmallestSize => (CompressionLevel)5,
                        _ => (CompressionLevel)1
                    };
                }

                using var cs = new MemoryStream(content.Length);
                using (var br = new BrotliStream(cs, brotliLevel))
                {
                    br.Write(content, 0, content.Length);
                    br.Flush();
                }

                brotliContent = cs.ToArray();
            }

            return brotliContent;
        }
    }
}