using System.Collections.Generic;

namespace Serenity.Services
{
    public class RetrieveResponse<T> : ServiceResponse
    {
        public T Entity { get; set; }
    }
}