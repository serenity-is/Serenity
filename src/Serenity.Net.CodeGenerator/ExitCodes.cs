namespace Serenity.CodeGenerator;

public enum ExitCodes
{
    NoCommand = -2,
    Help = -1,
    Success = 0,
    Exception = 1,
    InvalidCommand = 2,
    ProjectNotFound = 3,
    NoProjectFiles = 4,
    MultipleProjectFiles = 5,
    CantDeterminePackagesDir = 6,
    NoConnectionString = 7
}