using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Serenity.Web.Helpers
{
    /// <summary>
    ///   Static class contains helper functions to access drives and map.
    ///   </summary>
    /// <remarks>
    ///   All processes is done by functions imported over "mpr.dll"</remarks>
    public static class NetMappingHelper
    {
        /// <summary>
        ///   The Structure is used for passing network source parameters to WNetAddConnection </summary>
        /// <remarks>
        ///   http://msdn2.microsoft.com/en-us/library/aa385353(VS.85).aspx</remarks>
        [StructLayout(LayoutKind.Sequential)]
        public struct NETRESOURCE
        {
            /// <summary>
            ///   Scope. 
            ///   RESOURCE_CONNECTED = 1, RESOURCE_GLOBALNET = 2, RESOURCE_REMEMBERED = 3.</summary>
            public int dwScope;
            /// <summary>
            ///   Source Type. 
            ///   RESOURCETYPE_ANY = 0, RESOURCETYPE_DISK = 1, RESOURCETYPE_PRINT = 2</summary>
            public int dwType;
            /// <summary>
            ///   Source Display Type.
            ///   RESOURCEDISPLAYTYPE_GENERIC = 0, RESOURCE_DISPLAYTYPE_DOMAIN = 1...</summary>
            public int dwDisplayType;
            /// <summary>
            ///   Flags determines how to use network source
            ///   RESOURCEUSAGE_CONNECTABLE = 1, RESOURCEUSAGE_CONTAINER = 2...</summary>
            public int dwUsage;
            /// <summary>
            ///   When dwScope become, RESOURCE_CONNECTED or RESOURCE_REMEMBERED, Determines local device name
            ///  </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpLocalName;
            /// <summary>
            ///   Network device name. It must conform to Network Provider Naming convention.</summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpRemoteName;
            /// <summary>
            ///   An Information provided by Network  Provider.</summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpComment;
            /// <summary>
            ///   Network Provider Name.</summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpProvider;
        }

        /// <summary>
        ///   WIN32 API - WNETAddConnection2A function.</summary>        
        /// <param name="lpNetResource">
        ///   <see cref="NETRESOURCE"/> pointer expose structure.It used only dwType, lpLocalName,
        ///   lpRemoteName, lpProvider fields, the rest is ignored.</param>
        /// <param name="lpPassword">
        ///   Password</param>
        /// <param name="lpUserName">
        ///   Username.</param>
        /// <param name="dwFlags">
        ///   Connection preferences.
        ///   CONNECT_UPDATE_PROFILE = 1, CONNECT_UPDATE_RECENT = 2, CONNECT_TEMPORARY = 3...</param>
        /// <returns>
        ///   if successfult NO_ERROR = 0. else error code.</returns>
        /// <remarks>
        ///   http://msdn2.microsoft.com/en-us/library/aa385413.aspx</remarks>
        [DllImport("mpr.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int WNetAddConnection2A(
            [MarshalAs(UnmanagedType.LPArray)]
            NETRESOURCE[] lpNetResource,
            [MarshalAs(UnmanagedType.LPStr)]
            string lpPassword,
            [MarshalAs(UnmanagedType.LPStr)]
            string lpUserName,
            int dwFlags
        );

        /// <summary>
        ///    WIN32 API - WNetCancelConnection2A.</summary>
        /// <param name="lpName">
        ///   Source name.</param>
        /// <param name="dwFlags">
        ///   options.</param>
        /// <param name="fForce">
        ///   enforce to disconnect.</param>
        /// <returns>
        ///   If successful NO_ERROR = 0.</returns>
        /// <remarks>
        ///   http://msdn2.microsoft.com/en-us/library/aa385427(VS.85).aspx</remarks>
        [DllImport("mpr.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int WNetCancelConnection2A(
            [MarshalAs(UnmanagedType.LPStr)]
            string lpName,
            int dwFlags,
            int fForce
        );

        /// <summary>
        ///   Assign local drive name connecting to target network source.</summary>
        /// <param name="share">
        ///   Network source to be used. such as "\\server\share".</param>
        /// <param name="local">
        ///   Local source name. such as "m:".</param>
        /// <param name="user">
        ///   Username.</param>
        /// <param name="pwd">
        ///   Password.</param>
        /// <returns>
        ///   Function result. if successful NO_ERROR = 0, else error code.</returns>
        public static int Connect(string share, string local, string user, string pwd)
        {
            if (String.IsNullOrEmpty(share))
                throw new ArgumentNullException("share");
            if (String.IsNullOrEmpty(local))
                throw new ArgumentNullException("local");
            if (user == null)
                throw new ArgumentNullException("user");
            if (pwd == null)
                throw new ArgumentNullException("pwd");

            //The NETRESOURCE struct is required by the WinAPI function
            NETRESOURCE[] nr = new NETRESOURCE[1];
            nr[0].lpRemoteName = share;
            nr[0].lpLocalName = local;  //mLocalName;
            nr[0].dwType = 1;     //disk
            nr[0].dwDisplayType = 0;
            nr[0].dwScope = 0;
            nr[0].dwUsage = 0;
            nr[0].lpComment = "";
            nr[0].lpProvider = "";
            return (WNetAddConnection2A(nr, pwd, user, 0));
        }

        /// <summary>
        ///   Disconnect current connection of network source</summary>
        /// <param name="share">
        ///   Sharing name to be disconnect.</param>
        /// <returns>
        ///   if successful NO_ERROR = 0, else error code.</returns>
        public static int Disconnect(string share)
        {
            if (String.IsNullOrEmpty(share))
                throw new ArgumentNullException("share");

            return (WNetCancelConnection2A(share, 0, -1));
        }
    }
}