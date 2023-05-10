namespace Serenity.Services;

/// <summary>
/// Contains helper methods for two level cache invalidation
/// </summary>
public static class TwoLevelCacheInvalidationExtensions
{
    private class GenerationUpdater
    {
        private readonly string groupKey;
        private readonly ITwoLevelCache cache;

        public GenerationUpdater(ITwoLevelCache cache, string groupKey)
        {
            this.groupKey = groupKey ?? throw new ArgumentNullException(nameof(groupKey));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public void Update()
        {
            cache.ExpireGroupItems(groupKey);
        }
    }

    /// <summary>
    /// Invalidates cached items related to a group key when the 
    /// unit of work commits
    /// </summary>
    /// <param name="cache">Cache</param>
    /// <param name="uow">Unit of work</param>
    /// <param name="groupKey">Group key</param>
    /// <exception cref="ArgumentNullException">cache is null</exception>
    public static void InvalidateOnCommit(this ITwoLevelCache cache, IUnitOfWork uow, string groupKey)
    {
        if (cache is null)
            throw new ArgumentNullException(nameof(cache));

        if (groupKey.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(groupKey));

        var updater = cache.Memory.Get("BatchGenerationUpdater:UpdaterInstance:" + groupKey, TimeSpan.Zero,
            () => new GenerationUpdater(cache, groupKey));

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

    /// <summary>
    /// Invalidates cached items related to fields class group key and
    /// any related fields types specified using TwoLevelCached attributes
    /// on the row type.
    /// </summary>
    /// <param name="cache">Cache</param>
    /// <param name="uow">Unit of work</param>
    /// <param name="fields">Fields type</param>
    /// <exception cref="ArgumentNullException">Cache is null</exception>
    public static void InvalidateOnCommit(this ITwoLevelCache cache, IUnitOfWork uow, RowFieldsBase fields)
    {
        if (fields is null)
            throw new ArgumentNullException(nameof(fields));

        InvalidateOnCommit(cache, uow, fields.GenerationKey);

        var fieldsType = fields.GetType();
        if (fieldsType.IsNested && fieldsType.DeclaringType != null)
            ProcessTwoLevelCachedAttribute(cache, uow, fieldsType.DeclaringType);
    }

    /// <summary>
    /// Invalidates cached items on commit for specified row type
    /// and any related field types specified using TwoLevelCached attributes
    /// on the row type.
    /// </summary>
    /// <param name="cache">Cache</param>
    /// <param name="uow">Unit of work</param>
    /// <param name="row">Row type</param>
    /// <exception cref="ArgumentNullException">Cache is null</exception>
    public static void InvalidateOnCommit(this ITwoLevelCache cache, IUnitOfWork uow, IRow row)
    {
        if (row is null)
            throw new ArgumentNullException(nameof(row));

        InvalidateOnCommit(cache, uow, row.GetFields().GenerationKey);
        ProcessTwoLevelCachedAttribute(cache, uow, row.GetType());
    }
}