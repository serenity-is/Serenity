namespace Serenity.Web.EsBuild;

internal interface IEsBuildPlatformInfo
{
    string Platform { get; }
    string Architecture { get; }
}