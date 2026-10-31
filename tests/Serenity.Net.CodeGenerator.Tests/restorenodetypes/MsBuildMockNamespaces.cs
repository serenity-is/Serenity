namespace Microsoft.Build.Framework
{
    public class RequiredAttribute : Attribute { }

    public class OutputAttribute : Attribute { }

    public enum MessageImportance { High, Normal, Low }

    public interface ITaskItem
    {
        string ItemSpec { get; }
        string GetMetadata(string metadataName);
        void SetMetadata(string metadataName, string metadataValue);
    }
}

namespace Microsoft.Build.Utilities
{
    public class Task()
    {
        public virtual bool Execute() { return true; }
        public static class Log
        {
            public static bool HasLoggedErrors => false;
            public static void LogWarning(string text) { }
            public static void LogWarning(string format, params object[] args) { }
            public static void LogMessage(Microsoft.Build.Framework.MessageImportance importance, string format, params object[] args) { }
        }
    }

    public class TaskItem(string itemSpec) : Microsoft.Build.Framework.ITaskItem
    {
        private readonly Dictionary<string, string> metadata = new(StringComparer.OrdinalIgnoreCase);

        public string ItemSpec => itemSpec;

        public string GetMetadata(string metadataName)
        {
            if (metadata.TryGetValue(metadataName, out var value))
                return value;

            return string.Equals(metadataName, "FullPath", StringComparison.OrdinalIgnoreCase) ?
                System.IO.Path.GetFullPath(itemSpec) : "";
        }

        public void SetMetadata(string metadataName, string metadataValue)
        {
            metadata[metadataName] = metadataValue;
        }
    }
}
