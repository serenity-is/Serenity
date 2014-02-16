using System;
using System.Data;

namespace Serenity.Data
{
    /// <summary>
    ///   Extension methods for IDataReader objects.</summary>
    public static class DataReaderExtensions
    {
        /// <summary>
        ///   Reads value at field index. Returns DbNull as Null.DateTime.</summary>
        /// <param name="reader">
        ///   Reader (required).</param>
        /// <param name="index">
        ///   Index.</param>
        /// <returns>
        ///   Field value or Null.DateTime if value is DbNull.</returns>
        public static DateTime? AsDateTime(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;

            return reader.GetDateTime(index);
        }

        /// <summary>
        ///   Reads and converts value at field index to DateTime. Returns DbNull as Null.DateTime.</summary>
        /// <param name="reader">
        ///   Reader (required).</param>
        /// <param name="index">
        ///   Index.</param>
        /// <returns>
        ///   Field value or Null.DateTime if value is DbNull.</returns>
        public static DateTime? ToDateTime(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;

            return Convert.ToDateTime(reader.GetValue(index));
        }

        /// <summary>
        ///   Reads value at field index. Returns DbNull as Null.Decimal.</summary>
        /// <param name="reader">
        ///   Reader (required).</param>
        /// <param name="index">
        ///   Index.</param>
        /// <returns>
        ///   Field value or Null.Decimal if value is DbNull.</returns>
        public static Decimal? AsDecimal(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;

            return reader.GetDecimal(index);
        }

        /// <summary>
        ///   Reads and converts value at field index to Decimal. Returns DbNull as Null.Decimal.</summary>
        /// <param name="reader">
        ///   Reader (required).</param>
        /// <param name="index">
        ///   Index.</param>
        /// <returns>
        ///   Field value or Null.Decimal if value is DbNull.</returns>
        public static Decimal? ToDecimal(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;

            return Convert.ToDecimal(reader.GetValue(index));
        }

        /// <summary>
        ///   Reads value at field index. Returns DbNull as Null.Double.</summary>
        /// <param name="reader">
        ///   Reader (required).</param>
        /// <param name="index">
        ///   Index.</param>
        /// <returns>
        ///   Field value or Null.Double if value is DbNull.</returns>
        public static Double? AsDouble(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;

            return reader.GetDouble(index);
        }

        /// <summary>
        ///   Reads and converts value at field index to Double. Returns DbNull as Null.Double.</summary>
        /// <param name="reader">
        ///   Reader (required).</param>
        /// <param name="index">
        ///   Index.</param>
        /// <returns>
        ///   Field value or Null.Double if value is DbNull.</returns>
        public static Double? ToDouble(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;

            return Convert.ToDouble(reader.GetValue(index));
        }

        /// <summary>
        ///   Reads value at field index. Returns DbNull as Null.Int32.</summary>
        /// <param name="reader">
        ///   Reader (required).</param>
        /// <param name="index">
        ///   Index.</param>
        /// <returns>
        ///   Field value or Null.Int32 if value is DbNull.</returns>
        public static Int32? AsInt32(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;

            return reader.GetInt32(index);
        }

        /// <summary>
        ///   Reads and converts value at field index to Int32. Returns DbNull as Null.Int32.</summary>
        /// <param name="reader">
        ///   Reader (required).</param>
        /// <param name="index">
        ///   Index.</param>
        /// <returns>
        ///   Field value or Null.Int32 if value is DbNull.</returns>
        public static Int32? ToInt32(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;

            return Convert.ToInt32(reader.GetValue(index));
        }

        /// <summary>
        ///   Reads value at field index. Returns DbNull as Null.Int64.</summary>
        /// <param name="reader">
        ///   Reader (required).</param>
        /// <param name="index">
        ///   Index.</param>
        /// <returns>
        ///   Field value or Null.Int64 if value is DbNull.</returns>
        public static Int64? AsInt64(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;

            return reader.GetInt64(index);
        }

        /// <summary>
        ///   Reads and converts value at field index to Int64. Returns DbNull as Null.Int64.</summary>
        /// <param name="reader">
        ///   Reader (required).</param>
        /// <param name="index">
        ///   Index.</param>
        /// <returns>
        ///   Field value or Null.Int64 if value is DbNull.</returns>
        public static Int64? ToInt64(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;

            return Convert.ToInt64(reader.GetValue(index));
        }

        /// <summary>
        ///   Reads value at field index. Returns DbNull as null.</summary>
        /// <param name="reader">
        ///   Reader (required).</param>
        /// <param name="index">
        ///   Index.</param>
        /// <returns>
        ///   Field value or null if value is DbNull.</returns>
        public static String AsString(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;

            return reader.GetString(index);
        }

        /// <summary>
        ///   Reads and converts value at field index to String. Returns DbNull as null.</summary>
        /// <param name="reader">
        ///   Reader (required).</param>
        /// <param name="index">
        ///   Index.</param>
        /// <returns>
        ///   Field value or null if value is DbNull.</returns>
        public static String ToString(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;

            return Convert.ToString(reader.GetValue(index));
        }
    }
}