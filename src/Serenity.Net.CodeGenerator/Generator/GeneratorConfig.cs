namespace Serenity.CodeGenerator;

/// <summary>
/// Sergen (or Stargen) and Source Generator Configuration
/// that is usually stored in sergen.json in project directory
/// </summary>
public class GeneratorConfig
{
    /// <summary>
    /// If specified, the settings in this file extends
    /// settings in a base file similar to tsconfig.json.
    /// This can also be a special string like "defaults@6.6.0" to
    /// apply defaults introduced in a specific Serenity version.
    /// See "GeneratorDefaults.cs" file for a list of possible
    /// version values.
    /// </summary>
    public string Extends { get; set; }

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
    /// Will declare and use jFKTable type of constants for expressions in entities
    /// </summary>
    public bool? DeclareJoinConstants { get; set; }

    /// <summary>
    /// When true, enables usage of [GenerateFields] attributes on rows,
    /// causing the fields to be generated automatically by a source generator.
    /// This should only be used when Serenity.Pro.Coder is enabled in the project.
    /// </summary>
    public bool? EnableGenerateFields { get; set; }

    /// <summary>
    /// When true, enables RowTemplate class generation. This should
    /// only be used when Serenity.Pro.Coder is enabled in the project.
    /// Has no effect when EnableGenerateFields is true.
    /// EnableGenerateFields should be preferred over this as 
    /// RowTemplate might cause some issues.
    /// </summary>
    public bool? EnableRowTemplates { get; set; }

    /// <summary>
    /// End of line style. Can be "lf" or "crlf", default is based on 
    /// environment, e.g. for Windows CRLF, for Linux LF. It is recommended
    /// to have "lf" for multi platform projects.
    /// </summary>
    public string EndOfLine { get; set; }

    /// <summary>
    /// If true, prefers file scoped namespaces in generated code
    /// </summary>
    public bool? FileScopedNamespaces { get; set; }

    /// <summary>
    /// If true, global usings will be tried to be parsed
    /// from the current project
    /// Not implemented yet.
    /// </summary>
    public bool? ParseGlobalUsings { get; set; }

    /// <summary>
    /// If passed, these global usings will be assumed to be present,
    /// even if not parsed from the project.
    /// Not implemented yet.
    /// </summary>
    public List<string> IncludeGlobalUsings { get; set; }

    /// <summary>Used for Newtonsoft.JSON</summary>
    public bool ShouldSerializeIncludeGlobalUsings() => IncludeGlobalUsings != null && IncludeGlobalUsings.Count > 0;

    /// <summary>
    /// If passed, these global usings will be assumed to be not 
    /// present. Even if they are parsed from the project.
    /// Not implemented yet.
    /// </summary>
    public List<string> ExcludeGlobalUsings { get; set; }

    /// <summary>Used for Newtonsoft.JSON</summary>
    public bool ShouldSerializeExcludeGlobalUsings() => ExcludeGlobalUsings != null && ExcludeGlobalUsings.Count > 0;

    /// <summary>
    /// If true, the default schema name, e.g. "dbo" for SQL Server
    /// won't be emitted in generated field expressions in Row.cs
    /// This is true by default only for MySql as in MySql schema names
    /// are actually database names.
    /// </summary>
    public bool? OmitDefaultSchema { get; set; }

    /// <summary>
    /// When true, instead of using [Expression] attributes for foreign view fields,
    /// it will use Origin(nameof(jFK), nameof(FKRow.ViewField)) if the FKRow is already 
    /// generated in the project. It will also use ForeignKey(typeof(FKRow)) type of 
    /// foreign keys instead of strings.
    /// Not implemented yet.
    /// </summary>
    public bool? UseOriginAttribute { get; set; }

    /// <summary>
    /// If true (default) generated table options like Identity,
    /// Module etc. will be saved to the Connections array in 
    /// sergen.json after code generation
    /// </summary>
    public bool? SaveGeneratedTables { get; set; }

