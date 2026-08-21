namespace Serenity.Extensions;

/// <summary>
/// The response model for a get next number service.
/// </summary>
public class GetNextNumberResponse : ServiceResponse
{
    /// <summary>
    /// The next number in the sequence.
    /// </summary>
    public long Number { get; set; }
    /// <summary>
    /// The serial representation of the number, including the prefix.
    /// </summary>
    public string Serial { get; set; }
}