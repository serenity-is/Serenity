using Serenity.Services;
using System.Collections.Generic;

namespace Serenity.Reporting
{
    public class GenerateExcelFileRequest : ServiceRequest
    {
        public List<string> Captions { get; set; }
        public List<object[]> Data { get; set; }
        public string DownloadName { get; set; }
    }
}