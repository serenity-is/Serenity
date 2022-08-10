using FieldInfo = Serenity.Data.Schema.FieldInfo;

namespace Serenity.CodeGenerator
{
    public class RowGenerator
    {
        private static int DeterminePrefixLength<T>(IEnumerable<T> list, Func<T, string> getName)
        {
            if (!Enumerable.Any(list))
                return 0;
            string str1 = getName(Enumerable.First(list));
            int length = str1.IndexOf('_', StringComparison.Ordinal);
            if (length <= 0)
                return 0;
            string str2 = str1.Substring(0, length + 1);
            foreach (T obj in list)
            {
                if (!getName(obj).StartsWith(str2, StringComparison.Ordinal) || getName(obj).Length == str2.Length)
                    return 0;
            }
            return str2.Length;
        }

        public static string JI(string join, string field)
        {
            if (string.Compare(field, join, StringComparison.OrdinalIgnoreCase) == 0)
                return field;
            else
                return join + field;
        }

        public static string JU(string join, string field)
        {
            if (string.Compare(join, field, StringComparison.OrdinalIgnoreCase) == 0)
                return field;
            else
                return join + "_" + field;
        }

        public static string FieldTypeToTS(string ft)
        {
            return ft switch
            {
                "Boolean" => "boolean",
                "String" or "DateTime" or "TimeSpan" or "Guid" => "string",
                "Int32" or "Int16" or "Int64" or "Single" or "Double" or "Decimal" => "number",
                "Stream" or "ByteArray" => "number[]",
                _ => "any",
            };
        }

        private static EntityField ToEntityField(FieldInfo fieldInfo, int prefixLength)
        {
            List<string> flags;
            if (fieldInfo.IsIdentity)
                flags = new List<string> { "Serenity.Data.Mapping.Identity" };
            else if (fieldInfo.IsPrimaryKey)
                flags = fieldInfo.IsNullable ? new List<string> { "Serenity.Data.Mapping.PrimaryKey" } : new List<string> { "Serenity.Data.Mapping.PrimaryKey", "Serenity.Data.Mapping.NotNull" };
            else if (fieldInfo.DataType == "timestamp" || fieldInfo.DataType == "rowversion")
                flags = new List<string> { "Serenity.ComponentModel.Insertable(false)", "Serenity.ComponentModel.Updatable(false)", "Serenity.Data.Mapping.NotNull" };
            else if (!fieldInfo.IsNullable)
                flags = new List<string> { "Serenity.Data.Mapping.NotNull" };
            else
                flags = null;

            var fieldType = SchemaHelper.SqlTypeNameToFieldType(fieldInfo.DataType, fieldInfo.Size, out string dataType);
            dataType ??= fieldType;
            dataType = CodeGeneration.SystemTypes.ToCSKeyword(dataType) ?? dataType;
            return new EntityField
            {
                FieldType = fieldType,
                DataType = dataType,
                IsValueType = fieldType != "String" && fieldType != "Stream" && fieldType != "ByteArray",
                TSType = FieldTypeToTS(fieldType),
                Ident = GenerateVariableName(fieldInfo.FieldName[prefixLength..]),
                Title = Inflector.Inflector.Titleize(fieldInfo.FieldName[prefixLength..])?.Trim(),
                FlagList = flags,
                Name = fieldInfo.FieldName,
                Size = fieldInfo.Size == 0 ? null : fieldInfo.Size,
                Scale = fieldInfo.Scale
            };
        }

        public static EntityModel GenerateModel(IDbConnection connection, string tableSchema, string table,
            string module, string connectionKey, string entityClass, string permission, GeneratorConfig config, bool net5Plus)
        {
            var model = new EntityModel
            {
                Module = module
            };

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
            model.Title = Inflector.Inflector.Titleize(className)?.Trim();
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
                model.Identity = GenerateVariableName(identity.FieldName[prefix..]);
            else
            {
                identity = fields.FirstOrDefault(f => f.IsPrimaryKey == true) ??
                    fields.FirstOrDefault();
                if (identity != null)
                    model.Identity = GenerateVariableName(identity.FieldName[prefix..]);
            }

            string baseRowMatch = null;
            HashSet<string> baseRowFieldset = null;
            List<string> baseRowFieldList = new();
            foreach (var k in config.BaseRowClasses ?? new List<GeneratorConfig.BaseRowClass>())
            {
                var b = k.ClassName;
                var f = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var fl = new List<string>();
                bool skip = false;
                foreach (var s in k.Fields ?? new List<string>())
                {
                    string n = s.TrimToNull();
                    if (n == null || !fields.Any(z => z.FieldName[prefix..] == n))
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
                    if (baseRowFieldset.Contains(f.FieldName[prefix..]))
                    {
                        var ef = ToEntityField(f, prefix);
                        ef.FlagList = null;
                        model.RowBaseFields.Add(ef);
                        return false;
                    }
                    return true;
                }).ToList();
            }
            else
            {
                model.RowBaseClass = "Serenity.Data.Row";
                model.RowBaseFields = new List<EntityField>();
                model.FieldsBaseClass = "Serenity.Data.RowFieldsBase";
            }

            if (net5Plus)
                model.RowBaseClass = model.RowBaseClass + "<" + model.RowClassName + ".RowFields>";

            var fieldByIdent = new Dictionary<string, EntityField>(StringComparer.OrdinalIgnoreCase);

