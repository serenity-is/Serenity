using Serenity.IO;
using System.Diagnostics;
using System.IO;

namespace Serenity.Reporting
{
    /// <summary>
    /// HTML to PDF converter class using WKHTMLToPdf
    /// </summary>
    public class HtmlToPdfConverter : IHtmlToPdfOptions
    {       
        /// <summary>
        /// Creates a new instance of the class
        /// </summary>
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

        /// <summary>
        /// Executes the converter process and returns the PDF bytes
        /// </summary>
        /// <exception cref="ArgumentNullException">UtilityExePath or URL is null</exception>
        /// <exception cref="InvalidOperationException">An error occureed during process execution</exception>
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

        /// <inheritdoc/>
        public string Url { get; set; }

        /// <inheritdoc/>
        public List<string> AdditionalUrls { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, string> Cookies { get; set; }

        /// <inheritdoc/>
        public int TimeoutSeconds { get; set; }

        /// <inheritdoc/>
        public bool UsePrintMediaType { get; set; }

        /// <inheritdoc/>
        public bool PrintBackground { get; set; }

        /// <inheritdoc/>
        public string UtilityExePath { get; set; }

        /// <inheritdoc/>
        public string PageHeight { get; set; }
        
        /// <inheritdoc/>
        public string PageSize { get; set; }

        /// <inheritdoc/>
        public string PageWidth { get; set; }

        /// <inheritdoc/>
        public bool SmartShrinking { get; set; }

        /// <inheritdoc/>
        public int? Dpi { get; set; }

        /// <inheritdoc/>
        public bool Landscape { get; set; }

        /// <inheritdoc/>
        public string Zoom { get; set; }

        /// <inheritdoc/>
        public string MarginLeft { get; set; }

        /// <inheritdoc/>
        public string MarginRight { get; set; }
        
        /// <inheritdoc/>
        public string MarginBottom { get; set; }

        /// <inheritdoc/>
        public string MarginTop { get; set; }

        /// <inheritdoc/>
        public string HeaderHtmlUrl { get; set; }

        /// <inheritdoc/>
        public string FooterHtmlUrl { get; set; }


        /// <inheritdoc/>
        public Dictionary<string, string> FooterHeaderReplace { get; private set; }

        /// <inheritdoc/>
        public List<string> CustomArgs { get; private set; }

        /// <inheritdoc/>
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