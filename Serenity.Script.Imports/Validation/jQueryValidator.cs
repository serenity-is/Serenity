using System;
using System.Collections;
using System.Html;
using System.Runtime.CompilerServices;

namespace jQueryApi
{
    public delegate bool jQueryValidationHighlight(Element element, string errorClass, string validClass);
    public delegate bool jQueryValidationMethod(string value, Element element, object[] parameters);

    [Imported, IgnoreNamespace]
    public sealed class jQueryValidator
    {
        private jQueryValidator()
        {
        }

        /// <summary>
        /// Add compound class methods
        /// </summary>
        /// <param name="rules">A map of class name - rules pairs.</param>
        [ScriptAlias("$.validator.addClassRules")]
        public static void AddClassRules(JsDictionary rules)
        {
        }

        /// <summary>
        /// Adds a compound class method
        /// </summary>
        /// <param name="name">The name of the class rule to add</param>
        /// <param name="rules">The compound rules</param>
        [ScriptAlias("$.validator.addClassRules")]
        public static void AddClassRules(string name, JsDictionary rules)
        {
        }

        /// <summary>
        /// Adds a custom validation method.
        /// </summary>
        /// <param name="name">The name of the method. Must be a valid Javascript identifier</param>
        /// <param name="callback">The method callback to invoke during validation</param>
        [ScriptAlias("$.validator.addMethod")]
        public static void AddMethod(string name, jQueryValidationMethod callback)
        {
        }

        /// <summary>
        /// Adds a custom validation method.
        /// </summary>
        /// <param name="name">The name of the method. Must be a valid Javascript identifier</param>
        /// <param name="callback">The method callback to invoke during validation</param>
        /// <param name="message">The default message to display for this method.</param>
        [ScriptAlias("$.validator.addMethod")]
        public static void AddMethod(string name, jQueryValidationMethod callback, string message)
        {
        }

        /// <summary>
        /// Formats a string, replacing {n} placeholders with arguments.
        /// </summary>
        /// <param name="template">The template string</param>
        /// <param name="args">The arguments</param>
        /// <returns>A formatted string where each placeholder has been replaced by its argument.</returns>
        [ScriptAlias("$.validator.format")]
        public static string Format(string template, params object[] args)
        {
            return null;
        }

        /// <summary>
        /// Gets the number of invalid fields on the controlled form.
        /// </summary>
        /// <returns>The number of invalid fields</returns>
        [ScriptName("numberOfInvalids")]
        public int GetInvalidFieldCount()
        {
            return 0;
        }
        /// <summary>
        /// Checks if the current element is optional.
        /// </summary>
        /// <param name="element">The element to validate</param>
        /// <returns>True if the element is optional, otherwise returns false.</returns>
        public static bool Optional(Element element)
        {
            return false;
        }

        /// <summary>
        /// Resets the controlled form
        /// </summary>
        public void ResetForm()
        {
        }

        /// <summary>
        /// Sets the default settings for validation
        /// </summary>
        /// <param name="options">The validation options to by used by default during validation</param>
        [ScriptAlias("$.validator.setDefaults")]
        public static void SetDefaults(jQueryValidatorOptions options)
        {
        }

        /// <summary>
        /// Shows the specified errors messages.
        /// </summary>
        /// <param name="errors">Key/value pairs of error messages, where the keys relate to names of elements in the form, and
        /// the values are the error messages to display for those elements.</param>
        public void ShowErrors(JsDictionary errors)
        {
        }

        /// <summary>
        /// Validates the specified element
        /// </summary>
        /// <param name="element">The element to validate</param>
        /// <returns>True if the form is valid, otherwise returns false.</returns>
        [ScriptName("element")]
        public bool ValidateElement(Element element)
        {
            return false;
        }

        /// <summary>
        /// Validates the controlled form
        /// </summary>
        /// <returns>True if the form is valid, otherwise returns false.</returns>
        [ScriptName("form")]
        public bool ValidateForm()
        {
            return false;
        }
    }

    [Imported, IgnoreNamespace, ScriptName("Object")]
    public sealed class jQueryValidationRules
    {

        private jQueryValidationRules()
        {
        }

        /// <summary>
        /// Makes the element require a certain file extension
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// Makes the element require a creditcard number
        /// </summary>
        [ScriptName("creditcard")]
        public bool CreditCard { get; set; }

        /// <summary>
        /// Makes the element require a date
        /// </summary>
        public bool Date { get; set; }

        /// <summary>
        /// Makes the element require a ISO date
        /// </summary>
        [ScriptName("dateISO")]
        public bool DateISO { get; set; }

        /// <summary>
        /// Makes the element require digits only
        /// </summary>
        public bool Digits { get; set; }

