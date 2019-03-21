using System.Collections.Generic;
using System;
using System.Collections;

namespace Serenity.Services
{
    public class ListResponse<T> : ServiceResponse, IListResponse
    {
        IList IListResponse.Entities { get { return (IList)this.Entities; } }

        public List<T> Entities { get; set; }
        public List<object> Values { get; set; }
        public int TotalCount { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}