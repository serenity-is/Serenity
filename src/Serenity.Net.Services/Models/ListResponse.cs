using System.Collections;
using System.Collections.Generic;

namespace Serenity.Services
{
    public class ListResponse<T> : ServiceResponse, IListResponse
    {
        IList IListResponse.Entities => Entities;

        public List<T> Entities { get; set; }
        public List<object> Values { get; set; }
        public int TotalCount { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}