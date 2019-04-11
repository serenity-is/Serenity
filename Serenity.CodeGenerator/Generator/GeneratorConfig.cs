using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Serenity.CodeGenerator
{
    public class GeneratorConfig
    {
        public string RootNamespace { get; set; }
        public ServerTypingsConfig ServerTypings { get; set; }
        public ClientTypesConfig ClientTypes { get; set; }
        public MVCConfig MVC { get; set; }
        public List<Connection> Connections { get; set; }
        public string KDiff3Path { get; set; }
        public string TSCPath { get; set; }
        public List<BaseRowClass> BaseRowClasses { get; set; }
        public List<string> RemoveForeignFields { get; set; }
        public string CustomTemplates { get; set; }
        public Dictionary<string, string> CustomGenerate { get; set; }
        public Dictionary<string, object> CustomSettings { get; set; }
        public bool UseDBIdentifiers { get; set; }
        [JsonIgnore]
        public bool GenerateRow { get; set; }
        [JsonIgnore]
        public bool GenerateService { get; set; }
        [JsonIgnore]
        public bool GenerateUI { get; set; }
        [JsonIgnore]
        public bool GenerateCustom { get; set; }

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
            UseDBIdentifiers = false;
        }

        public string SaveToJson()
        {
            Connections.Sort((x, y) => x.Key.CompareTo(y.Key));
            foreach (var c in Connections)
                c.Tables.Sort((x, y) => x.Tablename.CompareTo(y.Tablename));

            return JSON.StringifyIndented(this, 2);
        }

        public static GeneratorConfig LoadFromFile(string sergenJson)
        {
            if (!File.Exists(sergenJson))
                return LoadFromJson(null);

            return LoadFromJson(File.ReadAllText(sergenJson));
        }

        public static GeneratorConfig LoadFromJson(string json)
        {
            var config = JSON.ParseTolerant<GeneratorConfig>(json.TrimToNull() ?? "{}");
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
            public bool LocalTexts { get; set; }
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