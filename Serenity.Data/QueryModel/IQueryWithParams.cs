using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   Interface for objects setting parameters by PARAM method (like SqlInsert, SqlUpdate, SqlDelete...)</summary>
    public interface IQueryWithParams
    {
        void AddParam(string name, object value);
        void SetParam(string name, object value);
        Parameter AutoParam();
        ISqlDialect Dialect { get; }
        IDictionary<string, object> Params { get; }
    }
}