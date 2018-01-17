using jQueryApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Serenity
{
    [Imported]
    public static class ValidationHelper
    {
        public static bool AsyncSubmit(jQueryObject form, Func<bool> validateBeforeSave, Action submitHandler)
        {
            var validator = form.As<jQueryValidationObject>().Validate();

            dynamic valSettings = validator.As<dynamic>().settings;
            if (valSettings.abortHandler != null)
                return false;

            if (validateBeforeSave != null &&
                validateBeforeSave() == false)
                return false;

            valSettings["abortHandler"] = new Action<jQueryValidator>(Q.Externals.ValidatorAbortHandler);
            valSettings["submitHandler"] = new Func<bool>(delegate()
            {
                if (submitHandler != null)
                    submitHandler();
                return false;
            });

            form.Trigger("submit");
            return true;
        }

        public static bool Submit(jQueryObject form, Func<bool> validateBeforeSave, Action submitHandler)
        {
            var validator = form.As<jQueryValidationObject>().Validate();

            dynamic valSettings = validator.As<dynamic>().settings;
            if (valSettings.abortHandler != null)
                return false;

            if (validateBeforeSave != null &&
                validateBeforeSave() == false)
                return false;

            if (!validator.ValidateForm())
                return false;
                
            if (submitHandler != null)
                submitHandler();

            return true;
        }

        public static jQueryValidator GetValidator(this jQueryObject element)
        {
            var form = element.Closest("form");
            if (form.Length == 0)
                return null;

            return (jQueryValidator)form.GetDataValue("validator");
        }

        [InlineCode("{element}.valid()")]
        public static bool Valid(this jQueryObject element)
        {
            return true;
        }
    }

}