using System.Collections;
using System.Data.Common;
using System.Threading.Tasks;
using TSQL.Statements;
using TSQL.Tokens;

namespace Serenity.Tests;

public class MockDbDataReader : DbDataReader
{
    protected bool closed;
    protected int index = -1;
    protected string[] props;
    protected object[][] values;

    public MockDbDataReader()
    {
        values = Array.Empty<object[]>();
        props = Array.Empty<string>();
    }

    public MockDbDataReader(IEnumerable<IDictionary<string, object>> items,
        params string[] props)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        this.props = props;
        values = items.Select(item => props.Select(p =>
                item.TryGetValue(p, out var o) ? o : null).ToArray()).ToArray();
    }

    public MockDbDataReader(params object[] anonymousItems) : this(null, anonymousItems)
    {
    }

    public MockDbDataReader(string commandText, params object[] anonymousItems)
    {
        if (anonymousItems == null || !anonymousItems.Any())
            throw new ArgumentNullException(nameof(anonymousItems));

        var sample = anonymousItems.First();
        if (anonymousItems.Any(x => x == null || x.GetType() != sample.GetType()))
            throw new ArgumentOutOfRangeException(nameof(anonymousItems),
                "All items passed to mock data reader constructor must be of same type!");

        if (!string.IsNullOrEmpty(commandText))
            props = ParseSelectFieldAliases(commandText)?.ToArray();

        if (props == null)
        {
            var properties = sample.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            props = properties.Select(x => x.Name).ToArray();
            values = anonymousItems.Select(item => properties.Select(p => p.GetValue(item)).ToArray()).ToArray();
        }
        else
        {
            var properties = sample.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

            var matchingProps = props.Select(x => properties.TryGetValue(x, out var p) ? p : null);

            values = anonymousItems.Select(item => matchingProps.Select(x => x?.GetValue(item)).ToArray()).ToArray();
        }
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
        GC.SuppressFinalize(this);
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
        return values?.FirstOrDefault()?[i]?.GetType() ?? typeof(object);
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

    public static IEnumerable<string> ParseSelectFieldAliases(string commandText)
    {
        var result = new List<string>();

        if (string.IsNullOrEmpty(commandText))
            return result;

        var statements = TSQL.TSQLStatementReader.ParseStatements(commandText);
        if (!statements.Any())
            return result;

        if (statements[0] is not TSQLSelectStatement statement ||
            statement.Select == null)
            return result;

        var tokens = statement.Select.Tokens;
        var index = 0;
        for (; index < tokens.Count; index++)
        {
            var token = tokens[index];

            if (token.Type != TSQLTokenType.Keyword)
                break;
        }

        var inParens = 0;
        var inQuote = false;
        var start = index;
        for (; index < tokens.Count; index++)
        {
            var token = tokens[index];
            var text = token.Text;

            if (token.Type == TSQLTokenType.Character)
            {
                if (text == "(")
                    inParens++;
                else if (text == ")")
                    inParens--;
                else if (text == "'")
                    inQuote = !inQuote;
                continue;
            }

            if (inParens > 0 || inQuote)
                continue;

            var more = index < tokens.Count - 1;
            var first = index == start;

            if (token.Type == TSQLTokenType.Keyword &&
                string.Equals(token.Text, "AS", StringComparison.OrdinalIgnoreCase) &&
                !first &&
                more &&
                tokens[index + 1].Type == TSQLTokenType.Identifier)
            {
                result.Add(SqlSyntax.Unquote(tokens[index + 1].Text));
                index++;
                continue;
            }

            if (token.Type == TSQLTokenType.Identifier)
            {
                if (more &&
                    tokens[index + 1].Type == TSQLTokenType.Keyword &&
                    string.Equals(tokens[index + 1].Text, "AS", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (more &&
                    (tokens[index + 1].Type != TSQLTokenType.Character ||
                     tokens[index + 1].Text != ","))
                    continue;

                if (!first &&
                    tokens[index - 1].Type == TSQLTokenType.Operator)
                    continue;

                result.Add(SqlSyntax.Unquote(text));
            }
        }

        return result;
    }

}