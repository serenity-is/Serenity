using Serenity.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Serenity.Localization;

/// <summary>
/// Regenerates the JSON texts of the projects of an application: the base template of the texts of each project,
/// <c>*.texts.__.json</c>, from the local texts in its assembly, and the other languages from it, keeping the
/// translations that are there. An application derives from this class and says which feature projects it has, see
/// <see cref="GetFeatureTargets"/>, and runs it from the command line, e.g. with <c>json-texts</c>, see
/// <see cref="RunIf"/>, or from a module initializer, so that a new project can have the texts of its own.
/// </summary>
/// <remarks>
/// The template of a project is what its texts are translated from, and what the constants of its text keys are
/// written from, see the TextKeys of a project, so that a key that the code uses is a key that the application has.
/// A project that has no template, e.g. one that is not generated with this one, is not a matter for the texts of the
/// application, as the ones of another project are not, and it is skipped.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="BaseJsonTextsGenerator"/> class.
/// </remarks>
/// <param name="projectFolder">The folder of the project of the application, or null for the one that the
/// application is run from, which is the project folder of it when it is run from there, e.g. with
/// <c>dotnet run --project YourProject.Web</c>, or from its folder with <c>dotnet YourProject.Web.dll json-texts</c>.
/// </param>
public abstract class BaseJsonTextsGenerator(string? projectFolder = null)
{

    /// <summary>
    /// Gets the folder of the project of the application, e.g. "…/YourProject.Web", which the application is run from by
    /// default, as its texts are in it.
    /// </summary>
    public string ProjectFolder { get; } = projectFolder ?? Environment.CurrentDirectory;

    /// <summary>
    /// The command that runs the generator, which an application passes to <see cref="RunIf"/>.
    /// </summary>
    public const string Command = "json-texts";

    /// <summary>
    /// The comment that the template of a project starts with, which tells that nothing else in it is to be changed.
    /// </summary>
    public const string TemplateComment = "Auto generated from the assembly to use as a template for translation. " +
        "Don't modify. This file is ignored at runtime.";

    /// <summary>
    /// The settings of the JSON files, which keep the texts as they are, e.g. with the apostrophes and the letters
    /// that are not in the ASCII set, as a translator reads them.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };


    /// <summary>
    /// A project whose texts are in a folder of the source tree, e.g. the web project itself and the projects of the
    /// features that it uses.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TextsTarget"/> class.
    /// </remarks>
    /// <param name="assembly">The assembly that has the texts of the project.</param>
    /// <param name="folder">The folder of the texts of the project, which is either a path that starts from the root
    /// of a drive, e.g. "C:\Projects\MyApp\src\MyApp.Web\texts\resources", or one that starts from the folder of
    /// the web project, e.g. "texts/resources" for the one of the application itself, or
    /// "../MyFeature/texts/resources" for one of a feature project next to it.</param>
    /// <param name="prefix">The prefix of the files of the project, e.g. "site.texts", for "site.texts.__.json".</param>
    protected sealed class TextsTarget(Assembly assembly, string folder, string prefix)
    {
        /// <summary>
        /// Gets the assembly that has the texts of the project.
        /// </summary>
        public Assembly Assembly { get; } = assembly ?? throw new ArgumentNullException(nameof(assembly));

        /// <summary>
        /// Gets the folder of the texts of the project.
        /// </summary>
        public string Folder { get; } = folder;

        /// <summary>
        /// Gets the prefix of the files of the project, e.g. "site.texts".
        /// </summary>
        public string Prefix { get; } = prefix;
    }

    /// <summary>
    /// Runs the generator when the first argument is <see cref="Command"/>, as the dynamic data generator does for its
    /// own command, so that the application can exit with the code that it returns.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns><c>null</c> when <paramref name="args"/> is not the command of the generator, so the application should
    /// start normally; otherwise the exit code the caller should return: 0 on success, 1 when the generator throws,
    /// 2 for extra arguments.</returns>
    public virtual int? RunIf(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args is not [Command, ..])
            return null;

        if (args.Length > 1)
        {
            Console.Error.WriteLine("usage: <app> " + Command);
            return 2;
        }

