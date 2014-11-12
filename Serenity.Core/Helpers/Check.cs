using System;
using System.Text;

namespace Serenity
{
    /// <summary>
    /// Contains argument validation methods
    /// </summary>
    public static class Check
    {
        /// <summary>
        /// Checks argument is not null
        /// </summary>
        public static void NotNull(object value, string paramName)
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Checks argument is not null or empty
        /// </summary>
        public static void NotNullOrEmpty(string value, string paramName)
        {
            if (value == null)
                throw new ArgumentNullException(paramName);

            if (value.Length == 0)
                throw new ArgumentException(String.Format("'{0}' cannot be empty!", paramName), paramName);
        }

        /// <summary>
        /// Checks argument is not null or string containing whitespace only
        /// </summary>
        public static void NotNullOrWhiteSpace(string value, string paramName)
        {
            if (value == null)
                throw new ArgumentNullException(paramName);

            if (value.Length == 0)
                throw new ArgumentException(String.Format("'{0}' cannot be empty!", paramName), paramName);

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(String.Format("'{0}' cannot be whitespace!", paramName), paramName);
        }
    }
}