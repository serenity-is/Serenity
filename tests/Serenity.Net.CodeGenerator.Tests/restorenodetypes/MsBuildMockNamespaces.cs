namespace Microsoft.Build.Utilities
{
    public class Task()
    {
        public virtual bool Execute() { return true; }
        public static class Log
        {
            public static void LogWarning(string text) { }
        }
    }
}