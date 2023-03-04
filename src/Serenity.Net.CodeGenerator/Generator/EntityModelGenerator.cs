namespace Serenity.CodeGenerator;

public class EntityModelGenerator : IEntityModelGenerator
{
    private static int DeterminePrefixLength<T>(IEnumerable<T> list, Func<T, string> getName)
    {
        if (!Enumerable.Any(list))
            return 0;
        string str1 = getName(Enumerable.First(list));
        int length = str1.IndexOf('_', StringComparison.Ordinal);
        if (length <= 0)
            return 0;
        string str2 = str1[..(length + 1)];
        foreach (T obj in list)
        {
            if (!getName(obj).StartsWith(str2, StringComparison.Ordinal) || getName(obj).Length == str2.Length)
                return 0;
        }
        return str2.Length;
    }

    private static string JI(string join, string field)
    {
        if (string.Compare(field, join, StringComparison.OrdinalIgnoreCase) == 0)
            return field;
        else
            return join + field;
    }

    private static string JU(string join, string field)
    {
        if (string.Compare(join, field, StringComparison.OrdinalIgnoreCase) == 0)
            return field;
        else
            return join + "_" + field;
    }

    private static string FieldTypeToTS(string ft)
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

    private static EntityField ToEntityField(Data.Schema.FieldInfo fieldInfo, int prefixLength)
    {
        List<AttributeTypeRef> flags;
        if (fieldInfo.IsIdentity)
            flags = new List<AttributeTypeRef> { new AttributeTypeRef("Serenity.Data.Mapping.Identity") };
        else if (fieldInfo.IsPrimaryKey)
            flags = fieldInfo.IsNullable ? new List<AttributeTypeRef> { new AttributeTypeRef("Serenity.Data.Mapping.PrimaryKey") } : new List<AttributeTypeRef> { new AttributeTypeRef("Serenity.Data.Mapping.PrimaryKey"), new AttributeTypeRef("Serenity.Data.Mapping.NotNull") };
        else if (fieldInfo.DataType == "timestamp" || fieldInfo.DataType == "rowversion")
            flags = new List<AttributeTypeRef> { new AttributeTypeRef("Serenity.ComponentModel.Insertable", "false"), new AttributeTypeRef("Serenity.ComponentModel.Updatable", "false"), new AttributeTypeRef("Serenity.Data.Mapping.NotNull") };
        else if (!fieldInfo.IsNullable)
            flags = new List<AttributeTypeRef> { new AttributeTypeRef("Serenity.Data.Mapping.NotNull") };
        else
            flags = null;

        var fieldType = SchemaHelper.SqlTypeNameToFieldType(fieldInfo.DataType, fieldInfo.Size, out string dataType);
        dataType ??= fieldType;
        dataType = CodeWriter.ToCSKeyword(dataType) ?? dataType;
        return new EntityField
        {
            FieldType = fieldType,
            DataType = dataType,
            IsValueType = fieldType != "String" && fieldType != "Stream" && fieldType != "ByteArray",
            TSType = FieldTypeToTS(fieldType),
            Ident = PropertyNameFor(fieldInfo.FieldName[prefixLength..]),
            Title = Inflector.Inflector.Titleize(fieldInfo.FieldName[prefixLength..])?.Trim(),
            FlagList = flags,
            Name = fieldInfo.FieldName,
            Size = fieldInfo.Size == 0 ? null : fieldInfo.Size,
            Scale = fieldInfo.Scale
        };
    }