    /// <summary>
    /// Client types code generation related configuration
    /// </summary>
    public ClientTypesConfig ClientTypes { get; set; }

    /// <summary>Used for Newtonsoft.JSON</summary>
    public bool ShouldSerializeClientTypes() => ClientTypes != null &&
        (!string.IsNullOrEmpty(ClientTypes.OutDir) ||
         ClientTypes.SourceGenerator == false);

    /// <summary>
    /// MVC (e.g. view locations) related configuration
    /// </summary>
    public MVCConfig MVC { get; set; }

    /// <summary>Used for Newtonsoft.JSON</summary>
    public bool ShouldSerializeMVC() => MVC != null &&
        (!string.IsNullOrEmpty(MVC.OutDir) ||
         MVC.InternalAccess != null ||
         MVC.SearchViewPaths?.Length > 0 ||
         MVC.StripViewPaths?.Length > 0 ||
         MVC.SourceGenerator == false);

    /// <summary>
    /// Server typings code generation related configuration
    /// </summary>
    public ServerTypingsConfig ServerTypings { get; set; }

    /// <summary>
    /// Sergen restore command related configuration
    /// </summary>
    [Obsolete("Sergen no longer supports restoring static content.")]
    public RestoreConfig Restore { get; set; }
    /// <summary>Used for Newtonsoft.JSON</summary>
    [Obsolete("Only used for JSON serialization of Restore")]
    public bool ShouldSerializeRestore() => Restore != null &&
        (!Restore.Include.IsEmptyOrNull() ||
         !Restore.Exclude.IsEmptyOrNull());

    /// <summary>
    /// TSBuild related configuration
    /// </summary>
    public TSBuildConfig TSBuild { get; set; }

    /// <summary>Used for Newtonsoft.JSON</summary>
    public bool ShouldSerializeTSBuild() => TSBuild != null &&
        (TSBuild.EntryPoints != null ||
            TSBuild.SourceGenerator != null ||
            TSBuild.ExtensionData?.Count > 0);

    /// <summary>
    /// List of connections. Is only needed when it is desired to 
    /// work / generated code based on a connection string that 
    /// is not in appsettings.json
    /// </summary>
    public List<Connection> Connections { get; set; }

    /// <summary>Used for Newtonsoft.JSON</summary>
    public bool ShouldSerializeConnections() => Connections != null && Connections.Count > 0;
   
    /// <summary>
    /// List of base row classes to be used instead of Row.
    /// They are matched by list of base properties.
    /// </summary>
    public List<BaseRowClass> BaseRowClasses { get; set; }
    /// <summary>Used for Newtonsoft.JSON</summary>
    public bool ShouldSerializeBaseRowClasses() =>
        BaseRowClasses != null && BaseRowClasses.Count > 0;

    /// <summary>
    /// The set of foreign fields to generate, default is All
    /// </summary>
    public FieldSelection? ForeignFieldSelection { get; set; }

    /// <summary>
    /// A list of foreign fields to include in generated code.
    /// This could be used to include some additional fields 
    /// in the foreign table when ForeignSelection is None or NameOnly.
    /// Not so useful if ForeignFieldSelection is All
    /// </summary>
    public List<string> IncludeForeignFields { get; set; }

    /// <summary>Used for Newtonsoft.JSON</summary>
    public bool ShouldSerializeIncludeForeignFields() =>
        IncludeForeignFields != null && IncludeForeignFields.Count > 0;

    /// <summary>
    /// A list of foreign fields to omit from generated code.
    /// This could be used to disable generating code for join fields 
    /// like CreatedBy, ModifiedBy etc, so properties like CustomerCreatedBy,
    /// CustomerModifiedBy etc. won't be generated in OrderRow.
    /// Not so useful if ForeignFieldSelection is None or NameOnly
    /// </summary>
    public List<string> RemoveForeignFields { get; set; }

