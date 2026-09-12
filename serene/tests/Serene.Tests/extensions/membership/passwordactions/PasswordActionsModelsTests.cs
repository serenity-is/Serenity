namespace Serenity.Extensions;

public class PasswordActionsModelsTests
{
    [Fact]
    public void ChangePasswordRequest_Properties_Roundtrip()
    {
        var request = new ChangePasswordRequest
        {
            OldPassword = "old",
            NewPassword = "new",
            ConfirmPassword = "new"
        };

        Assert.Equal("old", request.OldPassword);
        Assert.Equal("new", request.NewPassword);
        Assert.Equal("new", request.ConfirmPassword);
    }

    [Fact]
    public void ForgotPasswordRequest_Properties_Roundtrip()
    {
        var request = new ForgotPasswordRequest { Email = "a@b.c" };
        Assert.Equal("a@b.c", request.Email);
    }

    [Fact]
    public void ResetPasswordRequest_Properties_Roundtrip()
    {
        var request = new ResetPasswordRequest
        {
            Token = "t",
            NewPassword = "new",
            ConfirmPassword = "new"
        };

        Assert.Equal("t", request.Token);
        Assert.Equal("new", request.NewPassword);
        Assert.Equal("new", request.ConfirmPassword);
    }

    [Fact]
    public void ResetPasswordResponse_Properties_Roundtrip()
    {
        var response = new ResetPasswordResponse { RedirectHome = true };
        Assert.True(response.RedirectHome);
    }

    [Fact]
    public void SendResetPasswordResponse_Properties_Roundtrip()
    {
        var response = new SendResetPasswordResponse { DemoLink = "link" };
        Assert.Equal("link", response.DemoLink);
    }

    [Fact]
    public void ResetPasswordEmailModel_Properties_Roundtrip()
    {
        var model = new ResetPasswordEmailModel { DisplayName = "n", ResetLink = "l" };
        Assert.Equal("n", model.DisplayName);
        Assert.Equal("l", model.ResetLink);
    }
}
