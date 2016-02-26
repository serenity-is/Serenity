using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;

namespace Serenity.Testing
{
    public class DbScript
    {
        private StringBuilder script;

        public DbScript()
        {
            script = new StringBuilder(65536);
        }

        public void AddScript(string scriptText)
        {
            if (script.Length > 0)
            {
                script.AppendLine();
                script.AppendLine("GO");
                script.AppendLine();
            }

            script.AppendLine(scriptText);
        }

        private void AddCommandWithParameters(string commandText, IDictionary<string, object> parameters)
        {
            string statementText;
            using (var command = new SqlCommand())
            {
                command.CommandText = DatabaseCaretReferences.Replace(commandText);

                if (parameters != null)
                    foreach (var p in parameters)
                        SqlHelper.AddParamWithValue(command, command.Connection, p.Key, p.Value);

                statementText = SqlCommandDumper.GetCommandText(command);
            }

            AddScript(statementText);
        }

        public void Add(SqlInsert statement)
        {
            AddCommandWithParameters(statement.ToString(), statement.Params);
        }

        public void Add(SqlUpdate statement)
        {
            AddCommandWithParameters(statement.ToString(), statement.Params);
        }

        public void AddResourceScript(string resourceName)
        {
            AddScript(GetResourceScript(resourceName));
        }

        public string GetResourceScript(string resourceName, Assembly assembly = null)
        {
            if (assembly == null)
                assembly = this.GetType().Assembly;

            var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new ArgumentOutOfRangeException("resourceName", resourceName);

            using (var sr = new StreamReader(stream))
                return sr.ReadToEnd();
        }

        public override string ToString()
        {
            return script.ToString();
        }
    }
}