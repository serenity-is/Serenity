using Serenity.Reflection;
using System.Collections.ObjectModel;
using System.Reflection.Emit;

namespace Serenity.Data;

/// <summary>
/// Base class for row fields collection
/// </summary>
/// <seealso cref="Collection{Field}" />
/// <seealso cref="IAlias" />
/// <seealso cref="IHaveJoins" />
public partial class RowFieldsBase : Collection<Field>, IAlias, IHaveJoins
{
    internal Dictionary<string, Field> byName;
    internal Dictionary<string, Field> byPropertyName;
    internal Dictionary<Type, Field[]> byAttribute;
    internal Field idField;
    internal Field[] primaryKeys;
    internal Field nameField;
    internal Tuple<Field, bool>[] sortOrders;
    internal bool isInitialized;
    internal string fieldPrefix;
    internal Dictionary<string, Join> joins;
    internal string localTextPrefix;
    internal PropertyChangedEventArgs[] propertyChangedEventArgs;
    internal Func<IRow> rowFactory;
    internal Type rowType;
    internal IAnnotatedType annotations;
    internal ISqlDialect dialect;
    internal string moduleIdentifier;
    internal string connectionKey;
    internal string generationKey;
    internal object initializeLock;
    internal string database;
    internal string schema;
    internal string tableOnly;
    internal string tableName;
    internal string alias;
    internal string aliasDot;
    internal bool aliasLocked;

    /// <summary>
    /// Initializes a new instance of the <see cref="RowFieldsBase"/> class.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="fieldPrefix">The field prefix.</param>
    protected RowFieldsBase(string tableName = null, string fieldPrefix = "")
    {
        this.tableName = tableName;
        alias = "T0";
        aliasDot = "T0.";
        this.fieldPrefix = fieldPrefix;
        byName = new Dictionary<string, Field>(StringComparer.OrdinalIgnoreCase);
        byPropertyName = new Dictionary<string, Field>(StringComparer.OrdinalIgnoreCase);
        joins = new Dictionary<string, Join>(StringComparer.OrdinalIgnoreCase);
        initializeLock = new object();

        DetermineRowType();
        DetermineConnectionKey();
        DetermineTableName(new DialectExpressionSelector(SqlSettings.DefaultDialect));
        DetermineModuleIdentifier();
        DetermineLocalTextPrefix();

        CreateGeneratedFields();
    }

    /// <summary>
    /// Should be only used by row source generator to create field objects it generated
    /// </summary>
    protected virtual void CreateGeneratedFields()
    {
    }

    private void DetermineRowType()
    {
        var fieldsType = GetType();
        if (!fieldsType.IsNested)
            throw new InvalidProgramException(string.Format(
                "RowFields type {0} must be a nested type!", fieldsType.Name));

        rowType = fieldsType.DeclaringType;
        if (!typeof(IRow).IsAssignableFrom(rowType) ||
            rowType.IsInterface)
            throw new InvalidProgramException(string.Format(
                "RowFields {0}'s declaring row type {0} must be a subclass of Row!", fieldsType.Name, rowType.Name));

        var constructor = rowType.GetConstructors().FirstOrDefault(x => x.GetParameters().Length == 1 &&
            x.GetParameters()[0].GetType().IsSubclassOf(typeof(RowFieldsBase)));

        if (constructor != null)
            rowFactory = () => (IRow)Activator.CreateInstance(rowType, this);
        else
            rowFactory = () => (IRow)Activator.CreateInstance(rowType);
    }

    private void DetermineTableName(DialectExpressionSelector expressionSelector)
    {
        var attr = expressionSelector.GetBestMatch(rowType.GetCustomAttributes<TableNameAttribute>(),
            x => x.Dialect);

        if (tableName != null)
        {
            if (attr != null && string.Compare(tableName, attr.Name, StringComparison.OrdinalIgnoreCase) != 0)
                throw new InvalidProgramException(string.Format(
                    "Tablename in row type {0} can't be overridden by attribute!",
                        rowType.Name));
        }
        else if (attr != null)
        {
            tableName = attr.Name;
        }
        else
        {

            var name = rowType.Name;
            if (name.EndsWith("Row"))
                name = name[0..^3];

            tableName = name;
        }

        tableOnly = ParseDatabaseAndSchema(tableName, out database, out schema);
    }

    /// <summary>
    /// Parses the database and schema.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="database">The database.</param>
    /// <param name="schema">The schema.</param>
    /// <returns></returns>
    public static string ParseDatabaseAndSchema(string tableName,
        out string database, out string schema)
    {
        database = null;
        schema = null;

        if (tableName == null)
            return null;

        var idx1 = tableName.IndexOf('.');
        if (idx1 < 0)
            return tableName;

        var idx2 = tableName.IndexOf('.', idx1 + 1);
        if (idx2 < 0)
        {
            schema = tableName[..idx1];
            return tableName[(idx1 + 1)..];
        }

        database = tableName[..idx1];
        schema = tableName.Substring(idx1 + 1, idx2 - idx1 - 1);
        return tableName[(idx2 + 1)..];
    }

