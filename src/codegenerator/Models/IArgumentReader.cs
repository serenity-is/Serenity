using System.Collections.Generic;
#nullable enable

namespace Serenity.CodeGenerator
{
    public interface IArgumentReader
    {
        /// <summary>
        /// Gets the value as a dictionary for a switch, whose values are also in the name=value 
        /// format.
        /// </summary>
        /// <param name="names"></param>
        /// <param name="separators">Splits switch values by specified chars, so for example 
        /// providing the value in /param:A=B;C=D separated format is possible</param>
        /// <param name="requiresValue">False to allow empty values. Default is true.</param>
        /// <returns>A dictionary of key value pairs</returns>
        /// <exception cref="ArgumentException">Keys are specified multiple times</exception>
        Dictionary<string, string> GetDictionary(string[] names, bool requiresValue = true,
            char[]? separators = null);

        /// <summary>
        /// Determines the command from first argument which is not a switch
        /// </summary>
        /// <returns>Command, or null if not found</returns>
        string? GetCommand();

        /// <summary>
        /// Gets the value of a true/false switch.
        /// </summary>
        bool? GetBoolean(string[] names);

        /// <summary>
        /// Gets the value for a switch that can only be specified once.
        /// It removes the argument from the arguments array if it is found.
        /// </summary>
        /// <param name="names">Allowed switch names</param>
        /// <param name="requiresValue">False to allow empty values. Default is true.</param>
        /// <returns>The argument value or null if not found</returns>
        /// <exception cref="ArgumentException">The switch is specified multiple times,
        /// or its value is empty and <see cref="requiresValue" /> is true.</exception>
        string? GetString(string[] names, bool requiresValue = true);

        /// <summary>
        /// Gets the value for a switch that can be specified multiple times
        /// from this list.
        /// </summary>
        /// <param name="names">Allowed switch names.</param>
        /// <param name="requiresValue">False to allow empty values. Default is true.</param>
        /// <returns>The argument values</returns>
        string[] GetStrings(string[] names, bool requiresValue = true);

        /// <summary>
        /// Gets the value of a switch that may be used without one, such as
        /// "--overwrite" or "--overwrite:yes". The switch is removed if present.
        /// </summary>
        /// <param name="names">Allowed switch names</param>
        /// <returns>null when the switch is omitted; an empty string when it is used
        /// without a value; the value otherwise.</returns>
        string? GetOptionalString(string[] names);

        /// <summary>
        /// Removes and returns true when any of the specified switches is present.
        /// Switches of this kind take no value, e.g. "--no-interactive".
        /// </summary>
        /// <param name="names">Allowed switch names</param>
        /// <returns>True if one of the switches was specified</returns>
        /// <exception cref="ArgumentException">A switch is given a value</exception>
        bool HasSwitch(string[] names);

        /// <summary>
        /// Returns true if any of switches are -?, --help, or -h
        /// </summary>
        bool HasHelpSwitch();

        /// <summary>
        /// Gets remaining argument count
        /// </summary>
        int Remaining { get; }

        /// <summary>
        /// Throws argument null if arguments is not empty
        /// </summary>
        /// <exception cref="ArgumentException">Remaining argument count > 0</exception>
        void ThrowIfRemaining();
    }
}