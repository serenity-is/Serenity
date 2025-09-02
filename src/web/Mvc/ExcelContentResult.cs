using Microsoft.AspNetCore.Mvc;

namespace Serenity.Web;

/// <summary>
/// Excel content result helper methods
/// </summary>
public static class ExcelContentResult
{
    /// <summary>
    /// Creates a FileContentResult containing passed data
    /// </summary>
    /// <param name="data">Data containing Excel bytes</param>
    /// <returns></returns>
    public static FileContentResult Create(byte[] data)
    {
        return Create(data, null);
    }

    /// <summary>
    /// Creates a FileContentResult containing passed data and a download name
    /// </summary>
    /// <param name="data">Data containing Excel file bytes</param>
    /// <param name="downloadName">Optional download name</param>
    public static FileContentResult Create(byte[] data, string downloadName)
    {
        var result = new FileContentResult(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = downloadName ?? ("report" +
            DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + ".xlsx")
        };
        return result;
    }
}
