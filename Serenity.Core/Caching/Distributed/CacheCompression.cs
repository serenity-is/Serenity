using Serenity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Serenity
{
    public interface ICacheCompressor
    {
        void CompressBytes(Stream target, byte[] input);
        Stream CreateDecompressionStream(Stream input);
    }

    public static class CacheCompression
    {
        /// <param name="input">Compresses input string with ICacheCompressor implementation.</param>
        /// <returns>Compressed bytes or just UTF8 bytes.</returns>
        public static byte[] CompressString(string input)
        {
            if (input == null)
                return null;

            return CompressBytes(Encoding.UTF8.GetBytes(input));
        }

        /// <summary>
        /// Decompresses, compressed bytes with ICacheCompressor implementation.
        /// </summary>
        /// <param name="input">Data to decompress</param>
        /// <returns>Decompressed data</returns>
        public static string DecompressString(byte[] input)
        {
            if (input == null)
                return null;

            return Encoding.UTF8.GetString(DecompressBytes(input));
        }

        /// <summary>
        /// Compresses input bytes with ICacheCompressor implementation.
        /// </summary>
        /// <param name="input">Data to be compressed</param>
        /// <returns>Compressed bytes</returns>
        public static byte[] CompressBytes(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            using (var ms = new MemoryStream(input.Length))
            {
                IoC.Resolve<ICacheCompressor>().CompressBytes(ms, input);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Decompresses bytes, compressed with ICacheCompressor implementation.
        /// </summary>
        /// <param name="input">Data to be decompressed.</param>
        /// <returns>Decompressed bytes.</returns>
        public static byte[] DecompressBytes(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            using (var ms = new MemoryStream(input))
            {
                using (var ds = IoC.Resolve<ICacheCompressor>().CreateDecompressionStream(ms))
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

        /// <summary>
        /// Compresses byte array, if its larger than given size and 
        /// ICacheCompressor is available.
        /// </summary>
        /// <param name="input">Input array</param>
        /// <param name="minCompressLength">Compress if lower than this size</param>
        /// <returns>Compressed bytes or raw bytes with 0 appended in front</returns>
        public static byte[] CompressBytesIf(byte[] input, int minCompressLength = 4096)
        {
            if (input == null)
                return null;

            if (input.Length > minCompressLength &&
                IoC.CanResolve<ICacheCompressor>())
            {
                using (var ms = new MemoryStream(input.Length + 1))
                {
                    ms.WriteByte(1);
                    IoC.Resolve<ICacheCompressor>().CompressBytes(ms, input);

                    return ms.ToArray();
                }
            }

            var result = new byte[input.Length + 1];
            result[0] = 0;
            input.CopyTo(result, 1);
            return result;
        }

        /// <summary>
        /// Decompresses bytes already compressed with CompressBytesIf function.
        /// </summary>
        /// <param name="input">Input array</param>
        /// <returns>Decompressed data</returns>
        public static byte[] DecompressBytesIf(byte[] input)
        {
            if (input == null)
                throw new ArgumentOutOfRangeException("input");

            if (input.Length < 1)
                throw new ArgumentOutOfRangeException("input");

            var compressed = input[0] == (byte)1;
            var data = new byte[input.Length - 1];
            Array.Copy(input, 1, data, 0, input.Length - 1);

            if (compressed)
                return CacheCompression.DecompressBytes(data);
            else
                return data;
        }
    }

    // <summary>
    // LZ4 algoritmasını kullanarak string i sıkıştırır (LZ4 çok hızlı bir sıkıştırma yöntemidir)
    // http://lz4net.codeplex.com/
    // </summary>
    //public class LZ4Compressor : IFastCompressor
    //{
    //    public void CompressBytes(Stream target, byte[] input)
    //    {
    //        using (var cs = new LZ4.LZ4Stream(target, System.IO.Compression.CompressionMode.Compress, false, 8192))
    //        {
    //            cs.Write(input, 0, input.Length);
    //        }
    //    }

    //    public Stream CreateDecompressionStream(Stream input)
    //    {
    //        return new ds = new LZ4.LZ4Stream(input, System.IO.Compression.CompressionMode.Decompress, false, 8192);
    //    }
    //}
}