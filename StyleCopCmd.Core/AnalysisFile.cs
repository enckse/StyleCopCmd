//------------------------------------------------------------------------------
// <copyright 
//  file="AnalysisFile.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core
{
    /// <summary>
    /// A file for analysis
    /// </summary>
    public sealed class AnalysisFile
    {
        /// <summary>
        /// Gets or sets the full path of the file
        /// </summary>
        public string File { get; set; } 

        /// <summary>
        /// Gets or sets the file type
        /// </summary>
        public FileType Type { get; set; } 
    }
}
