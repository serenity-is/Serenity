using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Provides extension methods to work with identity values
    /// </summary>
    public static class IdExtensions
    {
        public static Int64? ConvertToId(this object value)
        {
            return Q.Externals.ToId(value);
        }

        public static bool IsPositiveId(Int64 id)
        {
            if (!Script.IsValue(id))
                return false;
            else if (Type.GetScriptType(id) == "string")
            {
                string idStr = id.As<string>();
                if (idStr.StartsWith("-"))
                    return false;

                return idStr.Length > 0;
            }
            else if (Type.GetScriptType(id) == "number")
                return id.As<long>() > 0;
            else
                return true;
        }

        public static bool IsNegativeId(Int64 id)
        {
            if (!Script.IsValue(id))
                return false;
            else if (Type.GetScriptType(id) == "string")
            {
                string idStr = id.As<string>();
                if (idStr.StartsWith("-"))
                    return true;
                return false;
            }
            else if (Type.GetScriptType(id) == "number")
                return id.As<long>() < 0;
            else
                return false;
        }
    }
}