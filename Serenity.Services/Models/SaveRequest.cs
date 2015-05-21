using Serenity.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Serenity.Services
{
    public interface ISaveRequest
    {
        object Entity { get; }
    }

    public class SaveRequest<TEntity> : ServiceRequest, ISaveRequest
    {
        public TEntity Entity { get; set; }

        object ISaveRequest.Entity
        {
            get { return this.Entity; }
        }
    }
}
