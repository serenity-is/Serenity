using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using Xunit;

namespace Serenity.Testing
{
    public class ScriptTests : SeleniumTestBase
    {
        protected override string GetWebPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "webroot");
        }

        private Dictionary<string, object> ExecuteQunitTests()
        {
            Browser.Manage().Timeouts().SetScriptTimeout(new TimeSpan(0, 1, 0));

            return ((OpenQA.Selenium.IJavaScriptExecutor)Browser).ExecuteAsyncScript(
            (
                "var callback = arguments[arguments.length - 1];" +
                "QUnit.done(callback); " +
                "QUnit.start();"
            )) as Dictionary<string, object>;
        }

        [Fact]
        public void SerenityTestsPasses()
        {
            GoToUrl("~/test.html?noautostart=1");
            try
            {
                var qunitResults = ExecuteQunitTests();
                int failed = -1;
                if (qunitResults.ContainsKey("failed"))
                    failed = Convert.ToInt32(qunitResults["failed"]);
                Assert.True(failed == 0, String.Format("{0} tests failed!", failed));
            }
            catch (Exception)
            {
                SaveErrorScreenshot();
                throw;
            }
        }
    }
}