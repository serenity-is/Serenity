using System;
using System.Collections.Generic;
using System.IO;

namespace Serenity.CodeGenerator
{
    public class GeneratorConfig
    {
        public List<Connection> Connections { get; set; }
        public string KDiff3Path { get; set; }
        public string WebProjectFile { get; set; }
        public string ScriptProjectFile { get; set; }
        public string RootNamespace { get; set; }
        public List<BaseRowClass> BaseRowClasses { get; set; }
        public List<string> RemoveForeignFields { get; set; }

        public GeneratorConfig()
        {
            Connections = new List<Connection>();
            KDiff3Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"KDiff3\kdiff3.exe");
            RootNamespace = "MyCompany";
            BaseRowClasses = new List<BaseRowClass>();
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
                return Key + " [" + ConnectionString + "], " +  ProviderName;
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
    }
}