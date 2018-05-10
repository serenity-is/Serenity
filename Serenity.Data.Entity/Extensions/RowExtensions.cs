using Serenity.Data.Mapping;
using System;
using System.Globalization;

namespace Serenity.Data
{
    public static class RowExtensions
    {
        /// <summary>
        ///   <see cref="Row"/> türevi nesnenin bir kopyasını çıkarır</summary>
        /// <remarks>
        ///   Bu fonksiyon bir extension metodu olduğundan direk olarak <c>row.Clone()</c> şeklinde
        ///   kullanılabilir. <see cref="Row.CloneRow()"/> metodu da bir row un kopyasını çıkarır, fakat
        ///   sonuç Row türündendir, tekrar asıl Row türevine typecast yapmak gerekir. Bu yardımcı
        ///   fonksiyonla explicit olarak typecast yapmaya gerek kalmaz.</remarks>
        /// <typeparam name="TRow">
        ///   Row'dan türemiş herhangi bir Row sınıfı</typeparam>
        /// <param name="row">
        ///   Kopyalanacak Row türevi nesne</param>
        /// <returns>
        ///   Fonksiyonun uygulandığı Row türevinin, bir kopyası (sadece alan değerleri)</returns>
        public static TRow Clone<TRow>(this TRow row) where TRow : Row
        {
            return (TRow)(row.CloneRow());
        }

        public static void ApplyDefaultValues(this Row row)
        {
            foreach (var field in row.GetFields())
            {
                var value = field.DefaultValue;
                if (value != null)
                    field.AsObject(row, field.ConvertValue(value, CultureInfo.InvariantCulture));
            }
        }

        public static TRowFields Init<TRowFields>(this TRowFields rowFields) 
            where TRowFields : RowFieldsBase
        {
            rowFields.Initialize();
            return rowFields;
        }

        public static Field GetNameField(this Row row, bool force = false)
        {
            var nameRow = row as INameRow;
            if (nameRow != null)
                return nameRow.NameField;

            var nameProperty = row.GetFields().GetFieldsByAttribute<NamePropertyAttribute>();
            if (nameProperty.Length > 0)
            {
                if (nameProperty.Length > 1)
                    throw new Exception(string.Format(
                        "Row type {0} has multiple fields with [NameProperty] attribute!", 
                        row.GetType().FullName));

                return nameProperty[0];
            }

            if (force)
                throw new Exception(string.Format(
                    "Row type {0} doesn't have a field with [NameProperty] attribute and doesn't implement INameRow!",
                    row.GetType().FullName));

            return null;
        }
    }
}