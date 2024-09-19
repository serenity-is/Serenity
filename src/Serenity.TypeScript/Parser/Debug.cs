
namespace Serenity.TypeScript;

internal static class Debug
{
    internal static void Assert(bool condition, string message = null, params object[] args)
    {
        if (!condition)
            throw new ParseException(string.Format(message ?? "Parser assert condition failed!", args));
    }

    internal static ParseException Fail(string message, params object[] args)
    {
        return new ParseException(string.Format(message, args));
    }

}