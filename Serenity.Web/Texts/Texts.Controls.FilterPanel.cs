namespace Serenity.Web
{
    internal static partial class Texts
    {
        public static class Controls
        {
            public static class FilterPanel
            {
                public static LocalText AddFilter = "filtre koşulu ekle";
                public static LocalText All = "<tümü>";
                public static LocalText And = "ve";
                public static LocalText AndInParens = "(ve)";
                public static LocalText ChangeAndOr = "VE/VEYA arasında geçiş yapmak için tıklayın";
                public static LocalText CurrentFilter = "Etkin Filtre: {0}";
                public static LocalText SelectField = "---alan seçiniz---";
                public static LocalText RemoveField = "bu koşulu sil";
                public static LocalText InvalidOperator = "hatalı operatör";
                public static LocalText InvalidDate = "geçersiz tarih!";
                public static LocalText InvalidNumber = "geçersiz sayı!";
                public static LocalText Or = "veya";
                public static LocalText OrInParens = "(veya)";
                public static LocalText ResetButton = "filtreyi temizle";
                public static LocalText SearchButton = "filtreyi uygula";
                public static LocalText ValueRequired = "alan için değer girilmeli!";
                public static LocalText GroupBy = "--grupla--";
                public static LocalText ThenBy = "--ve grupla--";
                public static LocalText ApplyGroups = "gruplamayı uygula";
                public static LocalText ClearGroups = "gruplamayı temizle";

                public static class OperatorNames
                {
                    public static LocalText @true = "evet";
                    public static LocalText @false = "hayır";
                    public static LocalText contains = "içerir";
                    public static LocalText startswith = "başlar";
                    public static LocalText eq = "eşit";
                    public static LocalText ne = "farklı";
                    public static LocalText gt = "büyük";
                    public static LocalText ge = "büyük eşit";
                    public static LocalText lt = "küçük";
                    public static LocalText le = "küçük eşit";
                    public static LocalText bw = "arasında";
                    public static LocalText @in = "içinden biri";
                    public static LocalText isnull = "boş";
                    public static LocalText isnotnull = "boş değil";
                }

                public static class OperatorFormats
                {
                    public static LocalText @true = "{0} = evet";
                    public static LocalText @false = "{0} = hayır";
                    public static LocalText contains = "{0}, '{1}' içeren";
                    public static LocalText startswith = "{0}, '{1}' ile başlayan";
                    public static LocalText eq = "{0} = {1}";
                    public static LocalText ne = "{0} <> {1}";
                    public static LocalText gt = "{0} > {1}";
                    public static LocalText ge = "{0} >= {1}";
                    public static LocalText lt = "{0} < {1}";
                    public static LocalText le = "{0} <= {1}";
                    public static LocalText bw = "{0} {1} ile {2} arasında";
                    public static LocalText @in = "{0}, [{1}] içinden biri";
                    public static LocalText isnull = "{0} boş";
                    public static LocalText isnotnull = "{0} boş değil";
                }
            }
        }
    }
}