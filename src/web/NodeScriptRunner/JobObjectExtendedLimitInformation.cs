using System.Runtime.InteropServices;

namespace Serenity.Web;

/// <summary>
/// Flattened layout of the Win32 <c>JOBOBJECT_EXTENDED_LIMIT_INFORMATION</c> structure,
/// which embeds <c>JOBOBJECT_BASIC_LIMIT_INFORMATION</c> and <c>IO_COUNTERS</c>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct JobObjectExtendedLimitInformation
{
    public long PerProcessUserTimeLimit;
    public long PerJobUserTimeLimit;
    public uint LimitFlags;
    public nuint MinimumWorkingSetSize;
    public nuint MaximumWorkingSetSize;
    public uint ActiveProcessLimit;
    public nuint Affinity;
    public uint PriorityClass;
    public uint SchedulingClass;
    public ulong ReadOperationCount;
    public ulong WriteOperationCount;
    public ulong OtherOperationCount;
    public ulong ReadTransferCount;
    public ulong WriteTransferCount;
    public ulong OtherTransferCount;
    public nuint ProcessMemoryLimit;
    public nuint JobMemoryLimit;
    public nuint PeakProcessMemoryUsed;
    public nuint PeakJobMemoryUsed;
}
