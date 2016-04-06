using System.Collections.Generic;

namespace Serenity.CodeGeneration
{
    public class ExternalAttribute
    {
        public ExternalAttribute()
        {
            Arguments = new List<ExternalArgument>();
        }

        public string Type { get; set; }
        public List<ExternalArgument> Arguments { get; private set; }
    }
}