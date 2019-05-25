using Serenity.Services;
using System.Collections.Generic;

namespace Serenity.Reporting
{
    public class GenerateCsvRequest : ServiceRequest
    {
        public List<string> Captions { get; set; }
        public List<string[]> Data { get; set; }
        public string DownloadName { get; set; }
    }
}