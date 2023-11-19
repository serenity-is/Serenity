namespace Serenity.TypeScript.TsParser;

internal class Debug
{
    internal static void Assert(bool condition, string message = null)
    {
        System.Diagnostics.Debug.Assert(condition, message);
    }

    internal static void Fail(string message = null)
    {
        System.Diagnostics.Debug.Fail(message);
    }
}