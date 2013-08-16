using Serenity.Data;
using System;
using System.Collections;

namespace Serenity.Services
{
    public static class BatchGenerationUpdater
    {
        public class GenerationUpdater
        {
            private string generationKey;

            public GenerationUpdater(string generationKey)
            {
                this.generationKey = generationKey;
            }

            public void Update()
            {
                TwoLevelCache.ChangeGlobalGeneration(this.generationKey);
            }
        }

        private static Hashtable byGenerationKey;

        static BatchGenerationUpdater()
        {
            byGenerationKey = new Hashtable();
        }

        public static GenerationUpdater GetUpdater(string generationKey)
        {
            if (generationKey.IsEmptyOrNull())
                throw new ArgumentNullException("generationKey");

            var updater = byGenerationKey[generationKey] as GenerationUpdater;

            if (updater == null)
            {
                var locked = Hashtable.Synchronized(byGenerationKey);
                updater = new GenerationUpdater(generationKey);
                byGenerationKey[generationKey] = byGenerationKey[generationKey] ?? updater;
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