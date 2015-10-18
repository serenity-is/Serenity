using Newtonsoft.Json;
using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity.PropertyGrid
{
    public partial class PropertyItemHelper
    {
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