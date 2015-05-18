using Serenity.Data;
using Newtonsoft.Json;
using System;

namespace Serenity.Services
{
    public class RetrieveLocalizationRequest : RetrieveRequest
    {
        public object CultureId { get; set; }
    }
}