namespace Serenity
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [Imported, Serializable, PreserveMemberCase]
    public class ListResponse<TEntity> : ServiceResponse
    {
        [PreserveCase]
        public List<TEntity> Entities;
        [PreserveCase]
        public int TotalCount;
        [PreserveCase]
        public int Skip;
        [PreserveCase]
        public int Take;
    }
}