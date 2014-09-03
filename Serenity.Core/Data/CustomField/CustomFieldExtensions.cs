using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    public static class CustomFieldExtensions
    {
        private const FieldFlags Required = FieldFlags.Required;
        private const FieldFlags NotRequired = FieldFlags.Default;

        public static LocalText ToLocalText(string caption)
        {
            LocalText lt = null;
            if (!caption.IsNullOrEmpty())
                lt = new LocalText(caption);
            return lt;
        }

        public static Field[] ToFieldArray(this IEnumerable<ICustomFieldDefinition> items,
            ICollection<Field> collection, string namePrefix, Action<Field, ICustomFieldDefinition> initialize = null)
        {
            var result = new List<Field>();
            foreach (var item in items)
            {
                var flags = item.IsRequired ? Required : NotRequired;
                var caption = ToLocalText(item.Title);
                var name = (namePrefix ?? "") + item.Name;
                Field field;

                switch (item.FieldType)
                {
                    case CustomFieldType.Boolean:
                        field = new BooleanField(collection, name, caption, 0, flags);
                        break;
                    case CustomFieldType.Date:
                        field = new DateTimeField(collection, name, caption, 0, flags) { DateTimeKind = DateTimeKind.Unspecified };
                        break;
                    case CustomFieldType.DateTime:
                        field = new DateTimeField(collection, name, caption, 0, flags) { DateTimeKind = DateTimeKind.Local };
                        break;
                    case CustomFieldType.Decimal:
                        field = new DecimalField(collection, name, caption, item.Size, flags);
                        break;
                    case CustomFieldType.Int32:
                        field = new Int32Field(collection, name, caption, item.Size, flags);
                        break;
                    case CustomFieldType.Int64:
                        field = new Int64Field(collection, name, caption, item.Size, flags);
                        break;
                    default:
                        field = new StringField(collection, name, caption, item.Size, flags);
                        break;
                }

                field.DefaultValue = item.DefaultValue.TrimToNull();
                result.Add(field);

                if (initialize != null)
                    initialize(field, item);
            }

            return result.ToArray();
        }

        //public static TField[] ToFieldArray<TField>(this IEnumerable<CustomFieldCache.CustomFieldItem> items,
        //    ICollection<Field> collection, string namePrefix, Func<ICollection<Field>, string, LocalText, int, FieldFlags, TField> createField)
        //{
        //    var result = new List<Field>();
        //    foreach (var item in items)
        //    {
        //        var meta = new FieldMeta((namePrefix ?? "") + item.Name,
        //            ToLocalText(item.Caption), item.Size, item.Required ? Required : NotRequired);

        //        Field field = createField(meta);
        //        result.Add(field);
        //    }

        //    return result.ToArray();
        //}

        //public static TField[] ToFieldArray<TField>(this IEnumerable<CustomFieldCache.CustomFieldItem> items,
        //    ICollection<Field> collection, string namePrefix, Func<FieldMeta, TField> createField)
        //    where TField: Field
        //{
        //    var result = new List<TField>();
        //    foreach (var item in items)
        //    {
        //        var meta = new FieldMeta((namePrefix ?? "") + item.Name,
        //            ToLocalText(item.Caption), item.Size, item.Required ? Required : NotRequired);

        //        TField field = createField(meta);
        //        result.Add(field);
        //    }

        //    return result.ToArray();
        //}

        //public static void DefaultFieldCaption(this RowFieldsBase fields, string fieldName, string caption)
        //{
        //    LocalText.Add(new List<LocalText.Entry>() { new LocalText.Entry(LocalText.DefaultLanguageID, "Db." + fields.TableName + "." + fieldName,
        //        caption) }, false);
        //}
    }
}