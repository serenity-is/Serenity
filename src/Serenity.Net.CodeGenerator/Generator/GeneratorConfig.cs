using Newtonsoft.Json.Linq;

namespace Serenity.CodeGenerator
{
    /// <summary>
    /// Sergen (or Stargen) and Source Generator Configuration
    /// that is usually stored in sergen.json in project directory
    /// </summary>
    public class GeneratorConfig
    {
        /// <summary>
        /// The root namespace for the target project. It is recommended
        /// to have this in sergen.json. If not there, it will be auto calculated
        /// from root namespace in CSPROJ file, or derived from project file name
        /// by removing .Web suffix.
        /// 
        /// Generated code will be placed under this namespace and project's own
        /// types are assumed to be under this namespace as well.
        /// </summary>
        public string RootNamespace { get; set; }
        
        /// <summary>
        /// Server typings code generation related configuration
        /// </summary>
        public ServerTypingsConfig ServerTypings { get; set; }

        /// <summary>
        /// End of line style. Can be "lf" or "crlf", default is based on 
        /// environment, e.g. for Windows CRLF, for Linux LF. It is recommended
        /// to have "lf" for multi platform projects.
        /// </summary>
        public string EndOfLine { get; set; }

        /// <summary>Used for Newtonsoft.JSON</summary>
        public bool ShouldSerializeEndOfLine() => EndOfLine != null;

        /// <summary>
        /// Client types code generation related configuration
        /// </summary>
        public ClientTypesConfig ClientTypes { get; set; }

        /// <summary>Used for Newtonsoft.JSON</summary>
        public bool ShouldSerializeClientTypes() => ClientTypes != null &&
            !string.IsNullOrEmpty(ClientTypes.OutDir);

        /// <summary>
        /// MVC (e.g. view locations) related configuration
        /// </summary>
        public MVCConfig MVC { get; set; }

        /// <summary>Used for Newtonsoft.JSON</summary>
        public bool ShouldSerializeMVC() => MVC != null &&
            (!string.IsNullOrEmpty(MVC.OutDir) ||
             MVC.UseRootNamespace != null ||
             MVC.SearchViewPaths?.Any() == true ||
             MVC.StripViewPaths?.Any() == true);

        /// <summary>
        /// Sergen restore command related configuration
        /// </summary>
        public RestoreConfig Restore { get; set; }
        /// <summary>Used for Newtonsoft.JSON</summary>
        public bool ShouldSerializeRestore() => Restore != null &&
            (!Restore.Include.IsEmptyOrNull() ||
             !Restore.Exclude.IsEmptyOrNull());

        /// <summary>
        /// List of connections. Is only needed when it is desired to 
        /// work / generated code based on a connection string that 
        /// is not in appsettings.json
        /// </summary>
        public List<Connection> Connections { get; set; }

        /// <summary>Used for Newtonsoft.JSON</summary>
        public bool ShouldSerializeConnections() => Connections != null && Connections.Count > 0;
       
        /// <summary>
        /// Full path of KDIFF3. This is no longer used.
        /// </summary>
        public string KDiff3Path { get; set; }
        /// <summary>Used for Newtonsoft.JSON</summary>
        public bool ShouldSerializeKDiff3Path() => !string.IsNullOrEmpty(KDiff3Path);

        /// <summary>
        /// Full path to TypeScript compiler. It is assumed to be in path
        /// if not specified.
        /// </summary>
        public string TSCPath { get; set; }
        /// <summary>Used for Newtonsoft.JSON</summary>
        public bool ShouldSerializeTSCPath() => !string.IsNullOrEmpty(TSCPath);

        /// <summary>
        /// List of base row classes to be used instead of Row.
        /// They are matched by list of base properties.
        /// </summary>
        public List<BaseRowClass> BaseRowClasses { get; set; }
        /// <summary>Used for Newtonsoft.JSON</summary>
        public bool ShouldSerializeBaseRowClasses() =>
            BaseRowClasses != null && BaseRowClasses.Any();

