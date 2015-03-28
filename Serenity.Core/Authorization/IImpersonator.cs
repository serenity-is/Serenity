
namespace Serenity.Abstractions
{
    /// <summary>
    /// Interface for authorization services that supports temporary impersonating
    /// </summary>
    public interface IImpersonator
    {
        void Impersonate(string username);
        void UndoImpersonate();
    }
}