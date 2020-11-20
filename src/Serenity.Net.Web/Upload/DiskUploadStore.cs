using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace Serenity.Web
{
    public class DiskUploadStore : IUploadStore
    {
        private string rootPath;
        private string tempPath;
        private string serveUrl;

        public DiskUploadStore(IOptions<DiskUploadStoreOptions> options)
        {
            var opt = (options ?? throw new ArgumentNullException(nameof(options))).Value;

            if (string.IsNullOrWhiteSpace(opt.RootPath))
                throw new ArgumentNullException(nameof(opt.RootPath));

            if (string.IsNullOrWhiteSpace(opt.ServeUrl))
                throw new ArgumentNullException(nameof(opt.ServeUrl));

            rootPath = opt.RootPath;
            if (!Path.IsPathRooted(rootPath))
                rootPath = Path.Combine(AppContext.BaseDirectory, PathHelper.ToPath(rootPath));

            var tempPath = string.IsNullOrEmpty(opt.TempPath) ? Path.Combine(rootPath, "temporary") :
                opt.TempPath;

            if (string.IsNullOrWhiteSpace(tempPath))
                throw new ArgumentNullException(nameof(opt.TempPath));

            tempPath = PathHelper.ToPath(tempPath);

            try
            {
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                    File.WriteAllText(Path.Combine(tempPath, ".temporary"), "");
                }
            }
            catch (Exception)
            {
            }
        }

        public string TempPath => tempPath;
        public string RootPath => rootPath;

        public string DbFilePath(string dbFileName)
        {
            return PathHelper.SecureCombine(rootPath, PathHelper.ToPath(dbFileName));
        }
    }
}