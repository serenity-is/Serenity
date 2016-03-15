using System.Runtime.CompilerServices;

namespace Serenity
{
    public static partial class Q
    {
        [Imported]
        public static class ErrorHandling
        {
            [InlineCode("Q.showServiceError({error})")]
            public static void ShowServiceError(ServiceError error)
            {
            }
        }
    }
}