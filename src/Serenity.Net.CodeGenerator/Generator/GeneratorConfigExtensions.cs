using System.IO;

namespace Serenity.CodeGenerator;

/// <summary>
/// Helper methods for generator config
/// </summary>
public static class GeneratorConfigExtensions
{
    /// <summary>
    /// Returns JSON serialized version
    /// </summary>
    public static string SaveToJson(this GeneratorConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        config.Connections.Sort((x, y) => string.Compare(x.Key, y.Key, StringComparison.OrdinalIgnoreCase));
        foreach (var c in config.Connections)
            c.Tables.Sort((x, y) => string.Compare(x.Tablename, y.Tablename, StringComparison.OrdinalIgnoreCase));

        using var sw = new StringWriter();
        using var jw = new Newtonsoft.Json.JsonTextWriter(sw)
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            IndentChar = ' ',
            Indentation = 2
        };
        var serializer = Newtonsoft.Json.JsonSerializer.Create(JsonSettings.Strict);

        serializer.Serialize(jw, config);
        jw.Flush();
        return sw.ToString();
    }

    /// <summary>
    /// Returns appsettings files
    /// </summary>
    public static string[] GetAppSettingsFiles(this GeneratorConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        if (config.AppSettingFiles != null &&
            config.AppSettingFiles.Length != 0)
            return config.AppSettingFiles;

        return
        [
            "appsettings.json",
            "appsettings.machine.json"
        ];
    }

    /// <summary>
    /// Gets root namespace for given project
    /// </summary>
    /// <param name="fileSystem">File system</param>
    /// <param name="csproj">CSProj file</param>
    /// <returns>Root namespace for given project</returns>
    /// <exception cref="ArgumentNullException">fileSystem is null</exception>
    public static string GetRootNamespaceFor(this GeneratorConfig config, IProjectFileInfo projectInfo)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(projectInfo);

        if (!string.IsNullOrEmpty(config.RootNamespace))
            return config.RootNamespace;

        string rootNamespace = projectInfo.GetRootNamespace();
        rootNamespace ??= projectInfo.FileSystem.GetFileNameWithoutExtension(projectInfo.ProjectFile);

        if (rootNamespace?.EndsWith(".Web", StringComparison.OrdinalIgnoreCase) == true)
            rootNamespace = rootNamespace[0..^4];

        return rootNamespace;
    }

    /// <summary>
    /// Loads config from given file
    /// </summary>
    /// <param name="fileSystem">File system</param>
    /// <param name="path">The folder path for sergen.sjon</param>
    /// <returns>Deserialized configuration</returns>
    /// <exception cref="ArgumentNullException">fileSystem is null</exception>
    public static GeneratorConfig LoadGeneratorConfig(this IFileSystem fileSystem,
        string path, string filename = "sergen.json")
    {
        ArgumentNullException.ThrowIfNull(fileSystem);
        ArgumentNullException.ThrowIfNull(path);

        if (!string.IsNullOrEmpty(filename))
            path = fileSystem.Combine(path, filename);

        GeneratorConfig config;
        if (!fileSystem.FileExists(path))
            config = new GeneratorConfig();
        else
        {
            config = ExtendsJsonReader.Read<GeneratorConfig>(
                fileSystem, path,
                extendsProp: nameof(GeneratorConfig.Extends),
                options: JSON.Defaults.Tolerant,
                getDefault: GeneratorDefaults.TryParse);
        }

        config.Connections ??= [];
        config.RemoveForeignFields ??= [];
        return config;
    }
}
