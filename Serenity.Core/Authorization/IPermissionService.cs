namespace Serenity.Abstractions
{
    public interface IPermissionService
    {
        bool HasPermission(string permission);
    }
}