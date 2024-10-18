using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Build;

public partial class ArgumentReader(IEnumerable<string> arguments)
{
    private static readonly char[] assignmentChars = [':', '='];
    private static readonly string[] helpSwitches = ["?", "h", "help"];
    private readonly List<string> arguments = (arguments ??
        throw new ArgumentNullException(nameof(arguments))).ToList();

    [GeneratedRegex("^(-|--|/)(\\?|([a-zA-Z_][a-zA-Z0-9_-]*)([:=].*)?)$")]
    private static partial Regex IsSwitchRegex();

    public static bool IsSwitch(string argument)
    {
        return argument != null && IsSwitchRegex().IsMatch(argument);
    }

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
        string[] names, bool? required)
    {
        ArgumentNullException.ThrowIfNull(names);
        if (names.Length == 0)
            throw new ArgumentNullException(nameof(names));

        bool isBoolean = required == null;
        static bool isTrue(string val) => string.Equals(val, "true", StringComparison.OrdinalIgnoreCase);
        static bool isFalse(string val) => string.Equals(val, "false", StringComparison.OrdinalIgnoreCase);

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
                        if (!isBoolean || isTrue(arguments[i]) || isFalse(arguments[i]))
                        {
                            value = arguments[i];
                            arguments.RemoveAt(i);
                        }
                    }
                    else if (isBoolean)
                    {
                        value = "true";
                    }
                    else
                    {
                        throw new ArgumentException($"The switch '{name}' requires " +
                            $"a value to be specified!", name);
                    }
                }

                if (isBoolean)
                {
                    if (string.IsNullOrEmpty(value) ||
                        (!isTrue(value) && !isFalse(value)))
                        throw new ArgumentException($"Only true/false can be passed when using " +
                            $"the switch '{name}'!", name);

                    value = isTrue(value) ? "true" : "false";
                }
                else if (required == true &&
                    string.IsNullOrEmpty(value))
                    throw new ArgumentException($"A value is required when using " +
                        $"the switch '{name}'!", name);

                yield return (name, value);
            }
            else
                i++;
        }
    }

    public void ThrowIfRemaining()
    {
        if (arguments.Count != 0)
            throw new ArgumentException($"Unknown argument: " +
                $"${arguments[0]}", nameof(arguments));
    }

    public bool HasHelpSwitch()
    {
        return arguments.Any(x => ParseSwitch(x, out _) is string s &&
            helpSwitches.Contains(s));
    }

    public string GetCommand()
    {
        string command = null;
        var commandIndex = arguments.FindIndex(x => !string.IsNullOrEmpty(x) && !IsSwitch(x));
        if (commandIndex >= 0)
        {
            command = arguments[commandIndex];
            arguments.RemoveAt(commandIndex);
            return command;
        }

        return null;
    }

    public bool? GetBoolean(string[] names)
    {
        string result = null;
        foreach (var (name, value) in EnumerateValueArguments(names, required: null))
        {
            if (result != null &&
                value != result)
                throw new ArgumentException($"The switch '{name}' can only be specified once!", name);

            result = value;
        }

        return result == null ? null : result == "true";
    }

    public string GetString(string[] names, bool required = true)
    {
        string result = null;
        foreach (var (name, value) in EnumerateValueArguments(names, required))
        {
            if (result != null &&
                value != result)
                throw new ArgumentException($"The switch '{name}' can only be specified once!", name);

            result = value;
        }

        return result;
    }

    public string[] GetStrings(string[] names, bool required = true)
    {
        var values = new List<string>();
        foreach (var (name, value) in EnumerateValueArguments(names, required))
        {
            values.Add(value);
        }

        return [.. values.Distinct(StringComparer.Ordinal)];
    }

    public Dictionary<string, string> GetDictionary(string[] names,
        bool required = false, char[] separators = null)
    {
        IEnumerable<string> assignments = GetStrings(names, required);
        if (separators != null && separators.Length > 0)
            assignments = assignments.SelectMany(x => x.Split(separators,
                StringSplitOptions.RemoveEmptyEntries));

        var result = new Dictionary<string, string>();

        foreach (var assignment in assignments)
        {
            var eq = assignment.IndexOf('=');
            if (eq < 0)
                throw new ArgumentException($"The values for switch '{names[0]}' " +
                    $"should use 'Name=Value' format!", names[0]);

            var propName = assignment[0..eq];
            if (propName.Length == 0)
                throw new ArgumentException($"One of the keys specified for " +
                    $"switch '{names[0]}' is empty!", names[0]);

            var value = assignment[(eq + 1)..];
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

    public int Remaining => arguments.Count;
}