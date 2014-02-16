using Serenity.Data;
using Newtonsoft.Json;
using System;

namespace Serenity.Services
{
    public class RetrieveLocalizationRequest : RetrieveRequest
    {
        public Int32? CultureId { get; set; }
    }
}