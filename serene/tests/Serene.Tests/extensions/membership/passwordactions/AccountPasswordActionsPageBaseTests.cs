using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MimeKit;

namespace Serenity.Extensions;

public class AccountPasswordActionsPageBaseTests
{
    private class CaptureEmailSender : IEmailSender
    {
        public MimeMessage? Message { get; private set; }

        public void Send(MimeMessage message, bool skipQueue = false)
        {
            Message = message;
        }
    }

    private class StubSiteAbsoluteUrl : ISiteAbsoluteUrl
    {
        public string ExternalUrl { get; set; } = "https://example.com";
        public string GetExternalUrl() => ExternalUrl;
        public string GetInternalUrl() => ExternalUrl;
    }

    private class StubPasswordValidator : IUserPasswordValidator
    {
        public PasswordValidationResult Result { get; set; } = PasswordValidationResult.Valid;
        public int Calls { get; private set; }

        public PasswordValidationResult Validate(ref string username, string password)
        {
            Calls++;
            return Result;
        }
    }

    private class PasswordUserDefinition : MockUserDefinition, IHasPassword
    {
        public bool HasPassword { get; set; }
    }

    private class TestPage : AccountPasswordActionsPageBase<MockUserRow>
    {
        public string ModuleFolderExposed => ModuleFolder;
        public string ModulePathExposed(string key) => ModulePath(key);
        public ModulePageModel GetForgotPasswordPageModelExposed() => GetForgotPasswordPageModel();
        public ModulePageModel GetResetPasswordPageModelExposed(string token, MembershipSettings settings) =>
            GetResetPasswordPageModel(token, settings);
        public string GenerateResetPasswordTokenExposed(MockUserRow user) => GenerateResetPasswordToken(user);
    }

    private class FakeView : IView
    {
        public string Path => "/Test/View.cshtml";
        public bool Exists => true;

        public Task RenderAsync(ViewContext context)
        {
            context.Writer.Write("RENDERED");
            return Task.CompletedTask;
        }
    }

    private class FakeViewEngine : IRazorViewEngine
    {
        public ViewEngineResult GetView(string? executingFilePath, string viewPath, bool isMainPage) =>
            ViewEngineResult.Found(viewPath, new FakeView());

        public ViewEngineResult FindView(ActionContext actionContext, string viewName, bool isMainPage) =>
            GetView(null, viewName, isMainPage);

        public RazorPageResult FindPage(ActionContext actionContext, string pageName) => new(pageName, []);

        public RazorPageResult GetPage(string? executingFilePath, string pagePath) => new(pagePath, []);

        public string GetAbsolutePath(string? executingFilePath, string pagePath) => pagePath;
    }

