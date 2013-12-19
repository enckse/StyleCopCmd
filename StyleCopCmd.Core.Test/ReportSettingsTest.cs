//------------------------------------------------------------------------------
// <copyright 
//  file="ReportSettingsTest.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core.Test
{ 
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
 
    /// <summary>
    /// Report settings testing for managing report settings
    /// </summary>
    [TestFixture]
    public class ReportSettingsTest
    {
        /// <summary>
        /// Get a set of files and add a descriptor type
        /// </summary>
        [Test]
        public void BasicConstruction()
        {
            ReportSettings settings = new ReportSettings();
            Assert.IsNull(settings.SolutionFiles);
            Assert.IsNull(settings.ProjectFiles);
            Assert.IsNull(settings.Files);
            Assert.IsNull(settings.Directories);
            Assert.IsNull(settings.IgnorePatterns);
            Assert.IsNull(settings.ProcessorSymbols);
            Assert.IsNull(settings.AddInDirectories);
            Assert.IsFalse(settings.RecursionEnabled);
            Assert.IsFalse(settings.EnableDebug);
            Assert.IsFalse(settings.AllowCaching);
            Assert.IsNull(settings.StyleCopSettingsFile);

            settings.SolutionFiles = new List<string>() { "solutions" };
            settings.ProjectFiles = new List<string>() { "proj", "proj2" };
            settings.Files = new List<string>() { "file", "file2", "file3" };
            settings.Directories = new List<string>() { "dir", "dir2" };
            settings.IgnorePatterns = new List<string>() { "pattern", "pattern2", "pattern3" };
            settings.ProcessorSymbols = new List<string>() { "sym" };
            settings.AddInDirectories = new List<string>() { "addin", "addin" };
            settings.RecursionEnabled = true;
            settings.EnableDebug = true;
            settings.StyleCopSettingsFile = "Settings.File";
            settings.AllowCaching = true;

            Assert.IsNotNull(settings.SolutionFiles);
            Assert.AreEqual(1, settings.SolutionFiles.Count);
            Assert.IsNotNull(settings.ProjectFiles);
            Assert.AreEqual(2, settings.ProjectFiles.Count);
            Assert.IsNotNull(settings.Files);
            Assert.AreEqual(3, settings.Files.Count);
            Assert.IsNotNull(settings.Directories);
            Assert.AreEqual(2, settings.Directories.Count);
            Assert.IsNotNull(settings.IgnorePatterns);
            Assert.AreEqual(3, settings.IgnorePatterns.Count);
            Assert.IsNotNull(settings.ProcessorSymbols);
            Assert.AreEqual(1, settings.ProcessorSymbols.Count);
            Assert.IsNotNull(settings.AddInDirectories);
            Assert.AreEqual(2, settings.AddInDirectories.Count);
            Assert.IsTrue(settings.RecursionEnabled);
            Assert.IsTrue(settings.EnableDebug);
            Assert.AreEqual("Settings.File", settings.StyleCopSettingsFile);
            Assert.IsTrue(settings.AllowCaching);
        }

        /// <summary>
        /// Get all analysis files
        /// </summary>
        [Test]
        public void GetAllFiles()
        {
            ReportSettings settings = new ReportSettings();
            settings.SolutionFiles = new List<string>() { "solutions" };
            settings.ProjectFiles = new List<string>() { "proj", "proj2" };
            settings.Files = new List<string>() { "file", "file2", "file3" };
            settings.Directories = new List<string>() { "dir", "dir2" };
            var allFiles = settings.GetAllFiles();
            Assert.AreEqual(8, allFiles.Count());
            Assert.AreEqual(1, allFiles.Where(x => x.File == "solutions" && x.Type == FileType.Solution).Count());
            Assert.AreEqual(1, allFiles.Where(x => x.File == "proj" && x.Type == FileType.Project).Count());
            Assert.AreEqual(1, allFiles.Where(x => x.File == "proj2" && x.Type == FileType.Project).Count());
            Assert.AreEqual(1, allFiles.Where(x => x.File == "file" && x.Type == FileType.File).Count());
            Assert.AreEqual(1, allFiles.Where(x => x.File == "file2" && x.Type == FileType.File).Count());
            Assert.AreEqual(1, allFiles.Where(x => x.File == "file3" && x.Type == FileType.File).Count());
            Assert.AreEqual(1, allFiles.Where(x => x.File == "dir2" && x.Type == FileType.Directory).Count());
            Assert.AreEqual(1, allFiles.Where(x => x.File == "dir" && x.Type == FileType.Directory).Count());
        }

        /// <summary>
        /// Testing a mix and match of files to analyze
        /// </summary>
        [Test]
        public void GetAllFilesMixed()
        {
            ReportSettings settings = new ReportSettings();
            settings.SolutionFiles = new List<string>() { "solutions" };
            settings.ProjectFiles = null;
            settings.Files = new List<string>() { "file", string.Empty };
            settings.Directories = new List<string>() { null, null };
            var allFiles = settings.GetAllFiles();
            Assert.AreEqual(2, allFiles.Count());
            Assert.AreEqual(1, allFiles.Where(x => x.File == "solutions" && x.Type == FileType.Solution).Count());
            Assert.AreEqual(1, allFiles.Where(x => x.File == "file" && x.Type == FileType.File).Count());
        }
    }
}
