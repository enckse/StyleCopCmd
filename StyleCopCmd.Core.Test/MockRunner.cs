//------------------------------------------------------------------------------
// <copyright 
//  file="MockRunner.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core.Test
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using StyleCop;

    /// <summary>
    /// Mock runner implementation and tests for RunnerBase
    /// </summary>
    [TestFixture]
    public class MockRunner : RunnerBase
    {
        /// <summary>
        /// Enable overriding the configuration call
        /// </summary>
        private bool overrideConfig = false;

        /// <summary>
        /// Indicates if the protected initialize method has been called
        /// </summary>
        private bool initializeCalled = false;

        /// <summary>
        /// Indicates if the protected add method has been called
        /// </summary>
        private bool sourceCalled = false;

        /// <summary>
        /// Indicates if the protected run method has been called
        /// </summary>
        private bool runCalled = false;

        /// <summary>
        /// Tests calling set on the runner
        /// </summary>
        [Test]
        public void SetTest()
        {
            var inst = new MockRunner();
            try
            {
                inst.Set(null);
                Assert.Fail("No settings were given");
            }
            catch (ArgumentNullException error)
            {
                Assert.IsTrue(error.Message.Contains("settings"));
            }

            inst.Set(new ReportSettings());
        }

        /// <summary>
        /// Tests calling configure on the runner
        /// </summary>
        [Test]
        public void ConfigureTest()
        {
            var inst = new MockRunner();
            try
            {
                inst.Configure();
                Assert.Fail("No settings were given");
            }
            catch (InvalidOperationException error)
            {
                Assert.IsTrue(error.Message.Contains("No settings"));
            }

            inst.Set(new ReportSettings());
            var cfg = inst.Configure();
            Assert.IsNotNull(cfg);
            inst.overrideConfig = true;
            cfg = inst.Configure();
            Assert.IsNull(cfg);
        }

        /// <summary>
        /// Tests calling add on the runner
        /// </summary>
        [Test]
        public void AddFileTest()
        {
            var inst = new MockRunner();
            try
            {
                inst.AddFile(null, null);
                Assert.Fail("No settings were given");
            }
            catch (InvalidOperationException error)
            {
                Assert.IsTrue(error.Message.Contains("No settings"));
            }

            inst.Set(new ReportSettings());
            try
            {
                inst.AddFile(null, null);
                Assert.Fail("No options were given");
            }
            catch (InvalidOperationException error)
            {
                Assert.IsTrue(error.Message.Contains("Instance is not initialized"));
            }

            inst.Initialize();
            Assert.IsFalse(inst.sourceCalled);
            inst.AddFile(null, null);
            Assert.IsTrue(inst.sourceCalled);
        }

        /// <summary>
        /// Tests calling start on the runner
        /// </summary>
        [Test]
        public void StartTest()
        {
            var inst = new MockRunner();
            try
            {
                inst.Start(null, null, null);
                Assert.Fail("No settings were given");
            }
            catch (InvalidOperationException error)
            {
                Assert.IsTrue(error.Message.Contains("No settings"));
            }

            inst.Set(new ReportSettings());
            try
            {
                inst.Start(null, null, null);
                Assert.Fail("No options were given");
            }
            catch (InvalidOperationException error)
            {
                Assert.IsTrue(error.Message.Contains("Instance is not initialized"));
            }

            inst.Initialize();
            try
            {
                inst.Start(null, null, null);
                Assert.Fail("Projects not set");
            }
            catch (ArgumentNullException error)
            {
                Assert.IsTrue(error.Message.Contains("projects"));
            }

            Assert.IsFalse(inst.runCalled);
            inst.Start(new List<CodeProject>(), null, null);
            Assert.IsTrue(inst.runCalled);
        }

        /// <summary>
        /// Tests calling initialize on the runner
        /// </summary>
        [Test]
        public void InitializeTest()
        {
            var inst = new MockRunner();
            inst.Set(new ReportSettings());
            Assert.IsFalse(inst.initializeCalled);
            inst.Initialize();
            Assert.IsTrue(inst.initializeCalled);
        }

        /// <summary>
        /// Getting a settings optional values
        /// </summary>
        [Test]
        public void GetValues()
        {
            var inst = new MockRunner();
            var settings = new ReportSettings();
            inst.Set(settings);
            var val = inst.Get<int>("test", 0);
            Assert.AreEqual(0, val);
            settings.OptionalValues = new Dictionary<string, object>();
            val = inst.Get<int>("test", 0);
            Assert.AreEqual(0, val);
            settings.OptionalValues["test"] = 1;
            val = inst.Get<int>("test", 0);
            Assert.AreEqual(1, val);
            settings.OptionalValues["test"] = null;
            val = inst.Get<int>("test", 0);
            Assert.AreEqual(0, val);
            try
            {
                settings.OptionalValues["test"] = "fail";
                inst.Get<int>("test", 0);
                Assert.Fail("Should have failed on convert");
            }
            catch (FormatException error)
            {
                Assert.AreEqual("Input string was not in the correct format", error.Message);
            }
        }

        /// <inheritdoc />
        public override Configuration Configure()
        {
            if (this.overrideConfig)
            {
                return null;
            }
            else
            {
                return base.Configure();
            }
        }

        /// <summary>
        /// Make the underlying call public
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="key">Key to get</param>
        /// <param name="defaultVal">Default value to use</param>
        /// <returns>The found value or default value</returns>
        internal T Get<T>(string key, T defaultVal)
        {
            return this.GetOptional<T>(key, defaultVal);
        }

        /// <inheritdoc />
        protected override StyleCopRunner InitInstance()
        {
            this.initializeCalled = true;
            return new StyleCopRunnerMock();
        }

        /// <inheritdoc />
        protected override void AddSource(CodeProject project, string path)
        {
            this.sourceCalled = true;
        }

        /// <inheritdoc />
        protected override void Run(IList<CodeProject> projects)
        {
            this.runCalled = true;
        }
    }
}
