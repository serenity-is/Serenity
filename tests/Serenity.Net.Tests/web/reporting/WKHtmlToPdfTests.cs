using Serenity.Web;

namespace Serenity.Reporting;

public class WKHtmlToPdfTests
{
    private sealed class FakeStartedProcess : IStartedProcess
    {
        public System.IO.StreamReader StandardOutput { get; set; } = new(System.IO.Stream.Null);
        public System.IO.StreamReader StandardError { get; set; } = new(System.IO.Stream.Null);
        public System.IO.TextWriter StandardInput { get; set; } = System.IO.TextWriter.Null;
        public bool EnableRaisingEvents { get; set; }
        public bool HasExited { get; set; }
        public int ExitCode { get; set; }
        public bool WaitForExitResult { get; set; } = true;
        public Action? OnWait { get; set; }

        public bool WaitForExit(int milliseconds)
        {
            OnWait?.Invoke();
            return WaitForExitResult;
        }

        public void Kill(bool entireProcessTree)
        {
        }
    }

    private static string CreateExeFile()
    {
        return System.IO.Path.GetTempFileName();
    }

    private static WKHtmlToPdf CreateConverter(string exe, FakeStartedProcess process, string output)
    {
        return new WKHtmlToPdf(new HtmlToPdfOptions { Url = "http://localhost/report" })
        {
            ExecutablePath = exe,
            ProcessFactory = _ => process,
            TempFileNameFactory = () => output
        };
    }

