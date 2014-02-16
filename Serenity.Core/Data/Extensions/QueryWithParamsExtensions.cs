using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   Extension methods for classes implementing IDbWhereParam interface.</summary>
    public static class QueryWithParamsExtensions
    {
        public static T SetParam<T>(this T self, Parameter param, object value) where T: IQueryWithParams
        {
            self.SetParam(param.Name, value);
            return self;
        }

        public static Parameter AddParam<T>(this T self, object value) where T: IQueryWithParams
        {
            var param = self.AutoParam();
            self.AddParam(param.Name, value);
            return param;
        }
    }
}