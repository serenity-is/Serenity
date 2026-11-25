namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    /// <summary>
    /// A generated texts file, containing a single proxyTexts export for a set of keys.
    /// </summary>
    /// <param name="name">Export name, e.g. LoginFormTexts or AdministrationDbTexts</param>
    /// <param name="prefix">Local text prefix, e.g. "Db.Administration.", empty for unprefixed classes,
    /// which contain the whole tree</param>
    /// <param name="filePath">Generated file path without extension, e.g. Administration/DbTexts</param>
    /// <param name="keys">Sorted (ordinal) local text keys in this bundle</param>
    /// <param name="isNestedClass">True if the bundle is generated for a NestedLocalTexts class</param>
    protected class TextsBundle(string name, string prefix, string filePath, List<string> keys, bool isNestedClass)
    {
        /// <summary>Export name, e.g. LoginFormTexts or AdministrationDbTexts</summary>
        public string Name { get; } = name;

        /// <summary>Local text prefix, e.g. "Db.Administration.", empty for unprefixed classes</summary>
        public string Prefix { get; } = prefix;

        /// <summary>Generated file path without extension, e.g. Administration/DbTexts</summary>
        public string FilePath { get; } = filePath;

        /// <summary>Sorted (ordinal) local text keys in this bundle</summary>
        public List<string> Keys { get; } = keys;

        /// <summary>True if the bundle is generated for a NestedLocalTexts class</summary>
        public bool IsNestedClass { get; } = isNestedClass;

        /// <summary>
        /// Returns true if the key is part of this bundle, so that it is not included in
        /// module Db texts or remaining texts. Unprefixed bundles are aliases for the
        /// whole tree, so they don't cover any key.
        /// </summary>
        public bool Covers(string key)
        {
            return Prefix.Length > 0 &&
                key.StartsWith(Prefix, StringComparison.Ordinal);
        }
    }
}
