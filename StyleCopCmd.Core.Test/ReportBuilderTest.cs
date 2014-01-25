//------------------------------------------------------------------------------
// <copyright 
//  file="ReportBuilderTest.cs" 
//  company="Schley Andrew Kutz">
//  Copyright (c) Schley Andrew Kutz. All rights reserved.
// </copyright>
// <authors>
//   <author>Schley Andrew Kutz</author>
// </authors>
//------------------------------------------------------------------------------
/*******************************************************************************
 * Copyright (c) 2008, Schley Andrew Kutz <sakutz@gmail.com>
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 *
 * * Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 * * Redistributions in binary form must reproduce the above copyright notice,
 *   this list of conditions and the following disclaimer in the documentation
 *   and/or other materials provided with the distribution.
 * * Neither the name of Schley Andrew Kutz nor the names of its 
 *   contributors may be used to endorse or promote products derived from this 
 *   software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 ******************************************************************************/
namespace StyleCopCmd.Core.Test
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;

    /// <summary>
    /// Tests the ReportBuilder class.
    /// </summary>
    [TestFixture]
    public class ReportBuilderTest
    {
        /// <summary>
        /// Constant test name for testing the StyleCop calls
        /// </summary>
        private const string TestName = "StyleCopTestProject";
        
        /// <summary>
        /// The base path for testing.
        /// </summary>
        private static readonly string BasePath = GetTestSolutionPath();
        
        /// <summary>
        /// The solution path.
        /// </summary>
        private static readonly string Solution = JoinAll(BasePath, TestName + ".sln");
        
        /// <summary>
        /// The project path.
        /// </summary>
        private static readonly string Project = JoinAll(BasePath, TestName, TestName + ".csproj");
        
        /// <summary>
        /// The project path for wildcards
        /// </summary>
        private static readonly string WildCardProject = JoinAll(BasePath, TestName, TestName + "WildCard.csproj");
        
        /// <summary>
        /// The directory path for testing
        /// </summary>
        private static readonly string DirectoryPath = JoinAll(BasePath, TestName) + Path.DirectorySeparatorChar;
        
        /// <summary>
        /// Tests the WithSolutionFiles method.
        /// </summary>
        [Test]
        public void WithSolutionFilesTest()
        {
            var report = new ReportBuilder()
                            .WithSolutionsFiles(new List<string>() { Solution });
            
            var result = ExecuteTest(report, null);
            Assert.AreEqual(4, result.Count, BasePath);
            Assert.AreEqual("8 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("AssemblyInfo.cs"), result[1]);
            Assert.IsTrue(result[2].EndsWith("ClassOne.cs"), result[2]);
            Assert.IsTrue(result[3].EndsWith("ClassTwo.cs"), result[3]);
        }
        
        /// <summary>
        /// With multiple solution files test.
        /// </summary>
        [Test]
        public void WithMultipleSolutionFilesTest()
        {
            var report = new ReportBuilder()
                            .WithSolutionsFiles(new List<string>() { Solution, Solution });
            
            var result = ExecuteTest(report, null);
            Assert.AreEqual(7, result.Count, BasePath);
            Assert.AreEqual("16 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("AssemblyInfo.cs"), result[1]);
            Assert.IsTrue(result[2].EndsWith("AssemblyInfo.cs"), result[2]);
            Assert.IsTrue(result[3].EndsWith("ClassOne.cs"), result[3]);
            Assert.IsTrue(result[4].EndsWith("ClassOne.cs"), result[4]);
            Assert.IsTrue(result[5].EndsWith("ClassTwo.cs"), result[5]);
            Assert.IsTrue(result[6].EndsWith("ClassTwo.cs"), result[6]);
        }
  
        /// <summary>
        /// Tests with no real settings
        /// </summary>
        [Test]
        public void WithNothingTest()
        {
            var report = new ReportBuilder();          
            var result = ExecuteTest(report, null);
            Assert.AreEqual(1, result.Count, BasePath);
            Assert.AreEqual("No violations encountered", result[0]);
        }
        
        /// <summary>
        /// Tests the WithProjectFiles method.
        /// </summary>
        [Test]
        public void WithProjectFilesTest()
        {
            var report = new ReportBuilder()
                            .WithProjectFiles(new List<string>() { Project });
            
            var result = ExecuteTest(report, null);
            Assert.AreEqual(4, result.Count, BasePath);
            Assert.AreEqual("8 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("AssemblyInfo.cs"), result[1]);
            Assert.IsTrue(result[2].EndsWith("ClassOne.cs"), result[2]);
            Assert.IsTrue(result[3].EndsWith("ClassTwo.cs"), result[3]);
        }
        
        /// <summary>
        /// With multiple project files test.
        /// </summary>
        [Test]
        public void WithMultipleProjectFilesTest()
        {
            var report = new ReportBuilder()
                            .WithProjectFiles(new List<string>() { Project, Project });
            
            var result = ExecuteTest(report, null);
            Assert.AreEqual(7, result.Count, BasePath);
            Assert.AreEqual("16 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("AssemblyInfo.cs"), result[1]);
            Assert.IsTrue(result[2].EndsWith("AssemblyInfo.cs"), result[2]);
            Assert.IsTrue(result[3].EndsWith("ClassOne.cs"), result[3]);
            Assert.IsTrue(result[4].EndsWith("ClassOne.cs"), result[4]);
            Assert.IsTrue(result[5].EndsWith("ClassTwo.cs"), result[5]);
            Assert.IsTrue(result[6].EndsWith("ClassTwo.cs"), result[6]);
        }

        /// <summary>
        /// Tests the WithDirectories method.
        /// </summary>
        [Test]
        public void WithDirectoriesTest()
        {
            var report = new ReportBuilder()
                            .WithDirectories(new List<string>() { DirectoryPath });
            
            var result = ExecuteTest(report, null);
            Assert.AreEqual(2, result.Count, BasePath);
            Assert.AreEqual("3 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("ClassOne.cs"), result[1]);
        }
  
        /// <summary>
        /// With multiple directories test.
        /// </summary>
        [Test]
        public void WithMultipleDirectoriesTest()
        {
            var report = new ReportBuilder()
                            .WithDirectories(new List<string>() { DirectoryPath, DirectoryPath });
            
            var result = ExecuteTest(report, null);
            Assert.AreEqual(3, result.Count, BasePath);
            Assert.AreEqual("6 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("ClassOne.cs"), result[1]);
            Assert.IsTrue(result[2].EndsWith("ClassOne.cs"), result[2]);
        }
        
        /// <summary>
        /// Tests the WithFiles method.
        /// </summary>
        [Test]
        public void WithFilesTest()
        {
            var report = new ReportBuilder()
                            .WithFiles(new List<string>() { DirectoryPath + "ClassOne.cs" });
            
            var result = ExecuteTest(report, null);
            Assert.AreEqual(2, result.Count, BasePath);
            Assert.AreEqual("3 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("ClassOne.cs"), result[1]);
        }
        
        /// <summary>
        /// With multiple files test.
        /// </summary>
        [Test]
        public void WithMultipleFilesTest()
        {
            var report = new ReportBuilder()
                            .WithFiles(new List<string>() { DirectoryPath + "ClassOne.cs", DirectoryPath + "SubNamespace" + Path.DirectorySeparatorChar + "ClassTwo.cs" });
            
            var result = ExecuteTest(report, null);
            Assert.AreEqual(3, result.Count, BasePath);
            Assert.AreEqual("7 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("ClassOne.cs"), result[1]);
            Assert.IsTrue(result[2].EndsWith("ClassTwo.cs"), result[2]);
        }
        
        /// <summary>
        /// Tests the WithIgnorePatterns method.
        /// </summary>
        [Test]
        public void WithIgnorePatternsTest()
        {
            var report = new ReportBuilder()
                            .WithProjectFiles(new List<string>() { Project })
                            .WithIgnorePatterns(new List<string>() { "ClassOne" });
            
            var result = ExecuteTest(report, null);
            Assert.AreEqual(3, result.Count, BasePath);
            Assert.AreEqual("5 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("AssemblyInfo.cs"), result[1]);
            Assert.IsTrue(result[2].EndsWith("ClassTwo.cs"), result[2]);
        }
        
        /// <summary>
        /// With multiple patterns test.
        /// </summary>
        [Test]
        public void WithMultiplePatternsTest()
        {
            var report = new ReportBuilder()
                            .WithProjectFiles(new List<string>() { Project })
                            .WithIgnorePatterns(new List<string>() { "ClassOne", "Info" });
            
            var result = ExecuteTest(report, null);
            Assert.AreEqual(2, result.Count, BasePath);
            Assert.AreEqual("4 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("ClassTwo.cs"), result[1]);
        }

        /// <summary>
        /// Tests the WithRecursion method.
        /// </summary>
        [Test]
        public void WithRecursionTest()
        {
            var report = new ReportBuilder()
                            .WithDirectories(new List<string>() { DirectoryPath })
                            .WithRecursion();
            
            var result = ExecuteTest(report, null);
            Assert.AreEqual(4, result.Count, BasePath);
            Assert.AreEqual("8 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("ClassOne.cs"), result[1]);
            Assert.IsTrue(result[2].EndsWith("AssemblyInfo.cs"), result[2]);
            Assert.IsTrue(result[3].EndsWith("ClassTwo.cs"), result[3]);
        }

        /// <summary>
        /// Tests the WithProcessorSymbols method.
        /// </summary>
        [Test]
        public void WithProcessorSymbolsTest()
        {
            var report = new ReportBuilder()
                            .WithDirectories(new List<string>() { DirectoryPath })
                            .WithProcessorSymbols(new List<string>() { "SOMEOTHER" });
            
            var result = ExecuteTest(report, null);
            Assert.AreEqual(2, result.Count, BasePath);
            Assert.AreEqual("6 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("ClassOne.cs"), result[1]);
        }
        
        /// <summary>
        /// With multiple symbols test.
        /// </summary>
        [Test]
        public void WithMultipleSymbolsTest()
        {
            var report = new ReportBuilder()
                            .WithDirectories(new List<string>() { DirectoryPath })
                            .WithProcessorSymbols(new List<string>() { "SOMEOTHER", "SOMECONDITIONAL" });
            
            var result = ExecuteTest(report, null);
            Assert.AreEqual(2, result.Count, BasePath);
            Assert.AreEqual("8 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("ClassOne.cs"), result[1]);
            
            report = new ReportBuilder()
                            .WithDirectories(new List<string>() { DirectoryPath })
                            .WithProcessorSymbols(new List<string>() { "!SOMEOTHER", "SOMECONDITIONAL" });
            
            result = ExecuteTest(report, null);
            Assert.AreEqual(2, result.Count, BasePath);
            Assert.AreEqual("5 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("ClassOne.cs"), result[1]);
            
            report = new ReportBuilder()
                            .WithDirectories(new List<string>() { DirectoryPath })
                            .WithProcessorSymbols(new List<string>() { "SOMEOTHER", "!SOMECONDITIONAL" });
            
            result = ExecuteTest(report, null);
            Assert.AreEqual(2, result.Count, BasePath);
            Assert.AreEqual("6 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("ClassOne.cs"), result[1]);
            
            report = new ReportBuilder()
                            .WithDirectories(new List<string>() { DirectoryPath })
                            .WithProcessorSymbols(new List<string>() { "!SOMEOTHER", "!SOMECONDITIONAL" });
            
            result = ExecuteTest(report, null);
            Assert.AreEqual(2, result.Count, BasePath);
            Assert.AreEqual("3 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("ClassOne.cs"), result[1]);
        }

        /// <summary>
        /// Tests the WithStyleCopSettingsFile method.
        /// </summary>
        [Test]
        public void WithStyleCopSettingsFileTest()
        {
            var report = new ReportBuilder()
                            .WithSolutionsFiles(new List<string>() { Solution })
                            .WithStyleCopSettingsFile(JoinAll(BasePath, "LocalSettings.Setting"));
            
            var result = ExecuteTest(report, null);
            Assert.AreEqual(4, result.Count, BasePath);
            Assert.AreEqual("7 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("AssemblyInfo.cs"), result[1]);
            Assert.IsTrue(result[2].EndsWith("ClassOne.cs"), result[2]);
            Assert.IsTrue(result[3].EndsWith("ClassTwo.cs"), result[3]);
        }
        
        /// <summary>
        /// Output report test.
        /// </summary>
        [Test]
        public void OutputReportTest()
        {
            var testReport = "test-output";
            var testReportFull = string.Format("{0}.xml", testReport);
            if (File.Exists(testReportFull))
            {
                File.Delete(testReportFull);
            }
            
            var report = new ReportBuilder();
            ExecuteTest(report, testReport);
            Assert.IsTrue(File.Exists(testReportFull));
        }
        
        /// <summary>
        /// Verbose output test
        /// </summary>
        [Test]
        public void ViolationsTest()
        {
            var violationList = new List<string>();
            var report = new ReportBuilder()
                            .WithSolutionsFiles(new List<string>() { Solution })
                            .WithViolationEventHandler((x, y) => { violationList.Add(((StyleCop.ViolationEventArgs)y).Message); });
            
            ExecuteTest(report, null);
            violationList = violationList.OrderBy(value => value).ToList();
            Assert.AreEqual(8, violationList.Count);
            Assert.AreEqual("A closing curly bracket must not be preceded by a blank line.", violationList[0]);
            Assert.AreEqual("A closing curly bracket must not be preceded by a blank line.", violationList[1]);
            Assert.AreEqual("All using directives must be placed inside of the namespace.", violationList[2]);
            Assert.AreEqual("An opening curly bracket must not be followed by a blank line.", violationList[3]);
            Assert.AreEqual("An opening curly bracket must not be followed by a blank line.", violationList[4]);
            Assert.AreEqual("The class must have a documentation header.", violationList[5]);
            Assert.AreEqual("The class must have a documentation header.", violationList[6]);
            Assert.AreEqual("The file has no header, the header Xml is invalid, or the header is not located at the top of the file.", violationList[7]);
        }
        
        /// <summary>
        /// Tests the usage of wildcards in the project file
        /// </summary>
        [Test]
        public void WildCardTest()
        {
            var report = new ReportBuilder()
                            .WithProjectFiles(new List<string>() { WildCardProject });
            
            var result = ExecuteTest(report, null);
            Assert.AreEqual(10, result.Count, BasePath);
            Assert.AreEqual("26 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("AssemblyInfo.cs"), result[1]);
            Assert.IsTrue(result[2].EndsWith("AssemblyInfo.cs"), result[2]);
            Assert.IsTrue(result[3].EndsWith("ClassOne.cs"), result[3]);
            Assert.IsTrue(result[4].EndsWith("ClassOne.cs"), result[4]);
            Assert.IsTrue(result[5].EndsWith("ClassOne.cs"), result[5]);
            Assert.IsTrue(result[6].EndsWith("ClassOne.cs"), result[6]);
            Assert.IsTrue(result[7].EndsWith("ClassTwo.cs"), result[7]);
            Assert.IsTrue(result[8].EndsWith("ClassTwo.cs"), result[8]);
            Assert.IsTrue(result[9].EndsWith("ClassTwo.cs"), result[9]);
        }

        /// <summary>
        /// Checking the debug settings
        /// </summary>
        [Test]
        public void DebugTesting()
        {
            var list = new List<string>();
            var report = new ReportBuilder()
                            .WithProjectFiles(new List<string>() { WildCardProject })
                            .WithDebug(x => { list.Add(x); });
            
            ExecuteTest(report, null);
            Assert.IsTrue(list.Count > 0);
        }
        
        /// <summary>
        /// Removing duplicates during checks
        /// </summary>
        [Test]
        public void DedupeReportTest()
        {
            var report = new ReportBuilder()
                            .WithDedupe()
                            .WithProjectFiles(new List<string>() { WildCardProject });
            
            var result = ExecuteTest(report, null);
            Assert.AreEqual(4, result.Count, BasePath);
            Assert.AreEqual("8 violations encountered.", result[0]);
            Assert.IsTrue(result[1].EndsWith("AssemblyInfo.cs"), result[1]);
            Assert.IsTrue(result[2].EndsWith("ClassOne.cs"), result[2]);
            Assert.IsTrue(result[3].EndsWith("ClassTwo.cs"), result[3]);
        }
        
        /// <summary>
        /// Executes the test.
        /// </summary>
        /// <returns>
        /// The output from StyleCop event output
        /// </returns>
        /// <param name='builder'>
        /// Input report for testing
        /// </param>
        /// <param name='outputFile'>
        /// Output file.
        /// </param>
        private static IList<string> ExecuteTest(ReportBuilder builder, string outputFile)
        {
            var outputList = new List<string>();
            builder.WithOutputEventHandler((x, y) => { outputList.Add(((StyleCop.OutputEventArgs)y).Output); });
            builder.Create(outputFile);
            return outputList.OrderBy(value => value).ToList();
        }
        
        /// <summary>
        /// Joins all paths into a single path
        /// </summary>
        /// <returns>
        /// The complete path
        /// </returns>
        /// <param name='path'>
        /// Path base.
        /// </param>
        /// <param name='paths'>
        /// Paths to append.
        /// </param>
        private static string JoinAll(string path, params string[] paths)
        {
            string output = path;
            if (paths != null && paths.Length > 0)
            {
                foreach (var item in paths)
                {
                    output = Path.Combine(output, item);
                }
            }
            
            return output;
        }

        /// <summary>
        /// Gets the path to the test solution.
        /// </summary>
        /// <returns>
        /// The path to the test solution.
        /// </returns>
        private static string GetTestSolutionPath()
        {
            var d = new DirectoryInfo(".");

            // Move backwards to the root of the solution.
            while (d != null)
            {
                // Is the solution in this directory?
                if (d.GetFiles().FirstOrDefault(f => f.Extension == ".sln") !=
                    null)
                {
                    break;
                }

                d = d.Parent;
            }

            var r = string.Join(
                Path.DirectorySeparatorChar.ToString(),
                new string[] { d.FullName, "StyleCopCmd.Core.Test", "data", TestName });

            return r;
        }
    }
}
