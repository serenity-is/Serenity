using System.Collections.Generic;

namespace Serenity.CodeGeneration
{
    public class ExternalMethod : ExternalMember
    {
        public ExternalMethod()
        {
            Arguments = new List<ExternalArgument>();
        }

        public List<ExternalArgument> Arguments { get; private set; }
        public bool IsConstructor { get; set; }
        public bool IsOverride { get; set; }
        public bool IsGetter { get; set; }
        public bool IsSetter { get; set; }
    }
}