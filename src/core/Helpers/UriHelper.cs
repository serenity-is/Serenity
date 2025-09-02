namespace Serenity;

/// <summary>
/// Contains URI related helper functions.
/// </summary>
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

        if (url == null)
            throw new ArgumentNullException("url");

        if (fileName.Contains(".."))
            throw new ArgumentOutOfRangeException("fileName");

        if (url.Contains(".."))
            throw new ArgumentOutOfRangeException("url");

        if (url.Length == 0)
        {
            return fileName;
        }
        else if (url.Length > 0 && url[^1] == '/')
        {
            if (fileName.Length > 0 && fileName[0] == '/')
                return url + fileName[1..];
            else
                return url + fileName;
        }
        else if (fileName.Length > 0 && fileName[0] == '/')
        {
            return url + fileName;
        }
        else
            return url + "/" + fileName;
    }
}