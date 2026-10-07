namespace Serenity.Services;

public class SensitiveFieldHandlerTests
{
    private const string ReadPermission = "Test:SensitiveRead";

    [ReadPermission(ReadPermission)]
    private class PasswordRow : Row<PasswordRow.RowFields>, IIdRow, IPasswordRow
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public string? PwdHash { get => fields.PwdHash[this]; set => fields.PwdHash[this] = value; }
        public string? PwdSalt { get => fields.PwdSalt[this]; set => fields.PwdSalt[this] = value; }
        public string? Normal { get => fields.Normal[this]; set => fields.Normal[this] = value; }

        StringField IPasswordRow.PasswordHashField => fields.PwdHash;
        StringField IPasswordRow.PasswordSaltField => fields.PwdSalt;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public StringField PwdHash = null!;
            public StringField PwdSalt = null!;
            public StringField Normal = null!;
        }
    }

    [ReadPermission(ReadPermission)]
    private class AnnotatedPasswordRow : Row<AnnotatedPasswordRow.RowFields>, IIdRow, IPasswordRow
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [MinSelectLevel(SelectLevel.List)]
        public string? PwdHash { get => fields.PwdHash[this]; set => fields.PwdHash[this] = value; }

        public string? PwdSalt { get => fields.PwdSalt[this]; set => fields.PwdSalt[this] = value; }

        StringField IPasswordRow.PasswordHashField => fields.PwdHash;
        StringField IPasswordRow.PasswordSaltField => fields.PwdSalt;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public StringField PwdHash = null!;
            public StringField PwdSalt = null!;
        }
    }

    private class TestListHandler<TRow>(IRequestContext context) : ListRequestHandler<TRow>(context)
        where TRow : class, IRow, new()
    {
        public bool Allow(Field field)
        {
            Row = new TRow();
            return AllowSelectField(field);
        }
    }

    private class TestRetrieveHandler<TRow>(IRequestContext context) : RetrieveRequestHandler<TRow>(context)
        where TRow : class, IRow, new()
    {
        public bool Allow(Field field)
        {
            Row = new TRow();
            return AllowSelectField(field);
        }
    }

    private class TestSaveHandler<TRow>(IRequestContext context) : SaveRequestHandler<TRow>(context)
        where TRow : class, IRow, IIdRow, new()
    {
        public HashSet<Field> Editable()
        {
            Row = new TRow();
            var editable = new HashSet<Field>();
            GetEditableFields(editable);
            return editable;
        }
    }

    private static NullRequestContext Context(Func<string, bool>? hasPermission = null) =>
        new NullRequestContext().WithPermissions(hasPermission ?? (_ => true));

    [Fact]
    public void List_AllowSelectField_Denies_IPasswordRow_Fields()
    {
        var handler = new TestListHandler<PasswordRow>(Context());
        var f = PasswordRow.Fields;

        Assert.False(handler.Allow(f.PwdHash));
        Assert.False(handler.Allow(f.PwdSalt));
        Assert.True(handler.Allow(f.Normal));
        Assert.True(handler.Allow(f.Id));
    }

    [Fact]
    public void Retrieve_AllowSelectField_Denies_IPasswordRow_Fields()
    {
        var handler = new TestRetrieveHandler<PasswordRow>(Context());
        var f = PasswordRow.Fields;

        Assert.False(handler.Allow(f.PwdHash));
        Assert.False(handler.Allow(f.PwdSalt));
        Assert.True(handler.Allow(f.Normal));
    }

    [Fact]
    public void List_AllowSelectField_Allows_Annotated_IPasswordRow_Field()
    {
        var handler = new TestListHandler<AnnotatedPasswordRow>(Context());
        var f = AnnotatedPasswordRow.Fields;

        Assert.True(handler.Allow(f.PwdHash));
        Assert.False(handler.Allow(f.PwdSalt));
    }

    [Fact]
    public void IsSensitiveFieldBasedOnInterfaces_Detects_IPasswordRow_Fields()
    {
        var row = new PasswordRow();
        var f = PasswordRow.Fields;

        Assert.True(EntityFieldExtensions.IsSensitiveFieldBasedOnInterfaces(f.PwdHash, row));
        Assert.True(EntityFieldExtensions.IsSensitiveFieldBasedOnInterfaces(f.PwdSalt, row));
        Assert.False(EntityFieldExtensions.IsSensitiveFieldBasedOnInterfaces(f.Normal, row));
        Assert.False(EntityFieldExtensions.IsSensitiveFieldBasedOnInterfaces(f.PwdHash, null));
    }

    [Fact]
    public void IsFieldFilterAllowed_Denies_IPasswordRow_Fields_When_Row_Passed()
    {
        var permissions = Context().Permissions;
        var row = new PasswordRow();
        var f = PasswordRow.Fields;

        Assert.False(CriteriaFieldExpressionReplacer.IsFieldFilterAllowed(f.PwdHash, permissions, row: row));
        Assert.False(CriteriaFieldExpressionReplacer.IsFieldFilterAllowed(f.PwdSalt, permissions, row: row));
        Assert.True(CriteriaFieldExpressionReplacer.IsFieldFilterAllowed(f.Normal, permissions, row: row));

        // without a row instance, only the name based / flag checks apply
        Assert.True(CriteriaFieldExpressionReplacer.IsFieldFilterAllowed(f.PwdHash, permissions));
    }

    [Fact]
    public void Save_GetEditableFields_Excludes_IPasswordRow_Fields()
    {
        var handler = new TestSaveHandler<PasswordRow>(Context());
        var editable = handler.Editable();
        var f = PasswordRow.Fields;

        Assert.DoesNotContain(f.PwdHash, editable);
        Assert.DoesNotContain(f.PwdSalt, editable);
        Assert.Contains(f.Normal, editable);
    }

    [Fact]
    public void List_Does_Not_Select_IPasswordRow_Fields()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                var columns = ((ISqlQueryExtensible)args.Query.AssertNotNull())
                    .Columns.Select(x => x.ColumnName).ToList();
                Assert.DoesNotContain("PwdHash", columns);
                Assert.DoesNotContain("PwdSalt", columns);
                Assert.Contains("Normal", columns);
                return new MockDbDataReader();
            });

        new ListRequestHandler<PasswordRow>(Context()).List(connection, new());
    }

    [Fact]
    public void Retrieve_Details_Does_Not_Select_IPasswordRow_Fields()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                var columns = ((ISqlQueryExtensible)args.Query.AssertNotNull())
                    .Columns.Select(x => x.ColumnName).ToList();
                Assert.DoesNotContain("PwdHash", columns);
                Assert.DoesNotContain("PwdSalt", columns);
                return args.ToMockReader(new { Id = 1, Normal = "A" });
            });

        new RetrieveRequestHandler<PasswordRow>(Context()).Retrieve(connection,
            new RetrieveRequest { EntityId = 1, ColumnSelection = RetrieveColumnSelection.Details });
    }
}
