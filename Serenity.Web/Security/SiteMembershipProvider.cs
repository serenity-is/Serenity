using System;
#if !NET45
using System.Security.Cryptography;
#endif

namespace Serenity.Web.Providers
{
    public static class SiteMembershipProvider
    {
        public static string ComputeSHA512(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException();

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(s);
#if !NET45
            var sha512 = SHA512.Create();
#else
            var sha512 = System.Security.Cryptography.SHA512Managed.Create();
#endif
            buffer = sha512.ComputeHash(buffer);

            return System.Convert.ToBase64String(buffer).Substring(0, 86); // strip padding
        }
    }
}