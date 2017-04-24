namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;

    [IgnoreNamespace]
    [Imported(ObeysTypeSystem = true)]
    [ScriptName("Array")]
    public abstract class BaseCriteria
    {
        public bool IsEmpty
        {
            [InlineCode("Serenity.Criteria.isEmpty({this})")]
            get { return false; }
        }

        [InlineCode("['is null', {this}]")]
        public BaseCriteria IsNull()
        {
            return null;
        }

        [InlineCode("['is not null', {this}]")]
        public BaseCriteria IsNotNull()
        {
            return null;
        }

        [InlineCode("[{this}, 'like', {mask}]")]
        public BaseCriteria Like(string mask)
        {
            return null;
        }

        [InlineCode("[{this}, 'not like', {mask}]")]
        public BaseCriteria NotLike(string mask)
        {
            return null;
        }

        [InlineCode("[{this}, 'like', ({mask} + '%')]")]
        public BaseCriteria StartsWith(string mask)
        {
            return null;
        }

        [InlineCode("[{this}, 'like', ('%' + {mask})]")]
        public BaseCriteria EndsWith(string mask)
        {
            return null;
        }

        [InlineCode("[{this}, 'like', ('%' + {mask} + '%')]")]
        public BaseCriteria Contains(string mask)
        {
            return null;
        }

        [InlineCode("[{this}, 'contains', {mask}]")]
        public BaseCriteria FullTextSearchContains(string mask)
        {
            return null;
        }

        [InlineCode("[{this}, 'not like', ('%' + {mask} + '%')]")]
        public BaseCriteria NotContains(string mask)
        {
            return null;
        }

        [InlineCode("[{this}, 'in', [{values}]]")]
        public BaseCriteria In<T>(params T[] values)
        {
            return null;
        }

        [InlineCode("[{this}, 'in', {statement}]")]
        public BaseCriteria In(BaseCriteria statement)
        {
            return null;
        }

        [InlineCode("[{this}, 'not in', [{values}]]")]
        public BaseCriteria NotIn<T>(params T[] values)
        {
            return null;
        }

        [InlineCode("[{this}, 'not in', {statement}]")]
        public BaseCriteria NotIn(BaseCriteria statement)
        {
            return null;
        }

        [InlineCode("['not', {criteria}]")]
        public static BaseCriteria operator !(BaseCriteria criteria)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '=', {criteria2}]")]
        public static BaseCriteria operator ==(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '=', {value}]")]
        public static BaseCriteria operator ==(BaseCriteria criteria1, int value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '=', {value}]")]
        public static BaseCriteria operator ==(BaseCriteria criteria1, Int64 value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '=', {value}]")]
        public static BaseCriteria operator ==(BaseCriteria criteria1, string value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '=', {value}]")]
        public static BaseCriteria operator ==(BaseCriteria criteria1, Double value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '=', {value}]")]
        public static BaseCriteria operator ==(BaseCriteria criteria1, Decimal value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '=', {value}]")]
        public static BaseCriteria operator ==(BaseCriteria criteria1, JsDate value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '=', {value}]")]
        public static BaseCriteria operator ==(BaseCriteria criteria1, Guid value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '!=', {criteria2}]")]
        public static BaseCriteria operator !=(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '!=', {value}]")]
        public static BaseCriteria operator !=(BaseCriteria criteria1, int value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '!=', {value}]")]
        public static BaseCriteria operator !=(BaseCriteria criteria1, Int64 value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '!=', {value}]")]
        public static BaseCriteria operator !=(BaseCriteria criteria1, string value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '!=', {value}]")]
        public static BaseCriteria operator !=(BaseCriteria criteria1, Double value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '!=', {value}]")]
        public static BaseCriteria operator !=(BaseCriteria criteria1, Decimal value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '!=', {value}]")]
        public static BaseCriteria operator !=(BaseCriteria criteria1, JsDate value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '!=', {value}]")]
        public static BaseCriteria operator !=(BaseCriteria criteria1, Guid value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '>', {criteria2}]")]
        public static BaseCriteria operator >(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '>', {value}]")]
        public static BaseCriteria operator >(BaseCriteria criteria1, int value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '>', {value}]")]
        public static BaseCriteria operator >(BaseCriteria criteria1, Int64 value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '>', {value}]")]
        public static BaseCriteria operator >(BaseCriteria criteria1, string value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '>', {value}]")]
        public static BaseCriteria operator >(BaseCriteria criteria1, Double value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '>', {value}]")]
        public static BaseCriteria operator >(BaseCriteria criteria1, Decimal value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '>', {value}]")]
        public static BaseCriteria operator >(BaseCriteria criteria1, JsDate value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '>', {value}]")]
        public static BaseCriteria operator >(BaseCriteria criteria1, Guid value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '>=', {criteria2}]")]
        public static BaseCriteria operator >=(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '>=', {value}]")]
        public static BaseCriteria operator >=(BaseCriteria criteria1, int value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '>=', {value}]")]
        public static BaseCriteria operator >=(BaseCriteria criteria1, Int64 value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '>=', {value}]")]
        public static BaseCriteria operator >=(BaseCriteria criteria1, string value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '>=', {value}]")]
        public static BaseCriteria operator >=(BaseCriteria criteria1, Double value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '>=', {value}]")]
        public static BaseCriteria operator >=(BaseCriteria criteria1, Decimal value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '>=', {value}]")]
        public static BaseCriteria operator >=(BaseCriteria criteria1, JsDate value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '>=', {value}]")]
        public static BaseCriteria operator >=(BaseCriteria criteria1, Guid value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '<', {criteria2}]")]
        public static BaseCriteria operator <(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '<', {value}]")]
        public static BaseCriteria operator <(BaseCriteria criteria1, int value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '<', {value}]")]
        public static BaseCriteria operator <(BaseCriteria criteria1, Int64 value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '<', {value}]")]
        public static BaseCriteria operator <(BaseCriteria criteria1, string value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '<', {value}]")]
        public static BaseCriteria operator <(BaseCriteria criteria1, Double value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '<', {value}]")]
        public static BaseCriteria operator <(BaseCriteria criteria1, Decimal value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '<', {value}]")]
        public static BaseCriteria operator <(BaseCriteria criteria1, JsDate value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '<', {value}]")]
        public static BaseCriteria operator <(BaseCriteria criteria1, Guid value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '<=', {criteria2}]")]
        public static BaseCriteria operator <=(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '<=', {value}]")]
        public static BaseCriteria operator <=(BaseCriteria criteria1, int value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '<=', {value}]")]
        public static BaseCriteria operator <=(BaseCriteria criteria1, Int64 value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '<=', {value}]")]
        public static BaseCriteria operator <=(BaseCriteria criteria1, string value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '<=', {value}]")]
        public static BaseCriteria operator <=(BaseCriteria criteria1, Double value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '<=', {value}]")]
        public static BaseCriteria operator <=(BaseCriteria criteria1, Decimal value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '<=', {value}]")]
        public static BaseCriteria operator <=(BaseCriteria criteria1, JsDate value)
        {
            return null;
        }

        [InlineCode("[{criteria1}, '<=', {value}]")]
        public static BaseCriteria operator <=(BaseCriteria criteria1, Guid value)
        {
            return null;
        }

        [InlineCode("Serenity.Criteria.join({criteria1}, 'and', {criteria2})")]
        public static BaseCriteria operator &(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return null;
        }

        [InlineCode("Serenity.Criteria.join({criteria1}, 'or', {criteria2})")]
        public static BaseCriteria operator |(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return null;
        }

        [InlineCode("Serenity.Criteria.join({criteria1}, 'xor', {criteria2})")]
        public static BaseCriteria operator ^(BaseCriteria criteria1, BaseCriteria criteria2)
        {
            return null;
        }

        [InlineCode("Serenity.Criteria.paren({criteria})")]
        public static BaseCriteria operator ~(BaseCriteria criteria)
        {
            return null;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}