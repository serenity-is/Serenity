using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetEditing(IPropertySource source, PropertyItem item)
        {
            var editorTypeAttr = source.GetAttribute<EditorTypeAttribute>();

            if (editorTypeAttr == null)
            {
                item.EditorType = AutoDetermineEditorType(source.ValueType, source.EnumType, item.EditorParams);
            }
            else
            {
                item.EditorType = editorTypeAttr.EditorType;
                editorTypeAttr.SetParams(item.EditorParams);
                if (item.EditorType == "Lookup" &&
                    !item.EditorParams.ContainsKey("lookupKey"))
                {
                    var distinct = source.GetAttribute<DistinctValuesEditorAttribute>();
                    if (distinct != null)
                    {
                        string prefix = null;
                        if (distinct.RowType != null)
                        {
                            if (!distinct.RowType.IsInterface &&
                                !distinct.RowType.IsAbstract &&
                                typeof(IRow).IsAssignableFrom(distinct.RowType))
                            {
                                prefix = ((IRow)Activator.CreateInstance(distinct.RowType))
                                    .Fields.LocalTextPrefix;
                            }
                        }
                        else
                        {
                            var isRow = source.Property != null &&
                                source.Property.ReflectedType != null &&
                                !source.Property.ReflectedType.IsAbstract &&
                                typeof(IRow).IsAssignableFrom(source.Property.ReflectedType);

                            if (!isRow)
                            {
                                if (!ReferenceEquals(null, source.BasedOnField))
                                    prefix = source.BasedOnField.Fields.LocalTextPrefix;
                            }
                            else
                                prefix = ((IRow)Activator.CreateInstance(source.Property.ReflectedType))
                                    .Fields.LocalTextPrefix;
                        }

                        if (prefix != null)
                        {
                            var propertyName = distinct.PropertyName.IsEmptyOrNull() ?
                                    (!ReferenceEquals(null, source.BasedOnField) ?
                                        (source.BasedOnField.PropertyName ?? source.BasedOnField.Name) :
                                    item.Name) : distinct.PropertyName;

                            if (!string.IsNullOrEmpty(propertyName))
                                item.EditorParams["lookupKey"] = "Distinct." + prefix + "." + propertyName;
                        }
                    }
                }

            }

            if (source.EnumType != null)
                item.EditorParams["enumKey"] = EnumMapper.GetEnumTypeKey(source.EnumType);

            var dtka = source.GetAttribute<DateTimeKindAttribute>();
            if (dtka != null && dtka.Value != DateTimeKind.Unspecified)
                item.EditorParams["useUtc"] = true;

            if (!ReferenceEquals(null, source.BasedOnField))
            {
                if (dtka == null &&
                    source.BasedOnField is DateTimeField &&
                    ((DateTimeField)source.BasedOnField).DateTimeKind != DateTimeKind.Unspecified)
                    item.EditorParams["useUtc"] = true;

                if (item.EditorType == "Decimal" &&
                    (source.BasedOnField is DoubleField ||
                     source.BasedOnField is SingleField ||
                     source.BasedOnField is DecimalField) &&
                    source.BasedOnField.Size > 0 &&
                    source.BasedOnField.Scale < source.BasedOnField.Size &&
                    !item.EditorParams.ContainsKey("minValue") &&
                    !item.EditorParams.ContainsKey("maxValue"))
                {
                    string minVal = new String('0', source.BasedOnField.Size - source.BasedOnField.Scale);
                    if (source.BasedOnField.Scale > 0)
                        minVal += "." + new String('0', source.BasedOnField.Scale);
                    string maxVal = minVal.Replace('0', '9');

                    if ((item.EditorParams.ContainsKey("allowNegatives") &&
                         Convert.ToBoolean(item.EditorParams["allowNegatives"] ?? false) == true) ||
                        (!item.EditorParams.ContainsKey("allowNegatives") &&
                        DecimalEditorAttribute.AllowNegativesByDefault))
                        minVal = "-" + maxVal;

                    item.EditorParams["minValue"] = minVal;
                    item.EditorParams["maxValue"] = maxVal;
                }
                else if (item.EditorType == "Integer" &&
                    (source.BasedOnField is Int32Field ||
                     source.BasedOnField is Int16Field ||
                     source.BasedOnField is Int64Field) &&
                    !item.EditorParams.ContainsKey("minValue") &&
                    !item.EditorParams.ContainsKey("maxValue"))
                {
                    item.EditorParams["maxValue"] = source.BasedOnField is Int16Field ? Int16.MaxValue
                        : source.BasedOnField is Int32Field ? Int32.MaxValue :
                        source.BasedOnField is Int64Field ? Int64.MaxValue : (object)null;

                    if ((item.EditorParams.ContainsKey("allowNegatives") &&
                         Convert.ToBoolean(item.EditorParams["allowNegatives"] ?? false) == true) ||
                        (!item.EditorParams.ContainsKey("allowNegatives") &&
                        DecimalEditorAttribute.AllowNegativesByDefault) &&
                        item.EditorParams["maxValue"] != null)
                        item.EditorParams["minValue"] = -Convert.ToInt64(item.EditorParams["maxValue"]) - 1;
                }
                else if (source.BasedOnField.Size > 0)
                {
                    item.EditorParams["maxLength"] = source.BasedOnField.Size;
                    item.MaxLength = source.BasedOnField.Size;
                }
            }

            var maxLengthAttr = source.GetAttribute<MaxLengthAttribute>();
            if (maxLengthAttr != null)
            {
                item.MaxLength = maxLengthAttr.MaxLength;
                item.EditorParams["maxLength"] = maxLengthAttr.MaxLength;
            }

            foreach (EditorOptionAttribute param in source.GetAttributes<EditorOptionAttribute>())
            {
                var key = param.Key;
                if (key != null &&
                    key.Length >= 1)
                    key = key.Substring(0, 1).ToLowerInvariant() + key.Substring(1);

                item.EditorParams[key] = param.Value;
            }

            SetServiceLookupParams(editorTypeAttr, item.EditorParams);
        }

        private static void SetServiceLookupParams(EditorTypeAttribute editorTypeAttr, Dictionary<string, object> editorParams) 
        {
            if (!(editorTypeAttr is ServiceLookupEditorAttribute sle) || sle.ItemType == null)
                return;

            if (!editorParams.ContainsKey("service"))
                editorParams["service"] = ServiceLookupEditorAttribute.AutoServiceFor(sle.ItemType);

            if (typeof(IRow).IsAssignableFrom(sle.ItemType) &&
                !sle.ItemType.IsAbstract &&
                (!editorParams.ContainsKey("idField") ||
                    !editorParams.ContainsKey("textField") ||
                    (!editorParams.ContainsKey("includeColumns") &&
                    !editorParams.ContainsKey("columnSelection"))))
            {
                try
                {
                    var rowInstance = Activator.CreateInstance(sle.ItemType) as IRow;
                    if (rowInstance is IIdRow idRow &&
                        !editorParams.ContainsKey("idField"))
                    {
                        var idField = ((Field)idRow.IdField);
                        editorParams["idField"] = idField.PropertyName ?? idField.Name;
                    }

                    if (!editorParams.ContainsKey("textField"))
                    {
                        var nameField = RowExtensions.GetNameField(rowInstance, false);
                        if (!ReferenceEquals(nameField, null))
                            editorParams["textField"] = nameField.PropertyName ??
                                nameField.Name;
                    }

                    if (!editorParams.ContainsKey("includeColumns") &&
                        !editorParams.ContainsKey("columnSelection"))
                    {
                        editorParams["includeColumns"] = rowInstance.Fields
                            .Where(x => x.GetAttribute<LookupIncludeAttribute>() != null)
                            .Select(x => x.PropertyName ?? x.Name)
                            .ToArray();
                    }
                }
                catch 
                {
                }
            }
        }

        private static string AutoDetermineEditorType(Type valueType, Type enumType, IDictionary<string, object> editorParams)
        {
			if (enumType != null)
				return "Enum";
			else if (valueType == typeof(string))
				return "String";
			else if (valueType == typeof(Int32) ||
				valueType == typeof(Int16))
			{
				if (IntegerEditorAttribute.AllowNegativesByDefault)
					editorParams["allowNegatives"] = true;

				if (valueType == typeof(Int16))
					editorParams["maxValue"] = Int16.MaxValue;

				return "Integer";
			}
			else if (valueType == typeof(DateTime))
				return "Date";
			else if (valueType == typeof(bool))
				return "Boolean";
			else if (valueType == typeof(Decimal) || valueType == typeof(Double) || valueType == typeof(Single))
			{
				if (DecimalEditorAttribute.AllowNegativesByDefault)
					editorParams["allowNegatives"] = true;
				return "Decimal";
			}
			else
				return "String";
        }
    }
}