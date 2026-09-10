namespace Serenity.Services;

public class UndeleteRequestHandlerTests_Basic
{
    [TableName("UndelRows")]
    private class UndelRow : Row<UndelRow.RowFields>, IIdRow, IIsDeletedRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }
        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }
        public BooleanField IsDeletedField => fields.IsDeleted;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public StringField Name;
            public BooleanField IsDeleted;
#pragma warning restore CS0649
        }
    }

    private static IRequestContext Context() => new NullRequestContext().WithPermissions(_ => true);

    [Fact]
    public void Undelete_RestoresRow()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { ID = 5, Name = "A", IsDeleted = true }))
            .InterceptManipulateRow(_ => 1)
            .InterceptExecuteNonQuery(_ => 1);
        var handler = new UndeleteRequestHandler<UndelRow>(Context());

        var response = handler.Undelete(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 });

        Assert.NotNull(response);
    }

    [Fact]
    public void Undelete_UninitializedProperties_Throw()
    {
        var handler = new UndeleteRequestHandler<UndelRow>(Context());
        Assert.Throws<InvalidOperationException>(() => handler.Row);
        Assert.Throws<InvalidOperationException>(() => handler.Request);
        Assert.Throws<InvalidOperationException>(() => handler.Response);
        Assert.Throws<InvalidOperationException>(() => handler.Connection);
        Assert.Throws<InvalidOperationException>(() => handler.UnitOfWork);
    }
}

