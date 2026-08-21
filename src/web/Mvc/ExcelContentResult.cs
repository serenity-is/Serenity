using Microsoft.AspNetCore.Mvc;

namespace Serenity.Web;

/// <summary>
/// Excel content result helper methods.
/// </summary>
public static class ExcelContentResult
{
    /// <summary>
    /// Creates a <see cref="FileContentResult"/> containing the passed data.
    /// </summary>
    /// <param name="data">The data containing Excel bytes.</param>
    /// <returns>A <see cref="FileContentResult"/>.</returns>
    public static FileContentResult Create(byte[] data)
    {
        return Create(data, null);
    }

    /// <summary>
    /// Creates a <see cref="FileContentResult"/> containing the passed data and a download name.
    /// </summary>
    /// <param name="data">The data containing Excel file bytes.</param>
    /// <param name="downloadName">The optional download name.</param>
    /// <returns>A <see cref="FileContentResult"/>.</returns>
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
