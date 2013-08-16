using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Serenity.Web.Helpers
{
    /// <summary>
    ///  Class which contains helper functions to make working process involve in system as a different user
    /// </summary>  
    public class ImpersonalizationHelper
    {
        /// <summary>
        ///   Win32API user logon function.</summary>
        /// <param name="lpszUserName">
        ///   Username.</param>
        /// <param name="lpszDomain">
        ///   Domain.</param>
        /// <param name="lpszPassword">
        ///   Password.</param>
        /// <param name="dwLogonType">
        ///   Logon Type.</param>
        /// <param name="dwLogonProvider">
        ///   Provider</param>
        /// <param name="phToken">
        ///   Parameter in which auth ticket returns</param>
        /// <returns>
        ///   if process is successful different value than 0?</returns>
        [DllImport("advapi32.dll")]
        public static extern int LogonUserA(string lpszUserName, string lpszDomain, string lpszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        /// <summary>
        ///   clones the given ticket</summary>
        /// <param name="hToken">
        ///   Token.</param>
        /// <param name="impersonationLevel">
        ///   Impersonation level.</param>
        /// <param name="hNewToken">
        ///   Parameter in which auth ticket returns.</param>
        /// <returns>
        ///   if process is successful different value than 0?</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

        /// <summary>
        ///   Etkin kullanıcıyı asıl kullanıcıya çevirir.</summary>
        /// <summary>
        ///   Revert active user to main user</summary>
        /// <returns>
        ///   if successful return true?</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

        /// <summary>
        ///   Closes given Win32 handle.</summary>
        /// <param name="handle">
        ///   Handle to be closed.</param>
        /// <returns>
        ///   if successful return true.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle); 
        
        private WindowsImpersonationContext impersonationContext;
        const int LOGON32_LOGON_INTERACTIVE = 2;
        const int LOGON32_PROVIDER_DEFAULT = 0;

        /// <summary>
        ///   Creates a new ImpersonalizationHelper</summary>
        public ImpersonalizationHelper()
        {
        }

        /// <summary>
        ///   seperate domain name from full username </summary>
        /// <param name="fullUserName">
        ///  Full username such as DOMAIN\NAME</param>
        /// <returns>
        ///   Domain name.</returns>
        public static string ExtractDomainName(string fullUserName)
        {
            int pos = fullUserName.IndexOf('\\');
            if (pos < 0) 
                return string.Empty;
            return fullUserName.Substring(0, pos);
        }

        /// <summary>
        ///   seperate username name from full username </summary>
        /// <param name="fullUserName">
        ///  Full username like DOMAIN\NAME</param>
        /// <returns>
        ///   Username.</returns>
        public static string ExtractUserName(string fullUserName)
        {
            int pos = fullUserName.IndexOf('\\');
            if (pos < 0) 
                return fullUserName;
            return fullUserName.Substring(pos + 1);
        }

        /// <summary>
        ///   Dosyanın sahibi olan kullanıcıyı verir.</summary>
        /// <summary>
        ///   Returns the file owner</summary>
        /// <param name="fileName">
        ///   Filename.</param>
        /// <returns>
        ///  Full username like DOMAIN\NAME</returns>
        public static string FileOwnerName(string fileName)
        {
            FileSecurity sec = new FileSecurity(fileName, AccessControlSections.Owner);
            IdentityReference id = sec.GetOwner(typeof(NTAccount));
            return id.Value;
        }

        /// <summary>
        ///   Returns current user</summary>
        /// <returns>
        ///   full username like DOMAIN\NAME </returns>
        public static string CurrentUserName()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).Identity.Name;
        }

        /// <summary>
        ///   logon specified user temporarily
        ///   </summary>
        /// <param name="userName">
        ///   Username. </param>
        /// <param name="domain">
        ///   Domain.</param>
        /// <param name="password">
        ///   Password.</param>
        /// <returns>
        ///   if process successful returns true.</returns>
        public bool ImpersonateValidUser(string userName, string domain, string password)
        {
            WindowsIdentity tempWindowsIdentity;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;

            if (RevertToSelf())
            {
                if (LogonUserA(userName, domain, password, LOGON32_LOGON_INTERACTIVE,
                    LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                {
                    if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                    {
                        tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                        impersonationContext = tempWindowsIdentity.Impersonate();
                        if (impersonationContext != null)
                        {
                            CloseHandle(token);
                            CloseHandle(tokenDuplicate);
                            return true;
                        }
                    }
                }
            }
            if (token != IntPtr.Zero)
                CloseHandle(token);
            if (tokenDuplicate != IntPtr.Zero)
                CloseHandle(tokenDuplicate);
            return false;
        }

        /// <summary>
        ///   if there is a temporarily logged user, logs him out.</summary>
        public void UndoImpersonation()
        {
            if (impersonationContext != null) impersonationContext.Undo();
        }
    }
}