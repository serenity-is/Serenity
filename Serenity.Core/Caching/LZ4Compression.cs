using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Serenity
{
    /// <summary>
    /// In memory distributed cache implementation, which emulates an IDistributedCache.
    /// </summary>
    public static class LZ4Compression
    {
        /// <summary>
        /// LZ4 algoritmasını kullanarak string i sıkıştırır (LZ4 çok hızlı bir sıkıştırma yöntemidir)
        /// http://lz4net.codeplex.com/
        /// </summary>
        /// <param name="data">Sıkıştırılacak veri</param>
        /// <returns>Sıkıştırılmış veri</returns>
        public static byte[] CompressString(string input)
        {
            if (input == null)
                return null;

            return CompressBytes(Encoding.UTF8.GetBytes(input));
        }

        /// <summary>
        /// LZ4 algoritmasını kullanarak string i açar (LZ4 çok hızlı bir sıkıştırma yöntemidir)
        /// http://lz4net.codeplex.com/
        /// </summary>
        /// <param name="data">Sıkıştırılacak veri</param>
        /// <returns>Sıkıştırılmış veri</returns>
        public static string DecompressString(byte[] input)
        {
            if (input == null)
                return null;

            return Encoding.UTF8.GetString(DecompressBytes(input));
        }

        /// <summary>
        /// LZ4 algoritmasını kullanarak veriyi sıkıştırır (LZ4 çok hızlı bir sıkıştırma yöntemidir)
        /// http://lz4net.codeplex.com/
        /// </summary>
        /// <param name="data">Sıkıştırılacak veri</param>
        /// <returns>Sıkıştırılmış veri</returns>
        public static byte[] CompressBytes(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            using (var ms = new MemoryStream(input.Length))
            {
                using (var cs = new LZ4.LZ4Stream(ms, System.IO.Compression.CompressionMode.Compress, false, 8192))
                {
                    cs.Write(input, 0, input.Length);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// LZ4 ile sıkıştırılmış veriyi açar
        /// </summary>
        /// <param name="data">Açılacak veri</param>
        /// <returns>Açılmış veri</returns>
        public static byte[] DecompressBytes(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            using (var ms = new MemoryStream(input))
            {
                using (var ds = new LZ4.LZ4Stream(ms, System.IO.Compression.CompressionMode.Decompress, false, 8192))
                {
                    using (var output = new MemoryStream())
                    {
                        ds.CopyTo(output, 8192);
                        output.Flush();
                        return output.ToArray();
                    }
                }
            }
        }

        public static byte[] CompressBytesIf(byte[] input, int minCompressLength = 4096)
        {
            if (input == null)
                return input;

            if (input.Length > minCompressLength)
                using (var ms = new MemoryStream(input.Length + 1))
                {
                    ms.WriteByte(1);

                    using (var cs = new LZ4.LZ4Stream(ms, System.IO.Compression.CompressionMode.Compress, false, 8192))
                    {
                        cs.Write(input, 0, input.Length);
                    }
                    return ms.ToArray();
                }

            byte[] result = new byte[input.Length + 1];
            result[0] = 0;
            input.CopyTo(result, 1);
            return result;
        }

        public static byte[] DecompressBytesIf(byte[] input)
        {
            if (input == null || input.Length < 1)
                throw new ArgumentOutOfRangeException("input");

            bool compressed = input[0] == (byte)1;
            byte[] data = new byte[input.Length - 1];
            Array.Copy(input, 1, data, 0, input.Length - 1);
            if (compressed)
                return LZ4Compression.DecompressBytes(data);
            else
                return data;
        }
    }
}