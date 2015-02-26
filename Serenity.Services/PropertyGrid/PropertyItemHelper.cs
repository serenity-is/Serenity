using Newtonsoft.Json;
using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace Serenity.PropertyGrid
{
    public class PropertyItemHelper
    {
        public static List<PropertyItem> GetPropertyItemsFor(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            var list = new List<PropertyItem>();

            var basedOnRow = GetBasedOnRow(type);
            var localizationRowHandler = GetLocalizationRowHandler(basedOnRow);

            foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Instance))
            {
                PropertyItem pi = new PropertyItem();

                if (member.MemberType != MemberTypes.Property &&
                    member.MemberType != MemberTypes.Field)
                    continue;

                var hiddenAttribute = member.GetCustomAttributes(typeof(InternalAttribute), false);
                if (hiddenAttribute.Length > 0)
                    continue;

                var memberType = member.MemberType == MemberTypes.Property ? 
                    ((PropertyInfo)member).PropertyType : 
                    ((FieldInfo)member).FieldType;

                var nullableType = Nullable.GetUnderlyingType(memberType);
                var valueType = nullableType ?? memberType;
                
                pi.Name = member.Name;

                Field basedOnField = null;
                if (basedOnRow != null)
                {
                    basedOnField = basedOnRow.FindField(member.Name);

                    if (ReferenceEquals(null, basedOnField))
                        basedOnField = basedOnRow.FindFieldByPropertyName(member.Name);
                }

                Func<Type, Attribute> getAttribute = attrType => GetAttribute(member, basedOnField, attrType);
                Func<Type, IEnumerable<Attribute>> getAttributes = attrType => GetAttributes(member, basedOnField, attrType);

                var displayNameAttribute = (DisplayNameAttribute)member.GetCustomAttribute(typeof(DisplayNameAttribute), false);
                if (displayNameAttribute != null)
                    pi.Title = displayNameAttribute.DisplayName;

                var hintAttribute = (HintAttribute)getAttribute(typeof(HintAttribute));
                if (hintAttribute != null)
                    pi.Hint = hintAttribute.Hint;

                var placeholderAttribute = (PlaceholderAttribute)getAttribute(typeof(PlaceholderAttribute));
                if (placeholderAttribute != null)
                    pi.Placeholder = placeholderAttribute.Value;

                var categoryAttribute = (CategoryAttribute)getAttribute(typeof(CategoryAttribute));
                if (categoryAttribute != null)
                    pi.Category = categoryAttribute.Category;
                else if (list.Count > 0)
                    pi.Category = list[list.Count - 1].Category;
                
                var cssClassAttr = (CssClassAttribute)getAttribute(typeof(CssClassAttribute));
                if (cssClassAttr != null)
                    pi.CssClass = cssClassAttr.CssClass;

                var alignmentAttr = (AlignmentAttribute)getAttribute(typeof(AlignmentAttribute));
                if (alignmentAttr != null)
                    pi.Alignment = alignmentAttr.Value;

                var sortOrderAttr = (SortOrderAttribute)getAttribute(typeof(SortOrderAttribute));
                if (sortOrderAttr != null && sortOrderAttr.SortOrder != 0)
                    pi.SortOrder = sortOrderAttr.SortOrder;

                if (getAttribute(typeof(OneWayAttribute)) != null)
                    pi.OneWay = true;

                if (getAttribute(typeof(ReadOnlyAttribute)) != null)
                    pi.ReadOnly = true;

                var resizableAttr = (ResizableAttribute)getAttribute(typeof(ResizableAttribute));
                pi.Resizable = resizableAttr == null || resizableAttr.Value;

                var widthAttr = (WidthAttribute)getAttribute(typeof(WidthAttribute));
                pi.Width = widthAttr == null ? (!ReferenceEquals(null, basedOnField) ? AutoWidth(basedOnField) : 80)  : widthAttr.Value;
                pi.MinWidth = widthAttr == null ? 0 : widthAttr.Min;
                pi.MaxWidth = widthAttr == null ? 0 : widthAttr.Max;

                var editLinkAttr = (EditLinkAttribute)getAttribute(typeof(EditLinkAttribute));
                pi.EditLink = editLinkAttr != null && editLinkAttr.Value;
                pi.EditLinkItemType = editLinkAttr != null ? editLinkAttr.ItemType : (string)null;
                pi.EditLinkIdField = editLinkAttr != null ? editLinkAttr.IdField : null;
                pi.EditLinkCssClass = editLinkAttr != null ? editLinkAttr.CssClass : null;

                if (pi.EditLinkItemType != null && pi.EditLinkIdField == null)
                    pi.EditLinkIdField = AutoDetermineIdField(basedOnField);

                if (pi.Title == null)
                {
                    if (!ReferenceEquals(null, basedOnField))
                    {
                        Field textualField = null;
                        if (basedOnField.TextualField != null)
                            textualField = basedOnField.Fields.FindFieldByPropertyName(basedOnField.TextualField) ?? 
                                basedOnField.Fields.FindField(basedOnField.TextualField);

                        if (!ReferenceEquals(null, textualField))
                        {
                            pi.Title = !object.ReferenceEquals(null, textualField.Caption) ? 
                                textualField.Caption.Key : textualField.Title;
                        }
                        else
                        {
                            pi.Title = !object.ReferenceEquals(null, basedOnField.Caption) ? 
                                basedOnField.Caption.Key : basedOnField.Title;
                        }
                    }
                    else
                        pi.Title = pi.Name;
                }

                var defaultValueAttribute = (DefaultValueAttribute)member.GetCustomAttribute(typeof(DefaultValueAttribute), false);
                if (defaultValueAttribute != null)
                    pi.DefaultValue = defaultValueAttribute.Value;
                else if (!ReferenceEquals(null, basedOnField) && basedOnField.DefaultValue != null)
                    pi.DefaultValue = basedOnField.DefaultValue;

                var insertableAttribute = member.GetCustomAttribute<InsertableAttribute>();
                if (insertableAttribute != null)
                    pi.Insertable = insertableAttribute.Value;
                else if (!ReferenceEquals(null, basedOnField))
                    pi.Insertable = (basedOnField.Flags & FieldFlags.Insertable) == FieldFlags.Insertable;
                else
                    pi.Insertable = true;

                var updatableAttribute = member.GetCustomAttribute<UpdatableAttribute>();
                if (updatableAttribute != null)
                    pi.Updatable = updatableAttribute.Value;
                else if (!ReferenceEquals(null, basedOnField))
                    pi.Updatable = (basedOnField.Flags & FieldFlags.Updatable) == FieldFlags.Updatable;
                else
                    pi.Updatable = true;

                pi.Localizable = getAttribute(typeof(LocalizableAttribute)) != null ||
                    (!ReferenceEquals(null, basedOnField) && localizationRowHandler != null && localizationRowHandler.IsLocalized(basedOnField));

                var visibleAttribute = (VisibleAttribute)getAttribute(typeof(VisibleAttribute));
                if (visibleAttribute != null && visibleAttribute.Value == false)
                    pi.Visible = false;

                var enumType = GetEnumType(valueType, basedOnField);
                
                var editorTypeAttr = (EditorTypeAttribute)getAttribute(typeof(EditorTypeAttribute));

                if (editorTypeAttr == null)
                {
                    pi.EditorType = AutoDetermineEditorType(valueType, enumType);
                }
                else
                {
                    pi.EditorType = editorTypeAttr.EditorType;
                    editorTypeAttr.SetParams(pi.EditorParams);
                }

                if (enumType != null)
                {
                    pi.EditorParams["enumKey"] = EnumMapper.GetEnumTypeKey(enumType);
                }

                if (!ReferenceEquals(null, basedOnField))
                {
                    if (pi.EditorType == "Decimal" &&
                        (basedOnField is DoubleField ||
                         basedOnField is DecimalField) &&
                        basedOnField.Size > 0 &&
                        basedOnField.Scale < basedOnField.Size)
                    {
                        string minVal = new String('0', basedOnField.Size - basedOnField.Scale);
                        if (basedOnField.Scale > 0)
                            minVal += "." + new String('0', basedOnField.Scale);
                        string maxVal = minVal.Replace('0', '9');
                        pi.EditorParams["minValue"] = minVal;
                        pi.EditorParams["maxValue"] = maxVal;
                    }
                    else if (basedOnField.Size > 0)
                    {
                        pi.EditorParams["maxLength"] = basedOnField.Size;
                        pi.MaxLength = basedOnField.Size;
                    }

                    if ((basedOnField.Flags & FieldFlags.NotNull) == FieldFlags.NotNull)
                        pi.Required = true;
                }

                var reqAttr = member.GetAttribute<RequiredAttribute>(true);
                if (reqAttr != null && (pi.Required != null || reqAttr.IsRequired))
                    pi.Required = reqAttr.IsRequired;

                var maxLengthAttr = (MaxLengthAttribute)getAttribute(typeof(MaxLengthAttribute));
                if (maxLengthAttr != null)
                {
                    pi.MaxLength = maxLengthAttr.MaxLength;
                    pi.EditorParams["maxLength"] = maxLengthAttr.MaxLength;
                }

                foreach (EditorOptionAttribute param in getAttributes(typeof(EditorOptionAttribute)))
                {
                    var key = param.Key;
                    if (key != null &&
                        key.Length >= 1)
                        key = key.Substring(0, 1).ToLowerInvariant() + key.Substring(1);
                        
                    pi.EditorParams[key] = param.Value;
                }

                HandleFormatter(member, valueType, enumType, basedOnField, pi);
                HandleFiltering(member, valueType, enumType, basedOnField, pi);

                list.Add(pi);
            }

            return list;
        }

        private static Attribute GetAttribute(MemberInfo member, Field basedOnField, Type attrType)
        {
            var attr = member == null ? null : member.GetCustomAttribute(attrType);

            if (attr == null &&
                !ReferenceEquals(null, basedOnField) &&
                basedOnField.CustomAttributes != null)
            {
                foreach (var a in basedOnField.CustomAttributes)
                    if (attrType.IsAssignableFrom(a.GetType()))
                        return (Attribute)a;
            }

            return attr;
        }

        private static IEnumerable<Attribute> GetAttributes(MemberInfo member, Field basedOnField, Type attrType)
        {
            var attrList = new List<Attribute>();
            if (member != null)
                attrList.AddRange(member.GetCustomAttributes(attrType));

            if (!ReferenceEquals(null, basedOnField) &&
                basedOnField.CustomAttributes != null)
            {
                foreach (var a in basedOnField.CustomAttributes)
                    if (attrType.IsAssignableFrom(a.GetType()))
                        attrList.Add((Attribute)a);
            }

            return attrList;
        }

        private static Row GetBasedOnRow(Type type)
        {
            var basedOnRowAttr = type.GetCustomAttribute<BasedOnRowAttribute>();
            Row basedOnRow = null;

            if (basedOnRowAttr != null)
            {
                var basedOnRowType = basedOnRowAttr.RowType;
                if (!basedOnRowType.IsSubclassOf(typeof(Row)))
                    throw new InvalidOperationException(String.Format("BasedOnRow özelliği için bir Row belirtilmeli!", type.Name));

                basedOnRow = (Row)Activator.CreateInstance(basedOnRowType);
            }

            return basedOnRow;
        }

        private static ILocalizationRowHandler GetLocalizationRowHandler(Row basedOnRow)
        {
            LocalizationRowAttribute localizationAttr = null;
            if (basedOnRow != null)
            {
                localizationAttr = basedOnRow.GetType().GetCustomAttribute<LocalizationRowAttribute>(false);
                if (localizationAttr != null)
                    return Activator.CreateInstance(typeof(LocalizationRowHandler<>)
                        .MakeGenericType(basedOnRow.GetType())) as ILocalizationRowHandler;
            }
            return null;
        }

        private static Type GetEnumType(Type valueType, Field basedOnField)
        {
            Type enumType = null;
            if (valueType.IsEnum)
                enumType = valueType;
            else if (!ReferenceEquals(null, basedOnField) && basedOnField is IEnumTypeField)
            {
                enumType = (basedOnField as IEnumTypeField).EnumType;
                if (enumType != null && !enumType.IsEnum)
                    enumType = null;
            }

            return enumType;
        }

        private static string AutoDetermineIdField(Field basedOnField)
        {
            if (ReferenceEquals(null, basedOnField) || basedOnField.Join == null)
                return null;

            var idField = basedOnField.Fields.FirstOrDefault(x => x.ForeignJoinAlias != null &&
                x.ForeignJoinAlias.Name == basedOnField.Join.Name);

            return ReferenceEquals(null, idField) ? null : (idField.PropertyName ?? idField.Name);
        }

        private static string AutoDetermineEditorType(Type valueType, Type enumType)
        {
            if (enumType != null)
                return "Enum";
            else if (valueType == typeof(String))
                return "String";
            else if (valueType == typeof(Int32))
                return "Integer";
            else if (valueType == typeof(DateTime))
                return "Date";
            else if (valueType == typeof(Boolean))
                return "Boolean";
            else if (valueType == typeof(Decimal) || valueType == typeof(Double))
                return "Decimal";
            else
                return "String";
        }

        private static CustomEditorAttribute FindLookupAttribute(Field basedOnField, string idField)
        {
            if (idField == null)
                return null;

            var field = basedOnField.Fields.FindFieldByPropertyName(idField) ?? basedOnField.Fields.FindField(idField);
            if (ReferenceEquals(null, field))
                return null;

            if (field.TextualField != basedOnField.PropertyName &&
                field.TextualField != basedOnField.Name)
                return null;

            return (CustomEditorAttribute)(GetAttribute(null, field, typeof(LookupEditorAttribute)) ??
                GetAttribute(null, field, typeof(AsyncLookupEditorAttribute)));
        }

        private static HashSet<string> standardFilteringEditors = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Date",
            "Serenity.Date",
            "DateTime",
            "Serenity.DateTime",
            "Boolean",
            "Serenity.Boolean",
            "Decimal",
            "Integer",
            "Serenity.Integer"
        };

        private static void HandleFiltering(MemberInfo member, Type valueType, Type enumType, Field basedOnField, PropertyItem pi)
        {
            var filterOnlyAttr = GetAttribute(member, basedOnField, typeof(FilterOnlyAttribute)) as FilterOnlyAttribute;
            var notFilterableAttr = GetAttribute(member, basedOnField, typeof(NotFilterableAttribute)) as NotFilterableAttribute;

            if (filterOnlyAttr != null && filterOnlyAttr.Value)
                pi.FilterOnly = true;

            if (notFilterableAttr != null && notFilterableAttr.Value)
                pi.NotFilterable = true;

            if (pi.NotFilterable == true)
                return;

            var filteringTypeAttr = (FilteringTypeAttribute)GetAttribute(member, basedOnField, typeof(FilteringTypeAttribute));
            if (filteringTypeAttr == null)
            {
                string idField = AutoDetermineIdField(basedOnField);
                CustomEditorAttribute lookupEditorAttr = null;
                var editorAttr = (EditorTypeAttribute)GetAttribute(member, basedOnField, typeof(EditorTypeAttribute));
                if (editorAttr == null)
                    lookupEditorAttr = FindLookupAttribute(basedOnField, idField);

                if (idField != null)
                {
                    pi.FilteringParams["idField"] = idField;
                    pi.FilteringIdField = idField;
                }

                if (editorAttr != null && !standardFilteringEditors.Contains(editorAttr.EditorType))
                {
                    pi.FilteringType = "Editor";
                    pi.FilteringParams["editorType"] = editorAttr.EditorType;
                    pi.FilteringParams["useLike"] = valueType == typeof(String);
                }
                else if (lookupEditorAttr != null)
                {
                    var async = lookupEditorAttr as AsyncLookupEditorAttribute;
                    pi.FilteringType = async != null ? "AsyncLookup" : "Lookup";
                    pi.FilteringParams["lookupKey"] = async != null ? async.LookupKey : ((LookupEditorAttribute)lookupEditorAttr).LookupKey;
                }
                else if (enumType != null)
                {
                    pi.FilteringType = "Enum";
                    pi.FilteringParams["enumKey"] = EnumMapper.GetEnumTypeKey(enumType);
                }
                else if (valueType == typeof(DateTime))
                {
                    if (!ReferenceEquals(null, basedOnField) && basedOnField is DateTimeField)
                    {
                        switch (((DateTimeField)basedOnField).DateTimeKind)
                        {
                            case DateTimeKind.Unspecified:
                                pi.FilteringType = "Date";
                                break;
                            default:
                                pi.FilteringType = "DateTime";
                                break;
                        }
                    }
                    else
                        pi.FilteringType = "Date";
                }
                else if (valueType == typeof(Boolean))
                    pi.FilteringType = "Boolean";
                else if (valueType == typeof(Decimal) ||
                    valueType == typeof(Double))
                {
                    pi.FilteringType = "Decimal";
                }
                else if (valueType == typeof(Int32) ||
                    valueType == typeof(Int16) ||
                    valueType == typeof(Int64))
                {
                    pi.FilteringType = "Integer";
                }
                else
                    pi.FilteringType = "String";
            }
            else
            {
                pi.FilteringType = filteringTypeAttr.FilteringType;
                filteringTypeAttr.SetParams(pi.FilteringParams);

                if (pi.FilteringType == "Editor")
                {
                    if (!pi.FilteringParams.ContainsKey("editorType"))
                    {
                        var editorAttr = (EditorTypeAttribute)GetAttribute(member, basedOnField, typeof(EditorTypeAttribute));
                        if (editorAttr != null)
                            pi.FilteringParams["editorType"] = editorAttr.EditorType;
                    }

                    if (!pi.FilteringParams.ContainsKey("useLike"))
                    {
                        if (valueType == typeof(String))
                            pi.FilteringParams["useLike"] = true;
                    }
                }

                object idField;
                if (pi.FilteringParams.TryGetValue("idField", out idField) && idField is string)
                    pi.FilteringIdField = (idField as string).TrimToNull();
                else
                    pi.FilteringIdField = AutoDetermineIdField(basedOnField);
            }

            var displayFormatAttr = (DisplayFormatAttribute)GetAttribute(member, basedOnField, typeof(DisplayFormatAttribute));
            if (displayFormatAttr != null)
            {
                pi.FilteringParams["displayFormat"] = displayFormatAttr.Value;
            }

            foreach (FilteringOptionAttribute param in GetAttributes(member, basedOnField, typeof(FilteringOptionAttribute)))
            {
                var key = param.Key;
                if (key != null &&
                    key.Length >= 1)
                    key = key.Substring(0, 1).ToLowerInvariant() + key.Substring(1);

                pi.FilteringParams[key] = param.Value;
            }
        }

        private static void HandleFormatter(MemberInfo member, Type valueType, Type enumType, Field basedOnField, PropertyItem pi)
        {
            var formatterTypeAttr = (FormatterTypeAttribute)GetAttribute(member, basedOnField, typeof(FormatterTypeAttribute));
            if (formatterTypeAttr == null)
            {
                if (enumType != null)
                {
                    pi.FormatterType = "Enum";
                    pi.FormatterParams["enumKey"] = EnumMapper.GetEnumTypeKey(enumType);
                }
                else if (valueType == typeof(DateTime) || valueType == typeof(DateTime?))
                {
                    if (!ReferenceEquals(null, basedOnField) && basedOnField is DateTimeField)
                    {
                        switch (((DateTimeField)basedOnField).DateTimeKind)
                        {
                            case DateTimeKind.Unspecified:
                                pi.FormatterType = "Date";
                                break;
                            default:
                                pi.FormatterType = "DateTime";
                                break;
                        }
                    }
                    else
                        pi.FormatterType = "Date";
                }
                else if (valueType == typeof(Boolean))
                    pi.FormatterType = "Checkbox";
                else if (valueType == typeof(Decimal) ||
                    valueType == typeof(Double) || 
                    valueType == typeof(Int32))
                {
                    pi.FormatterType = "Number";
                }
            }
            else
            {
                pi.FormatterType = formatterTypeAttr.FormatterType;
                formatterTypeAttr.SetParams(pi.FormatterParams);
            }

            var displayFormatAttr = (DisplayFormatAttribute)GetAttribute(member, basedOnField, typeof(DisplayFormatAttribute));
            if (displayFormatAttr != null)
            {
                pi.DisplayFormat = displayFormatAttr.Value;
                pi.FormatterParams["displayFormat"] = displayFormatAttr.Value;
            }

            foreach (FormatterOptionAttribute param in GetAttributes(member, basedOnField, typeof(FormatterOptionAttribute)))
            {
                var key = param.Key;
                if (key != null &&
                    key.Length >= 1)
                    key = key.Substring(0, 1).ToLowerInvariant() + key.Substring(1);

                pi.FormatterParams[key] = param.Value;
            }
        }

        public static int AutoWidth(Field field)
        {
            var name = field.Name;

            switch (field.Type)
            {
                case FieldType.String:
                    if (field.Size != 0 && field.Size <= 25)
                        return Math.Max(field.Size * 6, 150);
                    else if (field.Size == 0)
                        return 250;
                    else
                        return 150;
                case FieldType.Boolean:
                    return 40;
                case FieldType.DateTime:
                    return 85;
                case FieldType.Int16:
                    return 55;
                case FieldType.Int32:
                    return 65;
                case FieldType.Double:
                case FieldType.Decimal:
                    return 85;
                default:
                    return 80;
            }
        }

        public static List<PropertyItem> GetCustomFieldPropertyItems(IEnumerable<ICustomFieldDefinition> definitions, Row row, string fieldPrefix)
        {
            var list = new List<PropertyItem>();
            foreach (var def in definitions)
            {
                string name = fieldPrefix + def.Name;
                var field = row.FindFieldByPropertyName(name) ?? row.FindField(name);
                list.Add(GetCustomFieldPropertyItem(def, field));
            }
            return list;
        }

        public static PropertyItem GetCustomFieldPropertyItem(ICustomFieldDefinition definition, Field basedOnField)
        {
            PropertyItem pi = new PropertyItem();
            pi.Name = !ReferenceEquals(null, basedOnField) ? (basedOnField.PropertyName ?? basedOnField.Name) : definition.Name;
            pi.Category = definition.Category.TrimToNull();
            pi.ReadOnly = false;
            pi.Title = !ReferenceEquals(null, basedOnField) ? basedOnField.Title : definition.Title;
            pi.DefaultValue = definition.DefaultValue;
            pi.Insertable = ReferenceEquals(null, basedOnField) || ((basedOnField.Flags & FieldFlags.Insertable) == FieldFlags.Insertable);
            pi.Updatable = ReferenceEquals(null, basedOnField) || ((basedOnField.Flags & FieldFlags.Updatable) == FieldFlags.Updatable);
            pi.Localizable = definition.IsLocalizable;

            Type enumType = null;
            if (!ReferenceEquals(null, basedOnField) && basedOnField is IEnumTypeField)
            {
                enumType = (basedOnField as IEnumTypeField).EnumType;
                if (enumType != null && !enumType.IsEnum)
                    enumType = null;
            }

            if (!definition.EditorType.IsTrimmedEmpty())
            {
                pi.EditorType = definition.EditorType.TrimToNull();
            }
            else
            {
                if (enumType != null)
                    pi.EditorType = "Enum";
                else if (definition.FieldType == CustomFieldType.Date ||
                    definition.FieldType == CustomFieldType.DateTime)
                    pi.EditorType = "Date";
                else if (definition.FieldType == CustomFieldType.Boolean)
                    pi.EditorType = "Boolean";
                else if (definition.FieldType == CustomFieldType.Decimal)
                    pi.EditorType = "Decimal";
                else if (definition.FieldType == CustomFieldType.Int32 || definition.FieldType == CustomFieldType.Int64)
                    pi.EditorType = "Integer";
                else
                    pi.EditorType = "String";
            }

            if (enumType != null)
            {
                pi.EditorParams["enumKey"] = EnumMapper.GetEnumTypeKey(enumType);
            }

            if (!ReferenceEquals(null, basedOnField))
            {
                if (basedOnField is StringField &&
                    basedOnField.Size > 0)
                {
                    pi.EditorParams["maxLength"] = basedOnField.Size;
                    pi.MaxLength = basedOnField.Size;
                }

                if ((basedOnField.Flags & FieldFlags.NotNull) == FieldFlags.NotNull)
                    pi.Required = true;
            }

            if (definition.IsRequired)
                pi.Required = true;

            if (definition.Size != 0 &&
                definition.FieldType == CustomFieldType.String)
            {
                pi.MaxLength = definition.Size;
                pi.EditorParams["maxLength"] = definition.Size;
            }

            var editorOptionsJson = definition.EditorOptions.TrimToNull();
            if (editorOptionsJson != null &&
                editorOptionsJson.StartsWith("{"))
            {
                var editorOptions = JsonConvert.DeserializeObject<Dictionary<string, object>>(editorOptionsJson, JsonSettings.Tolerant);
                foreach (var option in editorOptions)
                    pi.EditorParams[option.Key] = option.Value;
            }

            return pi;
        }
    }
}