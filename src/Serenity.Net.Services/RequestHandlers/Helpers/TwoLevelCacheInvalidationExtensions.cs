using Serenity.Abstractions;
using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        private static void ProcessTwoLevelCachedAttribute(ITwoLevelCache cache, IUnitOfWork uow, Type type)
        {
            if (type == null)
                return;

            var attr = type.GetCustomAttribute<TwoLevelCachedAttribute>(true);
            if (attr == null)
                return;

            if (attr.GenerationKeys != null)
            {
                foreach (var key in attr.GenerationKeys)
                {
                    InvalidateOnCommit(cache, uow, key);
                }
            }

            if (attr.LinkedRows != null)
            {
                foreach (var rowType in attr.LinkedRows)
                {
                    var rowInstance = (IRow)Activator.CreateInstance(rowType);
                    InvalidateOnCommit(cache, uow, rowInstance.GetFields().GenerationKey);
                }
            }
        }

        public static void InvalidateOnCommit(this ITwoLevelCache cache, IUnitOfWork uow, RowFieldsBase fields)
        {
            if (fields is null)
                throw new ArgumentNullException(nameof(fields));

            InvalidateOnCommit(cache, uow, fields.GenerationKey);

            var fieldsType = fields.GetType();
            if (fieldsType.IsNested && fieldsType.DeclaringType != null)
                ProcessTwoLevelCachedAttribute(cache, uow, fieldsType.DeclaringType);
        }

        public static void InvalidateOnCommit(this ITwoLevelCache cache, IUnitOfWork uow, IRow row)
        {
            if (row is null)
                throw new ArgumentNullException(nameof(row));

            InvalidateOnCommit(cache, uow, row.GetFields().GenerationKey);
            ProcessTwoLevelCachedAttribute(cache, uow, row.GetType());
        }
    }
}