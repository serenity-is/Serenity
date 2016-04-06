using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Provides extension methods to work with identity values
    /// </summary>
    public static class IdExtensions
    {
        [Obsolete("Use ToInt64 or ToInt32")]
        [InlineCode("Q.toId({value})")]
        public static Int64? ConvertToId(this object value)
        {
            return null;
        }

        /// <summary>
        /// Tries to convert value to Int64 type.
        /// Use this with care as integer values > 15 digits
        /// are actually returned as a string to handle problems
        /// with big ID values, as javascript actually stores
        /// numbers as double, which has limited scale
        /// </summary>
        [InlineCode("Q.toId({value})")]
        public static Int64? ToInt64(this object value)
        {
            return null;
        }

        /// <summary>
        /// Tries to convert value to Int32 type.
        /// Use this with care as integer values > 15 digits
        /// are actually returned as a string to handle problems
        /// with big ID values, as javascript actually stores
        /// numbers as double, which has limited scale
        /// </summary>
        [InlineCode("Q.toId({value})")]
        public static Int32? ToInt32(this object value)
        {
            return null;
        }
    }
}