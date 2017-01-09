using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Serenity.CodeGenerator
{
    public class RowGenerator
    {
        public static string Generate(IDbConnection connection, string tableSchema, string table, string module, 
            string connectionKey, string entityClass, string permission, GeneratorConfig config)
        {
            var model = GenerateModel(connection, tableSchema, table, module, connectionKey, entityClass, permission, config);
            return Templates.Render(new Views.EntityRow(), model);
        }

        private static int DeterminePrefixLength<T>(IEnumerable<T> list, Func<T, string> getName)
        {
            if (!Enumerable.Any<T>(list))
                return 0;
            string str1 = getName(Enumerable.First<T>(list));
            int length = str1.IndexOf('_');
            if (length <= 0)
                return 0;
            string str2 = str1.Substring(0, length);
            foreach (T obj in list)
            {
                if (!getName(obj).StartsWith(str2))
                    return 0;
            }
            return str2.Length + 1;
        }

        public static string JI(string join, string field)
        {
            if (field.ToLowerInvariant() == join.ToLowerInvariant())
                return field;
            else
                return join + field;
        }

        public static string JU(string join, string field)
        {
            if (join.ToLowerInvariant() == field.ToLowerInvariant())
                return field;
            else
                return join + "_" + field;
        }

        public static string FieldTypeToTS(string ft)
        {
            switch (ft)
            {
                case "Boolean":
                    return "boolean";
                case "String":
                case "DateTime":
                case "TimeSpan":
                case "Guid":
                    return "string";
                case "Int32":
                case "Int16":
                case "Int64":
                case "Single":
                case "Double":
                case "Decimal":
                    return "number";
                case "Stream":
                case "ByteArray":
                    return "number[]";
            }

            return "any";
        }

        private static EntityField ToEntityField(SqlSchemaInfo.FieldInfo fieldInfo, int prefixLength)
        {
            string flags;
            if (fieldInfo.IsIdentity)
                flags = "Identity";
            else if (fieldInfo.IsPrimaryKey)
                flags = "PrimaryKey";
            else if (fieldInfo.DataType == "timestamp" || fieldInfo.DataType == "rowversion")
                flags = "Insertable(false), Updatable(false), NotNull";
            else if (!fieldInfo.IsNullable)
                flags = "NotNull";
            else
                flags = null;

            string dataType;
            var fieldType = SqlSchemaInfo.SqlTypeNameToFieldType(fieldInfo.DataType, fieldInfo.Size, out dataType);
            dataType = dataType ?? fieldType;
            return new EntityField
            {
                FieldType = fieldType,
                DataType = dataType,
                IsValueType = fieldType != "String" && fieldType != "Stream" && fieldType != "ByteArray",
                TSType = FieldTypeToTS(fieldType),
                Ident = GenerateVariableName(fieldInfo.FieldName.Substring(prefixLength)),
                Title = Inflector.Inflector.Titleize(fieldInfo.FieldName.Substring(prefixLength)),
                Flags = flags,
                Name = fieldInfo.FieldName,
                Size = fieldInfo.Size == 0 ? (Int32?)null : fieldInfo.Size,
                Scale = fieldInfo.Scale
            };
        }

        public static EntityModel GenerateModel(IDbConnection connection, string tableSchema, string table,
            string module, string connectionKey, string entityClass, string permission, GeneratorConfig config)
        {
            var model = new EntityModel();
            model.Module = module;

            if (connection.GetDialect().ServerType.StartsWith("MySql", StringComparison.OrdinalIgnoreCase))
                model.Schema = null;
            else
                model.Schema = tableSchema;

            model.Permission = permission;
            model.ConnectionKey = connectionKey;
            model.RootNamespace = config.RootNamespace;
            var className = entityClass ?? ClassNameFromTableName(table);
            model.ClassName = className;
            model.RowClassName = className + "Row";
            model.Tablename = table;
            model.Fields = new List<EntityField>();
            model.Joins = new List<EntityJoin>();
            model.Instance = true;

            var fields = SqlSchemaInfo.GetTableFieldInfos(connection, tableSchema, table);
            var foreigns = SqlSchemaInfo.GetTableSingleFieldForeignKeys(connection, tableSchema, table);

            var prefix = DeterminePrefixLength(fields, x => x.FieldName);

            model.FieldPrefix = fields.First().FieldName.Substring(0, prefix);

            var identity = fields.Find(f => f.IsIdentity == true);
            if (identity == null)
                identity = fields.Find(f => f.IsPrimaryKey == true);
            if (identity != null)
                model.Identity = GenerateVariableName(identity.FieldName.Substring(prefix));
            else
            {
                identity = fields.Find(f => f.IsPrimaryKey == true);
                if (identity != null)
                    model.Identity = GenerateVariableName(identity.FieldName.Substring(prefix));
            }

            string baseRowMatch = null;
            HashSet<string> baseRowFieldset = null;
            List<string> baseRowFieldList = new List<string>();
            foreach (var k in config.BaseRowClasses ?? new List<GeneratorConfig.BaseRowClass>())
            {
                var b = k.ClassName;
                var f = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var fl = new List<string>();
                bool skip = false;
                foreach (var s in k.Fields ?? new List<string>())
                {
                    string n = s.TrimToNull();
                    if (n == null || !fields.Exists(z => z.FieldName.Substring(prefix) == n))
                    {
                        skip = true;
                        break;
                    }
                    f.Add(n);
                    fl.Add(n);
                }

                if (skip)
                    continue;

                if (baseRowFieldset == null || f.Count > baseRowFieldset.Count)
                {
                    baseRowFieldset = f;
                    baseRowFieldList = fl;
                    baseRowMatch = b;
                }
            }

            var removeForeignFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var s in config.RemoveForeignFields ?? new List<string>())
            {
                string n = s.TrimToNull();
                if (n != null)
                    removeForeignFields.Add(n);
            }

            if (baseRowFieldset != null &&
                baseRowFieldset.Count > 0)
            {
                model.RowBaseClass = baseRowMatch;
                model.FieldsBaseClass = baseRowMatch + "Fields";
                model.RowBaseFields = new List<EntityField>();
                fields.RemoveAll(f =>
                {
                    if (baseRowFieldset.Contains(f.FieldName.Substring(prefix)))
                    {
                        var ef = ToEntityField(f, prefix);
                        ef.Flags = null;
                        model.RowBaseFields.Add(ef);
                        return true;
                    }
                    return false;
                });
            }
            else
            {
                model.RowBaseClass = "Row";
                model.RowBaseFields = new List<EntityField>();
                model.FieldsBaseClass = "RowFieldsBase";
            }

            foreach (var field in fields)
            {
                var f = ToEntityField(field, prefix);
                if (f.Name == className && f.FieldType == "String")
                    model.NameField = f.Name;

                var foreign = foreigns.Find((k) => k.FKColumn.Equals(field.FieldName, StringComparison.OrdinalIgnoreCase));
                if (foreign != null)
                {
                    if (f.Title.EndsWith(" Id") && f.Title.Length > 3)
                        f.Title = f.Title.SafeSubstring(0, f.Title.Length - 3);

                    f.PKSchema = foreign.PKSchema;
                    f.PKTable = foreign.PKTable;
                    f.PKColumn = foreign.PKColumn;

                    var frgfld = SqlSchemaInfo.GetTableFieldInfos(connection, foreign.PKSchema, foreign.PKTable);
                    int frgPrefix = RowGenerator.DeterminePrefixLength(frgfld, z => z.FieldName);
                    var j = new EntityJoin();
                    j.Fields = new List<EntityField>();
                    j.Name = GenerateVariableName(f.Name.Substring(prefix));
                    if (j.Name.EndsWith("Id") || j.Name.EndsWith("ID"))
                        j.Name = j.Name.Substring(0, j.Name.Length - 2);
                    f.ForeignJoinAlias = j.Name;
                    j.SourceField = f.Ident;

                    frgfld.RemoveAll(y => removeForeignFields.Contains(y.FieldName));

                    foreach (var frg in frgfld)
                    {
                        if (frg.FieldName.Equals(foreign.PKColumn, StringComparison.OrdinalIgnoreCase))
                            continue;

                        var k = ToEntityField(frg, frgPrefix);
                        k.Flags = null;
                        k.Title = Inflector.Inflector.Titleize(JU(j.Name, frg.FieldName.Substring(frgPrefix)));

                        if (f.TextualField == null && k.FieldType == "String")
                            f.TextualField = JI(j.Name, k.Ident);

                        j.Fields.Add(k);
                    }

                    model.Joins.Add(j);
                }

                model.Fields.Add(f);
            }

            if (model.NameField == null)
            {
                var fld = model.Fields.FirstOrDefault(z => z.FieldType == "String");
                if (fld != null)
                    model.NameField = fld.Ident;
            }

            return model;
        }

        private static bool IsStringLowerCase(string s)
        {
            foreach (char c in s)
                if (!Char.IsLower(c))
                    return false;
            return s.Length > 0;
        }

        public static string GenerateVariableName(string fieldName)
        {
            return Inflector.Inflector.Titleize(fieldName).Replace(" ", "");
        }

        public static string ClassNameFromTableName(string tableName)
        {
            tableName = tableName.Replace(" ", "");
            if (tableName.StartsWith("tb_"))
                tableName = tableName.Substring(3);
            else if (tableName.StartsWith("aspnet_"))
                tableName = "AspNet" + tableName.Substring(7);
            return RowGenerator.GenerateVariableName(tableName);
        }

        private static string ClassNameToLowerCase(string className)
        {
            className = StringHelper.TrimToNull(className);
            if (className == null)
                return className;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < className.Length; i++)
            {
                char c = className[i];
                if (Char.IsUpper(c) &&
                    c >= 'A' &&
                    c <= 'Z')
                {
                    c = Char.ToLowerInvariant(c);
                    if (i > 0 &&
                        !Char.IsUpper(className[i - 1]) &&
                        className[i - 1] != '_')
                        sb.Append("_");
                    sb.Append(c);
                }
                else
                    sb.Append(c);
            }
            return sb.ToString();

        }
    }
}