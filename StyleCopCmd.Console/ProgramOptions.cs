//------------------------------------------------------------------------------
// <copyright
//  file="ProgramOptions.cs"
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Console
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Program options
    /// </summary>
    internal sealed class ProgramOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramOptions" /> class.
        /// </summary>
        internal ProgramOptions()
        {
            this.SolutionFiles = new List<string>();
            this.ProjectFiles = new List<string>();
            this.IgnorePatterns = new List<string>();
            this.Directories = new List<string>();
            this.Files = new List<string>();
            this.Symbols = new List<string>();
            this.Addins = new List<string>();
            this.ProjectUnloads = new List<string>();
            this.StyleCopSettings = null;
            this.OutputXml = null;
            this.Recurse = false;
            this.NeedHelp = false;
            this.Violations = false;
            this.Quiet = false;
            this.Dedupe = false;
            this.WithDebug = false;
            this.MaxViolations = null;
            this.Generator = null;
            this.CurrentOp = null;
            this.Optionals = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Gets or sets the optional inputs
        /// </summary>
        internal IDictionary<string, object> Optionals { get; set; }

        /// <summary>
        /// Gets or sets the solution files to read
        /// </summary>
        internal IList<string> SolutionFiles { get; set; }

        /// <summary>
        /// Gets or sets the project files to read
        /// </summary>
        internal IList<string> ProjectFiles { get; set; }

        /// <summary>
        /// Gets or sets the file ignore patterns
        /// </summary>
        internal IList<string> IgnorePatterns { get; set; }

        /// <summary>
        /// Gets or sets the directories to search
        /// </summary>
        internal IList<string> Directories { get; set; }

        /// <summary>
        /// Gets or sets the files to read
        /// </summary>
        internal IList<string> Files { get; set; }

        /// <summary>
        /// Gets or sets the symbols to include
        /// </summary>
        internal IList<string> Symbols { get; set; }

        /// <summary>
        /// Gets or sets the add-ins directories to search
        /// </summary>
        internal IList<string> Addins { get; set; }

        /// <summary>
        /// Gets or sets the patterns to unload projects from a solution
        /// </summary>
        internal IList<string> ProjectUnloads { get; set; }

        /// <summary>
        /// Gets or sets the settings file
        /// </summary>
        internal string StyleCopSettings { get; set; }

        /// <summary>
        /// Gets or sets the output file name
        /// </summary>
        internal string OutputXml { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to recursively search on directories
        /// </summary>
        internal bool Recurse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show help
        /// </summary>
        internal bool NeedHelp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display violation detail
        /// </summary>
        internal bool Violations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display no outputs
        /// </summary>
        internal bool Quiet { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to remove duplicate file processing
        /// </summary>
        internal bool Dedupe { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use debugging output
        /// </summary>
        internal bool WithDebug { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to report a non-zero exit on violation
        /// </summary>
        internal int? MaxViolations { get; set; }

        /// <summary>
        /// Gets or sets the generator requested
        /// </summary>
        internal string Generator { get; set; }

        /// <summary>
        /// Gets or sets the current parsing operation
        /// </summary>
        internal string CurrentOp { get; set; }

        /// <summary>
        /// Validates the option set (determine if help should be display)
        /// </summary>
        internal void Validate()
        {
            this.NeedHelp = this.NeedHelp || (this.SolutionFiles.Count == 0 && this.ProjectFiles.Count == 0 && this.Directories.Count == 0 && this.Files.Count == 0);
        }
    }
}
