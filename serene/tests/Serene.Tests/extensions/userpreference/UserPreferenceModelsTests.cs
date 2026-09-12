namespace Serenity.Extensions;

public class UserPreferenceModelsTests
{
    [Fact]
    public void UserPreferenceUpdateRequest_Properties_Roundtrip()
    {
        var request = new UserPreferenceUpdateRequest
        {
            PreferenceType = "t",
            Name = "n",
            Value = "v"
        };

        Assert.Equal("t", request.PreferenceType);
        Assert.Equal("n", request.Name);
        Assert.Equal("v", request.Value);
    }

    [Fact]
    public void UserPreferenceRetrieveRequest_Properties_Roundtrip()
    {
        var request = new UserPreferenceRetrieveRequest
        {
            PreferenceType = "t",
            Name = "n"
        };

        Assert.Equal("t", request.PreferenceType);
        Assert.Equal("n", request.Name);
    }

    [Fact]
    public void UserPreferenceRetrieveResponse_Properties_Roundtrip()
    {
        var response = new UserPreferenceRetrieveResponse { Value = "v" };
        Assert.Equal("v", response.Value);
    }
}
