//------------------------------------------------------------------------------
// <copyright 
//  file="ConsoleRunner.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace StyleCopCmd.Core
{
    using System;
    using System.Collections.Generic;
    using StyleCop;

    /// <summary>
    /// Console runner implementation
    /// </summary>
    public sealed class ConsoleRunner : RunnerBase
    {
        /// <summary>
        /// Console used to run the analysis
        /// </summary>
        private StyleCopConsole console;

        /// <inheritdoc />
        protected override void InitInstance(RunnerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this.console = new StyleCopConsole(
                this.Settings.StyleCopSettingsFile,
                true,
                options.OutputFile,
                this.Settings.AddInDirectories,
                true);
        }

        /// <inheritdoc />
        protected override void AddSource(CodeProject project, string path)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            this.console.Core.Environment.AddSourceCode(project, path, null);
        }

        /// <inheritdoc />
        protected override void Run(IList<CodeProject> projects, EventHandler<OutputEventArgs> outputGenerated, EventHandler<ViolationEventArgs> violation)
        {
            if (projects == null)
            {
                throw new ArgumentNullException("projects");
            }

            if (outputGenerated != null)
            {
                this.console.OutputGenerated += outputGenerated;
            }
            
            if (violation != null)
            {
                this.console.ViolationEncountered += violation;
            }
   
            this.console.Start(projects, !this.Settings.AllowCaching);
            if (outputGenerated != null)
            {
                this.console.OutputGenerated -= outputGenerated;
            }
            
            if (violation != null)
            {
                this.console.ViolationEncountered -= violation;
            }
        }
    }
}
