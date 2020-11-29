using Serenity.Abstractions;
using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Services
{
    public static class TwoLevelCacheInvalidationExtensions
    {
        public class GenerationUpdater
        {
            private readonly HashSet<string> groupKeys;
            private readonly ITwoLevelCache cache;

            public GenerationUpdater(ITwoLevelCache cache)
            {
                groupKeys = new HashSet<string>();
                this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            }

            public void Add(string key)
            {
                lock (groupKeys)
                    groupKeys.Add(key);
            }

            public void Update()
            {
                string[] keys;
                lock (groupKeys)
                    keys = groupKeys.ToArray();

                foreach (var groupKey in keys)
                    cache.ExpireGroupItems(groupKey);
            }
        }

        public static void InvalidateOnCommit(this ITwoLevelCache cache, IUnitOfWork uow, string groupKey)
        {
            if (groupKey.IsNullOrEmpty())
                throw new ArgumentNullException("generationKey");

            var updater = cache.Memory.Get("BatchGenerationUpdater:UpdaterInstance", TimeSpan.Zero,
                () => new GenerationUpdater(cache));

            updater.Add(groupKey);

            uow.OnCommit -= updater.Update;
            uow.OnCommit += updater.Update;
        }

        public static void InvalidateOnCommit(this ITwoLevelCache cache, IUnitOfWork uow, RowFieldsBase fields)
        {
            InvalidateOnCommit(cache, uow, fields.GenerationKey);
        }

        public static void InvalidateOnCommit(ITwoLevelCache cache, IUnitOfWork uow, IRow row)
        {
            InvalidateOnCommit(cache, uow, row.GetFields().GenerationKey);
        }
    }
}