        /// <summary>
        /// A list of foreign fields to omit from generated code.
        /// This could be used to disable generating code for join fields 
        /// like CreatedBy, ModifiedBy etc, so properties like CustomerCreatedBy,
        /// CustomerModifiedBy etc. won't be generated in OrderRow.
        /// </summary>
        public List<string> RemoveForeignFields { get; set; }
        /// <summary>Used for Newtonsoft.JSON</summary>
        public bool ShouldSerializeRemoveForeignFields() =>
            RemoveForeignFields != null && RemoveForeignFields.Any();

        /// <summary>
        /// The location of custom templates folder. The files in this folder
        /// will override the default scriban templates in Sergen.
        /// Their names must match the names at https://github.com/serenity-is/Serenity/tree/master/src/Serenity.Net.CodeGenerator/Templates
        /// You may also include additional files to be generated
        /// </summary>
        public string CustomTemplates { get; set; }
        /// <summary>Used for Newtonsoft.JSON</summary>
        public bool ShouldSerializeCustomTemplates() =>
            !string.IsNullOrEmpty(CustomTemplates);

        /// <summary>
        /// The relative paths of custom generated files
        /// </summary>
        public Dictionary<string, string> CustomGenerate { get; set; }
        /// <summary>Used for Newtonsoft.JSON</summary>
        public bool ShouldSerializeCustomGenerate() =>
            CustomGenerate != null && CustomGenerate.Any();

        /// <summary>
        /// Custom settings to be passed to and used in custom templates
        /// </summary>
        public Dictionary<string, object> CustomSettings { get; set; }
        /// <summary>Used for Newtonsoft.JSON</summary>
        public bool ShouldSerializeCustomSettings() =>
            CustomSettings != null && CustomSettings.Any();

        /// <summary>
        /// List of appsettings.json files in order.
        /// Default is appsettings.json, appsettings.machine.json
        /// </summary>
        public string[] AppSettingFiles { get; set; }
        /// <summary>Used for Newtonsoft.JSON</summary>
        public bool ShouldSerializeAppSettingFiles() =>
            AppSettingFiles != null && AppSettingFiles.Any();

        /// <summary>
        /// Generate row class
        /// </summary>
        [JsonIgnore]
        public bool GenerateRow { get; set; }
        /// <summary>
        /// Generate service classes like repository, endpoint, service.ts etc.
        /// </summary>
        [JsonIgnore]
        public bool GenerateService { get; set; }
        /// <summary>
        /// Generate UI related classes like Grid/Dialog
        /// </summary>
        [JsonIgnore]
        public bool GenerateUI { get; set; }
        /// <summary>
        /// Generate custom code (user defined templates at CustomTemplates path)
        /// </summary>
        [JsonIgnore]
        public bool GenerateCustom { get; set; }

        /// <summary>
        /// Upgrade / migration related information used by stargen to determine
        /// the base version of template used, and already applied templates.
        /// </summary>
        public UpgradeInformation UpgradeInfo { get; set; }

        /// <summary>
        /// Holds extension data if any
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> ExtensionData { get; set; }

        public GeneratorConfig()
        {
            Connections = new List<Connection>();
            BaseRowClasses = new List<BaseRowClass>();
            CustomSettings = new Dictionary<string, object>();
            CustomGenerate = new Dictionary<string, string>();
            GenerateRow = true;
            GenerateService = true;
            GenerateUI = true;
            GenerateCustom = true;
        }

        /// <summary>
        /// Returns JSON serialized version
        /// </summary>
        public string SaveToJson()
        {
            Connections.Sort((x, y) => string.Compare(x.Key, y.Key, StringComparison.OrdinalIgnoreCase));
            foreach (var c in Connections)
                c.Tables.Sort((x, y) => string.Compare(x.Tablename, y.Tablename, StringComparison.OrdinalIgnoreCase));

            return JSON.StringifyIndented(this, 2);
        }

