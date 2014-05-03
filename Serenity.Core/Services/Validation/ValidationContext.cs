using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Serenity.Data;
using System.Data;

namespace Serenity
{
    public interface IValidationContext
    {
        object Value { get; }
        object GetFieldValue(string fieldName);
        IDbConnection Connection { get; }
    }
}