namespace Serenity.Web.EsBuild;

internal class EsBuildPlatformInfo : IEsBuildPlatformInfo
{
    public string Platform => Environment.OSVersion.Platform switch
    {
        PlatformID.Win32NT => "win32",
        PlatformID.Unix => "linux",
        PlatformID.MacOSX => "darwin",
        PlatformID.Win32S => "win32",
        PlatformID.Win32Windows => "win32",
        PlatformID.WinCE => "win32",
        _ => throw new Exception("Unsupported platform.")
    };

    public string Architecture => 
        System.Runtime.InteropServices.RuntimeInformation.OSArchitecture == System.Runtime.InteropServices.Architecture.Arm64 ? 
        "arm64" : Environment.Is64BitOperatingSystem ? "x64" : "x86";
}