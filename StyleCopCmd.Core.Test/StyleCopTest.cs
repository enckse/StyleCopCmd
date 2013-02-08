//------------------------------------------------------------------------------
// <copyright 
//  file="StyleCopTest.cs" 
//  company="Schley Andrew Kutz">
//  Copyright (c) Schley Andrew Kutz. All rights reserved.
// </copyright>
// <authors>
//   <author>Schley Andrew Kutz</author>
// </authors>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core.Test
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;
    
    /// <summary>Style Cop as a whole, testing</summary>
    [TestFixture]
    public class StyleCopTest
    {
        /// <summary>Testing the solution against itself!</summary>
        [Test]
        public void NoViolations()
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
          
            var path = Path.Combine(d.FullName, "StyleCopCmd.sln");
            
            var report = new ReportBuilder().WithSolutionsFiles(new List<string>() { path });
            var outputList = new List<string>();
            report.WithOutputEventHandler((x, y) => { outputList.Add(((StyleCop.OutputEventArgs)y).Output); });
            report.Create(null);
            var result = outputList.OrderBy(value => value).ToList();
            Assert.AreEqual(12, result.Count, "StyleCopCmd.sln");
            Assert.AreEqual("No violations encountered", result[0]);
        }
    }
}
