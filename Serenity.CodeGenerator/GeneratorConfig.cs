using System.Collections.Generic;

namespace Serenity.CodeGenerator
{
    public class GeneratorConfig
    {
        public ServerTypingsConfig ServerTypings { get; set; }
        public ClientTypesConfig ClientTypes { get; set; }
        public MVCConfig MVC { get; set; }
        public List<Connection> Connections { get; set; }
        public string KDiff3Path { get; set; }
        public string TSCPath { get; set; }
        public string RootNamespace { get; set; }
        public List<BaseRowClass> BaseRowClasses { get; set; }
        public List<string> RemoveForeignFields { get; set; }
        public bool GenerateSSImports { get; set; }
        public bool GenerateTSTypings { get; set; }
        public bool GenerateTSCode { get; set; }
        public bool RowFieldsSurroundWithRegion { get; set; }
        public bool GenerateRow { get; set; }
        public bool GenerateColumn { get; set; }
        public bool GenerateForm { get; set; }
        public bool GenerateEndpoint { get; set; }
        public bool GenerateRepository { get; set; }
        public bool GeneratePage { get; set; }
        public bool GenerateGrid { get; set; }
        public bool GenerateDialog { get; set; }
        public bool GenerateGridEditor { get; set; }
        public bool GenerateGridEditorDialog { get; set; }
        public bool GenerateLookupEditor { get; set; }
        public bool MaximizableDialog { get; set; }

        public GeneratorConfig()
        {
            Connections = new List<Connection>();
            BaseRowClasses = new List<BaseRowClass>();
            GenerateTSTypings = true;
            GenerateSSImports = false;
            GenerateTSCode = true;
            RowFieldsSurroundWithRegion = false;
            GenerateRow = true;
            GenerateColumn = true;
            GenerateForm = true;
            GenerateEndpoint = true;
            GenerateRepository = true;
            GeneratePage = true;
            GenerateGrid = true;
            GenerateDialog = true;
            GenerateGridEditor = false;
            GenerateGridEditorDialog = false;
            GenerateLookupEditor = false;
            MaximizableDialog = false;
        }

        public string SaveToJson()
        {
            Connections.Sort((x, y) => x.Key.CompareTo(y.Key));
            return JSON.StringifyIndented(this);
        }

        public static GeneratorConfig LoadFromJson(string json)
        {
            var config = JSON.Parse<GeneratorConfig>(json.TrimToNull() ?? "{}");
            config.Connections = config.Connections ?? new List<GeneratorConfig.Connection>();
            config.RemoveForeignFields = config.RemoveForeignFields ?? new List<string>();
            return config;
        }

        public class Connection
        {
            public string Key { get; set; }
            public string ConnectionString { get; set; }
            public string ProviderName { get; set; }
            public List<Table> Tables { get; set; }

            public Connection()
            {
                Tables = new List<Table>();
            }

            public override string ToString()
            {
                return Key + " [" + ConnectionString + "], " + ProviderName;
            }
        }

        public class Table
        {
            public string Tablename { get; set; }
            public string Identifier { get; set; }
            public string Module { get; set; }
            public string ConnectionKey { get; set; }
            public string PermissionKey { get; set; }
        }

        public class BaseRowClass
        {
            public string ClassName { get; set; }
            public List<string> Fields { get; set; }
        }

        public class ServerTypingsConfig
        {
            public string[] Assemblies { get; set; }
            public string OutDir { get; set; }           
        }

        public class ClientTypesConfig
        {
            public string OutDir { get; set; }
        }

        public class MVCConfig
        {
            public string OutDir { get; set; }
            public string[] SearchViewPaths { get; set; }
            public string[] StripViewPaths { get; set; }
        }
    }
}