namespace Serenity.ComponentModel;

/// <summary>
/// Used to assign upload options from another upload options class
/// </summary>
public interface IAssignUploadOptions
{
    /// <summary>
    /// Assigns upload options from another source
    /// </summary>
    /// <param name="source">Source options</param>
    void AssignFrom(IUploadOptions source);
}