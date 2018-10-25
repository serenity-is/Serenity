﻿using Serenity.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Xunit;

namespace Serenity.Test
{
    [Collection("AvoidParallel")]
    public class ScriptTests : SeleniumTestBase
    {
        protected override bool UseFileSystem()
        {
            return true;
        }

        protected override string GetWebPath()
        {
            return Path.GetDirectoryName(Path.GetDirectoryName(
                AppDomain.CurrentDomain.BaseDirectory));
        }

        private Dictionary<string, object> GetQunitTestResults()
        {
            return ((OpenQA.Selenium.IJavaScriptExecutor)Browser).ExecuteScript(
            (
                "return window.global_test_results"
            )) as Dictionary<string, object>;
        }

        [Fact]
        public void ScriptTestsPasses()
        {
            GoToUrl("~/ScriptTests/Runner.html");
            int tries = 0;
            while (tries++ < 3000)
            {
                if (Browser.Title.IndexOf("Serenity Tests") >= 0)
                    break;

                Thread.Sleep(100);
                tries++;
            }
            try
            {
                Assert.True(Browser.Title.IndexOf("Serenity Tests") >= 0,
                    "Couldn't load test page. Probably IIS express launch error!");

                Dictionary<string, object> qunitResults = null;
                tries = 0;
                while (tries++ < 3000 && qunitResults == null)
                {
                    Thread.Sleep(100);
                    qunitResults = GetQunitTestResults();
                }

                Assert.True(qunitResults != null, "Couldn't read QUNIT results!");

                int failed = -1;
                if (qunitResults.ContainsKey("failed"))
                    failed = Convert.ToInt32(qunitResults["failed"]);

                Assert.True(failed == 0, String.Format("{0} tests failed! Results: {1}", failed,
                    JSON.Stringify(qunitResults)));
            }
            catch (Exception)
            {
                SaveErrorScreenshot();
                throw;
            }
        }
    }
}