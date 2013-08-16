using Serenity.Data;
using Newtonsoft.Json;

namespace Serenity.Services
{
    public class SaveRequest<TEntity> : ServiceRequest
    {
        public TEntity Entity { get; set; }
    }
}
