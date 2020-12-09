using Serenity.Data;
using Serenity.Data.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Serenity.CodeGenerator
{
    public class RowGenerator
    {
        private static int DeterminePrefixLength<T>(IEnumerable<T> list, Func<T, string> getName)
        {
            if (!Enumerable.Any<T>(list))
                return 0;
            string str1 = getName(Enumerable.First<T>(list));
            int length = str1.IndexOf('_');
            if (length <= 0)
                return 0;
            string str2 = str1.Substring(0, length + 1);
            foreach (T obj in list)
            {
                if (!getName(obj).StartsWith(str2) || getName(obj).Length == str2.Length)
                    return 0;
            }
            return str2.Length;
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

        private static EntityField ToEntityField(FieldInfo fieldInfo, int prefixLength)
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
            var fieldType = SchemaHelper.SqlTypeNameToFieldType(fieldInfo.DataType, fieldInfo.Size, out dataType);
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
                Size = fieldInfo.Size == 0 ? (int?)null : fieldInfo.Size,
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
            model.Title = Inflector.Inflector.Titleize(className);
            model.Tablename = table;
            model.Fields = new List<EntityField>();
            model.Joins = new List<EntityJoin>();
            model.Instance = true;

            var schemaProvider = SchemaHelper.GetSchemaProvider(connection.GetDialect().ServerType);
            var fields = schemaProvider.GetFieldInfos(connection, tableSchema, table).ToList();
            if (!fields.Any(x => x.IsPrimaryKey))
            {
                var primaryKeys = new HashSet<string>(schemaProvider.GetPrimaryKeyFields(connection, tableSchema, table));
                foreach (var field in fields)
                    field.IsPrimaryKey = primaryKeys.Contains(field.FieldName);
            }

            if (!fields.Any(x => x.IsIdentity))
            {
                var identities = new HashSet<string>(schemaProvider.GetIdentityFields(connection, tableSchema, table));
                foreach (var field in fields)
                    field.IsIdentity = identities.Contains(field.FieldName);
            }

            var foreigns = schemaProvider.GetForeignKeys(connection, tableSchema, table)
                .ToLookup(x => x.FKName)
                .Where(x => x.Count() == 1)
                .SelectMany(x => x)
                .ToList();

            foreach (var field in fields)
            {
                var fk = foreigns.FirstOrDefault(x => x.FKColumn == field.FieldName);
                if (fk != null)
                {
                    field.PKSchema = fk.PKSchema;
                    field.PKTable = fk.PKTable;
                    field.PKColumn = fk.PKColumn;
                }
            }

            var prefix = DeterminePrefixLength(fields, x => x.FieldName);

            model.FieldPrefix = fields.First().FieldName.Substring(0, prefix);

            var identity = fields.FirstOrDefault(f => f.IsIdentity == true);
            if (identity == null)
                identity = fields.FirstOrDefault(f => f.IsPrimaryKey == true);
            if (identity != null)
                model.Identity = GenerateVariableName(identity.FieldName.Substring(prefix));
            else
            {
                identity = fields.FirstOrDefault(f => f.IsPrimaryKey == true) ??
                    fields.FirstOrDefault();
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
                    if (n == null || !fields.Any(z => z.FieldName.Substring(prefix) == n))
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

            removeForeignFields.Add("password");
            removeForeignFields.Add("passwordhash");
            removeForeignFields.Add("passwordsalt");

            if (baseRowFieldset != null &&
                baseRowFieldset.Count > 0)
            {
                model.RowBaseClass = baseRowMatch;
                model.FieldsBaseClass = baseRowMatch + "Fields";
                model.RowBaseFields = new List<EntityField>();
                fields = fields.Where(f =>
                {
                    if (baseRowFieldset.Contains(f.FieldName.Substring(prefix)))
                    {
                        var ef = ToEntityField(f, prefix);
                        ef.Flags = null;
                        model.RowBaseFields.Add(ef);
                        return false;
                    }
                    return true;
                }).ToList();
            }
            else
            {
                model.RowBaseClass = "Row";
                model.RowBaseFields = new List<EntityField>();
                model.FieldsBaseClass = "RowFieldsBase";
            }

            var fieldByIdent = new Dictionary<string, EntityField>(StringComparer.OrdinalIgnoreCase);

            foreach (var field in fields)
            {
                var f = ToEntityField(field, prefix);

                if (f.Ident == model.IdField)
                    f.ColAttributes = "EditLink, DisplayName(\"Db.Shared.RecordId\"), AlignRight";

                int i = 0;
                string ident = f.Ident;
                while (fieldByIdent.ContainsKey(ident))
                    ident = f.Ident + ++i;
                f.Ident = ident;
                fieldByIdent[ident] = f;

                if (f.Name == className && f.FieldType == "String")
                {
                    model.NameField = f.Name;
                    f.ColAttributes = f.ColAttributes ?? "EditLink";
                }

                var foreign = foreigns.Find((k) => k.FKColumn.Equals(field.FieldName, StringComparison.OrdinalIgnoreCase));
                if (foreign != null)
                {
                    if (f.Title.EndsWith(" Id") && f.Title.Length > 3)
                        f.Title = f.Title.SafeSubstring(0, f.Title.Length - 3);

                    f.PKSchema = foreign.PKSchema;
                    f.PKTable = foreign.PKTable;
                    f.PKColumn = foreign.PKColumn;

                    var frgfld = schemaProvider.GetFieldInfos(connection, foreign.PKSchema, foreign.PKTable).ToList();
                    int frgPrefix = DeterminePrefixLength(frgfld, z => z.FieldName);
                    var j = new EntityJoin();
                    j.Fields = new List<EntityField>();
                    j.Name = GenerateVariableName(f.Name.Substring(prefix));
                    if (j.Name.EndsWith("Id") || j.Name.EndsWith("ID"))
                        j.Name = j.Name.Substring(0, j.Name.Length - 2);
                    f.ForeignJoinAlias = j.Name;
                    j.SourceField = f.Ident;

                    frgfld = frgfld.Where(y => !removeForeignFields.Contains(y.FieldName)).ToList();

                    foreach (var frg in frgfld)
                    {
                        if (frg.FieldName.Equals(foreign.PKColumn, StringComparison.OrdinalIgnoreCase))
                            continue;
                        
                        var k = ToEntityField(frg, frgPrefix);
                        k.Flags = null;
                        k.Title = Inflector.Inflector.Titleize(JU(j.Name, frg.FieldName.Substring(frgPrefix)));
                        k.Ident = JI(j.Name, k.Ident);
                        i = 0;
                        ident = k.Ident;
                        while (fieldByIdent.ContainsKey(ident))
                            ident = k.Ident + ++i;
                        k.Ident = ident;
                        fieldByIdent[ident] = k;

                        var atk = new List<string>();
                        atk.Add("DisplayName(\"" + k.Title + "\")");
                        k.Expression = "j" + j.Name + ".[" + k.Name + "]";
                        atk.Add("Expression(\"" + k.Expression + "\")");
                        k.Attributes = string.Join(", ", atk);

                        if (f.TextualField == null && k.FieldType == "String")
                            f.TextualField = k.Ident;

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
                {
                    model.NameField = fld.Ident;
                    fld.ColAttributes = fld.ColAttributes ?? "EditLink";
                }
            }

            foreach (var x in model.Fields)
            {
                var attrs = new List<string>();
                attrs.Add("DisplayName(\"" + x.Title + "\")");

                if (x.Ident != x.Name)
                    attrs.Add("Column(\"" + x.Name + "\")");

                if ((x.Size ?? 0) > 0)
                    attrs.Add("Size(" + x.Size + ")");

                if (x.Scale > 0)
                    attrs.Add("Scale(" + x.Scale + ")");

                if (!string.IsNullOrEmpty(x.Flags))
                    attrs.Add(x.Flags);

                if (!string.IsNullOrEmpty(x.PKTable))
                {
                    attrs.Add("ForeignKey(\"" + (string.IsNullOrEmpty(x.PKSchema) ? x.PKTable : ("[" + x.PKSchema + "].[" + x.PKTable + "]")) + "\", \"" + x.PKColumn + "\")");
                    attrs.Add("LeftJoin(\"j" + x.ForeignJoinAlias + "\")");
                }

                if (model.NameField == x.Ident)
                    attrs.Add("QuickSearch");

                if (x.TextualField != null)
                    attrs.Add("TextualField(\"" + x.TextualField + "\")");

                x.Attributes = string.Join(", ", attrs.ToArray());
            }

            return model;
        }

        private static bool IsStringLowerCase(string s)
        {
            foreach (char c in s)
                if (!char.IsLower(c))
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
            return GenerateVariableName(tableName);
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
                if (char.IsUpper(c) &&
                    c >= 'A' &&
                    c <= 'Z')
                {
                    c = char.ToLowerInvariant(c);
                    if (i > 0 &&
                        !char.IsUpper(className[i - 1]) &&
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