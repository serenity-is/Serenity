using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Serenity.Data;

namespace Serenity
{
    public interface ICustomValidator
    {
        string Validate(IValidationContext context);
    }
}