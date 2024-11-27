namespace Serenity.Extensions;

public class GetNextNumberRequest : ServiceRequest
{
    public string Prefix { get; set; }
    public int Length { get; set; }
}