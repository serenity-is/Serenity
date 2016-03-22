using Serenity.ComponentModel;

namespace Serenity
{
    public class UrlFormatter : ISlickFormatter
    {
        public string Format(SlickFormatterContext ctx)
        {
            var display = !string.IsNullOrEmpty(DisplayProperty) ?
                ctx.Item[DisplayProperty] as string : ctx.Value;
            var url = !string.IsNullOrEmpty(UrlProperty) ?
                ctx.Item[UrlProperty] as string : ctx.Value;
            return "<a href='" + Q.HtmlEncode(url) + "'>" +
                Q.HtmlEncode(display) +
                "</a>";
        }

        [Option]
        public string DisplayProperty { get; set; }
        [Option]
        public string UrlProperty { get; set; }
    }
}
