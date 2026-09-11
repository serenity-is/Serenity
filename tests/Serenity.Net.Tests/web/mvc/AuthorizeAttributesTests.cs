namespace Serenity.Services;

public class AuthorizeAttributesTests
{
    [ReadPermission("Test:Read")]
    [InsertPermission("Test:Insert")]
    [UpdatePermission("Test:Update")]
    [DeletePermission("Test:Delete")]
    [ServiceLookupPermission("Test:Lookup")]
    private class RowWithPermissions
    {
    }

    [ReadPermission("Test:Read")]
    [ModifyPermission("Test:Modify")]
    private class RowWithModifyAndRead
    {
    }

    [ModifyPermission("Test:Modify")]
    private class RowWithModifyOnly
    {
    }

    [ReadPermission("Test:Read")]
    private class RowWithReadOnly
    {
    }

    private class TestAuthorizeListAttribute(Type sourceType) : AuthorizeListAttribute(sourceType)
    {
        public string? Or => OrPermission;
    }

    [Fact]
    public void AuthorizeCreate_Uses_InsertPermission()
    {
        Assert.Equal("Test:Insert", new AuthorizeCreateAttribute(typeof(RowWithPermissions)).Permission);
    }

    [Fact]
    public void AuthorizeCreate_Falls_Back_To_ModifyPermission()
    {
        Assert.Equal("Test:Modify", new AuthorizeCreateAttribute(typeof(RowWithModifyAndRead)).Permission);
        Assert.Equal("Test:Modify", new AuthorizeCreateAttribute(typeof(RowWithModifyOnly)).Permission);
    }

    [Fact]
    public void AuthorizeCreate_Falls_Back_To_ReadPermission()
    {
        Assert.Equal("Test:Read", new AuthorizeCreateAttribute(typeof(RowWithReadOnly)).Permission);
    }

    [Fact]
    public void AuthorizeDelete_Uses_DeletePermission()
    {
        Assert.Equal("Test:Delete", new AuthorizeDeleteAttribute(typeof(RowWithPermissions)).Permission);
    }

    [Fact]
    public void AuthorizeDelete_Falls_Back_To_ModifyPermission()
    {
        Assert.Equal("Test:Modify", new AuthorizeDeleteAttribute(typeof(RowWithModifyAndRead)).Permission);
        Assert.Equal("Test:Modify", new AuthorizeDeleteAttribute(typeof(RowWithModifyOnly)).Permission);
    }

    [Fact]
    public void AuthorizeDelete_Falls_Back_To_ReadPermission()
    {
        Assert.Equal("Test:Read", new AuthorizeDeleteAttribute(typeof(RowWithReadOnly)).Permission);
    }

    [Fact]
    public void AuthorizeUpdate_Uses_UpdatePermission()
    {
        Assert.Equal("Test:Update", new AuthorizeUpdateAttribute(typeof(RowWithPermissions)).Permission);
    }

    [Fact]
    public void AuthorizeUpdate_Falls_Back_To_ModifyPermission()
    {
        Assert.Equal("Test:Modify", new AuthorizeUpdateAttribute(typeof(RowWithModifyAndRead)).Permission);
        Assert.Equal("Test:Modify", new AuthorizeUpdateAttribute(typeof(RowWithModifyOnly)).Permission);
    }

    [Fact]
    public void AuthorizeUpdate_Falls_Back_To_ReadPermission()
    {
        Assert.Equal("Test:Read", new AuthorizeUpdateAttribute(typeof(RowWithReadOnly)).Permission);
    }

    [Fact]
    public void AuthorizeRetrieve_Uses_ReadPermission()
    {
        Assert.Equal("Test:Read", new AuthorizeRetrieveAttribute(typeof(RowWithPermissions)).Permission);
        Assert.Equal("Test:Read", new AuthorizeRetrieveAttribute(typeof(RowWithModifyAndRead)).Permission);
    }

    [Fact]
    public void AuthorizeRetrieve_Throws_When_No_ReadPermission()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new AuthorizeRetrieveAttribute(typeof(RowWithModifyOnly)));
    }

    [Fact]
    public void AuthorizeList_Uses_ReadPermission_And_Lookup_OrPermission()
    {
        var attribute = new TestAuthorizeListAttribute(typeof(RowWithPermissions));

        Assert.Equal("Test:Read", attribute.Permission);
        Assert.Equal("Test:Lookup", attribute.Or);
    }

    [Fact]
    public void AuthorizeList_OrPermission_Is_Null_Without_Lookup_Permission()
    {
        var attribute = new TestAuthorizeListAttribute(typeof(RowWithReadOnly));

        Assert.Null(attribute.Or);
    }
}
