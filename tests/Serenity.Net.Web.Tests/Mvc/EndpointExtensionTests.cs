namespace Serenity.Tests.Web.Mvc;

public class EndpointExtensionTests
{
    [Fact]
    public void ConvertToResponse_ShowsDetails_WhenShowDetailsIsTrue()
    {
        var mockExceptionLogger = new MockExceptionLogger();
        var exception = new ValidationError("Not Sensitive Error");

        var response = exception.ConvertToResponse<ServiceResponse>(mockExceptionLogger, NullTextLocalizer.Instance, showDetails: true);

        Assert.Equal("Not Sensitive Error", response.Error.Message);
    }

    [Fact]
    public void ConvertToResponse_HidesDetails_WhenShowDetailsIsFalse_AndErrorIsSensitive()
    {
        var mockExceptionLogger = new MockExceptionLogger();
        var exception = new ValidationError("Sensitive Error")
        {
            IsSensitiveMessage = true
        };

        var response = exception.ConvertToResponse<ServiceResponse>(mockExceptionLogger, new MockTextLocalizer(), showDetails: false);
        Assert.Equal("Services.GenericErrorMessage", response.Error.Message);
    }

    [Fact]
    public void ConvertToResponse_ShowsDetails_WhenShowDetailsIsFalse_AndErrorIsNotSensitive()
    {
        var mockExceptionLogger = new MockExceptionLogger();
        var exception = new ValidationError("Not Sensitive Error")
        {
            IsSensitiveMessage = false
        };

        var response = exception.ConvertToResponse<ServiceResponse>(mockExceptionLogger, NullTextLocalizer.Instance, showDetails: false);
        Assert.Equal("Not Sensitive Error", response.Error.Message);
    }

    [Fact]
    public void ConvertToResponse_HidesDetails_AndUsesConstantErrorMessage_WhenShowDetailsIsFalse_AndErrorIsSensitive_AndLocalizerIsNull()
    {
        var mockExceptionLogger = new MockExceptionLogger();
        var exception = new ValidationError("Sensitive Error")
        {
            IsSensitiveMessage = true
        };

        var response = exception.ConvertToResponse<ServiceResponse>(mockExceptionLogger, null, showDetails: false);
        Assert.Equal("An error occurred while processing your request.", response.Error.Message);
    }
}