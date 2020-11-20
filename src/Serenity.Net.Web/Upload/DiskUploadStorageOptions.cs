using Microsoft.Extensions.Options;

namespace Serenity.Web
{
    public class DiskUploadStorageOptions : IOptions<DiskUploadStorageOptions>
    {
        public string RootPath { get; set; }
        public string RootUrl { get; set; }
        public string TempPath { get; set; }

        public DiskUploadStorageOptions Value => this;
    }
}