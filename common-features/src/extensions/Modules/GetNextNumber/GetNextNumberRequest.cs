namespace Serenity.Extensions;

/// <summary>
/// The request model for a get next number service.
/// </summary>
public class GetNextNumberRequest : ServiceRequest
{
    /// <summary>
    /// The prefix of the number.
    /// </summary>
    public string Prefix { get; set; }
    /// <summary>
    /// The total length of the serial number, including the prefix.
    /// </summary>
    public int Length { get; set; }
}