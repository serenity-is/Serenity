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

        public static Parameter AddParam<T>(this T self, object value) where T: IDbParameterized
        {
            var param = self.AutoParam();
            self.AddParam(param.Name, value);
            return param;
        }
    }
}