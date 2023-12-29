namespace Serenity.TypeScript;

internal sealed class ParseException : ArgumentException
{
    internal ParseException(string message, int? position = null)
        : base(message)
    {
        Position = position;
    }

    public int? Position { get; set; }
}
