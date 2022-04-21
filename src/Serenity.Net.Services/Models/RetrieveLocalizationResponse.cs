namespace Serenity.Services
{
    public class RetrieveLocalizationResponse<TEntity> : ServiceResponse
        where TEntity : class, new()
    {
        public Dictionary<string, TEntity> Entities { get; set; }
    }
}