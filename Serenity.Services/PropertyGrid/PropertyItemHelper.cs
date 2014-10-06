using Newtonsoft.Json;
using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Serenity.PropertyGrid
{
    public class PropertyItemHelper
    {
        public static List<PropertyItem> GetPropertyItemsFor(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            var list = new List<PropertyItem>();

            var basedOnRowAttr = type.GetCustomAttributes(typeof(BasedOnRowAttribute), false);
            if (basedOnRowAttr.Length > 1)
                throw new InvalidOperationException(String.Format("{0} için birden fazla örnek row belirlenmiş!", type.Name));

            Row basedOnRow = null;

            if (basedOnRowAttr.Length == 1)
            {
                var basedOnRowType = ((BasedOnRowAttribute)basedOnRowAttr[0]).RowType;
                if (!basedOnRowType.IsSubclassOf(typeof(Row)))
                    throw new InvalidOperationException(String.Format("BasedOnRow özelliği için bir Row belirtilmeli!", type.Name));

                basedOnRow = (Row)Activator.CreateInstance(basedOnRowType);
            }

            ILocalizationRowHandler localizationRowHandler = null;
            LocalizationRowAttribute localizationAttr = null;
            if (basedOnRow != null)
            {
                localizationAttr = basedOnRow.GetType().GetCustomAttribute<LocalizationRowAttribute>(false);
                if (localizationAttr != null)
                    localizationRowHandler = Activator.CreateInstance(typeof(LocalizationRowHandler<>).MakeGenericType(basedOnRow.GetType())) as ILocalizationRowHandler;
            }

            foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Instance))
            {
                PropertyItem pi = new PropertyItem();

                if (member.MemberType != MemberTypes.Property &&
                    member.MemberType != MemberTypes.Field)
                    continue;

                var hiddenAttribute = member.GetCustomAttributes(typeof(HiddenAttribute), false);
                if (hiddenAttribute.Length > 0)
                    continue;

                Type memberType = member.MemberType == MemberTypes.Property ? ((PropertyInfo)member).PropertyType : ((FieldInfo)member).FieldType;

                pi.Name = member.Name;

                Field basedOnField = null;
                if (basedOnRow != null)
                {
                    basedOnField = basedOnRow.FindField(member.Name);

                    if (basedOnField == null)
                        basedOnField = basedOnRow.FindFieldByPropertyName(member.Name);
                }

                Func<Type, object> getAttribute = delegate(Type attrType)
                {
                    var attr = member.GetCustomAttribute(attrType);

                    if (attr == null && 
                        basedOnField != null && 
                        basedOnField.CustomAttributes != null)
                    {
                        foreach (var a in basedOnField.CustomAttributes)
                            if (attrType.IsAssignableFrom(a.GetType()))
                                return a;
                    }

                    return attr;
                };

                Func<Type, IEnumerable<object>> getAttributes = delegate(Type attrType)
                {
                    var attrList = new List<object>(member.GetCustomAttributes(attrType));

                    if (basedOnField != null &&
                        basedOnField.CustomAttributes != null)
                    {
                        foreach (var a in basedOnField.CustomAttributes)
                            if (attrType.IsAssignableFrom(a.GetType()))
                                attrList.Add(a);
                    }

                    return attrList;
                };

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
                
                var cssClassAttr = (CssClassAttribute)getAttribute(typeof(CssClassAttribute));
                if (cssClassAttr != null)
                    pi.CssClass = cssClassAttr.CssClass;

                if (getAttribute(typeof(OneWayAttribute)) != null)
                    pi.OneWay = true;

                if (getAttribute(typeof(ReadOnlyAttribute)) != null)
                    pi.ReadOnly = true;

                var resizableAttr = (ResizableAttribute)getAttribute(typeof(ResizableAttribute));
                pi.Resizable = resizableAttr == null || resizableAttr.Value;

                var widthAttr = (WidthAttribute)getAttribute(typeof(WidthAttribute));
                pi.Width = widthAttr == null ? (basedOnField != null ? AutoWidth(basedOnField) : 80)  : widthAttr.Value;
                pi.MinWidth = widthAttr == null ? 0 : widthAttr.Min;
                pi.MaxWidth = widthAttr == null ? 0 : widthAttr.Max;

                var editLinkAttr = (EditLinkAttribute)getAttribute(typeof(EditLinkAttribute));
                pi.EditLink = editLinkAttr != null && editLinkAttr.Value;
                pi.EditLinkItemType = editLinkAttr != null ? editLinkAttr.ItemType : (string)null;
                pi.EditLinkIdField = editLinkAttr != null ? editLinkAttr.IdField : null;
                pi.EditLinkCssClass = editLinkAttr != null ? editLinkAttr.CssClass : null;

                if (pi.Title == null)
                {
                    if (basedOnField != null)
                    {
                        Field textualField = null;
                        if (basedOnField.TextualField != null)
                            textualField = basedOnField.Fields.FindFieldByPropertyName(basedOnField.TextualField) ?? basedOnField.Fields.FindField(basedOnField.TextualField);

                        if (textualField != null)
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
                else if (basedOnField != null && basedOnField.DefaultValue != null)
                    pi.DefaultValue = basedOnField.DefaultValue;

                var insertableAttribute = member.GetCustomAttribute<InsertableAttribute>();
                if (insertableAttribute != null)
                    pi.Insertable = insertableAttribute.Value;
                else if (basedOnField != null)
                    pi.Insertable = (basedOnField.Flags & FieldFlags.Insertable) == FieldFlags.Insertable;
                else
                    pi.Insertable = true;

                var updatableAttribute = member.GetCustomAttribute<UpdatableAttribute>();
                if (updatableAttribute != null)
                    pi.Updatable = updatableAttribute.Value;
                else if (basedOnField != null)
                    pi.Updatable = (basedOnField.Flags & FieldFlags.Updatable) == FieldFlags.Updatable;
                else
                    pi.Updatable = true;

                pi.Localizable = getAttribute(typeof(LocalizableAttribute)) != null ||
                    (basedOnField != null && localizationRowHandler != null && localizationRowHandler.IsLocalized(basedOnField));

                Type nullableType = Nullable.GetUnderlyingType(memberType);
                Type enumType = null;
                if (memberType.IsEnum)
                    enumType = memberType;
                else if (nullableType != null && nullableType.IsEnum)
                    enumType = nullableType;
                else if (basedOnField != null && basedOnField is IEnumTypeField)
                {
                    enumType = (basedOnField as IEnumTypeField).EnumType;
                    if (enumType != null && !enumType.IsEnum)
                        enumType = null;
                }

                var editorTypeAttr = (EditorTypeAttribute)getAttribute(typeof(EditorTypeAttribute));

                if (editorTypeAttr == null)
                {
                    if (enumType != null)
                        pi.EditorType = "Select";
                    else if (memberType == typeof(DateTime) || memberType == typeof(DateTime?))
                        pi.EditorType = "Date";
                    else if (memberType == typeof(Boolean))
                        pi.EditorType = "Boolean";
                    else if (memberType == typeof(Decimal) || memberType == typeof(Decimal?) ||
                        memberType == typeof(Double) || memberType == typeof(Double?))
                        pi.EditorType = "Decimal";
                    else if (memberType == typeof(Int32) || memberType == typeof(Int32?))
                        pi.EditorType = "Integer";
                    else
                        pi.EditorType = "String";
                }
                else
                {
                    pi.EditorType = editorTypeAttr.EditorType;
                    editorTypeAttr.SetParams(pi.EditorParams);
                }

                if (enumType != null)
                {
                    List<string[]> options = new List<string[]>();
                    foreach (var val in Enum.GetValues(enumType))
                    {
                        string key = Convert.ToInt64(val).ToString();
                        string text = EnumMapper.FormatEnum(enumType, val);
                        options.Add(new string[] { key, text });
                    }
                    if (memberType == typeof(DayOfWeek)) // şimdilik tek bir özel durum
                    {
                        options.Add(options[0]);
                        options.RemoveAt(0);
                    }
                    pi.EditorParams["items"] = options.ToArray();
                }

                var formatterTypeAttr = (FormatterTypeAttribute)getAttribute(typeof(FormatterTypeAttribute));
                if (formatterTypeAttr == null)
                {
                    if (enumType != null)
                    {
                        pi.FormatterType = "Enum";
                        var enumKeyAttr = enumType.GetCustomAttribute<EnumKeyAttribute>();
                        var enumKey = enumKeyAttr != null ? enumKeyAttr.Value : enumType.FullName;
                        pi.FormatterParams["enumKey"] = enumKey;
                    }
                    else if (memberType == typeof(DateTime) || memberType == typeof(DateTime?))
                    {
                        if (basedOnField != null && basedOnField is DateTimeField)
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
                    else if (memberType == typeof(Boolean))
                        pi.FormatterType = "Checkbox";
                    else if (memberType == typeof(Decimal) || memberType == typeof(Decimal?) ||
                        memberType == typeof(Double) || memberType == typeof(Double?) ||
                        memberType == typeof(Int32) || memberType == typeof(Int32?))
                    {
                        pi.FormatterType = "Number";
                    }
                }
                else
                {
                    pi.FormatterType = formatterTypeAttr.FormatterType;
                    formatterTypeAttr.SetParams(pi.FormatterParams);
                }

                if (basedOnField != null)
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
                if (reqAttr != null)
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

                var displayFormatAttr = (DisplayFormatAttribute)getAttribute(typeof(DisplayFormatAttribute));
                if (displayFormatAttr != null)
                {
                    pi.DisplayFormat = displayFormatAttr.Value;
                    pi.FormatterParams["displayFormat"] = displayFormatAttr.Value;
                }

                foreach (FormatterOptionAttribute param in getAttributes(typeof(FormatterOptionAttribute)))
                {
                    var key = param.Key;
                    if (key != null &&
                        key.Length >= 1)
                        key = key.Substring(0, 1).ToLowerInvariant() + key.Substring(1);

                    pi.FormatterParams[key] = param.Value;
                }

                list.Add(pi);
            }

            return list;
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
            pi.Name = basedOnField != null ? (basedOnField.PropertyName ?? basedOnField.Name) : definition.Name;
            pi.Category = definition.Category.TrimToNull();
            pi.ReadOnly = false;
            pi.Title = basedOnField != null ? basedOnField.Title : definition.Title;
            pi.DefaultValue = definition.DefaultValue;
            pi.Insertable = basedOnField == null || ((basedOnField.Flags & FieldFlags.Insertable) == FieldFlags.Insertable);
            pi.Updatable = basedOnField == null || ((basedOnField.Flags & FieldFlags.Updatable) == FieldFlags.Updatable);
            pi.Localizable = definition.IsLocalizable;

            Type enumType = null;
            if (basedOnField != null && basedOnField is IEnumTypeField)
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
                    pi.EditorType = "Select";
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
                List<string[]> options = new List<string[]>();
                foreach (var val in Enum.GetValues(enumType))
                {
                    string key = Convert.ToInt64(val).ToString();
                    string text = EnumMapper.FormatEnum(enumType, val);
                    options.Add(new string[] { key, text });
                }

                pi.EditorParams["items"] = options.ToArray();
            }

            if (basedOnField != null)
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

    [Obsolete("Use PropertyItemHelper")]
    public class PropertyEditorHelper : PropertyItemHelper
    {
    }
}