    [Fact]
    public void Execute_Builds_All_Optional_Arguments()
    {
        var exe = CreateExeFile();
        var output = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            var options = new HtmlToPdfOptions
            {
                Url = "http://localhost/report",
                SmartShrinking = true,
                PageSize = "A4",
                PageWidth = "210mm",
                PageHeight = "297mm",
                MarginLeft = "1",
                MarginTop = "2",
                MarginRight = "3",
                MarginBottom = "4",
                Dpi = 96,
                Zoom = "1.2",
                UsePrintMediaType = true,
                PrintBackground = true,
                HeaderHtmlUrl = "header.html",
                FooterHtmlUrl = "footer.html",
                Landscape = true,
                DisableLocalFileAccess = true
            };
            options.Cookies["cookie"] = "value";
            options.FooterHeaderReplace["[page]"] = "1";
            options.AdditionalUrls.Add("http://extra");
            options.AllowedLocalPaths.Add("/allowed");
            options.CustomArgs.Add("--custom");

            var process = new FakeStartedProcess
            {
                OnWait = () => System.IO.File.WriteAllBytes(output, [7])
            };
            var converter = new WKHtmlToPdf(options)
            {
                ExecutablePath = exe,
                ProcessFactory = _ => process,
                TempFileNameFactory = () => output
            };

            Assert.Equal([7], converter.Execute());
        }
        finally
        {
            System.IO.File.Delete(exe);
            if (System.IO.File.Exists(output))
                System.IO.File.Delete(output);
        }
    }

    [Fact]
    public void Execute_Returns_Generated_Bytes()
    {
        var exe = CreateExeFile();
        var output = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            var process = new FakeStartedProcess
            {
                OnWait = () => System.IO.File.WriteAllBytes(output, [1, 2, 3])
            };
            var converter = CreateConverter(exe, process, output);

            var bytes = converter.Execute();

            Assert.Equal([1, 2, 3], bytes);
        }
        finally
        {
            System.IO.File.Delete(exe);
            if (System.IO.File.Exists(output))
                System.IO.File.Delete(output);
        }
    }

    [Fact]
    public void Execute_Allows_ExitCode_One()
    {
        var exe = CreateExeFile();
        var output = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            var process = new FakeStartedProcess
            {
                ExitCode = 1,
                OnWait = () => System.IO.File.WriteAllBytes(output, [9])
            };
            var converter = CreateConverter(exe, process, output);

            Assert.Equal([9], converter.Execute());
        }
        finally
        {
            System.IO.File.Delete(exe);
            if (System.IO.File.Exists(output))
                System.IO.File.Delete(output);
        }
    }

    [Fact]
    public void Execute_Throws_On_Timeout()
    {
        var exe = CreateExeFile();
        var output = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            var process = new FakeStartedProcess { WaitForExitResult = false };
            var converter = CreateConverter(exe, process, output);

            var exception = Assert.Throws<InvalidOperationException>(() => converter.Execute());
            Assert.Contains("Timeout", exception.Message);
        }
        finally
        {
            System.IO.File.Delete(exe);
        }
    }

    [Fact]
    public void Execute_Throws_On_Error_ExitCode()
    {
        var exe = CreateExeFile();
        var output = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            var process = new FakeStartedProcess { ExitCode = 2 };
            var converter = CreateConverter(exe, process, output);

            var exception = Assert.Throws<InvalidOperationException>(() => converter.Execute());
            Assert.Contains("error code 2", exception.Message);
        }
        finally
        {
            System.IO.File.Delete(exe);
        }
    }

    [Fact]
    public void Execute_Throws_When_Output_Missing()
    {
        var exe = CreateExeFile();
        var output = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            var converter = CreateConverter(exe, new FakeStartedProcess(), output);

            var exception = Assert.Throws<InvalidOperationException>(() => converter.Execute());
            Assert.Contains("Can't find generated PDF file", exception.Message);
        }
        finally
        {
            System.IO.File.Delete(exe);
        }
    }

    [Fact]
    public void Execute_Throws_When_Output_Empty()
    {
        var exe = CreateExeFile();
        var output = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            var process = new FakeStartedProcess
            {
                OnWait = () => System.IO.File.WriteAllBytes(output, [])
            };
            var converter = CreateConverter(exe, process, output);

            var exception = Assert.Throws<InvalidOperationException>(() => converter.Execute());
            Assert.Contains("empty", exception.Message);
        }
        finally
        {
            System.IO.File.Delete(exe);
        }
    }

    [Fact]
    public void Execute_Throws_Without_ExecutablePath()
    {
        Assert.Throws<ArgumentNullException>(() => new WKHtmlToPdf().Execute());
    }

    [Fact]
    public void Execute_Throws_When_Executable_Not_Found()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new WKHtmlToPdf { ExecutablePath = "not-existing-wkhtmltopdf.exe" }.Execute());

        Assert.Contains("Can't find wkhtmltopdf", exception.Message);
    }

    [Fact]
    public void Execute_Throws_Without_Url()
    {
        var exe = System.IO.Path.GetTempFileName();
        try
        {
            var options = new HtmlToPdfOptions { Url = null };
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new WKHtmlToPdf(options) { ExecutablePath = exe }.Execute());

            Assert.Equal("url", exception.ParamName);
        }
        finally
        {
            System.IO.File.Delete(exe);
        }
    }

    [Fact]
    public void Properties_RoundTrip_To_Options()
    {
        var options = new HtmlToPdfOptions();
        var pdf = new WKHtmlToPdf(options)
        {
            Url = "http://x",
            TimeoutSeconds = 5,
            UsePrintMediaType = true,
            PrintBackground = true,
            PageHeight = "h",
            PageSize = "A4",
            PageWidth = "w",
            SmartShrinking = true,
            Dpi = 96,
            Landscape = true,
            Zoom = "1.5",
            MarginLeft = "1",
            MarginRight = "2",
            MarginBottom = "3",
            MarginTop = "4",
            HeaderHtmlUrl = "header",
            FooterHtmlUrl = "footer",
            DisableLocalFileAccess = true,
            EditLaunchOptions = _ => { },
            EditPdfOptions = _ => { }
        };

        Assert.Equal("http://x", pdf.Url);
        Assert.Equal(5, pdf.TimeoutSeconds);
        Assert.True(pdf.UsePrintMediaType);
        Assert.True(pdf.PrintBackground);
        Assert.Equal("h", pdf.PageHeight);
        Assert.Equal("A4", pdf.PageSize);
        Assert.Equal("w", pdf.PageWidth);
        Assert.True(pdf.SmartShrinking);
        Assert.Equal(96, pdf.Dpi);
        Assert.True(pdf.Landscape);
        Assert.Equal("1.5", pdf.Zoom);
        Assert.Equal("1", pdf.MarginLeft);
        Assert.Equal("2", pdf.MarginRight);
        Assert.Equal("3", pdf.MarginBottom);
        Assert.Equal("4", pdf.MarginTop);
        Assert.Equal("header", pdf.HeaderHtmlUrl);
        Assert.Equal("footer", pdf.FooterHtmlUrl);
        Assert.True(pdf.DisableLocalFileAccess);
        Assert.NotNull(pdf.EditLaunchOptions);
        Assert.NotNull(pdf.EditPdfOptions);

        pdf.AdditionalUrls.Add("extra");
        pdf.Cookies["c"] = "v";
        pdf.FooterHeaderReplace["a"] = "b";
        pdf.AllowedLocalPaths.Add("path");
        pdf.CustomArgs.Add("--custom");

        Assert.Single(pdf.AdditionalUrls);
        Assert.Single(pdf.Cookies);
        Assert.Single(pdf.FooterHeaderReplace);
        Assert.Single(pdf.AllowedLocalPaths);
        Assert.Single(pdf.CustomArgs);

        pdf.MarginsAll = "5";
        Assert.Equal("5", pdf.MarginLeft);
        Assert.Equal("5", pdf.MarginTop);
        Assert.Equal("5", pdf.MarginRight);
        Assert.Equal("5", pdf.MarginBottom);
    }

    [Fact]
    public void HtmlToPdfConverter_UtilityExePath_RoundTrips()
    {
#pragma warning disable CS0618
        var converter = new HtmlToPdfConverter { UtilityExePath = "some/path" };
        Assert.Equal("some/path", converter.UtilityExePath);
        Assert.Equal("some/path", converter.ExecutablePath);
#pragma warning restore CS0618
    }
}
