using System;
using System.Collections.Generic;

namespace Serenity
{
    public interface IFilterableSource
    {
        IFilterField FindField(string fieldName);
        IEnumerable<IFilterField> GetFields();
    }
}