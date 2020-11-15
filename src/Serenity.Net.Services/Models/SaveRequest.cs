
using System.Collections;
using System.Collections.Generic;

namespace Serenity.Services
{
    public interface ISaveRequest
    {
        object EntityId { get; set; }
        object Entity { get; set; }
        IDictionary Localizations { get; set; }
    }

    public class SaveRequest<TEntity> : ServiceRequest, ISaveRequest
    {
        public object EntityId { get; set; }
        public TEntity Entity { get; set; }

        public Dictionary<string, TEntity> Localizations { get; set; }

        object ISaveRequest.Entity
        {
            get { return this.Entity; }
            set { this.Entity = (TEntity)value; }
        }

        IDictionary ISaveRequest.Localizations
        {
            get { return this.Localizations; }
            set { this.Localizations = (Dictionary<string, TEntity>) value; }
        }
    }
}