    public EntityModel GenerateModel(IEntityModelInputs inputs)
    {
        if (inputs is null)
            throw new ArgumentNullException(nameof(inputs));

        var className = inputs.Identifier ?? IdentifierForTable(inputs.Table);

        var model = new EntityModel
        {
            ClassName = className,
            ConnectionKey = inputs.ConnectionKey,
            Module = inputs.Module,
            NET5Plus = inputs.Net5Plus,
            Permission = inputs.PermissionKey,
            RootNamespace = inputs.Config.RootNamespace,
            RowClassName = className + "Row",
            Schema = inputs.OmitSchemaInExpressions ? null : inputs.Schema,
            Tablename = inputs.Table,
            Title = Inflector.Inflector.Titleize(className)?.Trim(),
        };

        model.GlobalUsings.AddRange(inputs.GlobalUsings);

        var fields = inputs.DataSchema.GetFieldInfos(inputs.Schema, inputs.Table);
        if (!fields.Any(x => x.IsPrimaryKey))
        {
            var primaryKeys = new HashSet<string>(inputs.DataSchema.GetPrimaryKeyFields(inputs.Schema, inputs.Table));
            foreach (var field in fields)
                field.IsPrimaryKey = primaryKeys.Contains(field.FieldName);
        }

        if (!fields.Any(x => x.IsIdentity))
        {
            var identities = new HashSet<string>(inputs.DataSchema.GetIdentityFields(inputs.Schema, inputs.Table));
            foreach (var field in fields)
                field.IsIdentity = identities.Contains(field.FieldName);
        }

        var foreigns = inputs.DataSchema.GetForeignKeys(inputs.Schema, inputs.Table)
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
        model.FieldPrefix = prefix > 0 ? fields.First().FieldName[..prefix] : "";

        var identity = fields.FirstOrDefault(f => f.IsIdentity == true);
        identity ??= fields.FirstOrDefault(f => f.IsPrimaryKey == true);
        if (identity != null)
            model.Identity = PropertyNameFor(identity.FieldName[prefix..]);
        else
        {
            identity = fields.FirstOrDefault(f => f.IsPrimaryKey == true) ??
                fields.FirstOrDefault();
            if (identity != null)
                model.Identity = PropertyNameFor(identity.FieldName[prefix..]);
        }

        string baseRowMatch = null;
        HashSet<string> baseRowFieldset = null;
        List<string> baseRowFieldList = new();
        if (inputs?.Config?.BaseRowClasses is { } baseRowClasses)
        {
            foreach (var k in inputs.Config.BaseRowClasses)
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
        }

        var removeForeignFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (inputs.Config.RemoveForeignFields is { } removeFK)
        {
            foreach (var s in removeFK)
            {
                string n = s.TrimToNull();
                if (n != null)
                    removeForeignFields.Add(n);
            }
        }

        removeForeignFields.Add("password");
        removeForeignFields.Add("passwordhash");
        removeForeignFields.Add("passwordsalt");

        if (baseRowFieldset != null &&
            baseRowFieldset.Count > 0)
        {
            model.RowBaseClass = baseRowMatch;
            model.FieldsBaseClass = baseRowMatch + "Fields";
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

        if (inputs.Net5Plus)
            model.RowBaseClass = model.RowBaseClass + "<" + model.RowClassName + ".RowFields>";

        var fieldByIdent = new Dictionary<string, EntityField>(StringComparer.OrdinalIgnoreCase);

        foreach (var field in fields)
        {
            var f = ToEntityField(field, prefix);

            if (f.Ident == model.IdField)
                f.ColAttributeList = new List<AttributeTypeRef> { new AttributeTypeRef("Serenity.ComponentModel.EditLink") , new AttributeTypeRef("System.ComponentModel.DisplayName", "\"Db.Shared.RecordId\""), new AttributeTypeRef("Serenity.ComponentModel.AlignRight") };
            
            int i = 0;
            string ident = f.Ident;
            while (fieldByIdent.ContainsKey(ident))
                ident = f.Ident + ++i;
            f.Ident = ident;
            fieldByIdent[ident] = f;

            if (f.Name == className && f.FieldType == "String")
            {
                model.NameField = f.Name;
                f.ColAttributeList ??= new List<AttributeTypeRef> { new AttributeTypeRef("Serenity.ComponentModel.EditLink") };
            }

            var foreign = foreigns.Find((k) => k.FKColumn.Equals(field.FieldName, StringComparison.OrdinalIgnoreCase));
            if (foreign != null)
            {
                if (f.Title.EndsWith(" Id", StringComparison.Ordinal) && f.Title.Length > 3)
                    f.Title = f.Title.SafeSubstring(0, f.Title.Length - 3);

                f.PKSchema = foreign.PKSchema;
                f.PKTable = foreign.PKTable;
                f.PKColumn = foreign.PKColumn;

                var frgfld = inputs.DataSchema.GetFieldInfos(foreign.PKSchema, foreign.PKTable).ToList();
                int frgPrefix = DeterminePrefixLength(frgfld, z => z.FieldName);
                var j = new EntityJoin
                {
                    Fields = new List<EntityField>(),
                    Name = PropertyNameFor(f.Name[prefix..])
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

                    var atk = new List<AttributeTypeRef>
                    {
                        new AttributeTypeRef("System.ComponentModel.DisplayName", "\"" + k.Title + "\"")
                    };
                    k.Expression = "j" + j.Name + ".[" + k.Name + "]";
                    atk.Add(new AttributeTypeRef("Serenity.Data.Mapping.Expression", "\"" + k.Expression + "\"") );
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
                fld.ColAttributeList ??= new List<AttributeTypeRef> { new AttributeTypeRef("Serenity.ComponentModel.EditLink") };
            }
        }

        foreach (var x in model.Fields)
        {
            var attrs = new List<AttributeTypeRef>
            {
                new AttributeTypeRef("System.ComponentModel.DisplayName", "\"" + x.Title + "\"")
            };

            if (x.Ident != x.Name)
                attrs.Add(new AttributeTypeRef("Serenity.Data.Mapping.Column", "\"" + x.Name + "\"" ));

            if ((x.Size ?? 0) > 0)
                attrs.Add(new AttributeTypeRef("Serenity.Data.Mapping.Size", x.Size.ToString()));

            if (x.Scale > 0)
                attrs.Add(new AttributeTypeRef("Serenity.Data.Mapping.Scale", x.Scale.ToString()));

            if (!x.FlagList.IsEmptyOrNull())
                attrs.AddRange(x.FlagList);

            if (!string.IsNullOrEmpty(x.PKTable))
            {
                attrs.Add(new AttributeTypeRef("Serenity.Data.Mapping.ForeignKey", "\"" + 
                    (string.IsNullOrEmpty(x.PKSchema) ? x.PKTable : ("[" + x.PKSchema + "].[" + x.PKTable + "]")) + "\", " +
                    "\"" + x.PKColumn + "\""));
                attrs.Add(new AttributeTypeRef("Serenity.Data.Mapping.LeftJoin", "\"j" + x.ForeignJoinAlias + "\""));
            }

            if (model.IdField == x.Ident && inputs.Net5Plus)
                attrs.Add(new AttributeTypeRef("Serenity.Data.IdProperty"));

            if (model.NameField == x.Ident)
            {
                attrs.Add(new AttributeTypeRef("Serenity.Data.Mapping.QuickSearch"));
                if (inputs.Net5Plus)
                    attrs.Add(new AttributeTypeRef("Serenity.Data.NameProperty"));
            }

            if (x.TextualField != null)
                attrs.Add(new AttributeTypeRef("Serenity.Data.Mapping.TextualField", "\"" + x.TextualField + "\""));

            x.AttributeList = attrs;
        }

        return model;
    }

    private static string PropertyNameFor(string fieldName)
    {
        return Inflector.Inflector.Titleize(fieldName).Replace(" ", "", StringComparison.Ordinal);
    }

    public static string IdentifierForTable(string tableName)
    {
        tableName = tableName.Replace(" ", "", StringComparison.Ordinal);
        if (tableName.StartsWith("tb_", StringComparison.Ordinal))
            tableName = tableName[3..];
        else if (tableName.StartsWith("aspnet_", StringComparison.Ordinal))
            tableName = "AspNet" + tableName[7..];
        return PropertyNameFor(tableName);
    }
}