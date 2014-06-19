//------------------------------------------------------------------------------
// <copyright 
//  file="Program.cs" 
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
namespace StyleCopCmd.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using NDesk.Options;

    /// <summary>
    /// The entry-point class for this application.
    /// </summary>
    public static class Program
    {
        /// <summary>Optional list of parameter name</summary>
        private const string ListParameter = "list";

        /// <summary>
        /// The command-line options for this application.
        /// </summary>
        private static OptionSet opts = new OptionSet();

        /// <summary>
        /// Indicates if the program execute an analysis with violations
        /// </summary>
        private static bool hadViolation;

        /// <summary>
        /// Available generators
        /// </summary>
        private enum Generator
        {
            /// <summary>
            /// Default generator (currently console runner)
            /// </summary>
            Default,

            /// <summary>
            /// Maps to the console runner
            /// </summary>
            Console,

            /// <summary>
            /// XML runner (output only, no reporting)
            /// </summary>
            Xml
        }

        /// <summary>
        /// The entry-point method for this application.
        /// </summary>
        /// <param name="args">
        /// The command line arguments passed to this method.
        /// </param>
        private static void Main(string[] args)
        {
            IList<string> solutionFiles = new List<string>();
            IList<string> projectFiles = new List<string>();
            IList<string> ignorePatterns = new List<string>();
            IList<string> directories = new List<string>();
            IList<string> files = new List<string>();
            IList<string> symbols = new List<string>();
            IList<string> addins = new List<string>();
            IList<string> projectUnloads = new List<string>();
            string styleCopSettings = null;
            string outputXml = null;
            bool recurse = false;
            bool needHelp = false;
            bool violations = false;
            bool quiet = false;
            bool dedupe = false;
            bool withDebug = false;
            bool terminate = false;
            string generator = null;
            string currentOp = null;

            var optionals = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            opts = new OptionSet()
            {
                { "s=|solutionFiles=", "Solution files to check (.sln)", opt => { solutionFiles.Add(opt); } },
                { "p=|projectFiles=", "Project files to check (.csproj)", opt => { projectFiles.Add(opt); } },
                { "i=|ignoreFilePattern=", "Regular expression patterns to ignore files", opt => { ignorePatterns.Add(opt); } },
                { "d=|directories=", "Directories to check for files (.cs)", opt => { directories.Add(opt); } },
                { "f=|files=", "Files to check (.cs)", opt => { files.Add(opt); } },
                { "r|recurse", "Recursive directory search", opt => { recurse = opt != null; } },
                { "c=|styleCopSettingsFile=", "Use the given StyleCop settings file", opt => { styleCopSettings = opt; } },
                { "o=|outputXmlFile=", "The file the XML output is written to", opt => { outputXml = opt; } },
                { "x=|configurationSymbols=", "Configuration symbols to pass to StyleCop (ex. DEBUG, RELEASE)", opt => { symbols.Add(opt); } },
                { "v|violations", "Print all violation information instead of the summary", opt => { violations = true; } },
                { "q|quiet", "Do not print any information to console", opt => { quiet = true; } },
                { "?|help", "Print the usage information", opt => { needHelp = true; } },
                { "e|eliminate", "Eliminate checking duplicate files/projects", opt => { dedupe = true; } },
                { "w|withDebug", "Perform checks with debug output", opt => { withDebug = true; } },
                { "a=|addIns=", "Addin paths to search", opt => { addins.Add(opt); } },
                { "t|terminate", "Report a non-zero exit code on violation", opt => { terminate = true; } },
                { "u|unloadProjects=", "Regular expressions to unload/ignore projects in a solution", opt => { projectUnloads.Add(opt); } },
                { "g=|generator", GetGeneratorHelp(), opt => { generator = opt; } },
                { "l|list", GetOptionalHelp(), opt => { currentOp = ListParameter; } },
                { "<>", opt =>
                    {
                        if (currentOp != null && currentOp == ListParameter)
                        {
                            string[] values = opt.Split(new char[] { ':', '=' }, 2);
                            optionals[values[0]] = values[1];
                        }
                    }
                }
            };
            
            Action<string> debugAction = null;
            try
            {
                opts.Parse(args);
                if (withDebug)
                {
                    debugAction = x => { Console.WriteLine(x); };
                    PrintVersion(debugAction);
                }

                needHelp = needHelp || (solutionFiles.Count == 0 && projectFiles.Count == 0 && directories.Count == 0 && files.Count == 0);
            }
            catch (OptionException error)
            {
                Console.WriteLine("Invalid arguments");
                Console.WriteLine(error);
                needHelp = true;
            }
            
            if (needHelp)
            {
                PrintUsageAndHelp();
                return;
            }
            
            var report = new ReportBuilder()
                .WithDedupe(dedupe)
                .WithStyleCopSettingsFile(styleCopSettings)
                .WithRecursion(recurse)
                .WithSolutionsFiles(solutionFiles)
                .WithProjectFiles(projectFiles)
                .WithDirectories(directories)
                .WithFiles(files)
                .WithIgnorePatterns(ignorePatterns)
                .WithDebug(debugAction)
                .WithAddins(addins)
                .WithProjectUnloads(projectUnloads);

            foreach (var opt in optionals)
            {
                report = report.AddOptional(opt.Key, opt.Value);
            }

            RunReport(report, generator, outputXml, quiet, violations);
            if (hadViolation && terminate)
            {
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Generate the help for the optional parameters
        /// </summary>
        /// <returns>The help for the optional parameters</returns>
        private static string GetOptionalHelp()
        {
            return "Include a set of optional parameters (key=value or key:value). Known optional parameters include: " + string.Join(", ", Enum.GetNames(typeof(Optional)));
        }

        /// <summary>
        /// Generate the help text about the generator types
        /// </summary>
        /// <returns>The help for the generator option</returns>
        private static string GetGeneratorHelp()
        {
            var output = new string[3];
            output[0] = "Specify a generator engine for the analysis.";
            foreach (var name in Enum.GetNames(typeof(Generator)))
            {
                switch (name)
                {
                    case "Xml":
                        output[2] = " Xml - A generator that only outputs the results file. Good for large projects.";
                        break;
                    case "Console":
                        output[1] = " Console - the default generator with caching and full console reporting available.";
                        break;
                }
            }

            return string.Join(Environment.NewLine, output);
        }

        /// <summary>
        /// Run the report (validates settings against the generator given)
        /// </summary>
        /// <param name="report">The report object to perform the analysis</param>
        /// <param name="generator">Generator type given</param>
        /// <param name="outputXml">Output XML file name/path</param>
        /// <param name="quiet">Indicates if any console output should be available</param>
        /// <param name="violations">True to report each violation encountered</param>
        private static void RunReport(ReportBuilder report, string generator, string outputXml, bool quiet, bool violations)
        {
            var generatorType = Generator.Default;
            if (generator != null)
            {
                generatorType = (Generator)Enum.Parse(typeof(Generator), generator, true);
            }

            switch (generatorType)
            {
                case Generator.Xml:
                    report = report.WithOutputEventHandler(
                        (x, y) => 
                            { 
                                hadViolation = true; 
                                if (!quiet)
                                {
                                    OutputGenerated(x, y);
                                }  
                            });     

                    report.Create<XmlRunner>(outputXml);

                    break;
                default:
                    EventHandler<StyleCop.ViolationEventArgs> callback = HadViolation;
                    if (!quiet)
                    {
                        if (violations)
                        {
                            callback = ViolationEncountered;
                        }
                        else
                        {
                            report = report.WithOutputEventHandler(OutputGenerated);
                        }
                    }

                    report = report.WithViolationEventHandler(callback);
                    report.Create<ConsoleRunner>(outputXml);
                    break;
            }
        }

        /// <summary>
        /// Prints the output of the StyleCop processor to the console.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The output message to print.</param>
        private static void OutputGenerated(
            object sender, 
            StyleCop.OutputEventArgs e)
        {
            Console.WriteLine(e.Output);
        }
        
        /// <summary>
        /// Prints each violation (to console) that the StyleCop processor encounters
        /// </summary>
        /// <param name='sender'>
        /// The event sender.
        /// </param>
        /// <param name='e'>
        /// The violation to print.
        /// </param>
        private static void ViolationEncountered(object sender, StyleCop.ViolationEventArgs e)
        {
            HadViolation(sender, e);
            Console.WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture, "File: {0}", e.SourceCode.Path));
            Console.WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Line: {0} -> {1}", e.LineNumber, e.Message));
            Console.WriteLine();
        }

        /// <summary>
        /// Indicates for execution if violations were encountered
        /// </summary>
        /// <param name='sender'>
        /// The event sender.
        /// </param>
        /// <param name='e'>
        /// The violation
        /// </param>
        private static void HadViolation(object sender, StyleCop.ViolationEventArgs e)
        {
            hadViolation = true;
        }

        /// <summary>
        /// Print version information out
        /// </summary>
        /// <param name='action'>Output action</param>
        private static void PrintVersion(Action<string> action)
        {
            ReportBuilder.PrintFileInformation(action, typeof(Program), typeof(OptionSet));
        }

        /// <summary>
        /// Prints this application's usage and help text to standard out.
        /// </summary>
        private static void PrintUsageAndHelp()
        {
            Console.WriteLine("StyleCopCmd");
            Console.WriteLine("Provides a command line interface for using StyleCop");
            opts.WriteOptionDescriptions(Console.Out);
        }
    }
}
