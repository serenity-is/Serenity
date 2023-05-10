using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System;

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
        if (config is null)
            throw new ArgumentNullException(nameof(config));

        config.Connections.Sort((x, y) => string.Compare(x.Key, y.Key, StringComparison.OrdinalIgnoreCase));
        foreach (var c in config.Connections)
            c.Tables.Sort((x, y) => string.Compare(x.Tablename, y.Tablename, StringComparison.OrdinalIgnoreCase));

        return JSON.StringifyIndented(config, 2);
    }

    /// <summary>
    /// Returns appsettings files
    /// </summary>
    public static string[] GetAppSettingsFiles(this GeneratorConfig config)
    {
        if (config is null)
            throw new ArgumentNullException(nameof(config));

        if (config.AppSettingFiles != null &&
            config.AppSettingFiles.Length != 0)
            return config.AppSettingFiles;

        return new string[]
        {
            "appsettings.json",
            "appsettings.machine.json"
        };
    }

    /// <summary>
    /// Gets root namespace for given project
    /// </summary>
    /// <param name="fileSystem">File system</param>
    /// <param name="csproj">CSProj file</param>
    /// <returns>Root namespace for given project</returns>
    /// <exception cref="ArgumentNullException">fileSystem is null</exception>
    public static string GetRootNamespaceFor(this GeneratorConfig config, IGeneratorFileSystem fileSystem, string csproj)
    {
        if (config is null)
            throw new ArgumentNullException(nameof(config));

        if (fileSystem is null)
            throw new ArgumentNullException(nameof(fileSystem));

        if (!string.IsNullOrEmpty(config.RootNamespace))
            return config.RootNamespace;

        string rootNamespace = null;

        if (fileSystem.FileExists(csproj)) {
             rootNamespace = ProjectFileHelper.ExtractPropertyFrom(fileSystem, csproj,
                xe => xe.Descendants("RootNamespace").FirstOrDefault()?.Value.TrimToNull());
        }

        rootNamespace ??= fileSystem.ChangeExtension(fileSystem.GetFileName(csproj), null);

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
        if (fileSystem is null)
            throw new ArgumentNullException(nameof(fileSystem));

        if (path is null)
            throw new ArgumentNullException(nameof(path));

        if (!string.IsNullOrEmpty(filename))
            path = System.IO.Path.Combine(path, filename);

        GeneratorConfig config;
        if (!fileSystem.FileExists(path))
            config = new GeneratorConfig();
        else
        {
            config = ExtendsJsonReader.Read<GeneratorConfig>(
                fileSystem, path,
                extendsProp: nameof(GeneratorConfig.Extends),
                options: new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters =
                    {
                        new JsonStringEnumConverter()
                    }
                },
                getDefault: GeneratorDefaults.TryParse);
        }

        config.Connections ??= new();
        config.RemoveForeignFields ??= new List<string>();
        return config;
    }
}
