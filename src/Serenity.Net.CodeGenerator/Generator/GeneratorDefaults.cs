namespace Serenity.CodeGenerator;

/// <summary>
/// Contains list of default versions that can be used in "extends"
/// setting like "defaults@6.6.0" in sergen.json to apply a specific
/// set of defaults.
/// </summary>
public class GeneratorDefaults
{
    private static readonly Dictionary<Version, string> ByVersion = new()
    {
        [new(6, 6, 0)] = v6_6_0,
    };

    private const string v6_6_0 = /*lang=json*/ @"{
  ""DeclareJoinConstants"": true,
  ""EndOfLine"": ""LF"",
  ""FileScopedNamespaces"": true,
  ""ForeignFieldSelection"": ""NameOnly"",
  ""OmitDefaultSchema"": true,
  ""SaveGeneratedTables"": false,
  ""Restore"": {
    ""Exclude"": [
      ""**/*""
    ],
    ""Typings"": false
  },
  ""ServerTypings"": {
    ""LocalTexts"": true
  }
}";

    /// <summary>
    /// Tries to parse "default@6.6.0" type of extends
    /// version
    /// </summary>
    /// <param name="extends">Extends statement like "defaults@6.6.0"</param>
    /// <returns></returns>
    public static string TryParse(string extends)
    {
        if (string.IsNullOrEmpty(extends) ||
            !extends.StartsWith("defaults@", StringComparison.Ordinal) ||
            !Version.TryParse(extends[9..], out Version version))
            return null;

        if (!ByVersion.TryGetValue(version, out string json))
            throw new ArgumentOutOfRangeException(
                $"Can't locate sergen defaults for version {version}!");

        return json;
    }
}
