using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Serenity.Web.Providers;
using System.Data;
using System.Globalization;
using System.IO;
using MyRow = Serene.Administration.UserRow;

namespace Serene.Administration;

public static class UserHelper
{
    private static MyRow.RowFields Fld { get { return MyRow.Fields; } }

    public static void CheckPublicDemo(this IOptions<EnvironmentSettings> environmentSettings, int? userID)
    {
        if (userID == 1 && environmentSettings.Value.IsPublicDemo)
            throw new ValidationError("Sorry, but no changes " +
                "are allowed in public demo on ADMIN user!");
    }

    public static bool IsValidPhone(string number)
    {
        // please change this to a valid check for mobile phones in your country
        return !number.IsNullOrEmpty() && number.Length > 7 && long.TryParse(number, out long _);
    }

    public static string ValidateDisplayName(string displayName, ITextLocalizer localizer)
    {
        displayName = displayName.TrimToNull();

        if (displayName == null)
            throw DataValidation.RequiredError(Fld.DisplayName, localizer);

        return displayName;
    }

    public static string ValidatePassword(string password, ITextLocalizer localizer)
    {
        password = password.TrimToNull();

        if (password == null ||
            password.Length < 6)
            throw new ValidationError("PasswordLength", "Password",
                string.Format(CultureInfo.CurrentCulture, PasswordStrengthValidationTexts.MinRequiredPasswordLength.ToString(localizer), 6));

        return password;
    }
    public static string CalculateHash(string password, string salt)
    {
        return SiteMembershipProvider.ComputeSHA512(password + salt);
    }

    public static string GenerateHash(string password, ref string salt)
    {
        salt ??= Serenity.IO.TemporaryFileHelper.RandomFileCode()[..5];
        return CalculateHash(password, salt);
    }

    public static string GetImpersonationToken(IMemoryCache memoryCache, IDataProtector dataProtector, 
        byte[] clientHash, string forUsername, string username)
    {
        if (memoryCache is null)
            throw new ArgumentNullException(nameof(memoryCache));

        return memoryCache.Get("ImpersonationToken:" + clientHash + ":" + username,
            TimeSpan.FromMinutes(30), () =>
            {
                byte[] bytes;
                using (var ms = new MemoryStream())
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(DateTime.UtcNow.AddMinutes(60).ToBinary());
                    bw.Write(forUsername);
                    bw.Write(username);
                    bw.Write(clientHash);
                    bw.Flush();
                    bytes = ms.ToArray();
                }

                var token = Convert.ToBase64String(dataProtector.Protect(bytes));
                return Uri.EscapeDataString(token);
            });
    }

    public static MyRow GetUser(IDbConnection connection, BaseCriteria filter)
    {
        var row = new MyRow();
        if (new SqlQuery().From(row)
            .Select(
                Fld.UserId,
                Fld.Username,
                Fld.DisplayName,
                Fld.PasswordHash,
                Fld.PasswordSalt,
                Fld.IsActive)
            .Where(filter)
            .GetFirst(connection))
        {
            return row;
        }

        return null;
    }

    public static bool IsInvariantLetter(char c)
    {
        return (c >= 'A' && c <= 'Z') ||
            (c >= 'a' && c <= 'z');
    }

    public static bool IsDigit(char c)
    {
        return (c >= '0' && c <= '9');
    }

    public static bool IsValidEmailChar(char c)
    {
        return IsInvariantLetter(c) ||
            IsDigit(c) ||
            c == '.' ||
            c == '-' ||
            c == '_' ||
            c == '@';
    }

    public static bool IsValidUsername(string name)
    {
        if (name == null ||
            name.Length < 0)
            return false;

        var c = name[0];
        if (!IsInvariantLetter(c))
            return false;

        for (var i = 1; i < name.Length - 1; i++)
        {
            c = name[i];
            if (!IsValidEmailChar(c))
                return false;
        }

        return true;
    }
}