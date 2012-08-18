//------------------------------------------------------------------------------
// <copyright 
//  file="ReportStore.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    
    /// <summary>
    /// Report store, represents a memory-backed model of the source code to check styling for
    /// </summary>
    /// <exception cref='ArgumentNullException'>
    /// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
    /// </exception>
    /// <exception cref='ArgumentException'>
    /// Is thrown when an argument passed to a method is invalid.
    /// </exception>
    public class ReportStore
    {
        /// <summary>
        /// Constant file project for files not associated with a project
        /// </summary>
        internal const int FileProject = 0;
        
        /// <summary>
        /// The current project id count
        /// </summary>
        private int currentCount;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="StyleCopCmd.Core.ReportStore"/> class.
        /// </summary>
        public ReportStore()
        {
            this.currentCount = FileProject + 1;
            this.Projects = new Collection<Project>();
            this.SourceFiles = new Collection<SourceFile>();
            this.Projects.Add(new Project(FileProject, "Files"));
            this.Dedupe = false;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether to provide a dedupe on projects and files
        /// </summary>
        /// <value>
        /// <c>true</c> if dedupe; otherwise, <c>false</c>.
        /// </value>
        public bool Dedupe
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets the projects in the store
        /// </summary>
        /// <value>
        /// The projects.
        /// </value>
        public Collection<Project> Projects
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets the source files in the store
        /// </summary>
        /// <value>
        /// The source files.
        /// </value>
        public Collection<SourceFile> SourceFiles
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Adds the project.
        /// </summary>
        /// <returns>
        /// The project identifier
        /// </returns>
        /// <param name='location'>
        /// Location of the project
        /// </param>
        /// <exception cref='ArgumentNullException'>
        /// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
        /// </exception>
        public int AddProject(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                throw new ArgumentNullException("location");
            }
            
            if (this.Dedupe)
            {
                var matches = this.Projects.Where(value => value.Location == location);
                if (matches.Any())
                {
                    return matches.First().Id;
                }
            }
            
            this.Projects.Add(new Project(this.currentCount, location));
            return this.currentCount++;
        }
        
        /// <summary>
        /// Adds the source file to the store.
        /// </summary>
        /// <param name='path'>
        /// Path to the file
        /// </param>
        /// <param name='project'>
        /// Project id reference
        /// </param>
        /// <exception cref='ArgumentException'>
        /// Is thrown when an argument passed to a method is invalid.
        /// </exception>
        /// <exception cref='ArgumentNullException'>
        /// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
        /// </exception>
        public void AddSourceFile(string path, int project)
        {
            if (!this.Projects.Where(value => value.Id == project).Any())
            {
                throw new ArgumentException("Invalid project id", "project");
            }
            
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            
            if (this.Dedupe)
            {
                var matches = this.SourceFiles.Where(value => value.ProjectId == project && value.Path == path);
                if (matches.Any())
                {
                    return;   
                }
            }
            
            this.SourceFiles.Add(new SourceFile(project, path));
        }
    }
}