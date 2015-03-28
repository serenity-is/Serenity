
namespace Serenity.Abstractions
{
    /// <summary>
    /// Interface for permission services that supports granting permissions temporarily
    /// </summary>
    public interface ITransientGrantor
    {
        void Grant(params string[] permissions);
        void GrantAll();
        void UndoGrant();
    }
}