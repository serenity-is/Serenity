using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;

namespace Serenity.Testing
{
    public abstract class SeleniumTestBase : IDisposable
    {
        IISProcessManager iisProcess;

        public void GoToUrl(string relativeUrl)
        {
            if (relativeUrl.StartsWith("~/"))
                Browser.Navigate().GoToUrl(UriHelper.Combine(ApplicationUrl, relativeUrl.Substring(2)));
            else
                Browser.Navigate().GoToUrl(relativeUrl);
        }

        public void SaveErrorScreenshot()
        {
            var ss = TakeScreenshot();
            const string errorFolder = @"c:\.build\errors";
            Directory.CreateDirectory(errorFolder);
            ss.SaveAsFile(errorFolder + @"\error_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".jpg", ImageFormat.Jpeg);
        }

        public OpenQA.Selenium.Screenshot TakeScreenshot()
        {
            return ((OpenQA.Selenium.ITakesScreenshot)Browser).GetScreenshot();
        }

        protected abstract string GetWebPath();

        public string ApplicationUrl
        {
            get
            {
                iisProcess = iisProcess ?? new IISProcessManager(GetWebPath());
                return iisProcess.ApplicationUrl;
            }
        }

        public static IWebDriver Browser
        {
            get
            {
                return SeleniumDriverManager.Current.WebDriver;
            }
        }

        private class SeleniumDriverManager
        {
            public static readonly SeleniumDriverManager Current = new SeleniumDriverManager();

            private static IWebDriver driver;

            private SeleniumDriverManager()
            {
                driver = new PhantomJSDriver();
                driver.Manage().Window.Size = new System.Drawing.Size(1366, 768);
            }

            public IWebDriver WebDriver
            {
                get { return driver; }
            }

            ~SeleniumDriverManager()
            {
                Dispose();
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);

                if (driver != null)
                {
                    driver.Dispose();
                    driver = null;
                }
            }
        }

        private class IISProcessManager
        {
            private Int32? iisPort;
            private Process iisProcess;
            private string applicationUrl;

            public IISProcessManager(string webRoot)
            {
                var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                var iisExpress = Path.Combine(programFiles, @"IIS Express\iisexpress.exe");
                if (!File.Exists(iisExpress))
                    throw new Exception(String.Format("IIS Express is not found at location: {0}", iisExpress));

                iisPort = TcpUtility.GetAvailableTCPPort(8000, 8100);

                var psi = new ProcessStartInfo(iisExpress)
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    ErrorDialog = true,
                    LoadUserProfile = true,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    Arguments = String.Format("/path:\"{0}\" /port:{1}", webRoot, iisPort.Value)
                };

                iisProcess = Process.Start(psi);

                applicationUrl = "http://localhost:" + iisPort.Value + "/";
            }

            public string ApplicationUrl
            {
                get { return applicationUrl; }
            }

            ~IISProcessManager()
            {
                Dispose();
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);

                if (iisProcess != null && !iisProcess.HasExited)
                {
                    iisProcess.CloseMainWindow();
                    if (!iisProcess.HasExited)
                    {
                        iisProcess.Kill();
                        iisProcess.Dispose();
                    }
                }

                iisProcess = null;
            }
        }

        public void Dispose()
        {
            if (iisProcess != null)
            {
                iisProcess.Dispose();
                iisProcess = null;
            }
        }
    }
}