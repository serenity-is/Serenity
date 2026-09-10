namespace Serenity.Services;

public class ModelsTests
{
    [Fact]
    public void SortBy_Constructors_And_Properties()
    {
        var empty = new SortBy();
        Assert.Null(empty.Field);
        Assert.False(empty.Descending);

        var byField = new SortBy("Name") { Descending = true };
        Assert.Equal("Name", byField.Field);
        Assert.True(byField.Descending);

        var byFieldAndDir = new SortBy("Id", true);
        Assert.Equal("Id", byFieldAndDir.Field);
        Assert.True(byFieldAndDir.Descending);
    }

    [Fact]
    public void ServiceRequest_Properties()
    {
        var request = new ServiceRequest
        {
            CustomData = new Dictionary<string, object?> { ["x"] = 1 }
        };
        Assert.Equal(1, request.CustomData["x"]);
    }

    [Fact]
    public void ServiceError_And_ServiceResponse_Properties()
    {
        var error = new ServiceError
        {
            Code = "code",
            Arguments = "args",
            Message = "message",
            Details = "details",
            ErrorId = "errorId"
        };

        Assert.Equal("code", error.Code);
        Assert.Equal("args", error.Arguments);
        Assert.Equal("message", error.Message);
        Assert.Equal("details", error.Details);
        Assert.Equal("errorId", error.ErrorId);

        var response = new ServiceResponse
        {
            Error = error,
            CustomData = new Dictionary<string, object?> { ["y"] = 2 }
        };

        Assert.Same(error, response.Error);
        Assert.Equal(2, response.CustomData["y"]);
    }

    [Fact]
    public void ListRequest_Properties()
    {
        var request = new ListRequest
        {
            Skip = 1,
            Take = 2,
            Sort = [new SortBy("Name")],
            ContainsText = "text",
            ContainsField = "field",
            Criteria = new Criteria("Name") == new Criteria("x"),
            IncludeDeleted = true,
            ExcludeTotalCount = true,
            EqualityFilter = new Dictionary<string, object?> { ["a"] = 1 },
            ColumnSelection = ColumnSelection.Details,
            IncludeColumns = ["A"],
            ExcludeColumns = ["B"],
            DistinctFields = [new SortBy("Id")],
            ExportColumns = ["C"],
            Localize = "en"
        };

        Assert.Equal(1, request.Skip);
        Assert.Equal(2, request.Take);
        Assert.Single(request.Sort);
        Assert.Equal("text", request.ContainsText);
        Assert.Equal("field", request.ContainsField);
        Assert.NotNull(request.Criteria);
        Assert.True(request.IncludeDeleted);
        Assert.True(request.ExcludeTotalCount);
        Assert.Equal(1, request.EqualityFilter["a"]);
        Assert.Equal(ColumnSelection.Details, request.ColumnSelection);
        Assert.Contains("A", request.IncludeColumns);
        Assert.Contains("B", request.ExcludeColumns);
        Assert.Single(request.DistinctFields);
        Assert.Contains("C", request.ExportColumns);
        Assert.Equal("en", request.Localize);
    }

    [Fact]
    public void ListResponse_Properties_And_Interface()
    {
        var entity = new IdNameRow { ID = 1 };
        var response = new ListResponse<IdNameRow>
        {
            TotalCount = 5,
            Skip = 1,
            Take = 2,
            Values = [1, 2]
        };
        response.Entities.Add(entity);

        Assert.Single(response.Entities);
        Assert.Same(entity, response.Entities[0]);
        Assert.Equal(5, response.TotalCount);
        Assert.Equal(1, response.Skip);
        Assert.Equal(2, response.Take);
        Assert.Equal(2, response.Values.Count);

        IListResponse iface = response;
        Assert.Same(response.Entities, iface.Entities);
        Assert.Equal(5, iface.TotalCount);
        Assert.Equal(1, iface.Skip);
        Assert.Equal(2, iface.Take);
    }

    [Fact]
    public void RetrieveRequest_Properties()
    {
        var request = new RetrieveRequest
        {
            EntityId = 5,
            ColumnSelection = RetrieveColumnSelection.KeyOnly,
            IncludeColumns = ["A"],
            ExcludeColumns = ["B"]
        };

        Assert.Equal(5, request.EntityId);
        Assert.Equal(RetrieveColumnSelection.KeyOnly, request.ColumnSelection);
        Assert.Contains("A", request.IncludeColumns);
        Assert.Contains("B", request.ExcludeColumns);
    }

    [Fact]
    public void RetrieveResponse_Properties_And_Interface()
    {
        var entity = new IdNameRow { ID = 1 };
        var response = new RetrieveResponse<IdNameRow> { Entity = entity };

        Assert.Same(entity, response.Entity);

        IRetrieveResponse iface = response;
        Assert.Same(entity, iface.Entity);

        var localizations = new Dictionary<string, IdNameRow> { ["en"] = entity };
        iface.Localizations = localizations;
        Assert.Same(localizations, response.Localizations);
        Assert.Same(localizations, iface.Localizations);
    }

    [Fact]
    public void SaveRequest_Properties_And_Interface()
    {
        var entity = new IdNameRow { ID = 1 };
        var request = new SaveRequest<IdNameRow>
        {
            EntityId = 1,
            Entity = entity
        };

        Assert.Equal(1, request.EntityId);
        Assert.Same(entity, request.Entity);

        ISaveRequest iface = request;
        Assert.Equal(1, iface.EntityId);
        Assert.Same(entity, iface.Entity);

        var localizations = new Dictionary<string, IdNameRow> { ["en"] = entity };
        iface.Localizations = localizations;
        Assert.Same(localizations, request.Localizations);
        Assert.Same(localizations, iface.Localizations);

        iface.Entity = null;
        Assert.Null(request.Entity);
    }

    [Fact]
    public void SaveResponse_Properties()
    {
        var response = new SaveResponse { EntityId = 42 };
        Assert.Equal(42, response.EntityId);
    }

    [Fact]
    public void DeleteRequest_And_Response_Properties()
    {
        var request = new DeleteRequest { EntityId = 1 };
        Assert.Equal(1, request.EntityId);

        var response = new DeleteResponse { WasAlreadyDeleted = true };
        Assert.True(response.WasAlreadyDeleted);
    }

    [Fact]
    public void UndeleteRequest_And_Response_Properties()
    {
        var request = new UndeleteRequest { EntityId = 1 };
        Assert.Equal(1, request.EntityId);

        var response = new UndeleteResponse { WasNotDeleted = true };
        Assert.True(response.WasNotDeleted);
    }

    [Fact]
    public void RetrieveColumnSelection_Has_Expected_Values()
    {
        Assert.Equal(0, (int)RetrieveColumnSelection.Details);
        Assert.Equal(1, (int)RetrieveColumnSelection.KeyOnly);
        Assert.Equal(2, (int)RetrieveColumnSelection.List);
        Assert.Equal(3, (int)RetrieveColumnSelection.None);
        Assert.Equal(4, (int)RetrieveColumnSelection.IdOnly);
        Assert.Equal(5, (int)RetrieveColumnSelection.Lookup);
    }
}
