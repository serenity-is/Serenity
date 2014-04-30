using System;

namespace Serenity.Web
{
    public interface ITwoLevelCached
    {
        string GlobalGenerationKey { get; }
        TimeSpan LocalExpiration { get; }
        TimeSpan RemoteExpiration { get; }
    }
}