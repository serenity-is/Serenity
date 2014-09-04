using System;

namespace Serenity
{
    public static class ExceptionExtensions
    {
        /// <summary>
        ///   Sets custom exception data with given property name and value. Sets the data in base exception.</summary>
        /// <param name="exception">
        ///   Exception to set custom data in.</param>
        /// <param name="property">
        ///   Custom exception data name.</param>
        /// <param name="value">
        ///   Custom exception data value.</param>
        public static void SetData(this Exception exception, string property, object value)
        {
            exception.GetBaseException().Data[property] = value;
        }
    }
}