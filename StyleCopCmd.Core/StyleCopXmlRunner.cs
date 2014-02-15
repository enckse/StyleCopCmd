//------------------------------------------------------------------------------
// <copyright 
//  file="StyleCopXmlRunner.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml.Linq;
    using StyleCop;

    /// <summary>Performs analysis and only outputs the violation XML</summary>
    public sealed class StyleCopXmlRunner : StyleCopRunner
    {
        /// <summary>File to write output to</summary>
        private readonly string outputFile;

        /// <summary>Input settings file</summary>
        private readonly string settingsPath;

        /// <summary>Method (reflective) call to cleanup attributes similar to StyleCop</summary>
        private System.Reflection.MethodInfo method = null;

        /// <summary>File writer used to output the results</summary>
        private System.IO.StreamWriter write = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleCopXmlRunner" /> class.
        /// </summary>
        /// <param name="settings">Settings file</param>
        /// <param name="outputFileName">Output file</param>
        /// <param name="addInPaths">Add-in paths to use</param>
        public StyleCopXmlRunner(string settings, string outputFileName, ICollection<string> addInPaths)
        {
            Param.Ignore(settings);
            Param.Ignore(outputFileName);
            Param.Ignore(addInPaths);

            this.settingsPath = settings;
            if (this.outputFile == null)
            {
                this.outputFile = "StyleCopViolations.xml";
            }
            else
            {
                this.outputFile = outputFileName;
            }

            this.Core = new StyleCopCore(null, null);
            this.CaptureViolations = false;
            this.Core.Initialize(addInPaths, true);
            this.Core.WriteResultsCache = true;
            this.method = typeof(StyleCopRunner).GetMethod("CreateSafeSectionName", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        }

        /// <summary>
        /// Start processing
        /// </summary>
        /// <param name="projects">Projects to analyze</param>
        /// <param name="fullAnalyze">True to perform a full analysis</param>
        /// <returns>Indicates if errors were encountered</returns>
        public bool Start(IList<CodeProject> projects, bool fullAnalyze)
        {
            Param.RequireNotNull(projects, "projects");
            Param.Ignore(fullAnalyze);

            bool error = false;
            string errorMessage = null;
            try
            {
                Settings mergedSettings = null;
                Settings localSettings = null;
                if (this.settingsPath != null)
                {
                    localSettings = this.Core.Environment.GetSettings(this.settingsPath, false);
                    if (localSettings != null)
                    {
                        SettingsMerger merger = new SettingsMerger(localSettings, this.Core.Environment);
                        mergedSettings = merger.MergedSettings;
                    }
                }

                foreach (CodeProject project in projects)
                {
                    Settings settingsToUse = mergedSettings;
                    if (settingsToUse == null)
                    {
                        settingsToUse = this.Core.Environment.GetProjectSettings(project, true);
                    }

                    if (settingsToUse != null)
                    {
                        project.Settings = settingsToUse;
                        project.SettingsLoaded = true;
                    }
                }

                this.Reset();

                // Delete the output file if it already exists.
                if (!string.IsNullOrEmpty(this.outputFile))
                {
                    this.Core.Environment.RemoveAnalysisResults(this.outputFile);
                }

                this.Core.ViolationEncountered += this.Violation;
                using (var outputStream = new System.IO.StreamWriter(this.outputFile))
                {
                    this.write = outputStream;
                    outputStream.WriteLine("<StyleCopViolations>");
                    if (fullAnalyze)
                    {
                        this.Core.FullAnalyze(projects);
                    }
                    else
                    {
                        this.Core.Analyze(projects);
                    }

                    outputStream.WriteLine("</StyleCopViolations>");
                }
            }
            catch (System.IO.IOException ioex)
            {
                errorMessage = ioex.Message;
                error = true;
            }
            catch (System.Security.SecurityException secex)
            {
                errorMessage = secex.Message;
                error = true;
            }
            catch (UnauthorizedAccessException unauthex)
            {
                errorMessage = unauthex.Message;
                error = true;
            }

            if (error)
            {           
                this.OnOutputGenerated(new OutputEventArgs(string.Format("Analysis error: {0}", errorMessage)));
            }

            return !error;
        }

        /// <summary>
        /// Get rule information for output
        /// </summary>
        /// <param name="element">Element to add to</param>
        /// <param name="e">Violation information</param>
        private static void GetRuleInfo(XElement element, ViolationEventArgs e)
        {
            element.Add(new XAttribute("RuleNamespace", e.Violation.Rule.Namespace));
            element.Add(new XAttribute("Rule", e.Violation.Rule.Name));
            element.Add(new XAttribute("RuleId", e.Violation.Rule.CheckId));
        }

        /// <summary>
        /// Get location information for output
        /// </summary>
        /// <param name="element">Element to add to</param>
        /// <param name="e">Violation information</param>
        private static void GetLocationInfo(XElement element, ViolationEventArgs e)
        {
            element.Add(new XAttribute("StartLine", e.Location.Value.StartPoint.LineNumber));
            element.Add(new XAttribute("StartColumn", e.Location.Value.StartPoint.IndexOnLine));
            element.Add(new XAttribute("EndLine", e.Location.Value.EndPoint.LineNumber));
            element.Add(new XAttribute("EndColumn", e.Location.Value.EndPoint.IndexOnLine));
        }

        /// <summary>
        /// Violation encountered
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Violation data</param>
        private void Violation(object sender, ViolationEventArgs e)
        {
            Param.Ignore(sender, e);
            lock (this)
            {
                if (this.write != null)
                {
                    var violation = new XElement("Violation");
                    if (e.Element != null)
                    {
                        var name = e.Element.FullyQualifiedName;
                        if (name != null && this.method != null)
                        {
                            var safe = this.method.Invoke(null, new object[] { name });
                            name = safe == null ? name : safe.ToString();
                        }

                        violation.Add(new XAttribute("Section", name));
                    }

                    violation.Add(new XAttribute("LineNumber", e.LineNumber));
                    if (e.Location != null)
                    {
                        GetLocationInfo(violation, e);
                    }

                    SourceCode sourceCode = e.SourceCode;
                    if (sourceCode == null && e.Element != null && e.Element.Document != null)
                    {
                        sourceCode = e.Element.Document.SourceCode;
                    }

                    if (sourceCode != null)
                    {
                        violation.Add(new XAttribute("Source", sourceCode.Path));
                    }

                    GetRuleInfo(violation, e);
                    violation.SetValue(e.Message);
                    this.write.WriteLine(violation.ToString());
                }
            }
        }
    }
}