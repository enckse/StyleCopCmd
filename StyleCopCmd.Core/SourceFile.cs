//------------------------------------------------------------------------------
// <copyright 
//  file="SourceFile.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core
{
    /// <summary>
    /// Source file represented in memory.
    /// </summary>
    public sealed class SourceFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StyleCopCmd.Core.SourceFile"/> class.
        /// </summary>
        /// <param name='project'>
        /// Project reference
        /// </param>
        /// <param name='path'>
        /// Path to the file
        /// </param>
        public SourceFile(int project, string path)
        {
            this.ProjectId = project;
            this.Path = path;
        }
        
        /// <summary>
        /// Gets the project id.
        /// </summary>
        /// <value>
        /// The project id.
        /// </value>
        public int ProjectId
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>
        /// The path to the file
        /// </value>
        public string Path
        {
            get;
            private set;
        }
    }
}