using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   Interface for objects setting parameters by PARAM method (like SqlInsert, SqlUpdate, SqlDelete...)</summary>
    /// <typeparam name="T">
    ///   Type of the parameterized object itself.</typeparam>
    public interface IDbParameterized
    {
        void AddParam(string name, object value);
        void SetParam(string name, object value);
        Parameter AutoParam();
        SqlDialect Dialect { get; }
    }
}