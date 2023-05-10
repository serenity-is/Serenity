namespace Serenity.Web;

static partial class WebTexts
{
    static partial class Controls
    {
        public static class FilterPanel
        {
            public static readonly LocalText AddFilter = "add criteria";
            public static readonly LocalText All = "<all>";
            public static readonly LocalText And = "and";
            public static readonly LocalText AndInParens = "(and)";
            public static readonly LocalText ChangeAndOr = "click to switch between AND/OR";
            public static readonly LocalText CurrentFilter = "Active Filter: {0}";
            public static readonly LocalText SelectField = "---select field---";
            public static readonly LocalText RemoveField = "remove criteria";
            public static readonly LocalText InvalidOperator = "invalid operator";
            public static readonly LocalText InvalidDate = "invalid date!";
            public static readonly LocalText InvalidNumber = "invalid number!";
            public static readonly LocalText Or = "or";
            public static readonly LocalText OrInParens = "(or)";
            public static readonly LocalText ResetButton = "reset";
            public static readonly LocalText SearchButton = "apply filter";
            public static readonly LocalText ValueRequired = "value required!";
            public static readonly LocalText GroupBy = "--group--";
            public static readonly LocalText ThenBy = "--then by--";
            public static readonly LocalText ApplyGroups = "apply grouping";
            public static readonly LocalText ClearGroups = "clear grouping";
            public static readonly LocalText DialogTitle = "Edit Filter";
            public static readonly LocalText EditFilter = "edit filter";
            public static readonly LocalText ResetFilterHint = "clear filter";
            public static readonly LocalText EffectiveFilter = "Active Filter:";
            public static readonly LocalText EffectiveEmpty = "none";
            public static readonly LocalText FixErrorsMessage = "Please fix errors that are marked in red";

            public static class OperatorNames
            {
                public static readonly LocalText @true = "yes";
                public static readonly LocalText @false = "no";
                public static readonly LocalText contains = "contains";
                public static readonly LocalText startswith = "starts with";
                public static readonly LocalText eq = "equal";
                public static readonly LocalText ne = "not equal";
                public static readonly LocalText gt = "greater than";
                public static readonly LocalText ge = "greater than or equal";
                public static readonly LocalText lt = "less than";
                public static readonly LocalText le = "less than or equal";
                public static readonly LocalText bw = "between";
                public static readonly LocalText @in = "in";
                public static readonly LocalText isnull = "is null";
                public static readonly LocalText isnotnull = "is not null";
            }

            public static class OperatorFormats
            {
                public static readonly LocalText @true = "{0} = yes";
                public static readonly LocalText @false = "{0} = no";
                public static readonly LocalText contains = "{0} contains '{1}'";
                public static readonly LocalText startswith = "{0} starts with '{1}'";
                public static readonly LocalText eq = "{0} = {1}";
                public static readonly LocalText ne = "{0} <> {1}";
                public static readonly LocalText gt = "{0} > {1}";
                public static readonly LocalText ge = "{0} >= {1}";
                public static readonly LocalText lt = "{0} < {1}";
                public static readonly LocalText le = "{0} <= {1}";
                public static readonly LocalText bw = "{0} is between {1} and {2}";
                public static readonly LocalText @in = "{0} is one of [{1}]";
                public static readonly LocalText isnull = "{0} is null";
                public static readonly LocalText isnotnull = "{0} is not null";
            }
        }
    }
}