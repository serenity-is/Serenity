using Serenity.Data;
using System;
using System.Collections;
using System.Reflection;

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

        public static void OnCommit(IUnitOfWork uow, Row row)
        {
            var generationKey = row.GetFields().GenerationKey;
            OnCommit(uow, generationKey);

            var attr = row.GetType().GetCustomAttribute<TwoLevelCachedAttribute>(false);
            if (attr?.GenerationKeys?.Length > 0)
            {
                foreach (var key in attr.GenerationKeys)
                {
                    OnCommit(uow, key);
                }
            }

            if (attr?.LinkedRows?.Length > 0)
            {
                foreach (var rowType in attr.LinkedRows)
                {
                    var rowInstance = (Row)Activator.CreateInstance(rowType);
                    OnCommit(uow, rowInstance.GetFields().GenerationKey);
                }
            }
        }
    }
}