    /// <summary>Used for Newtonsoft.JSON</summary>
    public bool ShouldSerializeRemoveForeignFields() =>
        RemoveForeignFields != null && RemoveForeignFields.Count > 0;

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
        CustomGenerate != null && CustomGenerate.Count > 0;

    /// <summary>
    /// Custom settings to be passed to and used in custom templates
    /// </summary>
    public Dictionary<string, object> CustomSettings { get; set; }
    /// <summary>Used for Newtonsoft.JSON</summary>
    public bool ShouldSerializeCustomSettings() =>
        CustomSettings != null && CustomSettings.Count > 0;

    /// <summary>
    /// List of appsettings.json files in order.
    /// Default is appsettings.json, appsettings.machine.json
    /// </summary>
    public string[] AppSettingFiles { get; set; }
    /// <summary>Used for Newtonsoft.JSON</summary>
    public bool ShouldSerializeAppSettingFiles() =>
        AppSettingFiles != null && AppSettingFiles.Length > 0;

    /// <summary>
    /// Generate row class
    /// </summary>
#if !ISSOURCEGENERATOR
    [JsonIgnore]
#endif
    [Newtonsoft.Json.JsonIgnore]
    public bool GenerateRow { get; set; }
    /// <summary>
    /// Generate service classes like repository, endpoint, service.ts etc.
    /// </summary>
#if !ISSOURCEGENERATOR
    [JsonIgnore]
#endif
    [Newtonsoft.Json.JsonIgnore]
    public bool GenerateService { get; set; }
    /// <summary>
    /// Generate UI related classes like Grid/Dialog
    /// </summary>
#if !ISSOURCEGENERATOR
    [JsonIgnore]
#endif
    [Newtonsoft.Json.JsonIgnore]
    public bool GenerateUI { get; set; }
    /// <summary>
    /// Generate custom code (user defined templates at CustomTemplates path)
    /// </summary>
#if !ISSOURCEGENERATOR
    [JsonIgnore]
#endif
    [Newtonsoft.Json.JsonIgnore]
    public bool GenerateCustom { get; set; }

    /// <summary>
    /// Upgrade / migration related information used by stargen to determine
    /// the base version of template used, and already applied templates.
    /// </summary>
    public UpgradeInformation UpgradeInfo { get; set; }

    /// <summary>
    /// Holds extension data if any
    /// </summary>
    [Newtonsoft.Json.JsonExtensionData]
#if !ISSOURCEGENERATOR
    [JsonExtensionData]
#endif
    public IDictionary<string, object> ExtensionData { get; set; }

    public GeneratorConfig()
    {
        Connections = [];
        BaseRowClasses = [];
        CustomSettings = [];
        CustomGenerate = [];
        GenerateRow = true;
        GenerateService = true;
        GenerateUI = true;
        GenerateCustom = true;
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
            Tables = [];
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
        /// Output directory for server typings generated files. Not used for modules.
        /// </summary>
        public string OutDir { get; set; }

        /// <summary>Used for Newtonsoft.JSON</summary>
        public bool ShouldSerializeOutDir() => !string.IsNullOrEmpty(OutDir);

        /// <summary>
        /// Generate local texts
        /// </summary>
        public bool LocalTexts { get; set; }

        /// <summary>
        /// Generate module re-exports. Defaults to true.
        /// </summary>
        public bool? ModuleReExports { get; set; }
        /// <summary>Used for Newtonsoft.JSON</summary>
        public bool ShouldSerializeModuleReExports() => ModuleReExports != null;

        /// <summary>
        /// Generate module typings. Defaults to true if you
        /// have "module" defined in tsconfig.json
        /// </summary>
        [Obsolete("This flag has no effect as only module typings are generated now.")]
        public bool? ModuleTypings { get; set; }

        /// <summary>Used for Newtonsoft.JSON</summary>
        [Obsolete("Only used for JSON serialization of ModuleTypings")]
        public bool ShouldSerializeModuleTypings() => ModuleTypings != null;

        /// <summary>
        /// Generate namespace typings. Defaults to true if you
        /// don't have "module" defined in tsconfig.json
        /// </summary>
        [Obsolete("Namespaces mode code generation is deprecated.")]
        public bool? NamespaceTypings { get; set; }
        /// <summary>Used for Newtonsoft.JSON</summary>
        [Obsolete("Only used for JSON serialization of NamespaceTypings")]
        public bool ShouldSerializeNamespaceTypings() => NamespaceTypings != null;

        /// <summary>
        /// Prefer using relative paths like "../ServerTypes/" instead of "@/ServerTypes/"
        /// </summary>
        public bool? PreferRelativePaths { get; set; }
        /// <summary>Used for Newtonsoft.JSON</summary>
        public bool ShouldSerializePreferRelativePaths() => PreferRelativePaths != null;

        /// <summary>
        /// Set false to disable the server typings source generator in Serenity.Pro.Coder
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
        /// <summary>
        /// Set false to disable the client types source generator in Serenity.Pro.Coder
        /// </summary>
        public bool? SourceGenerator { get; set; }
        /// <summary>Used for Newtonsoft.JSON</summary>
        public bool ShouldSerializeSourceGenerator() => SourceGenerator != null;
    }

