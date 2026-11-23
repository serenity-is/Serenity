using Serenity.CodeGenerator;

namespace Serenity.CodeGeneration;

/// <summary>
/// Finds out whether a project makes a type available with a using alias of its own, e.g. one that its project file
/// adds with a Using item, which the code that is generated for it would duplicate with one of its own, and which a
/// compilation does not allow.
/// </summary>
public static class UsingAliasHelper
{
    /// <summary>
    /// Checks whether the project has a using alias of the specified name, which the targets of Serenity pass to sergen
    /// with its global usings.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <param name="alias">The name of the alias, e.g. "ESM".</param>
    /// <returns>True when the project has the alias.</returns>
    public static bool HasAlias(this IProjectFileInfo? project, string alias)
    {
        if (project is null || string.IsNullOrEmpty(alias))
            return false;

        try
        {
            return project.GetGlobalUsings().Values.Any(x => string.Equals(x, alias, StringComparison.Ordinal));
        }
        catch
        {
            // a project that its global usings can not be read from has none that this one knows of
            return false;
        }
    }
}
