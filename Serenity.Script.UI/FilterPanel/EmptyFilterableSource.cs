using System;
using System.Collections.Generic;

namespace Serenity
{
    public class EmptyFilterableSource : IFilterableSource
    {
        public IFilterField FindField(string fieldName)
        {
            return null;
        }

        public IEnumerable<IFilterField> GetFields()
        {
            return new List<IFilterField>();
        }
    }
}