using System;

namespace Serenity.Data
{
    public static class SqlConversions
    {
        /// <summary>
        ///   Boolean tipinde değeri SQL'e uygun gösterimine çevirir.</summary>
        /// <param name="value">
        ///   SQL gösterimine çevrilecek Boolean değeri.</param>
        /// <returns>
        ///   Boolean değerin SQL'e uygun gösterimi. (true ise "1", false ise "0")</returns>
        /// <remarks>
        ///   <p>Bu bir extension fonksiyonu olduğundan direk <c>value.ToSql()</c> şeklinde de 
        ///   kullanılabilir.</p></remarks>
        public static string ToSql(this bool? value)
        {
            if (!value.HasValue)
                return SqlConsts.Null;
            return value.Value ? "1" : "0";
        }

        /// <summary>
        ///   Double tipinde değeri SQL'e uygun gösterimine çevirir.</summary>
        /// <param name="value">
        ///   SQL gösterimine çevrilecek Double değeri.</param>
        /// <returns>
        ///   Double değerin SQL'e uygun gösterimi. Değer NaN (pseudo-null) ise <c>NULL</c></returns>
        /// <remarks>
        ///   <p>Bu bir extension fonksiyonu olduğundan direk <c>value.ToSql()</c> şeklinde de 
        ///   kullanılabilir.</p></remarks>
        public static string ToSql(this Double? value)
        {
            if (!value.HasValue)
                return SqlConsts.Null;
            return value.Value.ToString(Invariants.NumberFormat);
        }

        /// <summary>
        ///   Decimal tipinde değeri SQL'e uygun gösterimine çevirir.</summary>
        /// <param name="value">
        ///   SQL gösterimine çevrilecek Decimal değeri.</param>
        /// <returns>
        ///   Decimal değerin SQL'e uygun gösterimi. Değer NaN (pseudo-null) ise <c>NULL</c></returns>
        /// <remarks>
        ///   <p>Bu bir extension fonksiyonu olduğundan direk <c>value.ToSql()</c> şeklinde de 
        ///   kullanılabilir.</p></remarks>
        public static string ToSql(this Decimal? value)
        {
            if (!value.HasValue)
                return SqlConsts.Null;
            return value.Value.ToString(Invariants.NumberFormat);
        }


        /// <summary>
        ///   Decimal tipinde değeri SQL'e uygun gösterimine çevirir.</summary>
        /// <param name="value">
        ///   SQL gösterimine çevrilecek Decimal değeri.</param>
        /// <returns>
        ///   Decimal değerin SQL'e uygun gösterimi. Değer NaN (pseudo-null) ise <c>NULL</c></returns>
        /// <remarks>
        ///   <p>Bu bir extension fonksiyonu olduğundan direk <c>value.ToSql()</c> şeklinde de 
        ///   kullanılabilir.</p></remarks>
        public static string ToSql(this Int64? value)
        {
            if (!value.HasValue)
                return SqlConsts.Null;
            return value.Value.ToString(Invariants.NumberFormat);
        }

        /// <summary>
        ///   DateTime tipinde değeri SQL'e uygun gösterimine çevirir.</summary>
        /// <param name="value">
        ///   SQL gösterimine çevrilecek DateTime değeri.</param>
        /// <returns>
        ///   DateTime değerin SQL'e uygun gösterimi. Değer DateTime.MinValue (pseudo-null) ise <c>NULL</c></returns>
        /// <remarks>
        ///   <p>Değerin başına ve sonuna tek tırnak konur ve varsa içindeki tek tırnaklar çiftlenir.</p>
        ///   <p>Bu bir extension fonksiyonu olduğundan direk <c>value.ToSql()</c> şeklinde de 
        ///   kullanılabilir.</p></remarks>
        public static string ToSql(this DateTime? value)
        {
            if (!value.HasValue)
                return SqlConsts.Null;
            
            if (value.Value.Date == value.Value)
                return value.Value.ToString(SqlSettings.CurrentDialect.DateFormat(), Invariants.DateTimeFormat);
            
            return value.Value.ToString(SqlSettings.CurrentDialect.DateTimeFormat(), Invariants.DateTimeFormat);
        }

        /// <summary>
        ///   DateTime tipinde değeri SQL'e uygun gösterimine çevirir.</summary>
        /// <param name="value">
        ///   SQL gösterimine çevrilecek DateTime değeri.</param>
        /// <returns>
        ///   DateTime değerin SQL'e uygun gösterimi. Değer DateTime.MinValue (pseudo-null) ise <c>NULL</c></returns>
        /// <remarks>
        ///   <p>Değerin başına ve sonuna tek tırnak konur ve varsa içindeki tek tırnaklar çiftlenir.</p>
        ///   <p>Bu bir extension fonksiyonu olduğundan direk <c>value.ToSql()</c> şeklinde de 
        ///   kullanılabilir.</p></remarks>
        public static string ToSql(this DateTime value)
        {
            if (value.Date == value)
                return value.ToString(SqlSettings.CurrentDialect.DateFormat(), Invariants.DateTimeFormat);
            return value.ToString(SqlSettings.CurrentDialect.DateTimeFormat(), Invariants.DateTimeFormat);
        }

