using Serenity.Data;

namespace Serenity.Services
{
    /// <summary>
    /// Represents a request handler behavior that is targeted to a field.
    /// </summary>
    public interface IFieldBehavior
    {
        Field Target { get; set; }
    }
}