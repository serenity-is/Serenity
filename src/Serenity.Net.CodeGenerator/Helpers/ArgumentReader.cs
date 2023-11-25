namespace Serenity.CodeGenerator;

/// <summary>
/// A simple command line arguments parser
/// </summary>
public partial class ArgumentReader(IEnumerable<string> arguments) : IArgumentReader
{
    private static readonly char[] assignmentChars = [':', '='];
    private static readonly string[] helpSwitches = ["?", "h", "help"];
    private readonly List<string> arguments = (arguments ??
        throw new ArgumentNullException(nameof(arguments))).ToList();

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
    /// <returns>True if matches switch format</returns>
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

    private IEnumerable<(string name, string value)> EnumerateValueArguments(
        string[] names, bool allowEmpty)
    {
        ArgumentNullException.ThrowIfNull(names);
        if (names.Length == 0)
            throw new ArgumentNullException(nameof(names));

        int i = 0;
        while (i < arguments.Count)
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
            else
                i++;
        }
    }

    /// <inheritdoc/>
    public void EnsureEmpty()
    {
        if (arguments.Count != 0)
            throw new ArgumentException($"Unknown argument: " +
                $"${arguments[0]}", nameof(arguments));
    }

    /// <inheritdoc/>
    public bool HasHelpSwitch()
    {
        return arguments.Any(x => ParseSwitch(x, out _) is string s &&
            helpSwitches.Contains(s));
    }

    /// <inheritdoc/>
    public string GetCommand()
    {
        string command = null;
        var commandIndex = arguments.FindIndex(x =>
            !string.IsNullOrWhiteSpace(x) && !IsSwitch(x));
        if (commandIndex >= 0)
        {
            command = arguments[commandIndex].TrimToNull()?.ToLowerInvariant();
            arguments.RemoveAt(commandIndex);
            return command;
        }

        return null;
    }


    /// <inheritdoc/>
    public string GetString(string[] names, bool allowEmpty = false)
    {
        foreach (var (name, value) in EnumerateValueArguments(names, allowEmpty))
        {
            if (arguments.Any(x => names.Contains(ParseSwitch(x, out string otherValue)) &&
                    otherValue != value))
                throw new ArgumentException($"The switch '{name}' can only be specified once!", name);

            return value;
        }

        return null;
    }

    /// <inheritdoc/>
    public string[] GetStrings(string[] names, bool allowEmptyValue = false)
    {
        var values = new List<string>();
        foreach (var (name, value) in EnumerateValueArguments(names, allowEmptyValue))
        {
            values.Add(value);
        }

        return [.. values.Distinct(StringComparer.Ordinal)];
    }

    /// <inheritdoc/>
    public Dictionary<string, string> GetDictionary(string[] names,
        char[] separators = null)
    {
        var assignments = GetStrings(names, allowEmptyValue: false)
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

            var value = assignment[(eq + 1)..].Trim();
            if (result.TryGetValue(propName, out string existingValue))
            {
                if (existingValue != value)
                    throw new ArgumentException($"The '{propName}' value is specified multiple " +
                        $"times for the switch '{names[0]}'!", names[0]);
            }
            else
                result[propName] = value;
        }

        return result;
    }

    /// <inheritdoc/>
    public int Remaining => arguments.Count;
}