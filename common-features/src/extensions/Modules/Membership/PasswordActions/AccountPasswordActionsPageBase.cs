using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace Serenity.Extensions;

public abstract class AccountPasswordActionsPageBase<TUserRow> : MembershipPageBase<TUserRow>
    where TUserRow : class, IRow, IIdRow, IEmailRow, IPasswordRow, new()
{
    protected virtual string ModuleFolder => "~/Serenity.Extensions/esm/Modules/Membership/PasswordActions/";
    protected virtual string ModulePath(string key) => ModuleFolder + key + "Page.js";

    [HttpGet, PageAuthorize]
    public virtual ActionResult ChangePassword(
        [FromServices] IUserRetrieveService userRetrieveService)
    {
        var userDefinition = User.GetUserDefinition<IUserDefinition>(userRetrieveService);
        if (userDefinition is IHasPassword hasPassword &&
            !hasPassword.HasPassword)
        {
            return SetPassword();
        }

        return this.PanelPage(ModulePath(nameof(ChangePassword)),
            ExtensionsTexts.Forms.Membership.ChangePassword.FormTitle);
    }

    [HttpGet, PageAuthorize]
    public ActionResult SetPassword()
    {
        return this.PanelPage(new()
        {
            Module = ModulePath("SetPassword"),
            PageTitle = SetPasswordFormTexts.PageTitle
        });
    }

    [HttpPost, ServiceAuthorize]
    public virtual ActionResult SendResetPassword(
        [FromServices] IUserRetrieveService userRetrieveService,
        [FromServices] IEmailSender emailSender,
        [FromServices] ISiteAbsoluteUrl siteAbsoluteUrl,
        [FromServices] ITwoLevelCache cache,
        [FromServices] ITextLocalizer localizer)
    {
        var userDefinition = User.GetUserDefinition<IUserDefinition>(userRetrieveService) ??
            throw new ValidationError("Couldn't find user definition.");

#if (IsPublicDemo)
        return this.UseConnection(GetConnectionKey(), connection =>
        {
            var user = connection.TryFirst<TUserRow>(new TUserRow().Fields.IdField == Convert.ToInt32(userDefinition.Id));
            if (user is null)
                throw new ValidationError("Couldn't find user.");

            return new SendResetPasswordResponse()
            {
                DemoLink = "/Account/ResetPassword?t=" + Uri.EscapeDataString(GenerateResetPasswordToken(user))
            };
        });
#else
        return ForgotPassword(new()
        {
            Email = userDefinition.Email
        }, emailSender, siteAbsoluteUrl, cache, localizer);
#endif
    }

    [HttpPost, JsonRequest, ServiceAuthorize]
    public virtual Result<ServiceResponse> ChangePassword(ChangePasswordRequest request,
        [FromServices] ITwoLevelCache cache,
        [FromServices] IUserPasswordValidator passwordValidator,
        [FromServices] IPasswordStrengthValidator passwordStrengthValidator,
        [FromServices] IUserRetrieveService userRetrieveService,
        [FromServices] IOptions<MembershipSettings> membershipOptions,
        [FromServices] IOptions<EnvironmentSettings> environmentOptions,
        [FromServices] ITextLocalizer localizer)
    {
        return this.InTransaction("Default", uow =>
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrEmpty(request.OldPassword);
            ArgumentNullException.ThrowIfNull(passwordValidator);

            var username = User.Identity?.Name;

            var userDefinition = User.GetUserDefinition<IUserDefinition>(userRetrieveService);

            if (userDefinition is not IHasPassword hasPassword ||
                hasPassword.HasPassword)
            {
                if (passwordValidator.Validate(ref username, request.OldPassword) != PasswordValidationResult.Valid)
                    throw new ValidationError("CurrentPasswordMismatch", localizer.Get("Validation.CurrentPasswordMismatch"));

                if (request.ConfirmPassword != request.NewPassword)
                    throw new ValidationError("PasswordConfirmMismatch", localizer.Get("Validation.PasswordConfirm"));
            }

            passwordStrengthValidator.Validate(request.NewPassword);
            request.NewPassword ??= "";

            var salt = GenerateSalt(membershipOptions.Value);
            var hash = CalculateHash(request.NewPassword, salt);
            var userId = User.GetIdentifier();
#if (IsPublicDemo)
            if (userId?.ToString() == "1")
                throw new ValidationError("Sorry, but no changes are allowed in public demo on ADMIN user!");
#endif

            var row = new TUserRow();
            row.IdField.AsObject(row, row.IdField.ConvertValue(userId, CultureInfo.InvariantCulture));
            if (row is IUpdateLogRow updateLogRow)
                updateLogRow.UpdateDateField[row] = DateTime.UtcNow;
            row.PasswordHashField[row] = hash;
            row.PasswordSaltField[row] = salt;
            uow.Connection.UpdateById(row);

            cache.InvalidateOnCommit(uow, row.Fields);

            return new ServiceResponse();
        });
    }

    [HttpGet]
    public virtual ActionResult ForgotPassword()
    {
        return this.PanelPage(GetForgotPasswordPageModel());
    }

    protected virtual ModulePageModel GetForgotPasswordPageModel()
    {
        return new ModulePageModel()
        {
            Module = ModulePath(nameof(ForgotPassword)),
            PageTitle = ForgotPasswordFormTexts.FormTitle,
            Layout = "_LayoutNoNavigation"
        };
    }

    [HttpPost, JsonRequest]
    public virtual Result<ServiceResponse> ForgotPassword(ForgotPasswordRequest request,
        [FromServices] IEmailSender emailSender,
        [FromServices] ISiteAbsoluteUrl siteAbsoluteUrl,
        [FromServices] ITwoLevelCache cache,
        [FromServices] ITextLocalizer localizer)
    {
        return this.InTransaction(GetConnectionKey(), uow =>
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrEmpty(request.Email);

            var fieldsRow = new TUserRow();

            var user = uow.Connection.TryFirst<TUserRow>(fieldsRow.EmailField == request.Email);
            if (user == null)
                return new ServiceResponse();

            var updateRow = new TUserRow();
            if (updateRow is IUpdateLogRow updateLog && updateLog.UpdateDateField is not null)
            {
                // set update date to make sure only the latest reset token can be used
                updateRow.IdField.AsObject(updateRow, user.IdField.AsObject(user));
                updateLog.UpdateDateField[user] = DateTime.UtcNow;
                updateLog.UpdateDateField.AsObject(updateRow, updateLog.UpdateDateField.AsObject(user));
                uow.Connection.UpdateById(updateRow);
                cache.InvalidateOnCommit(uow, user.Fields);
            }

            var token = GenerateResetPasswordToken(user);
            var externalUrl = siteAbsoluteUrl.GetExternalUrl();
            var resetLink = UriHelper.Combine(externalUrl, "Account/ResetPassword?t=");
            resetLink += Uri.EscapeDataString(token);

            var displayNameField = (fieldsRow as IDisplayNameRow).DisplayNameField ??
                fieldsRow.NameField as StringField ??
                fieldsRow.EmailField;

            var emailModel = new ResetPasswordEmailModel
            {
                DisplayName = displayNameField[user],
                ResetLink = resetLink
            };

            var emailSubject = ResetPasswordFormTexts.EmailSubject.ToString(localizer);
            var emailBody = TemplateHelper.RenderViewToString(HttpContext.RequestServices,
                MVC.Views.Membership.PasswordActions.ResetPasswordEmail, emailModel);

            ArgumentNullException.ThrowIfNull(emailSender);

            emailSender.Send(subject: emailSubject, body: emailBody, mailTo: user.EmailField[user]);

            return new ServiceResponse();
        });
    }

    protected virtual string GenerateResetPasswordToken(TUserRow user)
    {
        return HttpContext.RequestServices.GetDataProtector("ResetPassword").ProtectBinary(bw =>
        {
            bw.Write(DateTime.UtcNow.AddHours(3).ToBinary());
            bw.Write(Convert.ToString(user.IdField.AsObject(user), CultureInfo.InvariantCulture));
            bw.Write(GetNonceFor(user));
        });
    }

    [HttpGet]
    public virtual IActionResult ResetPassword(string t,
        [FromServices] ISqlConnections sqlConnections,
        [FromServices] ITextLocalizer localizer,
        [FromServices] IOptions<MembershipSettings> options)
    {
        object userId;
        int nonce;
        try
        {
            using var br = HttpContext.RequestServices.GetDataProtector("ResetPassword").UnprotectBinary(t);
            var dt = DateTime.FromBinary(br.ReadInt64());
            if (dt < DateTime.UtcNow)
                return Error(ExtensionsTexts.Validation.InvalidResetToken.ToString(localizer));

            userId = new TUserRow().IdField.ConvertValue(br.ReadString(), CultureInfo.InvariantCulture);
            nonce = br.ReadInt32();
        }
        catch (Exception)
        {
            return Error(ExtensionsTexts.Validation.InvalidResetToken.ToString(localizer));
        }

        using (var connection = sqlConnections.NewFor<TUserRow>())
        {
            var user = connection.TryById<TUserRow>(userId);
            if (user == null || nonce != GetNonceFor(user))
                return Error(ExtensionsTexts.Validation.InvalidResetToken.ToString(localizer));
        }

        return this.PanelPage(GetResetPasswordPageModel(t, options.Value));
    }

    protected virtual ModulePageModel GetResetPasswordPageModel(string token, MembershipSettings settings)
    {
        return new()
        {
            Module = ModulePath(nameof(ResetPassword)),
            PageTitle = ResetPasswordFormTexts.FormTitle,
            Layout = "_LayoutNoNavigation",
            Options = new ResetPasswordOptions
            {
                token = token,
                minPasswordLength = settings.MinPasswordLength
            }
        };
    }

    private const string ResetPasswordPurpose = "ResetPassword";

    [HttpPost, JsonRequest]
    public virtual Result<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request,
        [FromServices] ITwoLevelCache cache,
        [FromServices] ISqlConnections sqlConnections,
        [FromServices] ITextLocalizer localizer,
        [FromServices] IPasswordStrengthValidator passwordStrengthValidator,
        [FromServices] IOptions<EnvironmentSettings> environmentOptions,
        [FromServices] IOptions<MembershipSettings> membershipOptions)
    {
        return this.InTransaction(GetConnectionKey(), uow =>
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrEmpty(request.Token);

            using var br = HttpContext.RequestServices.GetDataProtector(ResetPasswordPurpose)
                .UnprotectBinary(request.Token);

            var dt = DateTime.FromBinary(br.ReadInt64());
            if (dt < DateTime.UtcNow)
                throw new ValidationError(ExtensionsTexts.Validation.InvalidResetToken.ToString(localizer));

            var userId = new TUserRow().IdField.ConvertValue(br.ReadString(), CultureInfo.InvariantCulture);
            var nonce = br.ReadInt32();

            ArgumentNullException.ThrowIfNull(sqlConnections);

            TUserRow user = uow.Connection.TryById<TUserRow>(userId);
            if (user == null || nonce != GetNonceFor(user))
                throw new ValidationError(ExtensionsTexts.Validation.InvalidResetToken.ToString(localizer));

            if (request.ConfirmPassword != request.NewPassword)
                throw new ValidationError("PasswordConfirmMismatch", localizer.Get("Validation.PasswordConfirm"));

            request.NewPassword ??= "";
            passwordStrengthValidator.Validate(request.NewPassword);

            var salt = GenerateSalt(membershipOptions.Value);
            var hash = CalculateHash(request.NewPassword, salt);
#if (IsPublicDemo)
            if (user.IdField.AsObject(user)?.ToString() == "1")
                throw new ValidationError("Sorry, but no changes are allowed in public demo on ADMIN user!");
#endif
            var row = new TUserRow();
            row.IdField.AsObject(row, user.IdField.AsObject(user));
            if (row is IUpdateLogRow updateLogRow)
                updateLogRow.UpdateDateField[row] = DateTime.UtcNow;
            row.PasswordHashField[row] = hash;
            row.PasswordSaltField[row] = salt;
            uow.Connection.UpdateById(row);

            cache.InvalidateOnCommit(uow, row.Fields);

            return new ResetPasswordResponse
            {
                RedirectHome = User.IsLoggedIn()
            };
        });
    }
}