        /// <summary>
        ///   DateTime tipinde değerin tarih kısmını SQL'e uygun gösterimine çevirir.</summary>
        /// <param name="value">
        ///   Tarih kısmı SQL gösterimine çevrilecek DateTime değeri.</param>
        /// <returns>
        ///   DateTime değerin tarih kısmının SQL'e uygun gösterimi. Değer DateTime.MinValue (pseudo-null) 
        ///   ise <c>NULL</c></returns>
        /// <remarks>
        ///   <p>Kullanılan format <c>'yyyyMMdd'</c> dir.</p>
        ///   <p>Bu bir extension fonksiyonu olduğundan direk <c>value.ToSql()</c> şeklinde de 
        ///   kullanılabilir.</p></remarks>
        public static string ToSqlDate(this DateTime? value)
        {
            if (!value.HasValue)
                return SqlConsts.Null;
            return value.Value.ToString(SqlSettings.CurrentDialect.DateFormat(), Invariants.DateTimeFormat);
        }

        /// <summary>
        ///   DateTime tipinde değerin tarih kısmını SQL'e uygun gösterimine çevirir.</summary>
        /// <param name="value">
        ///   Tarih kısmı SQL gösterimine çevrilecek DateTime değeri.</param>
        /// <returns>
        ///   DateTime değerin tarih kısmının SQL'e uygun gösterimi. Değer DateTime.MinValue (pseudo-null) 
        ///   ise <c>NULL</c></returns>
        /// <remarks>
        ///   <p>Kullanılan format <c>'yyyyMMdd'</c> dir.</p>
        ///   <p>Bu bir extension fonksiyonu olduğundan direk <c>value.ToSql()</c> şeklinde de 
        ///   kullanılabilir.</p></remarks>
        public static string ToSqlDate(this DateTime value)
        {
            return value.ToString(SqlSettings.CurrentDialect.DateFormat(), Invariants.DateTimeFormat);
        }

        /// <summary>
        ///   DateTime tipinde değerin zaman kısmını SQL'e uygun gösterimine çevirir.</summary>
        /// <param name="value">
        ///   Zaman kısmı SQL gösterimine çevrilecek DateTime değeri.</param>
        /// <returns>
        ///   DateTime değerin zaman kısmının SQL'e uygun gösterimi. Değer DateTime.MinValue (pseudo-null) 
        ///   ise <c>NULL</c></returns>
        /// <remarks>
        ///   <p>Kullanılan format <c>'HH:ss:mm'</c> dir.</p>
        ///   <p>Bu bir extension fonksiyonu olduğundan direk <c>value.ToSql()</c> şeklinde de 
        ///   kullanılabilir.</p></remarks>
        public static string ToSqlTime(this DateTime? value)
        {
            if (!value.HasValue)
                return SqlConsts.Null;
            return value.Value.ToString(SqlSettings.CurrentDialect.TimeFormat(), Invariants.DateTimeFormat);
        }

        /// <summary>
        ///   DateTime tipinde değerin zaman kısmını SQL'e uygun gösterimine çevirir.</summary>
        /// <param name="value">
        ///   Zaman kısmı SQL gösterimine çevrilecek DateTime değeri.</param>
        /// <returns>
        ///   DateTime değerin zaman kısmının SQL'e uygun gösterimi. Değer DateTime.MinValue (pseudo-null) 
        ///   ise <c>NULL</c></returns>
        /// <remarks>
        ///   <p>Kullanılan format <c>'HH:ss:mm'</c> dir.</p>
        ///   <p>Bu bir extension fonksiyonu olduğundan direk <c>value.ToSql()</c> şeklinde de 
        ///   kullanılabilir.</p></remarks>
        public static string ToSqlTime(this DateTime value)
        {
            return value.ToString(SqlSettings.CurrentDialect.TimeFormat(), Invariants.DateTimeFormat);
        }

        /// <summary>
        ///   Guid tipinde değeri SQL'e uygun gösterimine çevirir.</summary>
        /// <param name="value">
        ///   SQL gösterimine çevrilecek Guid değeri.</param>
        /// <returns>
        ///   Guid değerin SQL'e uygun gösterimi. Değer Guid.Empty (pseudo-null) ise <c>NULL</c></returns>
        /// <remarks>
        ///   <p>Bu bir extension fonksiyonu olduğundan direk <c>value.ToSql()</c> şeklinde de 
        ///   kullanılabilir.</p></remarks>
        public static string ToSql(this Guid? value)
        {
            if (!value.HasValue)
                return SqlConsts.Null;
            return "'" + value.Value.ToString("D", null) + "'";
        }

        /// <summary>
        ///   String tipinde değeri SQL'e uygun gösterimine çevirir.</summary>
        /// <param name="value">
        ///   SQL'e uygun gösterime çevrilecek String değer.</param>
        /// <returns>
        ///   String değerin SQL'e uygun gösterimi. Değer null ise <c>NULL</c></returns>
        /// <remarks>
        ///   <p>Değerin başına ve sonuna tek tırnak konur ve varsa içindeki tek tırnaklar çiftlenir.</p>
        ///   <p>Bu bir extension fonksiyonu olduğundan direk <c>value.ToSql()</c> şeklinde de 
        ///   kullanılabilir.</p></remarks>
        public static string ToSql(this string value)
        {
            if (value == null)
                return SqlConsts.Null;
            if (SqlSettings.CurrentDialect.PrefixUnicodeStringsWithN())
            {
                if (value.IndexOf('\'') >= 0)
                    return "N'" + value.Replace("'", "''") + "'";
                return "N'" + value + "'";
            }
            if (value.IndexOf('\'') >= 0)
                return "'" + value.Replace("'", "''") + "'";
            return "'" + value + "'";
        }

        public static string ToSql(this int? value)
        {
            if (!value.HasValue)
                return SqlConsts.Null;
            return value.Value.ToString(Invariants.NumberFormat);
        }
    }
}