            foreach (var field in fields)
            {
                var f = ToEntityField(field, prefix);

                if (f.Ident == model.IdField)
                    f.ColAttributeList = new List<string> { "Serenity.ComponentModel.EditLink", "System.ComponentModel.DisplayName(\"Db.Shared.RecordId\")", "Serenity.ComponentModel.AlignRight" };

                int i = 0;
                string ident = f.Ident;
                while (fieldByIdent.ContainsKey(ident))
                    ident = f.Ident + ++i;
                f.Ident = ident;
                fieldByIdent[ident] = f;

                if (f.Name == className && f.FieldType == "String")
                {
                    model.NameField = f.Name;
                    f.ColAttributeList ??= new List<string> { "Serenity.ComponentModel.EditLink" };
                }

                var foreign = foreigns.Find((k) => k.FKColumn.Equals(field.FieldName, StringComparison.OrdinalIgnoreCase));
                if (foreign != null)
                {
                    if (f.Title.EndsWith(" Id", StringComparison.Ordinal) && f.Title.Length > 3)
                        f.Title = f.Title.SafeSubstring(0, f.Title.Length - 3);

                    f.PKSchema = foreign.PKSchema;
                    f.PKTable = foreign.PKTable;
                    f.PKColumn = foreign.PKColumn;

                    var frgfld = schemaProvider.GetFieldInfos(connection, foreign.PKSchema, foreign.PKTable).ToList();
                    int frgPrefix = DeterminePrefixLength(frgfld, z => z.FieldName);
                    var j = new EntityJoin
                    {
                        Fields = new List<EntityField>(),
                        Name = GenerateVariableName(f.Name[prefix..])
                    };
                    if (j.Name.EndsWith("Id", StringComparison.Ordinal) || j.Name.EndsWith("ID", StringComparison.Ordinal))
                        j.Name = j.Name[0..^2];
                    f.ForeignJoinAlias = j.Name;
                    j.SourceField = f.Ident;

                    frgfld = frgfld.Where(y => !removeForeignFields.Contains(y.FieldName)).ToList();

                    foreach (var frg in frgfld)
                    {
                        if (frg.FieldName.Equals(foreign.PKColumn, StringComparison.OrdinalIgnoreCase))
                            continue;
                        
                        var k = ToEntityField(frg, frgPrefix);
                        k.FlagList = null;
                        k.Title = Inflector.Inflector.Titleize(JU(j.Name, frg.FieldName[frgPrefix..]))?.Trim();
                        k.Ident = JI(j.Name, k.Ident);
                        i = 0;
                        ident = k.Ident;
                        while (fieldByIdent.ContainsKey(ident))
                            ident = k.Ident + ++i;
                        k.Ident = ident;
                        fieldByIdent[ident] = k;

                        var atk = new List<string>
                        {
                            "System.ComponentModel.DisplayName(\"" + k.Title + "\")"
                        };
                        k.Expression = "j" + j.Name + ".[" + k.Name + "]";
                        atk.Add("Serenity.Data.Mapping.Expression(\"" + k.Expression + "\")");
                        k.AttributeList = atk;

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
                    fld.ColAttributeList ??= new List<string> { "Serenity.ComponentModel.EditLink" };
                }
            }

            foreach (var x in model.Fields)
            {
                var attrs = new List<string>
                {
                    "System.ComponentModel.DisplayName(\"" + x.Title + "\")"
                };

                if (x.Ident != x.Name)
                    attrs.Add("Serenity.Data.Mapping.Column(\"" + x.Name + "\")");

                if ((x.Size ?? 0) > 0)
                    attrs.Add("Serenity.Data.Mapping.Size(" + x.Size + ")");

                if (x.Scale > 0)
                    attrs.Add("Serenity.Data.Mapping.Scale(" + x.Scale + ")");

                if (!x.FlagList.IsEmptyOrNull())
                    attrs.AddRange(x.FlagList);

                if (!string.IsNullOrEmpty(x.PKTable))
                {
                    attrs.Add("Serenity.Data.Mapping.ForeignKey(\"" + (string.IsNullOrEmpty(x.PKSchema) ? x.PKTable : ("[" + x.PKSchema + "].[" + x.PKTable + "]")) + "\", \"" + x.PKColumn + "\")");
                    attrs.Add("Serenity.Data.Mapping.LeftJoin(\"j" + x.ForeignJoinAlias + "\")");
                }

                if (model.IdField == x.Ident && net5Plus)
                    attrs.Add("Serenity.Data.IdProperty");

                if (model.NameField == x.Ident)
                {
                    attrs.Add("Serenity.Data.Mapping.QuickSearch");
                    if (net5Plus)
                        attrs.Add("Serenity.Data.NameProperty");
                }

                if (x.TextualField != null)
                    attrs.Add("Serenity.Data.Mapping.TextualField(\"" + x.TextualField + "\")");

                x.AttributeList = attrs;
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
            return Inflector.Inflector.Titleize(fieldName).Replace(" ", "", StringComparison.Ordinal);
        }

        public static string ClassNameFromTableName(string tableName)
        {
            tableName = tableName.Replace(" ", "", StringComparison.Ordinal);
            if (tableName.StartsWith("tb_", StringComparison.Ordinal))
                tableName = tableName[3..];
            else if (tableName.StartsWith("aspnet_", StringComparison.Ordinal))
                tableName = "AspNet" + tableName[7..];
            return GenerateVariableName(tableName);
        }

        private static string ClassNameToLowerCase(string className)
        {
            className = StringHelper.TrimToNull(className);
            if (className == null)
                return className;
            StringBuilder sb = new();
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
                        sb.Append('_');
                    sb.Append(c);
                }
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}