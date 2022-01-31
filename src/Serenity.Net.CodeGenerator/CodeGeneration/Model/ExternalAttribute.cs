using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Serenity.CodeGeneration
{
    public class ExternalAttribute
    {
        public ExternalAttribute()
        {
            Arguments = new List<ExternalArgument>();
        }

        public string Type { get; set; }
        [JsonInclude]
        public List<ExternalArgument> Arguments { get; private set; }
    }
}