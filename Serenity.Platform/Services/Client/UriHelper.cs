using System.Net;
using System.IO;
using System;

namespace Serenity.Services
{
    public static class UriHelper
    {
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
    }
}