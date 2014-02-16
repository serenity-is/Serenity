using Serenity.ComponentModel;
using Serenity.Data.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;

namespace Serenity.Data
{
    public abstract partial class Row
    {
        private void InitFields()
        {
            InitFields_PropertyNames();
            InitFields_ByPropertyName();
            InitFields_PropertyDescriptors();
            InitFields_Schema();
            InitFields_InferTextualFields();
        }

        private void InitFields_GetMyFieldsAndProperties(
            out Dictionary<string, FieldInfo> myFields,
            out Dictionary<string, PropertyInfo> myProperties)
        {
            myFields = new Dictionary<string, FieldInfo>(StringComparer.OrdinalIgnoreCase);
            myProperties = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

            var members = this.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var member in members)
            {
                var fi = member as FieldInfo;
                if (fi != null)
                    myFields[fi.Name] = fi;
                else
                {
                    var pi = member as PropertyInfo;
                    if (pi != null)
                        myProperties[pi.Name] = pi;
                }
            }
        }

        private static Delegate CreateGetMethod(FieldInfo fieldInfo)
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

        private static Delegate CreateSetMethod(FieldInfo fieldInfo)
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

        private void InitFields_PropertyNames()
        {
            Dictionary<string, FieldInfo> myFields;
            Dictionary<string, PropertyInfo> myProperties;
            InitFields_GetMyFieldsAndProperties(out myFields, out myProperties);
                    
            foreach (var fieldInfo in _fields.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (fieldInfo.FieldType.IsSubclassOf(typeof(Field)))
                {
                    var field = (Field)fieldInfo.GetValue(_fields);

                    PropertyInfo property;
                    if (!myProperties.TryGetValue(fieldInfo.Name, out property))
                        property = null;

                    ColumnAttribute column = null;
                    DisplayNameAttribute display = null;
                    SizeAttribute size = null;
                    ExpressionAttribute expression = null;
                    ScaleAttribute scale = null;
                    MinSelectLevelAttribute selectLevel = null;
                    ForeignKeyAttribute foreign = null;
                    AddJoinAttribute join = null;

                    FieldFlags addFlags = (FieldFlags)0;
                    FieldFlags removeFlags = (FieldFlags)0;

                    if (property != null)
                    {
                        column = property.GetCustomAttribute<ColumnAttribute>(false);
                        display = property.GetCustomAttribute<DisplayNameAttribute>(false);
                        size = property.GetCustomAttribute<SizeAttribute>(false);
                        expression = property.GetCustomAttribute<ExpressionAttribute>(false);
                        scale = property.GetCustomAttribute<ScaleAttribute>(false);
                        selectLevel = property.GetCustomAttribute<MinSelectLevelAttribute>(false);
                        foreign = property.GetCustomAttribute<ForeignKeyAttribute>(false);
                        join = property.GetCustomAttribute<AddJoinAttribute>(false);

                        var insertable = property.GetCustomAttribute<InsertableAttribute>(false);
                        var updatable = property.GetCustomAttribute<UpdatableAttribute>(false);

                        if (insertable != null && !insertable.Value)
                            removeFlags |= FieldFlags.Insertable;

                        if (updatable != null && !updatable.Value)
                            removeFlags |= FieldFlags.Updatable;

                        foreach (var attr in property.GetCustomAttributes<SetFieldFlagsAttribute>(false))
                        {
                            addFlags |= attr.Add;
                            removeFlags |= attr.Remove;
                        }
                    }

                    if (field == null)
                    {
                        if (property == null)
                        {
                            throw new InvalidProgramException(String.Format(
                                "Field {0} in type {1} is null and has no corresponding property in entity!",
                                    fieldInfo.Name, this.GetType().Name));
                        }

                        object[] prm = new object[7];
                        prm[0] = this._fields; // owner
                        prm[1] = column == null ? property.Name : (column.Name.TrimToNull() ?? property.Name);
                        prm[2] = display != null ? new LocalText(display.DisplayName) : null;
                        prm[3] = size != null ? size.Value : 0;
                        prm[4] = (FieldFlags.Default ^ removeFlags) | addFlags;
                        prm[5] = null;
                        prm[6] = null;

                        FieldInfo storage;
                        if (myFields.TryGetValue("_" + property.Name, out storage) ||
                            myFields.TryGetValue("m_" + property.Name, out storage) ||
                            myFields.TryGetValue(property.Name.Substring(1), out storage))
                        {
                            prm[5] = CreateGetMethod(storage);
                            prm[6] = CreateSetMethod(storage);
                        }

                        field = (Field)Activator.CreateInstance(fieldInfo.FieldType, prm);
                        fieldInfo.SetValue(_fields, field);

                    }
                    else 
                    {
                        if (size != null)
                            field.Size = size.Value;

                        if (display != null)
                            field.Caption = new LocalText(display.DisplayName);

                        if ((int)addFlags != 0 || (int)removeFlags != 0)
                            field.Flags = (field.Flags ^ removeFlags) | addFlags;

                        if (column != null && String.Compare(column.Name, field.Name, StringComparison.OrdinalIgnoreCase) != 0)
                            throw new InvalidProgramException(String.Format(
                                "Field name '{0}' in type {1} can't be overridden by Column name attribute!",
                                    fieldInfo.Name, this.GetType().Name));
                    }

                    if (scale != null)
                        field.Scale = scale.Value;

                    if (selectLevel != null)
                        field.MinSelectLevel = selectLevel.Value;

                    if (expression != null)
                        field.Expression = expression.Value;

                    if (foreign != null)
                    {
                        field.ForeignTable = foreign.Table;
                        field.ForeignField = foreign.Field;
                    }

                    if (join != null)
                    {
                        new LeftJoin(_fields.Joins, field.ForeignTable, join.Alias,
                            new Criteria(join.Alias, field.ForeignField) == new Criteria(field));
                    }

                    if (property != null)
                        field.PropertyName = property.Name;
                }
            }
        }

        private void InitFields_Schema()
        {
            var schemaAttr = this.GetType().GetCustomAttribute<SchemaAttribute>();
            if (schemaAttr != null)
                _fields._schema = schemaAttr.Schema;
            else
                _fields._schema = "Default";
        }

        private void InitFields_ByPropertyName()
        {
            var byPropertyName = new Dictionary<string, Field>();

            foreach (var field in _fields)
            {
                field._propertyName = field._propertyName ?? field._name;
                byPropertyName[field._propertyName ?? field._name] = field;
            }

            _fields._byPropertyName = byPropertyName;
        }

        private void InitFields_PropertyDescriptors()
        {
            PropertyDescriptor[] properties = new PropertyDescriptor[_fields.Count];
            for (int i = 0; i < _fields.Count; i++)
            {
                Field field = _fields[i];
                field._rowType = this.GetType();
                properties[i] = new FieldDescriptor(field);
            }

            _fields._propertyDescriptors = new PropertyDescriptorCollection(properties);
        }

        private void InitFields_InferTextualFields()
        {
            foreach (var field in _fields)
            {
                if (!field.ForeignTable.IsEmptyOrNull() &&
                    field.TextualField == null)
                {
                    foreach (var join in _fields._joins.Values)
                    {
                        if (String.Compare(field.ForeignTable, join.Table) == 0 &&
                            join is LeftJoin &&
                            !Object.ReferenceEquals(null, join.OnCriteria) &&
                            join.OnCriteria.ToStringIgnoreParams().IndexOf(field.Expression, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            foreach (var f in _fields)
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
    }
}