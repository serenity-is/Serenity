using Serenity.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Serenity.Services
{
    public class SaveWithLocalizationRequest<TEntity> : SaveRequest<TEntity>
    {
        public Dictionary<string, TEntity> Localizations { get; set; }
    }
}
