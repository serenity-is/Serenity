using System;

namespace Serenity.Web
{
    public partial class DynamicScriptManager
    {
        internal class Script
        {
            internal string Hash;
            internal DateTime Time;
            internal DateTime Expiration;
            internal string ScriptText;
            internal byte[] UncompressedBytes;
            internal byte[] CompressedBytes;
        }
    }
}