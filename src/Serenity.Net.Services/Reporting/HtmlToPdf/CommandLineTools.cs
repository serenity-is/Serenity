namespace Serenity.IO;

/// <summary>
/// Contains functions related to command line
/// </summary>
public class CommandLineTools
{
    private static readonly Regex invalidChar = new Regex("[\x00\x0a\x0d]", RegexOptions.Compiled); //  these can not be escaped
    private static readonly Regex needsQuotes = new Regex(@"\s|""", RegexOptions.Compiled); // contains whitespace or two quote characters
    private static readonly Regex escapeQuote = new Regex(@"(\\*)(""|$)", RegexOptions.Compiled); // one or more '\' followed with a quote or end of string

    /// <summary>
    /// Quotes all arguments that contain whitespace, or begin with a quote and returns a single
    /// argument string for use with Process.Start().
    /// </summary>
    /// <param name="args">A list of strings for arguments, may not contain null, '\0', '\r', or '\n'</param>
    /// <returns>The combined list of escaped/quoted strings</returns>
    /// <exception cref="ArgumentNullException">Raised when one of the arguments is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Raised if an argument contains '\0', '\r', or '\n'</exception>
    public static string EscapeArguments(params string[] args)
    {
        var arguments = new StringBuilder();

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == null)
                throw new ArgumentNullException("args[" + i + "]");

            if (invalidChar.IsMatch(args[i]))
                throw new ArgumentOutOfRangeException("args[" + i + "]");

            if (i > 0)
                arguments.Append(' ');

            if (args[i] == string.Empty)
            {
                arguments.Append("\"\"");
            }
            else if (!needsQuotes.IsMatch(args[i]))
            {
                arguments.Append(args[i]);
                continue;
            }
            else
            {
                arguments.Append('"');
                arguments.Append(escapeQuote.Replace(args[i], m => m.Groups[1].Value + m.Groups[1].Value + (m.Groups[2].Value == "\"" ? "\\\"" : "")));
                arguments.Append('"');
            }
        }

        return arguments.ToString();
    }
}