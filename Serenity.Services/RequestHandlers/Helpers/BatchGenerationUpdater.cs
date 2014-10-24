using Serenity.Data;
using System;
using System.Collections;

namespace Serenity.Services
{
    public static class BatchGenerationUpdater
    {
        public class GenerationUpdater
        {
            private string groupKey;

            public GenerationUpdater(string groupKey)
            {
                this.groupKey = groupKey;
            }

            public void Update()
            {
                TwoLevelCache.ExpireGroupItems(this.groupKey);
            }
        }

        private static Hashtable byGroupKey;

        static BatchGenerationUpdater()
        {
            byGroupKey = new Hashtable();
        }

        public static GenerationUpdater GetUpdater(string groupKey)
        {
            if (groupKey.IsNullOrEmpty())
                throw new ArgumentNullException("generationKey");

            var updater = byGroupKey[groupKey] as GenerationUpdater;

            if (updater == null)
            {
                var locked = Hashtable.Synchronized(byGroupKey);
                updater = new GenerationUpdater(groupKey);
                byGroupKey[groupKey] = byGroupKey[groupKey] ?? updater;
            }

            return updater;
        }

        public static void OnCommit(IUnitOfWork uow, string generationKey)
        {
            var updater = GetUpdater(generationKey);
            uow.OnCommit -= updater.Update;
            uow.OnCommit += updater.Update;
        }
    }
}