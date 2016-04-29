
namespace Serenity
{
    public class UrlFormatter : ISlickFormatter
    {
        public string Format(SlickFormatterContext ctx)
        {
            var url = !string.IsNullOrEmpty(UrlProperty) ?
                ((object)(ctx.Item[UrlProperty] ?? "")).ToString() : 
                ((object)(ctx.Value ?? "")).ToString();

            if (string.IsNullOrEmpty(url))
                return "";

            if (!string.IsNullOrEmpty(UrlFormat))
                url = string.Format(UrlFormat, url);

            if (url != null && url.StartsWith("~/"))
                url = Q.ResolveUrl(url);

            var display = !string.IsNullOrEmpty(DisplayProperty) ?
                ((object)ctx.Item[DisplayProperty] ?? "").ToString() :
                ((object)(ctx.Value ?? "")).ToString();

            if (!string.IsNullOrEmpty(DisplayFormat))
                display = string.Format(DisplayFormat, display);

            var s = "<a href='" + Q.HtmlEncode(url) + "'";

            if (!string.IsNullOrEmpty(Target))
                s += " target='" + Target + "'";
            
            s += ">" + Q.HtmlEncode(display) + "</a>";

            return s;
        }

        [Option]
        public string DisplayProperty { get; set; }
        [Option]
        public string DisplayFormat { get; set; }

        [Option]
        public string UrlProperty { get; set; }
        [Option]
        public string UrlFormat { get; set; }

        [Option]
        public string Target { get; set; }
    }
}
