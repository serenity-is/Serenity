using Serenity.ComponentModel;
using Serenity.Data.Mapping;
using Serenity.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Serenity.Data
{
    public partial class RowFieldsBase : Collection<Field>, IAlias, IHaveJoins
    {
        internal Dictionary<string, Field> byName;
        internal Dictionary<string, Field> byPropertyName;
        internal Dictionary<Type, Field[]> byAttribute;
        internal Field[] primaryKeys;
        internal Tuple<Field, bool>[] sortOrders;
        internal bool isInitialized;
        internal string fieldPrefix;
        internal Dictionary<string, Join> joins;
        internal string localTextPrefix;
        internal PropertyChangedEventArgs[] propertyChangedEventArgs;
#if NET45
        internal PropertyDescriptorCollection propertyDescriptors;
#endif
        internal Func<Row> rowFactory;
        internal Type rowType;
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

        protected RowFieldsBase(string tableName = null, string fieldPrefix = "")
        {
            this.tableName = tableName;
            this.alias = "T0";
            this.aliasDot = "T0.";
            this.fieldPrefix = fieldPrefix;
            this.byName = new Dictionary<string, Field>(StringComparer.OrdinalIgnoreCase);
            this.byPropertyName = new Dictionary<string, Field>(StringComparer.OrdinalIgnoreCase);
            this.joins = new Dictionary<string, Join>(StringComparer.OrdinalIgnoreCase);
            this.initializeLock = new object();

            DetermineRowType();
            DetermineTableName();
            DetermineDatabaseAndSchema();
            DetermineConnectionKey();
            DetermineModuleIdentifier();
            DetermineLocalTextPrefix();
        }

        private void DetermineRowType()
        {
            var fieldsType = this.GetType();
            if (!fieldsType.IsNested)
                throw new InvalidProgramException(String.Format(
                    "RowFields type {0} must be a nested type!", fieldsType.Name));

            this.rowType = fieldsType.DeclaringType;
            if (!this.rowType.IsSubclassOf(typeof(Row)))
                throw new InvalidProgramException(String.Format(
                    "RowFields {0}'s declaring row type {0} must be a subclass of Row!", fieldsType.Name, this.rowType.Name));

            var constructor = this.rowType.GetConstructor(Type.EmptyTypes);
            if (constructor != null)
            {
                var method = new DynamicMethod("", typeof(Row), Type.EmptyTypes);
                var il = method.GetILGenerator();
                il.Emit(OpCodes.Newobj, constructor);
                il.Emit(OpCodes.Ret);
                this.rowFactory = (Func<Row>)method.CreateDelegate(typeof(Func<>).MakeGenericType(typeof(Row)));
            }
        }

        private void DetermineTableName()
        {
            var attr = this.rowType.GetCustomAttribute<TableNameAttribute>();

            if (tableName != null)
            {
                if (attr != null && String.Compare(tableName, attr.Name, StringComparison.OrdinalIgnoreCase) != 0)
                    throw new InvalidProgramException(String.Format(
                        "Tablename in row type {0} can't be overridden by attribute!",
                            rowType.Name));

                return;
            }

            if (attr != null)
            {
                tableName = attr.Name;
                return;
            }

            var name = rowType.Name;
            if (name.EndsWith("Row"))
                name = name.Substring(0, name.Length - 3);

            tableName = name;
        }

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
                schema = tableName.Substring(0, idx1);
                return tableName.Substring(idx1 + 1);
            }

            database = tableName.Substring(0, idx1);
            schema = tableName.Substring(idx1 + 1, idx2 - idx1 - 1);
            return tableName.Substring(idx2 + 1);
        }

        private void DetermineDatabaseAndSchema()
        {
            this.tableOnly = ParseDatabaseAndSchema(this.tableName, out this.database, out this.schema);
        }


        private void DetermineConnectionKey()
        {
            var connectionKeyAttr = rowType.GetCustomAttribute<ConnectionKeyAttribute>();
            if (connectionKeyAttr != null)
                this.connectionKey = connectionKeyAttr.Value;
            else
                this.connectionKey = "Default";
        }

        private void DetermineModuleIdentifier()
        {
            var moduleAttr = rowType.GetCustomAttribute<ModuleAttribute>();
            if (moduleAttr != null)
                this.moduleIdentifier = moduleAttr.Value;
            else
            {
                var ns = rowType.Namespace ?? "";

                if (ns.EndsWith(".Entities"))
                    ns = ns.Substring(0, ns.Length - 9);

                var idx = ns.IndexOf(".");
                if (idx >= 0)
                    ns = ns.Substring(idx + 1);

                this.moduleIdentifier = ns;
            }
        }

        private void DetermineLocalTextPrefix()
        {
            if (localTextPrefix != null)
                return;

            var localTextPrefixAttr = rowType.GetCustomAttribute<LocalTextPrefixAttribute>();
            if (localTextPrefixAttr != null)
            {
                this.localTextPrefix = localTextPrefixAttr.Value;
                return;
            }

            this.localTextPrefix = this.RowIdentifier;
        }

        private void GetRowFieldsAndProperties(
            out Dictionary<string, FieldInfo> rowFields,
            out Dictionary<string, IPropertyInfo> rowProperties)
        {
            var annotationTypes = Dependency.TryResolve<IAnnotationTypeRegistry>();
            var annotationType = annotationTypes != null ? annotationTypes.GetAnnotatedType(rowType) : null;
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
                        rowProperties[pi.Name] = annotationType != null ? 
                            annotationType.GetAnnotatedProperty(pi) : new WrappedProperty(pi);
                    }
                }
            }
        }

        protected virtual void AfterInitialize()
        {
        }

        public void Initialize()
        {
            if (isInitialized)
                return;

            lock (this.initializeLock)
            {
                Dictionary<string, FieldInfo> rowFields;
                Dictionary<string, IPropertyInfo> rowProperties;
                GetRowFieldsAndProperties(out rowFields, out rowProperties);

                var expressionSelector = new DialectExpressionSelector(connectionKey);
                var rowCustomAttributes = rowType.GetCustomAttributes().ToList();

                var fieldsReadPerm = rowType.GetCustomAttribute<FieldReadPermissionAttribute>();
                if (fieldsReadPerm != null && fieldsReadPerm.ApplyToLookups)
                    fieldsReadPerm = null; // ignore as need to specially handle in initialize

                PermissionAttributeBase fieldsModifyPerm = rowType.GetCustomAttribute<FieldModifyPermissionAttribute>();
                PermissionAttributeBase fieldsInsertPerm = rowType.GetCustomAttribute<FieldInsertPermissionAttribute>();
                PermissionAttributeBase fieldsUpdatePerm = rowType.GetCustomAttribute<FieldUpdatePermissionAttribute>();

                foreach (var fieldInfo in this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (fieldInfo.FieldType.IsSubclassOf(typeof(Field)))
                    {
                        var field = (Field)fieldInfo.GetValue(this);

                        IPropertyInfo property;
                        if (!rowProperties.TryGetValue(fieldInfo.Name, out property))
                            property = null;

                        ColumnAttribute column = null;
                        DisplayNameAttribute display = null;
                        SizeAttribute size = null;
                        ExpressionAttribute expression = null;
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

                        FieldFlags addFlags = (FieldFlags)0;
                        FieldFlags removeFlags = (FieldFlags)0;

                        OriginPropertyDictionary propertyDictionary = null;

                        if (property != null)
                        {
                            var origin = property.GetAttribute<OriginAttribute>();

                            column = property.GetAttribute<ColumnAttribute>();
                            display = property.GetAttribute<DisplayNameAttribute>();
                            size = property.GetAttribute<SizeAttribute>();

                            var expressions = property.GetAttributes<ExpressionAttribute>();
                            if (expressions.Any())
                                expression = expressionSelector.GetBestMatch(expressions, x => x.Dialect);

                            scale = property.GetAttribute<ScaleAttribute>();
                            selectLevel = property.GetAttribute<MinSelectLevelAttribute>();
                            foreignKey = property.GetAttribute<ForeignKeyAttribute>();
                            leftJoin = property.GetAttributes<LeftJoinAttribute>()
                                .FirstOrDefault(x => x.ToTable == null && x.OnCriteria == null);
                            innerJoin = property.GetAttributes<InnerJoinAttribute>()
                                .FirstOrDefault(x => x.ToTable == null && x.OnCriteria == null);
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
                                propertyDictionary = propertyDictionary ?? OriginPropertyDictionary.GetPropertyDictionary(this.rowType);
                                try
                                {
                                    if (!expressions.Any() && expression == null)
                                        expression = new ExpressionAttribute(propertyDictionary.OriginExpression(
                                            property.Name, origin, expressionSelector, "", rowCustomAttributes));

                                    if (display == null)
                                        display = new DisplayNameAttribute(propertyDictionary.OriginDisplayName(property.Name, origin));

                                    if (size == null)
                                        size = propertyDictionary.OriginAttribute<SizeAttribute>(property.Name, origin);

                                    if (scale == null)
                                        scale = propertyDictionary.OriginAttribute<ScaleAttribute>(property.Name, origin);
                                }
                                catch (DivideByZeroException)
                                {
                                    throw new InvalidProgramException(String.Format(
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

                        if (ReferenceEquals(null, field))
                        {
                            if (property == null)
                            {
                                throw new InvalidProgramException(String.Format(
                                    "Field {0} in type {1} is null and has no corresponding property in entity!",
                                        fieldInfo.Name, rowType.Name));
                            }

                            object[] prm = new object[7];
                            prm[0] = this; // owner
                            prm[1] = column == null ? property.Name : (column.Name.TrimToNull() ?? property.Name);
                            prm[2] = display != null ? new LocalText(display.DisplayName) : null;
                            prm[3] = size != null ? size.Value : 0;

                            var defaultFlags = FieldFlags.Default;
                            if (fieldInfo.FieldType.GetCustomAttribute<NotMappedAttribute>() != null)
                                defaultFlags |= FieldFlags.NotMapped;

                            prm[4] = (defaultFlags ^ removeFlags) | addFlags;
                            prm[5] = null;
                            prm[6] = null;

                            FieldInfo storage;
                            if (rowFields.TryGetValue("_" + property.Name, out storage) ||
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
                            if (size != null)
                                throw new InvalidProgramException(String.Format(
                                    "Field size '{0}' in type {1} can't be overridden by Size attribute!",
                                        fieldInfo.Name, rowType.FullName));

                            if (display != null)
                                field.Caption = new LocalText(display.DisplayName);

                            if ((int)addFlags != 0 || (int)removeFlags != 0)
                                field.Flags = (field.Flags ^ removeFlags) | addFlags;

                            if (column != null && String.Compare(column.Name, field.Name, StringComparison.OrdinalIgnoreCase) != 0)
                                throw new InvalidProgramException(String.Format(
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
                            field.Expression = expression.Value;
                        }

                        if (foreignKey != null)
                        {
                            field.ForeignTable = foreignKey.Table;
                            field.ForeignField = foreignKey.Field;
                        }

                        if ((leftJoin != null || innerJoin != null) && field.ForeignTable.IsEmptyOrNull())
                            throw new InvalidProgramException(String.Format("Property {0} of row type {1} has a [LeftJoin] or [InnerJoin] attribute " +
                                "but its foreign table is undefined. Make sure it has a valid [ForeignKey] attribute!",
                                    fieldInfo.Name, rowType.FullName));

                        if ((leftJoin != null || innerJoin != null) && field.ForeignField.IsEmptyOrNull())
                            throw new InvalidProgramException(String.Format("Property {0} of row type {1} has a [LeftJoin] or [InnerJoin] attribute " +
                                "but its foreign field is undefined. Make sure it has a valid [ForeignKey] attribute!",
                                    fieldInfo.Name, rowType.FullName));

                        if (leftJoin != null)
                        {
                            field.ForeignJoinAlias = new LeftJoin(this.joins, field.ForeignTable, leftJoin.Alias,
                                new Criteria(leftJoin.Alias, field.ForeignField) == new Criteria(field));
                        }

                        if (innerJoin != null)
                        {
                            field.ForeignJoinAlias = new InnerJoin(this.joins, field.ForeignTable, innerJoin.Alias,
                                new Criteria(innerJoin.Alias, field.ForeignField) == new Criteria(field));
                        }

                        if (textualField != null)
                        {
                            field.textualField = textualField.Value;
                        }

                        if (dateTimeKind != null && field is DateTimeField)
                        {
                            ((DateTimeField)field).DateTimeKind = dateTimeKind.Value;
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

                            foreach (var attr in property.GetAttributes<LeftJoinAttribute>())
                                if (attr.ToTable != null && attr.OnCriteria != null)
                                    new LeftJoin(this.joins, attr.ToTable, attr.Alias,
                                        new Criteria(attr.Alias, attr.OnCriteria) == new Criteria(field));

                            foreach (var attr in property.GetAttributes<InnerJoinAttribute>())
                                if (attr.ToTable != null && attr.OnCriteria != null)
                                    new InnerJoin(this.joins, attr.ToTable, attr.Alias,
                                        new Criteria(attr.Alias, attr.OnCriteria) == new Criteria(field));

                            field.PropertyName = property.Name;
                            this.byPropertyName[field.PropertyName] = field;

                            field.customAttributes = property.GetAttributes<Attribute>().ToArray();
                        }
                    }
                }

                foreach (var attr in rowCustomAttributes.OfType<LeftJoinAttribute>())
                    new LeftJoin(this.joins, attr.ToTable, attr.Alias, new Criteria(attr.OnCriteria));

                foreach (var attr in rowCustomAttributes.OfType<InnerJoinAttribute>())
                    new InnerJoin(this.joins, attr.ToTable, attr.Alias, new Criteria(attr.OnCriteria));

                foreach (var attr in rowCustomAttributes.OfType<OuterApplyAttribute>())
                    new OuterApply(this.joins, attr.InnerQuery, attr.Alias);

#if NET45
                var propertyDescriptorArray = new PropertyDescriptor[this.Count];
                for (int i = 0; i < this.Count; i++)
                {
                    var field = this[i];
                    propertyDescriptorArray[i] = new FieldDescriptor(field);
                }

                this.propertyDescriptors = new PropertyDescriptorCollection(propertyDescriptorArray);
#endif

                InferTextualFields();
                AfterInitialize();
            }

            isInitialized = true;
        }

        private Field[] EmptyFields = new Field[0];

        public Field[] GetFieldsByAttribute<TAttr>()
            where TAttr : Attribute
        {
            return GetFieldsByAttribute(typeof(TAttr));
        }

        public Field[] GetFieldsByAttribute(Type attrType)
        {
            var byAttribute = this.byAttribute;

            Field[] fieldList;
            if (byAttribute != null &&
                byAttribute.TryGetValue(attrType, out fieldList))
                return fieldList;

            List<Field> newList = new List<Field>();

            foreach (var field in this)
            {
                if (field.CustomAttributes == null)
                    continue;

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
                byAttribute = new Dictionary<Type, Field[]>();
                byAttribute.Add(attrType, fieldList);
            }
            else
            {
                byAttribute = new Dictionary<Type, Field[]>(byAttribute);
                byAttribute[attrType] = fieldList; 
            }

            this.byAttribute = byAttribute;
            return fieldList;
        }

        private static TAttr GetFieldAttr<TAttr>(Field x)
            where TAttr: Attribute
        {
            return x.CustomAttributes.FirstOrDefault(z => typeof(TAttr).IsAssignableFrom(z.GetType())) as TAttr;
        }

        private static Delegate CreateFieldGetMethod(FieldInfo fieldInfo)
        {
            Type[] arguments = new Type[1];
            arguments[0] = typeof(Row);

            var getter = new DynamicMethod(String.Concat("_Get", fieldInfo.Name, "_"),
                fieldInfo.FieldType, arguments, fieldInfo.DeclaringType);

            ILGenerator generator = getter.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass, fieldInfo.DeclaringType);
            generator.Emit(OpCodes.Ldfld, fieldInfo);
            generator.Emit(OpCodes.Ret);
            return getter.CreateDelegate(typeof(Func<,>).MakeGenericType(typeof(Row), fieldInfo.FieldType));
        }

        private static Delegate CreateFieldSetMethod(FieldInfo fieldInfo)
        {
            Type[] arguments = new Type[2];
            arguments[0] = typeof(Row);
            arguments[1] = fieldInfo.FieldType;

            var getter = new DynamicMethod(String.Concat("_Set", fieldInfo.Name, "_"),
                null, arguments, fieldInfo.DeclaringType);

            ILGenerator generator = getter.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass, fieldInfo.DeclaringType);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, fieldInfo);
            generator.Emit(OpCodes.Ret);
            return getter.CreateDelegate(typeof(Action<,>).MakeGenericType(typeof(Row), fieldInfo.FieldType));
        }

        private void InferTextualFields()
        {
            foreach (var field in this)
            {
                if (!field.ForeignTable.IsNullOrEmpty() &&
                    field.TextualField == null)
                {
                    foreach (var join in this.joins.Values)
                    {
                        if (String.Compare(field.ForeignTable, join.Table) == 0 &&
                            (join is LeftJoin || join is InnerJoin) &&
                            !Object.ReferenceEquals(null, join.OnCriteria) &&
                            join.OnCriteria.ToStringIgnoreParams().IndexOf(field.Expression, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            foreach (var f in this)
                                if (String.Compare(f.JoinAlias, join.Name, StringComparison.OrdinalIgnoreCase) == 0 &&
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

        public string TableName
        {
            get { return tableName; }
        }

        public string Database
        {
            get { return database; }
        }

        public string Schema
        {
            get { return schema; }
        }

        public string TableOnly
        {
            get { return tableOnly; }
        }

        public string FieldPrefix
        {
            get { return fieldPrefix ?? ""; }
            set { fieldPrefix = value; }
        }

        public string LocalTextPrefix
        {
            get { return localTextPrefix ?? RowIdentifier; }
            set { localTextPrefix = value; }
        }

        public string ModuleIdentifier
        {
            get { return moduleIdentifier; }
        }

        public string RowIdentifier
        {
            get
            {
                var name = rowType.Name;
                if (name.EndsWith("Row"))
                    name = name.Substring(0, name.Length - 3);

                return string.IsNullOrEmpty(moduleIdentifier) ? name : moduleIdentifier + "." + name;
            }
        }

        public string ConnectionKey
        {
            get { return connectionKey; }
        }

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

        public Field[] PrimaryKeys
        {
            get
            {
                if (primaryKeys == null)
                {
                    primaryKeys = this.Where(x => x.flags.HasFlag(FieldFlags.PrimaryKey))
                        .ToArray();
                }

                return primaryKeys;
            }
        }

        public Tuple<Field, bool>[] SortOrders
        {
            get
            {
                if (sortOrders == null)
                {
                    var list = new List<Tuple<Field, int>>();
                    foreach (var field in this)
                    {
                        if (field.CustomAttributes == null)
                            continue;

                        var sortAttr = field.CustomAttributes
                            .OfType<SortOrderAttribute>().FirstOrDefault();

                        if (sortAttr != null)
                            list.Add(new Tuple<Field, int>(field, sortAttr.SortOrder));
                    }

                    sortOrders = list.OrderBy(x => Math.Abs(x.Item2))
                        .Select(x => new Tuple<Field, bool>(x.Item1, x.Item2 < 0 ? true : false))
                        .ToArray();
                }

                return sortOrders;
            }
        }

        protected override void InsertItem(int index, Field item)
        {
            if (isInitialized)
                throw new InvalidOperationException("field collection can't be modified!");

            if (ReferenceEquals(null, item))
                throw new ArgumentNullException("item");

            if (byName.ContainsKey(item.Name))
                throw new ArgumentOutOfRangeException("item",
                    String.Format("field list already contains a field with name '{0}'", item.Name));

            if (item.Fields != null)
                item.Fields.Remove(item);

            base.InsertItem(index, item);

            item.Fields = this;
            item.Index = index;

            byName[item.Name] = item;
        }

        protected override void RemoveItem(int index)
        {
            if (isInitialized)
                throw new InvalidOperationException("field collection can't be modified!");

            var item = base[index];
            base.RemoveItem(index);
            item.Index = -1;
            item.Fields = null;
            byName.Remove(item.Name);
            for (int i = index; i < Count; i++)
                this[i].index = i;
        }

        protected override void SetItem(int index, Field item)
        {
            if (isInitialized)
                throw new InvalidOperationException("field collection can't be modified!");

            if (ReferenceEquals(null, item))
                throw new ArgumentNullException("item");

            if (byName.ContainsKey(item.Name))
                throw new ArgumentOutOfRangeException("item",
                    String.Format("field list already contains a field with name '{0}'", item.Name));

            var old = base[index];

            base.SetItem(index, item);

            old.Index = -1;
            old.Fields = null;
            byName.Remove(old.Name);

            item.Index = index;
            item.Fields = this;
            byName[item.Name] = item;
        }

        public Field FindField(string fieldName)
        {
            Field field;
            if (byName.TryGetValue(fieldName, out field))
                return field;
            else
                return null;
        }

        private bool initializedInstance;

        internal RowFieldsBase InitInstance(Row row)
        {
            this.Initialize();

            if (initializedInstance)
                return this;

            var readPerm = rowType.GetCustomAttribute<FieldReadPermissionAttribute>();
            if (readPerm != null && readPerm.Permission != null && !readPerm.ApplyToLookups)
            {
                var idField = (row as IIdRow)?.IdField as Field;
                var nameField = row.GetNameField();
                var permission = readPerm.Permission;
                foreach (var field in this)
                {
                    if (field.ReadPermission == null &&
                        !ReferenceEquals(idField, field) &&
                        !ReferenceEquals(nameField, field) &&
                        field.GetAttribute<LookupIncludeAttribute>() == null)
                    {
                        field.ReadPermission = permission;
                        if (field.InsertPermission == null)
                            field.InsertPermission = permission;
                        if (field.UpdatePermission == null)
                            field.UpdatePermission = permission;
                    }
                }
            }
            
            initializedInstance = true;
            return this;
        }

        public Field FindFieldByPropertyName(string propertyName)
        {
            Field field;
            if (byPropertyName.TryGetValue(propertyName, out field))
                return field;
            else
                return null;
        }

        public IDictionary<string, Join> Joins
        {
            get { return joins; }
        }

        string IAlias.Name
        {
            get { return alias; }
        }
        
        string IAlias.NameDot
        {
            get { return aliasDot; }
        }

        string IAlias.Table
        {
            get { return tableName; }
        }

        public string AliasName
        {
            get { return alias; }
        }
    }
}