namespace Serenity.Extensions;

/// <summary>
/// The request model for an Excel import service.
/// </summary>
public class ExcelImportRequest : ServiceRequest
{
    /// <summary>
    /// The name of the uploaded Excel file.
    /// </summary>
    public string FileName { get; set; }
}

/// <summary>
/// The response model for an Excel import service.
/// </summary>
public class ExcelImportResponse : ServiceResponse
{
    /// <summary>
    /// The number of records inserted.
    /// </summary>
    public int Inserted { get; set; }
    /// <summary>
    /// The number of records updated.
    /// </summary>
    public int Updated { get; set; }
    /// <summary>
    /// The list of errors encountered during import, if any.
    /// </summary>
    public List<string> ErrorList { get; set; }
}