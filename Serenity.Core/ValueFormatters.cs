using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Serenity.Data
{
    public static class ValueFormatters
    {
        public static string DateTime(object context, string column, object value, string format, IFormatProvider formatProvider)
        {
            if (value == null)
                return String.Empty;

            return ((DateTime)value).ToString(format ?? DateHelper.CurrentDateTimeFormat, formatProvider);
        }

        public static ValueFormatter Enum(Int32Field field)
        {
            return Enum(field.EnumType);
        }

        public static ValueFormatter Enum(Type enumType)
        {
            return (context, column, value, format, formatProvider) =>
            {
                return FormatEnum(enumType, value);
            };
        }

        public static string LogDate(object context, string column, object value, string format, IFormatProvider formatProvider)
        {
            return FormatLogDate((DateTime?)value, formatProvider);
        }

        public static ValueFormatter LogDateUser(DateTimeField logDate, StringField logUser)
        {
            return (context, column, value, format, formatProvider) =>
            {
                var row = (Row)context;
                return FormatLogDateUser(logDate[row], logUser[row], formatProvider);
            };
        }

        public static string GetName(this Enum value)
        {
            return System.Enum.GetName(value.GetType(), value);
        }

        public static string GetText(this Enum value)
        {
            return FormatEnum(value.GetType(), value);
        }

        public static string FormatEnum(Type enumType, object value)
        {
            if (value == null)
                return String.Empty;

            if (enumType != null &&
                enumType.IsEnum &&
                System.Enum.GetName(enumType, value) != null)
            {
                var typeName = enumType.Name;
                var enumName = System.Enum.GetName(enumType, value);
                var key = "Enums." + typeName + "." + enumName;
                var text = LocalText.TryGet(key);
                if (text == null)
                {
                    var memInfo = enumType.GetMember(enumName);
                    if (memInfo != null && memInfo.Length == 1)
                    {
                        var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (attributes.Length > 0)
                        {
                            text = ((DescriptionAttribute)attributes[0]).Description;
                            LocalText.Add(new List<LocalText.Entry>
                            {
                                new LocalText.Entry(LocalText.DefaultLanguageID, key, text)
                            }, false);
                        }
                    }
                }
                
                return text ?? enumName;
            }
            else
                return value.ToString();
        }

        public static string FormatLogDate(DateTime? date, IFormatProvider provider)
        {
            if (date == null)
                return String.Empty;
            else
                return date.Value.ToString(DateHelper.CurrentDateTimeFormat.Replace(":ss", ""), provider);
        }

        public static string FormatLogDateUser(DateTime? date, string user, IFormatProvider provider)
        {
            var text = "";
            if (date != null) 
                text = date.Value.ToString(DateHelper.CurrentDateTimeFormat.Replace(":ss", ""), provider);
            if (!String.IsNullOrEmpty(user))
            {
                if (text.Length > 0)
                    text += " ";
                text += "(" + user + ")";
            }
            return text;
        }

        public static void SetEnumFormatter(IDictionary<string, ValueFormatter> formatters, Int32Field field)
        {
            formatters[field.Name] = Enum(field.EnumType);
        }

        public static void SetEnumFormatters(IDictionary<string, ValueFormatter> formatters, Row row)
        {
            foreach (var field in row.GetFields())
            {
                var f = field as Int32Field;
                if (f != null && f.EnumType != null)
                    formatters[f.Name] = Enum(f.EnumType);
            }
        }

        public static void SetLoggingRowFormatters(IDictionary<string, ValueFormatter> formatters, ILoggingRow row)
        {
            //formatters[row.InsertDateField.Name] = LogDate;
            //formatters[row.UpdateDateField.Name] = LogDate;
            //formatters[row.InsertDateField.Name + "User"] = LogDateUser(row.InsertDateField, row.InsertUserField);
            //formatters[row.UpdateDateField.Name + "User"] = LogDateUser(row.UpdateDateField, row.UpdateUserField);
        }

        public static void SetAutomaticFormatters(IDictionary<string, ValueFormatter> formatters, Row row)
        {
            SetEnumFormatters(formatters, row);
            var loggingRow = row as ILoggingRow;
            if (loggingRow != null)
                SetLoggingRowFormatters(formatters, loggingRow);
        }

        public static void ApplyTimezoneOffset(IEnumerable<Row> rows, int timezoneOffset, params DateTimeField[] fields)
        {
            foreach (var row in rows)
                foreach (var field in fields)
                {
                    var date = field[row];
                    if (date != null)
                        field[row] = date.Value.AddMinutes(timezoneOffset);
                }
        }
    }
}
