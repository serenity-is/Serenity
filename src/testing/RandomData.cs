namespace Serenity.Testing;

public static class RandomData
{
    public static string UniqueString(IDbConnection connection, StringField field, int? length = null,
        Func<int, string> randomString = null)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(field);

        randomString ??= String;

        const int maxTries = 500;
        int tries = 0;
        while (tries++ < maxTries)
        {
            var code = randomString(length ?? (field.Size <= 0 ? 10 : Math.Min(field.Size, 10)));
            if (!new SqlQuery()
                .Select("1")
                .From(field.Fields)
                .Where(field == code)
                .Exists(connection))
                return code;
        }

        throw new InvalidOperationException($"Couldn't generate a random code of length {length} " +
            $" in max tries of {maxTries} on table '{field.Fields.TableName}', field '{field.Name}'!");
    }


    public static string String(StringField field, int length = 10)
    {
        return String(field.Size <= 0 ? 10 : Math.Min(field.Size, length));
    }

    public static string String(int length = 10)
    {
        if (length < 5)
            return TemporaryFileHelper.RandomFileCode().SafeSubstring(0, length);

        return ("TD-" + TemporaryFileHelper.RandomFileCode()).SafeSubstring(0, length);
    }

    public static string Letters(int length = 10)
    {
        var rnd = new Random();
        var s = "";
        for (var i = 0; i < length; i++)
        {
            s += (char)(rnd.Next('Z' - 'A') + 'A');
        }
        return s;
    }
    public static string Email(int length = 15)
    {
        var random = new Random();

        const string domain = "tstx.com";

        var localPartLength = random.Next(1, Math.Max(1, length - domain.Length - 1));
        var localPart = new string([.. Enumerable.Range(0, localPartLength).Select(_ => (char)random.Next('a', 'z' + 1))]);

        return $"{localPart}@{domain}";
    }

    public static double Double(double max = 1000, int decimals = 2)
    {
        return Math.Round(new Random().NextDouble() * max, decimals);
    }

    public static decimal Decimal(decimal max = 1000, int decimals = 2)
    {
        return (decimal)Math.Round(new Random().NextDouble() * (double)max, decimals);
    }

    public static short Int16(short max = 1000)
    {
        return (short)(new Random().Next(max));
    }

    public static short UniqueInt16(IDbConnection connection, Int16Field field)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(field);

        return (short)((connection.Query<short?>(new SqlQuery()
            .Select(Sql.Max(field))
            .From(field.Fields)).FirstOrDefault() ?? 0) + 1);
    }

    public static int Int32(int max = 1000)
    {
        return new Random().Next(max);
    }

    public static int UniqueInt32(IDbConnection connection, Int32Field field)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(field);

        return (connection.Query<int?>(new SqlQuery()
            .Select(Sql.Max(field))
            .From(field.Fields)).FirstOrDefault() ?? 0) + 1;
    }


    public static decimal UniqueDecimal(IDbConnection connection, DecimalField field)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(field);

        return (connection.Query<int?>(new SqlQuery()
            .Select(Sql.Max(field))
            .From(field.Fields)).FirstOrDefault() ?? 0) + 1;
    }

    public static double UniqueDouble(IDbConnection connection, DoubleField field)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(field);

        return (connection.Query<int?>(new SqlQuery()
            .Select(Sql.Max(field))
            .From(field.Fields)).FirstOrDefault() ?? 0) + 1;
    }

    public static bool Bool(double trueProbability = 0.5)
    {
        if (trueProbability < 0 || trueProbability > 1)
            throw new ArgumentException("True probability must be between 0 and 1.");

        return new Random().NextDouble() < trueProbability;
    }

    public static DateTime Date(DateTime? minDate = null, DateTime? maxDate = null)
    {
        minDate ??= DateTime.Today;
        maxDate ??= minDate.Value.AddYears(1);

        if (minDate >= maxDate)
            throw new ArgumentException("minDate must be less than maxDate");

        var random = new Random();
        var range = maxDate.Value - minDate.Value;
        var randomDays = random.Next((int)range.TotalDays);

        return minDate.Value.AddDays(randomDays);
    }


    public static TForeignRow Foreign<TForeignRow>(IDbConnection connection)
        where TForeignRow : class, IIdRow, new()
    {
        var row = new TForeignRow() { TrackWithChecks = true };
        var list = new SqlQuery().From(row)
                .SelectTableFields()
                .Take(10)
                .List(connection, row);

        if (list.Count == 0)
            throw new ArgumentException($"No records found in foreign row type: {typeof(TForeignRow).FullName}", typeof(TForeignRow).FullName);

        return list[new Random().Next(0, list.Count)];
    }

    public static void SetForeign<TForeignRow>(IDbConnection connection, IRow targetRow, Field foreignId, StringField foreignText = null)
        where TForeignRow : class, IIdRow, new()
    {
        var item = Foreign<TForeignRow>(connection);
        foreignId.AsInvariant(targetRow, item.GetIdField().AsObject(item));
        if (foreignText is null)
        {
            if (string.IsNullOrEmpty(foreignId.TextualField))
                throw new ArgumentException($"Field {foreignId.PropertyName} in {targetRow.GetType().FullName} has no TextuadField set!", nameof(foreignId));

            foreignText = (targetRow.FindFieldByPropertyName(foreignId.TextualField) as StringField 
                ?? targetRow.FindField(foreignId.TextualField) as StringField) ?? throw new ArgumentException($"TextualField {foreignId.TextualField} set for {foreignId.PropertyName} in {targetRow.GetType().FullName} is not found!", nameof(foreignId));
        }

        foreignText[targetRow] = item.NameField.AsObject(item)?.ToString();
        targetRow.ClearAssignment(foreignText);
    }


    public static void SetForeignCode<TForeignRow>(IDbConnection connection, IRow targetRow, StringField foreignId)
        where TForeignRow : class, IIdRow, new()
    {
        var item = Foreign<TForeignRow>(connection);
        foreignId.AsInvariant(targetRow, item.GetIdField().AsObject(item));
    }
}