        /// <summary>
        /// Returns appsettings files
        /// </summary>
        public string[] GetAppSettingsFiles()
        {
            if (AppSettingFiles != null &&
                AppSettingFiles.Length != 0)
                return AppSettingFiles;

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
        public string GetRootNamespaceFor(IGeneratorFileSystem fileSystem, string csproj)
        {
            if (fileSystem is null)
                throw new ArgumentNullException(nameof(fileSystem));

            if (!string.IsNullOrEmpty(RootNamespace))
                return RootNamespace;

            string rootNamespace = null;

            if (fileSystem.FileExists(csproj)) {
                 rootNamespace = ProjectFileHelper.ExtractPropertyFrom(fileSystem, csproj,
                    xe => xe.Descendants("RootNamespace").FirstOrDefault()?.Value.TrimToNull());
            }

            if (rootNamespace == null)
                rootNamespace = fileSystem.ChangeExtension(fileSystem.GetFileName(csproj), null);

            if (rootNamespace?.EndsWith(".Web", StringComparison.OrdinalIgnoreCase) == true)
                rootNamespace = rootNamespace[0..^4];

            return rootNamespace;
        }

        /// <summary>
        /// Loads config from given file
        /// </summary>
        /// <param name="fileSystem">File system</param>
        /// <param name="sergenJson">Sergen.json path</param>
        /// <returns>Deserialized configuration</returns>
        /// <exception cref="ArgumentNullException">fileSystem is null</exception>
        public static GeneratorConfig LoadFromFile(IGeneratorFileSystem fileSystem,
            string sergenJson)
        {
            if (fileSystem is null)
                throw new ArgumentNullException(nameof(fileSystem));

            if (!fileSystem.FileExists(sergenJson))
                return LoadFromJson(null);

            return LoadFromJson(fileSystem.ReadAllText(sergenJson));
        }

        /// <summary>
        /// Loads config from a json string
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <returns>Deserialized config</returns>
        public static GeneratorConfig LoadFromJson(string json)
        {
            var config = System.Text.Json.JsonSerializer.Deserialize<GeneratorConfig>(
                json.TrimToNull() ?? "{}", new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            config.Connections ??= new List<Connection>();
            config.RemoveForeignFields ??= new List<string>();
            return config;
        }

        /// <summary>
        /// Holds information about a connection string
        /// </summary>
        public class Connection
        {
            /// <summary>
            /// Key of connection
            /// </summary>
            public string Key { get; set; }
            /// <summary>
            /// Connection string
            /// </summary>
            public string ConnectionString { get; set; }
            /// <summary>
            /// Provider name
            /// </summary>
            public string ProviderName { get; set; }
            /// <summary>
            /// Dialect
            /// </summary>
            public string Dialect { get; set; }
            /// <summary>
            /// Generated table information
            /// </summary>
            public List<Table> Tables { get; set; }

            /// <summary>
            /// Creates a new instance
            /// </summary>
            public Connection()
            {
                Tables = new List<Table>();
            }

            /// <summary>
            /// String representation
            /// </summary>
            public override string ToString()
            {
                return Key + " [" + ConnectionString + "], " + ProviderName;
            }
        }

        /// <summary>
        /// Holds info about an already code generated table
        /// </summary>
        public class Table
        {
            /// <summary>
            /// Table name
            /// </summary>
            public string Tablename { get; set; }
            /// <summary>
            /// Row identifier without Row suffix
            /// </summary>
            public string Identifier { get; set; }
            /// <summary>
            /// Module name
            /// </summary>
            public string Module { get; set; }
            /// <summary>
            /// Permission key
            /// </summary>
            public string PermissionKey { get; set; }
        }
        
        /// <summary>
        /// Defines info about a custom base row class
        /// </summary>
        public class BaseRowClass
        {
            /// <summary>
            /// Class name
            /// </summary>
            public string ClassName { get; set; }
            /// <summary>
            /// List of fields that should exists in a table for this base class to be used
            /// </summary>
            public List<string> Fields { get; set; }
        }

        /// <summary>
        /// Server typings related configuration
        /// </summary>
        public class ServerTypingsConfig
        {
            /// <summary>
            /// Assemblies for server typings generation.
            /// Specify only if Sergen can't auto determine
            /// your project output file.
            /// </summary>
            public string[] Assemblies { get; set; }
            /// <summary>Used for Newtonsoft.JSON</summary>
            public bool ShouldSerializeAssemblies() => Assemblies != null && Assemblies.Length > 0;

            /// <summary>
            /// Output directory for server typings generated files
            /// </summary>
            public string OutDir { get; set; }
            /// <summary>Used for Newtonsoft.JSON</summary>
            public bool ShouldSerializeOutDir() => !string.IsNullOrEmpty(OutDir);

            /// <summary>
            /// Generate local texts
            /// </summary>
            public bool LocalTexts { get; set; }

            /// <summary>
            /// Generate module typings. Defaults to true if you
            /// have "module" defined in tsconfig.json
            /// </summary>
            public bool? ModuleTypings { get; set; }
            /// <summary>Used for Newtonsoft.JSON</summary>
            public bool ShouldSerializeModuleTypings() => ModuleTypings != null;

            /// <summary>
            /// Generate namespace typings. Defaults to true if you
            /// don't have "module" defined in tsconfig.json
            /// </summary>
            public bool? NamespaceTypings { get; set; }
            /// <summary>Used for Newtonsoft.JSON</summary>
            public bool ShouldSerializeNamespaceTypings() => NamespaceTypings != null;

            /// <summary>
            /// Disable source generator (Serenity.Pro.Coder) for ServerTypings
            /// </summary>
            public bool? SourceGenerator { get; set; }
            /// <summary>Used for Newtonsoft.JSON</summary>
            public bool ShouldSerializeSourceGenerator() => SourceGenerator != null;
        }

        /// <summary>
        /// Client types related config
        /// </summary>
        public class ClientTypesConfig
        {
            /// <summary>
            /// Output directory for generated client types .cs files
            /// </summary>
            public string OutDir { get; set; }
        }

        /// <summary>
        /// MVC (view paths) related config
        /// </summary>
        public class MVCConfig
        {
            /// <summary>
            /// Use project root namespace in generated file
            /// </summary>
            public bool? UseRootNamespace { get; set; }
            /// <summary>
            /// Output directory for MVC.cs
            /// </summary>
            public string OutDir { get; set; }
            /// <summary>
            /// View paths to search for, defaults to "Modules" and "Views".
            /// "ProjectName" is used instead of "Modules" if Razor SDK is used
            /// for the project, e.g. a Razor class library
            /// </summary>
            public string[] SearchViewPaths { get; set; }
            /// <summary>
            /// Strip view paths from generated files. Defaults to "Modules", "Views".
            /// E.g. a subclass for Modules and Views won't be generated.
            /// </summary>
            public string[] StripViewPaths { get; set; }
        }

        /// <summary>
        /// Restore related config
        /// </summary>
        public class RestoreConfig
        {
            /// <summary>
            /// List of files (git ignore like) to include while restoring
            /// </summary>
            public string[] Include { get; set; }
            /// <summary>
            /// List of files (git ignore like) to exclude while restoring
            /// </summary>
            public string[] Exclude { get; set; }
        }

        /// <summary>
        /// Holds info about stargen upgrades
        /// </summary>
        public class UpgradeInformation
        {
            /// <summary>
            /// Initial type of project. Available options are
            /// "Free" e.g. Sere, "Premium" e.g. StartSharp
            /// </summary>
            public string InitialType { get; set; }
            /// <summary>
            /// Version this project is created from. Used to determine
            /// which migrations should be applied to project (starting from)
            /// </summary>
            public string InitialVersion { get; set; }
            /// <summary>
            /// List of already applied upgrades if any
            /// </summary>
            public string[] AppliedUpgrades { get; set; }
            /// <summary>
            /// Extension data for upgrades
            /// </summary>
            [JsonExtensionData]
            public IDictionary<string, JToken> ExtensionData { get; set; }
        }
    }
} 