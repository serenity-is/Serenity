using Microsoft.Extensions.Options;

namespace Serenity.Web
{
    public class DiskUploadStoreOptions : IOptions<DiskUploadStoreOptions>
    {
        public string RootPath { get; set; }
        public string TempPath { get; set; }
        public string ServeUrl { get; set; }

        public DiskUploadStoreOptions Value => this;
    }
}