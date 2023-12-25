
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
        /// <returns>A dictionary of key value pairs</returns>
        /// <exception cref="ArgumentException">Keys are specified multiple times</exception>
        Dictionary<string, string> GetDictionary(string[] names, bool required = true,
            char[] separators = null);

        /// <summary>
        /// Determines the command from first argument which is not a switch
        /// </summary>
        /// <returns>Command, or null if not found</returns>
        string GetCommand();

        /// <summary>
        /// Gets the value for a switch that can only be specified once.
        /// It removes the argument from the arguments array if it is found.
        /// </summary>
        /// <param name="names">Allowed switch names</param>
        /// <param name="allowEmpty">True to allow empty values. Default is false.</param>
        /// <returns>The argument value or null if not found</returns>
        /// <exception cref="ArgumentException">The switch is specified multiple times,
        /// or its value is empty and allowEmpty is false.</exception>
        string GetString(string[] names, bool required = true);

        /// <summary>
        /// Gets the value for a switch that can be specified multiple times
        /// </summary>
        /// from this list.</param>
        /// <param name="names">Allowed switch names.</param>
        /// <param name="allowEmpty">True to allow empty values. Default is false.</param>
        /// <returns>The argument values</returns>
        string[] GetStrings(string[] names, bool required = true);

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