    /// <summary>
    /// Enumeration that determines which foreign view fields will be generated
    /// for current row
    /// </summary>
    public enum FieldSelection
    {
        /// <summary>
        /// Don't generate any foreign view fields except the ones
        /// explicitly included via IncludeForeignFields
        /// </summary>
        None = -1,

        /// <summary>
        /// Generate all the foreign view fields, except ones excluded 
        /// explicitly via RemoveForeignFields
        /// </summary>
        All = 0,

        /// <summary>
        /// Don't generate any foreign view fields except the Name property of the target row
        /// and ones explicitly included via IncludeForeignFields
        /// </summary>
        NameOnly = 1
    }

    /// <summary>
    /// MVC (view paths) related config
    /// </summary>
    public class MVCConfig
    {
        /// <summary>
        /// Obsolete. Sergen will now always use the root namespace.
        /// </summary>
        [Obsolete("Sergen will always use the root namespace to avoid class name clashes.")]
        public bool? UseRootNamespace { get; set; }
        /// <summary>
        /// Use internal instead of public for generated top level class (MVC/ESM).
        /// </summary>
        public bool? InternalAccess { get; set; }
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
        /// <summary>
        /// Set false to disable the view paths source generator in Serenity.Pro.Coder
        /// </summary>
        public bool? SourceGenerator { get; set; }
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

        /// <summary>
        /// Set false to disable namespace typings restore
        /// to typings/ folder.
        /// </summary>
        public bool? Typings { get; set; }
    }

    /// <summary>
    /// Holds info about stargen upgrades
    /// </summary>
    public class UpgradeInformation
    {
        /// <summary>
        /// Initial type of project. Available options are
        /// "Community" e.g. Serene, "Premium" e.g. StartSharp
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
        [Newtonsoft.Json.JsonExtensionData]
#if !ISSOURCEGENERATOR
        [JsonExtensionData]
#endif
        public IDictionary<string, object> ExtensionData { get; set; }
    }

    public class TSBuildConfig
    {
        /// <summary>
        /// List of entry point globs, default is "Modules/**/*Page.ts", "Modules/**/*Page.tsx", "Modules/**/ScriptInit.ts"
        /// Include "+" as the first item to append to default list.
        /// </summary>
        public List<string> EntryPoints { get; set; }

        /// <summary>
        /// Should the list of entry points, e.g. ESM be generated, only available with Serenity.Pro.Coder.
        /// </summary>
        public bool? SourceGenerator { get; set; }

        /// <summary>
        /// Extension data for TSBuild
        /// </summary>
        [Newtonsoft.Json.JsonExtensionData]
#if !ISSOURCEGENERATOR
        [JsonExtensionData]
#endif
        public IDictionary<string, object> ExtensionData { get; set; }
    }
}