        try
        {
            Run();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Generating the json texts failed: " + ex.Message);
            return 1;
        }

        Console.WriteLine("The json texts are generated successfully.");
        return 0;
    }

    /// <summary>
    /// Regenerates the json texts of the projects of the application, which are the ones that
    /// <see cref="GetFeatureTargets"/> and <see cref="GetSiteTarget"/> return, in the folder of the project that it is
    /// run from, see <see cref="EnsureProjectFolder"/>.
    /// </summary>
    public virtual void Run()
    {
        EnsureProjectFolder();

        // a folder that does not start from the root of a drive starts from the folder of the web project, which the
        // texts of a feature project next to it are under, e.g. "../MyFeature/texts/resources"
        string fullFolder(string folder) => Path.GetFullPath(folder, ProjectFolder);

        // the texts of the features are written before the ones of the application itself, as a key that it has as
        // well is its own, e.g. the title of a page that a feature project and the application both have
        var siteTarget = GetSiteTarget();
        var targets = GetFeatureTargets()
            .Concat([siteTarget])
            .Select(x => new TextsTarget(x.Assembly, fullFolder(x.Folder), x.Prefix))
            .ToList();
        var userTargets = GetUserTargets()
            .Select(x => (Folder: fullFolder(x.Folder), x.Prefix))
            .ToList();

        var visitedKeys = new HashSet<string>();
        var fallbackTranslation = new Dictionary<(string, string), string>();

        HashSet<string> allLanguageIds = [];
        foreach (var (folder, prefix) in targets.Select(x => (x.Folder, x.Prefix))
            .Concat(userTargets))
        {
            if (!Directory.Exists(folder))
                continue;

            foreach (var jsonPath in Directory.GetFiles(folder, prefix + ".*.json"))
            {
                var langID = JsonLocalTextRegistration.ParseLanguageIdFromPath(jsonPath);
                if (string.IsNullOrEmpty(langID))
                    continue;

                allLanguageIds.Add(langID);

                var langTexts = JSON.Parse<SortedDictionary<string, string?>>(File.ReadAllText(jsonPath).Trim());
                foreach (var langText in langTexts!)
                {
                    if (!string.IsNullOrEmpty(langText.Value))
                        fallbackTranslation[(langText.Key, langID)] = langText.Value;
                }
            }
        }

        // the texts of the previous run, to tell what the translations of the keys that are gone were for
        var oldTexts = new Dictionary<string, string>();
        foreach (var target in targets)
        {
            var oldBaseFile = Path.Combine(target.Folder, target.Prefix + ".__.json");
            if (!File.Exists(oldBaseFile))
                continue;

            foreach (var (key, value) in JSON.Parse<SortedDictionary<string, string?>>(
                File.ReadAllText(oldBaseFile).Trim())!)
            {
                if (key != "__comment__" && !string.IsNullOrEmpty(value))
                    oldTexts[key] = value;
            }
        }

        // the texts of the current assemblies, which is done before anything is written, so what is removed can be
        // looked at
        var currentTexts = new List<SortedDictionary<string, string>>();
        foreach (var target in targets)
        {
            var registry = new LocalTextRegistry();
            var typeSource = new DefaultTypeSource([target.Assembly]);
            registry.AddBaseTexts(typeSource);

            var baseTexts = new SortedDictionary<string, string>();
            foreach (var text in registry.GetAllTexts(false))
                if (text.Key.LanguageId == LocalText.InvariantLanguageID)
                    baseTexts[text.Key.Key] = text.Value!;

            foreach (var key in visitedKeys)
                baseTexts.Remove(key);

            visitedKeys.AddRange(baseTexts.Keys);
            currentTexts.Add(baseTexts);
        }

        var currentKeys = new HashSet<string>(currentTexts.SelectMany(x => x.Keys));

        // the translations of the keys that are not there anymore, e.g. renamed ones, by the language and the text they
        // were for, so a key that has no translation can get the one of the key that it replaced. They are looked at
        // before the files are rewritten, which removes them.
        var orphans = new Dictionary<(string Language, string Text), List<(string Key, string Translation)>>();
        foreach (var (folder, prefix) in targets.Select(x => (x.Folder, x.Prefix)).Concat(userTargets))
        {
            if (!Directory.Exists(folder))
                continue;

            foreach (var jsonPath in Directory.GetFiles(folder, prefix + ".*.json"))
            {
                var langID = JsonLocalTextRegistration.ParseLanguageIdFromPath(jsonPath);
                if (string.IsNullOrEmpty(langID))
                    continue;

                foreach (var (key, value) in JSON.Parse<SortedDictionary<string, string?>>(
                    File.ReadAllText(jsonPath).Trim())!)
                {
                    if (string.IsNullOrEmpty(value) || currentKeys.Contains(key) ||
                        !oldTexts.TryGetValue(key, out var oldText))
                        continue;

                    if (!orphans.TryGetValue((langID, oldText), out var list))
                        orphans[(langID, oldText)] = list = [];

                    list.Add((key, value));
                }
            }
        }

        static string lastPart(string key) => key[(key.LastIndexOf('.') + 1)..];

        // Finds the translation of a key that replaced another with the same text. When several keys had it, the ones
        // ending with the same name are preferred, and nothing is taken when they were translated in different ways.
        string? findRenamed(string key, string langID, string text)
        {
            if (!orphans.TryGetValue((langID, text), out var list))
                return null;

            var same = list.Where(x => lastPart(x.Key) == lastPart(key)).ToList();
            var translations = (same.Count > 0 ? same : list).Select(x => x.Translation).Distinct().ToList();
            return translations.Count == 1 ? translations[0] : null;
        }

        var staleCount = 0;

        void processTarget(TextsTarget target, SortedDictionary<string, string> baseTexts)
        {
            // the translations by language after the update, to tell which of the out of date ones were updated
            var translationsByLanguage = new Dictionary<string, SortedDictionary<string, string?>>();

            var baseTextsFile = Path.Combine(target.Folder, target.Prefix + ".__.json");
            Directory.CreateDirectory(target.Folder);

            // a language that a project has no file for is not one that its texts are translated to, for the
            // application itself, as it supports the languages that its own texts have, e.g. not every one of the
            // framework that it uses, while a feature project has the ones that it is translated to
            // the application itself is the last of the projects, see the one that the texts are written from
            var createLanguageFiles = target != targets[^1];
            if (createLanguageFiles && Directory.GetFiles(target.Folder, target.Prefix + ".*.json").Length > 0)
            {
                foreach (var languageID in allLanguageIds)
                {
                    var filename = Path.Combine(target.Folder, target.Prefix + "." + languageID + ".json");
                    if (!File.Exists(filename))
                        File.WriteAllText(filename, "{}");
                }
            }

            foreach (var jsonPath in Directory.GetFiles(target.Folder, target.Prefix + ".*.json"))
            {
                var langID = JsonLocalTextRegistration.ParseLanguageIdFromPath(jsonPath);

                // the template and the list of the out of date translations are not for a language, and the latter is
                // not a dictionary of texts, so it is skipped before it is parsed
                if (langID is null)
                    continue;

                var langTexts = JSON.Parse<SortedDictionary<string, string?>>(File.ReadAllText(jsonPath).Trim())!;

                foreach (var key in langTexts.Keys.ToArray())
                {
                    if (!baseTexts.TryGetValue(key, out var baseText))
                        langTexts.Remove(key);
                    else if (langTexts.TryGetValue(key, out var langText) && langText != null &&
                        ShouldResetTranslation(key, langText, baseText))
                        langTexts[key] = null;
                }

                foreach (var key in baseTexts.Keys)
                {
                    if (!langTexts.TryGetValue(key, out var value) || value == null)
                    {
                        if (!string.IsNullOrEmpty(langID) &&
                            fallbackTranslation.TryGetValue((key, langID), out var translation))
                            langTexts[key] = translation;
                        else if (!string.IsNullOrEmpty(langID) && baseTexts[key] is string text &&
                            findRenamed(key, langID, text) is string renamed &&
                            Enumerable.Range(0, 3).All(n => !text.Contains("{" + n + "}") || renamed.Contains("{" + n + "}")))
                            langTexts[key] = renamed;
                        else
                            langTexts[key] = null;
                    }
                }

                File.WriteAllText(jsonPath, JsonSerializer.Serialize(langTexts, JsonOptions).Replace("\r", ""));

                if (langID.Length > 0)
                    translationsByLanguage[langID] = langTexts;
            }

            staleCount += UpdateOutOfDateTranslations(Path.Combine(target.Folder, target.Prefix + ".__.stale.json"),
                baseTexts, oldTexts, translationsByLanguage);

            baseTexts["__comment__"] = TemplateComment;
            File.WriteAllText(baseTextsFile, JsonSerializer.Serialize(baseTexts, JsonOptions).Replace("\r", ""));
        }

        for (var i = 0; i < targets.Count; i++)
            processTarget(targets[i], currentTexts[i]);

        if (staleCount > 0)
            Console.WriteLine(staleCount + " translations are out of date, as the text of their key changed, see the " +
                "*.texts.__.stale.json files.");
    }

    /// <summary>
    /// Whether a translation of a key is to be reset, as it can not be what the text of the key needs, e.g. one that
    /// lost the placeholders of it, which would show a text without them.
    /// </summary>
    /// <param name="key">The key of the text.</param>
    /// <param name="translation">The translation that is there.</param>
    /// <param name="text">The text of the template that it is a translation of.</param>
    /// <returns>True to reset the translation, so that it is translated again.</returns>
    protected virtual bool ShouldResetTranslation(string key, string translation, string? text)
    {
        // a translation that lost the placeholders of the text would show them wrong
        for (var i = 0; i < 3; i++)
        {
            var placeholder = "{" + i + "}";
            if (text?.Contains(placeholder, StringComparison.Ordinal) == true &&
                !translation.Contains(placeholder, StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Checks that the folder that the application is run from is the folder of its project, which the generator
    /// assumes, as it writes the texts of the application there, and throws when it is not, rather than write a folder
    /// of texts somewhere else.
    /// </summary>
    /// <exception cref="InvalidOperationException">The folder is not the one of the project of the application.</exception>
    protected virtual void EnsureProjectFolder()
    {
        if (!File.Exists(Path.Combine(ProjectFolder, "appsettings.json")) ||
            !Directory.Exists(Path.Combine(ProjectFolder, "texts", "resources")))
        {
            throw new InvalidOperationException("The folder " + ProjectFolder + " does not look like the project " +
                "folder of an application, as it has no appsettings.json in it, or no texts/resources folder under " +
                "it. Run the generator from that folder, e.g. with 'dotnet run --project YourProject.Web'.");
        }
    }

    /// <summary>
    /// Gets the project of the application itself, which is the one that the generator is in, and whose texts are
    /// written to its "texts/resources" folder, as "site.texts", e.g. "site.texts.__.json", which is what the texts of
    /// an application are translated from.
    /// </summary>
    /// <returns>The project of the application itself.</returns>
    protected virtual TextsTarget GetSiteTarget()
    {
        // the assembly of the application is the one that this generator is in, as it is a file of the project
        return new(GetType().Assembly, Path.Combine("texts", "resources"), "site.texts");
    }

    /// <summary>
    /// Gets the projects of the features that the application uses, whose texts are written along with the ones of the
    /// application itself, before them, so that a text that the application has as well is its own. An application that
    /// has no such projects, e.g. one that uses them as packages, which have their templates in them, has none.
    /// </summary>
    /// <returns>The projects of the features, whose folders are either paths that start from the root of a drive, or
    /// ones that start from the folder of the web project, e.g. "../MyFeature/texts/resources".</returns>
    protected virtual IEnumerable<TextsTarget> GetFeatureTargets()
    {
        return [];
    }

    /// <summary>
    /// Gets the folders of the texts that an application has in it, e.g. the ones that its users add through the
    /// translation page, which are read for the translations of a key, but are not written by the generator. It is the
    /// "App_Data/texts" folder of the project of the application, which is where the translation page puts them.
    /// </summary>
    /// <returns>The folders, with the prefix of their files, e.g. "user.texts", which are either paths that start from
    /// the root of a drive, or ones that start from the folder of the web project, see <see cref="TextsTarget"/>.
    /// </returns>
    protected virtual IEnumerable<(string Folder, string Prefix)> GetUserTargets()
    {
        // the folder that the application loads the texts of its users from, e.g. the ones that its translation page
        // saves, which are read for the translations of a key, but are not written by the generator
        yield return (Path.Combine("App_Data", "texts"), "user.texts");
    }

    /// <summary>
    /// Keeps the list of the translations that are out of date, as the text of their key changed after they were made,
    /// in a file next to the texts of the project, which is ignored at runtime like the template. A translation is kept
    /// in the language files as it is, as an old one is still better than the one of the template, and it is listed
    /// with the text it was made for until someone changes it. An entry is removed when the translation is changed or
    /// emptied, the text is back to the old one, or the key is gone, and the file is removed when nothing is left.
    /// </summary>
    /// <param name="stalePath">The path of the file.</param>
    /// <param name="baseTexts">The current texts of the template.</param>
    /// <param name="oldTexts">The texts of the template of the previous run.</param>
    /// <param name="translations">The translations after the update, by language.</param>
    /// <returns>The number of translations that are out of date.</returns>
    public static int UpdateOutOfDateTranslations(string stalePath, SortedDictionary<string, string> baseTexts,
        Dictionary<string, string> oldTexts, Dictionary<string, SortedDictionary<string, string?>> translations)
    {
        ArgumentNullException.ThrowIfNull(stalePath);
        ArgumentNullException.ThrowIfNull(baseTexts);
        ArgumentNullException.ThrowIfNull(oldTexts);
        ArgumentNullException.ThrowIfNull(translations);

        var stale = File.Exists(stalePath) ? JsonNode.Parse(File.ReadAllText(stalePath))!.AsObject() : [];

        foreach (var property in stale.ToList())
            if (!baseTexts.ContainsKey(property.Key))
                stale.Remove(property.Key);

        foreach (var (key, text) in baseTexts)
        {
            var entry = stale[key] as JsonObject;
            if (entry is not null)
            {
                // the text went back to the one the translations were made for
                if ((string?)entry["old"] == text)
                {
                    stale.Remove(key);
                    continue;
                }

                entry["new"] = text;
                var recorded = (JsonObject)entry["translations"]!;
                foreach (var language in recorded.ToList())
                {
                    if (!translations.TryGetValue(language.Key, out var texts) ||
                        !texts.TryGetValue(key, out var current) || current != (string?)language.Value)
                        recorded.Remove(language.Key);
                }

                if (recorded.Count == 0)
                {
                    stale.Remove(key);
                    entry = null;
                }
            }

            if (!oldTexts.TryGetValue(key, out var old) || old == text)
                continue;

            // changed since the previous run, so the translations that exist now were made for the old text
            entry ??= new JsonObject { ["old"] = old, ["new"] = text, ["translations"] = new JsonObject() };
            var forLanguages = (JsonObject)entry["translations"]!;
            foreach (var (language, texts) in translations)
            {
                if (texts.TryGetValue(key, out var translation) && !string.IsNullOrEmpty(translation) &&
                    !forLanguages.ContainsKey(language))
                    forLanguages[language] = translation;
            }

            if (forLanguages.Count > 0)
                stale[key] = entry;
        }

        if (stale.Count == 0)
        {
            if (File.Exists(stalePath))
                File.Delete(stalePath);
            return 0;
        }

        // the nodes of the ones that are written are detached from the object that they are in, as a node can only be
        // in one of them, and the file keeps the keys in the order of their names
        var sorted = new JsonObject();
        foreach (var (key, value) in stale.OrderBy(x => x.Key, StringComparer.Ordinal).ToList())
        {
            stale.Remove(key);
            sorted[key] = value;
        }

        File.WriteAllText(stalePath, JsonSerializer.Serialize(sorted, JsonOptions).Replace("\r", "") + "\n");
        return sorted.Sum(x => x.Value is JsonObject entry ? ((JsonObject)entry["translations"]!).Count : 0);
    }
}
