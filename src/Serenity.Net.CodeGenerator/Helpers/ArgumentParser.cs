namespace Serenity.CodeGenerator;

/// <summary>
/// A simple command line arguments parser
/// </summary>
public partial class ArgumentParser
{
    private static readonly char[] assignmentChars = [':', '='];
    private static readonly string[] helpSwitches = ["?", "h", "help"];

    [GeneratedRegex("^(-|--|/)([a-zA-Z_][a-zA-Z0-9_-]*)([:=].*)?$")]
    private static partial Regex IsSwitchRegex();

    /// <summary>
    /// Returns true if argument starts with '--', '-', or '/' and is 
    /// followed by an argument name that starts with letters or underscore 
    /// and followed with zero or more letters, underscores, dashes and 
    /// numbers which can also be followed by equal sign or colon to assign 
    /// values. Some examples include "/p", "-p", "--p", "-p:1", "--p=3", 
    /// "--p-r-m:5"
    /// </summary>
    /// <param name="argument">Argument to check</param>
    /// <returns></returns>
    public static bool IsSwitch(string argument)
    {
        return argument != null && IsSwitchRegex().IsMatch(argument);
    }

    /// <summary>
    /// Parses a switch that conforms to the format specified 
    /// in the  <see cref="IsSwitch(string)"/> function.
    /// </summary>
    /// <param name="argument">Argument to parse</param>
    /// <param name="value">Argument value</param>
    /// <returns>Null for non-switches, the name of the argument
    /// otherwise. Value is null for switches without value,
    /// empty string for switches that is empty after equal sign,
    /// and the value otherwise</returns>
    public static string ParseSwitch(string argument, out string value)
    {
        value = null;

        if (!IsSwitch(argument))
            return null;

        if (argument.StartsWith("--"))
            argument = argument[2..];
        else if (argument.StartsWith('/') || argument.StartsWith('-'))
            argument = argument[1..];

        var index = argument.IndexOfAny(assignmentChars);
        if (index >= 0)
        {
            value = argument[(index + 1)..];
            argument = argument[..index];
        }

        return argument;
    }

    private static IEnumerable<(string name, string value)> EnumerateValueArguments(
        List<string> arguments, string[] names, bool allowEmpty)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        ArgumentNullException.ThrowIfNull(names);
        if (names.Length == 0)
            throw new ArgumentNullException(nameof(names));

        for (var i = 0; i < arguments.Count; i++)
        {
            var name = ParseSwitch(arguments[i], out string value);
            if (name != null && names.Contains(name, StringComparer.Ordinal))
            {
                arguments.RemoveAt(i);

                if (value is null)
                {
                    if (i < arguments.Count &&
                        !IsSwitch(arguments[i]))
                    {
                        value = arguments[i] ?? "";
                        arguments.RemoveAt(i);
                    }
                    else
                    {
                        throw new ArgumentException($"The switch '{name}' requires " +
                            $"a value to be specified!", name);
                    }
                }
                else
                {
                    value ??= "";
                }

                if (!allowEmpty &&
                    string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException($"A value is required when using " +
                        $"the switch '{name}'!", name);

                yield return (name, value.Trim());
            }
        }
    }

    /// <summary>
    /// Gets the value for a switch that can only be specified once.
    /// It removes the argument from the arguments array if it is found.
    /// </summary>
    /// <param name="arguments">Argument list</param>
    /// <param name="names">Allowed switch names</param>
    /// <param name="allowEmpty">True to allow empty values. Default is false.</param>
    /// <returns>The argument value or null if not found</returns>
    /// <exception cref="ArgumentException">The switch is specified multiple times,
    /// or its value is empty and allowEmpty is false.</exception>
    public static string GetSingleValue(List<string> arguments, string[] names, bool allowEmpty = false)
    {
        foreach (var (name, value) in EnumerateValueArguments(arguments, names, allowEmpty))
        {
            if (arguments.Any(x => names.Contains(ParseSwitch(x, out _))))
                throw new ArgumentException($"The switch '{name}' can only be specified once!", name);

            return value;
        }

        return null;
    }

    /// <summary>
    /// Gets the value for a switch that can be specified multiple times
    /// </summary>
    /// <param name="arguments">Argument list. The located arguments are removed
    /// from this list.</param>
    /// <param name="names">Allowed switch names.</param>
    /// <param name="allowEmpty">True to allow empty values. Default is false.</param>
    /// <returns>The argument values</returns>
    public static string[] GetMultipleValues(List<string> arguments, string[] names,
        bool allowEmpty = false)
    {
        var values = new List<string>();
        foreach (var (name, value) in EnumerateValueArguments(arguments, names, allowEmpty))
        {
            values.Add(value);
        }

        return [.. values];
    }

    /// <summary>
    /// Gets the value as a dictionary for a switch, whose values are also in the name=value 
    /// format.
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="names"></param>
    /// <param name="separators">Splits switch values by specified chars, so for example 
    /// providing the value in /param:A=B;C=D separated format is possible</param>
    /// <returns>A dictionary of key value pairs</returns>
    /// <exception cref="ArgumentException">Keys are specified multiple times</exception>
    public static Dictionary<string, string> GetDictionary(List<string> arguments, string[] names,
        char[] separators = null)
    {
        var assignments = GetMultipleValues(arguments, names, allowEmpty: false)
            .Select(x => (x ?? "").Trim())
            .SelectMany(x => separators?.Length > 0 ? x.Split(separators) : [x])
            .Where(x => !string.IsNullOrEmpty(x));

        var result = new Dictionary<string, string>();

        foreach (var assignment in assignments)
        {
            var eq = assignment.IndexOf('=');
            if (eq <= 0)
                throw new ArgumentException($"The values for switch '{names[0]}' " +
                    $"should use 'Name=Value' format!", names[0]);

            var propName = assignment[0..eq].Trim();
            if (propName.Length == 0)
                throw new ArgumentException($"One of the keys specified for " +
                    $"switch '{names[0]}' is empty!", names[0]);

            if (result.ContainsKey(propName))
                throw new ArgumentException($"The '{propName}' value is specified multiple " +
                    $"times for the switch '{names[0]}'!", names[0]);

            result[propName] = assignment[(eq + 1)..].Trim();
        }

        return result;
    }

    /// <summary>
    /// Returns true if any of switches are -?, --help, or -h
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public static bool HasHelpSwitch(IEnumerable<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        return arguments.Any(x => ParseSwitch(x, out _) is string s && 
            helpSwitches.Contains(s));
    }

    /// <summary>
    /// Throws argument null if arguments is not empty
    /// </summary>
    /// <param name="arguments">Argument list</param>
    /// <exception cref="ArgumentException"></exception>
    public static void EnsureEmpty(IEnumerable<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        if (arguments.Any())
            throw new ArgumentException($"Unknown argument: " +
                $"${arguments.First()}", nameof(arguments));
    }
}