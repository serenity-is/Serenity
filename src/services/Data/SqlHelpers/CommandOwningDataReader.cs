using System.Collections;
using System.Data.Common;

namespace Serenity.Data;

/// <summary>
/// An <see cref="IDataReader"/> decorator that owns the <see cref="IDbCommand"/>
/// it was created from, disposing the command when the reader is closed or
/// disposed. Used by <see cref="SqlHelper"/> reader paths, which can't dispose
/// the command at return time as the reader must stay open.
/// </summary>
/// <remarks>
/// Extends <see cref="DbDataReader"/> so <c>as DbDataReader</c> casts keep
/// working; all members delegate to the wrapped reader.
/// </remarks>
internal sealed class CommandOwningDataReader(IDataReader reader, IDbCommand command) : DbDataReader
{
    private readonly IDataReader reader = reader ?? throw new ArgumentNullException(nameof(reader));
    private readonly IDbCommand command = command ?? throw new ArgumentNullException(nameof(command));
    private bool released;
    private bool? hasRows;
    private bool hasPrefetchedRow;

    private void Release()
    {
        if (released)
            return;
        released = true;
        try
        {
            reader.Dispose();
        }
        finally
        {
            command.Dispose();
        }
    }

    /// <inheritdoc/>
    public override void Close()
    {
        if (released)
            return;
        try
        {
            reader.Close();
        }
        finally
        {
            Release();
        }
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            Release();
        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    public override async ValueTask DisposeAsync()
    {
        try
        {
            if (!released)
            {
                released = true;
                try
                {
                    if (reader is IAsyncDisposable asyncReader)
                        await asyncReader.DisposeAsync().ConfigureAwait(false);
                    else
                        reader.Dispose();
                }
                finally
                {
                    command.Dispose();
                }
            }
        }
        finally
        {
            await base.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public override int Depth => reader.Depth;

    /// <inheritdoc/>
    public override int FieldCount => reader.FieldCount;

    /// <inheritdoc/>
    public override bool HasRows
    {
        get
        {
            if (reader is DbDataReader dbReader)
                return dbReader.HasRows;

            if (!hasRows.HasValue)
            {
                hasRows = reader.Read();
                hasPrefetchedRow = hasRows.Value;
            }

            return hasRows.Value;
        }
    }

    /// <inheritdoc/>
    public override bool IsClosed => reader.IsClosed;

    /// <inheritdoc/>
    public override int RecordsAffected => reader.RecordsAffected;

    /// <inheritdoc/>
    public override object this[int ordinal] => reader[ordinal];

    /// <inheritdoc/>
    public override object this[string name] => reader[name];

    /// <inheritdoc/>
    public override bool GetBoolean(int ordinal) => reader.GetBoolean(ordinal);

    /// <inheritdoc/>
    public override byte GetByte(int ordinal) => reader.GetByte(ordinal);

    /// <inheritdoc/>
    public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
        => reader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);

    /// <inheritdoc/>
    public override char GetChar(int ordinal) => reader.GetChar(ordinal);

    /// <inheritdoc/>
    public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
        => reader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);

    /// <inheritdoc/>
    public override string GetDataTypeName(int ordinal) => reader.GetDataTypeName(ordinal);

    /// <inheritdoc/>
    public override DateTime GetDateTime(int ordinal) => reader.GetDateTime(ordinal);

    /// <inheritdoc/>
    public override decimal GetDecimal(int ordinal) => reader.GetDecimal(ordinal);

    /// <inheritdoc/>
    public override double GetDouble(int ordinal) => reader.GetDouble(ordinal);

    /// <inheritdoc/>
    public override Type GetFieldType(int ordinal) => reader.GetFieldType(ordinal);

    /// <inheritdoc/>
    public override float GetFloat(int ordinal) => reader.GetFloat(ordinal);

    /// <inheritdoc/>
    public override Guid GetGuid(int ordinal) => reader.GetGuid(ordinal);

    /// <inheritdoc/>
    public override short GetInt16(int ordinal) => reader.GetInt16(ordinal);

    /// <inheritdoc/>
    public override int GetInt32(int ordinal) => reader.GetInt32(ordinal);

    /// <inheritdoc/>
    public override long GetInt64(int ordinal) => reader.GetInt64(ordinal);

    /// <inheritdoc/>
    public override string GetName(int ordinal) => reader.GetName(ordinal);

    /// <inheritdoc/>
    public override int GetOrdinal(string name) => reader.GetOrdinal(name);

    /// <inheritdoc/>
    public override string GetString(int ordinal) => reader.GetString(ordinal);

    /// <inheritdoc/>
    public override object GetValue(int ordinal) => reader.GetValue(ordinal);

    /// <inheritdoc/>
    public override int GetValues(object[] values) => reader.GetValues(values);

    /// <inheritdoc/>
    public override bool IsDBNull(int ordinal) => reader.IsDBNull(ordinal);

    /// <inheritdoc/>
    public override bool NextResult()
    {
        var result = reader.NextResult();
        hasRows = null;
        hasPrefetchedRow = false;
        return result;
    }

    /// <inheritdoc/>
    public override bool Read()
    {
        if (hasPrefetchedRow)
        {
            hasPrefetchedRow = false;
            return true;
        }

        if (reader is not DbDataReader && hasRows == false)
            return false;

        var result = reader.Read();
        if (result)
            hasRows = true;
        else
            hasRows ??= false;

        return result;
    }

    /// <inheritdoc/>
    public override DataTable? GetSchemaTable() => reader.GetSchemaTable();

    /// <inheritdoc/>
    public override IEnumerator GetEnumerator() =>
        hasPrefetchedRow ? new DbEnumerator(this) :
        (reader as IEnumerable)?.GetEnumerator() ?? new DbEnumerator(this);
}
