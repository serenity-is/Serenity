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

            if (!IoC.CanResolve<ICacheCompressor>())
                return input;

            using (var ms = new MemoryStream(input.Length))
            {
                IoC.Resolve<ICacheCompressor>().CompressBytes(ms, input);
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

            if (!IoC.CanResolve<ICacheCompressor>())
                return input;

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

        public static byte[] CompressBytesIf(byte[] input, int minCompressLength = 4096)
        {
            if (input == null || !IoC.CanResolve<ICacheCompressor>())
                return input;

            if (input.Length > minCompressLength)
                using (var ms = new MemoryStream(input.Length + 1))
                {
                    ms.WriteByte(1);

                    IoC.Resolve<ICacheCompressor>().CompressBytes(ms, input);
                    return ms.ToArray();
                }

            byte[] result = new byte[input.Length + 1];
            result[0] = 0;
            input.CopyTo(result, 1);
            return result;
        }

        public static byte[] DecompressBytesIf(byte[] input)
        {
            if (input == null)
                throw new ArgumentOutOfRangeException("input");

            if (!IoC.CanResolve<ICacheCompressor>())
                return input;

            if (input.Length < 1)
                throw new ArgumentOutOfRangeException("input");

            bool compressed = input[0] == (byte)1;
            byte[] data = new byte[input.Length - 1];
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