namespace Serenity
{
    public class UrlFormatter : ISlickFormatter
    {
        public string Format(SlickFormatterContext ctx)
        {
            return "<a href='" + Q.HtmlEncode(ctx.Value) + "'>" +
                Q.HtmlEncode(ctx.Value) +
                "</a>";
        }
    }
}