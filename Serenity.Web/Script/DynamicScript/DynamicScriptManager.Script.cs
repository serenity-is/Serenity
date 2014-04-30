using System;

namespace Serenity.Web
{
    public static partial class DynamicScriptManager
    {
        internal class Script
        {
            internal string Hash;
            internal DateTime Time;
            internal byte[] UncompressedBytes;
            internal byte[] CompressedBytes;
        }
    }
}