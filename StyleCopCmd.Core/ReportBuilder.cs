//------------------------------------------------------------------------------
// <copyright 
//  file="ReportBuilder.cs" 
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
namespace StyleCopCmd.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using System.Xml.Xsl;
    using StyleCop;

    /// <summary>
    /// This class assists in building a StyleCop report.
    /// </summary>
    public class ReportBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StyleCopCmd.Core.ReportBuilder"/> class.
        /// </summary>
        public ReportBuilder()
        {
            this.Report = new ReportStore();
            this.Settings = new ReportSettings();
        }

        /// <summary>
        /// Occurs when the stle processor outputs a message.
        /// </summary>
        public event EventHandler<OutputEventArgs> OutputGenerated;

        /// <summary>
        /// Occurs when the stylecop processor encounters a violation
        /// </summary>
        public event EventHandler<ViolationEventArgs> ViolationEncountered;

        /// <summary>
        /// Gets or sets the underlying memory report store
        /// </summary>
        /// <value>
        /// The report store
        /// </value>
        private ReportStore Report { get; set; }

        /// <summary>
        /// Gets or sets the underlying report settings
        /// </summary>
        /// <value>
        /// The report settings
        /// </value>
        private ReportSettings Settings { get; set; }

        /// <summary>
        /// Perform the checking with debugging statements enabled
        /// </summary>
        /// <param name="enableDebug">True to enable debug statements</param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithDebug(bool enableDebug)
        {
            this.Settings.EnableDebug = enableDebug;
            return this;
        }
        
        /// <summary>
        /// Enables eliminating the duplicates.
        /// </summary>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithDedupe()
        {
            return this.WithDedupe(true);
        }
        
        /// <summary>
        /// Enables eliminating the duplicates.
        /// </summary>
        /// <param name="dedupe">True to enable dedupe of projects/files</param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithDedupe(bool dedupe)
        {
            this.Report.Dedupe = dedupe;
            return this;
        }
        
        /// <summary>
        /// Adds Visual Studio Solution files to check. Visual Studio 2008
        /// is supported.
        /// </summary>
        /// <param name="solutionsFiles">
        /// A list of fully-qualified paths to Visual Studio solutions files.
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithSolutionsFiles(
            IList<string> solutionsFiles)
        {
            this.Settings.SolutionFiles = solutionsFiles;
            return this;
        }

        /// <summary>
        /// Adds Visual Studio Project files to check. Visual Studio 2008
        /// is supported.
        /// </summary>
        /// <param name="projectFiles">
        /// A list of fully-qualified paths to Visual Studio project files.
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithProjectFiles(
            IList<string> projectFiles)
        {
            this.Settings.ProjectFiles = projectFiles;
            return this;
        }

        /// <summary>
        /// Adds directories to check.
        /// </summary>
        /// <param name="directories">
        /// A list of fully-qualifieid paths to directories.
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithDirectories(
            IList<string> directories)
        {
            this.Settings.Directories = directories;
            return this;
        }

        /// <summary>
        /// Adds files to check.
        /// </summary>
        /// <param name="files">
        /// A list of fully-qualified paths to files.
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithFiles(
            IList<string> files)
        {
            this.Settings.Files = files;
            return this;
        }

        /// <summary>
        /// Adds a list of patterns to ignore when checking files.
        /// </summary>
        /// <param name="ignorePatterns">
        /// A list of regular expression patterns to ignore when checking
        /// files.
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithIgnorePatterns(
            IList<string> ignorePatterns)
        {
            this.Settings.IgnorePatterns = ignorePatterns;
            return this;
        }

        /// <summary>
        /// Specifies to recurse directories.
        /// </summary>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithRecursion()
        {
            return this.WithRecursion(true);
        }

        /// <summary>
        /// Specifies to recurse directories.
        /// </summary>
        /// <param name="withRecursion">
        /// True to recurse; otherwise false.
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithRecursion(bool withRecursion)
        {
            this.Settings.RecursionEnabled = withRecursion;
            return this;
        }

        /// <summary>
        /// Adds a list of processor symbols to use when performing the check.
        /// </summary>
        /// <param name="processorSymbols">
        /// A list of processor symboles to use when performing the check.
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithProcessorSymbols(
            IList<string> processorSymbols)
        {
            this.Settings.ProcessorSymbols = processorSymbols;
            return this;
        }

        /// <summary>
        /// Adds a StyleCop settings file to use when performing the check.
        /// </summary>
        /// <param name="styleCopSettingsFile">
        /// A fully-qualified path to a StyleCop settings file to use.
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithStyleCopSettingsFile(
            string styleCopSettingsFile)
        {
            this.Settings.StyleCopSettingsFile = styleCopSettingsFile;
            return this;
        }

        /// <summary>
        /// Adds an event handler for when output is generated by the 
        /// StyleCop processor.
        /// </summary>
        /// <param name="outputEventHandler">
        /// The event handler to add.
        /// </param>
        /// <returns>
        /// This ReportBuilder.
        /// </returns>
        public ReportBuilder WithOutputEventHandler(
            EventHandler<OutputEventArgs> outputEventHandler)
        {
            this.OutputGenerated += outputEventHandler;
            return this;
        }
    
        /// <summary>
        /// Add a violation event handler for handling any violations encountered during StyleCop processing
        /// </summary>
        /// <returns>
        /// The violation event handler.
        /// </returns>
        /// <param name='violationHandler'>
        /// Violation handler.
        /// </param>
        public ReportBuilder WithViolationEventHandler(EventHandler<ViolationEventArgs> violationHandler)
        {
            this.ViolationEncountered = violationHandler;
            return this;
        }
    
        /// <summary>
        /// Creates a StyleCop report.
        /// </summary>
        /// <param name="outputXmlFile">
        /// The fully-qualified path to write the output of the report to.
        /// </param>
        public void Create(string outputXmlFile)
        {
            this.WriteDebugLine("Setting configuration symbols");

            // Create a StyleCop configuration specifying the configuration
            // symbols to use for this report.
            var cfg = new Configuration(
                this.Settings.ProcessorSymbols != null
                    ?
                        this.Settings.ProcessorSymbols.ToArray()
                    :
                        null);

            this.WriteDebugLine("Creating console for checking");
            
            // Create a new StyleCop console used to do the check.
            var scc = new StyleCopConsole(
                this.Settings.StyleCopSettingsFile,
                true,
                GetViolationsFile(outputXmlFile),
                this.Settings.AddInDirectories,
                true);

            this.WriteDebugLine("Setting files to analyze");
            this.SetFiles();
            this.WriteDebugLine("Preparing code projects");

            // Create a list of code projects from the data set.
            var cps = this.Report.Projects.Select(
                r => new CodeProject(
                         r.Id,
                         r.Location,
                         cfg)).ToList();
   
           this.WriteDebugLine("Preparing source code objects");

            // Add the source code files to the style cop checker
            foreach (var f in this.Report.SourceFiles)
            {
                var cp = cps.Where(i => i.Key == f.ProjectId).First();
                scc.Core.Environment.AddSourceCode(
                    cp,
                    f.Path,
                    null);
            }
            
            if (this.OutputGenerated != null)
            {
                scc.OutputGenerated += this.OutputGenerated;
            }
            
            if (this.ViolationEncountered != null)
            {
                scc.ViolationEncountered += this.ViolationEncountered;
            }
   
                this.WriteDebugLine("Validating spell checking situation");

            // The spell checking relies on office, is will cause issues in linux
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                foreach (var file in Directory.GetFiles(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)))
                {
                    if (file.Contains("mssp7en."))
                    {
                        throw new InvalidOperationException("The spell check library was found, this platform does not support spell checking rules (e.g. SA1650)");
                    }
                }
            }
            
            this.WriteDebugLine("Starting check");
            scc.Start(cps, true);
            this.WriteDebugLine("Checking done");
            if (this.OutputGenerated != null)
            {
                scc.OutputGenerated -= this.OutputGenerated;
            }
            
            if (this.ViolationEncountered != null)
            {
                scc.ViolationEncountered -= this.ViolationEncountered;
            }
        }

        /// <summary>
        /// Gets the path of the violations file to use.
        /// </summary>
        /// <param name="outputXmlFile">
        /// The output XML file.
        /// </param>
        /// <returns>The path of the violations file.</returns>
        private static string GetViolationsFile(string outputXmlFile)
        {
            return string.IsNullOrEmpty(outputXmlFile) ? null : string.Format(CultureInfo.CurrentCulture, "{0}.xml", Path.GetFileNameWithoutExtension(outputXmlFile));            
        }
        
        /// <summary>
        /// Expands the directories that contain wildcards
        /// </summary>
        /// <returns>
        /// The directories that have been expanded.
        /// </returns>
        /// <param name='paths'>
        /// Paths to expand.
        /// </param>
        private IEnumerable<string> ExpandDirectories(string[] paths)
        {
            foreach (var path in paths)
            {
                this.WriteDebugLine("Expanding wildcard path: " + path);

                // Only expand wildcards
                if (path.Contains("*"))
                {
                    var parts = path.Split(Path.DirectorySeparatorChar);
                    string basePath = Directory.GetDirectoryRoot(path);

                    // TODO: Network shares in windows? Will that even work outside of this?
                    // Supporting wildcard in windows and linux
                    // Start point in windows needs to be adjust by 1 when the root is a drive (e.g. C:)
                    var startIndex = 0;
                    if (basePath.Length >= 2)
                    {
                        if (basePath[0] >= 'A' && basePath[0] <= 'Z' && basePath[1] == ':')
                        {
                            startIndex = 1;
                        }
                    }

                    for (int index = startIndex; index < parts.Length; index++)
                    {
                        string part = parts[index];
                        
                        // On expansion, the remaining parts needs to be applied to each expanded directory
                        IList<string> subParts = new List<string>();
                        for (int subIndex = index + 1; subIndex < parts.Length; subIndex++)
                        {
                            subParts.Add(parts[subIndex]);   
                        }
                        
                        // Subdirectory for any expansions
                        var subDir = string.Join(Path.DirectorySeparatorChar.ToString(), subParts.ToArray());
                        if (part.Contains("*"))
                        {
                            // Expand all child directories including the current directory
                            if (part == "**")
                            {
                                yield return Path.Combine(basePath, subDir);
                                foreach (var directory in Directory.GetDirectories(basePath))
                                {
                                    yield return Path.Combine(directory, subDir);
                                }
                            }
                            else
                            {
                                // Single wildcards, look for any matching files and sub dirs
                                foreach (var directory in Directory.GetDirectories(basePath, part))
                                {
                                    yield return Path.Combine(directory, subDir);
                                }
                            
                                foreach (var file in Directory.GetFiles(basePath, part))
                                {
                                    yield return Path.Combine(file, subDir);
                                }
                            }
                            
                            break;
                        }
                        else
                        {
                            basePath = Path.Combine(basePath, part);
                        }
                    }
                }
                else
                {
                 yield return path;   
                }
            }
        }
        
        /// <summary>
        /// Add the directory's source files to the list of
        /// files that StyleCop checks.
        /// </summary>
        /// <param name="path">
        /// The fully-qualified path to the directory to add.
        /// </param>
        private void AddDirectory(string path)
        {
            this.WriteDebugLine("Adding directory: " + path);
            var recurse = this.Settings.RecursionEnabled
                              ?
                                  SearchOption.AllDirectories
                              :
                                  SearchOption.TopDirectoryOnly;

            var files = Directory.GetFiles(
                path,
                "*.cs",
                recurse);
        
            var pr = this.Report.AddProject(path);

            // Add the source files.
            Array.ForEach(
                files,
                f => this.AddFile(
                         f,
                         pr));
        }

        /// <summary>
        /// Add the given file to the list of files to
        /// be checked by StyleCop.
        /// </summary>
        /// <param name="filePath">
        /// The fully-qualified path of the file to add.
        /// </param>
        /// <param name="project">
        /// The project id for this file.
        /// </param>
        private void AddFile(string filePath, int? project)
        {
            this.WriteDebugLine("Adding file: " + filePath);
            if (this.Settings.IgnorePatterns != null)
            {
                // Check to see if this file should be ignored.
                if (this.Settings.IgnorePatterns.FirstOrDefault(
                        fp => Regex.IsMatch(
                                  Path.GetFileName(filePath),
                                  fp)) != null)
                {
                    return;
                }
            }
            
            var pr = project.HasValue ? project.Value : ReportStore.FileProject;

            this.Report.AddSourceFile(
                Path.GetFullPath(filePath),
                pr);
        }

        /// <summary>
        /// Adds a Visual Studio solution file and add its
        /// CSharp projects to the list of projects to be checked
        /// by StyleCop.
        /// </summary>
        /// <param name="solutionFilePath">
        /// The fully-qualified path to a Visual Studio solution file.
        /// </param>
        private void AddSolutionFile(string solutionFilePath)
        {
            this.WriteDebugLine("Adding solution file: " + solutionFilePath);

            // Get a list of the CSharp projects in the solutions file
            // and parse the project files.
            var sfin = File.ReadAllText(solutionFilePath);
            var smatches = Regex.Matches(
                sfin,
                @"^Project\(.*?\) = "".*?"", ""(?<ppath>.*?.csproj)""",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);
            foreach (Match sm in smatches)
            {
                var mstring = sm.Groups["ppath"].Value.Replace(@"\", Path.DirectorySeparatorChar.ToString());
                var ppath =
                    Path.GetFullPath(
                        Path.GetDirectoryName(
                            Path.GetFullPath(solutionFilePath)))
                    + Path.DirectorySeparatorChar.ToString() + mstring;
                this.AddProjectFile(ppath);
            }
        }

        /// <summary>
        /// Adds a Visual Studio CSharp project file and
        /// adds its source files to the list of files to be 
        /// checked by StyleCop.
        /// </summary>
        /// <param name="projectFilePath">
        /// The fully-qualified path to the project file.
        /// </param>
        private void AddProjectFile(string projectFilePath)
        {
            this.WriteDebugLine("Adding project file: " + projectFilePath);

            // Add a new project row.
            var pr = this.Report.AddProject(projectFilePath);
            
            var pf = XDocument.Load(projectFilePath);

            // Get the source files that are not auto-generated.
            var cnodes =
                pf.Root.Descendants().Where(
                    d => d.Name.LocalName == "Compile" &&
                         d.Attribute(XName.Get("Include")).Value.EndsWith(
                             "cs",
                             StringComparison.CurrentCultureIgnoreCase) &&
                         d.Elements().SingleOrDefault(
                             c =>
                             c.Name.LocalName == "AutoGen" &&
                             c.Value == "True") ==
                         null);

            // Add the source files.
            foreach (var n in cnodes)
            {
                var fpath = Path.Combine(
                    Path.GetFullPath(Path.GetDirectoryName(projectFilePath)), 
                    n.Attribute(XName.Get("Include")).Value.Replace(@"\", Path.DirectorySeparatorChar.ToString()));

                if (!File.Exists(fpath) && fpath.Contains("*"))
                {
                    // Could be a wildcard selector
                    this.DoWildCardAdd(fpath, pr);
                    continue;
                }
                
                this.AddFile(
                    fpath,
                    pr);
            }
        }
        
        /// <summary>
        /// Do a wildcard-based add of files
        /// </summary>
        /// <param name='path'>
        /// Base file path
        /// </param>
        /// <param name='project'>
        /// Project id
        /// </param>
        private void DoWildCardAdd(string path, int project)
        {
            this.WriteDebugLine("Adding wildcard path: " + path);
            var paths = new string[] { path };
            do
            {
                paths = this.ExpandDirectories(paths).ToArray();
            }
            while (paths.Where(x => x.Contains("*")).Any());
            
            // At this point, all wildcards have been expanded
            foreach (var item in paths)
            {
                this.WriteDebugLine("Wild card path expanded to: " + item);
                if (File.Exists(item))
                {
                    this.AddFile(item, project);
                }
            }
        }

        /// <summary>
        /// Write a line to the console for debug purposes
        /// </summary>
        /// <param name="message">Message to write</param>
        private void WriteDebugLine(string message)
        {
            this.Settings.Write(message);
        }

        /// <summary>
        /// Setting the files for analysis
        /// </summary>
        private void SetFiles()
        {
            foreach (var file in this.Settings.GetAllFiles())
            {
                switch (file.Type)   
                {
                    case FileType.Solution:
                        this.AddSolutionFile(file.File);
                        break;
                    case FileType.Project:
                        this.AddProjectFile(file.File);
                        break;
                    case FileType.Directory:
                        this.AddDirectory(file.File);
                        break;
                    case FileType.File:
                        this.AddFile(file.File, null);
                        break;
                }
            }
        }
    }
}
