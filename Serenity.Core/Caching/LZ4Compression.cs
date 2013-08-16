using System;
using System.Collections.Generic;

namespace Serenity
{
    /// <summary>
    /// In memory distributed cache implementation, which emulates an IDistributedCache.
    /// </summary>
    public static class LZ4Compression
    {
        ///// <summary>
        ///// LZ4 algoritmasını kullanarak string i sıkıştırır (LZ4 çok hızlı bir sıkıştırma yöntemidir)
        ///// http://lz4net.codeplex.com/
        ///// </summary>
        ///// <param name="data">Sıkıştırılacak veri</param>
        ///// <returns>Sıkıştırılmış veri</returns>
        //public static byte[] FastCompressString(string input)
        //{
        //    if (input == null)
        //        return null;

        //    return FastCompressBytes(Encoding.UTF8.GetBytes(input));
        //}

        ///// <summary>
        ///// LZ4 algoritmasını kullanarak string i açar (LZ4 çok hızlı bir sıkıştırma yöntemidir)
        ///// http://lz4net.codeplex.com/
        ///// </summary>
        ///// <param name="data">Sıkıştırılacak veri</param>
        ///// <returns>Sıkıştırılmış veri</returns>
        //public static string FastDecompressString(byte[] input)
        //{
        //    if (input == null)
        //        return null;

        //    return Encoding.UTF8.GetString(FastDecompressBytes(input));
        //}

        ///// <summary>
        ///// LZ4 algoritmasını kullanarak veriyi sıkıştırır (LZ4 çok hızlı bir sıkıştırma yöntemidir)
        ///// http://lz4net.codeplex.com/
        ///// </summary>
        ///// <param name="data">Sıkıştırılacak veri</param>
        ///// <returns>Sıkıştırılmış veri</returns>
        //public static byte[] FastCompressBytes(byte[] input)
        //{
        //    if (input == null)
        //        return input;

        //    using (var ms = new MemoryStream(input.Length))
        //    {
        //        using (var cs = new LZ4.LZ4Stream(ms, System.IO.Compression.CompressionMode.Compress, false, 8192))
        //        {
        //            cs.Write(input, 0, input.Length);
        //        }
        //        return ms.ToArray();
        //    }
        //}

        ///// <summary>
        ///// LZ4 ile sıkıştırılmış veriyi açar
        ///// </summary>
        ///// <param name="data">Açılacak veri</param>
        ///// <returns>Açılmış veri</returns>
        //public static byte[] FastDecompressBytes(byte[] input)
        //{
        //    if (input == null)
        //        return null;

        //    using (var ms = new MemoryStream(input))
        //    {
        //        using (var ds = new LZ4.LZ4Stream(ms, System.IO.Compression.CompressionMode.Decompress, false, 8192))
        //        {
        //            using (var output = new MemoryStream())
        //            {
        //                ds.CopyTo(output, 8192);
        //                output.Flush();
        //                return output.ToArray();
        //            }
        //        }
        //    }
        //}
    }
}