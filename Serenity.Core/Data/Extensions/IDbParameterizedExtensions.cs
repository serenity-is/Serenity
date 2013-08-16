using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   Extension methods for classes implementing IDbWhereParam interface.</summary>
    public static class IDbParameterizedExtensions
    {
        public static T SetParam<T>(this T self, Parameter param, object value) where T: IDbParameterized
        {
            self.SetParam(param.Name, value);
            return self;
        }

        public static Parameter Param<T>(this T self, object value) where T: IDbParameterized
        {
            var p = self.AutoParam();
            self.SetParam(p.Name, value);
            return p;
        }

        public static T Set<T>(this T self, Parameter param, object value) where T: IDictionary<string, object>
        {
            self[param.Name] = value;
            return self;
        }
    }
}