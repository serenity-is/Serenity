using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Serenity.Data;

namespace Serenity.CodeGenerator
{
    public class RowGenerator
    {
        public static string Generate(IDbConnection connection, string tableSchema, string table, string module, string connectionKey, string entityClass, string permission,
            GeneratorConfig config)
        {
            var model = GenerateModel(connection, tableSchema, table, module, connectionKey, entityClass, permission, config);
            return Templates.Render(GeneratorConfig.GetEntityRowView(config), model);
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
                    return "number[]";
            }

            return "any";
        }


        public static EntityCodeGenerationModel GenerateModel(IDbConnection connection, string tableSchema, string table,
            string module, string connectionKey, string entityClass, string permission, GeneratorConfig config)
        {
            var model = new EntityCodeGenerationModel();
            model.Module = module;

            if (connection.GetDialect() is MySqlDialect)
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
            model.Fields = new List<EntityCodeField>();
            model.Joins = new List<EntityCodeJoin>();
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
                model.RowBaseFields = new List<EntityCodeField>();
                fields.RemoveAll(f =>
                {
                    if (baseRowFieldset.Contains(f.FieldName.Substring(prefix)))
                    {
                        var ft = SqlSchemaInfo.SqlTypeNameToFieldType(f.DataType);
                        model.RowBaseFields.Add(new EntityCodeField
                        {
                            Type = ft,
                            TSType = FieldTypeToTS(ft),
                            Ident = GenerateVariableName(f.FieldName.Substring(prefix)),
                            Title = Inflector.Inflector.Titleize(f.FieldName.Substring(prefix)),
                            Name = f.FieldName,
                            IsValueType = ft != "String" && ft != "Stream",
                            Size = f.Size == 0 ? (Int32?)null : f.Size,
                            Scale = f.Scale
                        });

                        return true;
                    }
                    return false;
                });
            }
            else
            {
                model.RowBaseClass = "Row";
                model.RowBaseFields = new List<EntityCodeField>();
                model.FieldsBaseClass = "RowFieldsBase";
            }

            foreach (var field in fields)
            {
                string flags;
                if (field.IsIdentity)
                    flags = "Identity";
                else if (field.IsPrimaryKey)
                    flags = "PrimaryKey";
                else if (!field.IsNullable)
                    flags = "NotNull";
                else
                    flags = null;

                var fieldType = SqlSchemaInfo.SqlTypeNameToFieldType(field.DataType);

                var f = new EntityCodeField()
                {
                    Type = fieldType,
                    TSType = FieldTypeToTS(fieldType),
                    Ident = GenerateVariableName(field.FieldName.Substring(prefix)),
                    Title = Inflector.Inflector.Titleize(field.FieldName.Substring(prefix)),
                    Name = field.FieldName,
                    Flags = flags,
                    IsValueType = fieldType != "String" && fieldType != "Stream",
                    Size = field.Size == 0 ? (Int32?)null : field.Size,
                    Scale = field.Scale
                };

                if (f.Name == className && fieldType == "String")
                    model.NameField = f.Name;

                var foreign = foreigns.Find((k) => k.FKColumn.Equals(field.FieldName, StringComparison.InvariantCultureIgnoreCase));
                if (foreign != null)
                {
                    if (f.Title.EndsWith(" Id") && f.Title.Length > 3)
                        f.Title = f.Title.SafeSubstring(0, f.Title.Length - 3);

                    f.PKSchema = foreign.PKSchema;
                    f.PKTable = foreign.PKTable;
                    f.PKColumn = foreign.PKColumn;

                    var frgfld = SqlSchemaInfo.GetTableFieldInfos(connection, foreign.PKSchema, foreign.PKTable);
                    int frgPrefix = RowGenerator.DeterminePrefixLength(frgfld, z => z.FieldName);
                    var j = new EntityCodeJoin();
                    j.Fields = new List<EntityCodeField>();
                    j.Name = GenerateVariableName(f.Name.Substring(prefix));
                    if (j.Name.EndsWith("Id") || j.Name.EndsWith("ID"))
                        j.Name = j.Name.Substring(0, j.Name.Length - 2);
                    f.ForeignJoinAlias = j.Name;
                    j.SourceField = f.Ident;

                    frgfld.RemoveAll(y => removeForeignFields.Contains(y.FieldName));

                    if (frgfld.Find(y => y.FieldName.Substring(frgPrefix) == "SonGuncelleyenID") != null)
                    {
                        //frgfld.RemoveAll(y => LoggingBaseFields.Contains(y.FieldName.Substring(frgPrefix)));
                    }

                    foreach (var frg in frgfld)
                    {
                        if (frg.FieldName.Equals(foreign.PKColumn, StringComparison.InvariantCultureIgnoreCase))
                            continue;

                        var kType = SqlSchemaInfo.SqlTypeNameToFieldType(frg.DataType);

                        var k = new EntityCodeField()
                        {
                            Type = kType,
                            TSType = FieldTypeToTS(kType),
                            Ident = GenerateVariableName(frg.FieldName.Substring(frgPrefix)),
                            Title = Inflector.Inflector.Titleize(JU(j.Name, frg.FieldName.Substring(frgPrefix))),
                            Name = frg.FieldName,
                            Flags = flags,
                            IsValueType = kType != "String" && kType != "Stream",
                            Size = frg.Size == 0 ? (Int32?)null : frg.Size,
                            Scale = frg.Scale
                        };

                        if (f.TextualField == null && kType == "String")
                            f.TextualField = JI(j.Name, k.Ident);

                        j.Fields.Add(k);
                    }

                    model.Joins.Add(j);
                }

                model.Fields.Add(f);
            }

            if (model.NameField == null)
            {
                var fld = model.Fields.FirstOrDefault(z => z.Type == "String");
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

            /*string[] strArray = fieldName.Trim().Split(new char[1] { '_' });
            string str1 = string.Empty;
            foreach (string s in strArray)
            {
                string str2 = s;
                if (RowGenerator.IsStringLowerCase(s))
                    str2 = !(str2 == "id") ? ((object)char.ToUpper(str2[0], CultureInfo.InvariantCulture)).ToString() + ((object)str2.Remove(0, 1)).ToString() : "ID";
                str1 = str1 + str2;
            }
            return str1;*/
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
