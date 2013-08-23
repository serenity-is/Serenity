using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Serenity
{
    public static class CacheSerialization
    {
        public static byte[] BinaryWrite(Action<BinaryWriter> callback, int minCompressLength = 4096)
        {
            byte[] data;
            using (var ms = new MemoryStream())
            using (var sw = new BinaryWriter(ms, Encoding.UTF8))
            {
                callback(sw);
                data = ms.ToArray();
            }

            return LZ4Compression.CompressBytesIf(data, minCompressLength);
        }

        public static TValue BinaryRead<TValue>(byte[] input, Func<BinaryReader, TValue> deserialize)
        {
            using (var ms = new MemoryStream(LZ4Compression.DecompressBytesIf(input)))
            using (var sw = new BinaryReader(ms, Encoding.UTF8))
            {
                return deserialize(sw);
            }
        }
    }
}