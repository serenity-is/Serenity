namespace Serenity.CodeGenerator;

/// <summary>
/// What <see cref="GeneratedFileWriter"/> does when a target file exists and its
/// content differs from the generated one.
/// </summary>
public enum OverwriteMode
{
    /// <summary>
    /// Ask the user (the interactive default). "Skip all" and "Yes to all" answers
    /// apply to the remaining files of the run.
    /// </summary>
    Ask,

    /// <summary>
    /// Replace the file, keeping a "*.bak" copy of the previous content.
    /// This is "--overwrite:yes".
    /// </summary>
    Backup,

    /// <summary>
    /// Replace the file without keeping a copy. This is "--overwrite:force".
    /// </summary>
    Force,

    /// <summary>
    /// Leave the file alone and continue, like answering "Skip all" interactively.
    /// This is "--overwrite:skip".
    /// </summary>
    Skip,

    /// <summary>
    /// Leave the file alone and fail, so a script never loses hand edits.
    /// This is "--overwrite:no", and the default for "--no-interactive".
    /// </summary>
    Fail
}
