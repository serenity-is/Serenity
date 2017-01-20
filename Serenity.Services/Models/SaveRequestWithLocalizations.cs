using System;
using System.Collections.Generic;

namespace Serenity.Services
{
    [Obsolete("This object is no longer required. You can use SaveRequest directly.")]
    public class SaveWithLocalizationRequest<TEntity> : SaveRequest<TEntity>
    {
    }
}
