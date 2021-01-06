using System;

namespace Serenity.Web
{
    public interface IFileWatcher
    {
        event Action<string> Changed;
        void RaiseChanged(string name);
        public string Path { get; }
        public string Filter { get; }
    }
}