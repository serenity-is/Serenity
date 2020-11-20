using System;
using System.Security.Cryptography;

namespace Serenity.Web.Providers
{
    public static class SiteMembershipProvider
    {
        public static string ComputeSHA512(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException();

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(s);
            var sha512 = SHA512.Create();
            buffer = sha512.ComputeHash(buffer);
            return Convert.ToBase64String(buffer).Substring(0, 86); // strip padding
        }
    }
}