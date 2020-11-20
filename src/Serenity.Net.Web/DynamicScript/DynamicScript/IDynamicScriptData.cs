using System;

namespace Serenity.Web
{
    public interface IDynamicScriptData
    {
        byte[] CompressedBytes { get; }
        byte[] UncompressedBytes { get; }
        DateTime Time { get; set; }
    }
}