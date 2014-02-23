using System;
using System.Web;

namespace Serenity.Web
{
    /// <summary>
    ///   Static class contains helper functions associated with urls </summary>
    public static class UrlHelper
    {
        /// <summary>
        ///   Adds a query to Querystring</summary>
        /// <param name="url">
        ///   URL in which query string will be appended (required). Url can contain existent query strings</param>
        /// <param name="queryKey">
        ///   Query string key (required).</param>
        /// <param name="queryValue">
        ///   Query string value (required, encoded with UrlEncode).</param>
        public static string AddQuery(string url, string queryKey, string queryValue)
        {
            if (queryKey == null || queryKey.Length == 0)
                throw new ArgumentNullException("queryKey");
            if (queryValue == null)
                throw new ArgumentNullException("queryValue");

            queryValue = HttpUtility.UrlEncode(queryValue);

            url = url ?? String.Empty;
            if (url.IndexOf('?') >= 0)
                return url + "&" + queryKey + "=" + queryValue;
            else
                return url + "?" + queryKey + "=" + queryValue;
        }


        /// <summary>
        ///   Adds a returnUrl query string to URL</summary>
        /// <param name="url">
        ///   URL in which returnURL query string will be appended.</param>
        /// <param name="returnUrl">
        ///   ReturnUrl.if url is a redirection url like ends with "/go.aspx",that part is removed 
        ///  </param>
        /// <returns>
        ///   URL in which returnUrl query string is appended.</returns>
        public static string AddReturnUrl(string url, string returnUrl)
        {
            if (!returnUrl.IsNullOrEmpty())
            {
                return AddQuery(url, "returnUrl", returnUrl);
            }
            else
                return url;
        }

        /// <summary>
        ///   Checks if a current http request has been created, if not throws error.</summary>
        /// <returns>
        ///   Current HttpRequest object.</returns>
        public static HttpRequest CheckCurrentRequest()
        {
            if (HttpContext.Current == null ||
                HttpContext.Current.Request == null)
                throw new InvalidOperationException("no current valid request!");
            else
                return HttpContext.Current.Request;
        }

        /// <summary>
        ///   Combine a url with a filename by inserting "/" char between them.This works like Path.Combine
        ///  </summary>
        /// <param name="url">
        ///   URL will be combined (null or empty can be used).</param>
        /// <param name="fileName">
        ///   Filename or Path (required).</param>
        /// <returns>
        ///   New string which consists of combining URL and Filename by inserting "/" char 
        ///   between them
        /// </returns>
        /// <remarks>
        ///   This function is used for only simple combining actions. like ".." relative actions 
        ///   doesn't be checked as well as URL with querystring doesn't be supported. 
        ///  </remarks>
        public static string Combine(string url, string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            if (url == null || url.Length == 0)
                return fileName;
            else if (url.Length > 0 && url[url.Length - 1] == '/')
            {
                if (fileName.Length > 0 && fileName[0] == '/')
                    return url + fileName.Substring(1);
                else
                    return url + fileName;
            }
            else if (fileName.Length > 0 && fileName[0] == '/')
                return url + fileName;
            else
                return url + "/" + fileName;
        }

        /// <summary>
        ///   Gets the raw URL of current request.If there is an existent dispatch part, it removes it</summary>
        /// <returns>
        ///   Current request raw url (removes dispatch part if it is existed).</returns>
        /// <remarks>
        ///   If there is not a current request, an error is thrown.</remarks>
        public static string GetCurrentUrl()
        {
            CheckCurrentRequest();
            return HttpContext.Current.Request.RawUrl;
        }

        /// <summary>
        ///   Returns current request "returnURL" query string if existed.</summary>
        /// <summary>
        ///   Returns "returnUrl" query string value.</summary>
        /// <returns>
        ///   "returnUrl" query string value.</returns>
        /// <remarks>
        ///   if there is no any current request, an error is thrown.</remarks>
        public static string GetQueryReturnUrl()
        {
            CheckCurrentRequest();
            return HttpContext.Current.Request.QueryString["returnURL"];
        }

        /// <summary>
        ///   Checks if given URL is dangerous URL which can be used for XSS attacks.</summary>
        /// <param name="url">
        ///   URL will be checked (can be null).</param>
        /// <returns>
        ///   if URL is not empty, doesn't begin with "http://" or "https://", or if it doesn't contains ":" character
        ///   returns true.</returns>
        /// <remarks>
        ///   This function can be used to prevent URLs constructed like "javascript:..." from using XSS attacks etc.
        ///   It is obtained from ASP.NET original source codes by using Reflection</remarks>
        public static bool IsDangerousUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            url = url.Trim();
            int length = url.Length;

