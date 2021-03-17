using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Serenity.Tests
{
    public class MockDataReader : DbDataReader
    {
        protected bool closed;
        protected int index = -1;
        protected string[] props;
        protected object[][] values;

        public MockDataReader()
        {
            values = new object[0][];
            props = Array.Empty<string>();
        }

        public MockDataReader(IEnumerable<IDictionary<string, object>> items,
            params string[] props)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            this.props = props;
            values = items.Select(item => props.Select(p =>
                item.TryGetValue(p, out object o) ? o : null).ToArray()).ToArray();
        }

        public MockDataReader(params object[] anonymousItems)
        {
            if (anonymousItems == null || !anonymousItems.Any())
                throw new ArgumentNullException(nameof(anonymousItems));

            var sample = anonymousItems.First();
            if (anonymousItems.Any(x => x == null || x.GetType() != sample.GetType()))
                throw new ArgumentOutOfRangeException(nameof(anonymousItems),
                    "All items passed to mock data reader constructor must be of same type!");

            var props = sample.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            this.props = props.Select(x => x.Name).ToArray();
            values = anonymousItems.Select(item => props.Select(p => p.GetValue(item)).ToArray()).ToArray();
        }

        public override object this[int i] => values[index][i] ?? DBNull.Value;

        public override object this[string name] => values[index][Array.IndexOf(props, name)] ?? DBNull.Value;

        public override int Depth => 0;
        public override bool IsClosed => closed;
        public override int RecordsAffected => values.Length;
        public override int FieldCount => props.Length;

        public override bool HasRows => throw new NotImplementedException();

        public override void Close()
        {
            closed = true;
        }

        public override ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public override bool GetBoolean(int i)
        {
            return (bool)this[i];
        }

        public override byte GetByte(int i)
        {
            return (byte)this[i];
        }

        public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int i)
        {
            return (char)this[i];
        }

        public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return (long)this[i];
        }

        public override string GetDataTypeName(int i)
        {
            return "string";
        }

        public override DateTime GetDateTime(int i)
        {
            return (DateTime)this[i];
        }

        public override decimal GetDecimal(int i)
        {
            return (decimal)this[i];
        }

        public override double GetDouble(int i)
        {
            return (double)this[i];
        }

        public override Type GetFieldType(int i)
        {
            return this[i]?.GetType();
        }

        public override float GetFloat(int i)
        {
            return (float)this[i];
        }

        public override Guid GetGuid(int i)
        {
            return (Guid)this[i];
        }

        public override short GetInt16(int i)
        {
            return (short)this[i];
        }

        public override int GetInt32(int i)
        {
            return (int)this[i];
        }

        public override long GetInt64(int i)
        {
            return (long)this[i];
        }

        public override string GetName(int i)
        {
            return props[i];
        }

        public override int GetOrdinal(string name)
        {
            return Array.IndexOf(props, name);
        }

        public override DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public override string GetString(int i)
        {
            return (string)this[i];
        }

        public override object GetValue(int i)
        {
            return this[i];
        }

        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public override bool IsDBNull(int i)
        {
            return this[i] == null || this[i] is DBNull;
        }

        public override bool NextResult()
        {
            return false;
        }

        public override bool Read()
        {
            if (index >= values.Length - 1)
                return false;

            index++;
            return true;
        }

        public override IEnumerator GetEnumerator()
        {
            return values.GetEnumerator();
        }
    }
}