    private class FakeTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object?> LoadTempData(HttpContext context) => new Dictionary<string, object?>();
        public void SaveTempData(HttpContext context, IDictionary<string, object?> values)
        {
        }
    }

    private sealed class Ctx
    {
        public TestPage Page = null!;
        public MockDbConnection Connection = new();
        public IDataProtectionProvider Provider = new EphemeralDataProtectionProvider();
        public CaptureEmailSender EmailSender = new();
        public StubSiteAbsoluteUrl SiteUrl = new();
        public StubPasswordValidator PasswordValidator = new();
        public TestTwoLevelCache Cache = new();
        public MockUserRetrieveService UserRetrieve = new();
        public DefaultHttpContext HttpContext = new();
    }

    private static Ctx CreateContext(IUserDefinition? userDefinition = null, MockUserRow? connectionUser = null,
        bool loggedIn = true)
    {
        var ctx = new Ctx();
        ctx.Connection.InterceptManipulateRow(_ => 1);
        ctx.Connection.InterceptFindRow(_ => new OptionalValue<IRow>(connectionUser!));

        if (userDefinition != null)
            ctx.UserRetrieve.Add(userDefinition);

        if (loggedIn)
        {
            var identity = new GenericIdentity("user", "Test");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "1"));
            ctx.HttpContext.User = new ClaimsPrincipal(identity);
        }
        else
        {
            ctx.HttpContext.User = new ClaimsPrincipal(new GenericIdentity("", "Test"));
        }

        var services = new ServiceCollection();
        services.AddSingleton(ctx.Provider);
        services.AddSingleton<ISqlConnections>(new MockSqlConnections { OnNewByKey = _ => ctx.Connection });
        services.AddSingleton<IRazorViewEngine>(new FakeViewEngine());
        services.AddSingleton<ITempDataProvider>(new FakeTempDataProvider());
        services.AddSingleton<IModelMetadataProvider>(new EmptyModelMetadataProvider());
        services.AddSingleton<IHttpContextAccessor>(new MockHttpContextAccessor { HttpContext = ctx.HttpContext });
        ctx.HttpContext.RequestServices = services.BuildServiceProvider();

        ctx.Page = new TestPage
        {
            ControllerContext = new ControllerContext { HttpContext = ctx.HttpContext },
            TempData = new TempDataDictionary(ctx.HttpContext, new FakeTempDataProvider())
        };
        return ctx;
    }

    private static MockUserRow CreateUser(int id = 1)
    {
        return new MockUserRow
        {
            UserId = id,
            Name = "Name",
            DisplayName = "Display",
            Email = "user@example.com",
            PasswordHash = "hash",
            PasswordSalt = "salt",
            UpdateDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
    }

    [Fact]
    public void ModuleFolder_And_ModulePath_Are_Expected()
    {
        var ctx = CreateContext();
        Assert.Equal("~/Serenity.Extensions/esm/Modules/Membership/PasswordActions/", ctx.Page.ModuleFolderExposed);
        Assert.Equal("~/Serenity.Extensions/esm/Modules/Membership/PasswordActions/XPage.js", ctx.Page.ModulePathExposed("X"));
    }

    [Fact]
    public void GetForgotPasswordPageModel_Returns_Expected_Model()
    {
        var model = CreateContext().Page.GetForgotPasswordPageModelExposed();
        Assert.Contains("ForgotPasswordPage.js", model.Module);
        Assert.Equal("_LayoutNoNavigation", model.Layout);
    }

    [Fact]
    public void GetResetPasswordPageModel_Returns_Options()
    {
        var model = CreateContext().Page.GetResetPasswordPageModelExposed("tok",
            new MembershipSettings { MinPasswordLength = 7 });

        Assert.Contains("ResetPasswordPage.js", model.Module);
        Assert.Equal("_LayoutNoNavigation", model.Layout);
        var options = Assert.IsType<ResetPasswordOptions>(model.Options);
        Assert.Equal("tok", options.token);
        Assert.Equal(7, options.minPasswordLength);
    }

    [Fact]
    public void ChangePassword_Get_Returns_ChangePassword_Page()
    {
        var ctx = CreateContext(new MockUserDefinition("1", "user"));
        var result = ctx.Page.ChangePassword(ctx.UserRetrieve);
        var pageResult = Assert.IsType<ModulePageResult>(result);
        Assert.Contains("ChangePasswordPage.js", pageResult.Model.Module);
    }

    [Fact]
    public void ChangePassword_Get_Redirects_To_SetPassword_When_No_Password()
    {
        var ctx = CreateContext(new PasswordUserDefinition { Id = "1", Username = "user", HasPassword = false });
        var result = ctx.Page.ChangePassword(ctx.UserRetrieve);
        var pageResult = Assert.IsType<ModulePageResult>(result);
        Assert.Contains("SetPasswordPage.js", pageResult.Model.Module);
    }

    [Fact]
    public void SetPassword_Returns_SetPassword_Page()
    {
        var result = CreateContext().Page.SetPassword();
        var pageResult = Assert.IsType<ModulePageResult>(result);
        Assert.Contains("SetPasswordPage.js", pageResult.Model.Module);
    }

    [Fact]
    public void ForgotPassword_Get_Returns_Page()
    {
        var result = CreateContext().Page.ForgotPassword();
        var pageResult = Assert.IsType<ModulePageResult>(result);
        Assert.Contains("ForgotPasswordPage.js", pageResult.Model.Module);
    }

    [Fact]
    public void SendResetPassword_Calls_ForgotPassword()
    {
        var ctx = CreateContext(new PasswordUserDefinition { Id = "1", Username = "user", Email = "a@b.c" },
            CreateUser());
        var result = Assert.IsType<Result<ServiceResponse>>(ctx.Page.SendResetPassword(
            ctx.UserRetrieve, ctx.EmailSender, ctx.SiteUrl, ctx.Cache, NullTextLocalizer.Instance));

        Assert.NotNull(result.Data);
        Assert.Null(result.Data.Error);
        Assert.NotNull(ctx.EmailSender.Message);
    }

    [Fact]
    public void SendResetPassword_Returns_Error_When_User_Not_Found()
    {
        var ctx = CreateContext();
        Assert.Throws<ValidationError>(() => ctx.Page.SendResetPassword(
            ctx.UserRetrieve, ctx.EmailSender, ctx.SiteUrl, ctx.Cache, NullTextLocalizer.Instance));
    }

    [Fact]
    public void ForgotPassword_Sends_Email_For_Existing_User()
    {
        var ctx = CreateContext(connectionUser: CreateUser());
        var result = ctx.Page.ForgotPassword(new ForgotPasswordRequest { Email = "user@example.com" },
            ctx.EmailSender, ctx.SiteUrl, ctx.Cache, NullTextLocalizer.Instance);

        Assert.Null(result.Data.Error);
        var message = ctx.EmailSender.Message.AssertNotNull();
        Assert.Equal("user@example.com", Assert.Single(message.To.Mailboxes).Address);
    }

    [Fact]
    public void ForgotPassword_Returns_Success_When_User_Not_Found()
    {
        var ctx = CreateContext();
        var result = ctx.Page.ForgotPassword(new ForgotPasswordRequest { Email = "none@example.com" },
            ctx.EmailSender, ctx.SiteUrl, ctx.Cache, NullTextLocalizer.Instance);

        Assert.Null(result.Data.Error);
        Assert.Null(ctx.EmailSender.Message);
    }

    [Fact]
    public void ForgotPassword_Returns_Error_For_Null_Or_Empty_Request()
    {
        var ctx = CreateContext();
        var nullResult = ctx.Page.ForgotPassword(null!, ctx.EmailSender, ctx.SiteUrl, ctx.Cache, NullTextLocalizer.Instance);
        Assert.NotNull(nullResult.Data.Error);

        var emptyResult = ctx.Page.ForgotPassword(new ForgotPasswordRequest { Email = "" },
            ctx.EmailSender, ctx.SiteUrl, ctx.Cache, NullTextLocalizer.Instance);
        Assert.NotNull(emptyResult.Data.Error);
    }

    [Fact]
    public void ChangePassword_Post_Updates_Password_When_Valid()
    {
        var ctx = CreateContext(new PasswordUserDefinition { Id = "1", Username = "user", HasPassword = true });
        var result = ctx.Page.ChangePassword(new ChangePasswordRequest
        {
            OldPassword = "old",
            NewPassword = "Abcd1234!",
            ConfirmPassword = "Abcd1234!"
        }, ctx.Cache, ctx.PasswordValidator, new NullPasswordStrengthValidator(), ctx.UserRetrieve,
            Options.Create(new MembershipSettings()), Options.Create(new EnvironmentSettings()),
            NullTextLocalizer.Instance);

        Assert.Null(result.Data.Error);
        Assert.Equal(1, ctx.PasswordValidator.Calls);
    }

    [Fact]
    public void ChangePassword_Post_Returns_Error_When_OldPassword_Invalid()
    {
        var ctx = CreateContext(new PasswordUserDefinition { Id = "1", Username = "user", HasPassword = true });
        ctx.PasswordValidator.Result = PasswordValidationResult.Invalid;

        var result = ctx.Page.ChangePassword(new ChangePasswordRequest
        {
            OldPassword = "old",
            NewPassword = "Abcd1234!",
            ConfirmPassword = "Abcd1234!"
        }, ctx.Cache, ctx.PasswordValidator, new NullPasswordStrengthValidator(), ctx.UserRetrieve,
            Options.Create(new MembershipSettings()), Options.Create(new EnvironmentSettings()),
            NullTextLocalizer.Instance);

        Assert.NotNull(result.Data.Error);
    }

    [Fact]
    public void ChangePassword_Post_Returns_Error_When_Confirmation_Mismatch()
    {
        var ctx = CreateContext(new PasswordUserDefinition { Id = "1", Username = "user", HasPassword = true });
        var result = ctx.Page.ChangePassword(new ChangePasswordRequest
        {
            OldPassword = "old",
            NewPassword = "Abcd1234!",
            ConfirmPassword = "Other1234!"
        }, ctx.Cache, ctx.PasswordValidator, new NullPasswordStrengthValidator(), ctx.UserRetrieve,
            Options.Create(new MembershipSettings()), Options.Create(new EnvironmentSettings()),
            NullTextLocalizer.Instance);

        Assert.NotNull(result.Data.Error);
    }

    [Fact]
    public void ChangePassword_Post_Skips_OldPassword_For_Users_Without_Password()
    {
        var ctx = CreateContext(new PasswordUserDefinition { Id = "1", Username = "user", HasPassword = false });
        var result = ctx.Page.ChangePassword(new ChangePasswordRequest
        {
            OldPassword = "old",
            NewPassword = "Abcd1234!",
            ConfirmPassword = "Abcd1234!"
        }, ctx.Cache, ctx.PasswordValidator, new NullPasswordStrengthValidator(), ctx.UserRetrieve,
            Options.Create(new MembershipSettings()), Options.Create(new EnvironmentSettings()),
            NullTextLocalizer.Instance);

        Assert.Null(result.Data.Error);
        Assert.Equal(0, ctx.PasswordValidator.Calls);
    }

    [Fact]
    public void ChangePassword_Post_Returns_Error_For_Null_Request()
    {
        var ctx = CreateContext();
        var result = ctx.Page.ChangePassword(null!, ctx.Cache, ctx.PasswordValidator,
            new NullPasswordStrengthValidator(), ctx.UserRetrieve, Options.Create(new MembershipSettings()),
            Options.Create(new EnvironmentSettings()), NullTextLocalizer.Instance);

        Assert.NotNull(result.Data.Error);
    }

    [Fact]
    public void ResetPassword_Get_Returns_Page_For_Valid_Token()
    {
        var user = CreateUser();
        var ctx = CreateContext(connectionUser: user);
        var token = ctx.Page.GenerateResetPasswordTokenExposed(user);

        var result = ctx.Page.ResetPassword(token, new MockSqlConnections { OnNewByKey = _ => ctx.Connection },
            NullTextLocalizer.Instance, Options.Create(new MembershipSettings()));

        var pageResult = Assert.IsType<ModulePageResult>(result);
        Assert.Contains("ResetPasswordPage.js", pageResult.Model.Module);
    }

    [Fact]
    public void ResetPassword_Get_Returns_Error_For_Invalid_Token()
    {
        var ctx = CreateContext();
        var result = ctx.Page.ResetPassword("invalidtoken",
            new MockSqlConnections { OnNewByKey = _ => ctx.Connection },
            NullTextLocalizer.Instance, Options.Create(new MembershipSettings()));

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<ValidationError>(viewResult.ViewData.Model);
    }

    [Fact]
    public void ResetPassword_Get_Returns_Error_For_Expired_Token()
    {
        var ctx = CreateContext();
        var token = ctx.Provider.CreateProtector("ResetPassword").ProtectBinary(bw =>
        {
            bw.Write(DateTime.UtcNow.AddHours(-1).ToBinary());
            bw.Write("1");
            bw.Write(0);
        });

        var result = ctx.Page.ResetPassword(token,
            new MockSqlConnections { OnNewByKey = _ => ctx.Connection },
            NullTextLocalizer.Instance, Options.Create(new MembershipSettings()));

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<ValidationError>(viewResult.ViewData.Model);
    }

    [Fact]
    public void ResetPassword_Get_Returns_Error_When_User_Not_Found()
    {
        var ctx = CreateContext();
        var user = CreateUser();
        var token = ctx.Page.GenerateResetPasswordTokenExposed(user);

        var result = ctx.Page.ResetPassword(token,
            new MockSqlConnections { OnNewByKey = _ => ctx.Connection },
            NullTextLocalizer.Instance, Options.Create(new MembershipSettings()));

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<ValidationError>(viewResult.ViewData.Model);
    }

    [Fact]
    public void ResetPassword_Get_Returns_Error_When_Nonce_Mismatch()
    {
        var user = CreateUser();
        var ctx = CreateContext(connectionUser: user);
        var token = ctx.Provider.CreateProtector("ResetPassword").ProtectBinary(bw =>
        {
            bw.Write(DateTime.UtcNow.AddHours(1).ToBinary());
            bw.Write("1");
            bw.Write(12345);
        });

        var result = ctx.Page.ResetPassword(token,
            new MockSqlConnections { OnNewByKey = _ => ctx.Connection },
            NullTextLocalizer.Instance, Options.Create(new MembershipSettings()));

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<ValidationError>(viewResult.ViewData.Model);
    }

    [Fact]
    public void ResetPassword_Post_Resets_Password_For_Valid_Token()
    {
        var user = CreateUser();
        var ctx = CreateContext(connectionUser: user);
        var token = ctx.Page.GenerateResetPasswordTokenExposed(user);

        var result = ctx.Page.ResetPassword(new ResetPasswordRequest
        {
            Token = token,
            NewPassword = "Abcd1234!",
            ConfirmPassword = "Abcd1234!"
        }, ctx.Cache, new MockSqlConnections { OnNewByKey = _ => ctx.Connection }, NullTextLocalizer.Instance,
            new NullPasswordStrengthValidator(), Options.Create(new EnvironmentSettings()),
            Options.Create(new MembershipSettings()));

        Assert.Null(result.Data.Error);
        Assert.True(result.Data.RedirectHome);
    }

    [Fact]
    public void ResetPassword_Post_Returns_Error_For_Invalid_Token()
    {
        var ctx = CreateContext();
        var result = ctx.Page.ResetPassword(new ResetPasswordRequest
        {
            Token = "invalidtoken",
            NewPassword = "Abcd1234!",
            ConfirmPassword = "Abcd1234!"
        }, ctx.Cache, new MockSqlConnections { OnNewByKey = _ => ctx.Connection }, NullTextLocalizer.Instance,
            new NullPasswordStrengthValidator(), Options.Create(new EnvironmentSettings()),
            Options.Create(new MembershipSettings()));

        Assert.NotNull(result.Data.Error);
    }

    [Fact]
    public void ResetPassword_Post_Returns_Error_For_Expired_Token()
    {
        var ctx = CreateContext(connectionUser: CreateUser());
        var token = ctx.Provider.CreateProtector("ResetPassword").ProtectBinary(bw =>
        {
            bw.Write(DateTime.UtcNow.AddHours(-1).ToBinary());
            bw.Write("1");
            bw.Write(0);
        });

        var result = ctx.Page.ResetPassword(new ResetPasswordRequest
        {
            Token = token,
            NewPassword = "Abcd1234!",
            ConfirmPassword = "Abcd1234!"
        }, ctx.Cache, new MockSqlConnections { OnNewByKey = _ => ctx.Connection }, NullTextLocalizer.Instance,
            new NullPasswordStrengthValidator(), Options.Create(new EnvironmentSettings()),
            Options.Create(new MembershipSettings()));

        Assert.NotNull(result.Data.Error);
    }

    [Fact]
    public void ResetPassword_Post_Returns_Error_When_Confirmation_Mismatch()
    {
        var user = CreateUser();
        var ctx = CreateContext(connectionUser: user);
        var token = ctx.Page.GenerateResetPasswordTokenExposed(user);

        var result = ctx.Page.ResetPassword(new ResetPasswordRequest
        {
            Token = token,
            NewPassword = "Abcd1234!",
            ConfirmPassword = "Other1234!"
        }, ctx.Cache, new MockSqlConnections { OnNewByKey = _ => ctx.Connection }, NullTextLocalizer.Instance,
            new NullPasswordStrengthValidator(), Options.Create(new EnvironmentSettings()),
            Options.Create(new MembershipSettings()));

        Assert.NotNull(result.Data.Error);
    }

    [Fact]
    public void ResetPassword_Post_Returns_Error_For_Null_Request()
    {
        var ctx = CreateContext();
        var result = ctx.Page.ResetPassword(null!, ctx.Cache,
            new MockSqlConnections { OnNewByKey = _ => ctx.Connection }, NullTextLocalizer.Instance,
            new NullPasswordStrengthValidator(), Options.Create(new EnvironmentSettings()),
            Options.Create(new MembershipSettings()));

        Assert.NotNull(result.Data.Error);
    }

    [Fact]
    public void ResetPassword_Post_Returns_Error_When_User_Not_Found()
    {
        var ctx = CreateContext();
        var user = CreateUser();
        var token = ctx.Page.GenerateResetPasswordTokenExposed(user);

        var result = ctx.Page.ResetPassword(new ResetPasswordRequest
        {
            Token = token,
            NewPassword = "Abcd1234!",
            ConfirmPassword = "Abcd1234!"
        }, ctx.Cache, new MockSqlConnections { OnNewByKey = _ => ctx.Connection }, NullTextLocalizer.Instance,
            new NullPasswordStrengthValidator(), Options.Create(new EnvironmentSettings()),
            Options.Create(new MembershipSettings()));

        Assert.NotNull(result.Data.Error);
    }
}
