using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Data.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Serenity.Reporting
{
    public static class ReportColumnConverter
    {
        private static ReportColumn FromMember(MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            var result = new ReportColumn();
            result.Name = member.Name;
            var displayAttr = member.GetCustomAttribute<DisplayNameAttribute>();
            if (displayAttr != null)
                result.Title = displayAttr.DisplayName;

            var sizeAttr = member.GetCustomAttribute<SizeAttribute>();
            if (sizeAttr != null && sizeAttr.Value != 0)
                result.Width = sizeAttr.Value;

            return result;
        }

        public static ReportColumn FromFieldInfo(FieldInfo field)
        {
            var result = FromMember(field);
            result.DataType = field.FieldType;
            return result;
        }

        public static ReportColumn FromPropertyInfo(PropertyInfo property)
        {
            var result = FromMember(property);
            result.DataType = property.PropertyType;
            return result;
        }

        public static List<ReportColumn> ObjectTypeToList(Type objectType)
        {
            var list = new List<ReportColumn>();

            Row basedOnRow = null;
            var basedOnRowAttr = objectType.GetCustomAttribute<BasedOnRowAttribute>();
            if (basedOnRowAttr != null)
                basedOnRow = Activator.CreateInstance(basedOnRowAttr.RowType) as Row;

            foreach (MemberInfo member in objectType.GetMembers(BindingFlags.Instance | BindingFlags.Public))
            {
                var fieldInfo = member as FieldInfo;
                var propertyInfo = member as PropertyInfo;
                if (fieldInfo == null &&
                    propertyInfo == null)
                    continue;

                if (member.GetCustomAttribute<HiddenAttribute>() != null)
                    continue;

                ReportColumn column;
                if (fieldInfo != null)
                    column = FromFieldInfo(fieldInfo);
                else
                    column = FromPropertyInfo(propertyInfo);

                if (basedOnRow != null)
                {
                    var field = basedOnRow.FindFieldByPropertyName(column.Name) ?? basedOnRow.FindField(column.Name);
                    if (field != null)
                    {
                        if (column.Title == null)
                            column.Title = field.Title;

                        if (column.Width == null && field is StringField && field.Size != 0)
                            column.Width = field.Size;
                    }
                }

                list.Add(column);
            }

            return list;
        }

        public static ReportColumn FromField(Field field)
        {
            var column = new ReportColumn();
            column.Name = field.Name;
            column.Title = field.Title;

            if (field is StringField)
                if (field.Size != 0)
                    column.Width = field.Size;

            return column;
        }

        public static List<ReportColumn> EntityTypeToList(Row instance)
        {
            var list = new List<ReportColumn>();

            foreach (var field in instance.GetFields())
            {
                list.Add(FromField(field));
            }

            return list;
        }
    }
}