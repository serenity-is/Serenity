using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Options for the Select2
    /// </summary>
    [Imported, Serializable]
    public class Select2Options
    {
        /// <summary>
        /// Controls the width style attribute of the Select2 container div. The following values are supported:
        /// off
        ///   No width attribute will be set. Keep in mind that the container div copies classes from the source element so setting the width attribute may not always be necessary.
        /// element
        ///   Uses javascript to calculate the width of the source element.
        /// copy
        ///   Copies the value of the width style attribute set on the source element.
        /// resolve
        ///   First attempts to copy than falls back on element.
        /// other values
        ///   if the width attribute contains a function it will be avaluated, otherwise the value is used verbatim.
        /// </summary>
        public TypeOption<string, Func<string>> Width { get; set; }
        /// <summary>
        /// Number of characters necessary to start a search.
        /// </summary>
        public int MinimumInputLength { get; set; }
        /// <summary>
        /// Maximum number of characters that can be entered for an input.
        /// </summary>
        public int MaximumInputLength { get; set; }
        /// <summary>
        /// The minimum number of results that must be initially (after opening the dropdown for the first time) populated in order to keep the search field. 
        /// This is useful for cases where local data is used with just a few results, in which case the search box is not very useful and wastes screen space.
        /// The option can be set to a negative value to permanently hide the search field.
        /// </summary>
        public int MinimumResultsForSearch { get; set; }
        /// <summary>
        /// The maximum number of items that can be selected in a multi-select control. If this number is less than 1 selection is not limited.
        /// Once the number of selected items reaches the maximum specified the contents of the dropdown will be populated by the formatSelectionTooBig function.
        /// </summary>
        public TypeOption<int, Func<int>> MaximumSelectionSize { get; set; }
        /// <summary>
        /// Initial value that is selected if no other selection is made.
        /// 
        /// The placeholder can also be specified as a data-placeholder attribute on the select or input element that Select2 is attached to.
        /// 
        /// Note that because browsers assume the first option element is selected in non-multi-value select boxes an empty first option element must be provided (<option></option>) for the placeholder to work.
        /// </summary>
        public string PlaceHolder { get; set; }
        /// <summary>
        /// When attached to a select resolves the option that should be used as the placeholder. Can either be a function which given the select element should return the option element or a string first to indicate that the first option should be used.
        /// 
        /// This option is useful when Select2's default of using the first option only if it has no value and no text is not suitable.
        /// </summary>
        public TypeOption<string, Func<string>> PlaceHolderOption { get; set; }
        /// <summary>
        /// Separator character or string used to delimit ids in value attribute of the multi-valued selects. The default delimiter is the , character.
        /// </summary>
        public string Separator { get; set; }
        /// <summary>
        /// Whether or not a clear button is displayed when the select box has a selection. The button, when clicked, resets the value of the select box back to the placeholder, thus this option is only available when the placeholder is specified.
        /// 
        /// This option only works when the placeholder is specified.
        /// 
        /// When attached to a select an option with an empty value must be provided. This is the option that will be selected when the button is pressed since a select box requires at least one selection option.
        /// 
        /// Also, note that this option only works with non-multi-value based selects because multi-value selects always provide such a button for every selected option.
        /// </summary>
        public bool AllowClear { get; set; }
        /// Whether or not Select2 allows selection of multiple values.
        /// 
        /// When Select2 is attached to a select element this value will be ignored and select's multiple attribute will be used instead.
        public bool Multiple { get; set; }
        /// <summary>
        /// If set to false the dropdown is not closed after a selection is made, allowing for rapid selection of multiple items. By default this option is set to true.
        /// 
        /// Only applies when configured in multi-select mode.
        /// </summary>
        public bool CloseOnSelect { get; set; }
        /// <summary>
        /// If set to true the dropdown is opened when the user presses the enter key and Select2 is closed. By default this option is enabled.
        /// </summary>
        public bool OpenOnEnter { get; set; }
        /// <summary>
        /// Function used to get the id from the choice object or a string representing the key under which the id is stored.
        /// id(object)
        /// Parameter   Type    Description
        /// object      object  A choice object.
        /// returns     string  the id of the object.
        /// The default implementation expects the object to have a id property that is returned.
        /// </summary>
        public Func<dynamic, string> Id { get; set; }
        public Func<string, string, jQueryObject, Boolean> Matcher { get; set; }
        public Func<dynamic, jQueryObject, dynamic, dynamic> SortResults { get; set; }
        public Func<dynamic, jQueryObject, Func<string, string>, string> FormatSelection { get; set; }
        public Func<dynamic, jQueryObject, dynamic, Func<string, string>, string> FormatResult { get; set; }
        public Func<dynamic, string> FormatResultCssClass { get; set; }
        public Func<string, string> FormatNoMatches { get; set; }
        public Func<string> FormatSearching { get; set; }
        public Func<string, int, string> FormatInputTooShort { get; set; }
        public Func<string, string> FormatSelectionTooBig { get; set; }
        public Func<string, object> CreateSearchChoice { get; set; }
        public Func<jQueryObject, Func<object>> InitSelection { get; set; }
        public Func<string, dynamic[], Func<object, object>, dynamic, string> Tokenizer { get; set; }
        public string[] TokenSeparators { get; set; }
        public Action<Select2QueryOptions> Query { get; set; }
        public Select2AjaxOptions Ajax { get; set; }
        public TypeOption<object, Func<object>> Data { get; set; }
        public TypeOption<string[], Func<string[]>> Tags { get; set; }
        public TypeOption<object, Func<object>> ContainerCss { get; set; }
        public TypeOption<string, Func<string>> ContainerCssClass { get; set; }
        public TypeOption<object, Func<object>> DropdownCss { get; set; }
        public TypeOption<string, Func<string>> DropdownCssClass { get; set; }
        public bool DropdownAutoWidth { get; set; }
        public Func<string, string> AdaptContainerCssClass { get; set; }
        public Func<string, string> AdaptDropdownCssClass { get; set; }
        public Func<string, string> EscapeMarkup { get; set; }
        public Boolean SelectOnBlur { get; set; }
        public Int32 LoadMorePadding { get; set; }
        public Func<object, string, string> NextSearchTerm { get; set; }
    }

    [Imported, Serializable]
    public class Select2QueryOptions
    {
        public jQueryObject Element { get; set; }
        public string Term { get; set; }
        public Int32 Page { get; set; }
        public object Context { get; set; }
        public Action<Select2Result> Callback { get; set; }
    }

    [Imported, Serializable]
    public class Select2Result
    {
        public object Results { get; set; }
        public bool More { get; set; }
        public object Context { get; set; }
    }

    [Imported, Serializable]
    public class Select2AjaxOptions
    {
        public object Transport { get; set; }
        public TypeOption<string, Func<string>> Url { get; set; }
        public string DataType { get; set; }
        public int QuietMillis { get; set; }
        public bool Cache { get; set; }
        public TypeOption<string, Func<string>> JsonpCallback { get; set; }
        public Func<string, int, dynamic, object> Data { get; set; }
        public Func<dynamic, int, dynamic, object> Results { get; set; }
        public TypeOption<object, Func<object>> Params { get; set; }
    }

    [Imported]
    public static class Select2Extensions
    {
        [InstanceMethodOnFirstArgument]
        public static jQueryObject Select2(this jQueryObject element)
        {
            return null;
        }

        [InstanceMethodOnFirstArgument]
        public static jQueryObject Select2(this jQueryObject element, Select2Options options)
        {
            return null;
        }

        [InstanceMethodOnFirstArgument]
        public static jQueryObject Select2(this jQueryObject element, string action)
        {
            return null;
        }

        [InstanceMethodOnFirstArgument]
        public static jQueryObject Select2(this jQueryObject element, string option, object value)
        {
            return null;
        }

        [InstanceMethodOnFirstArgument, ScriptName("select2")]
        public static object Select2Get(this jQueryObject element, string option)
        {
            return null;
        }
    }
}