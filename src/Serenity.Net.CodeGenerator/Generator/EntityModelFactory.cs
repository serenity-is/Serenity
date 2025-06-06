namespace Serenity.CodeGenerator;

public class EntityModelFactory : IEntityModelFactory
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

    private static string JoinUnderscore(string join, string field)
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
            "String" or "DateTime" or "DateOnly" or "TimeSpan" or "Guid" => "string",
            "Int32" or "Int16" or "Int64" or "Single" or "Double" or "Decimal" => "number",
            "Stream" or "ByteArray" => "number[]",
            _ => "any",
        };
    }

    private static EntityField ToEntityField(Data.Schema.FieldInfo fieldInfo, int prefixLength, bool includeFlags)
    {
        var fieldType = SchemaHelper.SqlTypeNameToFieldType(fieldInfo.DataType, fieldInfo.Size, out string dataType);
        dataType ??= fieldType;
        dataType = CodeWriter.ToCSKeyword(dataType) ?? dataType;
        var entityField = new EntityField
        {
            FieldType = fieldType,
            DataType = dataType,
            IsValueType = fieldType != "String" && fieldType != "Stream" && fieldType != "ByteArray",
            TSType = FieldTypeToTS(fieldType),
            PropertyName = PropertyNameFor(fieldInfo.FieldName[prefixLength..]),
            Title = Inflector.Inflector.Titleize(fieldInfo.FieldName[prefixLength..])?.Trim(),
            Name = fieldInfo.FieldName,
            Size = fieldInfo.Size == 0 ? null : fieldInfo.Size,
            Scale = fieldInfo.Scale
        };

        if (includeFlags)
        {
            var flags = entityField.FlagList;
            if (fieldInfo.IsIdentity)
                flags.Add(new("Serenity.Data.Mapping.Identity"));
            else
            {
                bool version = fieldInfo.DataType == "timestamp" || fieldInfo.DataType == "rowversion";

                if (fieldInfo.IsPrimaryKey)
                    flags.Add(new("Serenity.Data.Mapping.PrimaryKey"));
                else if (version)
                {
                    flags.Add(new("Serenity.ComponentModel.Insertable", false));
                    flags.Add(new("Serenity.ComponentModel.Updatable", false));
                }

                if (!fieldInfo.IsNullable || version)
                    flags.Add(new("Serenity.Data.Mapping.NotNull"));
            }
        }

        return entityField;
    }

    public EntityModel Create(IEntityModelInputs inputs)
    {
        ArgumentNullException.ThrowIfNull(inputs);

        var className = inputs.Identifier ?? IdentifierForTable(inputs.Table);
        bool omitSchema = inputs.SchemaIsDatabase ||
            (inputs.Config.OmitDefaultSchema == true &&
             !string.IsNullOrEmpty(inputs.DataSchema.DefaultSchema) &&
             inputs.Schema == inputs.DataSchema.DefaultSchema);

        string normalizeSchema(string name)
        {
            if (name != null &&
                omitSchema && string.Equals(name, inputs.DataSchema.DefaultSchema,
                StringComparison.OrdinalIgnoreCase))
                return null;

            return name;
        }

        var model = new EntityModel
        {
            ClassName = className,
            ConnectionKey = inputs.ConnectionKey,
            DeclareJoinConstants = inputs.Config.DeclareJoinConstants ?? false,
            EnableGenerateFields = inputs.Config.EnableGenerateFields ?? false,
            EnableRowTemplates = inputs.Config.EnableRowTemplates ?? false,
            GenerateListExcel = inputs.Config.GenerateUI,
            FileScopedNamespaces = inputs.Config.FileScopedNamespaces ?? false,
            Module = inputs.Module,
            NET5Plus = inputs.Net5Plus,
            Permission = inputs.PermissionKey,
            RootNamespace = inputs.Config.RootNamespace,
            RowClassName = className + "Row",
            Schema = omitSchema ? null : inputs.Schema,
            Tablename = inputs.Table,
            Title = Inflector.Inflector.Titleize(className)?.Trim(),
        };

        model.GlobalUsings.AddRange(inputs.GlobalUsings);

        var fieldInfos = inputs.DataSchema.GetFieldInfos(inputs.Schema, inputs.Table);
        if (!fieldInfos.Any(x => x.IsPrimaryKey))
        {
            var primaryKeys = new HashSet<string>(inputs.DataSchema.GetPrimaryKeyFields(inputs.Schema, inputs.Table));
            foreach (var field in fieldInfos)
                field.IsPrimaryKey = primaryKeys.Contains(field.FieldName);
        }

        if (!fieldInfos.Any(x => x.IsIdentity))
        {
            var identities = new HashSet<string>(inputs.DataSchema.GetIdentityFields(inputs.Schema, inputs.Table));
            foreach (var field in fieldInfos)
                field.IsIdentity = identities.Contains(field.FieldName);
        }

        var prefix = DeterminePrefixLength(fieldInfos, x => x.FieldName);
        model.FieldPrefix = prefix > 0 ? fieldInfos.First().FieldName[..prefix] : "";

        var idFieldInfo = fieldInfos.FirstOrDefault(f => f.IsIdentity == true);
        idFieldInfo ??= fieldInfos.FirstOrDefault(f => f.IsPrimaryKey == true);
        if (idFieldInfo != null)
            model.IdField = PropertyNameFor(idFieldInfo.FieldName[prefix..]);
        else
        {
            idFieldInfo = fieldInfos.FirstOrDefault(f => f.IsPrimaryKey == true) ??
                fieldInfos.FirstOrDefault();
            if (idFieldInfo != null)
                model.IdField = PropertyNameFor(idFieldInfo.FieldName[prefix..]);
        }

        var foreignKeyInfos = !inputs.SkipForeignKeys ?
            inputs.DataSchema.GetForeignKeys(inputs.Schema, inputs.Table)
                .ToLookup(x => x.FKName)
                .Where(x => x.Count() == 1)
                .SelectMany(x => x)
                .ToList() : [];

        foreach (var field in fieldInfos)
        {
            var fk = foreignKeyInfos.FirstOrDefault(x => x.FKColumn == field.FieldName);
            if (fk != null)
            {
                field.PKSchema = normalizeSchema(fk.PKSchema);
                field.PKTable = fk.PKTable;
                field.PKColumn = fk.PKColumn;
            }
        }

        string baseRowMatch = null;
        HashSet<string> baseRowFieldset = null;
        List<string> baseRowFieldList = [];
        if (inputs?.Config?.BaseRowClasses is { } baseRowClasses)
        {
            foreach (var k in inputs.Config.BaseRowClasses)
            {
                var b = k.ClassName;
                var f = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var fl = new List<string>();
                bool skip = false;
                foreach (var s in k.Fields ?? [])
                {
                    string n = s.TrimToNull();
                    if (n == null || !fieldInfos.Any(z => z.FieldName[prefix..] == n))
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

        var removeForeignFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // these fields are always removed from foreign views as it might be a security
            // concern for rows that have a fk to user table etc if the user is not careful
            "pwd",
            "pass",
            "passw",
            "password",
            "passwordhash",
            "passwordsalt"
        };

        var foreignSelection = inputs.Config.ForeignFieldSelection ?? GeneratorConfig.FieldSelection.All;

        if (!inputs.SkipForeignKeys && inputs.Config.RemoveForeignFields is { } removeFK)
        {
            removeForeignFields.AddRange(removeFK.Select(
                x => x.TrimToNull()).Where(x => x != null));
        }

        var includeForeignFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!inputs.SkipForeignKeys && inputs.Config.IncludeForeignFields is { } includeFK)
        {
            includeForeignFields.AddRange(includeFK.Select(
                x => x.TrimToNull()).Where(x => x != null));
        }

        if (baseRowFieldset != null &&
            baseRowFieldset.Count > 0)
        {
            model.RowBaseClass = baseRowMatch;
            model.FieldsBaseClass = baseRowMatch + "Fields";
            fieldInfos = fieldInfos.Where(f =>
            {
                if (baseRowFieldset.Contains(f.FieldName[prefix..]))
                {
                    var ef = ToEntityField(f, prefix, includeFlags: false);
                    model.RowBaseFields.Add(ef);
                    return false;
                }
                return true;
            }).ToList();
        }

        if (inputs.Net5Plus)
            model.RowBaseClass = model.RowBaseClass + "<" + model.RowClassName + ".RowFields>";

        var usedPropertyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void makeUniquePropertyName(EntityField f)
        {
            int i = 0;
            string propertyName = f.PropertyName;
            while (usedPropertyNames.Contains(propertyName))
                propertyName = f.PropertyName + ++i;
            f.PropertyName = propertyName;
            usedPropertyNames.Add(propertyName);
        }

        foreach (var fieldInfo in fieldInfos)
        {
            var tableField = ToEntityField(fieldInfo, prefix, includeFlags: true);

            if (tableField.PropertyName == model.IdField)
            {
                tableField.ColAttributeList.Add(new("Serenity.ComponentModel.EditLink"));
                tableField.ColAttributeList.Add(new("System.ComponentModel.DisplayName", "Db.Shared.RecordId"));
                tableField.ColAttributeList.Add(new("Serenity.ComponentModel.AlignRight"));
                tableField.OmitInForm = true;
            }

            makeUniquePropertyName(tableField);

            if (tableField.Name == className && tableField.FieldType == "String")
            {
                model.NameField = tableField.Name;
                tableField.ColAttributeList.Add(new AttributeTypeRef("Serenity.ComponentModel.EditLink"));
            }

            model.Fields.Add(tableField);
        }

        if (model.NameField == null)
        {
            var fld = model.Fields.FirstOrDefault(z => z.FieldType == "String");
            if (fld != null)
            {
                model.NameField = fld.PropertyName;
                fld.ColAttributeList.Add(new("Serenity.ComponentModel.EditLink"));
            }
        }

        foreach (var tableField in model.Fields)
        {
            var foreignKeyInfo = foreignKeyInfos.Find((k) => k.FKColumn.Equals(tableField.Name, StringComparison.OrdinalIgnoreCase));
            if (foreignKeyInfo is null)
                continue;

            if (tableField.Title.EndsWith(" Id", StringComparison.Ordinal) && tableField.Title.Length > 3)
                tableField.Title = tableField.Title.SafeSubstring(0, tableField.Title.Length - 3);

            tableField.PKSchema = normalizeSchema(foreignKeyInfo.PKSchema);
            tableField.PKTable = foreignKeyInfo.PKTable;
            tableField.PKColumn = foreignKeyInfo.PKColumn;

            var foreignFields = inputs.DataSchema.GetFieldInfos(foreignKeyInfo.PKSchema, foreignKeyInfo.PKTable).ToList();
            int foreignPrefixLength = DeterminePrefixLength(foreignFields, z => z.FieldName);
            var entityJoin = new EntityJoin
            {
                Name = PropertyNameFor(tableField.Name[prefix..])
            };

            if (entityJoin.Name.EndsWith("Id", StringComparison.Ordinal) || entityJoin.Name.EndsWith("ID", StringComparison.Ordinal))
                entityJoin.Name = entityJoin.Name[0..^2];

            tableField.ForeignJoinAlias = entityJoin.Alias;
            entityJoin.SourceField = tableField.PropertyName;

            var pkRow = inputs.Application?.GetRowByTablename(tableField.PKTable);
            var pkNameProperty = pkRow?.NameProperty;
            string pkNameFieldName = null;
            if (pkNameProperty != null &&
                foreignSelection != GeneratorConfig.FieldSelection.None)
            {
                var pkNameProp = pkRow.GetProperty(pkNameProperty);
                if (pkNameProp != null)
                {
                    var columnName = pkNameProp.ColumnName;
                    if (!string.IsNullOrEmpty(columnName) &&
                        foreignFields.Any(x => x.FieldName == columnName))
                    {
                        pkNameFieldName = columnName;
                    }
                    else
                    {
                        var nameViewField = new EntityField
                        {
                            PropertyName = pkNameProp.PropertyName,
                            Name = pkNameProp.ColumnName,
                            DataType = "string",
                            FieldType = "String",
                            TSType = "string"
                        };

                        nameViewField.Title = Inflector.Inflector.Titleize(JoinUnderscore(entityJoin.Name,
                            nameViewField.Name))?.Trim();

                        nameViewField.AttributeList.Add(new("System.ComponentModel.DisplayName",
                            nameViewField.Title));

                        object joinExpr = model.DeclareJoinConstants ?
                            new RawCode(entityJoin.Alias) : entityJoin.Alias;

                        nameViewField.AttributeList.Add(new("Serenity.Data.Mapping.Origin",
                            joinExpr, new NameOfRef(pkRow.FullName, pkNameProp.PropertyName)));

                        var propName = nameViewField.PropertyName;
                        nameViewField.PropertyName = propName.StartsWith(entityJoin.Name, StringComparison.Ordinal) &&
                            !usedPropertyNames.Contains(propName) ? propName : (entityJoin.Name + propName);

                        makeUniquePropertyName(nameViewField);

                        tableField.TextualField = nameViewField.PropertyName;
                        entityJoin.Fields.Add(nameViewField);
                    }
                }
            }

            foreach (var foreignField in foreignFields)
            {
                if (foreignField.FieldName.Equals(foreignKeyInfo.PKColumn, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (removeForeignFields.Contains(foreignField.FieldName))
                    continue;

                if (foreignSelection == GeneratorConfig.FieldSelection.None &&
                    !includeForeignFields.Contains(foreignField.FieldName))
                    continue;

                var viewField = ToEntityField(foreignField, foreignPrefixLength, includeFlags: false);

                var propName = viewField.PropertyName;
                viewField.PropertyName = propName.StartsWith(entityJoin.Name, StringComparison.Ordinal) &&
                    !usedPropertyNames.Contains(propName) ? propName : (entityJoin.Name + propName);

                makeUniquePropertyName(viewField);

                if (tableField.TextualField == null && (viewField.FieldType == "String" ||
                    (pkNameFieldName != null && foreignField.FieldName == pkNameFieldName)))
                    tableField.TextualField = viewField.PropertyName;
                else if (foreignSelection == GeneratorConfig.FieldSelection.NameOnly &&
                    !includeForeignFields.Contains(foreignField.FieldName))
                {
                    usedPropertyNames.Remove(viewField.PropertyName);
                    continue;
                }

                viewField.Title = Inflector.Inflector.Titleize(JoinUnderscore(entityJoin.Name,
                    foreignField.FieldName[foreignPrefixLength..]))?.Trim();

                viewField.AttributeList.Add(new("System.ComponentModel.DisplayName",
                    viewField.Title));

                viewField.Expression = entityJoin.Alias + ".[" + viewField.Name + "]";

                var originProp = pkRow?.GetTableField(foreignField.FieldName);
                if (originProp != null)
                {
                    object joinExpr = model.DeclareJoinConstants ?
                        new RawCode(entityJoin.Alias) : entityJoin.Alias;

                    viewField.AttributeList.Add(new("Serenity.Data.Mapping.Origin",
                        joinExpr, new NameOfRef(pkRow.FullName, originProp.PropertyName)));
                }
                else
                {
                    object expr = model.DeclareJoinConstants ?
                        new RawCode("$\"{" + entityJoin.Alias + "}.[" + viewField.Name + "]\"") :
                        (viewField.Expression);

                    viewField.AttributeList.Add(new("Serenity.Data.Mapping.Expression", expr));
                }

                entityJoin.Fields.Add(viewField);
            }

            model.Joins.Add(entityJoin);
        }

        foreach (var tableField in model.Fields)
        {
            var attrs = tableField.AttributeList;

            attrs.Add(new("System.ComponentModel.DisplayName", tableField.Title));

            if (tableField.PropertyName != tableField.Name)
                attrs.Add(new("Serenity.Data.Mapping.Column", tableField.Name));

            if ((tableField.Size ?? 0) > 0)
                attrs.Add(new("Serenity.Data.Mapping.Size", tableField.Size ?? 0));

            if (tableField.Scale > 0)
                attrs.Add(new("Serenity.Data.Mapping.Scale", tableField.Scale));

            if (!tableField.FlagList.IsEmptyOrNull())
                attrs.AddRange(tableField.FlagList);

            IRowMetadata pkRow = null;
            IRowPropertyMetadata pkProperty = null;
            bool pkPropertyIsId = false;
            if (!inputs.SkipForeignKeys && !string.IsNullOrEmpty(tableField.PKTable))
            {
                var pkTable = string.IsNullOrEmpty(tableField.PKSchema) ? tableField.PKTable :
                    ("[" + tableField.PKSchema + "].[" + tableField.PKTable + "]");
                pkRow = inputs.Application?.GetRowByTablename(pkTable);
                pkProperty = pkRow?.GetTableField(tableField.PKColumn);
                pkPropertyIsId = pkProperty != null && pkRow.IdProperty == pkProperty.PropertyName;

                if (pkRow != null)
                {
                    if (pkPropertyIsId)
                        attrs.Add(new("Serenity.Data.Mapping.ForeignKey", new TypeOfRef(pkRow.FullName)));
                    else
                        attrs.Add(new("Serenity.Data.Mapping.ForeignKey", new TypeOfRef(pkRow.FullName), tableField.PKColumn));
                }
                else
                    attrs.Add(new("Serenity.Data.Mapping.ForeignKey", pkTable, tableField.PKColumn));

                object alias = model.DeclareJoinConstants ?
                    new RawCode(tableField.ForeignJoinAlias) : tableField.ForeignJoinAlias;
                attrs.Add(new("Serenity.Data.Mapping.LeftJoin", alias));
            }

            if (model.IdField == tableField.PropertyName && inputs.Net5Plus)
                attrs.Add(new("Serenity.Data.IdProperty"));

            if (model.NameField == tableField.PropertyName)
            {
                attrs.Add(new("Serenity.Data.Mapping.QuickSearch"));
                if (inputs.Net5Plus)
                    attrs.Add(new("Serenity.Data.NameProperty"));
            }

            if (tableField.TextualField != null)
                attrs.Add(new("Serenity.Data.Mapping.TextualField",
                    new RawCode("nameof(" + tableField.TextualField + ")")));

            if (pkRow != null && pkPropertyIsId)
            {
                if (pkRow.HasLookupScriptAttribute)
                    attrs.Add(new("Serenity.ComponentModel.LookupEditor", new TypeOfRef(pkRow.FullName),
                        new RawCode("Async = true")));
                else
                {
                    var route = pkRow.ListServiceRoute;
                    if (!string.IsNullOrEmpty(route))
                    {
                        var defaultRoute = DefaultListServiceRoute(pkRow.FullName, pkRow.Module);
                        if (defaultRoute != route)
                        {
                            attrs.Add(new("Serenity.ComponentModel.ServiceLookupEditor",
                                new TypeOfRef(pkRow.FullName)));
                        }
                        else
                        {
                            if (route.StartsWith("Services/", StringComparison.Ordinal))
                                route = route[("Services/".Length)..];
                            else
                                route = "~/" + route;

                            attrs.Add(new("Serenity.ComponentModel.ServiceLookupEditor",
                                new TypeOfRef(pkRow.FullName), new RawCode("Service = " + route.ToDoubleQuoted())));
                        }
                    }
                }
            }
        }

        // define service lookup permission only if not multiple primary keys, which are likely M-N
        if (!string.IsNullOrEmpty(model.NameField) &&
            !string.IsNullOrEmpty(model.IdField) &&
            fieldInfos.Count(x => x.IsPrimaryKey == true) <= 1)
        {
            model.ServiceLookupPermission = model.Permission;
        }

        return model;
    }

    private static string DefaultListServiceRoute(string fullName, string module)
    {
        var name = fullName;
        if (name.EndsWith("Row", StringComparison.Ordinal))
            name = name[0..^("Row".Length)];
        var idx = name.LastIndexOf('.');
        if (idx >= 0)
        {
            if (module == null)
            {
                module = name[0..idx];
                if (module.EndsWith(".Entities"))
                    module = module[0..^9];
                else if (module.EndsWith(".Scripts"))
                    module = module[0..^8];
                else if (module.EndsWith(".Lookups"))
                    module = module[0..^8];

                var idx2 = module.IndexOf('.');
                if (idx2 >= 0)
                    module = module[(idx2 + 1)..];
            }
            name = name[(idx + 1)..];
        }
        else
            module = "";

        return "Services/" + (string.IsNullOrEmpty(module) ? name : module + "/" + name) + "/List";
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