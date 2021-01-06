using Microsoft.Extensions.Options;

namespace Serenity.Web
{
    public class UploadSettings : IOptions<UploadSettings>
    {
        public const string SectionKey = "UploadSettings";

        public string Path { get; set; }
        public string Url { get; set; }

        public UploadSettings()
        {
            Path = "App_Data/upload/";
            Url = "~/upload/";
        }

        public UploadSettings Value => this;
    }
}