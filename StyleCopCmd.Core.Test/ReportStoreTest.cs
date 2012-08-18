//------------------------------------------------------------------------------
// <copyright 
//  file="ReportStoreTest.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core.Test
{ 
    using System;
    using NUnit.Framework;
 
    /// <summary>
    /// Report store testing for adding to the report store.
    /// </summary>
    [TestFixture]
    public class ReportStoreTest
    {
        /// <summary>
        /// Adding a project to the store
        /// </summary>
        [Test]
        public void AddProject()
        {
            ReportStore store = new ReportStore();
            Assert.AreEqual(1, store.AddProject("test"));
            Assert.AreEqual(2, store.AddProject("test"));
        }
        
        /// <summary>
        /// Adding a file.
        /// </summary>
        [Test]
        public void AddFile()
        {
            ReportStore store = new ReportStore();
            store.AddSourceFile("test", 0);
            store.AddSourceFile("test", 0);
        }
        
        /// <summary>
        /// Adds a file and a project
        /// </summary>
        [Test]
        public void AddFileAndProject()
        {
            ReportStore store = new ReportStore();
            store.AddSourceFile("test", store.AddProject("test"));
        }
        
        /// <summary>
        /// Invalid project argument
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidProject()
        {
            ReportStore store = new ReportStore();
            store.AddProject(null);
        }
        
        /// <summary>
        /// Invalid project reference id
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidProjectReference()
        {
            ReportStore store = new ReportStore();
            store.AddSourceFile("test", 1);
        }
        
        /// <summary>
        /// Invalid file path argument
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidFile()
        {
            ReportStore store = new ReportStore();
            store.AddSourceFile(null, 0);
        }
    }
}