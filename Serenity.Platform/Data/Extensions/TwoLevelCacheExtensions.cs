using Serenity.Data;
using System;

namespace Serenity
{
    public static class TwoLevelCacheExtensions
    {
        public static void ChangeGlobalGeneration(Row row)
        {
            TwoLevelCache.ChangeGlobalGeneration(row.GetFields().GenerationKey);
        }

        public static void ChangeGlobalGeneration(RowFieldsBase fields)
        {
            TwoLevelCache.ChangeGlobalGeneration(fields.GenerationKey);
        }
    }
}