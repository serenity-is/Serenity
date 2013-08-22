using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   Interface for objects setting parameters by PARAM method (like SqlInsert, SqlUpdate, SqlDelete...)</summary>
    /// <typeparam name="T">
    ///   Type of the parameterized object itself.</typeparam>
    public interface IDbParameterized
    {
        Dictionary<string, object> Params { get; set; }
    }
}