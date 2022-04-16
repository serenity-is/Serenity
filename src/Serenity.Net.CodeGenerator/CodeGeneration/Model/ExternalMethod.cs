using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Serenity.CodeGeneration
{
    public class ExternalMethod : ExternalMember
    {
        public ExternalMethod()
        {
        }

        [JsonInclude]
        public List<ExternalArgument> Arguments { get; set; }
        public bool IsConstructor { get; set; }
        public bool IsOverride { get; set; }
        public bool IsGetter { get; set; }
        public bool IsSetter { get; set; }
    }
}