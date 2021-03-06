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
        /// Occurs when the style processor outputs a message.
        /// </summary>
        public event EventHandler<OutputEventArgs> OutputGenerated;

        /// <summary>
        /// Occurs when the StyleCop processor encounters a violation
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
        /// Prints file/assembly information to the output action (assuming it is set)
        /// </summary>
        /// <param name="outputAction">Output action to write to</param>
        /// <param name="externalTypes">Additional types to print output for</param>
        public static void PrintFileInformation(Action<string> outputAction, params Type[] externalTypes)
        {
            // Nothing to output to
            if (outputAction == null)
            {
                return;
            }

            if (externalTypes != null && externalTypes.Any())
            {
                foreach (var type in externalTypes)
                {
                    outputAction(GetAssemblyInfoForType(type));
                }
            }

            outputAction(GetAssemblyInfoForType(typeof(ReportBuilder)));
            outputAction(GetAssemblyInfoForType(typeof(StyleCop.StyleCopRunner)));
            outputAction(GetAssemblyInfoForType(typeof(StyleCop.CSharp.CsParser)));
            outputAction(GetAssemblyInfoForType(typeof(StyleCop.CSharp.DocumentationRules)));
        }

        /// <summary>
        /// Perform the checking with debugging statements enabled
        /// </summary>
        /// <param name="debugAction">Debug action to use</param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithDebug(Action<string> debugAction)
        {
            this.Settings.DebugAction = debugAction;
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
        /// <param name="dedupe">True to enable duplication removal of projects/files</param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithDedupe(bool dedupe)
        {
            this.Report.Dedupe = dedupe;
            return this;
        }
        
        /// <summary>
        /// Adds Visual Studio Solution files to check.
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
        /// Set of regular expression values to test to unload the projects (skip) when reading a solution
        /// </summary>
        /// <param name="unloads">
        /// List of unload regular expression patterns to use
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithProjectUnloads(IList<string> unloads)
        {
            this.Settings.ProjectUnloads = unloads;
            return this;
        }

        /// <summary>
        /// Adds Visual Studio Project files to check.
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
        /// A list of fully-qualified paths to directories.
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
        /// Specifies to recursively search directories.
        /// </summary>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithRecursion()
        {
            return this.WithRecursion(true);
        }

        /// <summary>
        /// Specifies to recursively search directories.
        /// </summary>
        /// <param name="withRecursion">
        /// True to recursively search; otherwise false.
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
        /// A list of processor symbols to use when performing the check.
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
        /// Adds an optional parameter to the settings
        /// </summary>
        /// <param name="key">
        /// Optional key setting name
        /// </param>
        /// <param name="value">
        /// Value to store
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder AddOptional(string key, object value)
        {
            if (this.Settings.OptionalValues == null)
            {
                this.Settings.OptionalValues = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            }

            this.Settings.OptionalValues[key] = value;
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
        /// Set optional add-in paths to for StyleCop to search
        /// </summary>
        /// <param name="addins">Additional add-in paths for StyleCop to search</param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithAddins(
            IList<string> addins)
        {
            this.Settings.AddInDirectories = addins;
            return this;
        }

        /// <summary>
        /// Creates a StyleCop report.
        /// </summary>
        /// <param name="runner">Runner to use</param>
        public void Create(RunnerBase runner)
        {
            if (runner == null)
            {
                throw new ArgumentNullException("runner");
            }

            runner.Set(
                this.Settings.ProcessorSymbols,
                this.Settings.OptionalValues,
                this.Settings.AddInDirectories,
                this.Settings.StyleCopSettingsFile,
                this.Settings.Write);

            this.WriteDebugLine("Setting configuration symbols");
            var cfg = runner.Configure();
            this.WriteDebugLine("Creating console for checking");
            runner.Initialize();
            this.WriteDebugLine("Setting files to analyze");
            this.SetFiles();
            this.WriteDebugLine("Preparing code projects");

            // Create a list of code projects from the data set.
            var cps = this.Report.Projects.Select(r => new CodeProject(r.Id, r.Location, cfg)).ToList();
            this.WriteDebugLine("Preparing source code objects");

            // Add the source code files to the style cop checker
            foreach (var f in this.Report.SourceFiles)
            {
                var cp = cps.Where(i => i.Key == f.ProjectId).First();
                runner.AddFile(cp, f.Path);
            }
            
            this.WriteDebugLine("Validating spell checking situation");

            // The spell checking relies on office, is will cause issues in linux
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix || System.Environment.OSVersion.Platform == System.PlatformID.MacOSX)
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
            runner.Start(cps, this.OutputGenerated, this.ViolationEncountered);
            this.WriteDebugLine("Checking done");
        }

        /// <summary>
        /// Prints assembly information for a given type
        /// </summary>
        /// <param name='type'>Type to get the assembly for</param>
        /// <returns>Simple assembly and version information string</returns>
        private static string GetAssemblyInfoForType(Type type)
        {
            var assembly = System.Reflection.Assembly.GetAssembly(type);
            var fileInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return string.Format(CultureInfo.CurrentCulture, "{0} - {1}", assembly.GetName().Name, fileInfo.FileVersion);
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
        private string[] ExpandDirectories(string[] paths)
        {
            List<string> pathsToReturn = new List<string>();
            foreach (var path in paths)
            {
                this.WriteDebugLine("Expanding wildcard path: " + path);

                // Only expand wildcards
                if (path.Contains("*"))
                {
                    var parts = path.Split(Path.DirectorySeparatorChar);
                    string basePath = Directory.GetDirectoryRoot(path);
                    this.WriteDebugLine("Using base path for wildcard path: " + basePath);

                    // Supporting wildcard in windows and linux
                    // Start point in windows needs to be adjusted by 1 when the root is a drive (e.g. C:)
                    // Windows: This will only work for directories based on directory lettering (no network paths)
                    // For network paths in Windows map the network path to a drive.
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
                        this.WriteDebugLine("Current part: " + part);
                        
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
                            this.WriteDebugLine("Path contains a wildcard");

                            // Expand all child directories including the current directory
                            if (part == "**")
                            {
                                this.WriteDebugLine("Path contains a double wildcard");
                                pathsToReturn.Add(Path.Combine(basePath, subDir));
                                foreach (var directory in Directory.GetDirectories(basePath))
                                {
                                    pathsToReturn.Add(Path.Combine(directory, subDir));
                                }
                            }
                            else
                            {
                                this.WriteDebugLine("Path contains only a single wildcard");

                                // Single wildcards, look for any matching files and sub dirs
                                foreach (var directory in Directory.GetDirectories(basePath, part))
                                {
                                    pathsToReturn.Add(Path.Combine(directory, subDir));
                                }
                            
                                foreach (var file in Directory.GetFiles(basePath, part))
                                {
                                    pathsToReturn.Add(Path.Combine(file, subDir));
                                }
                            }
                            
                            break;
                        }
                        else
                        {
                            basePath = Path.Combine(basePath, part);
                            this.WriteDebugLine("New base path: " + basePath);
                        }
                    }
                }
                else
                {
                    pathsToReturn.Add(path);
                }
            }
            
            return pathsToReturn.ToArray();
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
            var recurse = this.Settings.RecursionEnabled ? SearchOption.AllDirectories :
                                  SearchOption.TopDirectoryOnly;

            var files = Directory.GetFiles(path, "*.cs", recurse);
            var pr = this.Report.AddProject(path);

            // Add the source files.
            Array.ForEach(files, f => this.AddFile(f, pr));
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
            if (this.CheckIsFilteredOut(Path.GetFileName(filePath), this.Settings.IgnorePatterns))
            {
                return;
            }

            var pr = project.HasValue ? project.Value : ReportStore.FileProject;
            this.Report.AddSourceFile(Path.GetFullPath(filePath), pr);
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

                if (this.CheckIsFilteredOut(ppath, this.Settings.ProjectUnloads))
                {
                    continue;
                }

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
                paths = this.ExpandDirectories(paths);
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
            this.Settings.Write(typeof(ReportBuilder), message);
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

        /// <summary>
        /// Check if a value should be filtered out (using regular expressions)
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="filters">Possible filters</param>
        /// <returns>True if the value matches any of the input filters</returns>
        private bool CheckIsFilteredOut(string input, IList<string> filters)
        {
            if (filters != null && filters.Count > 0)
            {
                // Check to see if this file should be ignored.
                if (filters.Where(x => Regex.IsMatch(input, x)).Any())
                {
                    this.WriteDebugLine(string.Format("Input is being filtered out: {0}", input));
                    return true;
                }
            }

            return false;
        }
    }
}
