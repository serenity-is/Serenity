namespace Serenity.Extensions;

public class PasswordStrengthValidator(IOptions<MembershipSettings> membershipSettings, 
    ITextLocalizer localizer) : IPasswordStrengthValidator
{
    private readonly IOptions<MembershipSettings> membershipSettings = membershipSettings 
        ?? throw new ArgumentNullException(nameof(membershipSettings));

    private readonly ITextLocalizer localizer = localizer 
        ?? throw new ArgumentNullException(nameof(localizer));

    public virtual void Validate(string password)
    {
        password ??= "";

        var rules = membershipSettings.Value;

        if (password.Length < (rules.MinPasswordLength))
            throw new ValidationError(nameof(MembershipSettings.MinPasswordLength), "Password",
                string.Format(CultureInfo.CurrentCulture, PasswordStrengthValidationTexts.MinRequiredPasswordLength.ToString(localizer), 
                membershipSettings.Value.MinPasswordLength));

        var lowerCount = 0;
        var upperCount = 0;
        var numericCount = 0;
        var nonAlphaCount = 0;

        foreach (var c in password)
        {
            if (char.IsLower(c)) 
                lowerCount++;

            if (char.IsUpper(c)) 
                upperCount++;

            if (char.IsDigit(c)) 
                numericCount++;

            if (!char.IsWhiteSpace(c) &&
                !char.IsLetterOrDigit(c)) 
                nonAlphaCount++;
        }

        if (rules.RequireUppercase && upperCount == 0)
            throw new ValidationError(nameof(rules.RequireUppercase), "Password", 
                PasswordStrengthValidationTexts.PasswordStrengthRequireUppercase.ToString(localizer));

        if (rules.RequireLowercase && lowerCount == 0)
            throw new ValidationError(nameof(rules.RequireLowercase), "Password",
                PasswordStrengthValidationTexts.PasswordStrengthRequireUppercase.ToString(localizer));

        if (rules.RequireDigit && lowerCount == 0)
            throw new ValidationError(nameof(rules.RequireDigit), "Password",
                PasswordStrengthValidationTexts.PasswordStrengthRequireDigit.ToString(localizer));

        if (rules.RequireNonAlphanumeric && nonAlphaCount == 0)
            throw new ValidationError(nameof(rules.RequireNonAlphanumeric), "Password",
                PasswordStrengthValidationTexts.PasswordStrengthRequireNonAlphanumeric.ToString(localizer));
    }
}
