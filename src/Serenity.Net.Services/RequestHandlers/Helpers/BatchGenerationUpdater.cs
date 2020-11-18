using Serenity.Abstractions;
using Serenity.Data;
using System;
using System.Collections;

namespace Serenity.Services
{
    public static class BatchGenerationUpdater
    {
        public class GenerationUpdater
        {
            private readonly string groupKey;
            private readonly ITwoLevelCache cache;

            public GenerationUpdater(ITwoLevelCache cache, string groupKey)
            {
                this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
                this.groupKey = groupKey;
            }

            public void Update()
            {
                cache.ExpireGroupItems(this.groupKey);
            }
        }

        private static readonly Hashtable byGroupKey;

        static BatchGenerationUpdater()
        {
            byGroupKey = new Hashtable();
        }

        public static GenerationUpdater GetUpdater(ITwoLevelCache cache, string groupKey)
        {
            if (groupKey.IsNullOrEmpty())
                throw new ArgumentNullException("generationKey");

            if (!(byGroupKey[groupKey] is GenerationUpdater updater))
            {
                var locked = Hashtable.Synchronized(byGroupKey);
                updater = new GenerationUpdater(cache, groupKey);
                locked[groupKey] = locked[groupKey] ?? updater;
            }

            return updater;
        }

        public static void OnCommit(IUnitOfWork uow, ITwoLevelCache cache, string generationKey)
        {
            var updater = GetUpdater(cache, generationKey);
            uow.OnCommit -= updater.Update;
            uow.OnCommit += updater.Update;
        }
    }
}