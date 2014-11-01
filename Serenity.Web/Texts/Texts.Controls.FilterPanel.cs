namespace Serenity.Web
{
    internal static partial class Texts
    {
        public static partial class Controls
        {
            public static class FilterPanel
            {
                public static LocalText AddFilter = "add criteria";
                public static LocalText All = "<all>";
                public static LocalText And = "and";
                public static LocalText AndInParens = "(and)";
                public static LocalText ChangeAndOr = "click to switch between AND/OR";
                public static LocalText CurrentFilter = "Active Filter: {0}";
                public static LocalText SelectField = "---select field---";
                public static LocalText RemoveField = "remove criteria";
                public static LocalText InvalidOperator = "invalid operator";
                public static LocalText InvalidDate = "invalid date!";
                public static LocalText InvalidNumber = "invalid number!";
                public static LocalText Or = "or";
                public static LocalText OrInParens = "(or)";
                public static LocalText ResetButton = "clear filter";
                public static LocalText SearchButton = "apply filter";
                public static LocalText ValueRequired = "value required!";
                public static LocalText GroupBy = "--group--";
                public static LocalText ThenBy = "--then by--";
                public static LocalText ApplyGroups = "apply grouping";
                public static LocalText ClearGroups = "clear grouping";

                public static class OperatorNames
                {
                    public static LocalText @true = "yes";
                    public static LocalText @false = "no";
                    public static LocalText contains = "contains";
                    public static LocalText startswith = "start with";
                    public static LocalText eq = "equal";
                    public static LocalText ne = "not equal";
                    public static LocalText gt = "greater than";
                    public static LocalText ge = "greater than or equal";
                    public static LocalText lt = "less than";
                    public static LocalText le = "less than or equal";
                    public static LocalText bw = "between";
                    public static LocalText @in = "in";
                    public static LocalText isnull = "is null";
                    public static LocalText isnotnull = "is not null";
                }

                public static class OperatorFormats
                {
                    public static LocalText @true = "{0} = yes";
                    public static LocalText @false = "{0} = no";
                    public static LocalText contains = "{0} contains '{1}'";
                    public static LocalText startswith = "{0} starts with '{1}'";
                    public static LocalText eq = "{0} = {1}";
                    public static LocalText ne = "{0} <> {1}";
                    public static LocalText gt = "{0} > {1}";
                    public static LocalText ge = "{0} >= {1}";
                    public static LocalText lt = "{0} < {1}";
                    public static LocalText le = "{0} <= {1}";
                    public static LocalText bw = "{0} is between {1} and {2}";
                    public static LocalText @in = "{0} is one of [{1}]";
                    public static LocalText isnull = "{0} is null";
                    public static LocalText isnotnull = "{0} is not null";
                }
            }
        }
    }
}