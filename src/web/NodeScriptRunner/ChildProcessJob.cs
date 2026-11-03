using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Serenity.Web;

/// <summary>
/// A Windows job object that kills every process assigned to it, and their descendants, when
/// its handle is closed. Windows closes the handle when the owning process dies, so this also
/// covers a hard termination, e.g. a debugger stopping the application, where the application
/// stopping token never fires.
/// </summary>
internal sealed class ChildProcessJob : IDisposable
{
    private const int JobObjectExtendedLimitInformationClass = 9;
    private const uint JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x2000;

    private static readonly Lazy<ChildProcessJob?> shared = new(Create);

    private readonly SafeFileHandle handle;

    private ChildProcessJob(SafeFileHandle handle)
    {
        this.handle = handle;
    }

    /// <summary>
    /// Gets the job shared by the application, or null if job objects are not available.
    /// It is deliberately never disposed, and rooted by this static field as closing the
    /// handle, including by a finalizer, would kill the assigned processes.
    /// </summary>
    public static ChildProcessJob? Shared => shared.Value;

    /// <summary>
    /// Creates a new job.
    /// </summary>
    /// <returns>The job, or null if not running on Windows or the job could not be created.</returns>
    public static ChildProcessJob? Create()
    {
        if (!OperatingSystem.IsWindows())
            return null;

        var handle = CreateJobObjectW(IntPtr.Zero, null);
        if (handle.IsInvalid)
        {
            handle.Dispose();
            return null;
        }

        var info = new JobObjectExtendedLimitInformation
        {
            LimitFlags = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE
        };

        if (!SetInformationJobObject(handle, JobObjectExtendedLimitInformationClass, ref info,
            (uint)Marshal.SizeOf<JobObjectExtendedLimitInformation>()))
        {
            handle.Dispose();
            return null;
        }

        return new ChildProcessJob(handle);
    }

    /// <summary>
    /// Assigns a process to the job. Processes it starts afterwards join the job automatically.
    /// </summary>
    /// <param name="process">The process.</param>
    /// <returns>True if the process was assigned to the job.</returns>
    public bool TryAssign(Process process)
    {
        ArgumentNullException.ThrowIfNull(process);
        return AssignProcessToJobObject(handle, process.SafeHandle);
    }

    /// <summary>
    /// Closes the job handle, which kills the assigned processes.
    /// </summary>
    public void Dispose() => handle.Dispose();

    [DllImport("kernel32", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern SafeFileHandle CreateJobObjectW(IntPtr jobAttributes, string? name);

    [DllImport("kernel32", ExactSpelling = true, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetInformationJobObject(SafeFileHandle job, int infoClass,
        ref JobObjectExtendedLimitInformation info, uint infoLength);

    [DllImport("kernel32", ExactSpelling = true, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AssignProcessToJobObject(SafeFileHandle job, SafeProcessHandle process);
}
