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
        private static ReportColumn FromMember(MemberInfo member, Type dataType, Field baseField)
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

            var formatAttr = member.GetCustomAttribute<DisplayFormatAttribute>();
            if (formatAttr != null)
                result.Format = formatAttr.Value;
            else
            {
                var dtf = baseField as DateTimeField;
                if (!ReferenceEquals(null, dtf) &&
                    dtf.DateTimeKind != DateTimeKind.Unspecified)
                {
                    result.Format = "dd/MM/yyyy HH:mm";
                }
                else if (!ReferenceEquals(null, dtf) ||
                         dataType == typeof (DateTime) ||
                         dataType == typeof (DateTime?))
                {
                    result.Format = "dd/MM/yyyy";
                }
            }

            if (!ReferenceEquals(null, baseField))
            {
                if (result.Title == null)
                    result.Title = baseField.Title;

                if (result.Width == null && baseField is StringField && baseField.Size != 0)
                    result.Width = baseField.Size;
            }

            result.DataType = dataType;

            return result;
        }

        public static ReportColumn FromFieldInfo(FieldInfo field, Field baseField = null)
        {
            return FromMember(field, field.FieldType, baseField);
        }

        public static ReportColumn FromPropertyInfo(PropertyInfo property, Field baseField = null)
        {
            return FromMember(property, property.PropertyType, baseField);
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

                if (member.GetCustomAttribute<IgnoreAttribute>() != null)
                    continue;

                Field baseField;
                if (basedOnRow != null)
                {
                    var name = ((MemberInfo) fieldInfo ?? propertyInfo).Name;
                    baseField = basedOnRow.FindFieldByPropertyName(name) ?? basedOnRow.FindField(name);
                }
                else
                {
                    baseField = null;
                }

                ReportColumn column;
                if (fieldInfo != null)
                    column = FromFieldInfo(fieldInfo, baseField);
                else
                    column = FromPropertyInfo(propertyInfo, baseField);

                var cellDecorator = member.GetCustomAttribute<CellDecoratorAttribute>();
                if (cellDecorator != null)
                {
                    var decorator = ((ICellDecorator)Activator.CreateInstance(cellDecorator.DecoratorType));
                    column.Decorator = decorator;
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