    private void DetermineConnectionKey()
    {
        var connectionKeyAttr = rowType.GetCustomAttribute<ConnectionKeyAttribute>();
        if (connectionKeyAttr != null)
            connectionKey = connectionKeyAttr.Value;
        else
            connectionKey = "Default";
    }

    private void DetermineModuleIdentifier()
    {
        var moduleAttr = rowType.GetCustomAttribute<ModuleAttribute>();
        if (moduleAttr != null)
            moduleIdentifier = moduleAttr.Value;
        else
        {
            var ns = rowType.Namespace ?? "";

            if (ns.EndsWith(".Entities"))
                ns = ns[0..^9];

            var idx = ns.IndexOf(".");
            if (idx >= 0)
                ns = ns[(idx + 1)..];

            moduleIdentifier = ns;
        }
    }

    private void DetermineLocalTextPrefix()
    {
        if (localTextPrefix != null)
            return;

        var localTextPrefixAttr = rowType.GetCustomAttribute<LocalTextPrefixAttribute>();
        if (localTextPrefixAttr != null)
        {
            localTextPrefix = localTextPrefixAttr.Value;
            return;
        }

        localTextPrefix = RowIdentifier;
    }

    private void GetRowFieldsAndProperties(
        out Dictionary<string, FieldInfo> rowFields,
        out Dictionary<string, IPropertyInfo> rowProperties)
    {
        rowFields = new Dictionary<string, FieldInfo>(StringComparer.OrdinalIgnoreCase);
        rowProperties = new Dictionary<string, IPropertyInfo>(StringComparer.Ordinal);

        var members = rowType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var member in members)
        {
            var fi = member as FieldInfo;
            if (fi != null)
                rowFields[fi.Name] = fi;
            else
            {
                var pi = member as PropertyInfo;
                if (pi != null)
                {
                    rowProperties[pi.Name] = annotations != null ?
                        annotations.GetAnnotatedProperty(pi) : new WrappedProperty(pi);
                }
            }
        }
    }

    /// <summary>
    /// Afters the initialize.
    /// </summary>
    protected virtual void AfterInitialize()
    {
    }

    /// <summary>
    /// Initializes the specified annotations.
    /// </summary>
    /// <param name="annotations">The annotations.</param>
    /// <param name="dialect">The dialect.</param>
    /// <exception cref="ArgumentNullException">dialect</exception>
    /// <exception cref="InvalidProgramException">
    /// </exception>
    public void Initialize(IAnnotatedType annotations, ISqlDialect dialect)
    {
        if (isInitialized)
            return;

        lock (initializeLock)
        {
            this.annotations = annotations;
            GetRowFieldsAndProperties(out Dictionary<string, FieldInfo> rowFields, out Dictionary<string, IPropertyInfo> rowProperties);

            this.dialect = dialect ?? throw new ArgumentNullException(nameof(dialect));
            var expressionSelector = new DialectExpressionSelector(dialect);

            var rowCustomAttributes = rowType.GetCustomAttributes().ToList();

            DetermineTableName(expressionSelector);

            var fieldsReadPerm = rowType.GetCustomAttribute<FieldReadPermissionAttribute>();
            if (fieldsReadPerm != null && !fieldsReadPerm.ApplyToLookups)
                fieldsReadPerm = null; // ignore as need to specially handle in initialize

            PermissionAttributeBase fieldsModifyPerm = rowType.GetCustomAttribute<FieldModifyPermissionAttribute>();
            PermissionAttributeBase fieldsInsertPerm = rowType.GetCustomAttribute<FieldInsertPermissionAttribute>();
            PermissionAttributeBase fieldsUpdatePerm = rowType.GetCustomAttribute<FieldUpdatePermissionAttribute>();

            foreach (var fieldInfo in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (fieldInfo.FieldType.IsSubclassOf(typeof(Field)))
                {
                    var field = (Field)fieldInfo.GetValue(this);

                    if (!rowProperties.TryGetValue(fieldInfo.Name, out IPropertyInfo property))
                        property = null;

                    ColumnAttribute column = null;
                    DisplayNameAttribute display = null;
                    SizeAttribute size = null;
                    BaseExpressionAttribute expression = null;
                    ScaleAttribute scale = null;
                    MinSelectLevelAttribute selectLevel = null;
                    ForeignKeyAttribute foreignKey = null;
                    LeftJoinAttribute leftJoin = null;
                    InnerJoinAttribute innerJoin = null;
                    DefaultValueAttribute defaultValue = null;
                    TextualFieldAttribute textualField = null;
                    DateTimeKindAttribute dateTimeKind = null;

                    PermissionAttributeBase readPermission;
                    PermissionAttributeBase insertPermission;
                    PermissionAttributeBase updatePermission;

                    FieldFlags addFlags = 0;
                    FieldFlags removeFlags = 0;

                    OriginPropertyDictionary propertyDictionary = null;

                    if (property != null)
                    {
                        var origin = property.GetAttribute<OriginAttribute>();

                        column = property.GetAttribute<ColumnAttribute>();
                        display = property.GetAttribute<DisplayNameAttribute>();
                        size = property.GetAttribute<SizeAttribute>();

                        var expressions = property.GetAttributes<BaseExpressionAttribute>();
                        if (expressions.Any())
                        {
                            try
                            {
                                expression = expressionSelector.GetBestMatch(expressions, x => x is ExpressionAttribute exp ? exp.Dialect : null);
                            }
                            catch (AmbiguousMatchException ex)
                            {
                                throw new AmbiguousMatchException(string.Format(
                                    "Error while determining expression for row type {0}, property '{1}'",
                                    rowType.FullName, property.Name), ex);
                            }
                        }

                        scale = property.GetAttribute<ScaleAttribute>();
                        selectLevel = property.GetAttribute<MinSelectLevelAttribute>();
                        foreignKey = property.GetAttribute<ForeignKeyAttribute>();

                        try
                        {
                            leftJoin = expressionSelector.GetBestMatch(
                                property.GetAttributes<LeftJoinAttribute>()
                                    .Where(x => x.ToTable == null && x.OnCriteria == null),
                                x => x.Dialect);

                            innerJoin = expressionSelector.GetBestMatch(
                                property.GetAttributes<InnerJoinAttribute>()
                                    .Where(x => x.ToTable == null && x.OnCriteria == null),
                                x => x.Dialect);
                        }
                        catch (AmbiguousMatchException ex)
                        {
                            throw new AmbiguousMatchException(string.Format(
                                "Error while determining join attributes for row type {0}, property '{1}'",
                                    rowType.FullName, property.Name), ex);
                        }

                        defaultValue = property.GetAttribute<DefaultValueAttribute>();
                        textualField = property.GetAttribute<TextualFieldAttribute>();
                        dateTimeKind = property.GetAttribute<DateTimeKindAttribute>();
                        readPermission = property.GetAttribute<ReadPermissionAttribute>() ?? (PermissionAttributeBase)fieldsReadPerm;
                        insertPermission = property.GetAttribute<InsertPermissionAttribute>() ?? fieldsInsertPerm ??
                            property.GetAttribute<ModifyPermissionAttribute>() ?? fieldsModifyPerm ?? readPermission ?? fieldsReadPerm;
                        updatePermission = property.GetAttribute<UpdatePermissionAttribute>() ?? fieldsUpdatePerm ??
                            property.GetAttribute<ModifyPermissionAttribute>() ?? fieldsModifyPerm ?? readPermission ?? fieldsReadPerm;

                        if (origin != null)
                        {
                            propertyDictionary ??= OriginPropertyDictionary.GetPropertyDictionary(rowType);
                            try
                            {
                                if (!expressions.Any() && expression == null)
                                    expression = new ExpressionAttribute(propertyDictionary.OriginExpression(
                                        property.Name, origin, expressionSelector, "", rowCustomAttributes));

                                display ??= new DisplayNameAttribute(propertyDictionary.OriginDisplayName(
                                        property.Name, origin, expressionSelector));

                                size ??= propertyDictionary.OriginAttribute<SizeAttribute>(
                                        property.Name, expressionSelector);

                                scale ??= propertyDictionary.OriginAttribute<ScaleAttribute>(
                                        property.Name, expressionSelector);
                            }
                            catch (DivideByZeroException)
                            {
                                throw new InvalidProgramException(string.Format(
                                    "Infinite recursion detected while determining origins " +
                                    "for property '{0}' on row type '{1}'",
                                    property.Name, rowType.FullName));
                            }
                        }

                        var insertable = property.GetAttribute<InsertableAttribute>();
                        var updatable = property.GetAttribute<UpdatableAttribute>();

                        if (insertable != null && !insertable.Value)
                            removeFlags |= FieldFlags.Insertable;

                        if (updatable != null && !updatable.Value)
                            removeFlags |= FieldFlags.Updatable;

                        foreach (var attr in property.GetAttributes<SetFieldFlagsAttribute>())
                        {
                            addFlags |= attr.Add;
                            removeFlags |= attr.Remove;
                        }
                    }
                    else
                    {
                        readPermission = fieldsReadPerm;
                        insertPermission = fieldsInsertPerm ?? fieldsModifyPerm ?? fieldsReadPerm;
                        updatePermission = fieldsUpdatePerm ?? fieldsUpdatePerm ?? fieldsReadPerm;
                    }

                    if (fieldInfo.FieldType.GetCustomAttribute<NotMappedAttribute>() != null)
                        addFlags |= FieldFlags.NotMapped;

                    if (field is null)
                    {
                        if (property == null)
                        {
                            throw new InvalidProgramException(string.Format(
                                "Field {0} in type {1} is null and has no corresponding property in entity!",
                                    fieldInfo.Name, rowType.Name));
                        }

                        object[] prm = new object[7];
                        prm[0] = this; // owner
                        prm[1] = column == null ? property.Name : (column.Name.TrimToNull() ?? property.Name);
                        prm[2] = display != null ? new LocalText(display.DisplayName) : null;
                        prm[3] = size != null ? size.Value : 0;

                        prm[4] = (FieldFlags.Default ^ removeFlags) | addFlags;
                        prm[5] = null;
                        prm[6] = null;

                        if (rowFields.TryGetValue("_" + property.Name, out FieldInfo storage) ||
                            rowFields.TryGetValue("m_" + property.Name, out storage) ||
                            rowFields.TryGetValue(property.Name, out storage))
                        {
                            prm[5] = CreateFieldGetMethod(storage);
                            prm[6] = CreateFieldSetMethod(storage);
                        }

                        field = (Field)Activator.CreateInstance(fieldInfo.FieldType, prm);
                        fieldInfo.SetValue(this, field);
                    }
                    else
                    {
                        if (size != null && size.Value != field.Size)
                            field.Size = size.Value;

                        if (display != null)
                            field.Caption = new LocalText(display.DisplayName);

                        if (addFlags != 0 || removeFlags != 0)
                            field.Flags = (field.Flags ^ removeFlags) | addFlags;

                        if (column != null && string.Compare(column.Name, field.Name, StringComparison.OrdinalIgnoreCase) != 0)
                            throw new InvalidProgramException(string.Format(
                                "Field name '{0}' in type {1} can't be overridden by Column name attribute!",
                                    fieldInfo.Name, rowType.FullName));
                    }

                    if (scale != null)
                    {
                        field.Scale = scale.Value;
                    }

                    if (defaultValue != null)
                    {
                        field.DefaultValue = defaultValue.Value;
                    }

                    if (selectLevel != null)
                    {
                        field.MinSelectLevel = selectLevel.Value;
                    }

                    if (expression != null)
                    {
                        field.Expression = expression.ToString(dialect);
                    }

                    if (foreignKey != null)
                    {
                        field.ForeignTable = foreignKey.Table ??
                            expressionSelector.GetBestMatch(foreignKey.RowType
                                .GetCustomAttributes<TableNameAttribute>(), x => x.Dialect).Name;
                        field.ForeignField = foreignKey.Field;
                    }

                    if ((leftJoin != null || innerJoin != null) && field.ForeignTable.IsEmptyOrNull())
                        throw new InvalidProgramException(string.Format("Property {0} of row type {1} has a [LeftJoin] or [InnerJoin] attribute " +
                            "but its foreign table is undefined. Make sure it has a valid [ForeignKey] attribute!",
                                fieldInfo.Name, rowType.FullName));

                    if ((leftJoin != null || innerJoin != null) && field.ForeignField.IsEmptyOrNull())
                        throw new InvalidProgramException(string.Format("Property {0} of row type {1} has a [LeftJoin] or [InnerJoin] attribute " +
                            "but its foreign field is undefined. Make sure it has a valid [ForeignKey] attribute!",
                                fieldInfo.Name, rowType.FullName));

                    if (leftJoin != null)
                    {
                        field.ForeignJoinAlias = new LeftJoin(joins, field.ForeignTable, leftJoin.Alias,
                            new Criteria(leftJoin.Alias, field.ForeignField) == new Criteria(field));
                    }

                    if (innerJoin != null)
                    {
                        field.ForeignJoinAlias = new InnerJoin(joins, field.ForeignTable, innerJoin.Alias,
                            new Criteria(innerJoin.Alias, field.ForeignField) == new Criteria(field));
                    }

                    if (textualField != null)
                    {
                        field.textualField = textualField.Value;
                    }

                    if (dateTimeKind != null && field is DateTimeField dtf)
                    {
                        dtf.DateTimeKind = dateTimeKind.Value;
                    }

                    if (readPermission != null)
                    {
                        field.readPermission = readPermission.Permission ?? "?";
                    }

                    if (insertPermission != null)
                    {
                        field.insertPermission = insertPermission.Permission ?? "?";
                    }

                    if (updatePermission != null)
                    {
                        field.updatePermission = updatePermission.Permission ?? "?";
                    }

                    if (property != null)
                    {
                        if (property.PropertyType != null &&
                            field is IEnumTypeField)
                        {
                            if (property.PropertyType.IsEnum)
                            {
                                (field as IEnumTypeField).EnumType = property.PropertyType;
                            }
                            else
                            {
                                var nullableType = Nullable.GetUnderlyingType(property.PropertyType);
                                if (nullableType != null && nullableType.IsEnum)
                                    (field as IEnumTypeField).EnumType = nullableType;
                            }
                        }

                        var propJoinAttributes = ((IEnumerable<ISqlJoin>)property.GetAttributes<LeftJoinAttribute>())
                            .Concat(property.GetAttributes<InnerJoinAttribute>())
                            .Where(x => x.ToTable != null && x.OnCriteria != null)
                            .GroupBy(x => x.Alias, StringComparer.OrdinalIgnoreCase);

                        foreach (var propJoinGroup in propJoinAttributes)
                        {
                            ISqlJoin bestMatch;
                            try
                            {
                                bestMatch = expressionSelector.GetBestMatch(propJoinGroup, x => x.Dialect);
                            }
                            catch (AmbiguousMatchException ex)
                            {
                                throw new AmbiguousMatchException(string.Format(
                                    "Error while determining join attributes for row type {0}, property '{1}'",
                                        rowType.FullName, property.Name), ex);
                            }

                            if (bestMatch is LeftJoinAttribute lja)
                            {
                                new LeftJoin(joins, lja.ToTable, lja.Alias,
                                    new Criteria(lja.Alias, lja.OnCriteria) == new Criteria(field));
                            }
                            else if (bestMatch is InnerJoinAttribute ija)
                            {
                                new InnerJoin(joins, ija.ToTable, ija.Alias,
                                    new Criteria(ija.Alias, ija.OnCriteria) == new Criteria(field));
                            }
                        }

                        field.PropertyName = property.Name;
                        byPropertyName[field.PropertyName] = field;

                        field.customAttributes = property.GetAttributes<Attribute>().ToArray();
                    }

                    var idFieldAttribute = field.GetAttribute<IdPropertyAttribute>();
                    if (idFieldAttribute != null)
                    {
                        if (idField is not null)
                            throw new InvalidProgramException($"{rowType.FullName} have multiple [IdProperty] attributes!");

                        idField = field;
                    }

                    var nameFieldAttribute = field.GetAttribute<NamePropertyAttribute>();
                    if (nameFieldAttribute != null)
                    {
                        if (nameField is not null)
                            throw new InvalidProgramException($"{rowType.FullName} have multiple [NameProperty] attributes!");

                        nameField = field;
                    }
                }
            }

            var rowJoinAttributes = rowCustomAttributes.OfType<ISqlJoin>()
                .GroupBy(x => x.Alias, StringComparer.OrdinalIgnoreCase);

            foreach (var rowJoinGroup in rowJoinAttributes)
            {
                ISqlJoin bestMatch;
                try
                {
                    bestMatch = expressionSelector.GetBestMatch(rowJoinGroup, x => x.Dialect);
                }
                catch (AmbiguousMatchException ex)
                {
                    throw new AmbiguousMatchException(string.Format(
                        "Error while determining join attributes for row type {0}, join alias '{1}'",
                            rowType.FullName, rowJoinGroup.Key), ex);
                }

                if (bestMatch is LeftJoinAttribute lja)
                    new LeftJoin(joins, lja.ToTable, lja.Alias, new Criteria(lja.OnCriteria));
                else if (bestMatch is InnerJoinAttribute ija)
                    new InnerJoin(joins, ija.ToTable, ija.Alias, new Criteria(ija.OnCriteria));
                else if (bestMatch is OuterApplyAttribute oua)
                    new OuterApply(joins, oua.InnerQuery, oua.Alias);
            }

            primaryKeys = this.Where(x => x.flags.HasFlag(FieldFlags.PrimaryKey))
                .ToArray();

            if (idField is null)
            {
                var identity = this.Where(x => x.flags.HasFlag(FieldFlags.Identity));
                if (identity.Count() == 1)
                    idField = identity.First();
                else if (primaryKeys.Length == 1)
                    idField = primaryKeys[0];
            }

            foreach (var field in this)
            {
                if (ReferenceEquals(idField, field) ||
                    ReferenceEquals(nameField, field) ||
                    field.GetAttribute<LookupIncludeAttribute>() != null)
                    field.IsLookup = true;
            }

            InferTextualFields();
            AfterInitialize();
        }

        isInitialized = true;
    }

    /// <summary>
    /// Gets the fields by attribute.
    /// </summary>
    /// <typeparam name="TAttr">The type of the attribute.</typeparam>
    /// <returns></returns>
    public Field[] GetFieldsByAttribute<TAttr>()
        where TAttr : Attribute
    {
        return GetFieldsByAttribute(typeof(TAttr));
    }

    /// <summary>
    /// Gets the fields by attribute.
    /// </summary>
    /// <param name="attrType">Type of the attribute.</param>
    /// <returns></returns>
    public Field[] GetFieldsByAttribute(Type attrType)
    {
        var byAttribute = this.byAttribute;

        if (byAttribute != null &&
            byAttribute.TryGetValue(attrType, out Field[] fieldList))
            return fieldList;

        List<Field> newList = new();

        foreach (var field in this)
        {
            foreach (var attr in field.CustomAttributes)
            {
                if (attrType.IsAssignableFrom(attr.GetType()))
                {
                    newList.Add(field);
                    continue;
                }
            }
        }

        fieldList = newList.ToArray();

        if (byAttribute == null)
        {
            byAttribute = new Dictionary<Type, Field[]>
            {
                { attrType, fieldList }
            };
        }
        else
        {
            byAttribute = new Dictionary<Type, Field[]>(byAttribute)
            {
                [attrType] = fieldList
            };
        }

        this.byAttribute = byAttribute;
        return fieldList;
    }

    private static Delegate CreateFieldGetMethod(FieldInfo fieldInfo)
    {
        Type[] arguments = new Type[1];
        arguments[0] = typeof(IRow);

        var getter = new DynamicMethod(string.Concat("_Get", fieldInfo.Name, "_"),
            fieldInfo.FieldType, arguments, fieldInfo.DeclaringType);

        ILGenerator generator = getter.GetILGenerator();
        generator.Emit(OpCodes.Ldarg_0);
        generator.Emit(OpCodes.Castclass, fieldInfo.DeclaringType);
        generator.Emit(OpCodes.Ldfld, fieldInfo);
        generator.Emit(OpCodes.Ret);
        return getter.CreateDelegate(typeof(Func<,>).MakeGenericType(typeof(IRow), fieldInfo.FieldType));
    }

    private static Delegate CreateFieldSetMethod(FieldInfo fieldInfo)
    {
        Type[] arguments = new Type[2];
        arguments[0] = typeof(IRow);
        arguments[1] = fieldInfo.FieldType;

        var getter = new DynamicMethod(string.Concat("_Set", fieldInfo.Name, "_"),
            null, arguments, fieldInfo.DeclaringType);

        ILGenerator generator = getter.GetILGenerator();
        generator.Emit(OpCodes.Ldarg_0);
        generator.Emit(OpCodes.Castclass, fieldInfo.DeclaringType);
        generator.Emit(OpCodes.Ldarg_1);
        generator.Emit(OpCodes.Stfld, fieldInfo);
        generator.Emit(OpCodes.Ret);
        return getter.CreateDelegate(typeof(Action<,>).MakeGenericType(typeof(IRow), fieldInfo.FieldType));
    }

    private void InferTextualFields()
    {
        foreach (var field in this)
        {
            if (!field.ForeignTable.IsNullOrEmpty() &&
                field.TextualField == null)
            {
                foreach (var join in joins.Values)
                {
                    if (string.Compare(field.ForeignTable, join.Table) == 0 &&
                        (join is LeftJoin || join is InnerJoin) &&
                        join.OnCriteria is object &&
                        join.OnCriteria.ToStringIgnoreParams().IndexOf(field.Expression, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        foreach (var f in this)
                            if (string.Compare(f.JoinAlias, join.Name, StringComparison.OrdinalIgnoreCase) == 0 &&
                                f is StringField)
                            {
                                field.TextualField = f.Name;
                                break;
                            }
                    }

                    if (field.TextualField != null)
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    /// <value>
    /// The name of the table.
    /// </value>
    public string TableName => tableName;

    /// <summary>
    /// Gets the database.
    /// </summary>
    /// <value>
    /// The database.
    /// </value>
    public string Database => database;

    /// <summary>
    /// Gets the schema.
    /// </summary>
    /// <value>
    /// The schema.
    /// </value>
    public string Schema => schema;

    /// <summary>
    /// Gets the table only.
    /// </summary>
    /// <value>
    /// The table only.
    /// </value>
    public string TableOnly => tableOnly;

    /// <summary>
    /// Gets or sets the field prefix.
    /// </summary>
    /// <value>
    /// The field prefix.
    /// </value>
    public string FieldPrefix
    {
        get { return fieldPrefix ?? ""; }
        set { fieldPrefix = value; }
    }

    /// <summary>
    /// Gets or sets the local text prefix.
    /// </summary>
    /// <value>
    /// The local text prefix.
    /// </value>
    public string LocalTextPrefix
    {
        get { return localTextPrefix ?? RowIdentifier; }
        set { localTextPrefix = value; }
    }

    /// <summary>
    /// Gets the module identifier.
    /// </summary>
    /// <value>
    /// The module identifier.
    /// </value>
    public string ModuleIdentifier => moduleIdentifier;

    /// <summary>
    /// Gets the row identifier.
    /// </summary>
    /// <value>
    /// The row identifier.
    /// </value>
    public string RowIdentifier
    {
        get
        {
            var name = rowType.Name;
            if (name.EndsWith("Row"))
                name = name[0..^3];

            return string.IsNullOrEmpty(moduleIdentifier) ? name : moduleIdentifier + "." + name;
        }
    }

    /// <summary>
    /// Gets the connection key.
    /// </summary>
    /// <value>
    /// The connection key.
    /// </value>
    public string ConnectionKey => connectionKey;

    /// <summary>
    /// Gets the dialect.
    /// </summary>
    /// <value>
    /// The dialect.
    /// </value>
    public ISqlDialect Dialect => dialect;

    /// <summary>
    /// Gets or sets the generation key.
    /// </summary>
    /// <value>
    /// The generation key.
    /// </value>
    public string GenerationKey
    {
        get
        {
            if (generationKey != null)
                return generationKey;

            generationKey = (connectionKey + "." + TableName);
            return generationKey;
        }
        set
        {
            generationKey = value;
        }
    }

    /// <summary>
    /// Gets the identifier field.
    /// </summary>
    /// <value>
    /// The identifier field.
    /// </value>
    public Field IdField => idField;
    /// <summary>
    /// Gets the name field.
    /// </summary>
    /// <value>
    /// The name field.
    /// </value>
    public Field NameField => nameField;
    /// <summary>
    /// Gets the primary keys.
    /// </summary>
    /// <value>
    /// The primary keys.
    /// </value>
    public Field[] PrimaryKeys => primaryKeys;

    /// <summary>
    /// Gets the sort orders.
    /// </summary>
    /// <value>
    /// The sort orders.
    /// </value>
    public Tuple<Field, bool>[] SortOrders
    {
        get
        {
            if (sortOrders == null)
            {
                var list = new List<Tuple<Field, int>>();
                foreach (var field in this)
                {
                    var sortAttr = field.CustomAttributes
                        .OfType<SortOrderAttribute>().FirstOrDefault();

                    if (sortAttr != null)
                        list.Add(new Tuple<Field, int>(field, sortAttr.SortOrder));
                }

                sortOrders = list.OrderBy(x => Math.Abs(x.Item2))
                    .Select(x => new Tuple<Field, bool>(x.Item1, x.Item2 < 0))
                    .ToArray();
            }

            return sortOrders;
        }
    }

    /// <summary>
    /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1" /> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
    /// <param name="item">The object to insert. The value can be <see langword="null" /> for reference types.</param>
    /// <exception cref="InvalidOperationException">field collection can't be modified!</exception>
    /// <exception cref="ArgumentNullException">item</exception>
    /// <exception cref="ArgumentOutOfRangeException">item</exception>
    protected override void InsertItem(int index, Field item)
    {
        if (isInitialized)
            throw new InvalidOperationException("field collection can't be modified!");

        if (item is null)
            throw new ArgumentNullException("item");

        if (byName.ContainsKey(item.Name))
            throw new ArgumentOutOfRangeException("item",
                string.Format("field list already contains a field with name '{0}'", item.Name));

        item.Fields?.Remove(item);

        base.InsertItem(index, item);

        item.fields = this;
        item.Index = index;

        byName[item.Name] = item;
    }

    /// <summary>
    /// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1" />.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <exception cref="InvalidOperationException">field collection can't be modified!</exception>
    protected override void RemoveItem(int index)
    {
        if (isInitialized)
            throw new InvalidOperationException("field collection can't be modified!");

        var item = base[index];
        base.RemoveItem(index);
        item.Index = -1;
        item.fields = null;
        byName.Remove(item.Name);
        for (int i = index; i < Count; i++)
            this[i].index = i;
    }

    /// <summary>
    /// Replaces the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to replace.</param>
    /// <param name="item">The new value for the element at the specified index. The value can be <see langword="null" /> for reference types.</param>
    /// <exception cref="InvalidOperationException">field collection can't be modified!</exception>
    /// <exception cref="ArgumentNullException">item</exception>
    /// <exception cref="ArgumentOutOfRangeException">item</exception>
    protected override void SetItem(int index, Field item)
    {
        if (isInitialized)
            throw new InvalidOperationException("field collection can't be modified!");

        if (item is null)
            throw new ArgumentNullException("item");

        if (byName.ContainsKey(item.Name))
            throw new ArgumentOutOfRangeException("item",
                string.Format("field list already contains a field with name '{0}'", item.Name));

        var old = base[index];

        base.SetItem(index, item);

        old.Index = -1;
        old.fields = null;
        byName.Remove(old.Name);

        item.Index = index;
        item.fields = this;
        byName[item.Name] = item;
    }

    /// <summary>
    /// Finds the field.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns></returns>
    public Field FindField(string fieldName)
    {
        if (byName.TryGetValue(fieldName, out Field field))
            return field;
        else
            return null;
    }

    private bool calledRowCreated;

    internal void RowCreated(IRow row)
    {
        if (calledRowCreated)
            return;

        if (row is IIdRow && idField is null)
            throw new ArgumentOutOfRangeException(nameof(IdField),
                $"Row type {GetType().FullName} has IIdRow interface but does not have a field with [IdProperty] attribute!");

        if (row is INameRow && nameField is null)
            throw new ArgumentOutOfRangeException(nameof(IdField),
                $"Row type {GetType().FullName} has INameRow interface but does not have a field with [NameProperty] attribute!");

        var readPerm = rowType.GetCustomAttribute<FieldReadPermissionAttribute>();
        if (readPerm != null && readPerm.Permission != null && !readPerm.ApplyToLookups)
        {
            var permission = readPerm.Permission;
            foreach (var field in this)
            {
                if (field.ReadPermission == null &&
                    !ReferenceEquals(idField, field) &&
                    !ReferenceEquals(nameField, field) &&
                    field.GetAttribute<LookupIncludeAttribute>() == null)
                {
                    field.ReadPermission = permission;
                    field.InsertPermission ??= permission;
                    field.UpdatePermission ??= permission;
                }
            }
        }

        calledRowCreated = true;
    }

    /// <summary>
    /// Finds the name of the field by property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public Field FindFieldByPropertyName(string propertyName)
    {
        if (byPropertyName.TryGetValue(propertyName, out Field field))
            return field;
        else
            return null;
    }

    /// <summary>
    /// Locks the alias.
    /// </summary>
    public void LockAlias()
    {
        aliasLocked = true;
    }

    /// <summary>
    /// Replaces the alias with.
    /// </summary>
    /// <param name="newAlias">The new alias.</param>
    /// <exception cref="ArgumentNullException">newAlias</exception>
    /// <exception cref="InvalidOperationException">Please use As() method to alias this fields object!</exception>
    public void ReplaceAliasWith(string newAlias)
    {
        if (string.IsNullOrEmpty(newAlias))
            throw new ArgumentNullException(nameof(newAlias));

        if (aliasLocked)
            throw new InvalidOperationException(
                "Please use As() method to alias this fields object!");

        if (newAlias == alias)
        {
            aliasLocked = true;
            return;
        }

        var aliasPrefix = newAlias + "_";

        var joinByKey = new HashSet<string>(joins.Keys, StringComparer.OrdinalIgnoreCase);

        string mapAlias(string x)
        {
            if (x == "t0" || x == "T0")
                return newAlias;

            if (!joinByKey.Contains(x))
                return x;

            return aliasPrefix + x;
        }

        string mapExpression(string x)
        {
            if (x == null)
                return null;

            return JoinAliasLocator.ReplaceAliases(x, mapAlias);
        }

        aliasLocked = true;

        foreach (var field in this)
        {
            field.expression = mapExpression(field.expression);

            if (field.referencedAliases != null && field.ReferencedAliases.Count > 0)
            {
                var old = field.ReferencedAliases.ToArray();
                field.ReferencedAliases.Clear();
                foreach (var x in old)
                    field.ReferencedAliases.Add(mapAlias(x));
            }

            field.join = null;
            field.joinAlias = field.joinAlias == null ? null : mapAlias(field.joinAlias);
        }

        var oldJoins = joins.ToArray();
        joins.Clear();
        foreach (var join in oldJoins)
        {
            BaseCriteria onCriteria;
            if (join.Value.OnCriteria is BinaryCriteria bc)
                onCriteria = new BinaryCriteria(
                    new Criteria(mapExpression(bc.LeftOperand.ToString())),
                    bc.Operator,
                    new Criteria(mapExpression(bc.RightOperand.ToString())));
            else
            {
                if (join.Value.OnCriteria is null)
                    onCriteria = null;
                else
                    onCriteria = new Criteria(mapExpression(join.Value.OnCriteria.ToString()));
            }

            new ReplacedJoin(joins,
                mapExpression(join.Value.Table),
                mapAlias(join.Value.Name),
                onCriteria,
                join.Value.GetKeyword());
        }

        alias = newAlias;
        aliasDot = newAlias + ".";
    }

    private class ReplacedJoin : Join
    {
        private readonly string keyword;

        public ReplacedJoin(IDictionary<string, Join> joins,
            string toTable, string alias, ICriteria onCriteria, string keyword)
            : base(joins, toTable, alias, onCriteria)
        {
            this.keyword = keyword;
        }

        public override string GetKeyword()
        {
            return keyword;
        }
    }

    /// <summary>
    /// List of all joins in entity
    /// </summary>
    public IDictionary<string, Join> Joins => joins;

    string IAlias.Name => alias;

    string IAlias.NameDot => aliasDot;

    string IAlias.Table => tableName;

    /// <summary>
    /// Gets the name of the alias.
    /// </summary>
    /// <value>
    /// The name of the alias.
    /// </value>
    public string AliasName => alias;
}