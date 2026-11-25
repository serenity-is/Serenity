namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    /// <summary>
    /// Information about a class with NestedLocalTexts attribute, whose name ends with Texts.
    /// </summary>
    /// <param name="name">Class name, which is also the name of the generated export</param>
    /// <param name="prefix">Local text prefix, empty string if none</param>
    /// <param name="ns">Script namespace the class maps to</param>
    protected class NestedLocalTextsClass(string name, string prefix, string? ns)
    {
        /// <summary>Class name, which is also the name of the generated export</summary>
        public string Name { get; } = name;

        /// <summary>Local text prefix, empty string if none</summary>
        public string Prefix { get; } = prefix;

        /// <summary>Script namespace the class maps to</summary>
        public string? Namespace { get; } = ns;
    }
}
