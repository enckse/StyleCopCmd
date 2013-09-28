//------------------------------------------------------------------------------
// <copyright 
//  file="ReportSettings.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Report analysis settings
    /// </summary>
    public class ReportSettings
    {
        /// <summary>
        /// Gets or sets a list of Visual Studio Solution files to check.
        /// </summary>
        public IList<string> SolutionFiles { get; set; }

        /// <summary>
        /// Gets or sets a list of Visual Studio Project files to check.
        /// </summary>
        public IList<string> ProjectFiles { get; set; }

        /// <summary>
        /// Gets or sets a list of directories to check.
        /// </summary>
        public IList<string> Directories { get; set; }

        /// <summary>
        /// Gets or sets a list of files to check.
        /// </summary>
        public IList<string> Files { get; set; }

        /// <summary>
        /// Gets or sets a list of regular expression patterns used
        /// to ignore files (if a file name matches any of the patterns, the
        /// file is not checked).
        /// </summary>
        public IList<string> IgnorePatterns { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not directories are 
        /// recursed.
        /// </summary>
        public bool RecursionEnabled { get; set; }

        /// <summary>
        /// Gets or sets a list of processor symbols (ex. DEBUG, CODE_ANALYSIS)
        /// to be used by StyleCop.
        /// </summary>
        public IList<string> ProcessorSymbols { get; set; }

        /// <summary>
        /// Gets or sets the path to the StyleCop setting file to use.
        /// </summary>
        public string StyleCopSettingsFile { get; set; }

        /// <summary>
        /// Gets or sets a list of directories used by StyleCop to search for 
        /// add-ins. Currently not available from the command line
        /// </summary>
        public IList<string> AddInDirectories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable debug statements
        /// </summary>
        public bool EnableDebug { get; set; }

        /// <summary>
        /// Get all files stored in the settings for analysis
        /// </summary>
        /// <returns>The set of all files for analysis with file name and type</returns>
        public IEnumerable<AnalysisFile> GetAllFiles()
        {
            this.Write("Processing solution files");
            foreach (var solution in GetFiles(this.SolutionFiles, FileType.Solution))
            {
                yield return solution;
            }

            this.Write("Processing project files");
            foreach (var proj in GetFiles(this.ProjectFiles, FileType.Project))
            {
                yield return proj;
            }

            this.Write("Processing directories");
            foreach (var dir in GetFiles(this.Directories, FileType.Directory))
            {
                yield return dir;
            }

            this.Write("Processing files");
            foreach (var file in GetFiles(this.Files, FileType.File))
            {
                yield return file;
            }
        }

        /// <summary>
        /// Write a debug message (if enabled)
        /// </summary>
        /// <param name="message">Message to write</param>
        internal void Write(string message)
        {
            if (this.EnableDebug)
            {
                System.Console.WriteLine(message);
            }
        }

        /// <summary>
        /// Get a set of files and add a descriptor type
        /// </summary>
        /// <param name="files">File list to get files from</param>
        /// <param name="type">File type to add to any files</param>
        /// <returns>The set of none empty files in the given set marked with a type</returns>
        private static IEnumerable<AnalysisFile> GetFiles(IList<string> files, FileType type)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (!string.IsNullOrEmpty(file))
                    {
                        yield return new AnalysisFile() { File = file, Type = type };
                    }
                }
            }
        }
    }
}
