using jQueryApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Serenity
{
    public static class ValidationHelper
    {
        public static bool TriggerSubmit(jQueryObject form, Func<bool> validateBeforeSave, Action submitHandler)
        {
            var validator = form.As<jQueryValidationObject>().Validate();

            object valSettings = Type.GetField(validator, "settings");
            if (Type.GetField(valSettings, "abortHandler") != null)
                return false;

            if (validateBeforeSave != null &&
                validateBeforeSave() == false)
                return false;

            Type.SetField(valSettings, "abortHandler", new Action<jQueryValidator>(Q.Externals.ValidatorAbortHandler));
            Type.SetField(valSettings, "submitHandler", new Func<bool>(delegate()
            {
                if (submitHandler != null)
                    submitHandler();
                return false;
            }));

            form.Trigger("submit");
            return true;
        }
    }

}