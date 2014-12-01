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
        /// Number of violations encountered
        /// </summary>
        private static volatile int violationCount;

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
            var options = GetOptions();            
            Action<string> debugAction = null;
            try
            {
                opts.Parse(args);
                if (options.WithDebug)
                {
                    debugAction = x => { Console.WriteLine(x); };
                    PrintVersion(debugAction);
                }

                options.Validate();
            }
            catch (OptionException error)
            {
                Console.WriteLine("Invalid arguments");
                Console.WriteLine(error);
                options.NeedHelp = true;
            }
            
            if (options.NeedHelp)
            {
                PrintUsageAndHelp();
                return;
            }
            
            var report = new ReportBuilder()
                .WithDedupe(options.Dedupe)
                .WithStyleCopSettingsFile(options.StyleCopSettings)
                .WithRecursion(options.Recurse)
                .WithSolutionsFiles(options.SolutionFiles)
                .WithProjectFiles(options.ProjectFiles)
                .WithDirectories(options.Directories)
                .WithFiles(options.Files)
                .WithIgnorePatterns(options.IgnorePatterns)
                .WithDebug(debugAction)
                .WithAddins(options.Addins)
                .WithProjectUnloads(options.ProjectUnloads)
                .WithProcessorSymbols(options.Symbols);

            foreach (var opt in options.Optionals)
            {
                report = report.AddOptional(opt.Key, opt.Value);
            }

            RunReport(report, options.Generator, options.OutputXml, options.Quiet, options.Violations);
            if (options.MaxViolations.HasValue && violationCount > options.MaxViolations.Value)
            {
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Get the program options
        /// </summary>
        /// <returns>Program options</returns>
        private static ProgramOptions GetOptions()
        {
            var options = new ProgramOptions();
            opts = new OptionSet()
            {
                { "s=|solutionFiles=", "Solution files to check (.sln)", opt => { options.SolutionFiles.Add(opt); } },
                { "p=|projectFiles=", "Project files to check (.csproj)", opt => { options.ProjectFiles.Add(opt); } },
                { "i=|ignoreFilePattern=", "Regular expression patterns to ignore files", opt => { options.IgnorePatterns.Add(opt); } },
                { "d=|directories=", "Directories to check for files (.cs)", opt => { options.Directories.Add(opt); } },
                { "f=|files=", "Files to check (.cs)", opt => { options.Files.Add(opt); } },
                { "r|recurse", "Recursive directory search", opt => { options.Recurse = opt != null; } },
                { "c=|styleCopSettingsFile=", "Use the given StyleCop settings file", opt => { options.StyleCopSettings = opt; } },
                { "o=|outputXmlFile=", "The file the XML output is written to", opt => { options.OutputXml = opt; } },
                { "x=|configurationSymbols=", "Configuration symbols to pass to StyleCop (ex. DEBUG, RELEASE)", opt => { options.Symbols.Add(opt); } },
                { "v|violations", "Print all violation information instead of the summary", opt => { options.Violations = true; } },
                { "q|quiet", "Do not print any information to console", opt => { options.Quiet = true; } },
                { "?|help", "Print the usage information", opt => { options.NeedHelp = true; } },
                { "e|eliminate", "Eliminate checking duplicate files/projects", opt => { options.Dedupe = true; } },
                { "w|withDebug", "Perform checks with debug output", opt => { options.WithDebug = true; } },
                { "a=|addIns=", "Addin paths to search", opt => { options.Addins.Add(opt); } },
                { "t|terminate", "Report a non-zero exit code on violation", opt => { options.MaxViolations = 0; } },
                { "u|unloadProjects=", "Regular expressions to unload/ignore projects in a solution", opt => { options.ProjectUnloads.Add(opt); } },
                { "m|maxViolations=", "Report a non-zero exit for any violations beyond this value", opt => { options.MaxViolations = TryIntParse(opt, "maxViolations"); } },
                { "g=|generator", GetGeneratorHelp(), opt => { options.Generator = opt; } },
                { "l|list", GetOptionalHelp(), opt => { options.CurrentOp = ListParameter; } },
                { "<>", opt =>
                    {
                        if (options.CurrentOp != null && options.CurrentOp == ListParameter)
                        {
                            string[] values = opt.Split(new char[] { ':', '=' }, 2);
                            options.Optionals[values[0]] = values[1];
                        }
                    }
                }
            };
            
            return options;
        }

        /// <summary>
        /// Try and parse a value. Value must be valid and greater than or equal to zero
        /// </summary>
        /// <param name="opt">Option input value</param>
        /// <param name="option">Option name</param>
        /// <returns>Parsed value</returns>
        private static int TryIntParse(string opt, string option)
        {
            if (opt != null)
            {
                int returnValue = 0;
                if (int.TryParse(opt, out returnValue))
                {
                    if (returnValue >= 0)
                    {
                        return returnValue;
                    }
                }
            }

            throw new OptionException(string.Format("The input value was invalid for {0}", option), option);
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

            FileRunner runner;
            switch (generatorType)
            {
                case Generator.Xml:
                    report = report.WithOutputEventHandler(
                        (x, y) => 
                            { 
                                violationCount++;
                                if (!quiet)
                                {
                                    OutputGenerated(x, y);
                                }  
                            });     

                    runner = new XmlRunner();
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
                    runner = new ConsoleRunner();
                    break;
            }

            runner.OutputFile = string.IsNullOrEmpty(outputXml) ? null : string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}.xml", System.IO.Path.GetFileNameWithoutExtension(outputXml));
            report.Create(runner);
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
            violationCount++;
        }

        /// <summary>
        /// Print version information out
        /// </summary>
        /// <param name='action'>Output action</param>
        private static void PrintVersion(Action<string> action)
        {
            action(string.Empty);
            action("Version Information:");
            ReportBuilder.PrintFileInformation(action, typeof(Program), typeof(OptionSet));
            action(string.Empty);
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
