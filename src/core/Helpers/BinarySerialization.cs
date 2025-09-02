using System.IO;

namespace Serenity.Data;

/// <summary>
/// Contains helper functions for serialization and deserialization of data items (e.g.cached items)
/// </summary>
public static class BinarySerialization
{
    /// <summary>
    /// A helper method to serialize objects with BinaryWriter. Creates a memory stream 
    /// and a BinaryWriter on it, and invokes the callback specified.
    /// </summary>
    /// <param name="serialize">Serialization delegate</param>
    public static byte[] Serialize(Action<BinaryWriter> serialize)
    {
        byte[] data;
        using (var ms = new MemoryStream())
        using (var sw = new BinaryWriter(ms, Encoding.UTF8))
        {
            serialize(sw);
            data = ms.ToArray();
        }

        return data;
    }

    /// <summary>
    /// A helper method to deserialize objects with BinaryWriter. Creates a memory stream 
    /// and a BinaryReader on it, and invokes the callback specified.
    /// </summary>
    /// <param name="input">Input array</param>
    /// <param name="deserialize">Deserialization delegate</param>
    public static TValue Deserialize<TValue>(byte[] input, Func<BinaryReader, TValue> deserialize)
    {
        using var ms = new MemoryStream(input);
        using var sw = new BinaryReader(ms, Encoding.UTF8);
        return deserialize(sw);
    }
}