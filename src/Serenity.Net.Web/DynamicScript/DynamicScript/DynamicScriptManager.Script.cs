using System;

namespace Serenity.Web
{
    public partial class DynamicScriptManager
    {
        internal class Script : IDynamicScriptData
        {
            public string Hash { get; set; }
            public DateTime Time { get; set; }
            public DateTime Expiration { get; set; }
            public string ScriptText { get; set; }
            public byte[] UncompressedBytes { get; set; }
            public byte[] CompressedBytes { get; set; }
        }
    }
}