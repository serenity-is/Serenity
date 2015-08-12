using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Provides extension methods to work with identity values
    /// </summary>
    public static class IdExtensions
    {
        /// <summary>
        /// Converts value to its 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int64? ConvertToId(this object value)
        {
            return Q.Externals.ToId(value);
        }

        public static Int32? ToInt32(this object value)
        {
            return Q.Externals.ToId(value).As<Int32?>();
        }

        public static bool IsPositiveId(Int64 id)
        {
            if (!Script.IsValue(id))
                return false;
            else if (Script.TypeOf(id) == "string")
            {
                string idStr = id.As<string>();
                if (idStr.StartsWith("-"))
                    return false;

                return idStr.Length > 0;
            }
            else if (Script.TypeOf(id) == "number")
                return id.As<long>() > 0;
            else
                return true;
        }

        public static bool IsNegativeId(Int64 id)
        {
            if (!Script.IsValue(id))
                return false;
            else if (Script.TypeOf(id) == "string")
            {
                string idStr = id.As<string>();
                if (idStr.StartsWith("-"))
                    return true;
                return false;
            }
            else if (Script.TypeOf(id) == "number")
                return id.As<long>() < 0;
            else
                return false;
        }
    }
}