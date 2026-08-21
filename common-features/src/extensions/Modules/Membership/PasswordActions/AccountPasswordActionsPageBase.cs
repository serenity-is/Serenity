using Microsoft.AspNetCore.DataProtection;

namespace Serenity.Extensions;

/// <summary>
/// Base class for account password action pages, such as change password,
/// set password, forgot password, and reset password.
/// </summary>
public abstract class AccountPasswordActionsPageBase<TUserRow> : MembershipPageBase<TUserRow>
    where TUserRow : class, IRow, IIdRow, IEmailRow, IPasswordRow, new()
{
    /// <summary>
    /// The folder containing the password action module scripts.
    /// </summary>
    protected virtual string ModuleFolder => "~/Serenity.Extensions/esm/Modules/Membership/PasswordActions/";
    /// <summary>
    /// Gets the module script path for the specified key.
    /// </summary>
    /// <param name="key">The page key.</param>
    /// <returns>The module script path.</returns>
    protected virtual string ModulePath(string key) => ModuleFolder + key + "Page.js";

    /// <summary>
    /// Renders the change password page, or the set password page if the user has no password set.
    /// </summary>
    /// <param name="userRetriever">The user retrieve service.</param>
    /// <returns>The change password or set password page result.</returns>
    [HttpGet, PageAuthorize]
    public virtual ActionResult ChangePassword(
        [FromServices] IUserRetrieveService userRetriever)
    {
        if (userRetriever.GetUserDefinition(User) is IHasPassword hasPassword &&
            !hasPassword.HasPassword)
        {
            return SetPassword();
        }

        return this.PanelPage(ModulePath(nameof(ChangePassword)),
            ChangePasswordFormTexts.FormTitle);
    }

    /// <summary>
    /// Renders the set password page.
    /// </summary>
    /// <returns>The set password page result.</returns>
    [HttpGet, PageAuthorize]
    public ActionResult SetPassword()
    {
        return this.PanelPage(new()
        {
            Module = ModulePath("SetPassword"),
            PageTitle = SetPasswordFormTexts.PageTitle
        });
    }

    /// <summary>
    /// Sends a reset password email to the current user, or returns a demo link in public demo mode.
    /// </summary>
    /// <param name="userRetriever">The user retrieve service.</param>
    /// <param name="emailSender">The email sender.</param>
    /// <param name="siteAbsoluteUrl">The site absolute URL service.</param>
    /// <param name="cache">The two level cache.</param>
    /// <param name="localizer">The text localizer.</param>
    /// <returns>The send reset password response.</returns>
    [HttpPost, ServiceAuthorize]
    public virtual ActionResult SendResetPassword(
        [FromServices] IUserRetrieveService userRetriever,
        [FromServices] IEmailSender emailSender,
        [FromServices] ISiteAbsoluteUrl siteAbsoluteUrl,
        [FromServices] ITwoLevelCache cache,
        [FromServices] ITextLocalizer localizer)
    {
        var userDefinition = userRetriever.GetUserDefinition(User) ??
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

    /// <summary>
    /// Changes the password of the current user after validating the old password and strength.
    /// </summary>
    /// <param name="request">The change password request.</param>
    /// <param name="cache">The two level cache.</param>
    /// <param name="passwordValidator">The password validator.</param>
    /// <param name="passwordStrengthValidator">The password strength validator.</param>
    /// <param name="userRetriever">The user retrieve service.</param>
    /// <param name="membershipOptions">The membership settings.</param>
    /// <param name="environmentOptions">The environment settings.</param>
    /// <param name="localizer">The text localizer.</param>
    /// <returns>The service response.</returns>
    [HttpPost, JsonRequest, ServiceAuthorize]
    public virtual Result<ServiceResponse> ChangePassword(ChangePasswordRequest request,
        [FromServices] ITwoLevelCache cache,
        [FromServices] IUserPasswordValidator passwordValidator,
        [FromServices] IPasswordStrengthValidator passwordStrengthValidator,
        [FromServices] IUserRetrieveService userRetriever,
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

            var userDefinition = userRetriever.GetUserDefinition(User);

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
            row.IdField.AsInvariant(row, userId);
            if (row is IUpdateDateRow updateDateRow)
                updateDateRow.UpdateDateField[row] = DateTime.UtcNow;
            row.PasswordHashField[row] = hash;
            row.PasswordSaltField[row] = salt;
            uow.Connection.UpdateById(row);

            cache.InvalidateOnCommit(uow, row.Fields);

            return new ServiceResponse();
        });
    }

    /// <summary>
    /// Renders the forgot password page.
    /// </summary>
    /// <returns>The forgot password page result.</returns>
    [HttpGet]
    public virtual ActionResult ForgotPassword()
    {
        return this.PanelPage(GetForgotPasswordPageModel());
    }

    /// <summary>
    /// Gets the module page model for the forgot password page.
    /// </summary>
    /// <returns>The forgot password page model.</returns>
    protected virtual ModulePageModel GetForgotPasswordPageModel()
    {
        return new ModulePageModel()
        {
            Module = ModulePath(nameof(ForgotPassword)),
            PageTitle = ForgotPasswordFormTexts.FormTitle,
            Layout = "_LayoutNoNavigation"
        };
    }

    /// <summary>
    /// Sends a reset password email to the user with the specified email address.
    /// </summary>
    /// <param name="request">The forgot password request.</param>
    /// <param name="emailSender">The email sender.</param>
    /// <param name="siteAbsoluteUrl">The site absolute URL service.</param>
    /// <param name="cache">The two level cache.</param>
    /// <param name="localizer">The text localizer.</param>
    /// <returns>The service response.</returns>
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
            if (updateRow is IUpdateDateRow updateDateRow && updateDateRow.UpdateDateField is not null)
            {
                // set update date to make sure only the latest reset token can be used
                updateRow.IdField.AsObject(updateRow, user.IdField.AsObject(user));
                updateDateRow.UpdateDateField[user] = DateTime.UtcNow;
                updateDateRow.UpdateDateField.AsObject(updateRow, updateDateRow.UpdateDateField.AsObject(user));
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

    /// <summary>
    /// Generates a protected reset password token for the specified user.
    /// </summary>
    /// <param name="user">The user row.</param>
    /// <returns>The reset password token.</returns>
    protected virtual string GenerateResetPasswordToken(TUserRow user)
    {
        return HttpContext.RequestServices.GetDataProtector("ResetPassword").ProtectBinary(bw =>
        {
            bw.Write(DateTime.UtcNow.AddHours(3).ToBinary());
            bw.Write(Convert.ToString(user.IdField.AsObject(user), CultureInfo.InvariantCulture));
            bw.Write(GetNonceFor(user));
        });
    }

    /// <summary>
    /// Validates the reset token and renders the reset password page if it is valid.
    /// </summary>
    /// <param name="t">The reset token.</param>
    /// <param name="sqlConnections">The SQL connections.</param>
    /// <param name="localizer">The text localizer.</param>
    /// <param name="options">The membership settings.</param>
    /// <returns>The reset password page or an error result.</returns>
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
                return Error(ChangePasswordValidationTexts.InvalidResetToken.ToString(localizer));

            userId = new TUserRow().IdField.ConvertValue(br.ReadString(), CultureInfo.InvariantCulture);
            nonce = br.ReadInt32();
        }
        catch (Exception)
        {
            return Error(ChangePasswordValidationTexts.InvalidResetToken.ToString(localizer));
        }

        using (var connection = sqlConnections.NewFor<TUserRow>())
        {
            var user = connection.TryById<TUserRow>(userId);
            if (user == null || nonce != GetNonceFor(user))
                return Error(ChangePasswordValidationTexts.InvalidResetToken.ToString(localizer));
        }

        return this.PanelPage(GetResetPasswordPageModel(t, options.Value));
    }

    /// <summary>
    /// Gets the module page model for the reset password page.
    /// </summary>
    /// <param name="token">The reset token.</param>
    /// <param name="settings">The membership settings.</param>
    /// <returns>The reset password page model.</returns>
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

    /// <summary>
    /// Resets the password of the user identified by the reset token.
    /// </summary>
    /// <param name="request">The reset password request.</param>
    /// <param name="cache">The two level cache.</param>
    /// <param name="sqlConnections">The SQL connections.</param>
    /// <param name="localizer">The text localizer.</param>
    /// <param name="passwordStrengthValidator">The password strength validator.</param>
    /// <param name="environmentOptions">The environment settings.</param>
    /// <param name="membershipOptions">The membership settings.</param>
    /// <returns>The reset password response.</returns>
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
                throw new ValidationError(ChangePasswordValidationTexts.InvalidResetToken.ToString(localizer));

            var userId = new TUserRow().IdField.ConvertValue(br.ReadString(), CultureInfo.InvariantCulture);
            var nonce = br.ReadInt32();

            ArgumentNullException.ThrowIfNull(sqlConnections);

            TUserRow user = uow.Connection.TryById<TUserRow>(userId);
            if (user == null || nonce != GetNonceFor(user))
                throw new ValidationError(ChangePasswordValidationTexts.InvalidResetToken.ToString(localizer));

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
            if (row is IUpdateDateRow updateDateRow)
                updateDateRow.UpdateDateField[row] = DateTime.UtcNow;
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