namespace Serenity.Web
{
    public class IDbFileNameGenerator
    {
        public string Format(object identity, string originalName, object entity);
    }
}