using Microsoft.Extensions.DependencyInjection;
using Serenity.Web;
using System;
using System.IO;

namespace Serenity
{
    public static class FakeUploadStorageExtensions
    {
        private class FakeUploadStorage : DefaultUploadStorage, IDisposable
        {
            private bool disposed;
            private readonly string tempPath;

            public FakeUploadStorage(string tempPath)
                : base(new UploadSettings
                {
                    Path = tempPath,
                    Url = "~/upload/"
                })
            {
                this.tempPath = tempPath;
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposed)
                    disposed = true;

                var isTemporaryCheck = Path.Combine(tempPath, ".istemporary");
                if (File.Exists(isTemporaryCheck) &&
                    File.ReadAllText(isTemporaryCheck) == "YES!")
                    Directory.Delete(tempPath, true);
            }

            ~FakeUploadStorage()
            {
                Dispose(disposing: false);
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }

        public static string AddFakeUploadStorage(this IServiceCollection collection)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), ".tempupload", Path.GetRandomFileName());
            if (Directory.Exists(tempDir))
                throw new InvalidOperationException();
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(".istemporary", "YES!");
            var uploadStorage = new FakeUploadStorage(tempDir);
            collection.AddSingleton<IUploadStorage>(uploadStorage);
            return tempDir;
        }
    }
}