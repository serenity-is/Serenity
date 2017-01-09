
namespace Serenity.Services
{
    public interface ISaveRequest
    {
        object EntityId { get; set; }
        object Entity { get; set; }
    }

    public class SaveRequest<TEntity> : ServiceRequest, ISaveRequest
    {
        public object EntityId { get; set; }
        public TEntity Entity { get; set; }

        object ISaveRequest.Entity
        {
            get { return this.Entity; }
            set { this.Entity = (TEntity)value; }
        }
    }
}
