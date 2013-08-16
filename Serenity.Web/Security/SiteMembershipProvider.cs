using Serenity.Data;
using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace Serenity.Web.Providers
{
    public class SiteMembershipProvider : MembershipProvider
    {
        private const int DefaultMinRequiredPasswordLength = 8;
        private const string EmailPattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        private NameValueCollection _configuration;

        static SiteMembershipProvider()
        {
        }

        public SiteMembershipProvider()
        {
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrEmpty(name))
                name = "SiteMembershipProvider";

            if (String.IsNullOrEmpty(config["description"]))
                config["description"] = "Site Membership Provider";

            base.Initialize(name, config);

            _configuration = config;
        }

        public override string ApplicationName
        {
            get { return "site"; }
            set { }
        }

        public override bool EnablePasswordReset
        {
            get { return true; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 0; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return System.Convert.ToInt32(this.GetConfig("minRequiredNonAlphanumericCharacters", "0")); }
        }

        public override int MinRequiredPasswordLength
        {
            get { return System.Convert.ToInt32(this.GetConfig("minRequiredPasswordLength", DefaultMinRequiredPasswordLength.ToString())); }
        }

        public override int PasswordAttemptWindow
        {
            get { return 0; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return System.Convert.ToBoolean(this.GetConfig("requiresUniqueEmail", "true")); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return this.GetConfig("passwordStrengthRegularExpression", ""); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override MembershipUser CreateUser(string username, string password, string email,
            string passwordQuestion, string passwordAnswer, bool isApproved,
            object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (String.IsNullOrEmpty(username) || username.Length > 100)
                return null;

            var user = IoC.Resolve<IUserRetrieveService>().ByUsername(username);
            if (user != null)
                return UserCacheItemToMembershipUser(user);

            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (providerUserKey == null)
                return null;

            int userId = System.Convert.ToInt32(providerUserKey, Invariants.NumberFormat);

            var user = IoC.Resolve<IUserRetrieveService>().ById(userId);
            if (user != null)
                return UserCacheItemToMembershipUser(user);

            return null;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new ProviderException("Password questions are not implemented in this provider.");
        }

        public override string GetPassword(string username, string answer)
        {
            throw new ProviderException("Password retrieval is not possible for hashed passwords.");
        }

        public override string GetUserNameByEmail(string email)
        {
            if (String.IsNullOrEmpty(email) || email.Length > 100)
                return null;

            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            if (String.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) ||
                username.Length > 100)
                return false;

            return IoC.Resolve<IUserAuthenticationService>().Validate(username, password);
        }

        public void ValidateNewPassword(string username, string newPassword, bool isNewUser)
        {
            if (newPassword.Length < MinRequiredPasswordLength)
                throw new ArgumentException(String.Format("The length of parameter 'newPassword' needs to be greater or equal to '{0}'.",
                    MinRequiredPasswordLength.ToString(Invariants.NumberFormat)));

            if (MinRequiredNonAlphanumericCharacters > 0)
            {
                int count = 0;
                for (int i = 0; i < newPassword.Length; i++)
                    if (!Char.IsLetterOrDigit(newPassword, i)) count++;

                if (count < MinRequiredNonAlphanumericCharacters)
                    throw new ArgumentException(String.Format("Non alpha numeric characters in 'newPassword' needs to be greater than or equal to '{0}'.",
                        MinRequiredNonAlphanumericCharacters.ToString(Invariants.NumberFormat)));
            }

            if (!string.IsNullOrEmpty(PasswordStrengthRegularExpression) &&
                !Regex.IsMatch(newPassword, PasswordStrengthRegularExpression))
                throw new ArgumentException("The parameter 'newPassword' does not match the regular expression specified in config file.");

            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPassword, true);
            OnValidatingPassword(args);

            if (args.Cancel)
            {
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string username)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string nameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            return 0;
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        private bool SetPassword(string username, string password)
        {
            throw new NotImplementedException();
        }

        private MembershipUser UserCacheItemToMembershipUser(IUserDefinition item)
        {
            var empty = new DateTime();
            return new MembershipUser(this.Name, item.Username, item.UserId, item.Email, String.Empty, null, item.IsActive > 0,
                item.IsActive <= 0, empty, empty, empty, empty, empty);
        }

        private static bool IsEmail(string email)
        {
            if (String.IsNullOrEmpty(email) || email.Length > 100)
                return false;

            return Regex.IsMatch(email, EmailPattern);
        }

        public bool CheckUserExists(string username)
        {
            return IoC.Resolve<IUserRetrieveService>().ByUsername(username) != null;
        }

        private string GetConfig(string name, string defaultValue)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            string value = _configuration[name];

            if (string.IsNullOrEmpty(value))
                value = defaultValue;

            return value;
        }

        public static string ComputeSHA512(string s)
        {
            if (String.IsNullOrEmpty(s))
                throw new ArgumentNullException();

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(s);

            buffer = System.Security.Cryptography.SHA512Managed.Create().ComputeHash(buffer);

            return System.Convert.ToBase64String(buffer).Substring(0, 86); // strip padding
        }
    }
}