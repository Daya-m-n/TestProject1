﻿using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestProject1.CustomMethods
{
    public class ExtentReportClass
    {
        public static ExtentReports _extent;
        protected ExtentTest _test;
        public IWebDriver driver;

        [OneTimeSetUp]
        protected void Setup()
        {
            if (_extent == null)
            {
                var path = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
                var actualPath = path.Substring(0, path.LastIndexOf("bin"));
                var projectPath = new Uri(actualPath).LocalPath;
                if (Directory.Exists(projectPath.ToString() + "Reports") == false)
                {
                    Directory.CreateDirectory(projectPath.ToString() + "Reports");
                }
                //Directory.CreateDirectory(projectPath.ToString() + "Reports");
                var reportPath = projectPath + "Reports\\index.html";
                var htmlReporter = new ExtentHtmlReporter(reportPath);
                _extent = new ExtentReports();
                _extent.AttachReporter(htmlReporter);
                _extent.AddSystemInfo("Host Name", "LocalHost");
                _extent.AddSystemInfo("Browser", TestContext.Parameters.Get("browser"));
                _extent.AddSystemInfo("Portal", TestContext.Parameters.Get("portal"));
                _extent.AddSystemInfo("UserName", TestContext.Parameters.Get("username"));
            }

        }
        [OneTimeTearDown]
        protected void TearDown()
        {
            _extent.Flush();
        }
        [SetUp]
        public void BeforeTest()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl(TestContext.Parameters.Get("url"));
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(60);
            driver.Manage().Window.Maximize();
            _test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
        }
        [TearDown]
        public void AfterTest()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace)
                    ? ""
                    : string.Format("{0}", TestContext.CurrentContext.Result.StackTrace); Status logstatus;
            switch (status)
            {
                case TestStatus.Failed:
                    logstatus = Status.Fail;
                    DateTime time = DateTime.Now;
                    //String fileName = "Screenshot_" + time.ToString("h_mm_ss") + ".png";
                    //String screenShotPath = Capture(driver, fileName);
                    _test.Log(Status.Fail, "Fail");
                    //_test.Log(Status.Fail, "Snapshot below: " + _test.AddScreenCaptureFromPath("Screenshots\\" + fileName));
                    break;
                case TestStatus.Inconclusive:
                    logstatus = Status.Warning;
                    break;
                case TestStatus.Skipped:
                    logstatus = Status.Skip;
                    break;
                default:
                    logstatus = Status.Pass;
                    break;
            }
            _test.Log(logstatus, "Test ended with " + logstatus + stacktrace);
            _extent.Flush();
            BaseClass baseClass = new BaseClass();
            baseClass.CloseTest(driver);
        }
        public IWebDriver GetDriver()
        {
            return driver;
        }
        public static string Capture(IWebDriver driver, String screenShotName)
        {
            ITakesScreenshot ts = (ITakesScreenshot)driver;
            Screenshot screenshot = ts.GetScreenshot();
            var pth = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
            var actualPath = pth.Substring(0, pth.LastIndexOf("bin"));
            var reportPath = new Uri(actualPath).LocalPath;
            Directory.CreateDirectory(reportPath + "Reports\\" + "Screenshots");
            var finalpth = pth.Substring(0, pth.LastIndexOf("bin")) + "Reports\\Screenshots\\" + screenShotName;
            var localpath = new Uri(finalpth).LocalPath;
            screenshot.SaveAsFile(localpath, ScreenshotImageFormat.Png);
            return reportPath;
        }
    }
}