        /// <summary>
        /// Makes the element require a valid email address
        /// </summary>
        public bool Email { get; set; }

        /// <summary>
        /// Requires the element to be the same as another one
        /// </summary>
        public string EqualTo { get; set; }

        /// <summary>
        /// Makes the element require a given maximum
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        /// Makes the element require a given maximum length
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Makes the element require given minimum
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        /// Makes the element require a given minimum length
        /// </summary>
        public int MinLength { get; set; }

        /// <summary>
        /// Makes the element require a decimal number
        /// </summary>
        public bool Number { get; set; }

        [ScriptName("phoneUS")]
        public bool PhoneUS { get; set; }

        /// <summary>
        /// Makes the value require a given value range
        /// </summary>
        public int[] Range { get; set; }

        /// <summary>
        /// Makes the element require a given value range
        /// </summary>
        public int[] RangeLength { get; set; }

        /// <summary>
        /// Requests a resource to check the element for validity
        /// </summary>
        public object Remote { get; set; }

        /// <summary>
        /// Makes the element required, depending on the result of the given callback
        /// </summary>
        [ScriptName("required")]
        public Func<bool> RequiredCallback { get; set; }

        /// <summary>
        /// Makes the element required, depending on the result of the given expression
        /// </summary>
        [ScriptName("required")]
        public string RequiredExpression { get; set; }

        /// <summary>
        /// Makes the element always required
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Makes the element require a valid url
        /// </summary>
        public bool Url { get; set; }
    }

    [Imported, IgnoreNamespace]
    public sealed class jQueryValidationObject : jQueryObject
    {
        private jQueryValidationObject()
        {
        }

        /// <summary>
        /// Returns the validation rules for the first selected argument.
        /// </summary>
        /// <returns>The rules which have been assigned to the specified element.</returns>
        [ScriptName("rules")]
        public jQueryValidationRules GetRules()
        {
            return null;
        }

        /// <summary>
        /// Removes the specified attributes from the first matched element
        /// </summary>
        /// <param name="attributes">The attribute names to remove</param>
        /// <returns>The attributes which have been removed</returns>
        [ScriptName("removeAttrs")]
        public string RemoveAttributes(string attributes)
        {
            return null;
        }

        /// <summary>
        /// Adds or removes the specified rules to the first matched element. Requires that the parent form has 
        /// been validated prior to the call.
        /// </summary>
        /// <param name="action">The action to perform. Should be 'add' or 'remove'</param>
        /// <param name="rules">The rules to add or remove to the specified element.</param>
        /// <returns>The current state of the rules collection for the first matched element.</returns>
        public jQueryValidationRules Rules(string action, jQueryValidationRules rules)
        {
            return null;
        }

        /// <summary>
        /// Checks whether the selected form is valid or whether all selected elements are valid.
        /// </summary>
        /// <returns>True if the form is valid, otherwise false.</returns>
        public bool Valid()
        {
            return false;
        }

        /// <summary>
        /// Validates the selected form.
        /// </summary>
        /// <returns>An instance of the validation controller for the specified form.</returns>
        public jQueryValidator Validate()
        {
            return null;
        }

        /// <summary>
        /// Validates the selected form.
        /// </summary>
        /// <param name="options">The validation options</param>
        /// <returns>An instance of the validation controller for the specified form.</returns>
        public jQueryValidator Validate(jQueryValidatorOptions options)
        {
            return null;
        }
    }

    [Imported, Serializable]
    public class jQueryValidatorOptions
    {
        public bool Debug { get; set; }
        public string ErrorClass { get; set; }
        public string ErrorContainer { get; set; }
        public string ErrorElement { get; set; }
        public string ErrorLabelContainer { get; set; }
        public bool FocusCleanup { get; set; }
        public bool FocusInvalid { get; set; }
        public JsDictionary Groups { get; set; }
        public jQueryValidationHighlight Highlight { get; set; }
        [ScriptName("ignore")]
        public string IgnoreTitle { get; set; }
        public Action<jQueryEvent, jQueryValidator> InvalidHandler { get; set; }
        public JsDictionary Messages { get; set; }
        public string Meta { get; set; }
        public JsDictionary Rules { get; set; }
        public Action<jQueryObject> SubmitHandler { get; set; }
        public jQueryValidationHighlight Unhighlight { get; set; }
        [ScriptName("onclick")]
        public bool ValidateOnClick { get; set; }
        [ScriptName("onfocusout")]
        public bool ValidateOnFocusOut { get; set; }
        [ScriptName("onkeyup")]
        public bool ValidateOnKeyUp { get; set; }
        [ScriptName("onsubmit")]
        public bool ValidateOnSubmit { get; set; }
        public string ValidClass { get; set; }
        public string Wrapper { get; set; }
    }
}