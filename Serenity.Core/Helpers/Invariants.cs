using System;
using System.Globalization;

namespace Serenity
{
    /// <summary>
    /// Invariant culture related constants and formats.
    /// </summary>
    public static class Invariants
    {
        /// <summary>
        ///   Number format information for invariant culture</summary>
        public static readonly NumberFormatInfo NumberFormat;

        /// <summary>
        ///   Date time format information for invariant culture</summary>
        public static readonly DateTimeFormatInfo DateTimeFormat;

        /// <summary>
        ///   Statik DataHelper contructor'ı. Varsayılan bağlantı string'i ve bağlantı kültürü parametlerini
        ///   initialize eder.
        /// </summary>
        static Invariants()
        {
            NumberFormat = NumberFormatInfo.InvariantInfo;
            DateTimeFormat = DateTimeFormatInfo.InvariantInfo;
        }

        /// <summary>
        /// Determines whether type of the value is an integer type (Int16, Int32, Int64).
        /// Avoid using this function as it is obsolete.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if integer type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIntegerType(object value)
        {
            return value != null &&
                (value is Int32 ||
                 value is Int64 ||
                 value is Int16);
        }

        /// <summary>
        /// Converts value to string using invariant culture.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ToInvariant(this int value)
        {
            return value.ToString(Invariants.NumberFormat);
        }

        /// <summary>
        /// Converts value to string using invariant culture.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Converted string.</returns>
        public static string ToInvariant(this Int64 value)
        {
            return value.ToString(Invariants.NumberFormat);
        }

        /// <summary>
        /// Converts value to string using invariant culture.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Converted string.</returns>
        public static string ToInvariant(this Double value)
        {
            return value.ToString(Invariants.NumberFormat);
        }

        /// <summary>
        /// Converts value to string using invariant culture.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Converted string.</returns>
        public static string ToInvariant(this Decimal value)
        {
            return value.ToString(Invariants.NumberFormat);
        }

        /// <summary>
        ///   <p>Tries to converts an ID's string representation to its numerical ID value (Int64).</p>
        ///   <p>Unlike <see cref="Int64.Parse(string)"/>, <c>null</c>, empty string and all other
        ///   invalid strings results in <see cref="System.Int64"/> value (not an exception).</p></summary>
        /// <param name="str">
        ///   String representation of an ID.</param>
        /// <returns>
        ///   Numerical ID value or Null.Int64 if null, empty, or invalid string.</returns>
        /// <seealso cref="TryParseID(string)"/>
        /// <seealso cref="Int64.Parse(string)"/>
        public static Int64? TryParseID(this string str)
        {
            if (long.TryParse(str, out long id))
                return id;
            else
                return null;
        }

        /// <summary>
        ///   <p>Tries to converts an ID's string representation to its numerical ID value (Int64).</p>
        ///   <p>Unlike <see cref="Int64.Parse(string)"/>, <c>null</c>, empty string and all other
        ///   invalid strings results in <see cref="System.Int64"/> value (not an exception).</p></summary>
        /// <param name="str">
        ///   String representation of an ID.</param>
        /// <returns>
        ///   Numerical ID value or Null.Int64 if null, empty, or invalid string.</returns>
        /// <seealso cref="TryParseID(string)"/>
        /// <seealso cref="Int64.Parse(string)"/>
        public static Int32? TryParseID32(this string str)
        {
            if (int.TryParse(str, out int id))
                return id;
            else
                return null;
        }

        /// <summary>
        ///   Converts an ID value, to its string representation.</summary>
        /// <param name="id">
        ///   ID value.</param>
        /// <returns>
        ///   If <paramref name="id"/> has <see cref="System.Int64"/> value, <c>String.Empty</c>, 
        ///   otherwise its string representation</returns>
        public static string IDString(this Int64? id)
        {
            if (id == null)
                return String.Empty;
            else
                return id.ToString();
        }
    }
}