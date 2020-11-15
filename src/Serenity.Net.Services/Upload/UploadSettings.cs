using Serenity.ComponentModel;

namespace Serenity.Web
{
    [SettingScope("Application"), SettingKey("UploadSettings")]
    public class UploadSettings
    {
        public string Url { get; set; }
        public string Path { get; set; }
    }
}