            if (((((length > 4) &&  
                   ((url[0] == 'h') || (url[0] == 'H'))) && 
                   ((url[1] == 't') || (url[1] == 'T'))) &&  
                   (((url[2] == 't') || (url[2] == 'T')) && 
                   ((url[3] == 'p') || (url[3] == 'P')))) && 
                   ((url[4] == ':') || 
                 (((length > 5) && 
                   ((url[4] == 's') || (url[4] == 'S'))) && 
                   (url[5] == ':'))))
                return false;

            if (url.IndexOf(':') == -1)
                return false;

            return true;
        }

        /// <summary>
        ///   Removes query string part from given URL.</summary>
        /// <param name="url">
        ///   URL (can be null).</param>
        /// <returns>
        ///   URL without querystring.</returns>
        public static string RemoveQuery(string url)
        {
            if (url == null || url.Length == 0)
                return null;

            int p = url.IndexOf('?');
            if (p >= 0)
                return url.Substring(0, p);
            else
                return url;
        }

        /// <summary>
        ///   Such as Control.ResolveUrl it resolves urls which is relative to application path  like "~/path/page.aspx".
        /// </summary>
        /// <param name="url">
        ///   URL will be resolved (null olabilir).</param>
        /// <returns>
        ///   Returns a url which is resolved according to application root.</returns>
        /// <remarks>
        ///   If there is no any current request an error is thrown.</remarks>
        public static string ResolveUrl(string url)
        {
            CheckCurrentRequest();
            return ResolveUrl(HttpContext.Current.Request.ApplicationPath, url);
        }

        /// <summary>
        ///   Such as Control.ResolveUrl it resolves urls which is relative 
        ///   to given application  path like "~/path/page.aspx".
        /// </summary>
        /// <param name="appPath">
        ///   Root application path required to be used (required, like "/application" ).</param>
        /// <param name="url">
        ///   URL will be resolved (can be used).</param>
        /// <returns>
        ///   Returns a url which is resolved according to given application root.</returns>
        /// <returns>
        ///   .</returns>
        public static string ResolveUrl(string appPath, string url)
        {
            url = url ?? String.Empty;

            if (url.Length == 0 || url[0] != '~')
                return url;     
            else
            {
                if (url.Length == 1)
                    return appPath; // only "~"

                if (url[1] == '/' || url[1] == '\\')
                {
                    // url ~/ or like ~\ 
                    if (appPath.Length > 1)
                        return appPath + "/" + url.Substring(2);
                    else
                        return "/" + url.Substring(2);
                }
                else
                {
                    // url like ~path 
                    if (appPath.Length > 1)
                        return appPath + "/" + url.Substring(1);
                    else
                        return appPath + url.Substring(1);
                }
            }
        }
        
        /// <summary>
        ///   Returns application root path.</summary>
        /// <returns>
        ///   Application root path like "/application/" .</returns>
        /// <remarks>
        ///   if there isn't a current request, an error is thrown.</remarks>
        public static string GetApplicationRootPath()
        {
            CheckCurrentRequest();

            string path = HttpContext.Current.Request.ApplicationPath;
            if (path.Length == 0 || path[0] != '/')
                path = "/" + path;
            if (path[path.Length - 1] != '/')
                path += "/";
            return path;
        }

        /// <returns>
        ///   "http://localhost:8080" gibi kök path.</returns>
        /// <remarks>
        ///   Etkin bir HTTP request yoksa hata oluşur.</remarks>
        /// <summary>
        ///   Returns server root url.</summary>
        /// <returns>
        ///   such as "http://localhost:8080" </returns>
        /// <remarks>
        ///   if there isn't a current request, an error is thrown.</remarks>
        public static string GetServerRootUrl()
        {
            CheckCurrentRequest();

            int port = HttpContext.Current.Request.Url.Port;
            string sPort = "";
            if (port != 80)
                sPort = ":" + port.ToString();
            return
                String.Format("{0}://{1}{2}",
                    HttpContext.Current.Request.Url.Scheme,
                    HttpContext.Current.Request.Url.Host,
                    sPort);
        }

        /// <summary>
        ///   Converts backslashes to forward slashes</summary>
        /// <param name="fileName">
        ///   Filename.</param>
        /// <returns>
        ///   Converted filename.</returns>
        public static string ToUrl(string fileName)
        {
            if (fileName != null && fileName.IndexOf('\\') >= 0)
                return fileName.Replace('\\', '/');
            else
                return fileName;
        }
    }
}