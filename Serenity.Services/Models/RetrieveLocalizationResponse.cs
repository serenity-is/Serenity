using Serenity.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Serenity.Services
{
    public class RetrieveLocalizationResponse<TEntity> : ServiceResponse
        where TEntity : class, new()
    {
        public Dictionary<string, TEntity> Entities { get; set; }
    }
}