namespace Serenity.TestUtils;

/// <summary>
/// A reader whose <see cref="GetValue(int)"/> does not return a byte array, forcing
/// consumers to read binary fields through <see cref="GetBytes(int, long, byte[]?, int, int)"/>.
/// <see cref="GetBytes(int, long, byte[]?, int, int)"/> intentionally returns at most
/// <c>chunkSize</c> bytes per call to simulate providers that perform partial reads.
/// </summary>
public class GetBytesOnlyDataReader(byte[] data, int chunkSize = int.MaxValue) : MockDbDataReader(new { Value = new object() })
{
    public override long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length)
    {
        if (buffer == null)
            return data.Length;

        var count = Math.Min(Math.Min(length, chunkSize), data.Length - (int)fieldOffset);
        if (count <= 0)
            return 0;

        Array.Copy(data, (int)fieldOffset, buffer, bufferoffset, count);
        return count;
    }
}
