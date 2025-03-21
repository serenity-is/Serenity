namespace Serenity.Services;

public partial class UpdatableExtensionBehavior_ForeignThisKey_Test
{
    [Fact]
    public void ForeignKeyField_CanBeUsedAs_ThisKey_WithManualIdAssigned()
    {
        bool saveDetailCalled = false;
        var handlerFactory = new MockHandlerFactory((rowType, intfType) =>
        {
            Assert.Equal(typeof(DetailRow), rowType);
            if (intfType == typeof(IListRequestProcessor))
            {
                return new MockListHandler<DetailRow>(x =>
                {
                    x.Response.Entities = [new() { Id = 1357 }];
                });
            }
            else
            {
                Assert.Equal(typeof(ISaveRequestProcessor), intfType);
                return new MockSaveHandler<DetailRow>(x =>
                {
                    Assert.Equal(SaveRequestType.Update, x.RequestType);
                    Assert.Equal(1357, x.Request?.EntityId);
                    var detail = Assert.IsType<DetailRow>(x.Request.Entity);
                    Assert.Equal(1357, detail.Id);
                    Assert.Equal("TestDetail", detail.Text);
                    saveDetailCalled = true;
                });
            }
        });
        var row = new MainRow()
        {
            DetailId = 1357,
            DetailText = "TestDetail"
        };
        var connection = new MockDbConnection();
        var handler = new MockSaveHandler<MainRow>()
        {
            Connection = connection,
            Row = row
        };
        var behavior = new UpdatableExtensionBehavior(handlerFactory);
        Assert.True(behavior.ActivateFor(row));
        behavior.OnBeforeSave(handler);
        behavior.OnAfterSave(handler);
        Assert.True(saveDetailCalled);
    }

    [Fact]
    public void ForeignKeyField_CanBeUsedAs_ThisKey_WithoutDetailId()
    {
        bool saveDetailCalled = false;
        var handlerFactory = new MockHandlerFactory((rowType, intfType) =>
        {
            Assert.Equal(typeof(DetailRow), rowType);
            Assert.Equal(typeof(ISaveRequestProcessor), intfType);
            return new MockSaveHandler<DetailRow>(x =>
            {
                Assert.Equal(SaveRequestType.Create, x.RequestType);
                Assert.Null(x.Request?.EntityId);
                var detail = Assert.IsType<DetailRow>(x.Request.Entity);
                Assert.Null(detail.Id);
                Assert.Equal("TestDetail", detail.Text);
                saveDetailCalled = true;
                x.Response.EntityId = 1357;
            });
        });
        var row = new MainRow()
        {
            DetailText = "TestDetail"
        };
        var connection = new MockDbConnection();
        var handler = new MockSaveHandler<MainRow>()
        {
            Connection = connection,
            Row = row
        };
        var behavior = new UpdatableExtensionBehavior(handlerFactory);
        Assert.True(behavior.ActivateFor(row));
        behavior.OnBeforeSave(handler);
        behavior.OnAfterSave(handler);
        Assert.True(saveDetailCalled);
        Assert.Equal(1357, row.DetailId);
    }

    [TableName("Mains")]
    [UpdatableExtension("d", typeof(DetailRow), ThisKey = "DetailId")]
    public class MainRow : Row<MainRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [ForeignKey(typeof(DetailRow)), LeftJoin("d")]
        public int? DetailId { get => fields.DetailId[this]; set => fields.DetailId[this] = value; }

        [Expression("d.Text")]
        public string DetailText { get => fields.DetailText[this]; set => fields.DetailText[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public Int32Field DetailId;
            public StringField DetailText;
        }
    }

    [TableName("Details")]
    public class DetailRow : Row<DetailRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [NotNull]
        public string Text { get => fields.Text[this]; set => fields.Text[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Text;
        }
    }
}