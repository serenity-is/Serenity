using Serenity.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Serenity.Reporting
{
    public class HtmlToPdfConverter : IHtmlToPdfOptions
    {       
        public HtmlToPdfConverter()
        {
            AdditionalUrls = new List<string>();
            Cookies = new Dictionary<string, string>();
            CustomArgs = new List<string>();
            FooterHeaderReplace = new Dictionary<string, string>();
            TimeoutSeconds = 300;
            UsePrintMediaType = true;
            PrintBackground = true;
            PageSize = "A4";
        }

        public byte[] Execute()
        {

            var exePath = UtilityExePath ?? throw new ArgumentNullException(nameof(UtilityExePath));
            
            if (!File.Exists(exePath))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, 
                    "Can't find wkhtmltopdf.exe which is required for PDF generation.\n" +
                    "Please download a stable version from http://wkhtmltopdf.org/downloads.html\n and place it under directory '{0}'.", 
                    Path.GetDirectoryName(exePath)));
            }

            if (string.IsNullOrEmpty(Url))
                throw new ArgumentNullException("url");

            var args = new List<string>
            {
                !SmartShrinking ? "--disable-smart-shrinking" : "--enable-smart-shrinking"
            };

            if (!string.IsNullOrEmpty(PageSize))
            {
                args.Add("--page-size");
                args.Add(PageSize);
            }

            if (!string.IsNullOrEmpty(PageWidth))
            {
                args.Add("--page-width");
                args.Add(PageWidth);
            }

            if (!string.IsNullOrEmpty(PageHeight))
            {
                args.Add("--page-height");
                args.Add(PageHeight);
            }

            if (MarginLeft != null)
            {
                args.Add("--margin-left");
                args.Add(MarginLeft);
            }

            if (MarginTop != null)
            {
                args.Add("--margin-top");
                args.Add(MarginTop);
            }

            if (MarginRight != null)
            {
                args.Add("--margin-right");
                args.Add(MarginRight);
            }

            if (MarginBottom != null)
            {
                args.Add("--margin-bottom");
                args.Add(MarginBottom);
            }

            if (Dpi != null)
            {
                args.Add("--dpi");
                args.Add(Dpi.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (Zoom != null)
            {
                args.Add("--zoom");
                args.Add(Zoom);
            }

            if (UsePrintMediaType)
                args.Add("--print-media-type");
            else
                args.Add("--no-print-media-type");

            if (PrintBackground)
                args.Add("--background");
            else
                args.Add("--no-background");

            if (HeaderHtmlUrl != null)
            {
                args.Add("--header-html");
                args.Add(HeaderHtmlUrl);
            }

            if (FooterHtmlUrl != null)
            {
                args.Add("--footer-html");
                args.Add(FooterHtmlUrl);
            }

            if (Landscape)
            {
                args.Add("--orientation");
                args.Add("Landscape");
            }

            foreach (var cookie in Cookies)
            {
                args.Add("--cookie");
                args.Add(cookie.Key);
                args.Add(cookie.Value);
            }

            foreach (var replace in FooterHeaderReplace)
            {
                args.Add("--replace");
                args.Add(replace.Key);
                args.Add(replace.Value);
            }

            args.Add(Url);
            foreach (var additional in AdditionalUrls)
                args.Add(additional);

            foreach (var arg in CustomArgs)
                args.Add(arg);

            var tempFile = Path.GetTempFileName();
            try
            {
                args.Add(tempFile);

                var process = new Process 
                { 
                    StartInfo = new ProcessStartInfo(exePath, CommandLineTools.EscapeArguments(args.ToArray()))
                    {
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                if (!process.Start())
                    throw new InvalidOperationException("An error occurred while starting PDF generator!");

                if (!process.WaitForExit(TimeoutSeconds * 1000)) // max 300 seconds
                    throw new InvalidOperationException("Timeout while PDF generation!");

                if (process.ExitCode != 0 && process.ExitCode != 1)
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                        "PDF generator returned error code {0}!", process.ExitCode));

                if (!File.Exists(tempFile))
                    throw new InvalidOperationException("Can't find generatored PDF file!");

                var bytes = File.ReadAllBytes(tempFile);
                if (bytes.Length == 0)
                    throw new InvalidOperationException("Generated PDF file is empty!");

                return bytes;
            }
            finally
            {
                TemporaryFileHelper.TryDelete(tempFile);
            }
        }

        public string Url { get; set; }
        public List<string> AdditionalUrls { get; set; }
        public Dictionary<string, string> Cookies { get; set; }
        public int TimeoutSeconds { get; set; }
        public bool UsePrintMediaType { get; set; }
        public bool PrintBackground { get; set; }
        public string UtilityExePath { get; set; }
        public string PageHeight { get; set; }
        public string PageSize { get; set; }
        public string PageWidth { get; set; }
        public bool SmartShrinking { get; set; }
        public int? Dpi { get; set; }
        public bool Landscape { get; set; }
        public string Zoom { get; set; }
        public string MarginLeft { get; set; }
        public string MarginRight { get; set; }
        public string MarginBottom { get; set; }
        public string MarginTop { get; set; }
        public string HeaderHtmlUrl { get; set; }
        public string FooterHtmlUrl { get; set; }

        public Dictionary<string, string> FooterHeaderReplace { get; private set; }
        public List<string> CustomArgs { get; private set; }

        public string MarginsAll
        {
            set
            {
                MarginLeft = value;
                MarginTop = value;
                MarginRight = value;
                MarginBottom = value;
            }
        }
    }
}